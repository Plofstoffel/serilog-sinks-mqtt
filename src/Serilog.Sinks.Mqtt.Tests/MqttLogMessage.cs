namespace Serilog.Sinks.Mqtt.Tests
{
    public class MqttLogMessage
    {
        public DateTime Timestamp { get; set; }
        public string? Level { get; set; }
        public string? MessageTemplate { get; set; }
    }
}
