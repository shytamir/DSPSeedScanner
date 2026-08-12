# DSP Seed Scanner

DSP Seed Scanner is a BepInEx mod for evaluating whether one procedurally
generated Dyson Sphere Program cluster suits a player's intended run. It uses
the installed game runtime to generate evidence and returns bounded,
context-specific conclusions rather than a universal seed score.

## Status

This package automatically resolves each completed New Game cluster preview.
A small non-interactive corner panel shows waiting, cache reuse, scan progress,
completion, cancellation, unsupported-runtime, and failure states. The next
presentation phase will add the neutral context conclusions themselves.

## Supported runtime

The current compatibility contract is limited to Dyson Sphere Program
`0.10.34.28529`, galaxy algorithm `20200403`, the recorded Assembly-CSharp and
generation-method identities, ordered themes `1..25`, and BepInEx `5.4.17`.
Other plugins or preloader assemblies are conservatively rejected because
generation compatibility is not established.

## Implemented core

- Immediate preview conclusions cover accepted topology, power, gas-product,
  energy-system, sphere-geometry, Dark Fog, grouping, role, and trait cases.
- The New Game workflow automatically reuses valid complete conclusions or
  runs one cooperative complete-cluster scan at one solid planet per frame.
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

The panel defaults to the bottom-right. Set `Presentation.PanelCorner` to `2`
for bottom-left, `3` for top-left, or `4` for top-right in the generated BepInEx
configuration file. Developer probe invocation and build requirements are
documented in the repository. Batch search, parallel generation, exports,
telemetry, publication automation, and wider runtime compatibility are not
included.
