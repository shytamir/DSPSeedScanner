# Thunderstore Package Contract

The GitHub Actions workflow builds, validates, and retains an installable
presentation-neutral scanner-core package. It does not publish or promote a
Thunderstore release.

## ZIP layout

```text
manifest.json
README.md
icon.png
BepInEx/
  plugins/
    DSPSeedScanner/
      DSPSeedScanner.dll
      DSPSeedScanner.Core.dll
      DSPSeedScanner.Runtime.dll
```

The three files required by Thunderstore are placed at the ZIP root with exact
case. Only the three scanner-owned assemblies are installed under the intended
BepInEx plugin path. DSP, Unity, BepInEx, CI reference, log, probe, and build
files are excluded.

## Version mapping

`VERSION` supplies the manually selected major (`M`) and minor (`m`) values.
The GitHub Actions run number supplies the automatically increasing patch
value (`N`):

```text
Package/plugin version: M.m.N
Semantic version:       M.m.N
Assembly/file version:  M.m.N.0
Diagnostic label:       M.m.N.<short-commit>
```

The initial release line is `0.1`. The same generated semantic version is used
by the manifest and BepInEx plugin attribute; all three scanner assemblies use
the generated four-part assembly/file version. The diagnostic commit suffix is
kept in product metadata and build reports.

## Build and validation

On each push to `main`, and when manually dispatched, the workflow:

1. verifies that it checked out the triggering commit;
2. derives all version classes from `VERSION`, run number, and commit;
3. builds and tests the runtime-neutral core;
4. downloads the declared BepInEx compile reference and builds the real plugin
   against narrow source-defined DSP API shapes used only by the hosted runner;
5. validates non-empty scanner assemblies, synchronized versions, and the
   exact public-build DLL allowlist;
6. builds the Thunderstore ZIP from the real outputs and package assets;
7. validates the exact archive allowlist, root files, portable names, manifest
   shapes, semantic version, README, PNG format and dimensions, and every DLL
   hash; and
8. uploads the ZIP, three scanner DLLs, build information, and validation
   reports as one artifact retained for 30 days.

Validation deliberately avoids product-specific assertions about scanner
features, text, UI, plugin metadata, or final file contents.

## Build prerequisites

- Local plugin builds require the supported DSP installation and BepInEx at
  the `GameRoot` path. The project reads those assemblies as compile inputs and
  never copies them into source control or package output.
- Hosted CI downloads BepInEx `5.4.17` from Thunderstore. Because a hosted
  runner cannot possess the licensed game, `ci/DSPGame.Reference` supplies
  only the source declarations needed to compile; it contains no game logic or
  extracted binary and is never packaged.
- .NET SDK 8 and PowerShell are required by the automated build scripts.

The generic validator intentionally checks package structure and integrity,
not scanner feature claims. Runtime conformance remains governed by
[the scanner conformance record](CONFORMANCE.md).

## Integration boundary

After BepInEx loads `DSPSeedScannerPlugin`, an integration may obtain the
plugin instance through its GUID `io.github.shytamir.dspseedscanner` and call
the public `ScanPreview`, `GenerateRawPlanet`,
`GenerateBirthSystemResources`, `GenerateCompleteClusterResources`, or
`StartCompleteClusterResources` methods. Complete-scan semantic conclusion
bundles may be read, stored, or cleared through `TryGetCachedCompleteCluster`,
`TryStoreCompleteCluster`, and `ClearCompleteClusterCache`. The start method
returns a disposable presentation-neutral operation; each `Advance` call
completes at most one solid planet and restores shared DSP state before
returning. Inputs and outputs are presentation-neutral Core and Runtime
contracts. The approved New Game presentation roadmap consumes these reports
without moving presentation policy into the scanner core. The currently
accepted scanner-core package contains no panel, hooks, controls, or
presentation copy.
