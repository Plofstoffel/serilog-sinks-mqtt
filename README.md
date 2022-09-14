# serilog-sinks-mqtt
A Serilog sink that writes events to [MQTT](https://mqtt.org/)

[![.NET](https://github.com/Plofstoffel/serilog-sinks-mqtt/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/Plofstoffel/serilog-sinks-mqtt/actions/workflows/dotnet.yml)
[![Coverage Status](https://coveralls.io/repos/github/Plofstoffel/serilog-sinks-mqtt/badge.svg?branch=main)](https://coveralls.io/github/Plofstoffel/serilog-sinks-mqtt?branch=main)
![GitHub](https://img.shields.io/github/license/Plofstoffel/serilog-sinks-mqtt)
[![NuGet Version](https://img.shields.io/nuget/v/Serilog.Sinks.Mqtt)](https://www.nuget.org/packages/Serilog.Sinks.Mqtt)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Serilog.Sinks.Mqtt)](https://www.nuget.org/packages/Serilog.Sinks.Mqtt)


## General

* Sinks log messages to MQTT broker using [MQTTnet](https://www.nuget.org/packages/MQTTnet)'s [ManagedClient](https://www.nuget.org/packages/MQTTnet.Extensions.ManagedClient/)

## How to use the Sink

~~~C#
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

    var log = new LoggerConfiguration()
    .WriteTo.MqttSink(mqttSinkOptions)
    .CreateLogger();

    log.Information("Information message");
~~~
