using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using Serilog.Core;
using Serilog.Events;
using System.IO;
using System.Threading.Tasks;

namespace Serilog.Sinks.Mqtt
{
    public class MqttSink : ILogEventSink
    {
        private readonly IManagedMqttClient _mqttClient;
        private readonly MqttSinkOptions _mqttSinkOptions;

        public bool Initiated { get; protected set; }

        public MqttSink(MqttSinkOptions mqttSinkOptions)
        {
            _mqttClient = new MqttFactory().CreateManagedMqttClient();
            _mqttSinkOptions = mqttSinkOptions;
        }

        public Task Initialize { get; }

        public async Task CreateInstanceAsync()
        {
            if (!Initiated)
            {
                await _mqttClient.StartAsync(_mqttSinkOptions.ManagedMqttClientOptions);
                Initiated = true;
            }
        }

        public void Emit(LogEvent logEvent)
        {
            using var render = new StringWriter();
            _mqttSinkOptions.TextFormatter.Format(logEvent, render);
            _mqttClient.EnqueueAsync(_mqttSinkOptions.DefaultTopic, render.ToString());

        }

        ~MqttSink()
        {
            _mqttClient.Dispose();
        }
    }
}
