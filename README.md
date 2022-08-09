# serilog-sinks-mqtt
A Serilog sink that writes events to [MQTT](https://mqtt.org/)

[![.NET](https://github.com/Plofstoffel/serilog-sinks-mqtt/actions/workflows/dotnet.yml/badge.svg?branch=main)](https://github.com/Plofstoffel/serilog-sinks-mqtt/actions/workflows/dotnet.yml)
[![Coverage Status](https://coveralls.io/repos/github/Plofstoffel/serilog-sinks-mqtt/badge.svg?branch=main)](https://coveralls.io/github/Plofstoffel/serilog-sinks-mqtt?branch=main)
![GitHub](https://img.shields.io/github/license/Plofstoffel/serilog-sinks-mqtt)

## Features

### General

* Sinks log messages to MQTT broker using [MQTTnet](https://www.nuget.org/packages/MQTTnet)'s [ManagedClient](https://www.nuget.org/packages/MQTTnet.Extensions.ManagedClient/)