# Thunderstore Package Contract

The GitHub Actions workflow currently builds and validates a placeholder
Thunderstore package. It proves the versioning, archive construction,
validation, and artifact-publication path before a real plugin DLL exists.

The generated package is not a usable mod and must not be uploaded to
Thunderstore.

## ZIP layout

```text
manifest.json
README.md
icon.png
BepInEx/
  plugins/
    DSPSeedScanner/
      DSPSeedScanner.dll
```

The three files required by Thunderstore are placed at the ZIP root with exact
case. The placeholder DLL uses the intended BepInEx installation path.

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

The initial release line is `0.1`. The assembly/file version is generated now
for contract continuity and will be applied to the compiled assembly once the
real plugin project replaces the placeholder DLL.

## Build and validation

On each push to `main`, and when manually dispatched, the workflow:

1. verifies that it checked out the triggering commit;
2. derives all version classes from `VERSION`, run number, and commit;
3. stages the tracked empty DLL as the placeholder build output;
4. builds the Thunderstore ZIP from the manifest template and package assets;
5. validates the required root files, portable entry names, manifest field
   shapes, semantic version, README, PNG format and dimensions, and DLL hash;
6. uploads the ZIP, build information, and validation report as one GitHub
   Actions artifact retained for 30 days.

Validation deliberately avoids product-specific assertions about scanner
features, text, UI, plugin metadata, or final file contents.

## Placeholder inputs

- `packaging/DSPSeedScanner.dll`: intentionally empty stand-in for the future
  compiled plugin.
- `packaging/manifest.template.json`: valid manifest shape with placeholder
  product copy.
- `packaging/README.md`: explicit lorem ipsum package-page placeholder.
- `packaging/icon.png`: mechanically generated placeholder PNG.

These inputs should be replaced, and the empty-DLL allowance removed from the
validator, when the executable project skeleton is introduced.
