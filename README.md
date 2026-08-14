# DSP Seed Scanner

DSP Seed Scanner is a
[Dyson Sphere Program](https://store.steampowered.com/app/1366540/Dyson_Sphere_Program/)
mod for evaluating whether a procedurally generated cluster suits a player's
intended run. It uses the installed game runtime as its generation authority
and reports context-specific conclusions rather than a universal seed score.

The current implementation automatically resolves each completed New Game
cluster preview and shows operational state plus bounded neutral conclusions
grouped for fresh starts, megafactories, compact expansion, and sphere or
energy goals. Dark Fog occupation appears as neutral status metadata
rather than a farming judgment. The compact panel uses
color-coded strength, preference-sensitive, and limitation columns; omits
unknown components; and identifies systems by DSP display name and star type.
Each context has one centered heading in a fixed translucent viewport. Long
results scroll inside the panel instead of expanding across the preview, and
sparse contexts share complementary column space. Use the mouse wheel over the
panel to inspect conclusions below the visible viewport.

The BepInEx setting `Presentation.PanelCorner` selects the panel corner: `1`
bottom-right (default), `2` bottom-left, `3` top-left, or `4` top-right.

Other BepInEx plugins may be installed alongside DSP Seed Scanner. The scanner
uses the live runtime it receives; generation changes or conflicts introduced
by another plugin may affect its results and are not always detectable.

## Project status

[docs/PROJECT.md](docs/PROJECT.md) is the authoritative source for current
project status and steering decisions. Active product development follows the
[user-feedback roadmap](docs/management/ROADMAP.md). FEED-01 and FEED-02 were
accepted, establishing a clean slate for panel work. FEED-03 was accepted, and
FEED-04 was accepted, establishing a fully enabled panel scaffold. FEED-05 was
accepted, and FEED-06's exact per-body resource table passed its direct-build
story gate after narrow hosted-linkage, DSP planet-number, statistics, and
presentation corrections. The exact CI artifact passed human validation,
establishing the fully populated home-system panel. FEED-07 was accepted after
adding bounded nearby resource locations to the cluster panel in light-years.
FEED-08 now awaits owner acceptance at its story gate after adding exact per-
planet Unipolar Magnet supply and distribution to that panel. The completed
[presentation refinement roadmap](docs/archive/PRESENTATION-REFINEMENT-ROADMAP.md)
records the preceding stories, acceptance gates, and implementation history.

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
[project steering document](docs/PROJECT.md), and the current
[user-feedback roadmap](docs/management/ROADMAP.md) before changing scope or
behavior.

## License

DSP Seed Scanner is licensed under the [Apache License 2.0](LICENSE).

## Disclaimer

This is an unofficial community project. Dyson Sphere Program and its assets
belong to their respective owners. The game and BepInEx are required but are
not included.
