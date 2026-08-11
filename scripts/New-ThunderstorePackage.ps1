[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string[]]$DllPaths,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$VersionNumber,

    [string]$RepositoryRoot = (Split-Path -Parent $PSScriptRoot),

    [string]$ManifestTemplatePath = (
        Join-Path $RepositoryRoot 'packaging\manifest.template.json'
    ),

    [string]$ReadmePath = (
        Join-Path $RepositoryRoot 'packaging\README.md'
    ),

    [string]$IconPath = (
        Join-Path $RepositoryRoot 'packaging\icon.png'
    ),

    [string]$OutputDirectory = (
        Join-Path $RepositoryRoot 'artifacts\packages'
    )
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

foreach ($requiredPath in @(
        $DllPaths,
        $ManifestTemplatePath,
        $ReadmePath,
        $IconPath
    )) {
    if (-not (Test-Path -LiteralPath $requiredPath -PathType Leaf)) {
        throw "Required package input was not found: $requiredPath"
    }
}

$expectedDllNames = @(
    'DSPSeedScanner.dll',
    'DSPSeedScanner.Core.dll',
    'DSPSeedScanner.Runtime.dll'
)
$actualDllNames = @($DllPaths | ForEach-Object { Split-Path -Leaf $_ })
if ($actualDllNames.Count -ne $expectedDllNames.Count -or
    @($actualDllNames | Where-Object { $expectedDllNames -cnotcontains $_ }).Count -ne 0 -or
    ($actualDllNames | Select-Object -Unique).Count -ne $actualDllNames.Count) {
    throw 'Package DLL inputs must be exactly the scanner plugin, core, and runtime assemblies.'
}

$template = Get-Content -Raw -LiteralPath $ManifestTemplatePath
$placeholder = '{{VERSION_NUMBER}}'
if (([regex]::Matches(
            $template,
            [regex]::Escape($placeholder)
        )).Count -ne 1) {
    throw "Manifest template must contain exactly one $placeholder placeholder."
}

$manifestText = $template.Replace($placeholder, $VersionNumber)
$manifest = $manifestText | ConvertFrom-Json
if ($manifest.version_number -cne $VersionNumber) {
    throw 'Manifest version replacement failed.'
}

New-Item -ItemType Directory -Force -Path $OutputDirectory | Out-Null
$packagePath = Join-Path $OutputDirectory "DSPSeedScanner-$VersionNumber.zip"

if (Test-Path -LiteralPath $packagePath) {
    Remove-Item -LiteralPath $packagePath -Force
}

$archive = [System.IO.Compression.ZipFile]::Open(
    $packagePath,
    [System.IO.Compression.ZipArchiveMode]::Create
)
try {
    $manifestEntry = $archive.CreateEntry(
        'manifest.json',
        [System.IO.Compression.CompressionLevel]::Optimal
    )
    $manifestStream = $manifestEntry.Open()
    try {
        $encoding = New-Object System.Text.UTF8Encoding($false)
        $writer = New-Object System.IO.StreamWriter(
            $manifestStream,
            $encoding
        )
        try {
            $writer.Write($manifestText)
        }
        finally {
            $writer.Dispose()
        }
    }
    finally {
        $manifestStream.Dispose()
    }

    [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile(
        $archive,
        $ReadmePath,
        'README.md',
        [System.IO.Compression.CompressionLevel]::Optimal
    ) | Out-Null
    [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile(
        $archive,
        $IconPath,
        'icon.png',
        [System.IO.Compression.CompressionLevel]::Optimal
    ) | Out-Null
    foreach ($dllPath in $DllPaths | Sort-Object { Split-Path -Leaf $_ }) {
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile(
            $archive,
            $dllPath,
            ('BepInEx/plugins/DSPSeedScanner/' + (Split-Path -Leaf $dllPath)),
            [System.IO.Compression.CompressionLevel]::Optimal
        ) | Out-Null
    }
}
finally {
    $archive.Dispose()
}

Write-Output "Thunderstore package created: $packagePath"
Write-Output "Package version: $VersionNumber"
