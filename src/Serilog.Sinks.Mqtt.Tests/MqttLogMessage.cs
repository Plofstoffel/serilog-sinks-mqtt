using Serilog.Events;

namespace Serilog.Sinks.Mqtt.Tests
{
    public class MqttLogMessage
    {
        public DateTime Timestamp { get; set; }
        public LogEventLevel Level { get; set; }
        public string? MessageTemplate { get; set; }
    }
}
