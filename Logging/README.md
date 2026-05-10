# TCSystem.Logging

Wraps [Serilog](https://serilog.net/) behind an abstract `Logger` class where debug logging is **not compiled** into your code when building in Release mode (using `[Conditional("DEBUG")]`).

## Features

- Abstract `Logger` class with conditional debug logging
- Namespace-scoped logging via static `Log.cs` per namespace
- Console, file, and debug output sinks
- Async sink support for non-blocking log writes
- Thread enrichment for log context

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

## Usage

### Initialize Logging

```csharp
Factory.InitLogging(Factory.LoggingOptions.Console | Factory.LoggingOptions.File,
                    "Output.txt", 1, 10 * 1024);
```

### Create a Log.cs in each namespace

```csharp
namespace Hello.World;

internal static class Log
{
    public static Logger Instance { get; } = Factory.GetLogger(typeof(Log));
}
```

### Log messages

```csharp
Log.Instance.Info("Hello World");
Log.Instance.Debug("This is only compiled in Debug builds");
```
