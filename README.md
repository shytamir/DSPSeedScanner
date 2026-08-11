# DSP Seed Scanner

DSP Seed Scanner is an upcoming mod for
[Dyson Sphere Program](https://store.steampowered.com/app/1366540/Dyson_Sphere_Program/)
that will help players decide whether a procedurally generated star cluster
suits the run they intend to play.

The installed game runtime will remain the source of truth. The scanner will
generate candidate clusters, normalize faithfully reproducible evidence, and
draw context-specific conclusions without modifying player saves or factory
state. It will not claim that one seed is universally best.

## Project status

The product specification is complete. The scanner core implementation roadmap
is active; IMPL-01 is completed and pending acceptance, and no user story is
currently active.

See [docs/PROJECT.md](docs/PROJECT.md) for the product definition, the completed
[specification roadmap](docs/management/ROADMAP.md), and the active
[implementation roadmap](docs/management/IMPLEMENTATION-ROADMAP.md). The
accepted [implementation-planning
boundary](docs/specification/IMPLEMENTATION-PLANNING-BOUNDARY.md) records the
initial scope, runtime gates, validation obligations, and deferrals.

## Planned shape

```text
Complete generation identity
    -> DSP runtime generation
    -> normalized cluster evidence
    -> context-specific interpretation
    -> decision conclusions
    -> future New Game presentation
```

The initial implementation is expected to be a BepInEx-dependent C# plugin.
Dyson Sphere Program, BepInEx, and the game-provided Unity assemblies will be
local runtime or build dependencies; they will not be redistributed here.

## Development

Implementation work is ordered by the scanner core roadmap. IMPL-01 established
a runtime-independent normalized evidence and conclusion-report boundary for
the shared-satellite predicate. The repository does not yet contain a usable
plugin or any DSP/BepInEx runtime integration.

Build and run the focused core checks with:

```powershell
dotnet build DSPSeedScanner.sln --configuration Release
dotnet run --project tests/DSPSeedScanner.Core.Tests/DSPSeedScanner.Core.Tests.csproj --configuration Release --no-build
```

The existing GitHub Actions workflow packages an intentionally empty DLL solely
to validate release plumbing; it does not produce a usable mod. See
[docs/THUNDERSTORE-PACKAGE.md](docs/THUNDERSTORE-PACKAGE.md) for that contract.
Contributors should read [AGENTS.md](AGENTS.md),
[docs/PROJECT.md](docs/PROJECT.md), and the current roadmap before changing
scope or behavior.

## Repository layout

```text
.
|-- AGENTS.md
|-- .github/workflows/build.yml
|-- DSPSeedScanner.sln
|-- docs/
|   |-- management/IMPLEMENTATION-ROADMAP.md
|   |-- management/ROADMAP.md
|   |-- specification/
|   |-- PROJECT.md
|   `-- THUNDERSTORE-PACKAGE.md
|-- packaging/
|-- scripts/
|-- src/DSPSeedScanner.Core/
|-- tests/DSPSeedScanner.Core.Tests/
|-- VERSION
|-- LICENSE
`-- README.md
```

## Safety and data handling

The scanner is intended to operate independently of player progression and
must not alter saves, factories, or game state beyond the transient runtime
work required to generate a cluster. Generated scan output and local game
assemblies should not be committed.

## License

DSP Seed Scanner is licensed under the [Apache License 2.0](LICENSE).

## Disclaimer

This is an unofficial community project. Dyson Sphere Program and its assets
belong to their respective owners. The game and BepInEx are required but are
not included.
