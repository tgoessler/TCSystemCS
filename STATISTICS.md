# Project Statistics

Snapshot generated on **2026-07-20 19:02 +02:00** from the repository working tree on branch `develop`, based on commit `1c9a126`.
Untracked files other than this report and its generator, generated output, and ignored files are excluded from the source-size statistics.

## Repository overview

| Metric | Count |
|---|---:|
| Repository files represented | 174 |
| .NET projects | 13 |
| Library projects | 6 |
| Command-line tool projects | 2 |
| Test projects | 5 |
| Tracked C# files | 110 |
| Production C# files | 68 |
| Test C# files | 42 |
| Target frameworks (libraries) | `netstandard2.1`, `net8.0`, `net10.0` |
| Target frameworks (tests) | `net8.0`, `net10.0` |

## C# source size

"Code" is a non-blank line that is not a `//` comment-only line. Preprocessor directives, braces, and declarations are code. The comment count includes file headers and XML documentation.

| Area | Scope | Files | Code | Comments | Blank | Physical lines |
|---|---|---:|---:|---:|---:|---:|
| Gps | Production | 3 | 135 | 119 | 53 | 307 |
| Gps | Tests | 2 | 135 | 38 | 23 | 196 |
| Logging | Production | 4 | 245 | 144 | 80 | 469 |
| MetaData | Production | 24 | 1,878 | 936 | 554 | 3,368 |
| MetaData | Tests | 20 | 1,633 | 407 | 392 | 2,432 |
| MetaDataDB | Production | 16 | 2,918 | 377 | 537 | 3,832 |
| MetaDataDB | Tests | 10 | 1,460 | 199 | 370 | 2,029 |
| Thread | Production | 13 | 472 | 282 | 168 | 922 |
| Thread | Tests | 6 | 543 | 140 | 160 | 843 |
| Tools/DBConverter | Production | 2 | 379 | 41 | 83 | 503 |
| Tools/TakeoutReader | Production | 2 | 104 | 39 | 23 | 166 |
| Util | Production | 4 | 75 | 97 | 30 | 202 |
| Util | Tests | 4 | 208 | 76 | 49 | 333 |
| **Production subtotal** |  | **68** | **6,206** | **2,035** | **1,528** | **9,769** |
| **Test subtotal** |  | **42** | **3,979** | **860** | **994** | **5,833** |
| **Total** |  | **110** | **10,185** | **2,895** | **2,522** | **15,602** |

Test code is **64.12%** of production code by this physical code-line measure.

## Test results

The `net8.0` coverage run completed successfully with **285 tests: 263 passed, 22 skipped, and 0 failed**.

| Test project | Passed | Skipped | Failed | Total |
|---|---:|---:|---:|---:|
| TCSystem.Gps.Tests | 4 | 0 | 0 | 4 |
| TCSystem.MetaData.Tests | 148 | 0 | 0 | 148 |
| TCSystem.MetaDataDB.Tests | 52 | 22 | 0 | 74 |
| TCSystem.Thread.Tests | 38 | 0 | 0 | 38 |
| TCSystem.Util.Tests | 21 | 0 | 0 | 21 |
| **Total** | **263** | **22** | **0** | **285** |

The skipped MetaDataDB converter tests require optional legacy database fixtures that are not stored in the repository.

## Code coverage

Coverage was collected for `net8.0` with Coverlet 10.0.1 in OpenCover format. Results from all five test projects were merged by production module and coverage point; a point visited by any test suite is considered visited. Test assemblies are excluded.

| Production library | Line coverage | Branch coverage | Method coverage |
|---|---:|---:|---:|
| TCSystem.Gps | 88.10% (37/42) | 72.22% (13/18) | 84.62% (11/13) |
| TCSystem.Logging | 15.91% (14/88) | 10.53% (4/38) | 22.22% (6/27) |
| TCSystem.MetaData | 88.08% (717/814) | 72.38% (262/362) | 91.00% (263/289) |
| TCSystem.MetaDataDB | 85.01% (1,440/1,694) | 66.50% (272/409) | 87.63% (170/194) |
| TCSystem.Thread | 89.35% (151/169) | 76.32% (29/38) | 95.12% (39/41) |
| TCSystem.Util | 100.00% (17/17) | 100.00% (10/10) | 100.00% (5/5) |
| **Combined libraries** | **84.14% (2,376/2,824)** | **67.43% (590/875)** | **86.82% (494/569)** |

The coverage scope is the six production libraries loaded by the test suites. The two command-line tools have no dedicated test projects and were not instrumented, so they are excluded from the combined percentage rather than counted as 0%. Logging has no dedicated test project; its reported coverage comes from execution through Thread and MetaDataDB tests. Coverlet "line coverage" is based on instrumented sequence points and is not the same denominator as physical source lines above.

## Reproduction

Run the generator from the repository root:

```powershell
.\update-statistics.ps1
```

The generator uses temporary TRX and OpenCover reports and removes them after updating this file. Its effective coverage command for each test project is:

```powershell
dotnet test <test-project> --configuration Release --framework net8.0 `
  --logger "trx;LogFileName=results.trx" --results-directory <temporary-directory> `
  -p:CollectCoverage=true -p:CoverletOutputFormat=opencover `
  -p:CoverletOutput=<temporary-coverage-prefix>
```

Environment used:

- .NET SDK `10.0.300`
- .NET runtime `8.0.27`
- Coverlet MSBuild `10.0.1`

Source-size statistics are calculated from `git ls-files '*.cs'`; files under `bin/` and `obj/` are not tracked and therefore are not included.
