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

Fresh-start guidance distinguishes the starter giant's product, adequate
mineable Fire Ice, sibling-moon power and the nearest rich Deuterium supply.
Megafactory guidance identifies qualified rare and plentiful resource systems;
sphere candidates require luminous stars and are selected by home distance.

The statistics panel shows native oil rates, explicit moon order and resource
emphasis. Its notable-star table compares blue giants, the brightest star,
O stars and nearby Aquatica hosts with the requested distance and sphere facts.
See the [statistics presentation contract](docs/specification/STATISTICS-PRESENTATION.md)
for column meanings and color boundaries.

The BepInEx setting `Presentation.PanelCorner` selects the panel corner: `1`
bottom-right (default), `2` bottom-left, `3` top-left, or `4` top-right.

Other BepInEx plugins may be installed alongside DSP Seed Scanner. The scanner
uses the live runtime it receives; generation changes or conflicts introduced
by another plugin may affect its results and are not always detectable.

## Project documentation

[PROJECT.md](docs/PROJECT.md) is the sole authority for project steering and
work status. The [Panel Improvement Roadmap](docs/management/ROADMAP.md)
defines delivery scope, execution restrictions, and validation gates.

The [documentation index](docs/INDEX.md) links specifications, delivery notes,
management documents, and archived scope and evidence.

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
[Panel Improvement Roadmap](docs/management/ROADMAP.md) before changing scope or
behavior.

## License

DSP Seed Scanner is licensed under the [Apache License 2.0](LICENSE).

## Disclaimer

This is an unofficial community project. Dyson Sphere Program and its assets
belong to their respective owners. The game and BepInEx are required but are
not included.
