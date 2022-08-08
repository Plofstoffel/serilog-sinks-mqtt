using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Server;
using Serilog.Debugging;
using Serilog.Sinks.Mqtt;
using System.Text;

namespace Serilog.Sinks.Mqtt.Tests
{
    [TestClass]
    public class MqttSinkUnitTests
    {
        private MqttServer _mqttServer;
        private IManagedMqttClient _mqttClient;
        private static List<string> _recievedMessages = new();

        [TestInitialize]
        public async Task InitializeTests()
        {
            try
            {
                await SetupTheServer();
                await SubscribeTestClient();
            }
            catch (Exception e)
            {
                SelfLog.WriteLine(e.Message);
                throw;
            }
        }

        private async Task SetupTheServer()
        {
            var mqttFactory = new MqttFactory();
            var mqttServerOptions = new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(1882)
                .Build();

            _mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions);
            await _mqttServer.StartAsync();

            _recievedMessages.Clear();
        }

        private async Task SubscribeTestClient()
        {
            var mqttFactory = new MqttFactory();

            _mqttClient = mqttFactory.CreateManagedMqttClient();

            var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1884)
            .WithClientId($"TestClient{Guid.NewGuid().ToString()}")
            .Build();

            var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(mqttClientOptions)
                .Build();

            _mqttClient.ApplicationMessageReceivedAsync += e =>
            {
                _recievedMessages.Add(Encoding.Default.GetString(e.ApplicationMessage.Payload));

                return Task.CompletedTask;
            };

            _mqttClient.ConnectingFailedAsync += e =>
            {
                SelfLog.WriteLine(e.Exception.Message);

                return Task.CompletedTask;
            };

            _mqttClient.DisconnectedAsync += e =>
            {
                SelfLog.WriteLine(e.ReasonString);

                return Task.CompletedTask;
            };

            _mqttClient.ConnectedAsync += e =>
            {
                SelfLog.WriteLine(e.ConnectResult.ReasonString);

                return Task.CompletedTask;
            };

            await _mqttClient.StartAsync(managedMqttClientOptions);
            await _mqttClient.SubscribeAsync("testtopic/logs", MQTTnet.Protocol.MqttQualityOfServiceLevel.AtLeastOnce);
        }

        [TestCleanup]
        public async Task DisposeServer()
        {
            await _mqttClient.StopAsync();
            _mqttClient.Dispose();
            await _mqttServer.StopAsync();
            _mqttServer.Dispose();
        }


        [TestMethod]
        public void WhenALogIsEmittedToTheSinkAllSubscribersRecieveTheMessage()
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", 1884)
            .WithClientId($"TestClient{Guid.NewGuid()}")
            .Build();

            var managedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(mqttClientOptions)
                .Build();

            var log = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.MqttSink(managedMqttClientOptions, "testtopic/logs")
            .CreateLogger();

            SpinWait.SpinUntil(() => _mqttClient.IsConnected, 5000);
            
            log.Information("Information message");

            SpinWait.SpinUntil(() => _recievedMessages.Count == 1, 15000);

            Assert.AreEqual(1, _recievedMessages.Count);
            Assert.AreEqual("Information message", _recievedMessages.First());
        }
    }
}