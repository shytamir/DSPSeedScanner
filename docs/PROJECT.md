# DSP Seed Scanner - Project Steering

This document is authoritative for accepted project decisions and current
tracked status. Detailed contracts belong in the specification documents;
completed story scope, evidence, and history belong in archived roadmaps.

**Current status:** FEED-01 and FEED-02 were accepted on 2026-08-13;
`ready-for-new-panel` passed with no unresolved blocker, establishing the
**Clean slate for panel work** milestone. FEED-03 and FEED-04 were then
accepted; `ready-for-panel-population` passed, establishing the **Panel renders
with all features enabled even if not yet consumed** milestone. FEED-05 was
accepted. FEED-06 passed its story gate on 2026-08-14 after correcting hosted
reference linkage, DSP planet-number scope, exact resource retention, and the
home-system table presentation. The owner validated the exact CI artifact;
`ready-for-cluster-panel-population` passed and established the **Panel home
system fully populated** milestone. FEED-07 was accepted on 2026-08-14.
FEED-08 was then accepted. FEED-09 is implemented at its acceptance gate and
awaits owner acceptance. The statistics panel presents every home-system body
in stable game order with distinct body, world, Solar, Wind, ores, oil, and
gas-product fields.
After complete coverage or cache reuse, solid rows show compact exact resource
amounts and group counts without retaining raw planets or cluster resources.
FEED-07 adds at most two nearest planet locations for sulfuric-acid oceans and
six supported rare ores, measured from home to their host system in light-
years. FEED-08 adds one exact, cacheable line for every planet with generated
Unipolar Magnet deposits, including host-system distance, vein nodes, total
amount, and groups.
FEED-09 adds one lightweight-preview statistic for the highest-rate Deuterium
gas giant within 8.125 light-years of home, with deterministic ties and native
four-decimal rate formatting; later stories remain inactive behind their
roadmap gates.
The earlier scanner core, New Game presentation, and presentation refinement
roadmaps were completed and accepted on 2026-08-11 and 2026-08-12. HOTFIX-01 corrected
pathless runtime-assembly fingerprinting, and FSOR-01 completed local runtime-
filesystem hardening on 2026-08-13 without changing identity or conclusion
semantics. TD-004 remains pending evaluation and does not affect ordinary
player behavior.

## Product decision

DSP Seed Scanner helps players decide whether a procedurally generated Dyson
Sphere Program cluster suits the run they intend to play. It presents bounded,
context-specific conclusions and does not define a universally best seed.

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

- The retained decision contexts are fresh start, megafactory, compact
  expansion, and sphere or energy. Dark Fog occupation is shown only as
  neutral status metadata. Redundant decision-relevant traits are not emitted.
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
- Only presentation-ready semantic conclusions derived from a successful
  complete scan are persisted in a versioned, bounded local cache under the
  mod configuration area. Raw or normalized resource evidence, execution
  diagnostics, and rendered wording are not cached.
- The panel presents concise natural-language strengths,
  preference-sensitive results, and limitations without requiring player
  input. Unknown and not-applicable components remain omitted. Named candidates
  must retain evidence-backed attribution; the panel does not expose internal
  identifiers, raw runtime units, or mechanical evidence summaries.
- Its numeric corner setting defaults to `1` for
  bottom-right, then proceeds clockwise as `2` bottom-left, `3` top-left, and
  `4` top-right. Border-center placement is prohibited. The panel shows visible
  activity and terminal failure states. Its translucent scrollable conclusion
  viewport occupies 37% of resolution width and height and groups each player
  context once across the three outcome columns.
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
- Co-installed BepInEx plugins and preloader assemblies do not by themselves
  make the scanner unsupported, including when they alter generation. Their
  inventory and the observed assembly, algorithm, catalogue, and generation-
  method identity remain part of the cache key. Unsupported game versions,
  missing required members, incomplete evidence, and runtime failures still
  fail closed. Plugin interactions are an accepted compatibility risk rather
  than a reason to require an isolated installation.

## Current scope exclusions

The active product scope does not include an independent galaxy generator,
universal seed ranking, subjective quality claims, post-start guarantees,
adaptive panel placement, player scoring or required preferences, manual scan
or retry controls, seed comparison, broad compatibility promises, telemetry,
or publication to an external service. New scope requires an explicit steering
decision and corresponding roadmap change.

## Management and documentation

The active [user-feedback roadmap](management/ROADMAP.md) owns current story
order, phase gates, milestones, and acceptance boundaries. The completed
[presentation refinement roadmap](archive/PRESENTATION-REFINEMENT-ROADMAP.md)
and earlier roadmap records are retained in the [archive](archive/INDEX.md).
The
[technical debt register](management/TECHNICAL-DEBT.md) owns explicitly
deferred obligations and their closure gates; recording debt does not weaken
the accepted safety contract. The completed scanner-core, New Game
presentation, and product-planning roadmaps are retained in the archive. The
[documentation index](INDEX.md) lists all current and archived documents with
their purpose.
