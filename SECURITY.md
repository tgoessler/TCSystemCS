# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 5.x     | :white_check_mark: |
| < 5.0   | :x:                |

## Dependency Vulnerability Checks

After restoring the solution with the .NET 10 SDK, run the following before releasing packages:

```bash
dotnet package list --project TCSystem.slnx --vulnerable --include-transitive --no-restore
```

## Reporting a Vulnerability

If you find a vulnerability, please report it directly to [@TGOESSLER](https://github.com/tgoessler).
