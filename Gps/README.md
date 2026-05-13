# TCSystem.Gps

[![NuGet](https://img.shields.io/nuget/v/TCSystem.Gps.svg)](https://www.nuget.org/packages/TCSystem.Gps/)

Reader for GPS data from Google Takeout location history files (`records.json`). Uses `System.Text.Json` with async streaming for efficient processing of large files.

## Installation

```bash
dotnet add package TCSystem.Gps
```

## Features

- Async streaming reader for Google Takeout `records.json`
- Efficient memory usage via `System.Text.Json` streaming APIs
- Integration with `TCSystem.MetaData` GPS coordinate types

## Dependencies

- System.Text.Json
- TCSystem.MetaData

## Targets

- netstandard2.1
- net8.0
- net10.0
