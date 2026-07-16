# TCSystem.MetaData

[![NuGet](https://img.shields.io/nuget/v/TCSystem.MetaData.svg)](https://www.nuget.org/packages/TCSystem.MetaData/)

Domain types for image metadata, including timestamps, orientation, GPS/location data, processing state, people, faces,
and tags. Models use return-new-instance change helpers rather than in-place mutation and support Newtonsoft.Json
serialization.

## Installation

```bash
dotnet add package TCSystem.MetaData
```

## Features

- Image metadata and processing-information models
- GPS points, positions, addresses, and locations
- Person, face, and person-tag models
- Fixed-point coordinate and rectangle types
- JSON serialization and deserialization helpers
- Immutable-style `ImageExt` change operations

## Repository Development

Repository-wide prerequisites, target frameworks, dependencies, build, test, coverage, and API-documentation instructions
are maintained in the [main README](../README.md).
