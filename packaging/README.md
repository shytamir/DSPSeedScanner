# DSP Seed Scanner

DSP Seed Scanner is a BepInEx mod core for evaluating whether one procedurally
generated Dyson Sphere Program cluster suits a player's intended run. It uses
the installed game runtime to generate evidence and returns bounded,
context-specific conclusions rather than a universal seed score.

## Status

This package contains the conformant presentation-neutral scanner core. It has
no player-facing panel, controls, or automatic scan trigger yet. Installation
is useful for integration development and the documented developer probes;
ordinary players will not see an interface.

## Supported runtime

The current compatibility contract is limited to Dyson Sphere Program
`0.10.34.28529`, galaxy algorithm `20200403`, the recorded Assembly-CSharp and
generation-method identities, ordered themes `1..25`, and BepInEx `5.4.17`.
Other plugins or preloader assemblies are conservatively rejected because
generation compatibility is not established.

## Implemented core

- Immediate preview conclusions cover accepted topology, power, gas-product,
  energy-system, sphere-geometry, Dark Fog, grouping, role, and trait cases.
- Explicit on-demand operations return exact birth-system common resources
  and complete-cluster rare-resource access.
- Unsupported settings or evidence remain explicit unknown or not-applicable
  results. No score or hidden weighting is produced.
- Generation is serialized, cancellable at safe boundaries, and limited to
  one requested 64-star identity; a complete-cluster operation rejects more
  than 256 solid planets before raw generation.

## Install

Extract the archive into the Dyson Sphere Program directory. The three scanner
assemblies belong under:

```text
BepInEx/plugins/DSPSeedScanner/
```

The public report-returning methods on `DSPSeedScannerPlugin` are the
integration boundary for a separately reviewed future New Game presentation
modification. Developer probe invocation and build requirements are documented
in the repository. Batch search, parallel generation, persistence, exports,
telemetry, publication automation, and wider runtime compatibility are not
included.
