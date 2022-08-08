using MQTTnet.Extensions.ManagedClient;
using Serilog.Configuration;
using Serilog.Sinks.Mqtt;
using System;

namespace Serilog
{
    public static class MqttSinkExtensions
    {
        public static LoggerConfiguration MqttSink(
              this LoggerSinkConfiguration loggerConfiguration,
              ManagedMqttClientOptions options,
              string defaultTopic,
              IFormatProvider formatProvider = null)
        {
            var mqttClient = new MqttSink(formatProvider, options, defaultTopic);
            mqttClient.CreateInstanceAsync();
            return loggerConfiguration.Sink(mqttClient);
        }
    }
}
