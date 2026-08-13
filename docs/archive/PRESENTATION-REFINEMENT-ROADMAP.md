# Presentation Refinement Roadmap

**Status:** Completed and accepted on 2026-08-12. RFIN-01 through RFIN-10
passed their acceptance gates, including the final installed 4K validation.

**Active user story:** None.

This roadmap refined the accepted New Game panel without reopening its
lifecycle, cache, or 37% by 37% viewport contracts. It replaced mechanical
evidence summaries with concise conclusions and removed contexts that did not
add a defensible player decision.

## Product return

The accepted hands-off panel remains stable while cache-miss scans move their
expensive terrain phase off the render thread. Fresh start, Megafactory,
Compact expansion, and Sphere / energy cards answer distinct player questions
in brief natural language. Dark Fog moved to neutral status metadata, and
redundant trait conclusions were removed.

## Governing presentation rules

- The panel presents conclusions, not an evidence table. A supporting fact is
  shown only when it is uniquely useful and brief.
- Strength, preference-sensitive, and limitation remain the three columns.
  Unknown and not-applicable results remain omitted.
- Removing Dark Fog judgments removed every emitted tradeoff and
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

RFIN-01 was independent. RFIN-02 supplied planet attribution needed by RFIN-04.
RFIN-03 supplied bounded system candidates needed by RFIN-06 and RFIN-08.
RFIN-05 removed Dark Fog roles before RFIN-06 rewrote Megafactory and RFIN-07
summarized the remaining route roles. RFIN-09 reconciled removals and panel
finish after all context stories. Human in-game validation occurred only in
RFIN-10.

## Validation policy

RFIN-01 through RFIN-09 used focused automated fixtures, builds, and package
checks. They did not require human in-game validation. RFIN-10 owned the single
installed-game validation phase and exercised representative seeds within that
phase.

## Phase 1 - Prepare refinement inputs

### RFIN-01: Add scan recovery frames

**State:** Accepted on 2026-08-12 without semantic change. Its initial
recovery-frame implementation was superseded during RFIN-10 validation by the
scanner-owned terrain-worker correction.

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

**Implemented:** The cooperative complete-cluster operation alternated each
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

**Produced at this gate:** A recovery-frame phase in
`CompleteClusterRawOperation`, an explicit orchestration contract documenting
alternating planet and recovery frames, and strengthened cadence, equivalence,
cancellation, failure, and serialization fixtures. RFIN-10's installed-game
work later demonstrated that delay alone did not provide acceptable pacing and
replaced this tactic without changing scan evidence or conclusions.

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

**Implemented:** The normalized birth-system evidence retained every solid
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

**Implemented:** A presentation-safe candidate projection derived three
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

**Implemented:** Fresh start received a dedicated natural-language presenter.
It uses starter-gas-giant singular or plural product presence and known
absence; attributed planet-first Solar, Wind, and permanent-solar-source
statements; gas-giant-neighbor conclusions; and concise per-resource amount,
vein-group, and local Fire Ice statements. Solar and Wind remain separate
cards. The combined starter-deposit total is intentionally omitted because it
duplicates the individual resource decisions.

The automatic complete scan retained a bounded starter-resource aggregate
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
were included in the persisted presentation result.

### RFIN-05: Separate Dark Fog facts from judgments

**State:** Accepted on 2026-08-12 without semantic change.

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

**State:** Accepted on 2026-08-12 without semantic change.

As a player, I want to know which candidates exist for each supported
megafactory role.

**Return:** Apply the Megafactory contract below to stellar energy, large
shells, contained orbits, and rare access. Present up to three evidence-backed
candidates and group several roles belonging to one system.

**Acceptance gate:** Each supported state uses approved copy and deterministic
selection. Candidate lists do not exceed three, larger sets use approved
`many` forms, and no internal roles, raw resource amounts, `@`, Dark Fog, or
operational claims appear. The resolved strong-energy role rule below remains
unchanged.

**Out of scope:** Factory capacity, throughput, logistics performance, new
resource-abundance ranges, or new system roles.

**Implemented:** Megafactory received a dedicated natural-language presenter
for energy, large-shell, contained-orbit, and rare-access candidates. It uses
the existing accepted predicates against the bounded RFIN-03 candidate lanes,
groups several supported roles under one DSP system name, and names no more
than three examples. When more systems qualify, the retained supporting count
selects the approved `many` form without retaining or exposing the omitted
systems.

Complete rare-access reports retained their already known nearest-system
subject when the resource is present. This allows `Nearby Fire Ice in
Alsciaukat`, `Fire Ice in Alsciaukat`, and `Distant Fire Ice in Alsciaukat`
forms without preserving raw amounts in the presentation layer. Known absence
remains resource-scoped and renders as `No Fire Ice`; larger same-class sets use
bounded natural qualifiers.

**Resolved energy-role decision:** The accepted `strong-energy` role continues
to depend only on a supporting `MF-ENERGY-SYSTEM.output` outcome. Leader
separation does not create, remove, or upgrade the role. It is used only to
choose the approved energy wording: `outshines all` or `unusually bright` for
supporting output, and `brightest` or `bright` for preference-sensitive output.
This preserves the accepted predicate while preventing presenter inference
from changing Compact expansion inputs.

**Acceptance evidence:**

- focused fixtures covered `outshines all`, `unusually bright`, `brightest`,
  `bright`, and `No bright stars` without changing energy-role predicates;
- energy, large-sphere, and contained-orbit fixtures proved deterministic
  candidate ordering, a three-name bound, and approved `many` forms when the
  supporting count exceeded that bound;
- system-first fixtures combined energy, large-sphere, and contained-orbit
  roles under one DSP display name without star types or internal roles;
- complete-scan fixtures rendered nearby and distant rare resources with their
  retained destination, kept known absence distinct, and exposed no raw
  resource amounts; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 conclusion checks passed, and all 56 runtime-boundary checks passed.

**Produced:** A bounded Megafactory sentence composer, small supporting-count
metadata on the existing candidate projection, and destination-attributed
rare-access reports. Cache schema version 5 invalidates schema-4 entries as
ordinary misses because cached rare-access subjects carried the destination
needed by the presentation contract.

### RFIN-07: Summarize Compact expansion routes

**State:** Accepted on 2026-08-12 without semantic change.

As a player, I want to know how easy or difficult expansion is for the roles
this seed supports.

**Return:** Replace system-pair data with `Short routes`, `Normal routes`, and
`Long routes`, optionally followed by up to three of `starter`, `energy`,
`sphere`, `orbits`, and `rares`.

**Acceptance gate:** Roles are deduplicated deterministically into one approved
distance class, copy matches the Compact contract below, and no exact distance,
pair, system, star type, internal role, or intermediate predicate appears. The
resolved shortest-eligible-route rule below is applied consistently.

**Out of scope:** Route planning, travel time, throughput, system-pair display,
or new role predicates.

**Implemented:** Compact expansion reduced pair-level distance conclusions
to one natural route class per supported role. `starter-anchor`,
`strong-energy`, `large-shell`, `orbit-containment`, and `rare-access` map only
to `starter`, `energy`, `sphere`, `orbits`, and `rares`. The presenter emits
`Short routes`, `Normal routes`, or `Long routes` with at most three role names
in that fixed order; pair subjects and system names never enter the copy.

Derived grouping reports inherited the latest evidence stage of their two
source roles. A route involving a complete-scan rare role is therefore retained
with complete results and in the semantic cache, while preview-only routes
remain live preview evidence. At presentation time both sets are reduced
together so a completed scan and its cache hit publish the same final role
classes.

**Resolved reduction decision:** For each natural role, choose the shortest
eligible route class in the accepted predicate order: `Short`, then `Normal`,
then `Long`. Exact distance breaks no further tie because routes within one
class produce the same conclusion. A role appears once even when it participates
in many pairs. The final line names only the first three roles in the approved
order and does not explain omitted roles.

**Acceptance evidence:**

- focused fixtures covered short, normal, and long classes at the accepted
  distance predicates without exposing an exact distance;
- a role participating in several pair reports was deduplicated to its shortest
  eligible class, and repeated orbit roles appeared only once;
- fixtures combined preview roles with a complete-scan rare role, reproduced
  the same summary from cache, and verified that rare-derived grouping retained
  complete-cluster attribution;
- a five-role fixture enforced the three-role display bound and approved role
  order; no pair, system, star type, internal role, conclusion ID, or `@`
  notation reached the rendered text; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 conclusion checks passed, and all 57 runtime-boundary checks passed.

**Produced:** A Compact expansion route composer, deterministic role-class
reduction, and source-stage preservation for derived grouping reports. Cache
schema version 6 invalidates schema-5 entries as ordinary misses because older
entries omitted complete-scan rare routes from their persisted grouping set.

### RFIN-08: Rewrite Sphere / energy candidates

**State:** Accepted on 2026-08-12 without semantic change.

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

**Implemented:** Sphere / energy applied the accepted radius and
contained-orbit predicates independently to the existing bounded candidate
lanes. Shell results read `Grand shell`, `Normal shell`, or `Tiny shell`;
containment results read `Many contained orbits`, `1 contained orbit`, or `No
contained orbits`. Each conclusion attaches up to three evidence-backed system
names with natural `at` phrasing. Systems sharing one outcome are grouped while
the two components remain independent.

Candidate order continues to be radius or contained-orbit count descending,
with stable system identity resolving ties before the three-candidate bound.
The presenter introduces no score, threshold, cache field, or additional scan
work.

**Acceptance evidence:**

- one focused fixture rendered all three accepted shell outcomes and all three
  accepted containment outcomes from real bounded candidate values;
- a grouped fixture retained exactly the top three systems for each component
  in deterministic candidate order;
- repeated projection produced identical copy, and every emitted line remained
  within the established presentation bound;
- focused assertions excluded internal radius, orbit distance, `+N`, `@`, raw
  geometry, star type, conclusion IDs, and receiver/output language; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 conclusion checks and 58 runtime-boundary checks passed, and the local
  versioned artifact and Thunderstore package validators passed.

**Produced:** A dedicated Sphere / energy candidate composer over the existing
immutable candidate projection. The scan, accepted predicates, semantic cache,
and runtime evidence contract were unchanged.

## Phase 3 - Reconcile and validate the panel

### RFIN-09: Remove redundant traits and finish the panel

**State:** Accepted on 2026-08-12 without semantic change.

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

**Implemented:** The redundant trait registry, evaluator, subject and context
enum values, presenter mappings, and positive trait fixtures were removed.
Fresh start, Megafactory, Compact expansion, and Sphere / energy remain the
only decision contexts. The former trait sources continue to emit unchanged in
their owning contexts, while no duplicate trait identifier or report is
produced or persisted.

The established context-card aggregation reconciled exactly those four
contexts across immediate and complete evidence. Its deterministic line
deduplication and per-context grouping were retained. The native Unity scroll
view still owns wheel, drag, clipping, and overflow behavior; only its visual
skin changed to a narrow translucent track and a discoverable muted thumb that
fits the accepted panel surface.

Cache schema version 7 invalidates schema-6 entries as ordinary misses because
older payloads may contain retired complete-scan trait reports. No migration is
attempted, and the next successful scan writes only current semantic reports.

**Acceptance evidence:**

- source fixtures kept shared topology, tidal lock, gas product, containment,
  energy, and rare-access conclusions while asserting that no derived trait
  report remained;
- preview and complete-scan fixtures exposed exactly the four retained contexts
  and no trait enum, subject kind, identifier, mapping, or rendered output;
- the existing accepted copy, aggregation, deterministic ordering, three-item
  bounds, cache reuse, and panel document checks remained green across every
  retained context;
- an obsolete schema-6 cache payload was rejected and removed before reuse;
- the supported installed-game plugin compiled against Unity's native scroll
  API with zero warnings, preserving its interaction path and accepted 37% by
  37% viewport; and
- the Release suites passed all 14 conclusion and 58 runtime-boundary checks,
  and the local versioned artifact and Thunderstore package validators passed.

**Produced:** A four-context conclusion surface, schema-7 semantic cache, and
panel-matched native scrollbar skin. No source predicate, context copy,
viewport geometry, ordering policy, filter, or new interaction was introduced.

### RFIN-10: Validate the refined experience

**State:** Accepted on 2026-08-12 after the single installed-game human
validation phase passed.

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

**Automated result:** A release-candidate snapshot drove one representative
Combat identity through uncached completion and then cache reuse. It asserts
identical final presentation text; exact Fresh start, Megafactory, Compact
expansion, and Sphere / energy context coverage; neutral Dark Fog metadata;
approved representative copy; and absence of retired contexts, mechanical
identifiers, raw units, and unsupported notation.

The focused fixtures collectively cover every approved copy outcome,
three-example aggregation bounds, deterministic ordering, unknown omission,
schema-6 rejection by schema 7, duplicate coalescing, replacement, exit,
cancellation, progress, restoration, and cache identity behavior. The
seven-step [human validation sequence](../management/RFIN-10-HUMAN-VALIDATION.md) recorded the
installed 4K observations, including cache-miss pacing and responsiveness.

**Automated evidence:**

- the Release solution and supported installed-game plugin built with zero
  warnings;
- all 14 conclusion checks and 59 runtime-boundary checks passed;
- the local semantic-versioned artifact and exact Thunderstore package
  validators passed; and
- no human result was inferred from compilation, fixtures, or packaging.

**Human evidence:** All seven installed 4K steps passed. The accepted build
showed monotonic planet progress with no observed frame drops, readable and
scrollable four-context conclusions, correct Dark Fog metadata boundaries,
cache reuse, safe seed replacement, safe preview exit, and correct Peace-mode
behavior.

**Produced:** A validated release-candidate contract, the passed seven-step
installed 4K record, and the accepted refined presentation experience.

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

**Resolved contract decision:** Output strength alone creates the `energy` role
used by Compact expansion. Leader separation selects natural energy wording
but does not create, remove, or upgrade the role.

### Compact expansion

**Question:** How easy or difficult is expansion for the available roles?

**Copy:** `Short routes`, `Normal routes`, or `Long routes`, optionally followed
by up to three roles: `Short routes: starter, energy, sphere`.

**Aggregation:** Summarize natural roles, never pairs. Approved roles are
`starter`, `energy`, `sphere`, `orbits`, and `rares`.

**Never show:** Exact figures, pairs, systems, star types, inputs, intermediate
predicates, internal identifiers, or non-final conclusions.

**Resolved reduction decision:** When one role participates in pair reports
across multiple distance classes, use its shortest eligible route class:
`Short`, then `Normal`, then `Long`. Within-class exact distance does not alter
the conclusion.

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
depends on player preference. RFIN-05 removed the context and downstream roles.
Only exact initial occupation remains as neutral status metadata in eligible
Combat previews.

### Decision-relevant traits

Every trait repeats a Fresh start, Megafactory, Compact expansion, or Sphere /
energy strength. RFIN-09 removed the registry and all derived output. Nothing
replaced it in the status area.

## Roadmap coverage

| Refinement requirement | Covered by |
| --- | --- |
| Smooth cache-miss scanning with visible progress | RFIN-01, RFIN-10 |
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

This roadmap does not change evidence thresholds or accepted role predicates.
It adds no scoring, preferences, comparison, route planning, raw-data view,
new Dark Fog judgment, localization, publication, or compatibility expansion.
The scanner-owned terrain worker added during RFIN-10 was the bounded
correction to the failed pacing tactic, not parallel seed generation.

## Completion

RFIN-01 through RFIN-10 were individually accepted, and RFIN-10 recorded the
passing single installed-game validation on 2026-08-12. No story remains
active.

## Refinement and validation record

The presentation was refined through direct 4K observation: layout and copy
were bounded, conclusion families were rewritten and grouped, unsupported
output was removed, and lifecycle and cache behavior were exercised in the
real New Game preview. Cache-miss tests then showed that adding recovery frames
could not prevent synchronous planet generation from starving the renderer.
Investigation isolated terrain generation as the dominant cost and confirmed
DSP's native modeling path, but a first queue-based adaptation temporarily
exposed candidate game state and caused renderer faults.

The final correction kept candidate state private, ran only DSP's terrain
algorithm on a scanner-owned background worker, and performed the short vein
generation and summarization step synchronously under immediately restored
runtime globals. The obsolete delay-yield and native-queue mechanisms were
removed. Automated equivalence, restoration, presentation, build, and package
checks passed, followed by all seven installed 4K steps with the original
progress sequence intact and no observed frame drops.

Return to the [active roadmap](../management/ROADMAP.md),
[project steering](../PROJECT.md), or the [documentation index](../INDEX.md).
