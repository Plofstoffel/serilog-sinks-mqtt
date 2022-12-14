using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Extensions.ManagedClient;
using MQTTnet.Server;
using Newtonsoft.Json;
using Serilog.Debugging;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;
using Serilog.Sinks.Mqtt;
using System.Text;
using System.Text.Json.Serialization;

namespace Serilog.Sinks.Mqtt.Tests
{
    [TestClass]
    public class MqttSinkUnitTests
    {
        private MqttServer? _mqttServer;
        private IManagedMqttClient? _mqttClient;
        private static readonly List<string> _recievedMessages = new();
        private const int testPort = 1882;
        private const string testTopic = "testtopic/logs";

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
                .WithDefaultEndpointPort(testPort)
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
            .WithTcpServer("localhost", testPort)
            .WithClientId($"testclient")
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

            await _mqttClient.SubscribeAsync(testTopic, MQTTnet.Protocol.MqttQualityOfServiceLevel.ExactlyOnce);
            await _mqttClient.StartAsync(managedMqttClientOptions);
        }

        [TestCleanup]
        public async Task DisposeServer()
        {
            if (_mqttClient != null)
            {
                await _mqttClient.StopAsync();
                _mqttClient.Dispose();
            }
            if (_mqttServer != null)
            {
                await _mqttServer.StopAsync();
                _mqttServer.Dispose();
            }
        }


        [TestMethod]
        public void WhenALogIsEmittedToTheSinkAllSubscribersRecieveTheMessage()
        {
            var mqttClientOptions = new MqttClientOptionsBuilder()
            .WithTcpServer("localhost", testPort)
            .WithClientId($"logclient")
            .Build();

            MqttSinkOptions mqttSinkOptions = new()
            {
                DefaultTopic = testTopic,
                ManagedMqttClientOptions = new ManagedMqttClientOptionsBuilder()
                .WithClientOptions(mqttClientOptions)
                .Build(),
                MqttQualityOfServiceLevel = MQTTnet.Protocol.MqttQualityOfServiceLevel.AtMostOnce,
                RetainMessages = false
            };

            using var log = new LoggerConfiguration()
            .MinimumLevel.Information()
            .WriteTo.MqttSink(mqttSinkOptions)
            .CreateLogger();

            SpinWait.SpinUntil(() => _mqttClient?.IsConnected == true, 5000);

            Thread.Sleep(500);

            log.Information("Information message");

            SpinWait.SpinUntil(() => _recievedMessages.Count == 1, 30000);

            Assert.AreEqual(1, _recievedMessages.Count);
            var lastMessage = JsonConvert.DeserializeObject<MqttLogMessage>(_recievedMessages.First());
            Assert.AreEqual(LogEventLevel.Information, lastMessage?.Level);
            Assert.AreEqual("Information message", lastMessage?.MessageTemplate);

        }
    }
}