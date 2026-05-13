# Copilot Instructions for TCSystemCS

## Project Overview

TCSystemCS is a collection of reusable .NET libraries written in **C# 14** targeting **netstandard2.1**, **net8.0**, and **net10.0**. The solution is organized into library projects, console tool projects, and test projects.

## Solution Structure

```
/
├── Gps/              → TCSystem.Gps (Google Takeout GPS reader)
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

### Language & Compiler Settings

- **C# 14** (`LangVersion 14`) — use the latest language features available.
- **All warnings are errors** (`TreatWarningsAsErrors`). Code must compile warning-free.
- **.NET analyzers** are enabled at the `latest` analysis level with code-style enforcement at build time (`EnforceCodeStyleInBuild`).
- Analyzer rule **CA2000** (dispose objects before losing scope) is treated as an error.

### File Layout

- Every source file starts with the standard TCSystem copyright header (ASCII art logo, GitHub URL, copyright 2003–2026 Thomas Goessler, legal notice).
- Use **file-scoped namespaces** (`namespace TCSystem.Feature;`).
- Group `using` directives inside a `#region Usings` block, ordered: System → third-party → project references.
- Organize members into `#region Public` and `#region Private` blocks.

### Design Patterns

- **Immutable data models** — domain types (e.g., `Image`, `PersonTag`) use primary constructors and return new instances from static factory methods instead of mutating state.
- **Abstract Logger with conditional compilation** — debug-level logging uses `[Conditional("DEBUG")]` so calls are eliminated from Release builds.
- **Namespace-scoped logging** — each namespace has its own `Log.cs` with a static `Logger` instance obtained from `Factory.GetLogger(typeof(Log))`.
- **Instance pooling / using pattern** — database access (e.g., `DB2`) uses `InstanceAcquire` with `using` statements for thread-safe resource management.
- **Sealed classes** where polymorphism is not required.

### Performance

- Use `[MethodImpl(MethodImplOptions.AggressiveInlining)]` for small, hot utility methods.
- Prefer `ValueTask` over `Task` for async methods that often complete synchronously.
- Use `ReferenceEquals` checks before expensive equality comparisons.

### Testing

- Test framework: **NUnit 4.x** with `NUnit3TestAdapter`.
- Test projects live in `{Project}/Tests/` directories and use `NoPackaging.props`.
- Code coverage is collected in **opencover** format and reported to SonarCloud.

### NuGet Packaging

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
