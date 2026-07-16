# TCSystem.Gps

[![NuGet](https://img.shields.io/nuget/v/TCSystem.Gps.svg)](https://www.nuget.org/packages/TCSystem.Gps/)

Reader and lookup helpers for Google Takeout location-history `records.json` files. JSON is deserialized asynchronously
with `System.Text.Json` into `TakeoutRecords`, which can filter records and find the timestamp-nearest location.

## Installation

```bash
dotnet add package TCSystem.Gps
```

## Features

- Asynchronous `records.json` deserialization from a stream
- Google Takeout timestamp, form-factor, and coordinate mapping
- Phone, tablet, and desktop filtering, with unspecified-form-factor records included alongside the requested type
- Timestamp ordering and binary-search nearest-location lookup
- Integration with `TCSystem.MetaData.GpsPoint`

The complete location collection is held in memory after deserialization; size memory capacity appropriately for large
Takeout exports.

## Repository Development

Repository-wide prerequisites, target frameworks, dependencies, build, test, coverage, and API-documentation instructions
are maintained in the [main README](../README.md).
