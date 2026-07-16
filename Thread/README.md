# TCSystem.Thread

[![NuGet](https://img.shields.io/nuget/v/TCSystem.Thread.svg)](https://www.nuget.org/packages/TCSystem.Thread/)

Concurrency helpers for queued worker execution, bounded parallel work, asynchronous update coordination, semaphore
scopes, and wait handles.

## Installation

```bash
dotnet add package TCSystem.Thread
```

## Features

- FIFO worker thread with synchronous and `Task`-based queue APIs
- Worker lifecycle, idle events, cancellation token, and queue clearing
- Bounded multiple-task executor with wait and cancellation support
- Async update helper and disposable update scopes
- Synchronous and asynchronous `SemaphoreSlim` lock scopes
- Awaitable `WaitHandle` extension

Create the main abstractions through `TCSystem.Thread.Factory` rather than constructing implementation classes directly.

## Repository Development

Repository-wide prerequisites, target frameworks, dependencies, build, test, coverage, and API-documentation instructions
are maintained in the [main README](../README.md).
