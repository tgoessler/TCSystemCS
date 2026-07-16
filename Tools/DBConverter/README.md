# TCSystem.Tools.DBConverter

Console application that copies an existing metadata database through the current schema converter and validates the
converted data.

## Usage

```bash
dotnet run --project Tools/DBConverter/TCSystem.Tools.DBConverter.csproj --configuration Release -- <source-db> <target-db>
```

Quote paths containing spaces, for example:

```bash
dotnet run --project Tools/DBConverter/TCSystem.Tools.DBConverter.csproj --configuration Release -- "D:\Data\metadata-old.db" "D:\Data\metadata-new.db"
```

## Data Safety

- The source database is copied to a temporary file and is not converted in place.
- The target path is deleted before conversion and then recreated.
- Source data is compared with the converted database after conversion.
- Back up important databases and do not use an existing file as the target unless overwriting it is intended.

## Repository Development

Repository-wide prerequisites, targets, dependencies, and build instructions are maintained in the
[main README](../../README.md).
