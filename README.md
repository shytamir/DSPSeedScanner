# DSP Seed Scanner

DSP Seed Scanner is an upcoming mod for
[Dyson Sphere Program](https://store.steampowered.com/app/1366540/Dyson_Sphere_Program/)
that will use the game's own runtime to generate and inspect procedurally
generated star clusters.

The project is intended for repeatable, automated searches across many galaxy
seeds. Runtime generation remains the source of truth; the scanner will
normalize relevant cluster data, evaluate explicit criteria, and report
matching seeds without modifying player saves or factory state.

## Project status

The repository is in its bootstrap phase. The product contract and engineering
boundaries are documented, but the mod, scan criteria, command surface, result
format, and packaging workflow have not yet been implemented.

See [docs/PROJECT.md](docs/PROJECT.md) for the current scope and architecture.

## Planned shape

```text
Scan request
    -> DSP runtime generation
    -> normalized cluster data
    -> criteria evaluation
    -> scan results
```

The initial implementation is expected to be a BepInEx-dependent C# plugin.
Dyson Sphere Program, BepInEx, and the game-provided Unity assemblies will be
local runtime or build dependencies; they will not be redistributed here.

## Development

Development and build instructions will be added with the first executable
project skeleton. Until then, contributors should read [AGENTS.md](AGENTS.md)
and [docs/PROJECT.md](docs/PROJECT.md) before making structural or behavioral
changes.

## Repository layout

```text
.
|-- AGENTS.md
|-- docs/
|   `-- PROJECT.md
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
