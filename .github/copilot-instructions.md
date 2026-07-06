# Copilot Instructions for TCSystemCS

## Project Overview

TCSystemCS is a collection of reusable .NET libraries written in **C# 14** targeting **netstandard2.1**, **net8.0**, and **net10.0**. The solution is organized into library projects, console tool projects, and test projects.

## Solution Structure

```
/
├── Gps/              → TCSystem.Gps (Google Takeout GPS reader)
│   └── Tests/        → TCSystem.Gps.Tests (NUnit)
├── Logging/          → TCSystem.Logging (Serilog wrapper with conditional debug)
├── MetaData/         → TCSystem.MetaData (image metadata classes)
│   └── Tests/        → TCSystem.MetaData.Tests (NUnit)
├── MetaDataDB/       → TCSystem.MetaDataDB (SQLite metadata storage)
│   └── Tests/        → TCSystem.MetaDataDB.Tests (NUnit)
├── Thread/           → TCSystem.Thread (worker thread, async helpers)
│   └── Tests/        → TCSystem.Thread.Tests (NUnit)
├── Util/             → TCSystem.Util (extension methods, utilities)
│   └── Tests/        → TCSystem.Util.Tests (NUnit)
└── Tools/
    ├── DBConverter/  → Console app – database schema converter
    └── TakeoutReader/→ Console app – Google Takeout importer
```

## Build & Test

```bash
dotnet restore
dotnet build -c Release
dotnet test
```

- Tests use **NUnit 4.x** with **Coverlet** for code coverage.
- CI runs on **windows-latest** using .NET 10 SDK.

## Coding Conventions

See [`CodingStyle.md`](../CodingStyle.md) for the authoritative C# style guide.

## NuGet Packaging

- Library projects import `Packaging.props` and produce NuGet packages on build.
- Tool/test projects import `NoPackaging.props` to disable packaging.
- Symbol packages (`.snupkg`) are generated for all library packages.
- Version is defined in `Version.props` (currently **5.0.0**); CI builds append a `-ci.{timestamp}` suffix.

## Dependency Guidelines

- Keep external dependencies minimal.
- Current key dependencies: **Serilog** (logging), **Newtonsoft.Json** (metadata serialization), **System.Text.Json** (GPS), **Microsoft.Data.Sqlite** (database).
- Dependabot runs weekly against the `develop` branch for both NuGet and GitHub Actions updates.

## Branching

- Main development happens on the `develop` branch.
- Dependabot targets `develop` for pull requests.
- The `main` branch receives merges for releases.
