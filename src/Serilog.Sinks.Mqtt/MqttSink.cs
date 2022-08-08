using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using Serilog.Core;
using Serilog.Events;
using System;
using System.Threading.Tasks;

namespace Serilog.Sinks.Mqtt
{
    public class MqttSink: ILogEventSink
    {
        private const string _emptyMessage = "";
        private readonly IFormatProvider _formatProvider;
        private readonly IManagedMqttClient _mqttClient;
        private readonly ManagedMqttClientOptions _options;
        private readonly string _topic;

        public bool Initiated { get; protected set; }

        public MqttSink(IFormatProvider formatProvider, ManagedMqttClientOptions options, string topic)
        {
            _formatProvider = formatProvider;
            _mqttClient = new MqttFactory().CreateManagedMqttClient();
            _options = options;
            _topic = topic;
        }

        public Task Initialize { get; }

        public async Task CreateInstanceAsync()
        {
            if (!Initiated)
            {
                await _mqttClient.StartAsync(_options);
                await _mqttClient.EnqueueAsync(_topic, _emptyMessage);
                Initiated = true;
            }
        }

        public void Emit(LogEvent logEvent)
        {
            var message = logEvent.RenderMessage(_formatProvider);
            _mqttClient.EnqueueAsync(_topic, message);
        }

        ~MqttSink()
        {
            _mqttClient.Dispose();
        }
    }
}
