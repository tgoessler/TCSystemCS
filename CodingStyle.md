# TCSystem Coding Style

This document summarizes the coding style used in this repository.

## Tooling Baseline

- Language version: **C# 14**.
- Target frameworks for libraries: `netstandard2.1`, `net8.0`, `net10.0`.
- Warnings are errors (`TreatWarningsAsErrors=true`).
- .NET analyzers are enabled at the latest analysis level.
- `.editorconfig` currently enforces `CA2000` as an error: dispose objects before losing scope.
- ReSharper cleanup profile: `PickBash: Full Cleanup`.

## File Header

Every C# source file must start with the TCSystem copyright header used throughout the project:

```csharp
﻿// *******************************************************************************
// 
//  *******   ***   ***               *
//     *     *     *                  *
//     *    *      *                *****
//     *    *       ***  *   *   **   *    **    ***
//     *    *          *  * *   *     *   ****  * * *
//     *     *         *   *      *   * * *     * * *
//     *      ***   ***    *     **   **   **   *   *
//                         *
// *******************************************************************************
//  see https://github.com/tgoessler/TCSystemCS for details.
//  Copyright (C) 2003 - 2026 Thomas Goessler. All Rights Reserved.
// *******************************************************************************
// 
//  TCSystem is the legal property of its developers.
//  Please refer to the COPYRIGHT file distributed with this source distribution.
// 
// *******************************************************************************
```

Update the copyright year as needed.

## File Layout

Use this high-level order in C# files:

1. File header.
2. `#region Usings` containing all `using` directives.
3. File-scoped namespace declaration.
4. Type declaration.
5. Type members grouped by access level regions.

Example:

```csharp
#region Usings

using System;
using NUnit.Framework;
using TCSystem.MetaData;

#endregion

namespace TCSystem.Feature;

public sealed class Example
{
#region Public

    public void DoWork()
    {
    }

#endregion

#region Private

    private readonly int _value;

#endregion
}
```

### Regions

- Use `#region Usings` for `using` directives when a file has usings.
- Use access-level regions inside types:
  - `#region Public`
  - `#region Internal`
  - `#region Protected`
  - `#region Private`
- ReSharper file layout orders members inside each access region as:
  1. Constants and enums
  2. Constructors / destructors
  3. Methods
  4. Event-dispatching methods named `Raise*`
  5. Events
  6. Properties / auto-properties
  7. Other non-field members
  8. Fields
- NUnit test fixtures are an exception: ReSharper removes regions and orders setup/teardown, test methods, then other members.

## Namespaces and Types

- Use **file-scoped namespaces**:

```csharp
namespace TCSystem.MetaData;
```

- Prefer `sealed` classes unless inheritance is explicitly required.
- Use primary constructors for small immutable model classes where appropriate.
- Interfaces use the `I` prefix.
- Type and namespace names use PascalCase.

## Usings

- Put `using` directives inside `#region Usings`.
- Keep usings optimized and sorted by cleanup tooling.
- Typical ordering is:
  1. `System` namespaces
  2. Third-party namespaces
  3. TCSystem/project namespaces
- Use aliases for conflicting factory names, for example:

```csharp
using Factory = TCSystem.Logging.Factory;
```

## Naming

Naming rules are based on ReSharper settings:

| Element | Style | Prefix |
|---------|-------|--------|
| Namespaces, classes, structs, enums, delegates | PascalCase | none |
| Interfaces | PascalCase | `I` |
| Methods | PascalCase | none |
| Local functions | PascalCase | none |
| Properties | PascalCase | none |
| Events | PascalCase | none |
| Enum members | PascalCase | none |
| Constants | PascalCase | none |
| Private constants | PascalCase | none |
| Local constants | camelCase | none |
| Parameters | camelCase | none |
| Local variables | camelCase | none |
| Private fields | camelCase | `_` |
| Static/private readonly fields | camelCase | `_` |
| Non-private fields | camelCase | `_` unless public struct field rules apply |
| Public struct fields | PascalCase | none |
| Type parameters | PascalCase | `T` |
| Primary-constructor parameters used as fields | camelCase | `_` |

`DB` is configured as an accepted abbreviation.

Examples:

```csharp
public interface IDB2Read;

internal sealed class DB2Files(DB2Instance _instance)
{
#region Private

    private const string TableName = "Files";
    private readonly SemaphoreSlim _semaphore = new(1);

#endregion
}
```

## Formatting

- Indentation: **4 spaces**, no tabs in C# source files.
- ReSharper flags tabs/spaces mismatch as an error.
- Wrap limit: **180 characters**.
- Align multiline parameter lists and base/implements lists.
- Empty blocks may be kept on one line when appropriate:

```csharp
public TakeoutLocation() { }
```

- Preprocessor regions are not indented.
- Primary-constructor parameters:
  - Indent wrapped parameters inside parentheses.
  - Wrap if long.
  - Keep up to 4 primary-constructor parameters on one line where possible.

## Braces and Control Flow

Braces are required for:

- `if` / `else`
- `for`
- `foreach`
- `while`
- `do`
- `lock`
- `fixed`
- `using` statements

Example:

```csharp
if (value != null)
{
    Process(value);
}
```

Do not rely on single-line unbraced statements.

## Type Usage

- Use `var` only when the type is evident from the right-hand side.
- Prefer explicit types when they improve readability or the right-hand side does not make the type obvious.
- Use target-typed `new` when the type is evident:

```csharp
private readonly Queue<string> _items = new();
Location location = new(Address.Undefined, gps);
```

- Prefer built-in C# type keywords (`string`, `int`, `long`) over framework type names where appropriate.
- Let cleanup remove redundant qualifiers and redundant parentheses.

## Project Design Patterns

These project-level patterns are part of the coding conventions and should be followed for new code:

- **Immutable data models**: domain types such as `Image`, `PersonTag`, `Location`, and related metadata classes should behave immutably. Prefer constructors/factory/change methods that return new instances instead of mutating existing objects.
- **Abstract logging facade**: use `TCSystem.Logging.Logger` instead of referencing Serilog directly from library code.
- **Conditional debug logging**: debug logging goes through the logging abstraction so debug calls are compiled out in Release builds via `[Conditional("DEBUG")]`.
- **Namespace-scoped logging**: each namespace should have a local `Log.cs` with a static logger obtained through `Factory.GetLogger(typeof(Log))`.
- **Database instance pooling**: database access uses the `DB2` / `InstanceAcquire` pattern with `using` statements for thread-safe resource acquisition and release.
- **Factory APIs**: create and destroy database/logging infrastructure through the provided factory types instead of constructing implementation types from consumers.
- **Sealed implementation classes**: mark classes `sealed` when polymorphism is not required.
- **Threading helpers**: use the provided worker-thread, semaphore, and async-update helpers instead of duplicating concurrency primitives at call sites.

## Performance Conventions

- Use `[MethodImpl(MethodImplOptions.AggressiveInlining)]` for small, hot utility methods when profiling or usage patterns justify it.
- Prefer `ValueTask` over `Task` for async methods that often complete synchronously.
- Use `ReferenceEquals` checks before expensive equality comparisons.
- Avoid unnecessary allocations in utility and data-access paths.

## Nullability and Equality

- Use pattern matching for null/state checks when it improves clarity:

```csharp
if (image.Location is not { IsSet: true })
{
}
```

- Prefer `ReferenceEquals` before expensive equality comparisons when comparing domain models.
- Domain models implement `IEquatable<T>` and use immutable return-new-instance methods for changes.

## Resources and Disposal

- Dispose disposable objects with `using`, `await using`, or explicit factory destroy methods as required.
- `CA2000` is an error.
- Database objects use the `InstanceAcquire` / `using` pattern.
- TCSystem DB factory objects should be destroyed via `MetaDataDB.Factory.Destroy(ref db)` when used by tools.

## Async and Threading

- Prefer `ValueTask` where async APIs frequently complete synchronously.
- Use `CancellationToken` for cancellable operations.
- Avoid fire-and-forget tasks unless failure handling is explicit.
- Worker-thread APIs queue commands FIFO and log failures.

## Logging

- Use namespace-scoped `Log.cs` files:

```csharp
namespace TCSystem.Feature;

internal static class Log
{
#region Public

    public static Logger Instance { get; } = Factory.GetLogger(typeof(Log));

#endregion
}
```

- Debug logging is conditionally compiled out in Release builds via the logging abstraction.
- Prefer structured, useful messages; do not log secrets.

## Tests

- Test framework: NUnit 4.x.
- Test projects live in `{Project}/Tests/`.
- Test projects target `net8.0;net10.0` and import `NoPackaging.props`.
- Test source files follow the same header, namespace, using, and formatting rules.
- Use descriptive test names following `MethodOrScenario_Condition_ExpectedResult` where practical.
- Use `Assert.That(...)` syntax.

## Code Cleanup Expectations

The ReSharper full cleanup profile performs, among other things:

- Reformat code and documentation comments.
- Optimize usings and keep them inside `#region Usings`.
- Reorder type members according to the file layout rules.
- Sort modifiers.
- Add missing access modifiers.
- Arrange braces and arguments.
- Shorten references and remove redundant qualifiers.
- Make fields `readonly` where possible.
- Make auto-properties get-only where possible.
- Update file headers.
- Remove code redundancies.

Run cleanup before committing when using JetBrains Rider/ReSharper.
