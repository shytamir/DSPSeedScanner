# DSP Seed Scanner - Project Steering

This document is authoritative for accepted project decisions and current
tracked status. Detailed contracts belong in the specification documents;
story scope, evidence, and history belong in the roadmap.

**Current status:** The scanner core roadmap is active. IMPL-04 is completed
and pending acceptance. The two runtime-validation debts accepted with
IMPL-03 remain deferred to the IMPL-08 conformance gate. No user story is
active.

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

- Low-cost preview conclusions are eligible for immediate evaluation.
- Exact terrain and resource conclusions require explicit on-demand work.
- Reports remain presentation-neutral. A New Game selection panel is future
  planning work and is not part of the active scanner-core roadmap.
- The project evaluates one explicitly requested generation identity at a time.
  Batch search, parallel generation, unattended databases, and exports require
  later steering decisions.

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
player-facing UI, broad compatibility promises, or publication to an external
service. New scope requires an explicit steering decision and corresponding
roadmap change.

## Management and documentation

The active [scanner core roadmap](management/ROADMAP.md) owns story scope,
acceptance evidence, sequencing, and history. The
[technical debt register](management/TECHNICAL-DEBT.md) owns explicitly
deferred obligations and their closure gates; recording debt does not weaken
the accepted safety contract. The completed product planning roadmap is
retained in the [archive](archive/PLANNING-ROADMAP.md). The [documentation
index](INDEX.md) lists all current and archived documents with their purpose.
