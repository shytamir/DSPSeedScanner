[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$BuildDirectory,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$ExpectedSemanticVersion,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d+\.\d+\.\d+\.0$')]
    [string]$ExpectedAssemblyVersion,

    [Parameter(Mandatory = $true)]
    [string]$ExpectedReleaseLabel,

    [string]$RepositoryRoot = (Split-Path -Parent $PSScriptRoot),

    [string]$ReportPath = (
        Join-Path $RepositoryRoot 'artifacts\BUILD-TEST-REPORT.md'
    )
)

$ErrorActionPreference = 'Stop'
$expectedNames = @(
    'DSPSeedScanner.dll',
    'DSPSeedScanner.Core.dll',
    'DSPSeedScanner.Runtime.dll'
)
$actualDlls = @(Get-ChildItem -LiteralPath $BuildDirectory -Filter '*.dll' -File)
$actualNames = @($actualDlls | ForEach-Object Name)
if ($actualNames.Count -ne $expectedNames.Count -or
    @($actualNames | Where-Object { $expectedNames -cnotcontains $_ }).Count -ne 0) {
    throw 'Public build output contains missing or unintended DLLs.'
}

foreach ($dll in $actualDlls) {
    if ($dll.Length -eq 0) {
        throw "Build output is empty: $($dll.Name)"
    }
    $assemblyVersion = (
        [Reflection.AssemblyName]::GetAssemblyName($dll.FullName)
    ).Version.ToString()
    if ($assemblyVersion -cne $ExpectedAssemblyVersion) {
        throw "$($dll.Name) assembly version is $assemblyVersion; expected $ExpectedAssemblyVersion."
    }
    $file = [Diagnostics.FileVersionInfo]::GetVersionInfo($dll.FullName)
    if ($file.FileVersion -cne $ExpectedAssemblyVersion) {
        throw "$($dll.Name) file version is $($file.FileVersion); expected $ExpectedAssemblyVersion."
    }
    if ($file.ProductVersion -cne $ExpectedReleaseLabel) {
        throw "$($dll.Name) product version is $($file.ProductVersion); expected $ExpectedReleaseLabel."
    }
}

$versionSource = Get-Content -Raw -LiteralPath (
    Join-Path $RepositoryRoot 'src\DSPSeedScanner.Plugin\BuildVersion.cs'
)
if (-not $versionSource.Contains(
        "BepInPluginVersion = `"$ExpectedSemanticVersion`"")) {
    throw 'Generated BepInEx plugin version does not match the package version.'
}
if (-not $versionSource.Contains(
        "PluginVersion = `"$ExpectedSemanticVersion`"")) {
    throw 'Generated plugin version does not match the semantic version.'
}
$pluginSource = Get-Content -Raw -LiteralPath (
    Join-Path $RepositoryRoot 'src\DSPSeedScanner.Plugin\DSPSeedScannerPlugin.cs'
)
if (-not $pluginSource.Contains(
        '[BepInPlugin(PluginGuid, PluginName, BuildVersion.BepInPluginVersion)]')) {
    throw 'BepInPlugin does not use the generated numeric plugin version.'
}

$reportDirectory = Split-Path -Parent $ReportPath
New-Item -ItemType Directory -Force -Path $reportDirectory | Out-Null
@"
# Scanner build verification

| Check | Result |
| --- | --- |
| Scanner-owned DLL allowlist | Passed |
| Non-empty assemblies | Passed |
| Semantic/plugin version | ``$ExpectedSemanticVersion`` |
| Assembly/file version | ``$ExpectedAssemblyVersion`` |
| Product/release label | ``$ExpectedReleaseLabel`` |
| External runtime assemblies copied | No |
"@ | Set-Content -LiteralPath $ReportPath -Encoding utf8

Write-Output "Scanner build verification passed: $ExpectedSemanticVersion"
