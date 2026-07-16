# TCSystem — C# Utility Libraries

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=TCSystemCS&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=TCSystemCS)

[API documentation](https://tgoessler.github.io/TCSystemCS/)

TCSystemCS is a collection of reusable .NET libraries for logging, image metadata, GPS data, SQLite metadata storage,
threading, and general utilities. The repository also contains two command-line tools and NUnit test projects.

## Projects

### Libraries

| Package                                     | Purpose                                                   | NuGet                                                                                                                   |
|---------------------------------------------|-----------------------------------------------------------|-------------------------------------------------------------------------------------------------------------------------|
| [TCSystem.Util](Util/README.md)             | General extension methods and equality/math helpers       | [![NuGet](https://img.shields.io/nuget/v/TCSystem.Util.svg)](https://www.nuget.org/packages/TCSystem.Util/)             |
| [TCSystem.Logging](Logging/README.md)       | Serilog-based logging facade                              | [![NuGet](https://img.shields.io/nuget/v/TCSystem.Logging.svg)](https://www.nuget.org/packages/TCSystem.Logging/)       |
| [TCSystem.MetaData](MetaData/README.md)     | Immutable image metadata models and JSON serialization    | [![NuGet](https://img.shields.io/nuget/v/TCSystem.MetaData.svg)](https://www.nuget.org/packages/TCSystem.MetaData/)     |
| [TCSystem.MetaDataDB](MetaDataDB/README.md) | SQLite persistence for image metadata                     | [![NuGet](https://img.shields.io/nuget/v/TCSystem.MetaDataDB.svg)](https://www.nuget.org/packages/TCSystem.MetaDataDB/) |
| [TCSystem.Gps](Gps/README.md)               | Google Takeout location-history reader and lookup helpers | [![NuGet](https://img.shields.io/nuget/v/TCSystem.Gps.svg)](https://www.nuget.org/packages/TCSystem.Gps/)               |
| [TCSystem.Thread](Thread/README.md)         | Worker-thread, semaphore, and async update helpers        | [![NuGet](https://img.shields.io/nuget/v/TCSystem.Thread.svg)](https://www.nuget.org/packages/TCSystem.Thread/)         |

Library projects target `netstandard2.1`, `net8.0`, and `net10.0`.

### Tools

| Project                                                       | Purpose                                            | Target    |
|---------------------------------------------------------------|----------------------------------------------------|-----------|
| [TCSystem.Tools.DBConverter](Tools/DBConverter/README.md)     | Convert and validate a metadata database           | `net10.0` |
| [TCSystem.Tools.TakeoutReader](Tools/TakeoutReader/README.md) | Import missing GPS coordinates from Google Takeout | `net10.0` |

### Unit Tests

| Test project                                            | Project under test  |
|---------------------------------------------------------|---------------------|
| [TCSystem.Gps.Tests](Gps/Tests/README.md)               | TCSystem.Gps        |
| [TCSystem.MetaData.Tests](MetaData/Tests/README.md)     | TCSystem.MetaData   |
| [TCSystem.MetaDataDB.Tests](MetaDataDB/Tests/README.md) | TCSystem.MetaDataDB |
| [TCSystem.Thread.Tests](Thread/Tests/README.md)         | TCSystem.Thread     |
| [TCSystem.Util.Tests](Util/Tests/README.md)             | TCSystem.Util       |

All test projects target both `net8.0` and `net10.0`. There is currently no dedicated `TCSystem.Logging.Tests` project.

## Dependency Overview

- `TCSystem.Util` has no external dependencies.
- `TCSystem.Logging` wraps Serilog and its configured sinks/enrichers.
- `TCSystem.MetaData` depends on `TCSystem.Util` and Newtonsoft.Json.
- `TCSystem.Thread` depends on `TCSystem.Logging`.
- `TCSystem.Gps` depends on `TCSystem.MetaData` and System.Text.Json.
- `TCSystem.MetaDataDB` depends on `TCSystem.Logging`, `TCSystem.MetaData`, `TCSystem.Thread`, Microsoft.Data.Sqlite,
  and the native SQLite bundle.
- The tools compose these libraries through project references.

## Build from Source

All commands below are intended to run from the repository root.

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0) for C# 14, `net10.0`, and `.slnx` support.
- .NET 8 runtime as well as the .NET 10 runtime to execute the complete two-framework unit-test matrix.
- No additional workloads or external database server are required. SQLite is supplied through NuGet.

The repository does not pin an SDK with `global.json`. Confirm the selected SDK and installed runtimes with:

```bash
dotnet --version
dotnet --list-runtimes
```

### Restore and Build the Complete Solution

```bash
dotnet restore TCSystem.slnx
dotnet build TCSystem.slnx --configuration Release --no-restore
```

The build compiles libraries for all three target frameworks, tools for `net10.0`, and tests for `net8.0` and `net10.0`.
Warnings are treated as errors. Compiled output is written below each project's `bin/Release` directory.

For an ordinary development build, omit `--configuration Release` (the default configuration is Debug):

```bash
dotnet build TCSystem.slnx
```

## Build API Documentation

[DocFX](https://dotnet.github.io/docfx/) generates a searchable static site from the library projects and their XML
comments:

```bash
dotnet tool restore
dotnet restore TCSystem.slnx
dotnet docfx docs/docfx.json --warningsAsErrors
```

The generated site is written to the ignored `docs/_site` directory. To build and preview it locally, use:

```bash
dotnet docfx docs/docfx.json --serve
```

The `docs.yml` workflow builds and deploys this site when changes reach `main`, and it can also be run manually. In the
GitHub repository settings, select **GitHub Actions** as the Pages source. The published project site is
`https://tgoessler.github.io/TCSystemCS/`.

## Run Unit Tests

### Complete Test Matrix

After the Release build above:

```bash
dotnet test TCSystem.slnx --configuration Release --no-build --no-restore
```

This runs every test project for both `net8.0` and `net10.0`. To let `dotnet test` restore and build automatically, use
the shorter command:

```bash
dotnet test TCSystem.slnx --configuration Release
```

### One Target Framework

Use this when validating one runtime or when the .NET 8 runtime is not installed:

```bash
dotnet test TCSystem.slnx --configuration Release --framework net10.0
```

The SonarCloud analysis workflow uses the equivalent `net8.0` test run.

### One Test Project

```bash
dotnet test Gps/Tests/TCSystem.Gps.Tests.csproj --configuration Release
dotnet test MetaData/Tests/TCSystem.MetaData.Tests.csproj --configuration Release
dotnet test MetaDataDB/Tests/TCSystem.MetaDataDB.Tests.csproj --configuration Release
dotnet test Thread/Tests/TCSystem.Thread.Tests.csproj --configuration Release
dotnet test Util/Tests/TCSystem.Util.Tests.csproj --configuration Release
```

See the linked test-project READMEs for single-framework and coverage examples. Some `MetaDataDB` converter tests are
reported as skipped when optional legacy database fixtures are not present; the regular database tests create temporary
SQLite databases and require no setup.

### Code Coverage

Coverlet MSBuild is referenced by every test project. To reproduce the CI coverage format for `net8.0`, first build
Release and then run:

```bash
dotnet test TCSystem.slnx --configuration Release --no-build --no-restore --framework net8.0 -p:CollectCoverage=true -p:CoverletOutputFormat=opencover
```

The generated, ignored reports are named `coverage.net8.0.opencover.xml` in the test project directories.

## Run the Tools

Each tool README documents its arguments and data-safety considerations:

- [Run DBConverter](Tools/DBConverter/README.md#usage)
- [Run TakeoutReader](Tools/TakeoutReader/README.md#usage)

## Maintenance

### Dependency Vulnerability Audit

After restore, with the .NET 10 SDK:

```bash
dotnet package list --project TCSystem.slnx --vulnerable --include-transitive --no-restore
```

See [SECURITY.md](SECURITY.md) for the security policy.

### Clean Generated Build Output

```bash
dotnet clean TCSystem.slnx --configuration Release
```

If a local SonarScanner run was interrupted and later builds reference missing analyzer files, delete the ignored
`.sonarqube` directory before rebuilding.

### Coding and Contribution Guidance

- [CodingStyle.md](CodingStyle.md) defines the C# style and analyzer expectations.
- [AGENTS.md](AGENTS.md) defines repository instructions for coding agents.
- [`.github/copilot-instructions.md`](.github/copilot-instructions.md) summarizes project patterns.

## NuGet Packaging

Library projects import `Packaging.props` for NuGet metadata, XML API documentation, symbol-package settings, and
publish targets. Generated XML files are included beside the assemblies so IDEs can show documentation for public types
and members. Test and tool projects import `NoPackaging.props` so ordinary builds do not generate or publish packages.
Package versioning is defined in `Version.props`; `IsCiBuild` defaults to `true` and adds a timestamped `-ci.*` suffix.
The manual `nuget_deploy.yml` workflow explicitly invokes the `Pack` and `NugetPush` targets and can select a stable or
CI version. Do not invoke `NugetPush` for an ordinary local build.

## CI/CD

| Workflow           | Trigger                             | Purpose                                                                      |
|--------------------|-------------------------------------|------------------------------------------------------------------------------|
| `analyze.yml`      | Push to `develop` or `main`; manual | Release build, `net8.0` tests with OpenCover output, and SonarCloud analysis |
| `docs.yml`         | Push to `main`; manual              | Build the DocFX site and deploy it to GitHub Pages                           |
| `dotnet.yml`       | Manual                              | Release restore and build validation                                         |
| `nuget_deploy.yml` | Manual                              | Release build, full test matrix, package, and NuGet publish                  |

CI runs on `windows-latest` with the .NET 10 SDK. Dependabot checks NuGet packages and GitHub Actions weekly and targets
`develop`.

## License

[MIT](LICENSE) — Copyright © 2003–2026 Thomas Gößler
