# TCSystem.Logging

[![NuGet](https://img.shields.io/nuget/v/TCSystem.Logging.svg)](https://www.nuget.org/packages/TCSystem.Logging/)

A logging facade over [Serilog](https://serilog.net/). Calls to the facade's debug methods are decorated with
`[Conditional("DEBUG")]`, so those calls are omitted by the compiler when the caller is built without `DEBUG`.

## Installation

```bash
dotnet add package TCSystem.Logging
```

## Features

- Abstract `Logger` facade
- Conditional debug logging
- Console, rolling file, and debugger sinks
- Asynchronous file sink
- Thread ID and log-context enrichment
- Optional Serilog configuration callback

## Usage

Initialize logging once at application startup:

```csharp
using TCSystem.Logging;

Factory.InitLogging(Factory.LoggingOptions.Console | Factory.LoggingOptions.File,
                    "Output.txt", 1, 10 * 1024);
```

Create one namespace-scoped `Log.cs` facade where required:

```csharp
namespace Hello.World;

internal static class Log
{
    public static Logger Instance { get; } = Factory.GetLogger(typeof(Log));
}
```

Log through that facade and balance initialization during application shutdown:

```csharp
Log.Instance.Info("Hello World");
Log.Instance.Debug("This call is omitted when the caller is built without DEBUG");
Factory.DeInitLogging();
```

Repeated initialization is reference-counted; each successful initialization must have a matching `DeInitLogging()`
call.

## Dependencies

- Serilog
- Serilog.Enrichers.Thread
- Serilog.Sinks.Async
- Serilog.Sinks.Console
- Serilog.Sinks.Debug
- Serilog.Sinks.File

## Targets

- netstandard2.1
- net8.0
- net10.0

## Development

See the repository [build and test instructions](../README.md#build-from-source). There is currently no dedicated
Logging test project; the complete solution tests exercise logging through dependent projects.
