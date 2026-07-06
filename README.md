# TCSystem — C# Utility Libraries

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=TCSystemCS&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=TCSystemCS)

A collection of reusable .NET libraries for logging, image metadata management, GPS data processing, threading utilities, and more. All libraries are published as NuGet packages and target **netstandard2.1**, **net8.0**, and **net10.0**.

## Libraries

| Package | NuGet |
|---------|-------|
| [TCSystem.Util](Util/README.md) | [![NuGet](https://img.shields.io/nuget/v/TCSystem.Util.svg)](https://www.nuget.org/packages/TCSystem.Util/) |
| [TCSystem.Logging](Logging/README.md) | [![NuGet](https://img.shields.io/nuget/v/TCSystem.Logging.svg)](https://www.nuget.org/packages/TCSystem.Logging/) |
| [TCSystem.MetaData](MetaData/README.md) | [![NuGet](https://img.shields.io/nuget/v/TCSystem.MetaData.svg)](https://www.nuget.org/packages/TCSystem.MetaData/) |
| [TCSystem.MetaDataDB](MetaDataDB/README.md) | [![NuGet](https://img.shields.io/nuget/v/TCSystem.MetaDataDB.svg)](https://www.nuget.org/packages/TCSystem.MetaDataDB/) |
| [TCSystem.Gps](Gps/README.md) | [![NuGet](https://img.shields.io/nuget/v/TCSystem.Gps.svg)](https://www.nuget.org/packages/TCSystem.Gps/) |
| [TCSystem.Thread](Thread/README.md) | [![NuGet](https://img.shields.io/nuget/v/TCSystem.Thread.svg)](https://www.nuget.org/packages/TCSystem.Thread/) |


## Tools

### [TCSystem.Tools.DBConverter](Tools/DBConverter/README.md)

### [TCSystem.Tools.TakeoutReader](Tools/TakeoutReader/README.md)


## Project Dependencies

```
TCSystem.Util
  └─► TCSystem.MetaData ─► Newtonsoft.Json
        ├─► TCSystem.Gps ─► System.Text.Json
        └─► TCSystem.MetaDataDB ─► Microsoft.Data.Sqlite / SQLitePCLRaw.lib.e_sqlite3
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

### Security Audit

```bash
dotnet list TCSystem.slnx package --vulnerable --include-transitive
```

### Coding Style

See [CodingStyle.md](CodingStyle.md) for the C# style guide derived from ReSharper settings and existing code.

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