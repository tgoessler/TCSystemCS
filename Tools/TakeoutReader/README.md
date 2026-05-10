# TCSystem.Tools.TakeoutReader

Console application that reads a Google Takeout `records.json` file and imports GPS data into the metadata database.

## Usage

```bash
dotnet run --project Tools/TakeoutReader/TCSystem.Tools.TakeoutReader.csproj -- <records.json> <database>
```

## Dependencies

- TCSystem.Gps
- TCSystem.Logging
- TCSystem.MetaDataDB

## Target

- net8.0
