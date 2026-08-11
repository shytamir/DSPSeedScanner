# DSP Seed Scanner - Project Definition

## Product state

DSP Seed Scanner is in bootstrap. This document establishes the initial
product boundary and architecture before executable code is introduced.

The following decisions remain intentionally open until they are addressed by
a focused implementation task:

- the first supported scan criteria;
- the user-facing way to start, stop, and configure a scan;
- the persisted result format and schema;
- batching, concurrency, and performance targets;
- final package identity, copy, and release publishing.

Open decisions are not implemented behavior.

## Purpose

DSP Seed Scanner will search procedurally generated Dyson Sphere Program star
clusters by asking the installed game runtime to generate them, extracting a
stable model of relevant properties, and evaluating explicit criteria against
that model.

The project exists to make searches across many seeds repeatable and auditable
without maintaining an independent reimplementation of the game's generation
algorithm.

## Product invariants

- The installed DSP runtime is authoritative for cluster generation.
- The same seed, game version, and generation settings produce the same scan
  input and therefore the same evaluation result.
- Scanning does not modify player factories, progression, or save files.
- A result records enough provenance to identify how it was produced.
- Generation, extraction, normalization, evaluation, and reporting remain
  separate responsibilities.
- Scan criteria operate on normalized data rather than mutable game objects.
- Batch scans are bounded, observable, and interruptible.
- A failed or incomplete seed is reported as such and never presented as a
  valid non-match.
- DSP, Unity, and BepInEx assemblies are external dependencies and are not
  redistributed.

## Initial scope

The initial product scope includes:

- a BepInEx-dependent mod loaded by Dyson Sphere Program;
- generation of star clusters for explicitly supplied seeds and settings;
- extraction of the cluster properties required by supported criteria;
- deterministic evaluation of those criteria;
- progress, cancellation, and per-seed failure reporting;
- machine-readable results with game and scanner provenance.

The bootstrap skeleton does not yet choose the first criteria or promise a
specific interface or result schema.

## Outside initial scope

- replacing or reproducing DSP's galaxy-generation algorithm;
- modifying a player's save, technology state, inventory, or factory;
- automating gameplay after a desirable seed is found;
- redistributing game, Unity, or BepInEx binaries;
- treating results from different game versions or generation settings as
  interchangeable;
- general-purpose save analysis unrelated to generated cluster properties.

## Architecture

```text
Scan request
    |
Runtime coordinator
    |
DSP cluster generation
    |
Runtime extraction
    |
Normalized cluster model
    |
Criteria evaluation
    |
Result reporting
```

### Runtime coordinator

Owns scan lifecycle, scheduling, cancellation, progress, and error isolation.
It must respect Unity and DSP thread-affinity requirements. Throughput
optimizations must not assume that game generation APIs are thread-safe.

### DSP cluster generation

Invokes the installed game's generation path with explicit seed and generation
settings. This layer contains game-version-sensitive integration and should be
kept narrow.

### Runtime extraction

Reads only the game objects and fields needed to build the normalized model.
It must not leak mutable runtime objects into evaluation code.

### Normalized cluster model

Represents the stable facts required by supported criteria, independent of
Unity presentation types and transient runtime ownership. Its contract should
be versioned when persisted results depend on it.

### Criteria evaluation

Applies explicit, deterministic rules to normalized data. Criteria should be
composable and testable without launching the game when supplied with known
normalized inputs.

### Result reporting

Emits matches, non-matches when requested, errors, progress, and provenance.
Presentation and serialization must not contain generation or scoring logic.

## Determinism and provenance

At minimum, a persisted result should be attributable to:

- galaxy seed;
- relevant generation settings;
- Dyson Sphere Program version;
- scanner version;
- result schema version once a schema exists;
- criteria and criterion configuration;
- completion or failure state.

Any nondeterministic or environment-dependent input discovered during
implementation must either be controlled or recorded. Comparisons across
different game versions should be explicit rather than silently combined.

## Runtime safety and performance

Scanning may create and discard many clusters in one game session. The
implementation must therefore:

- release per-seed references after evaluation;
- avoid retaining Unity or DSP objects in accumulated results;
- keep work off frame-critical paths where the runtime permits it;
- yield often enough to keep cancellation and progress responsive;
- isolate a seed failure so the remaining batch can continue when safe;
- measure before introducing concurrency or caching.

Performance claims require an in-game benchmark with stated game version,
hardware, settings, seed count, and success or failure totals. A successful
build alone does not validate runtime safety or throughput.

## Toolchain direction

The executable project will target the framework and C# language version
compatible with the selected BepInEx 5 and installed DSP runtime. Local builds
should resolve game and Unity references from an explicit game-root setting,
not from committed binaries.

Exact project files, build commands, dependency versions, and output layout
will be defined with the executable skeleton and then documented in
`README.md`.

The placeholder CI pipeline already establishes the release version mapping:
`VERSION` supplies major and minor, the GitHub Actions run number supplies the
patch, assembly/file versions append `.0`, and diagnostic labels append the
short commit. It also validates the generic Thunderstore archive contract.
See [THUNDERSTORE-PACKAGE.md](THUNDERSTORE-PACKAGE.md). These build artifacts
remain non-installable until a compiled plugin replaces the empty DLL.

## Contracts to establish

The first implementation milestones should deliberately establish and
version, where appropriate:

1. plugin identity and supported runtime versions;
2. scan request and configuration contract;
3. normalized cluster model;
4. criterion interface and initial criteria;
5. result and provenance schema;
6. cancellation, error, and progress behavior;
7. local build and packaging workflow;
8. deterministic and in-game validation procedures.

This list defines decisions to make, not a committed delivery order.

## Definition of the first usable release

The first usable release should:

- load through the selected BepInEx version;
- accept a bounded seed range and explicit generation settings;
- evaluate at least one documented criterion;
- report progress and allow cancellation;
- produce deterministic, machine-readable results with provenance;
- survive per-seed failures without misclassifying them;
- pass focused deterministic tests and an in-game smoke test;
- exclude game and framework binaries from its source and release artifacts.
