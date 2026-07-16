# TCSystem.MetaDataDB

[![NuGet](https://img.shields.io/nuget/v/TCSystem.MetaDataDB.svg)](https://www.nuget.org/packages/TCSystem.MetaDataDB/)

SQLite persistence for `TCSystem.MetaData` images, locations, tags, people, faces, and processing state. Database
operations acquire pooled SQLite instances internally to serialize access safely.

## Installation

```bash
dotnet add package TCSystem.MetaDataDB
```

## Features

- Read-only, write-only, and read/write interfaces
- SQLite schema creation and version conversion
- Metadata, file, location, tag, person, and face queries
- Thread-safe internal connection-instance pooling
- Add/change/remove notifications

## Lifecycle

Create database interfaces through `TCSystem.MetaDataDB.Factory` and always release them with the matching `Destroy`
overload:

```csharp
IDB2 db = null;
try
{
    db = Factory.CreateReadWrite(databasePath);
    Image image = db.GetMetaData(fileName);
}
finally
{
    Factory.Destroy(ref db);
}
```

`CreateReadWrite` creates a database when the path does not exist. `CreateRead` opens an existing database in read-only
mode.

## Dependencies

- Microsoft.Data.Sqlite
- SQLitePCLRaw.lib.e_sqlite3 (explicit native SQLite dependency)
- TCSystem.Logging
- TCSystem.MetaData
- TCSystem.Thread

## Targets

- netstandard2.1
- net8.0
- net10.0

## Development

See the repository [build instructions](../README.md#build-from-source) and
the [TCSystem.MetaDataDB.Tests instructions](Tests/README.md). Tests use temporary SQLite files; optional legacy
converter fixtures are described in the test README.
