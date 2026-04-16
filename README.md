# TCSystem — C# Utility Libraries

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=TCSystemCS&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=TCSystemCS)

A collection of reusable .NET libraries for logging, image metadata management, GPS data processing, threading utilities, and more. All libraries are published as NuGet packages and target **netstandard2.1**, **net8.0**, and **net10.0**.

## Libraries

### TCSystem.Logging

Wraps [Serilog](https://serilog.net/) behind an abstract `Logger` class where debug logging is **not compiled** into your code when building in Release mode (using `[Conditional("DEBUG")]`).

The recommended pattern is to add a `Log.cs` file in each folder/namespace so that each log output can be filtered by its namespace.

#### Init Logging

```csharp
Factory.InitLogging(Factory.LoggingOptions.Console | Factory.LoggingOptions.File,
                    "Output.txt", 1, 10 * 1024);
```

#### Log.cs example

```csharp
namespace Hello.World
{
    internal static class Log
    {
        public static Logger Instance { get; } = Factory.GetLogger(typeof(Log));
    }
}
```

#### Usage in your code

```csharp
Log.Instance.Info("Hello World");
```

### TCSystem.MetaData

Classes for handling and storing image metadata (date/time, GPS location, face/person tags). Supports JSON serialization via Newtonsoft.Json.

### TCSystem.MetaDataDB

SQLite database abstraction (via `Microsoft.Data.Sqlite`) for storing, querying, and filtering image metadata. Provides thread-safe access through an instance-pooling pattern.

### TCSystem.Gps

Reader for GPS data from Google Takeout location history files (`records.json`). Uses `System.Text.Json` with async streaming.

### TCSystem.Thread

Thread helper classes including a **worker thread** that queues commands and executes them in order. Supports async `Task`-based APIs and cancellation tokens.

### TCSystem.Util

Lightweight extension methods for containers, enumerables, and common patterns such as null-safe equality comparisons.

## Tools

| Tool | Description |
|------|-------------|
| `TCSystem.Tools.DBConverter` | Converts an existing metadata database file to a new schema |
| `TCSystem.Tools.TakeoutReader` | Reads a Google Takeout records file and imports GPS data |

Both tools are .NET 8.0 console applications and are not published as NuGet packages.

## Project Dependencies

```
TCSystem.Util
  └─► TCSystem.MetaData ─► Newtonsoft.Json
        ├─► TCSystem.Gps ─► System.Text.Json
        └─► TCSystem.MetaDataDB ─► Microsoft.Data.Sqlite
              ├─► TCSystem.Logging ─► Serilog
              └─► TCSystem.Thread
                    └─► TCSystem.Logging
```

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download) or later (net10.0 also supported)

### Build

```bash
dotnet restore
dotnet build -c Release
```

### Test

```bash
dotnet test
```

Tests use [NUnit](https://nunit.org/) with code coverage via [Coverlet](https://github.com/coverlet-coverage/coverlet).

### NuGet Packages

All library projects produce NuGet packages on build. Packages are published to [nuget.org](https://www.nuget.org/) via the `nuget_deploy` GitHub Actions workflow.

## CI / CD

| Workflow | Trigger | Purpose |
|----------|---------|---------|
| `analyze.yml` | Push to `develop`/`main` | SonarCloud analysis with code coverage |
| `dotnet.yml` | Manual | Release validation build |
| `nuget_deploy.yml` | Manual | Build, test, and publish NuGet packages |

Dependency updates are managed by [Dependabot](https://docs.github.com/en/code-security/dependabot) (weekly, targeting the `develop` branch).

## License

[MIT](LICENSE) — Copyright © 2003–2026 Thomas Gößler