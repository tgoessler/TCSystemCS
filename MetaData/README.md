# TCSystem.MetaData

[![NuGet](https://img.shields.io/nuget/v/TCSystem.MetaData.svg)](https://www.nuget.org/packages/TCSystem.MetaData/)

Domain types for image metadata, including timestamps, orientation, GPS/location data, processing state, people, faces,
and tags. Models use return-new-instance change helpers rather than in-place mutation and support Newtonsoft.Json
serialization.

## Installation

```bash
dotnet add package TCSystem.MetaData
```

## Key Concepts

- `Image` holds file details, dates, orientation, location, processing flags, text tags, and person tags. Its change
  methods return updated copies instead of modifying the supplied instance.
- `PersonTag` links a `Person` to a detected `Face`, including its rectangle, detector, visibility, quality, and optional
  128-value descriptor.
- `Location`, `Address`, and `GpsPoint` model geographic metadata; GPS points can calculate Haversine distance.
- Fixed-point coordinates, normalized `Rectangle` values, and `Orientation` provide stable face geometry and EXIF
  transformations.
- Core models provide Newtonsoft.Json serialization helpers.
- `ProcessingInfos` tracks which analysis calculations have already been run for an image, such as face-detection and
  image-classification passes.

## Face-Matching Quality

`FaceQuality` only classifies suitability for finding similar faces; it does not control matching. The semantic scale is
`Unusable` → `Poor` → `Normal` → `Good` → `Excellent`.

Persisted values are `Normal = 0`, `Poor = 1`, `Good = 2`, `Unusable = 3`, and `Excellent = 4`. They preserve existing
JSON and SQLite compatibility but are not in semantic order, so compare named values rather than integers.

## Repository Development

Repository-wide prerequisites, target frameworks, dependencies, build, test, coverage, and API-documentation instructions
are maintained in the [main README](../README.md).
