[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [string]$PackagePath,

    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d+\.\d+\.\d+$')]
    [string]$ExpectedVersion,

    [Parameter(Mandatory = $true)]
    [string[]]$ExpectedDllPaths,

    [string]$RepositoryRoot = (Split-Path -Parent $PSScriptRoot),

    [string]$ReportPath = (
        Join-Path $RepositoryRoot 'artifacts\PACKAGE-REPORT.md'
    )
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem

function Read-ZipText {
    param([System.IO.Compression.ZipArchiveEntry]$Entry)

    $stream = $Entry.Open()
    try {
        $encoding = New-Object System.Text.UTF8Encoding($false, $true)
        $reader = New-Object System.IO.StreamReader(
            $stream,
            $encoding,
            $true
        )
        try {
            return $reader.ReadToEnd()
        }
        finally {
            $reader.Dispose()
        }
    }
    finally {
        $stream.Dispose()
    }
}

if (-not (Test-Path -LiteralPath $PackagePath -PathType Leaf)) {
    throw "Package was not found: $PackagePath"
}
foreach ($expectedDllPath in $ExpectedDllPaths) {
    if (-not (Test-Path -LiteralPath $expectedDllPath -PathType Leaf)) {
        throw "Expected DLL was not found: $expectedDllPath"
    }
}
$expectedDllNames = @(
    'DSPSeedScanner.dll',
    'DSPSeedScanner.Core.dll',
    'DSPSeedScanner.Runtime.dll'
)
$actualDllNames = @($ExpectedDllPaths | ForEach-Object { Split-Path -Leaf $_ })
if ($actualDllNames.Count -ne $expectedDllNames.Count -or
    @($actualDllNames | Where-Object { $expectedDllNames -cnotcontains $_ }).Count -ne 0 -or
    ($actualDllNames | Select-Object -Unique).Count -ne $actualDllNames.Count) {
    throw 'Expected DLL inputs must be exactly the scanner plugin, core, and runtime assemblies.'
}

$requiredRootEntries = @('manifest.json', 'README.md', 'icon.png')
$expectedDllEntries = @(
    'BepInEx/plugins/DSPSeedScanner/DSPSeedScanner.dll',
    'BepInEx/plugins/DSPSeedScanner/DSPSeedScanner.Core.dll',
    'BepInEx/plugins/DSPSeedScanner/DSPSeedScanner.Runtime.dll'
)
$expectedEntries = @($requiredRootEntries) + $expectedDllEntries
$archive = [System.IO.Compression.ZipFile]::OpenRead(
    (Resolve-Path -LiteralPath $PackagePath)
)
try {
    $fileEntries = @(
        $archive.Entries |
            Where-Object { -not $_.FullName.EndsWith('/') }
    )
    $entryNames = @($fileEntries | ForEach-Object {
        $_.FullName.Replace('\', '/')
    })

    if (@($fileEntries | Where-Object {
                $_.FullName.Contains('\')
            }).Count -gt 0) {
        throw 'Package contains non-portable backslash entry names.'
    }
    if (($entryNames | Select-Object -Unique).Count -ne $entryNames.Count) {
        throw 'Package contains duplicate file entries.'
    }
    foreach ($requiredEntry in $requiredRootEntries) {
        if ($entryNames -cnotcontains $requiredEntry) {
            throw "Required root entry is missing or incorrectly cased: $requiredEntry"
        }
    }
    if ($entryNames.Count -ne $expectedEntries.Count -or
        @($entryNames | Where-Object { $expectedEntries -cnotcontains $_ }).Count -ne 0) {
        throw 'Package contains missing or unintended files.'
    }

    $manifestEntry = $fileEntries |
        Where-Object { $_.FullName -ceq 'manifest.json' } |
        Select-Object -First 1
    $manifest = (Read-ZipText -Entry $manifestEntry) | ConvertFrom-Json

    foreach ($field in @(
            'name',
            'version_number',
            'website_url',
            'description',
            'dependencies'
        )) {
        if ($null -eq $manifest.PSObject.Properties[$field]) {
            throw "Manifest is missing required field: $field"
        }
    }
    if ($manifest.name -cnotmatch '^[A-Za-z0-9_]+$') {
        throw 'Manifest name contains unsupported characters.'
    }
    if ($manifest.version_number -cne $ExpectedVersion) {
        throw "Manifest version is invalid: $($manifest.version_number)"
    }
    if ([string]::IsNullOrWhiteSpace($manifest.description) -or
        $manifest.description.Length -gt 250) {
        throw 'Manifest description must contain 1 to 250 characters.'
    }
    if (-not [string]::IsNullOrWhiteSpace($manifest.website_url)) {
        $website = $null
        if (-not [uri]::TryCreate(
                $manifest.website_url,
                [System.UriKind]::Absolute,
                [ref]$website
            ) -or $website.Scheme -notin @('http', 'https')) {
            throw 'Manifest website_url must be empty or an absolute HTTP(S) URL.'
        }
    }
    if ($manifest.dependencies -isnot [System.Array]) {
        throw 'Manifest dependencies must be an array.'
    }
    foreach ($dependency in @($manifest.dependencies)) {
        if ($dependency -cnotmatch '^[A-Za-z0-9_]+-[A-Za-z0-9_]+-\d+\.\d+\.\d+$') {
            throw "Manifest dependency is invalid: $dependency"
        }
    }

    $readmeEntry = $fileEntries |
        Where-Object { $_.FullName -ceq 'README.md' } |
        Select-Object -First 1
    $readme = Read-ZipText -Entry $readmeEntry
    if ([string]::IsNullOrWhiteSpace($readme)) {
        throw 'Package README is empty.'
    }

    Add-Type -AssemblyName System.Drawing
    $iconEntry = $fileEntries |
        Where-Object { $_.FullName -ceq 'icon.png' } |
        Select-Object -First 1
    $iconStream = $iconEntry.Open()
    try {
        $icon = [System.Drawing.Image]::FromStream($iconStream)
        try {
            if ($icon.Width -ne 256 -or $icon.Height -ne 256) {
                throw "Package icon is $($icon.Width)x$($icon.Height); expected 256x256."
            }
            if ($icon.RawFormat.Guid -ne
                [System.Drawing.Imaging.ImageFormat]::Png.Guid) {
                throw 'Package icon is not a PNG image.'
            }
        }
        finally {
            $icon.Dispose()
        }
    }
    finally {
        $iconStream.Dispose()
    }

    foreach ($expectedDllPath in $ExpectedDllPaths) {
        if ((Get-Item -LiteralPath $expectedDllPath).Length -eq 0) {
            throw "Expected DLL is empty: $expectedDllPath"
        }
        $expectedDllHash = (
            Get-FileHash -LiteralPath $expectedDllPath -Algorithm SHA256
        ).Hash.ToLowerInvariant()
        $expectedDllEntry = 'BepInEx/plugins/DSPSeedScanner/' +
            (Split-Path -Leaf $expectedDllPath)
        $dllEntry = $fileEntries |
            Where-Object {
                $_.FullName.Replace('\', '/') -ceq $expectedDllEntry
            } |
            Select-Object -First 1
        if ($null -eq $dllEntry) {
            throw "Packaged DLL is missing: $expectedDllEntry"
        }
        $dllStream = $dllEntry.Open()
        try {
            $sha256 = [System.Security.Cryptography.SHA256]::Create()
            try {
                $packagedDllHash = [BitConverter]::ToString(
                    $sha256.ComputeHash($dllStream)
                ).Replace('-', '').ToLowerInvariant()
            }
            finally {
                $sha256.Dispose()
            }
        }
        finally {
            $dllStream.Dispose()
        }
        if ($packagedDllHash -cne $expectedDllHash) {
            throw "Packaged DLL does not match its build input: $expectedDllEntry"
        }
    }
}
finally {
    $archive.Dispose()
}

$packageHash = (
    Get-FileHash -LiteralPath $PackagePath -Algorithm SHA256
).Hash.ToLowerInvariant()
$packageLength = (Get-Item -LiteralPath $PackagePath).Length
$reportDirectory = Split-Path -Parent $ReportPath
New-Item -ItemType Directory -Force -Path $reportDirectory | Out-Null

@"
# Thunderstore package verification

| Check | Result |
| --- | --- |
| Package | ``$PackagePath`` |
| Version | ``$ExpectedVersion`` |
| Required root files | Passed |
| Manifest format | Passed |
| Semantic version | Passed |
| README | Passed |
| Icon format and dimensions | Passed |
| Exact archive allowlist | Passed |
| Packaged DLL integrity | Passed for all scanner assemblies |
| Size | $packageLength bytes |
| SHA-256 | ``$packageHash`` |
"@ | Set-Content -LiteralPath $ReportPath -Encoding utf8

Write-Output "Thunderstore package verification passed: $ExpectedVersion"
Write-Output "Package SHA-256: $packageHash"
