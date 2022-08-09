using MQTTnet;
using MQTTnet.Extensions.ManagedClient;
using Serilog.Core;
using Serilog.Events;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Serilog.Sinks.Mqtt
{
    public class MqttSink : ILogEventSink, IDisposable
    {
        private readonly IManagedMqttClient _mqttClient;
        private readonly MqttSinkOptions _mqttSinkOptions;
        private bool disposed = false;

        public bool Initiated { get; protected set; }

        public MqttSink(MqttSinkOptions mqttSinkOptions)
        {
            _mqttClient = new MqttFactory().CreateManagedMqttClient();
            _mqttSinkOptions = mqttSinkOptions;
        }

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

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _mqttClient.Dispose();
                }
                disposed = true;
            }
        }

        ~MqttSink()
        {
            Dispose(disposing: false);
        }
    }
}
