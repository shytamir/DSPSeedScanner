# Implementation-Planning Boundary

**Status:** Candidate produced by SPEC-07; pending product acceptance.

This handoff prepares closure of product specification without creating
implementation stories. It identifies the smallest useful product contract,
the constraints engineering must preserve, and the evidence required before
runtime-dependent code can be treated as production-ready.

## Readiness decision

The accepted technical baseline is sufficient for a separate implementation
roadmap, subject to acceptance of this handoff. Runtime probes demonstrated
that the intended BepInEx boundary can generate the required preview and raw
evidence. The accepted contracts define deterministic identity, evidence
stages, neutral outcomes, predicates, ranges, validation fixtures, and declined
claims.

This is not approval to implement from this roadmap. Acceptance of this
handoff closes the specification phase and authorizes planning the engineering
work in a new roadmap.

## Normative baseline

Implementation planning must use these artifacts together:

| Authority | Accepted artifact | Governs |
| --- | --- | --- |
| Product | [Project definition](../PROJECT.md) | Purpose, scope, invariants, and architecture |
| Identity | [Generation identity](GENERATION-IDENTITY.md) | Reproduction keys, compatibility, and rejection behavior |
| Evidence | [Runtime evidence feasibility](RUNTIME-EVIDENCE-FEASIBILITY.md) | Runtime sources, stages, costs, and unsupported facts |
| Meaning | [Conclusion contract](CONCLUSION-CONTRACT.md) `0.1.0` | Contexts, outcome semantics, neutrality, and declined claims |
| Evaluation | [Predicate, range, and validation catalogue](PREDICATE-RANGE-VALIDATION.md) `0.1.0` | Predicates, thresholds, fixtures, settings limits, and expected outcomes |

The player taxonomy and decision matrix remain rationale and traceability.
They do not override the narrower accepted conclusion and predicate contracts.
If normative artifacts appear to conflict, implementation stops for a contract
decision rather than selecting the most convenient interpretation.

## Minimum initial product contract

The first implementation target evaluates one explicitly requested generation
identity at a time. It is not an exhaustive seed search, background database,
or parallel generator.

Its context set is exactly the six accepted lenses: fresh start, megafactory,
Dark Fog farming, compact expansion, sphere showcase or energy focus, and
decision-relevant traits. No speedrun, difficulty, theme, or universal-quality
profile is implied. Only the active components below can produce a positive,
negative, caution, preference-sensitive, or tradeoff result.

### Immediate preview surface

After a complete compatible galaxy snapshot, the core must evaluate every
applicable component of:

- `FS-TOPOLOGY`: shared birth-giant satellites;
- `FS-POWER`: birth-system tidal lock, solar, and wind components;
- `FS-GAS-ROUTE`: hydrogen, deuterium, and fire-ice product presence;
- `MF-ENERGY-SYSTEM`: cluster energy output and leader separation;
- `MF-SPHERE-GEOMETRY`: maximum radius and orbital containment;
- `MF-SYSTEM-ROLE`: only roles inherited from stable upstream components;
- `DF-OCCUPATION`: initial-hive opportunity and birth-system exposure;
- `CX-GROUPING`: distance between independently supported roles; and
- `TRAIT-SUMMARY`: only the accepted derived trait registry.

These components must be evaluated independently. A family may return several
component outcomes; no score, grade, hidden weight, or context-free verdict is
permitted.

### Explicit on-demand surface

Only an explicit request may start raw planet generation. The first on-demand
scope contains:

- `FS-RESOURCES`: birth-system common totals, per-resource amounts and groups,
  and fire-ice presence; and
- `RR-ACCESS.distance`: complete-cluster rare-resource presence and distance
  from the birth system.

The request declares its raw coverage before work begins. Progress and
cancellation occur at safe runtime boundaries, at minimum between planets.
Partial coverage never produces a complete-scope conclusion.
Roles, grouping, and trait summaries are evaluated again in a new attributed
report after eligible on-demand evidence becomes complete; raw evidence never
silently changes an earlier preview report.

### Required unknowns and setting limits

The core report retains, rather than guesses around:

- `FS-GAS-ROUTE.rate` as **unknown**;
- `MF-RESOURCE-SCOPE` strength and concentration as **unknown**;
- `RR-ACCESS.amount` as **unknown**;
- starter amount conclusions outside resource multiplier `1` as **unknown**;
- quantitative conclusions outside their accepted 64-star reference identity
  as **unknown** unless a later accepted range covers that identity;
- Dark Fog opportunity outside the accepted default combat settings as
  **unknown**, and the entire family as **not applicable** in peace mode; and
- any dependent conclusion as **unknown** when compatibility, source evidence,
  or declared coverage is incomplete.

Fixed predicates may remain eligible when their complete inputs are supported
and their definition is not settings-specific. This does not permit reuse of a
quantitative range under different settings.

## Resolved boundary questions

| Apparent tension | Planning resolution |
| --- | --- |
| Twelve semantic families versus deferred components | Retain the semantic family and explicit unknown state; implement only accepted active predicates. |
| Immediate New Game value versus expensive exact resources | Preview evaluation is automatic; raw generation is a separate on-demand operation. |
| Automatic context evaluation versus optional player preferences | Evaluate every applicable active component neutrally. Preferences may later filter or explain but never create or reverse an outcome. |
| Cluster scanning versus bounded initial scope | Process one requested identity per operation. Batch search and parallel generation require later planning. |
| Supported raw facts versus unsafe runtime mutation | Generate only isolated candidate objects, serialize access to DSP generation state, and restore shared state on every exit path. |
| Accepted predicates versus other star counts or settings | Preserve exact facts and identity, but return unknown for a quantitative conclusion without a matching accepted range. |
| Cross-machine reproducibility not established | Make no shared-cache or byte-identical cross-machine guarantee. Initial results are local and carry full provenance. |
| Future panel versus current core contract | Produce presentation-neutral conclusions and diagnostics. No UI hook, layout, or copy is part of this handoff. |

No contradiction requires reopening an accepted specification artifact.

## Required architecture boundaries

Implementation planning must keep these responsibilities independently
testable:

```text
Generation identity and compatibility gate
    -> serialized DSP runtime generation
    -> immutable normalized evidence with coverage
    -> versioned predicate evaluation
    -> context-attributed conclusion report
    -> future presentation
```

- The compatibility gate rejects an unsupported runtime before evaluation.
- The runtime adapter owns DSP objects, static-state restoration, and raw-data
  cleanup; downstream layers receive normalized values, not live game objects.
- Evidence records carry generation identity, source stage, declared and
  completed coverage, units, and compatibility status.
- Evaluators are deterministic pure logic over normalized evidence plus the
  accepted definition version.
- Reports preserve component outcomes, decisive facts, subjects, tradeoffs,
  unknown causes, and not-applicable reasons.
- Orchestration owns serialization, cancellation, progress, and per-seed
  failure attribution. It does not own scoring rules.

[DSPSeedDatabase](https://github.com/SuperB3333/DSPSeedDatabase) informed the
pipeline separation, [DSPSeedSearch](https://github.com/HoneyTauOverTwo/DSPSeedSearch)
informed bounded operational control, and
[dsp-csv-gen](https://github.com/GreyHak/dsp-csv-gen) informed the narrow game
runtime extraction boundary. No independent generator, database architecture,
UI behavior, or third-party threshold is adopted from them.

## Runtime probes required during implementation

Pure identity, evidence, outcome, and evaluator models can be implemented from
the accepted contracts. The following gates must pass before their associated
runtime path is considered production-ready:

| Gate | Required evidence | Blocks |
| --- | --- | --- |
| Compatibility fingerprint | Capture the full DSP version, galaxy algorithm, assembly identity, ordered theme IDs, scanner contract versions, and loaded-mod compatibility status; reject a deliberate mismatch or uncertain generation-altering patch | Any non-unknown runtime result |
| Lifecycle and thread affinity | From the intended BepInEx lifecycle, prove the supported generation call sequence and thread, serialize generation, restore shared `GameDesc` and related state after success, failure, and cancellation, and show active save/factory state is untouched | Preview and raw runtime adapters |
| Reachable raw algorithms | For every solid-planet `algoId` reachable through the supported ordered theme catalogue, exercise DSP's selected raw path with required initialization, including `RandomTable.Init()`, and record explicit failures | On-demand raw evidence |
| Full-cluster repeat and cleanup | Repeat at least one complete rare-access fixture in independent game processes; compare normalized evidence exactly, cancel a second run between planets, and verify no partial result or retained candidate objects cross the operation boundary | Complete-cluster `RR-ACCESS` |
| Derivation parity | Reconfirm light-year conversion and maximum-shell rounding against the supported runtime using accepted boundary fixtures | Distance and sphere conclusions |
| Compatibility failure paths | Simulate unavailable members, unknown enum values, changed catalogue identity, and raw-stage failure; confirm dependent unknowns and seed/stage diagnostics | Release readiness |

These are focused compatibility and safety probes, not exhaustive benchmarks.
Broader performance tuning begins only after the runtime path is correct. The
implementation roadmap should nevertheless set measured per-operation bounds
before enabling raw generation for users.

## Validation obligations

An implementation is not conformant until it demonstrates:

1. exact identity equality and stage-specific cache keys;
2. pure evaluator tests for every accepted SPEC-06 positive, negative,
   endpoint, preference-sensitive, tradeoff, unknown, and not-applicable case;
3. component independence, including failures for hidden weighting and
   prohibited proxies;
4. independent-process repeatability for representative preview, birth-system
   raw, and complete-cluster raw fixtures;
5. cancellation, failure, and partial-coverage propagation without fabricated
   conclusions;
6. no mutation of active save, factory, progression, or New Game settings;
7. deterministic subject and evidence attribution in every report; and
8. successful build and generic package validation without redistributing DSP,
   Unity, or BepInEx assemblies.

Compilation and pure tests do not replace the in-game runtime checks. Raw
probe output, copied assemblies, scan dumps, and generated package contents
remain outside source control except for the existing intentional dummy
packaging fixture until a later implementation story replaces it.

## Deferred planning inputs

The implementation roadmap must not silently absorb:

- New Game panel design or hooking, presentation copy, styling, or interaction;
- batch seed search, parallel generation, unattended databases, or exports;
- support for additional DSP builds, galaxy algorithms, theme catalogues, star
  counts, resource ranges, or combat-setting ranges;
- gas-rate, cluster-resource-strength, or rare-abundance predicates;
- cross-machine result equivalence or shared persistent caches;
- speedrun routing, buildable-area claims, logistics throughput, receiver
  output, Dark Fog farm yield, aesthetics, novelty, or universal ranking; or
- generalized compatibility with generation-altering mods.

Each deferred item requires an explicit later story and, where it changes a
conclusion, the contract versioning process.

## Inputs for the implementation roadmap

A separate roadmap should order work by dependency, without treating this list
as pre-approved stories:

1. contract models and pure predicate evaluation;
2. compatibility identity and normalized preview extraction;
3. serialized preview orchestration and conclusion reporting;
4. isolated on-demand raw generation with progress, cancellation, and cleanup;
5. runtime conformance and package replacement; and
6. only then a separate presentation roadmap for the inactive New Game story.

The first vertical proof should use one accepted preview fixture end to end.
The first raw proof should remain a developer-invoked harness until the raw
runtime gates pass. This sequencing limits risk without prescribing classes,
frameworks, schedules, or estimates.

## SPEC-07 acceptance

Acceptance confirms that:

1. the normative baseline contains no unresolved product contradiction;
2. the minimum active and explicitly unknown conclusion set is adequate;
3. the architecture and runtime gates are sufficient implementation
   constraints;
4. the deferred list is outside the initial implementation plan; and
5. implementation work will be planned in a separate roadmap.

Acceptance without semantic change closes the product-specification phase.
