# DSP Seed Scanner

DSP Seed Scanner is a
[Dyson Sphere Program](https://store.steampowered.com/app/1366540/Dyson_Sphere_Program/)
mod for evaluating whether a procedurally generated cluster suits a player's
intended run. It uses the installed game runtime as its generation authority
and reports context-specific conclusions rather than a universal seed score.

The current implementation automatically resolves each completed New Game
cluster preview and shows operational state plus bounded neutral conclusions
grouped for fresh starts, megafactories, Dark Fog farming, compact expansion,
sphere or energy goals, and decision-relevant traits. The compact panel uses
color-coded strength, preference-sensitive, and limitation columns; omits
unknown components; and identifies systems by DSP display name and star type.
Each context spans the three columns under one centered heading on a
transparent surface.

The BepInEx setting `Presentation.PanelCorner` selects the panel corner: `1`
bottom-right (default), `2` bottom-left, `3` top-left, or `4` top-right.

## Project status

[docs/PROJECT.md](docs/PROJECT.md) is the authoritative source for current
project status and steering decisions. Story state, acceptance evidence, and
implementation sequencing are maintained in the active
[roadmap](docs/management/ROADMAP.md).

The [documentation index](docs/INDEX.md) lists the accepted specifications,
delivery notes, management documents, and archived planning material.

## Development

Build and run the focused core checks with:

```powershell
dotnet build DSPSeedScanner.sln --configuration Release
dotnet run `
  --project tests/DSPSeedScanner.Core.Tests/DSPSeedScanner.Core.Tests.csproj `
  --configuration Release --no-build
```

Building the BepInEx plugin additionally requires the supported local Dyson
Sphere Program installation and BepInEx under the `GameRoot` declared in the
plugin project. Hosted CI uses a narrow compile-only API contract and does not
redistribute game assemblies. Package construction, versioning, and the
integration boundary are documented in the
[Thunderstore package contract](docs/THUNDERSTORE-PACKAGE.md).

Contributors should read [AGENTS.md](AGENTS.md), the authoritative
[project steering document](docs/PROJECT.md), and the active
[roadmap](docs/management/ROADMAP.md) before changing scope or behavior.

## License

DSP Seed Scanner is licensed under the [Apache License 2.0](LICENSE).

## Disclaimer

This is an unofficial community project. Dyson Sphere Program and its assets
belong to their respective owners. The game and BepInEx are required but are
not included.
