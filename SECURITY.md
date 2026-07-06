# Security Policy

## Supported Versions

| Version | Supported          |
| ------- | ------------------ |
| 5.x     | :white_check_mark: |
| < 5.0   | :x:                |

## Dependency Vulnerability Checks

Run the following before releasing packages:

```bash
dotnet list TCSystem.slnx package --vulnerable --include-transitive
```

## Reporting a Vulnerability

If you find a vulnerability, please report it directly to [@TGOESSLER](https://github.com/tgoessler).
