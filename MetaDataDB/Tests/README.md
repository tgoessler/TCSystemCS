# TCSystem.MetaDataDB.Tests

NUnit tests for `TCSystem.MetaDataDB`, targeting `net8.0` and `net10.0`. Regular tests create isolated SQLite databases in the operating system's temporary directory and clean them up after each test.

## Prerequisites

See the repository [build and test prerequisites](../../README.md#prerequisites). Run the commands below from the repository root. No external SQLite installation or database server is required.

## Run Tests

Run both target frameworks, restoring and building as needed:

```bash
dotnet test MetaDataDB/Tests/TCSystem.MetaDataDB.Tests.csproj --configuration Release
```

Run only one framework:

```bash
dotnet test MetaDataDB/Tests/TCSystem.MetaDataDB.Tests.csproj --configuration Release --framework net10.0
```

After the complete solution has already been built in Release, add `--no-build --no-restore` for a faster repeat run.

## Optional Converter Fixtures

`ConverterTests` looks for these legacy fixtures in `MetaDataDB/Tests/TestData/`:

- `MetaData2-v11.db`
- `MetaData2-v12.db`

Those database files are not stored in this repository. Converter cases are reported as skipped (`DB file not available`) when the fixtures are absent; this does not indicate a test failure.

## Coverage

```bash
dotnet test MetaDataDB/Tests/TCSystem.MetaDataDB.Tests.csproj --configuration Release --framework net8.0 -p:CollectCoverage=true -p:CoverletOutputFormat=opencover
```

The generated `MetaDataDB/Tests/coverage.net8.0.opencover.xml` file is ignored by Git.

## Test Stack

- NUnit 4
- NUnit3TestAdapter
- Microsoft.NET.Test.Sdk
- Coverlet MSBuild
