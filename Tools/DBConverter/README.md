# TCSystem.Tools.DBConverter

A `net10.0` console application that copies an existing metadata database through the current schema converter and
validates the converted data.

## Prerequisites

Install the .NET 10 SDK and build from the repository root. See the
main [build instructions](../../README.md#build-from-source).

## Build

```bash
dotnet build Tools/DBConverter/TCSystem.Tools.DBConverter.csproj --configuration Release
```

## Usage

```bash
dotnet run --project Tools/DBConverter/TCSystem.Tools.DBConverter.csproj --configuration Release -- <source-db> <target-db>
```

Quote paths containing spaces, for example:

```bash
dotnet run --project Tools/DBConverter/TCSystem.Tools.DBConverter.csproj --configuration Release -- "D:\Data\metadata-old.db" "D:\Data\metadata-new.db"
```

If the Release build already exists, add `--no-build` before `--`.

## Data Safety

- The source database is copied to a temporary file and is not converted in place.
- The target path is deleted before conversion and then recreated.
- Source data is compared with the converted database after conversion.
- Back up important databases and do not use an existing file as the target unless overwriting it is intended.

## Dependencies

- TCSystem.MetaDataDB
