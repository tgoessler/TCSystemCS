# TCSystem.Util.Tests

NUnit tests for `TCSystem.Util`, targeting `net8.0` and `net10.0`.

## Prerequisites

See the repository [build and test prerequisites](../../README.md#prerequisites). Run the commands below from the repository root.

## Run Tests

Run both target frameworks, restoring and building as needed:

```bash
dotnet test Util/Tests/TCSystem.Util.Tests.csproj --configuration Release
```

Run only one framework:

```bash
dotnet test Util/Tests/TCSystem.Util.Tests.csproj --configuration Release --framework net10.0
```

After the complete solution has already been built in Release, add `--no-build --no-restore` for a faster repeat run.

## Coverage

```bash
dotnet test Util/Tests/TCSystem.Util.Tests.csproj --configuration Release --framework net8.0 -p:CollectCoverage=true -p:CoverletOutputFormat=opencover
```

The generated `Util/Tests/coverage.net8.0.opencover.xml` file is ignored by Git.

## Test Stack

- NUnit 4
- NUnit3TestAdapter
- Microsoft.NET.Test.Sdk
- Coverlet MSBuild
