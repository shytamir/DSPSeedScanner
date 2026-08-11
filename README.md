# DSP Seed Scanner

DSP Seed Scanner is an upcoming
[Dyson Sphere Program](https://store.steampowered.com/app/1366540/Dyson_Sphere_Program/)
mod for evaluating whether a procedurally generated cluster suits a player's
intended run. It uses the installed game runtime as its generation authority
and reports context-specific conclusions rather than a universal seed score.

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

Contributors should read [AGENTS.md](AGENTS.md), the authoritative
[project steering document](docs/PROJECT.md), and the active
[roadmap](docs/management/ROADMAP.md) before changing scope or behavior.

Release plumbing is described by the separately maintained
[Thunderstore package contract](docs/THUNDERSTORE-PACKAGE.md).

## License

DSP Seed Scanner is licensed under the [Apache License 2.0](LICENSE).

## Disclaimer

This is an unofficial community project. Dyson Sphere Program and its assets
belong to their respective owners. The game and BepInEx are required but are
not included.
