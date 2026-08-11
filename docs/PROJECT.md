# DSP Seed Scanner - Project Steering

This document is authoritative for accepted project decisions and current
tracked status. Detailed contracts belong in the specification documents;
story scope, evidence, and history belong in the roadmap.

**Current status:** The scanner core roadmap was completed and accepted on
2026-08-11. The New Game presentation roadmap was approved on 2026-08-12.
PRES-01 was accepted, and PRES-02 implemented cooperative complete-cluster
execution and is at its acceptance gate. No player-facing panel or automatic
scan invocation is implemented. Panel anchoring is resolved as a four-corner
configuration with bottom-right as the default. One packaging refinement
remains tracked as non-blocking technical debt.

## Product decision

DSP Seed Scanner will help players decide whether a procedurally generated
Dyson Sphere Program cluster suits the run they intend to play. It will present
bounded, context-specific conclusions and will not define a universally best
seed.

## Accepted steering decisions

### Evidence authority

- The installed Dyson Sphere Program runtime is authoritative for generated
  cluster evidence.
- A seed is meaningful only with its complete generation identity and relevant
  settings.
- Community material and prior tools may identify player questions and test
  cases, but cannot override runtime-confirmed behavior.
- Unsupported compatibility, incomplete coverage, or unavailable evidence
  produces an explicit unknown rather than an approximation.
- Raw vein positions may use deterministic invariant-decimal normalization of
  DSP's single-precision values; preserving the source floating-point bit
  pattern is not required by the active conclusion contract.

The detailed boundaries are maintained in the accepted
[generation identity](specification/GENERATION-IDENTITY.md) and
[runtime evidence feasibility](specification/RUNTIME-EVIDENCE-FEASIBILITY.md)
documents.

### Decision contract

- Conclusions are evaluated automatically across the accepted fresh-start,
  megafactory, Dark Fog farming, compact-expansion, sphere or energy, and
  decision-relevant-trait contexts.
- Neutral outcomes must survive the complete accepted preference range.
  Optional preferences may filter or explain an outcome but cannot create or
  reverse it.
- Components remain independent. Material conflicts remain visible as
  tradeoffs; no global score or hidden weighting may collapse them.
- Unsupported claims remain declined even when adjacent diagnostic facts are
  available.

The accepted semantics and thresholds are maintained in the
[conclusion contract](specification/CONCLUSION-CONTRACT.md) and
[predicate and validation catalogue](specification/PREDICATE-RANGE-VALIDATION.md).

### Delivery boundary

- Each completed New Game cluster-preview load creates exactly one resolution
  attempt for its complete generation identity.
- A valid local cache hit resolves without a new scan. Otherwise the mod
  evaluates immediate preview evidence and automatically runs at most one
  bounded full raw scan for that preview load.
- Replaced or exited previews cancel obsolete work at a safe boundary, and a
  stale result can never update the current panel.
- Only successful complete results are persisted in a versioned, bounded local
  cache under the mod configuration area.
- The panel presents immediate and complete neutral conclusions without
  requiring player input. Its numeric corner setting defaults to `1` for
  bottom-right, then proceeds clockwise as `2` bottom-left, `3` top-left, and
  `4` top-right. Border-center placement is prohibited. The panel shows visible
  activity and terminal failure states.
- The project resolves one current generation identity at a time. Batch search,
  parallel generation, unattended databases, shared caches, and exports
  require later steering decisions.

### Safety and responsibility boundaries

- Scanning must not modify player saves, factories, progression, or persistent
  game state.
- Generation, runtime extraction, normalization, evaluation, orchestration,
  and presentation remain separate responsibilities.
- Long-running work must be bounded, observable, cancellable at safe
  boundaries, and attributable to its seed and stage.
- DSP, Unity, and BepInEx assemblies remain external dependencies and are not
  redistributed.

## Current scope exclusions

The active product scope does not include an independent galaxy generator,
universal seed ranking, subjective quality claims, post-start guarantees,
adaptive panel placement, player scoring or required preferences, manual scan
or retry controls, seed comparison, broad compatibility promises, telemetry,
or publication to an external service. New scope requires an explicit steering
decision and corresponding roadmap change.

## Management and documentation

The active [New Game presentation roadmap](management/ROADMAP.md) owns story
scope, acceptance evidence, sequencing, and history. The
[technical debt register](management/TECHNICAL-DEBT.md) owns explicitly
deferred obligations and their closure gates; recording debt does not weaken
the accepted safety contract. The completed scanner-core and product-planning
roadmaps are retained in the [archive](archive/INDEX.md). The
[documentation index](INDEX.md) lists all current and archived documents with
their purpose.
