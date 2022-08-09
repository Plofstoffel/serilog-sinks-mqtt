﻿using MQTTnet.Extensions.ManagedClient;
using Serilog.Formatting;
using Serilog.Formatting.Json;

namespace Serilog.Sinks.Mqtt
{
    public class MqttSinkOptions
    {
        public ManagedMqttClientOptions ManagedMqttClientOptions { get; set; }
        public string DefaultTopic { get; set; }
        public ITextFormatter TextFormatter { get; set; } = new JsonFormatter();
    }
}
