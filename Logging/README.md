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

## Repository Development

Repository-wide prerequisites, target frameworks, dependencies, build, test, coverage, and API-documentation instructions
are maintained in the [main README](../README.md).
