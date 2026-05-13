# TCSystem.Thread

[![NuGet](https://img.shields.io/nuget/v/TCSystem.Thread.svg)](https://www.nuget.org/packages/TCSystem.Thread/)

Thread helper classes including a worker thread that queues commands and executes them in order. Supports async `Task`-based APIs and cancellation tokens.

## Installation

```bash
dotnet add package TCSystem.Thread
```

## Features

- Worker thread with command queue (FIFO execution)
- Async `Task`-based APIs
- Cancellation token support

## Dependencies

- TCSystem.Logging

## Targets

- netstandard2.1
- net8.0
- net10.0
