# Presentation Refinement Roadmap

**Status:** Active. RFIN-01 through RFIN-04 were accepted on 2026-08-12.
RFIN-05 reached its automated acceptance gate on 2026-08-12 and awaits product
acceptance.

**Active user story:** RFIN-05 is at its acceptance gate.

This roadmap refines the accepted New Game panel without reopening its
lifecycle, cache, or 37% by 37% viewport contracts. It replaces mechanical
evidence summaries with concise conclusions and removes contexts that do not
add a defensible player decision.

## Product return

The accepted hands-off panel will remain stable while cache-miss scans yield
more recovery frames. Fresh start, Megafactory, Compact expansion, and Sphere /
energy cards will answer distinct player questions in brief natural language.
Dark Fog will move to neutral status metadata, and redundant trait conclusions
will disappear.

## Governing presentation rules

- The panel presents conclusions, not an evidence table. A supporting fact is
  shown only when it is uniquely useful and brief.
- Strength, preference-sensitive, and limitation remain the three columns.
  Unknown and not-applicable results remain omitted.
- Removing Dark Fog judgments removes every currently emitted tradeoff and
  caution. The approved provisional `T` and `C` badge treatment is therefore
  retained as a deferred presentation rule, not an active story, until a future
  accepted conclusion emits one of those outcomes.
- Player-visible names are evidence-backed. The presenter never invents a
  planet or assigns separate facts to the same planet without attribution.
- Lists contain at most three examples. Larger sets use an approved natural
  qualifier such as `many`; they never use `+N` or an omitted-count sentence.
- Internal identifiers, `@`, raw runtime units, unsupported operational claims,
  and mechanical labels never reach the panel.
- The accepted viewport size, anchors, scroll behavior, session lifecycle, and
  cache-hit behavior do not change.

## Sequencing

RFIN-01 is independent. RFIN-02 supplies planet attribution needed by RFIN-04.
RFIN-03 supplies bounded system candidates needed by RFIN-06 and RFIN-08.
RFIN-05 removes Dark Fog roles before RFIN-06 rewrites Megafactory and RFIN-07
summarizes the remaining route roles. RFIN-09 reconciles removals and panel
finish after all context stories. Human in-game validation occurs only in
RFIN-10.

## Validation policy

RFIN-01 through RFIN-09 use focused automated fixtures, builds, and package
checks. They do not require human in-game validation. RFIN-10 owns the single
installed-game validation phase and may exercise multiple representative seeds
within that phase.

## Phase 1 - Prepare refinement inputs

### RFIN-01: Add scan recovery frames

**State:** Accepted on 2026-08-12 without semantic change.

As a player inspecting a new seed, I want the automatic scan to disrupt the
preview less, even if complete conclusions arrive later.

**Return:** Add one recovery frame after every existing safe yield, doubling
the yielded frames while retaining visible planet progress.

**Acceptance gate:** Scheduling fixtures prove twice as many yielded frames for
the same scan work, monotonic progress, identical final evidence, safe
cancellation, serialization, and restoration of preparation statics. The final
human gate records cache-miss smoothness and duration.

**Out of scope:** Background or parallel generation, adaptive frame budgets,
yielding while DSP generation statics are installed, or scan acceleration.

**Implemented:** The cooperative complete-cluster operation now alternates each
planet-generation advance with one recovery-only advance. The recovery advance
does not enter DSP generation, does not change progress, and completes only
after confirming the runtime session remains restored. The final planet also
receives its recovery frame before evaluation and publication.

**Acceptance evidence:**

- the focused three-planet scheduling fixture required six advances: three
  planet steps and three recovery-only steps in strict alternation;
- completed-planet progress advanced only on planet steps and remained
  monotonic, while synchronous and cooperative execution returned identical
  coverage, rare-resource evidence, and conclusion reports;
- cancellation at the recovery boundary published no complete conclusions,
  injected failure remained attributable to the affected planet, and both
  paths restored the runtime session and captured preparation state;
- the shared runtime lease rejected competing preview work during planet and
  recovery advances, then released it after the terminal recovery step; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 conclusion checks passed, and all 49 runtime-boundary checks passed.

**Produced:** A recovery-frame phase in `CompleteClusterRawOperation`, an
explicit orchestration contract documenting alternating planet and recovery
frames, and strengthened cadence, equivalence, cancellation, failure, and
serialization fixtures. Per the roadmap validation policy, installed-game
smoothness and cache-miss duration remain deferred to RFIN-10's sole human
validation phase.

### RFIN-02: Preserve planet attribution

**State:** Accepted on 2026-08-12 without semantic change.

As a player, I want every named planet to own the fact that qualified it.

**Return:** Retain presentation-safe planet attribution for per-planet Solar,
Wind, and tidal-lock facts. Retain starter gas-giant count and per-giant product
membership so singular, plural, presence, and known-absence copy is supported.
Names and ownership remain evidence, not presenter inference.

**Acceptance gate:** Fixtures map each retained fact to its DSP planet, prevent
cross-planet merging, and preserve deterministic ordering and DSP display
names. Gas fixtures distinguish zero, one, and multiple giants and never infer
that every giant carries an aggregate product. Incomplete attribution is
unknown and omitted. Any affected persisted contract is versioned and
incompatible entries fail as cache misses.

**Out of scope:** New thresholds, planet scoring, terrain judgments, localized
name guarantees, or copy changes.

**Implemented:** The normalized birth-system evidence now retains every solid
planet's DSP ID, display name, Solar ratio, Wind ratio, and tidal-lock state.
Each starter gas giant separately owns its DSP display name and complete
product membership. Attribution is sorted by DSP planet ID and remains one
immutable evidence contract through preview evaluation and presentation
resolution, so the presenter cannot merge separately owned facts.

**Acceptance evidence:**

- fixtures preserved DSP display names and per-planet Solar, Wind, tidal-lock,
  and gas-product ownership in deterministic planet-ID order;
- duplicate planet identities and incomplete gas-product attribution failed
  closed, while absent attribution remained explicitly unknown and unpublished;
- zero-, one-, and two-giant fixtures retained distinct cardinality, and
  differently stocked giants never inherited the birth system's aggregate
  product set;
- a cached conclusion reload regenerated attribution from its current live
  preview and retained it for presentation without starting another complete
  scan; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 conclusion checks passed, and all 51 runtime-boundary checks passed.

**Produced:** Presentation-safe `NormalizedBirthPlanetEvidence` owned by the
birth system, live-preview extraction using DSP planet display names, and a
resolution handoff that deliberately does not add attribution to the persisted
semantic-conclusion cache. Because neither conclusion semantics nor persisted
cache shape changed, no cache contract version changed; existing valid entries
remain reusable and attribution is always regenerated from the loaded preview.

### RFIN-03: Preserve bounded system candidates

**State:** Accepted on 2026-08-12 without semantic change.

As a player, I want the panel to name several useful systems when the evidence
supports more than one candidate.

**Return:** Retain presentation-safe per-system energy and shell evidence so
later stories can select up to three candidates instead of receiving only the
cluster maximum. Candidate ownership and ordering remain evidence-backed.

**Acceptance gate:** Fixtures retain each qualifying system's decisive facts,
rank candidates deterministically, keep facts attached to their source system,
and expose no more evidence than the presentation contracts require. Missing
or partial candidate evidence remains unknown. Any affected persisted contract
is versioned and incompatible entries fail as cache misses.

**Out of scope:** New thresholds, composite scores, changed role predicates,
copy, or displaying more than three candidates.

**Implemented:** A presentation-safe candidate projection now derives three
independent lanes from complete normalized preview evidence: Dyson luminosity,
maximum shell radius, and contained-orbit count. Each lane retains at most
three systems with its DSP display name, stable system identity, and the one
decisive value for that lane. Values remain attached to their source system;
ranking is descending by the decisive value and then by stable identity.

**Acceptance evidence:**

- focused fixtures independently ranked energy, shell-radius, and
  contained-orbit candidates and capped every lane at three systems;
- deliberately different measurements proved that a system's name and value
  stayed together across all lanes, while equal measurements used stable
  identity as the deterministic tie-breaker;
- a missing energy fact made only the energy lane unknown while complete shell
  and containment lanes remained available;
- a cached conclusion reload retained the freshly projected live-preview
  candidates without starting another complete scan or persisting candidate
  evidence; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 conclusion checks passed, and all 53 runtime-boundary checks passed.

**Produced:** Bounded immutable `RuntimeSystemCandidates` lanes available to
the active presentation resolution. The projection exposes no star type,
all-system evidence collection, composite score, new predicate, or copy. Since
the semantic conclusion set and cache payload remain unchanged, no persisted
contract version changed and existing valid entries remain cache-compatible.

## Phase 2 - Rewrite decision presentation

### RFIN-04: Rewrite Fresh start conclusions

**State:** Accepted on 2026-08-12 without semantic change.

As a player, I want to know how this seed's conditions complement a fresh
start.

**Return:** Apply the Fresh start contract below to gas products,
renewable power, shared-giant topology, starter resources, vein groups, and
local Fire Ice.

**Acceptance gate:** Every known family and outcome has deterministic bounded
copy, planet names appear only with RFIN-02 attribution, and no raw quantities,
star-type suffixes, `@`, or unsupported output claims appear. Unknowns remain
omitted. The Fresh start wording decisions were resolved during implementation.

**Out of scope:** New predicates, changed ranges, resource viability, mining
performance, or raw evidence views.

**Implemented:** Fresh start now has a dedicated natural-language presenter.
It uses starter-gas-giant singular or plural product presence and known
absence; attributed planet-first Solar, Wind, and permanent-solar-source
statements; gas-giant-neighbor conclusions; and concise per-resource amount,
vein-group, and local Fire Ice statements. Solar and Wind remain separate
cards. The combined starter-deposit total is intentionally omitted because it
duplicates the individual resource decisions.

The automatic complete scan now retains a bounded starter-resource aggregate
from the birth-system planets it already generates. Those accepted predicates
are published with the complete result and persisted as semantic conclusions,
so cache hits reproduce the same completed Fresh start presentation without a
new scan.

**Acceptance evidence:**

- focused fixtures covered singular and plural gas wording, known product
  absence, attributed Solar, Wind, and tidal-lock sentences, topology,
  resource amount and vein-group outcomes, and Fire Ice presence or absence;
- planet names appeared only when complete RFIN-02 attribution identified the
  qualifying planets; missing attribution omitted gas, power, and tidal copy
  without fabricating a fallback;
- no Fresh start line contained star-type suffixes, `@`, percentages, ratios,
  raw amounts, mechanical distribution labels, omitted counts, or the combined
  starter-deposit total;
- automatic complete-scan and cache-hit fixtures produced identical completed
  Fresh start text; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 conclusion checks passed, and all 55 runtime-boundary checks passed.

**Produced:** The Fresh start sentence composer and bounded automatic
birth-system resource aggregation. Cache schema version 3 invalidates older
semantic entries as misses because completed Fresh start resource conclusions
are now included in the persisted presentation result.

### RFIN-05: Separate Dark Fog facts from judgments

**State:** Implemented through its automated acceptance gate on 2026-08-12;
awaiting product acceptance.

As a player, I want the generated Dark Fog occupation reported without a
farming verdict.

**Return:** Remove Dark Fog conclusion reports, cards, tradeoffs, cautions,
Megafactory roles, and Compact expansion roles. Preserve the underlying
occupation evidence. In Combat mode with complete compatible preview coverage,
show one neutral fixed-status line:

`Dark Fog: 36 initial hives; 1 in starter system`

Use natural singular forms. Peace, incomplete, and unsupported cases omit the
line; the identity continues to state `Peace` or `Combat`.

**Acceptance gate:** Exact cluster and starter counts appear only in the fixed
status area under eligible Combat previews. No Dark Fog outcome remains visible
or influences another conclusion. Normalized occupation evidence remains
available for future specification. The changed conclusion set invalidates
older semantic-cache entries safely.

**Out of scope:** Farming suitability, bases, levels, loot, threat, attack
timing, future occupation, icons, or combat-setting changes.

**Implemented:** Dark Fog hive counts no longer enter the conclusion engine.
The preview runtime instead projects one immutable occupation fact containing
the exact cluster and starter-system initial-hive counts only when Combat mode
and complete preview evidence make both counts authoritative. The panel renders
that fact as a neutral status line above the outcome columns. Peace mode and
incomplete, failed, or unsupported previews publish no line.

Removing the Dark Fog conclusion family also removed its farming card,
opportunity and exposure outcomes, tradeoff and caution, `fog-opportunity`
Megafactory role, and every Compact expansion grouping it previously induced.
Normalized per-system `InitialHiveCount` evidence remains unchanged for future
specification work.

**Acceptance evidence:**

- core fixtures retained exact normalized hive counts while proving that no
  `DF-` report, Dark Fog context, tradeoff, caution, or `fog-opportunity` role
  was emitted or propagated into route grouping;
- runtime fixtures projected `40` cluster hives and `1` starter-system hive as
  `Dark Fog: 40 initial hives; 1 in starter system`, and covered the singular
  `1 initial hive` form;
- Peace mode and incomplete hive coverage omitted the status fact, while
  altered Combat settings retained the same neutral counts without turning
  them into a verdict;
- presentation fixtures contained no Dark Fog card and placed the exact counts
  only in the fixed status area; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 conclusion checks passed, and all 55 runtime-boundary checks passed.

**Produced:** A presentation-safe `RuntimeDarkFogOccupation` projection and
neutral status rendering independent of semantic conclusions. Cache schema
version 4 invalidates schema-3 semantic entries as ordinary misses so cached
Dark Fog judgments cannot survive the changed conclusion set.

### RFIN-06: Rewrite Megafactory candidates

**State:** Approved; inactive.

As a player, I want to know which candidates exist for each supported
megafactory role.

**Return:** Apply the Megafactory contract below to stellar energy, large
shells, contained orbits, and rare access. Present up to three evidence-backed
candidates and group several roles belonging to one system.

**Acceptance gate:** Each supported state uses approved copy and deterministic
selection. Candidate lists do not exceed three, larger sets use approved
`many` forms, and no internal roles, raw resource amounts, `@`, Dark Fog, or
operational claims appear. The open strong-energy role rule below must be
settled before this story is activated.

**Out of scope:** Factory capacity, throughput, logistics performance, new
resource-abundance ranges, or new system roles.

### RFIN-07: Summarize Compact expansion routes

**State:** Approved; inactive.

As a player, I want to know how easy or difficult expansion is for the roles
this seed supports.

**Return:** Replace system-pair data with `Short routes`, `Normal routes`, and
`Long routes`, optionally followed by up to three of `starter`, `energy`,
`sphere`, `orbits`, and `rares`.

**Acceptance gate:** Roles are deduplicated deterministically into one approved
distance class, copy matches the Compact contract below, and no exact distance,
pair, system, star type, internal role, or intermediate predicate appears. The
open role-class reduction rule below must be settled before activation.

**Out of scope:** Route planning, travel time, throughput, system-pair display,
or new role predicates.

### RFIN-08: Rewrite Sphere / energy candidates

**State:** Approved; inactive.

As a player, I want to know how favorable this seed is for sphere construction.

**Return:** Present shell size and contained-orbit roles using the approved
Sphere / energy contract below. Select no more than three candidates per
component: shell candidates by radius, containment candidates by orbit count,
then stable identity for ties. No composite score is introduced.

**Acceptance gate:** All three outcomes for both components use approved copy,
lists remain deterministic and bounded, and no internal radius, orbit distance,
`+N`, `@`, raw geometry, or receiver/output claim appears.

**Out of scope:** Sphere design, receiver effectiveness, aesthetics, composite
ranking, or threshold changes.

## Phase 3 - Reconcile and validate the panel

### RFIN-09: Remove redundant traits and finish the panel

**State:** Approved; inactive.

As a player, I want every panel section to add a distinct decision and remain
easy to scan.

**Return:** Remove the trait registry, trait evaluation, cached trait reports,
trait context, and trait tests without altering their source conclusions.
Apply the approved per-context aggregation rules and visually harmonize the
scrollbar with the existing panel.

**Acceptance gate:** No trait identifier or derived output remains. Source
conclusions are unchanged, aggregation snapshots match every context contract,
mechanical identifiers are absent, and scrolling remains discoverable and
functional within the accepted viewport. The changed conclusion set invalidates
older semantic-cache entries safely.

**Out of scope:** New summaries, sorting, filters, tabs, charts, comparisons,
final icons, or panel resizing.

### RFIN-10: Validate the refined experience

**State:** Approved; inactive.

As a player installing the refined package, I want smoother scanning and the
revised conclusions to work together in the supported New Game flow.

**Return:** Exercise the complete refined package after RFIN-01 through RFIN-09
have passed their automated gates.

**Acceptance gate:** Automated snapshots cover every approved copy outcome,
aggregation boundary, omission rule, and cache-version transition. Automated
suites, hosted-reference build, and exact package validation pass. The
installed 4K phase covers representative cached and uncached seeds, smoother
cache-miss pacing and recorded duration, replacement, exit, scrolling, all four
retained contexts, Dark Fog metadata, and absence of Dark Fog judgments and
traits. Residual limits are documented without assuming product acceptance.

**Out of scope:** Publication, wider compatibility, localization, comparison,
or unrelated technical debt.

## Context contracts

### Fresh start

**Question:** How are this seed's conditions complementary to a fresh start?

**Copy:**

- Gas products: `Starter gas giant has Fire Ice / Hydrogen`; use `Starter gas
  giants have/lack ...` when multiple giants exist. Known absence may be a
  limitation; incomplete coverage is omitted unknown.
- Solar and Wind are separate. Solar uses `bright`, `normal`, or `dim`; Wind uses
  `strong`, `normal`, or `weak`. Percentages are omitted and planet names require
  RFIN-02 attribution.
- Tidal lock: `Permanent solar source on Aspidiske II` or `No permanent solar
  sources`.
- Shared-giant topology: `2 gas giant neighbors` or `No gas giant neighbors`.
  The displayed neighbor count is the stored shared-body count minus the birth
  planet itself.
- Resource amount: `[Resource] plentiful` or `[Resource] scarce`; no amount is
  shown.
- Vein groups use `[Resource] has many vein groups` and `[Resource] has few vein
  groups` for the accepted outer outcomes.
- Fire Ice veins: `Found Fire Ice veins` or `No Fire Ice veins`.

**Aggregation:** Show at most three planets within the starter system when a
sentence needs planet targets. Group only planets sharing the stated fact and
outcome. Never combine Solar and Wind classifications.

**Never show:** Star-type suffixes on planet conclusions, `@`, raw quantities,
superfluous numbers, fabricated attribution, realized power, mining performance,
or universal starter viability.

**Resolved wording decisions:** Planet-first power uses `Aspidiske II has
bright solar` and the corresponding Wind form. Middle resource amount and
vein-group outcomes use `normal`. The combined starter-deposit total is not
displayed because the individual resource conclusions are more actionable.

### Megafactory

**Question:** Which candidates are present for each supported megafactory role?

**Copy:**

- Energy combinations: `Venator outshines all`, `Venator unusually bright`,
  `Venator brightest`, `Venator bright`, or `No bright stars`.
- Numeric luminosity is normally omitted. An out-of-band exceptional form may
  use `Venator unusually bright: 2.70` or `No bright stars: best 2.40`.
- Sphere size: `Large sphere at Venator`, `Large spheres at Venator,
  Alsciaukat, and Shaula`, `Many large-sphere systems`, or `No large spheres`.
- Orbit containment: `Contained orbit at Lambda Librae`, `3 contained orbits
  at Lambda Librae`, `Contained orbits at Lambda Librae, Shaula, and Venator`,
  `Many contained-orbit systems`, or `No contained orbits`. Megafactory shows
  qualifying role candidates; other classes remain under Sphere / energy.
- Rare access: `Nearby Fire Ice in Alsciaukat`, `Fire Ice in Alsciaukat`,
  `Distant Fire Ice in Alsciaukat`, or `No Fire Ice`.
- System-first grouping: `Venator: unusually bright, large sphere, 3 contained
  orbits` or `Alsciaukat: Fire Ice, contained orbit`.

**Aggregation:** Show at most three systems per role and three rare resources
per distance class. Larger groups use forms such as `Many bright stars:
Venator, Shaula, Alsciaukat`, `Many large spheres: Venator, Shaula,
Alsciaukat`, or `Many nearby rares: Fire Ice, Kimberlite, Organic Crystal`.
Retain destination systems when grouped rare resources have different
destinations. Never show an omitted count.

**Never show:** Internal roles, Dark Fog roles, `@`, raw resource amounts,
mechanical language, factory capacity, throughput, or logistics claims.

**Open contract decision:** Define how output class and leader separation
create the `energy` role used by Compact expansion. The accepted implementation
uses output strength alone; the approved copy treats separation as evidence of
a distinct leader. This must be an explicit predicate decision, not presenter
inference.

### Compact expansion

**Question:** How easy or difficult is expansion for the available roles?

**Copy:** `Short routes`, `Normal routes`, or `Long routes`, optionally followed
by up to three roles: `Short routes: starter, energy, sphere`.

**Aggregation:** Summarize natural roles, never pairs. Approved roles are
`starter`, `energy`, `sphere`, `orbits`, and `rares`.

**Never show:** Exact figures, pairs, systems, star types, inputs, intermediate
predicates, internal identifiers, or non-final conclusions.

**Open reduction decision:** When one role participates in pair reports across
multiple distance classes, choose one deterministic final class. The recommended
rule is the shortest eligible route for that role; this must be approved before
RFIN-07 begins.

### Sphere / energy

**Question:** How favorable are this seed's conditions for sphere construction?

**Copy:** `Grand shell`, `Normal shell`, `Tiny shell`, `Many contained orbits`,
`1 contained orbit`, and `No contained orbits`. Attach a system naturally when
identifying a candidate, such as `Grand shell at Venator`.

**Aggregation:** Group systems by shared conclusion. Show at most three shell
candidates ordered by radius and three containment candidates ordered by orbit
count, with stable identity as the tie-breaker.

**Never show:** Internal radius, orbit distance, `+N`, `@`, raw geometry,
receiver effectiveness, realized output, or aesthetics.

## Removed conclusion contexts

### Dark Fog farming

Hive counts do not support a neutral farming judgment because their direction
depends on player preference. RFIN-05 removes the context and downstream roles.
Only exact initial occupation remains as neutral status metadata in eligible
Combat previews.

### Decision-relevant traits

Every trait repeats a Fresh start, Megafactory, Compact expansion, or Sphere /
energy strength. RFIN-09 removes the registry and all derived output. Nothing
replaces it in the status area.

## Roadmap coverage

| Refinement requirement | Covered by |
| --- | --- |
| Twice the current safe recovery frames with visible progress | RFIN-01 |
| Evidence-backed planet names and gas-giant ownership | RFIN-02 |
| Up to three evidence-backed system candidates | RFIN-03 |
| Fresh start natural-language conclusions | RFIN-04 |
| Neutral Dark Fog status without judgments or roles | RFIN-05 |
| Megafactory role candidates | RFIN-06 |
| Compact expansion route summaries | RFIN-07 |
| Sphere construction candidates | RFIN-08 |
| Trait removal, aggregation, and panel finish | RFIN-09 |
| Automated, package, and installed-game acceptance | RFIN-10 |

## Roadmap-wide exclusions

This roadmap does not change evidence thresholds unless the open energy-role
decision explicitly revises that role predicate. It adds no scoring,
preferences, comparison, route planning, raw-data view, new Dark Fog judgment,
background generation, localization, publication, or compatibility expansion.

## Completion

The roadmap is complete only when RFIN-01 through RFIN-10 are individually
accepted and RFIN-10 records the single installed-game validation.
