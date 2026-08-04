[CmdletBinding()]
param(
    [string]$Configuration = 'Release',
    [string]$Framework = 'net8.0'
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$repositoryRoot = $PSScriptRoot
$statisticsPath = Join-Path $repositoryRoot 'STATISTICS.md'
$culture = [System.Globalization.CultureInfo]::InvariantCulture
$separator = [char]0x1f

$testProjects = @(
    [pscustomobject]@{ Name = 'TCSystem.Gps.Tests'; Path = 'Gps/Tests/TCSystem.Gps.Tests.csproj' },
    [pscustomobject]@{ Name = 'TCSystem.MetaData.Tests'; Path = 'MetaData/Tests/TCSystem.MetaData.Tests.csproj' },
    [pscustomobject]@{ Name = 'TCSystem.MetaDataDB.Tests'; Path = 'MetaDataDB/Tests/TCSystem.MetaDataDB.Tests.csproj' },
    [pscustomobject]@{ Name = 'TCSystem.Thread.Tests'; Path = 'Thread/Tests/TCSystem.Thread.Tests.csproj' },
    [pscustomobject]@{ Name = 'TCSystem.Util.Tests'; Path = 'Util/Tests/TCSystem.Util.Tests.csproj' }
)

$libraryNames = @(
    'TCSystem.Gps',
    'TCSystem.Logging',
    'TCSystem.MetaData',
    'TCSystem.MetaDataDB',
    'TCSystem.Thread',
    'TCSystem.Util'
)

$areaOrder = @(
    'Gps',
    'Logging',
    'MetaData',
    'MetaDataDB',
    'Thread',
    'Tools/DBConverter',
    'Tools/TakeoutReader',
    'Util'
)

function Invoke-Git {
    param([Parameter(Mandatory)][string[]]$Arguments)

    $output = @(& git -C $repositoryRoot @Arguments)
    if ($LASTEXITCODE -ne 0)
    {
        throw "git $($Arguments -join ' ') failed with exit code $LASTEXITCODE."
    }

    return $output
}

function Format-Integer {
    param([long]$Value)

    return $Value.ToString('N0', $culture)
}

function Format-Percent {
    param(
        [long]$Visited,
        [long]$Total
    )

    if ($Total -eq 0)
    {
        return 'n/a'
    }

    return ((100.0 * $Visited / $Total).ToString('0.00', $culture) + '%')
}

function Add-MergedValue {
    param(
        [Parameter(Mandatory)][hashtable]$Map,
        [Parameter(Mandatory)][string]$Key,
        [Parameter(Mandatory)][bool]$Visited
    )

    if (-not $Map.ContainsKey($Key))
    {
        $Map[$Key] = $Visited
    }
    elseif ($Visited)
    {
        $Map[$Key] = $true
    }
}

function Get-SourceArea {
    param([Parameter(Mandatory)][string]$Path)

    $parts = $Path.Split('/')
    if ($parts[0] -eq 'Tools')
    {
        return "$($parts[0])/$($parts[1])"
    }

    return $parts[0]
}

function Get-SourceScope {
    param([Parameter(Mandatory)][string]$Path)

    if ($Path.Contains('/Tests/'))
    {
        return 'Tests'
    }

    return 'Production'
}

function Get-RuntimeVersion {
    param([Parameter(Mandatory)][string]$TargetFramework)

    if ($TargetFramework -notmatch '^net(?<major>\d+)\.0$')
    {
        return 'unknown'
    }

    $major = $Matches.major
    $versions = @()
    foreach ($line in @(& dotnet --list-runtimes))
    {
        if ($line -match "^Microsoft\.NETCore\.App (?<version>$major\.\d+\.\d+) ")
        {
            $versions += [version]$Matches.version
        }
    }

    if ($versions.Count -eq 0)
    {
        return 'not installed'
    }

    return (($versions | Sort-Object -Descending | Select-Object -First 1).ToString())
}

foreach ($command in @('git', 'dotnet'))
{
    if ($null -eq (Get-Command $command -ErrorAction SilentlyContinue))
    {
        throw "Required command '$command' was not found."
    }
}

$null = Invoke-Git -Arguments @('rev-parse', '--show-toplevel')
$temporaryRoot = Join-Path ([System.IO.Path]::GetTempPath()) ("TCSystem-statistics-" + [guid]::NewGuid().ToString('N'))
$testResults = [ordered]@{}
$coverageReports = [System.Collections.Generic.List[string]]::new()

try
{
    foreach ($testProject in $testProjects)
    {
        $projectPath = Join-Path $repositoryRoot $testProject.Path
        if (-not (Test-Path $projectPath -PathType Leaf))
        {
            throw "Test project '$($testProject.Path)' was not found."
        }

        $projectTemporaryName = $testProject.Name.Replace('.', '-')
        $resultsDirectory = Join-Path $temporaryRoot "TestResults/$projectTemporaryName"
        $coverageDirectory = Join-Path $temporaryRoot "Coverage/$projectTemporaryName"
        $coveragePrefix = Join-Path $coverageDirectory 'coverage'
        $null = New-Item -ItemType Directory -Path $resultsDirectory -Force
        $null = New-Item -ItemType Directory -Path $coverageDirectory -Force

        Write-Host "Testing $($testProject.Name) ($Framework)..."
        $dotnetArguments = @(
            'test',
            $projectPath,
            '--configuration', $Configuration,
            '--framework', $Framework,
            '--logger', 'trx;LogFileName=results.trx',
            '--results-directory', $resultsDirectory,
            '-p:CollectCoverage=true',
            '-p:CoverletOutputFormat=opencover',
            "-p:CoverletOutput=$coveragePrefix"
        )
        & dotnet @dotnetArguments
        if ($LASTEXITCODE -ne 0)
        {
            throw "Tests for $($testProject.Name) failed with exit code $LASTEXITCODE."
        }

        $trxPath = Join-Path $resultsDirectory 'results.trx'
        if (-not (Test-Path $trxPath -PathType Leaf))
        {
            throw "Test result '$trxPath' was not generated."
        }

        [xml]$trx = Get-Content $trxPath -Raw
        $counters = $trx.SelectSingleNode("/*[local-name()='TestRun']/*[local-name()='ResultSummary']/*[local-name()='Counters']")
        if ($null -eq $counters)
        {
            throw "Test counters were not found in '$trxPath'."
        }

        $passed = [int]$counters.GetAttribute('passed')
        $failed = [int]$counters.GetAttribute('failed')
        $total = [int]$counters.GetAttribute('total')
        $testResults[$testProject.Name] = [pscustomobject]@{
            Passed = $passed
            Skipped = $total - $passed - $failed
            Failed = $failed
            Total = $total
        }

        $projectCoverageReports = @(Get-ChildItem $coverageDirectory -Filter '*.opencover.xml' -File -Recurse)
        if ($projectCoverageReports.Count -ne 1)
        {
            throw "Expected one OpenCover report for $($testProject.Name), but found $($projectCoverageReports.Count)."
        }

        $coverageReports.Add($projectCoverageReports[0].FullName)
    }

    $coverageData = @{}
    foreach ($libraryName in $libraryNames)
    {
        $coverageData[$libraryName] = @{
            Sequence = @{}
            Branch = @{}
            Method = @{}
        }
    }

    foreach ($coverageReport in $coverageReports)
    {
        [xml]$coverageDocument = Get-Content $coverageReport -Raw
        foreach ($module in $coverageDocument.SelectNodes('/CoverageSession/Modules/Module'))
        {
            $moduleName = $module.SelectSingleNode('./ModuleName').InnerText
            if (-not $coverageData.ContainsKey($moduleName))
            {
                continue
            }

            $filePaths = @{}
            foreach ($file in $module.SelectNodes('./Files/File'))
            {
                $filePaths[$file.GetAttribute('uid')] = $file.GetAttribute('fullPath').ToLowerInvariant()
            }

            foreach ($class in $module.SelectNodes('./Classes/Class'))
            {
                $className = $class.SelectSingleNode('./FullName').InnerText
                foreach ($method in $class.SelectNodes('./Methods/Method'))
                {
                    $methodName = $method.SelectSingleNode('./Name').InnerText
                    $methodKey = $className + $separator + $methodName
                    $methodSummary = $method.SelectSingleNode('./Summary')
                    $methodVisited = [int]$methodSummary.GetAttribute('visitedMethods') -gt 0
                    Add-MergedValue -Map $coverageData[$moduleName].Method -Key $methodKey -Visited $methodVisited

                    foreach ($point in $method.SelectNodes('./SequencePoints/SequencePoint'))
                    {
                        $pointKeyParts = @(
                            $filePaths[$point.GetAttribute('fileid')],
                            $className,
                            $methodName,
                            $point.GetAttribute('ordinal'),
                            $point.GetAttribute('sl'),
                            $point.GetAttribute('sc'),
                            $point.GetAttribute('el'),
                            $point.GetAttribute('ec')
                        )
                        $pointKey = $pointKeyParts -join $separator
                        Add-MergedValue -Map $coverageData[$moduleName].Sequence -Key $pointKey -Visited ([int]$point.GetAttribute('vc') -gt 0)
                    }

                    foreach ($point in $method.SelectNodes('./BranchPoints/BranchPoint'))
                    {
                        $pointKeyParts = @(
                            $filePaths[$point.GetAttribute('fileid')],
                            $className,
                            $methodName,
                            $point.GetAttribute('ordinal'),
                            $point.GetAttribute('path'),
                            $point.GetAttribute('offset'),
                            $point.GetAttribute('offsetend'),
                            $point.GetAttribute('sl')
                        )
                        $pointKey = $pointKeyParts -join $separator
                        Add-MergedValue -Map $coverageData[$moduleName].Branch -Key $pointKey -Visited ([int]$point.GetAttribute('vc') -gt 0)
                    }
                }
            }
        }
    }

    foreach ($libraryName in $libraryNames)
    {
        if ($coverageData[$libraryName].Sequence.Count -eq 0)
        {
            throw "No coverage data was collected for $libraryName."
        }
    }
}
finally
{
    if (Test-Path $temporaryRoot)
    {
        Remove-Item $temporaryRoot -Recurse -Force
    }
}

$sourceRows = @{}
$sourceFiles = @(Invoke-Git -Arguments @('ls-files', '--', '*.cs'))
foreach ($sourceFile in $sourceFiles)
{
    $area = Get-SourceArea -Path $sourceFile
    $scope = Get-SourceScope -Path $sourceFile
    $rowKey = "$area$separator$scope"
    if (-not $sourceRows.ContainsKey($rowKey))
    {
        $sourceRows[$rowKey] = [pscustomobject]@{
            Area = $area
            Scope = $scope
            Files = 0
            Code = 0
            Comments = 0
            Blank = 0
            Total = 0
        }
    }

    $row = $sourceRows[$rowKey]
    $row.Files++
    foreach ($line in [System.IO.File]::ReadAllLines((Join-Path $repositoryRoot $sourceFile)))
    {
        $row.Total++
        if ([string]::IsNullOrWhiteSpace($line))
        {
            $row.Blank++
        }
        elseif ($line.TrimStart().StartsWith('//', [System.StringComparison]::Ordinal))
        {
            $row.Comments++
        }
        else
        {
            $row.Code++
        }
    }
}

$orderedSourceRows = [System.Collections.Generic.List[object]]::new()
foreach ($area in $areaOrder)
{
    foreach ($scope in @('Production', 'Tests'))
    {
        $rowKey = "$area$separator$scope"
        if ($sourceRows.ContainsKey($rowKey))
        {
            $orderedSourceRows.Add($sourceRows[$rowKey])
        }
    }
}

$productionRows = @($orderedSourceRows | Where-Object Scope -eq 'Production')
$testSourceRows = @($orderedSourceRows | Where-Object Scope -eq 'Tests')
function Get-SourceSubtotal {
    param([Parameter(Mandatory)][object[]]$Rows)

    return [pscustomobject]@{
        Files = [long](($Rows | Measure-Object Files -Sum).Sum)
        Code = [long](($Rows | Measure-Object Code -Sum).Sum)
        Comments = [long](($Rows | Measure-Object Comments -Sum).Sum)
        Blank = [long](($Rows | Measure-Object Blank -Sum).Sum)
        Total = [long](($Rows | Measure-Object Total -Sum).Sum)
    }
}

$productionSubtotal = Get-SourceSubtotal -Rows $productionRows
$testSourceSubtotal = Get-SourceSubtotal -Rows $testSourceRows
$sourceTotal = Get-SourceSubtotal -Rows @($orderedSourceRows)

$trackedFiles = @(Invoke-Git -Arguments @('ls-files'))
$repositoryFiles = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
foreach ($trackedFile in $trackedFiles)
{
    $null = $repositoryFiles.Add($trackedFile)
}
foreach ($generatedFile in @('STATISTICS.md', 'update-statistics.ps1'))
{
    if (Test-Path (Join-Path $repositoryRoot $generatedFile))
    {
        $null = $repositoryFiles.Add($generatedFile)
    }
}

$projectFiles = @(Invoke-Git -Arguments @('ls-files', '--', '*.csproj'))
$testProjectCount = @($projectFiles | Where-Object { $_.Contains('/Tests/') }).Count
$toolProjectCount = @($projectFiles | Where-Object { $_.StartsWith('Tools/') }).Count
$libraryProjectCount = $projectFiles.Count - $testProjectCount - $toolProjectCount

$testTotals = [pscustomobject]@{
    Passed = [long](($testResults.Values | Measure-Object Passed -Sum).Sum)
    Skipped = [long](($testResults.Values | Measure-Object Skipped -Sum).Sum)
    Failed = [long](($testResults.Values | Measure-Object Failed -Sum).Sum)
    Total = [long](($testResults.Values | Measure-Object Total -Sum).Sum)
}

$coverageRows = [System.Collections.Generic.List[object]]::new()
foreach ($libraryName in $libraryNames)
{
    $data = $coverageData[$libraryName]
    $coverageRows.Add([pscustomobject]@{
        Name = $libraryName
        VisitedSequence = [long]@($data.Sequence.Values | Where-Object { $_ }).Count
        Sequence = [long]$data.Sequence.Count
        VisitedBranch = [long]@($data.Branch.Values | Where-Object { $_ }).Count
        Branch = [long]$data.Branch.Count
        VisitedMethod = [long]@($data.Method.Values | Where-Object { $_ }).Count
        Method = [long]$data.Method.Count
    })
}
$coverageTotal = [pscustomobject]@{
    VisitedSequence = [long](($coverageRows | Measure-Object VisitedSequence -Sum).Sum)
    Sequence = [long](($coverageRows | Measure-Object Sequence -Sum).Sum)
    VisitedBranch = [long](($coverageRows | Measure-Object VisitedBranch -Sum).Sum)
    Branch = [long](($coverageRows | Measure-Object Branch -Sum).Sum)
    VisitedMethod = [long](($coverageRows | Measure-Object VisitedMethod -Sum).Sum)
    Method = [long](($coverageRows | Measure-Object Method -Sum).Sum)
}

$branch = (Invoke-Git -Arguments @('branch', '--show-current') | Select-Object -First 1)
if ([string]::IsNullOrWhiteSpace($branch))
{
    $branch = '(detached HEAD)'
}
$commit = (Invoke-Git -Arguments @('rev-parse', '--short', 'HEAD') | Select-Object -First 1)
$snapshotTime = [DateTimeOffset]::Now.ToString('yyyy-MM-dd HH:mm zzz', $culture)
$dotnetSdkVersion = (& dotnet --version | Select-Object -First 1)
if ([string]::IsNullOrWhiteSpace($dotnetSdkVersion))
{
    throw 'Unable to determine the .NET SDK version.'
}
$runtimeVersion = Get-RuntimeVersion -TargetFramework $Framework

[xml]$firstTestProject = Get-Content (Join-Path $repositoryRoot $testProjects[0].Path) -Raw
$coverletReference = $firstTestProject.SelectSingleNode("/Project/ItemGroup/PackageReference[@Include='coverlet.msbuild']")
$coverletVersion = if ($null -eq $coverletReference) { 'unknown' } else { $coverletReference.GetAttribute('Version') }

$lines = [System.Collections.Generic.List[string]]::new()
$lines.Add('# Project Statistics')
$lines.Add('')
$lines.Add(('Snapshot generated on **{0}** from the repository working tree on branch `{1}`, based on commit `{2}`.' -f $snapshotTime, $branch, $commit))
$lines.Add('Untracked files other than this report and its generator, generated output, and ignored files are excluded from the source-size statistics.')
$lines.Add('')
$lines.Add('## Repository overview')
$lines.Add('')
$lines.Add('| Metric | Count |')
$lines.Add('|---|---:|')
$lines.Add(('| Repository files represented | {0} |' -f (Format-Integer $repositoryFiles.Count)))
$lines.Add(('| .NET projects | {0} |' -f (Format-Integer $projectFiles.Count)))
$lines.Add(('| Library projects | {0} |' -f (Format-Integer $libraryProjectCount)))
$lines.Add(('| Command-line tool projects | {0} |' -f (Format-Integer $toolProjectCount)))
$lines.Add(('| Test projects | {0} |' -f (Format-Integer $testProjectCount)))
$lines.Add(('| Tracked C# files | {0} |' -f (Format-Integer $sourceTotal.Files)))
$lines.Add(('| Production C# files | {0} |' -f (Format-Integer $productionSubtotal.Files)))
$lines.Add(('| Test C# files | {0} |' -f (Format-Integer $testSourceSubtotal.Files)))
$lines.Add('| Target frameworks (libraries) | `netstandard2.1`, `net8.0`, `net10.0` |')
$lines.Add('| Target frameworks (tests) | `net8.0`, `net10.0` |')
$lines.Add('')
$lines.Add('## C# source size')
$lines.Add('')
$lines.Add('"Code" is a non-blank line that is not a `//` comment-only line. Preprocessor directives, braces, and declarations are code. The comment count includes file headers and XML documentation.')
$lines.Add('')
$lines.Add('| Area | Scope | Files | Code | Comments | Blank | Physical lines |')
$lines.Add('|---|---|---:|---:|---:|---:|---:|')
foreach ($row in $orderedSourceRows)
{
    $lines.Add(('| {0} | {1} | {2} | {3} | {4} | {5} | {6} |' -f $row.Area, $row.Scope, (Format-Integer $row.Files), (Format-Integer $row.Code), (Format-Integer $row.Comments), (Format-Integer $row.Blank), (Format-Integer $row.Total)))
}
$lines.Add(('| **Production subtotal** |  | **{0}** | **{1}** | **{2}** | **{3}** | **{4}** |' -f (Format-Integer $productionSubtotal.Files), (Format-Integer $productionSubtotal.Code), (Format-Integer $productionSubtotal.Comments), (Format-Integer $productionSubtotal.Blank), (Format-Integer $productionSubtotal.Total)))
$lines.Add(('| **Test subtotal** |  | **{0}** | **{1}** | **{2}** | **{3}** | **{4}** |' -f (Format-Integer $testSourceSubtotal.Files), (Format-Integer $testSourceSubtotal.Code), (Format-Integer $testSourceSubtotal.Comments), (Format-Integer $testSourceSubtotal.Blank), (Format-Integer $testSourceSubtotal.Total)))
$lines.Add(('| **Total** |  | **{0}** | **{1}** | **{2}** | **{3}** | **{4}** |' -f (Format-Integer $sourceTotal.Files), (Format-Integer $sourceTotal.Code), (Format-Integer $sourceTotal.Comments), (Format-Integer $sourceTotal.Blank), (Format-Integer $sourceTotal.Total)))
$lines.Add('')
$testCodeRatio = 100.0 * $testSourceSubtotal.Code / $productionSubtotal.Code
$lines.Add(('Test code is **{0}%** of production code by this physical code-line measure.' -f $testCodeRatio.ToString('0.00', $culture)))
$lines.Add('')
$lines.Add('## Test results')
$lines.Add('')
$lines.Add(('The `{0}` coverage run completed successfully with **{1} tests: {2} passed, {3} skipped, and {4} failed**.' -f $Framework, (Format-Integer $testTotals.Total), (Format-Integer $testTotals.Passed), (Format-Integer $testTotals.Skipped), (Format-Integer $testTotals.Failed)))
$lines.Add('')
$lines.Add('| Test project | Passed | Skipped | Failed | Total |')
$lines.Add('|---|---:|---:|---:|---:|')
foreach ($testProject in $testProjects)
{
    $result = $testResults[$testProject.Name]
    $lines.Add(('| {0} | {1} | {2} | {3} | {4} |' -f $testProject.Name, (Format-Integer $result.Passed), (Format-Integer $result.Skipped), (Format-Integer $result.Failed), (Format-Integer $result.Total)))
}
$lines.Add(('| **Total** | **{0}** | **{1}** | **{2}** | **{3}** |' -f (Format-Integer $testTotals.Passed), (Format-Integer $testTotals.Skipped), (Format-Integer $testTotals.Failed), (Format-Integer $testTotals.Total)))
$lines.Add('')
$lines.Add('The skipped MetaDataDB converter tests require optional legacy database fixtures that are not stored in the repository.')
$lines.Add('')
$lines.Add('## Code coverage')
$lines.Add('')
$lines.Add(('Coverage was collected for `{0}` with Coverlet {1} in OpenCover format. Results from all five test projects were merged by production module and coverage point; a point visited by any test suite is considered visited. Test assemblies are excluded.' -f $Framework, $coverletVersion))
$lines.Add('')
$lines.Add('| Production library | Line coverage | Branch coverage | Method coverage |')
$lines.Add('|---|---:|---:|---:|')
foreach ($row in $coverageRows)
{
    $lineCoverage = '{0} ({1}/{2})' -f (Format-Percent $row.VisitedSequence $row.Sequence), (Format-Integer $row.VisitedSequence), (Format-Integer $row.Sequence)
    $branchCoverage = '{0} ({1}/{2})' -f (Format-Percent $row.VisitedBranch $row.Branch), (Format-Integer $row.VisitedBranch), (Format-Integer $row.Branch)
    $methodCoverage = '{0} ({1}/{2})' -f (Format-Percent $row.VisitedMethod $row.Method), (Format-Integer $row.VisitedMethod), (Format-Integer $row.Method)
    $lines.Add(('| {0} | {1} | {2} | {3} |' -f $row.Name, $lineCoverage, $branchCoverage, $methodCoverage))
}
$totalLineCoverage = '{0} ({1}/{2})' -f (Format-Percent $coverageTotal.VisitedSequence $coverageTotal.Sequence), (Format-Integer $coverageTotal.VisitedSequence), (Format-Integer $coverageTotal.Sequence)
$totalBranchCoverage = '{0} ({1}/{2})' -f (Format-Percent $coverageTotal.VisitedBranch $coverageTotal.Branch), (Format-Integer $coverageTotal.VisitedBranch), (Format-Integer $coverageTotal.Branch)
$totalMethodCoverage = '{0} ({1}/{2})' -f (Format-Percent $coverageTotal.VisitedMethod $coverageTotal.Method), (Format-Integer $coverageTotal.VisitedMethod), (Format-Integer $coverageTotal.Method)
$lines.Add(('| **Combined libraries** | **{0}** | **{1}** | **{2}** |' -f $totalLineCoverage, $totalBranchCoverage, $totalMethodCoverage))
$lines.Add('')
$lines.Add('The coverage scope is the six production libraries loaded by the test suites. The two command-line tools have no dedicated test projects and were not instrumented, so they are excluded from the combined percentage rather than counted as 0%. Logging has no dedicated test project; its reported coverage comes from execution through Thread and MetaDataDB tests. Coverlet "line coverage" is based on instrumented sequence points and is not the same denominator as physical source lines above.')
$lines.Add('')
$lines.Add('## Reproduction')
$lines.Add('')
$lines.Add('Run the generator from the repository root:')
$lines.Add('')
$lines.Add('```powershell')
$lines.Add('.\update-statistics.ps1')
$lines.Add('```')
$lines.Add('')
$lines.Add('The generator uses temporary TRX and OpenCover reports and removes them after updating this file. Its effective coverage command for each test project is:')
$lines.Add('')
$lines.Add('```powershell')
$lines.Add(('dotnet test <test-project> --configuration {0} --framework {1} `' -f $Configuration, $Framework))
$lines.Add('  --logger "trx;LogFileName=results.trx" --results-directory <temporary-directory> `')
$lines.Add('  -p:CollectCoverage=true -p:CoverletOutputFormat=opencover `')
$lines.Add('  -p:CoverletOutput=<temporary-coverage-prefix>')
$lines.Add('```')
$lines.Add('')
$lines.Add('Environment used:')
$lines.Add('')
$lines.Add(('- .NET SDK `{0}`' -f $dotnetSdkVersion))
$lines.Add(('- .NET runtime `{0}`' -f $runtimeVersion))
$lines.Add(('- Coverlet MSBuild `{0}`' -f $coverletVersion))
$lines.Add('')
$lines.Add("Source-size statistics are calculated from ``git ls-files '*.cs'``; files under ``bin/`` and ``obj/`` are not tracked and therefore are not included.")

$utf8WithoutBom = [System.Text.UTF8Encoding]::new($false)
[System.IO.File]::WriteAllLines($statisticsPath, $lines, $utf8WithoutBom)
Write-Host "Updated $statisticsPath"
