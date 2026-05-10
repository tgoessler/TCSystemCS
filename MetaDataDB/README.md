# TCSystem.MetaDataDB

SQLite database abstraction for storing, querying, and filtering image metadata. Provides thread-safe access through an instance-pooling pattern.

## Features

- SQLite-based persistent storage for image metadata
- Thread-safe database access via instance pooling (`InstanceAcquire` / `using`)
- Query and filter support for metadata collections

## Dependencies

- Microsoft.Data.Sqlite
- TCSystem.Logging
- TCSystem.MetaData
- TCSystem.Thread

## Targets

- netstandard2.1
- net8.0
- net10.0
