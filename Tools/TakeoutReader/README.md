# TCSystem.Tools.TakeoutReader

Console application that reads a Google Takeout `records.json` file and imports missing GPS coordinates into the metadata database.

## Usage

```bash
dotnet run --project Tools/TakeoutReader/TCSystem.Tools.TakeoutReader.csproj -- <records.json> <database>
```

The tool opens the database read/write, leaves existing metadata locations unchanged, and fills only files whose timestamp is within one hour of the nearest phone location record. Back up the database before running bulk imports.

## Dependencies

- TCSystem.Gps
- TCSystem.Logging
- TCSystem.MetaDataDB

## Target

- net8.0
