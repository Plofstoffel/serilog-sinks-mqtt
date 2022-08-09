using MQTTnet.Extensions.ManagedClient;
using Serilog.Configuration;
using Serilog.Sinks.Mqtt;
using System;
using System.Threading.Tasks;

namespace Serilog
{
    public static class MqttSinkExtensions
    {
        public static LoggerConfiguration MqttSink(
              this LoggerSinkConfiguration loggerConfiguration,
              MqttSinkOptions mqttSinkOptions)
        {
            var mqttClient = new MqttSink(mqttSinkOptions);
            var task = Task.Run(async () => await mqttClient.CreateInstanceAsync());
            task.Wait();
            
            return loggerConfiguration.Sink(mqttClient);
        }
    }
}
