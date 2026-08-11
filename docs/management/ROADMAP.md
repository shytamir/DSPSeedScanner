# Scanner Core Roadmap

**Status:** Active; accepted for execution on 2026-08-11.

**Active user story:** None. IMPL-01 through IMPL-04 are accepted; IMPL-05 is
completed and pending acceptance. IMPL-03 carried two explicit
runtime-validation debts into the IMPL-08 conformance gate.

This roadmap turns the accepted product specification into a trustworthy,
presentation-neutral scanner core. It selects stories by the usable capability
they deliver, while respecting the dependency order and runtime gates in the
[implementation-planning boundary](../specification/IMPLEMENTATION-PLANNING-BOUNDARY.md).

Acceptance of this roadmap does not activate its first story. Each story must
be activated explicitly, completed with its stated evidence, reviewed, and
accepted before a dependent story begins.

## Product return

On completion, the repository will contain an installable BepInEx scanner core
that can safely evaluate one requested DSP generation identity, produce all
accepted immediate preview conclusions, generate exact starter-resource and
rare-access conclusions on demand, and refuse or qualify every unsupported
case without modifying the player's game state.

The core will expose presentation-neutral reports for a later New Game panel.
This roadmap does not build that panel or any substitute interface.

## Governing constraints

Every story inherits the accepted project definition and specification. In
particular:

- the installed DSP runtime is the only generation authority;
- the complete generation identity and evidence stage accompany every result;
- runtime generation is serialized and isolated from active save and factory
  state;
- normalized evidence contains no retained live DSP objects;
- predicates are deterministic, versioned, and independent;
- partial, incompatible, cancelled, or unsupported evidence becomes explicit
  **unknown**, never an approximation;
- preview work is automatic only for one requested identity, while exact raw
  work requires an explicit operation;
- no global score or universal best-seed conclusion is permitted; and
- DSP, Unity, and BepInEx assemblies remain external dependencies.

Implementation should use the smallest direct design that preserves the
accepted architecture. New abstractions, compatibility layers, persistence,
and extension systems require demonstrated scope within an active story.

## Story standards

An implementation story is complete only when:

- its player or maintainer return works end to end at the declared boundary;
- focused automated tests cover deterministic code and accepted fixtures;
- required in-game probes were actually performed and recorded;
- failure, cancellation, compatibility, and coverage behavior matches the
  accepted contracts;
- the affected project builds and the repository contains no generated output,
  copied game assemblies, scan dumps, or probe artifacts;
- user and contributor documentation states what is implemented and what is
  still unavailable; and
- the final change contains no work assigned to a later story.

Compilation alone does not satisfy a runtime acceptance criterion.

Each story below introduces one principal behavior or runtime risk. If its
acceptance criteria require a second independent capability, the roadmap should
be revised before activation rather than widening the active story silently.

## Phase 1 - Make the decision contract executable

### IMPL-01: Prove the normalized conclusion boundary

**State:** Accepted on 2026-08-11.

As a maintainer building decision support, I want one accepted conclusion to
flow through normalized evidence into an attributed report so that the core
architecture is proven before the full rule catalogue is implemented.

**Delivered:** A runtime-independent vertical slice evaluated
`FS-TOPOLOGY.shared-satellites` from fixtures and returned deterministic
positive, negative, and unknown reports.

**Implemented:**

- buildable core and test projects without DSP or BepInEx references;
- immutable models for generation identity, evidence stage and coverage,
  subject, decisive fact, component outcome, diagnostic cause, and report;
- exact identity equality and stage-specific key semantics without a
  persistent cache;
- definition and contract version attribution;
- the shared-satellite predicate only; and
- focused equality, coverage, deterministic-report, and predicate tests.

**Acceptance evidence:**

- accepted supporting seed `16315224` and non-supporting seed `73339583`
  produced the expected reports;
- missing topology coverage produced unknown with its cause;
- equal inputs produced equal reports and unequal stages produced unequal keys;
- reports carried identity, stage, coverage, subject, decisive evidence,
  outcome, and contract versions;
- reflection confirmed that the core assembly referenced no DSP, Unity, or
  BepInEx assembly; and
- the Release build completed with zero warnings and all seven focused checks
  passed.

**Produced:** `DSPSeedScanner.Core`, a dependency-free executable test harness,
and CI build/test steps. No in-game probe was required for this
runtime-independent story.

**Excluded:** Every other predicate, tradeoff composition, role and trait
derivation, DSP runtime access, orchestration, persistence, UI, and packaging.

### IMPL-02: Complete the accepted conclusion engine

**State:** Accepted on 2026-08-11.

As a maintainer extending the proven decision boundary, I want the complete
accepted predicate catalogue executable against normalized fixtures so that
runtime integration cannot introduce policy or hidden scoring.

**Delivered:** A pure engine evaluated every active definition `0.1.0`
component and every mandatory unknown or not-applicable case without loading
DSP.

**Implemented:**

- immutable normalized cluster, system, gas, starter-resource, rare-resource,
  distance, settings, compatibility, and coverage inputs;
- every fixed and quantitative predicate, endpoint direction, starter amount
  and group range, and definition `0.1.0` settings boundary;
- independent component outcomes, Dark Fog caution and tradeoff composition,
  supported-role derivation, compact grouping, and the accepted trait registry;
- explicit dependent unknowns, peace-mode not-applicable results, and retained
  diagnostic facts outside an accepted range; and
- deterministic context-attributed reports carrying identity, settings, stage,
  coverage, subject, units, source conclusion, and contract versions.

**Acceptance evidence:**

- all named SPEC-06 fixtures and every range direction and endpoint produced
  the expected positive, negative, preference-sensitive, caution, tradeoff,
  unknown, or not-applicable outcome;
- resource-multiplier, combat-setting, star-count, partial-coverage, and
  compatibility cases preserved exact facts and affected only dependent
  components;
- sensitive and unknown evidence created no supported role or trait, while all
  accepted supported roles and traits retained their source conclusions;
- separate resource amount and group results demonstrated that components did
  not cancel, rank, weight, or aggregate one another;
- equal complete normalized inputs produced equal ordered reports; and
- the Release build completed with zero warnings and all 14 focused checks
  passed, including reflection checks for scoring APIs and DSP, Unity, or
  BepInEx references.

**Produced:** The complete runtime-independent conclusion definition, immutable
normalized evidence contract, pure conclusion engine, and expanded executable
fixture suite. No in-game probe was required for this runtime-independent
story.

**Excluded:** DSP or BepInEx references, runtime extraction, threading,
progress, persistent storage, package replacement, and predicates or ranges
outside definition `0.1.0`.

## Phase 2 - Establish and use the safe preview boundary

### IMPL-03: Establish the compatible runtime boundary

**State:** Accepted on 2026-08-11 with deferred technical debt.

As a player relying on generated evidence, I want the scanner to recognize its
exact runtime and isolate a preview request from game state so that unsupported
or unsafe environments cannot produce apparently valid conclusions.

**Delivered:** A developer-invoked BepInEx operation captured the supported
runtime identity, generated one isolated preview, normalized birth topology,
and returned the existing shared-satellite conclusion without retaining DSP
objects.

**Implemented:**

- a game-independent serialized coordinator and a thin locally referenced
  BepInEx/DSP adapter;
- an exact fingerprint for DSP version, galaxy algorithm, Assembly-CSharp
  SHA-256, ordered themes, scanner contract versions, required members, and
  conservative loaded-plugin status;
- main-thread enforcement, busy rejection, safe-boundary cancellation,
  seed-and-stage failures, raw unknown-enum diagnostics, and ordered call
  traces;
- scoped capture and restoration of New Game, active game, galaxy, factory,
  history, statistics, and player references on every post-capture exit; and
- minimal `UniverseGen.CreateGalaxy` topology extraction with guaranteed
  `GalaxyData.Free` cleanup and presentation-neutral output.

**Acceptance evidence:**

- isolated DSP `0.10.34.28529` generated seed `16315224` with algorithm
  `20200403`, the accepted assembly hash, themes `1..25`, and no other loaded
  plugin; it returned `FS-TOPOLOGY.shared-satellites = Supports` from fact `3`;
- the in-game trace recorded fingerprinting, compatibility, state capture,
  `UniverseGen.CreateGalaxy` on managed thread `1`, extraction,
  `GalaxyData.Free`, evaluation, and successful restoration;
- focused tests rejected deliberate assembly, theme-order, version,
  algorithm, required-member, loaded-generation-mod, thread, and raw-enum
  incompatibilities before a valid conclusion could escape;
- success, injected failure, cancellation, and re-entrant busy paths restored
  their captured state in the coordinator harness; and
- the Release solution and local plugin builds completed with zero warnings,
  all 14 conclusion checks passed, all 9 runtime-boundary checks passed, and
  assembly/reference inspection found no retained DSP objects or redistributed
  external assemblies.

**Produced:** `DSPSeedScanner.Runtime`, its executable acceptance harness, the
initial `DSPSeedScanner.Plugin` project, and CI coverage for the runtime-neutral
boundary. The plugin still requires local DSP and BepInEx references and is not
the packaged artifact.

**Deferred debt:** Acceptance allowed implementation to proceed without more
game execution at this stage. [TD-001](TECHNICAL-DEBT.md#td-001-complete-non-success-runtime-isolation-probes)
retains the missing in-game failure, cancellation, and busy isolation proof;
[TD-002](TECHNICAL-DEBT.md#td-002-detect-preloader-and-in-memory-generation-patch-uncertainty)
retains conservative preloader and in-memory patch detection. Both debts are
release-blocking and must close by IMPL-08 acceptance.

**Excluded:** Broad preview extraction, quantitative derivations, raw planet
generation, batch or parallel scanning, another runtime identity,
player-facing invocation, persistence, and packaging replacement.

### IMPL-04: Return all immediate preview conclusions

**State:** Accepted on 2026-08-11.

As a player considering a generated cluster, I want all supported immediate
conclusions from one safe preview request so that I receive useful decision
evidence without starting expensive raw generation.

**Delivered:** The developer-invoked BepInEx operation captured a complete
compatible 64-system preview and returned every applicable definition `0.1.0`
preview conclusion in one deterministic attributed report.

**Implemented:**

- immutable normalized system evidence for birth topology, tidal locking,
  solar and wind ratios, gas products and diagnostic rates, Dyson luminosity,
  maximum shell radius, orbit containment, and initial hive counts;
- complete normalized pairwise system distances using `GalaxyData.LY`, plus
  the accepted runtime shell-radius rounding formula;
- actual generated-star-count output and fail-closed checks for omitted systems
  or pairwise distances;
- complete preview evaluation through the existing neutral engine, including
  roles, grouping, traits, Dark Fog caution and tradeoff, and deferred raw
  unknowns without a preview proxy;
- faithful resource multiplier, peace mode, initial-colonization, and maximum-
  density request settings, with non-reference quantitative settings retained
  as explicit unknowns; and
- deterministic multi-seed probe output containing fingerprint, result,
  report attribution, and ordered call trace records.

**Acceptance evidence:**

- two independent isolated DSP processes evaluated the 20 unique accepted
  preview fixture seeds and produced byte-identical output with SHA-256
  `CDD47CDF2142FBBD494EB19DE108A93142FEA38E0667712873492233EB59A969`;
- all requests returned success, 64 generated systems, restored state, and
  13,223 deterministic report rows across the fixture set;
- the accepted topology, tidal, solar, gas-product, energy output and
  separation, sphere radius and containment, Dark Fog opportunity and
  tradeoff, grouping, role, and trait expectations all matched;
- shell fixtures reproduced `76,200`, `191,400`, and `234,200` radius units,
  while the starter-to-energy distances rounded to the accepted `2.274181`,
  `4.621132`, and `19.521508` light-year fixtures;
- traces contained `UniverseGen.CreateGalaxy`, preview extraction,
  normalization, `GalaxyData.Free`, evaluation, and restoration, with no raw
  planet-generation call;
- focused settings checks preserved fixed results and returned quantitative
  unknowns for another star count and altered combat settings, while peace
  mode returned Dark Fog not applicable; and
- the Release solution and local plugin builds completed with zero warnings,
  all 14 conclusion checks passed, and all 14 runtime-boundary checks passed.

**Produced:** A complete presentation-neutral preview report boundary and a
repeatable multi-seed developer probe. No scan output or external game
assembly entered the repository, and the real plugin is still not the package
artifact.

**Retained debt:** IMPL-04 did not close [TD-001 or TD-002](TECHNICAL-DEBT.md).
Their temporary constraints and IMPL-08 closure gate remain unchanged.

**Excluded:** Raw evidence, starter or rare-resource conclusions, batch search,
persistent caching, New Game hooks, player controls, layout, and presentation
copy.

## Phase 3 - Establish and use the on-demand raw boundary

### IMPL-05: Certify isolated raw planet generation

**State:** Accepted on 2026-08-11.

As a maintainer adding exact evidence, I want one safe normalized raw-planet
boundary certified across the supported catalogue so that later on-demand
features do not discover algorithm or cleanup failures in player operations.

**Delivered:** A developer-invoked BepInEx operation generated and released one
isolated candidate planet through every solid-planet algorithm reachable from
the supported theme catalogue and returned exact normalized evidence or an
explicit non-success result.

**Implemented:**

- an immutable single-planet request, coverage, node, group, evidence, result,
  and compatibility-diagnostic boundary without DSP, Unity, or BepInEx types;
- one shared operation gate serializing preview and raw work, plus main-thread,
  fingerprint, request-identity, state-capture, and restoration checks;
- isolated candidate galaxy ownership with exact target and algorithm checks,
  `PlanetModelingManager.PrepareWorks`, `RandomTable.Init()`, DSP's selected
  atomic raw call, deterministic normalization, and guaranteed release;
- exact resource type, product, amount, and group evidence plus deterministic
  invariant-decimal positions, with oil represented separately as flow
  semantics and its runtime multiplier;
- a raw-specific state lease that restored the three `RandomTable` and seven
  `PlanetModelingManager` preparation-array references in addition to the
  existing game, save, factory, progression, and descriptor state;
- complete or unavailable single-planet coverage, seed/planet/stage errors,
  explicit unknown runtime-value diagnostics, and no partial evidence escape;
  and
- a deterministic developer certification mode that derived the reachable
  algorithms from the accepted ordered theme catalogue and selected one real
  generated candidate for each.

**Acceptance evidence:**

- the supported catalogue exposed solid algorithms `1` through `13`, and each
  completed through its runtime-selected raw path with complete coverage;
- two independent isolated DSP processes produced byte-identical normalized
  output with SHA-256
  `47DC2C493A02FAB0E249E934C6E96D094520C427860029ED82484F43BBCE81E8`;
- the repeated evidence contained 7,563 exact vein nodes and 487 groups;
  full request and fingerprint provenance plus product, amount, group,
  position, and unit fields matched, 81 oil nodes alone used oil-flow
  semantics, and no finite deposit carried an oil multiplier;
- every successful trace recorded raw preparation, `RandomTable.Init()`, one
  atomic start and completion, normalization, candidate release, and restored
  state;
- an injected post-atomic failure and cancellation both before raw work and
  after the atomic call returned unavailable coverage, no evidence, precise
  seed/planet/stage results, and restored state; every created candidate was
  released, and all ten preparation-array identities were restored; and
- the Release solution and local plugin builds completed with zero warnings,
  all 14 conclusion checks passed, and all 19 runtime-boundary checks passed.

**Produced:** The certified developer-only single-planet raw boundary and its
repeatable catalogue harness. Probe output, copied dependencies, and generated
assemblies remained outside the repository; preview traces continued to prove
that automatic preview evaluation never invoked raw generation.

**Accepted position limitation:** Raw node and group positions are normalized
from DSP's single-precision values through deterministic invariant-decimal
conversion, not preserved as their original IEEE-754 bit patterns. That loss
is accepted because no active conclusion depends on bit-exact raw positions;
resource type, product, amount, group, provenance, and coverage remain exact.

**Retained debt:** IMPL-05 did not close [TD-001 or TD-002](TECHNICAL-DEBT.md).
Their release constraints and IMPL-08 closure gate remain unchanged.

**Excluded:** Birth-system or cluster orchestration, resource conclusions,
per-planet queue progress, performance bounds, background work, UI, and broad
terrain or buildable-area interpretation.

### IMPL-06: Return exact starter-resource conclusions

**State:** Accepted on 2026-08-11.

As a player judging a fresh start, I want exact birth-system resources on
explicit request so that starter conclusions use generated deposits rather
than preview proxies.

**Return:** A cancellable on-demand birth-system operation returns every
accepted `FS-RESOURCES` component at supported settings and explicit unknowns
elsewhere.

**In scope:**

- serialized raw generation across all solid birth-system planets;
- declared and completed coverage, per-planet progress, cancellation between
  planets, and seed/planet/stage failure attribution;
- common-resource totals, per-resource amounts and groups, oil semantics, and
  fire-ice presence;
- accepted multiplier-`1` predicates and exact diagnostic facts with unknown
  amount conclusions at other multipliers; and
- a new on-demand report that does not mutate the prior preview report.

**Acceptance:**

- accepted starter positive, negative, and endpoint fixtures repeat exactly in
  independent game processes;
- resource-multiplier variants preserve their facts and required unknowns
  without threshold scaling;
- cancellation and injected planet failure retain partial coverage diagnostics
  but produce no complete birth-system conclusion;
- raw data and shared runtime state are released on every exit path; and
- the operation starts only through its explicit developer invocation.

**Out of scope:** Complete-cluster generation, rare-resource distance, gas-rate
conclusions, other resource ranges, buildable-area judgments, player controls,
background queues, and generalized benchmarking.

**Delivered:** An explicit developer-invoked operation declared every solid
birth-system planet, generated each through the accepted raw boundary, reported
immutable per-planet progress, and returned a new complete `FS-RESOURCES`
report without changing the earlier preview result.

**Implemented:**

- a runtime-neutral plan, target, progress, coverage, and result contract with
  exact expected/completed counts and affected-planet attribution;
- one serialized main-thread operation that captured compatibility and shared
  state once, normalized a complete preview, and invoked the certified raw
  boundary for each declared solid birth-system planet;
- exact per-resource amount and vein-group aggregation, independent oil-flow
  facts, finite common-deposit totals, and fire-ice presence;
- complete-only evaluation: partial cancellation or failure retained coverage
  and progress diagnostics but exposed no reports;
- reference-multiplier conclusions and unscaled exact facts with explicit
  unknown amount/group conclusions at other multipliers; and
- a developer-only probe covering accepted fixtures, altered settings,
  cancellation between planets, injected planet failure, state restoration,
  and candidate release traces.

**Acceptance evidence:**

- 22 accepted positive, negative, total-endpoint, per-resource amount-endpoint,
  group-endpoint, and fire-ice fixtures matched the earlier runtime catalogue
  with zero fact mismatches in two independent DSP processes;
- both normalized probe files were byte-identical with SHA-256
  `8CFDA61B9A356C80F2C38E7D4B61F3634B0169BE1F38F8C5C7D6D1B0310F2E98`;
- all 23 complete requests restored state and declared/completed three planets;
  the multiplier-`0.5` variant preserved exact facts, returned unknown range
  outcomes without scaling thresholds, and retained invariant fire-ice state;
- cancellation and injected failure each stopped after one completed planet,
  retained partial `1/3` coverage and the affected planet, and returned no
  conclusions;
- all 25 plans, 72 created raw candidates, and all 25 captured state leases
  recorded release or successful restoration on their respective exit paths;
  and
- the Release plugin build completed with zero warnings, all 14 conclusion
  checks passed, and all 23 runtime-boundary checks passed.

**Clarified oil semantics:** The accepted named fixture totals excluded oil,
while one catalogue sentence had included it despite oil using flow semantics.
The implementation and corrected catalogue keep oil amount and groups as exact
independent components and do not add oil flow units to the finite-deposit
common total.

**Produced:** The first exact on-demand player-decision report boundary and a
repeatable developer harness. Generated evidence and copied game-linked
artifacts remained outside the repository.

**Retained debt:** IMPL-06 did not close [TD-001 or TD-002](TECHNICAL-DEBT.md).
Their release constraints and IMPL-08 closure gate remain unchanged.

### IMPL-07: Return exact rare-resource access

**State:** Completed; pending acceptance.

As a player planning expansion, I want an explicit complete-cluster scan for
rare-resource distance so that nearby access is based on actual deposits and
not theme declarations.

**Return:** A bounded, cancellable operation reports each supported rare
resource's presence and birth-system distance, then returns a new report with
eligible roles, grouping, and traits.

**In scope:**

- serialized raw generation for every solid planet in one requested cluster;
- full-cluster coverage, per-planet progress, cancellation, cleanup, and
  deterministic aggregation;
- `RR-ACCESS.distance` evaluated separately for each supported rare resource;
- exact amount and group diagnostics while `RR-ACCESS.amount` and
  `MF-RESOURCE-SCOPE` remain unknown;
- reevaluation of eligible `MF-SYSTEM-ROLE`, `CX-GROUPING`, and
  `TRAIT-SUMMARY` components in a newly attributed report; and
- time and retained-memory observations sufficient to propose a safe
  single-operation bound.

**Acceptance:**

- at least one accepted complete-cluster fixture repeats exactly after
  normalization in independent game processes;
- positive, preference-sensitive, negative, absent, partial, and incompatible
  rare-access cases match the contract;
- cancellation stops at a planet boundary, produces no complete-cluster
  conclusion, restores state, and retains no candidate planet objects;
- the operation remains serialized; and
- diagnostic amounts cannot upgrade deferred abundance or resource-strength
  components.

**Out of scope:** Rare-abundance ranges, cluster resource scoring, parallelism,
exhaustive search, unattended queues, travel time, logistics throughput,
databases, exports, UI, and performance optimization.

**Delivered:** An explicit developer-invoked complete-cluster operation
generated every solid planet in one owned candidate galaxy and returned a new
complete report with exact rare-resource access, eligible derived roles,
grouping, and traits.

**Implemented:**

- immutable complete-cluster plan, target, progress, coverage, evidence, and
  result contracts without game, Unity, or BepInEx types;
- one serialized main-thread operation with compatibility checks, a declared
  maximum of 256 solid planets, safe-boundary cancellation, affected-planet
  attribution, and complete-only result publication;
- one retained candidate galaxy per raw operation, reusing the certified raw
  preparation, atomic generation, normalization, and shared-state lease while
  streaming normalized evidence into bounded aggregation;
- exact supported rare-resource presence, nearest system and birth distance,
  amount, and group diagnostics, plus the exact finite common-resource cluster
  total;
- complete-cluster coverage for `RR-ACCESS` and `MF-RESOURCE-SCOPE`, preserving
  unknown abundance and strength outcomes while reevaluating eligible roles,
  grouping conclusions, and registered traits; and
- a developer-only acceptance mode covering exact fixtures, cancellation,
  injected incompatibility, release/restoration traces, elapsed time, managed
  allocation pressure, and post-collection retained memory.

**Acceptance evidence:**

- seeds `73339583`, `96178012`, and `45772` completed exact raw coverage of
  218, 196, and 216 solid planets respectively in each of two independent DSP
  processes;
- all accepted rare amounts, groups, finite common totals, and coverage counts
  matched the earlier runtime catalogue with zero fact mismatches;
- kimberlite at `2.02785704789118` light-years supported close access,
  unipolar magnets at `7.35336482256494` light-years were
  preference-sensitive, and unipolar magnets at `38.4949468921654`
  light-years did not support close access;
- normalized results, rare evidence, reports, and release/restoration traces
  from both processes were byte-identical after excluding observations, with
  SHA-256
  `B67A4D824DD784A8D0FE53156E6172FD76974147FDA8C26E17A1B44CE94C8936`;
- cancellation stopped at `3/218` planets and injected incompatibility at
  `1/218`; both retained partial coverage and the affected planet, exposed no
  rare evidence or conclusions, released both plan and raw candidates, and
  restored shared state;
- all five operations per process recorded plan-candidate release,
  raw-candidate release, and successful state restoration; absent and runtime
  incompatibility cases also passed the focused boundary suite; and
- all 14 conclusion checks and all 27 runtime-boundary checks passed, and the
  Release solution and game-linked plugin builds completed with zero warnings.

**Proposed single-operation bound:** One serialized 64-star cluster request may
declare at most 256 solid planets. The six complete fixture observations took
23,670 to 28,143 ms and showed at most 1,295,376,384 bytes of temporary managed
heap growth. Developer-only post-collection checks retained at most 2,510,848
bytes, including the returned report and progress, with no candidate planet or
galaxy exposed. These observations bound the current operation; they are not a
performance guarantee or authorization for queues or parallel scans.

**Produced:** The exact complete-cluster rare-access boundary and its
repeatable developer acceptance harness. Generated evidence, observations,
and game-linked artifacts remained outside the repository.

**Retained debt:** IMPL-07 did not close [TD-001 or TD-002](TECHNICAL-DEBT.md).
Their release constraints and IMPL-08 closure gate remain unchanged.

## Phase 4 - Prove conformance and package the core

### IMPL-08: Prove scanner-core runtime conformance

**State:** Proposed; depends on IMPL-01 through IMPL-07 acceptance.

As a player relying on scanner results, I want failures, cancellation, and
unsupported environments challenged across the complete core so that no
apparently valid report survives missing or unsafe evidence.

**Return:** A reviewable conformance record demonstrates the implemented core's
determinism, isolation, failure behavior, and safe operating bounds before it
is packaged as a real mod.

**In scope:**

- the complete pure fixture suite and prohibited-proxy cases;
- supported-runtime preview, birth-system raw, and full-cluster raw repetition
  across independent processes;
- injected identity, member, catalogue, enum, planet-stage, cancellation,
  partial-coverage, busy, and cleanup failures;
- verification of stage keys, provenance, subject attribution, component
  independence, unknown/not-applicable propagation, and state isolation;
- measured preview, birth-system, and full-cluster time and retained-memory
  observations, with enforced single-operation bounds; and
- a concise conformance record containing actual checks and residual limits.

**Acceptance:**

- every implementation-planning runtime gate has recorded passing evidence for
  the supported identity;
- no failure case fabricates a complete result, loses its seed and stage, or
  mutates save, factory, progression, or New Game settings;
- no live candidate object survives an operation boundary;
- enforced bounds stop or reject work predictably without parallel generation;
  and
- documentation distinguishes compilation, pure tests, and in-game evidence.

**Out of scope:** New conclusion behavior, refactoring for hypothetical
extensions, broad performance optimization, another runtime identity,
packaging replacement, publication, UI, and telemetry.

### IMPL-09: Package the conformant scanner core

**State:** Proposed; depends on IMPL-08 acceptance.

As a maintainer preparing presentation work, I want the conformant core built
as an installable BepInEx package so that later integration depends on a real
mod rather than dummy artifacts.

**Return:** CI builds and validates the real presentation-neutral scanner core
with automatic semantic versioning and accurate package metadata.

**In scope:**

- replace the dummy DLL input with the real project build;
- preserve the accepted automatic semantic version mechanism;
- replace placeholder manifest and package metadata with accurate contents;
- install the generated package into an isolated supported runtime and confirm
  clean load plus developer-invoked preview and raw operations;
- document build prerequisites, supported identity, implemented conclusions,
  operating bounds, integration invocation, and deferred behavior; and
- exclude build output, game assemblies, logs, scan results, and probe artifacts
  from source control and the archive.

**Acceptance:**

- CI builds the real plugin and the generic package validator accepts the
  automatically versioned archive;
- the archive contains only intentional mod files and no DSP, Unity, or BepInEx
  assemblies;
- an isolated installation loads and invokes the accepted core operations;
- documentation and metadata state that no player-facing panel exists; and
- the report boundary is documented as the input to a separately reviewed
  presentation roadmap.

**Out of scope:** New conformance behavior, Thunderstore publication, release
promotion, player-facing hooks or controls, UI, keybindings, telemetry, batch
search, wider compatibility, persistence, and deferred predicates.

## Roadmap coverage

| Planning-boundary obligation | Covered by |
| --- | --- |
| Normalized contracts, identity equality, stage keys, and one vertical proof | IMPL-01 |
| Every accepted predicate, outcome, role, trait, unknown, and counterexample | IMPL-02 |
| Compatibility fingerprint, lifecycle/thread proof, serialization, state isolation, and minimal runtime proof | IMPL-03 |
| Complete preview extraction, derivation parity, and every immediate conclusion | IMPL-04 |
| Every reachable raw algorithm, `RandomTable` preparation, isolation, normalization, and cleanup | IMPL-05 |
| Exact starter evidence, birth-system progress and cancellation, and `FS-RESOURCES` | IMPL-06 |
| Full-cluster repetition, rare distance, revised roles/grouping/traits, and proposed bounds | IMPL-07 |
| Failure-path conformance, repeated runtime evidence, no game-state mutation, and enforced bounds | IMPL-08 |
| Real plugin/package replacement, automatic versioning, isolated install, documentation, and presentation handoff | IMPL-09 |
| Explicit unknowns, provenance, component independence, and no copied assemblies | Every applicable story; release-gated by IMPL-08 and IMPL-09 |

## Roadmap-wide exclusions

The following are not backlog items hidden inside these stories:

- New Game panel design or hooks, presentation copy, accessibility, styling,
  comparison interaction, and player controls;
- batch search, parallel generation, background queues, databases, exports, or
  shared persistent caches;
- additional DSP builds, galaxy algorithms, ordered theme catalogues, star-count
  ranges, resource-setting ranges, combat-setting ranges, or generalized mod
  compatibility;
- gas-rate, rare-abundance, and megafactory resource-strength predicates;
- cross-machine equivalence;
- speedrun routing, buildable area, mining layout, logistics throughput,
  receiver output, evolving Dark Fog behavior or yield, aesthetics, novelty,
  challenge quality, and universal ranking; and
- publishing a release to an external service.

Adding any excluded item requires an explicit roadmap change or a later
roadmap. Accepted conclusion semantics change only through their independent
contract versioning rules.

## Roadmap completion

This roadmap is complete when IMPL-01 through IMPL-09 are individually accepted
and the conformant scanner core package exists. Completion authorizes proposing
a separate presentation roadmap; it does not activate or predefine the New
Game panel story.
