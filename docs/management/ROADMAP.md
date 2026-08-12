# New Game Presentation Roadmap

**Status:** In progress. PRES-01 through PRES-06 are accepted; PRES-07 has a
repaired candidate at its human revalidation gate.

**Active user story:** PRES-07.

This roadmap turns the accepted scanner core into a hands-off decision panel
in Dyson Sphere Program's New Game cluster preview. It is deliberately limited
to the behavior accepted during presentation planning.

Approval of this roadmap did not activate its first story. Each story must be
activated, implemented through its stated automated gate, reviewed, and
accepted before dependent work begins.

## Product return

On completion, each successfully loaded New Game cluster preview will initiate
one resolution attempt for its complete generation identity. A valid local
cache hit will resolve immediately; otherwise the panel will show immediate
preview conclusions, run one bounded full scan automatically, display visible
activity while work remains, and replace that activity with concise neutral
conclusions when complete.

Changing the seed or leaving the preview will make obsolete work ineligible to
update the panel. The player will not need to invoke, configure, or interpret
the scanner to receive its supported conclusions.

## Governing decisions

- The trigger is a completed DSP cluster-preview load, not a keystroke, text
  edit, button action, or timer.
- One preview load creates exactly one **resolution attempt**. A cache hit
  performs no new scan; a cache miss may perform at most one new scan.
- Duplicate runtime callbacks for one load are coalesced. Reloading the same
  identity creates a new resolution attempt and may use its cached complete
  conclusions.
- Immediate preview conclusions remain useful while complete raw evidence is
  pending. Raw completion produces a new attributed report and never silently
  rewrites the earlier report.
- Full raw scanning is automatic for this New Game workflow. It remains one
  serialized, bounded operation and may yield or cancel only at safe runtime
  boundaries.
- A result may update the panel only while its preview session and complete
  generation identity are still current.
- Only complete-scan semantic conclusions admitted from a successful complete
  result may be persisted. Raw or normalized evidence, execution diagnostics,
  preview conclusions, and rendered wording are excluded. Cache corruption,
  incompatibility, or version mismatch is a miss, never current evidence.
- The panel renders the accepted neutral conclusion contract. It introduces no
  score, ranking, hidden weighting, new predicate, or required player input.
- Scan failure receives a terminal diagnostic for that preview load. There is
  no automatic retry loop.
- The panel uses a numeric corner setting based on the available space observed
  in the 4K New Game preview: `1` bottom-right by default, then clockwise as
  `2` bottom-left, `3` top-left, and `4` top-right. The center of every screen
  border is prohibited. Adaptive placement, dragging, and collision solving
  are not requirements.

## Validation policy

Stories PRES-01 through PRES-06 use focused automated tests, deterministic
harnesses, builds, and automated game-linked probes where runtime behavior must
be exercised. They do not require human in-game validation.

Human in-game validation occurs only in PRES-07. This accepted sequencing risk
does not relax the final runtime, responsiveness, lifecycle, or presentation
acceptance gate. A failure discovered there is repaired within PRES-07 only
when it is necessary to satisfy an already stated requirement; it does not
authorize new product behavior.

## Phase 1 - Establish presentation-safe execution

### PRES-01: Recognize one current preview session

**State:** Accepted on 2026-08-12.

As a player changing New Game seeds, I want the scanner to respond to the
cluster that DSP actually loaded so that edits and duplicate callbacks cannot
start redundant or stale work.

**Return:** A presentation-neutral lifecycle adapter emits one session for each
completed preview load, captures its complete generation identity, coalesces
duplicate callbacks, and retires the session when another preview replaces it
or the New Game preview is left.

**Acceptance gate:** Automated lifecycle fixtures prove keyboard entry, paste,
randomization, repeated callbacks, same-identity reload, different-identity
replacement, and preview exit all produce the required session count and stale
publication behavior without starting a scan.

**Out of scope:** Scanning, caching, panel creation, conclusion copy, placement,
retry, and changes to DSP's seed controls.

**Delivered:** A presentation-neutral lifecycle boundary now creates one
current session from each completed preview-load sequence and retains the full
layered galaxy, resource, and pre-play combat identity for later resolution.

**Implemented:**

- a monotonic load-sequence contract that coalesces repeated callbacks while
  treating a later sequence as a new session even when its identity is equal;
- exact preview identity equality across the accepted galaxy identity,
  resource multiplier, combat mode, initial colonization, maximum density, and
  canonical combat-settings key;
- replacement and preview-exit retirement with a cancellable session lifetime;
- rejection of an inconsistent identity reused with one load sequence, plus
  suppression of late and already-retired load callbacks; and
- a current-session publication gate that makes replaced and exited sessions
  ineligible to update later presentation state.

**Acceptance evidence:**

- completed keyboard-entry, paste, and randomization fixtures created exactly
  one session each, and no input-specific trigger entered the lifecycle API;
- a duplicate callback reused its session, while a later load of the same
  identity created a new session and retired the earlier lifetime;
- different-identity replacement cancelled the old lifetime, rejected its
  publication, and ignored its late completion without replacing the current
  session;
- preview exit retired once, cancelled the session lifetime, rejected stale
  publication, and prevented a repeated completion from resurrecting it;
- the runtime assembly remained free of DSP, Unity, and BepInEx references;
  and
- the Release solution and game-linked plugin built with zero warnings, all 14
  conclusion checks passed, and all 33 runtime-boundary checks passed.

**Produced:** `PreviewSessionLifecycle`, its immutable identity and transition
contracts, and four focused automated lifecycle fixtures. Per the roadmap's
validation policy, no human in-game validation was performed or required.

### PRES-02: Keep the full scan responsive

**State:** Accepted on 2026-08-12.

As a player waiting on exact conclusions, I want the long scan to yield between
safe units so that the New Game interface and its activity indicator can remain
responsive.

**Return:** Complete-cluster raw work can advance incrementally on the required
game thread, report bounded progress, and stop at the next accepted safe
boundary without changing its evidence or conclusions.

**Acceptance gate:** Automated runtime probes and deterministic comparisons
show incremental and existing complete execution return equivalent successful
reports; progress is monotonic; cancellation prevents complete conclusions;
generation remains serialized; cleanup and game-state restoration hold on
success, cancellation, and injected failure; and the operation yields often
enough for presentation updates between completed planets.

**Out of scope:** Background DSP generation, parallelism, throughput tuning,
automatic invocation, cache storage, panel code, and new scan bounds.

**Delivered:** Complete-cluster raw generation now exposes a cooperative
game-thread operation that completes at most one solid planet per explicit
advance. The established synchronous entry point remains available and drives
the same operation to completion without changing its result contract.

**Implemented:**

- a disposable operation with explicit ready and completed states, bounded
  planet progress, one-planet advances, and a result only at a terminal state;
- cancellation checks before the next planet and immediately after DSP's
  indivisible raw-generation call, with no complete conclusions from partial
  coverage;
- a retained, scanner-owned candidate galaxy whose DSP global pointers and
  raw-preparation static references are restored before every yield and freed
  on success, cancellation, failure, or disposal;
- one shared runtime-operation lease held for the operation lifetime, keeping
  preview and raw generation serialized across frame boundaries; and
- a developer-only probe mode that advances the cooperative operation once
  per Unity update and compares it with the synchronous entry point.

**Acceptance evidence:**

- deterministic fixtures returned identical rare-resource evidence and
  conclusion reports through synchronous and cooperative execution, with one
  completed planet per advance and monotonic planned, started, and completed
  progress;
- cancellation after `1/3`, injected failure on the second planet, and early
  disposal all restored the simulated runtime state and exposed no complete
  evidence or conclusions;
- competing preview requests remained busy before and between cooperative
  advances, then succeeded after the terminal step released serialization;
- an isolated supported DSP/BepInEx probe for seed `73339583` returned equal
  successful synchronous and cooperative evidence and reports, advanced all
  `218/218` solid planets on 218 distinct Unity frames, preserved monotonic
  progress, and passed every per-yield and final state-restoration check; and
- the Release solution and game-linked plugin built with zero warnings, all 14
  conclusion checks passed, and all 36 runtime-boundary checks passed.

**Produced:** `CompleteClusterRawOperation`, its incremental runtime-session
boundary, the synchronous compatibility wrapper, three focused automated
fixtures, and the cooperative game-linked probe. No automatic invocation,
cache, or panel behavior was introduced, and no human in-game validation was
performed or required by this story.

### PRES-03: Reuse trustworthy local results

**State:** Accepted on 2026-08-12.

As a player revisiting a preview, I want an already completed local scan reused
so that I do not repeatedly wait for identical evidence.

**Return:** A bounded local cache in the mod configuration area stores and
retrieves only complete-scan semantic conclusion bundles by full generation
identity, complete coverage, and the applicable scanner contract versions.
Live preview conclusions are regenerated rather than duplicated.

**Acceptance gate:** Automated storage tests prove deterministic key equality,
atomic replacement, the 256 KiB per-entry ceiling, bounded retention,
successful semantic-conclusion round trips, exclusion of preview conclusions
and scan evidence or diagnostics, and safe misses for absent, partial, failed,
cancelled, corrupt, incompatible, oversized, or obsolete entries. A documented
manual clear operation removes cached conclusions.

**Out of scope:** Shared or cross-machine caches, cloud storage, databases,
cache browsing UI, migration promises, incomplete-result resumption, and
changes to conclusion semantics. Raw and normalized scan evidence, execution
history, and rendered presentation copy are explicitly not cache payloads.

**Delivered:** Presentation-ready complete-cluster semantic conclusions can
now be reused from a bounded local cache only when the current supported
runtime, full generation identity, complete evidence stage, and scanner
contracts match exactly.

**Implemented:**

- a deterministic canonical key and SHA-256 filename covering the DSP build,
  generation implementation, ordered themes, seed and star count, creation and
  resource settings, pre-play combat identity, complete-cluster stage, and
  applicable scanner contract versions;
- one checksummed, dependency-free versioned binary entry per identity,
  limited to 256 KiB and written through a flushed same-directory temporary
  file and atomic replace;
- successful-complete-only admission that extracts reports attributed to
  complete-cluster evidence after validating identity, settings, versions,
  coverage, fingerprint, and restored runtime state;
- a distinct cached-conclusions contract containing only identity, complete
  coverage, and semantic reports; preview reports, normalized rare-resource
  evidence, execution diagnostics, performance observations, and rendered
  wording have no persisted representation;
- fail-closed reads that treat absent, partial, failed, cancelled,
  incompatible, corrupt, oversized, or obsolete material as a miss and remove
  an invalid file encountered at the current key;
- most-recently-used retention bounded to 256 entries by default, plus a
  presentation-neutral clear operation; and
- plugin integration rooted at `BepInEx/config/DSPSeedScanner/cache`, without
  invoking the cache automatically or adding player-facing controls.

**Acceptance evidence:**

- equivalent identities with differently scaled decimals produced the same
  canonical key, while seed and resource-setting changes produced different
  keys and unsupported fingerprints could not create keys;
- a successful complete result round-tripped only its identical complete-stage
  semantic reports and coverage, excluded its preview reports, and atomically
  replaced an existing invalid destination without leaving a temporary file;
- reflection checks confirmed the cached contract exposes no rare-resource
  evidence, progress, trace, elapsed-time, or memory surface, while an
  otherwise valid semantic payload over 256 KiB was not persisted;
- a two-entry fixture deterministically evicted the least-recent entry, kept
  the two current identities readable, and the clear operation removed every
  cache entry and remained idempotent;
- partial, failed, cancelled, and incompatible results were not written;
  absent, obsolete-schema, and checksum-corrupt entries returned misses, and
  invalid current-key files were removed; and
- the Release solution, game-linked plugin, and hosted-CI reference build
  completed with zero warnings; all 14 conclusion and 40 runtime-boundary
  checks passed; and the semantic-versioned DLL and Thunderstore package
  validators accepted the resulting three-assembly package.

**Produced:** `CompleteClusterCacheKey`, `CachedCompleteClusterConclusions`,
`CompleteClusterConclusionCache`, four focused storage fixtures, the BepInEx
configuration-path adapter, and the [cache operation record](../CACHE.md).
Cache-or-scan orchestration, cache UI, migration, rendered copy, and human
in-game validation remained outside this story.

## Phase 2 - Deliver the hands-off workflow

### PRES-04: Resolve every preview automatically once

**State:** Accepted on 2026-08-12.

As a player entering a cluster preview, I want its available conclusions
resolved automatically so that using the mod requires no scan command.

**Return:** Each preview session performs one resolution attempt: it evaluates
immediate preview evidence, uses a valid complete cache hit when available, or
starts at most one incremental full scan and persists its successful result.
Replacement and exit cancel obsolete work, and stale completion can never be
published to the current session.

**Acceptance gate:** An automated orchestration matrix proves cache-hit,
cache-miss, duplicate-event, seed replacement, same-identity reload, preview
exit, busy, incompatibility, cancellation, and failure behavior. Every case
has one attributable terminal state, no retry loop, and no fabricated complete
result.

**Out of scope:** Panel rendering, presentation wording, player controls,
multiple queued identities, batch scanning, and cache management UI.

**Implemented:** The presentation-neutral `PreviewResolutionCoordinator`
now owns one attributable attempt per lifecycle session. It evaluates the
live preview, reads the validated complete-conclusion cache, or starts one
cooperative complete scan; a successful scan is admitted to that cache.
Duplicate callbacks reuse the same attempt, while replacement and exit retire
and cancel obsolete work before another scan may acquire the shared runtime
gate. Only the lifecycle's current session is publishable.

The BepInEx plugin patches the completion boundary of DSP's
`UIGalaxySelect.SetStarmapGalaxy` and the preview's `_OnClose` lifecycle
boundary. Each completed method call receives a new monotonic load sequence
and exact generation identity. The plugin advances at most one solid planet
for the current operation per Unity frame; it does not infer loads from input
events, timers, or frame polling.

**Acceptance evidence:**

- a cache miss evaluated immediate reports, started one incremental scan,
  reached one complete terminal state, and persisted only the successful
  complete reports; a later load of the same identity evaluated fresh preview
  reports and reached one cached terminal state without another complete scan;
- a duplicate completion callback did not repeat preview evaluation or start
  another scan;
- seed replacement cancelled the obsolete attempt at a restored boundary,
  made its output unpublishable, and admitted one new attempt; preview exit
  cancelled the current attempt and left no publishable state;
- busy, incompatible, preview-failure, and complete-scan-failure fixtures each
  reached one attributable terminal state, remained stable when advanced
  again, and exposed no fabricated complete reports; and
- the Release solution and installed-game plugin builds completed with zero
  warnings, all 14 conclusion checks and 43 runtime-boundary checks passed,
  the hosted-runner reference build completed, and the semantic-versioned DLL
  and Thunderstore package validators accepted the three-assembly package.

**Produced:** `PreviewResolutionAttempt`, `PreviewResolutionCoordinator`,
focused automatic-resolution fixtures, the completed-load and preview-close
Harmony integration, and hosted-CI compile references for that integration.
Panel rendering, presentation wording, player controls, multiple queued
identities, and human in-game validation remained outside this story.

### PRES-05: Show current operational state

**State:** Accepted on 2026-08-12. The corner-anchor
requirement was resolved on 2026-08-12.

As a player viewing a cluster preview, I want a small panel to show what the
scanner is doing so that waiting, cache reuse, completion, and failure are
never ambiguous.

**Return:** The panel appears and disappears with the preview session at the
configured corner. The numeric setting maps `1` to bottom-right by default,
then clockwise to `2` bottom-left, `3` top-left, and `4` top-right. It
distinguishes waiting, cached, scanning, complete, cancelled, unsupported, and
failed states; active work displays a simple animated spinner and quiet planet
progress.

**Acceptance gate:** Automated UI-state tests cover every state transition,
ensure inactive or obsolete sessions cannot alter the visible panel, verify
the spinner advances between scan steps, verify the exact `1` through `4`
corner mapping and default, keep every border center unused, and enforce the
agreed text bounds at each configured corner.

**Out of scope:** Adaptive placement, dragging, overlap detection, non-corner
anchors, conclusion cards, raw evidence tables, preferences, manual retry, and
visual redesign of DSP controls.

**Implemented:** A presentation-neutral panel model now maps the current
resolution attempt to waiting, cached, scanning, complete, cancelled,
unsupported, or failed operational state. Active states use a four-frame ASCII
spinner and scanning includes completed-versus-expected planet progress.
Rendered copy is limited to one 32-character title and one 64-character detail
line; raw diagnostics and conclusion wording do not enter this layer.

`PreviewPanelController` admits updates only from its active, non-retired
session and hides exactly that session or the current preview. The plugin binds
`Presentation.PanelCorner` through BepInEx configuration, defaults invalid or
absent values to `1`, advances the spinner once per Unity update, and renders a
non-interactive 520-by-116 IMGUI panel with a 24-pixel corner margin. The panel
is hidden on preview close and plugin destruction without modifying DSP's UI
hierarchy or controls.

**Acceptance evidence:**

- mapping fixtures covered waiting, cached, scanning, complete, cancelled,
  unsupported, busy-as-unavailable, and failed inputs; only active states
  carried a spinner, and planning remained visibly distinct from planet
  progress;
- successive active steps selected different spinner frames and scanning
  exposed quiet completed-versus-expected planet counts;
- configuration values `1` through `4` mapped exactly to bottom-right,
  bottom-left, top-left, and top-right, while out-of-range values returned the
  bottom-right default;
- 4K placement fixtures kept all four rectangles within their selected corner,
  outside both screen center axes, and away from every border center;
- every rendered title and detail remained inside its fixed single-line bound;
  and obsolete, retired, mismatched, and exited sessions could not replace or
  hide the current view; and
- the Release solution and installed-game plugin builds completed with zero
  warnings, all 14 conclusion checks and 46 runtime-boundary checks passed,
  the hosted-reference build completed, and the semantic-versioned DLL and
  Thunderstore package validators accepted the three-assembly package.

**Produced:** `PreviewPanelStateMapper`, `PreviewPanelLayout`,
`PreviewPanelController`, `PreviewStatusPanelRenderer`, the numeric BepInEx
corner setting, and focused state, placement, spinner, text-bound, and stale
publication fixtures. Conclusion cards, raw evidence tables, adaptive layout,
player controls, and human in-game validation remained outside this story.

### PRES-06: Present concise neutral conclusions

**State:** Accepted on 2026-08-12.

As a player deciding whether a seed suits an intended run, I want its supported
conclusions grouped by context so that I can understand strengths, limitations,
tradeoffs, and uncertainty without decoding raw statistics.

**Return:** The panel presents an identity header, immediate preview summary,
and context-grouped conclusion cards for the accepted fresh-start,
megafactory, Dark Fog farming, compact-expansion, sphere or energy, and
decision-relevant-trait contexts. The pending detailed section shows activity
until complete evidence replaces it, and cached conclusions are identified.

**Acceptance gate:** Snapshot and mapping tests cover every accepted outcome,
tradeoff, unknown, not-applicable state, subject attribution, and supported
context using bounded copy. Immediate and complete reports remain visibly
distinct, all decisive conflicts survive, and no score, universal verdict,
unsupported claim, or raw-number wall is introduced.

**Out of scope:** New predicates or ranges, player preference controls,
discovery-mode controls, seed comparison, sorting, exports, charts, detailed
evidence browsing, localization, and package-branding refinement.

**Implemented at acceptance:** `PreviewConclusionPresenter` mapped only the twelve accepted
semantic families into the six accepted contexts. Each card carries an
explicit strength, limitation, preference-sensitive, tradeoff, caution,
unknown, or not-applicable outcome and a bounded subject summary. Repeated
subject-level reports are collapsed only when their context, evidence stage,
family, and outcome agree. Every tradeoff and caution remains an independent
card, and non-rendered source IDs retain traceability without exposing contract
identifiers to players.

At acceptance, the panel document began with seed, star count, resource multiplier, and
combat mode. It keeps complete Galaxy Preview cards under `Immediate preview`
and Complete Cluster Raw cards under a separately labelled detailed section.
That section says scanning while work remains, complete after a successful
scan, cached after reuse, or unavailable after a terminal rejection. Lines are
limited to 112 characters and documents to 72 lines; decisive fact values,
units, diagnostics, raw identifiers, and scores have no rendered path.

The accepted IMGUI renderer expanded the existing corner panel to the bounded document
size, applies separate identity, section, context, and card styles, and retains
the configured anchor and operational spinner or progress line. It adds no
interaction, sorting, preference, inspection, or comparison control.

**Acceptance evidence:**

- focused mapping fixtures covered all seven accepted outcomes and every
  subject kind used by the conclusion contract, including birth systems, star
  systems, clusters, resources, system pairs, and traits;
- the live deterministic preview fixture produced all six context groups in
  accepted order, and every rendered immediate card was attributable only to
  Galaxy Preview evidence;
- completed and cached fixtures replaced the pending detailed section with
  Complete Cluster Raw cards, kept the immediate section unchanged, and
  identified cache reuse explicitly;
- the number of rendered caution and tradeoff cards exactly matched the
  decisive source reports, with each source retained independently;
- deterministic snapshots enforced the identity and section hierarchy, line
  and document bounds, all four 4K corner fits, and absence of contract IDs,
  raw units, scores, rankings, universal verdicts, or best-seed language; and
- the Release solution and installed-game plugin builds completed with zero
  warnings and all 14 conclusion and 49 runtime-boundary checks passed, while
  the hosted-reference build completed and the semantic-versioned DLL and
  Thunderstore package validators accepted the three-assembly package.

**Produced:** `PresentedConclusionCard`, `PresentedContextGroup`,
`PreviewConclusionPresentation`, `PreviewPanelDocument`, the accepted-family
and outcome copy mapper, the expanded bounded renderer, and focused mapping,
conflict, cache, stage-separation, snapshot, and placement fixtures. New
predicates, preferences, comparisons, charts, detailed evidence browsing,
localization, and human in-game validation remained outside this story.

## Phase 3 - Accept the installed experience

### PRES-07: Validate the complete New Game experience

**State:** Implemented on 2026-08-12; pending human revalidation.

As a player installing DSP Seed Scanner, I want the hands-off panel to behave
correctly through real New Game preview changes so that I can rely on what it
shows before starting a game.

**Return:** The packaged mod is exercised end to end in the supported game,
and the completed presentation behavior is documented accurately for players
and maintainers.

**Acceptance gate:** Human in-game validation confirms panel placement and text
fit, responsive spinner and controls during a cache miss, immediate cache-hit
reuse, exactly one resolution attempt per completed preview load, correct
replacement after seed changes, safe cancellation on preview exit, complete
context conclusions, and terminal unsupported or failure states without stale
results. Automated suites, build, exact package validation, and isolated
installation also pass for the accepted commit.

**Out of scope:** Further UX features, adaptive anchoring, broader runtime or
mod compatibility, performance guarantees beyond the accepted operation bound,
publication, telemetry, comparison, preferences, exports, and closure of
unrelated technical debt.

**Implemented:** The first isolated 4K run exposed two acceptance defects and
one misleading diagnostic. The panel now scales its readable coordinate space,
reserves additional bottom clearance, and replaces the tall indented document
with three wrapped columns for strengths, preference-sensitive results, and
limitations. Tradeoffs remain in the preference-sensitive column and cautions
remain in the limitations column. Unknown and not-applicable components are no
longer rendered, and the redundant immediate and detailed section labels were
removed.

The live preview now carries a presentation-only lookup from stable system
identifiers to DSP's display name and star-type text. Cards never expose an
unresolved internal system identifier. Distance cards require a decisive
light-year fact, render it to roughly three significant figures, and include
the player-visible system name when the report identifies one. Wrapped cards
replace list-era ellipses. Unsupported states now distinguish other-plugin or
preloader uncertainty from a DSP-version or generation-runtime mismatch.

**Human evidence so far:** In the isolated supported runtime, the panel became
readable at 4K, cache-miss progress remained visibly active, controls remained
usable despite the expected performance cost, and a 204-solid-planet scan
completed within the previously estimated acceptable duration. The completed
result rendered without text overlap or clipping. That pass also found the
excessive footprint, unknown cards, raw system identifiers, omitted distance
values, and redundant stage headings repaired above; therefore it did not
close this story's human gate.

**Automated evidence:** The Release solution and installed-game plugin build
with zero warnings, all 14 conclusion checks and 49 runtime-boundary checks
pass, system-display metadata stays outside the evidence identity and cache,
and focused fixtures enforce outcome-to-column mapping, suppression of unknown
and not-applicable cards, player-visible system labels, short distance values,
bottom clearance, stale-state rejection, and bounded copy. The installed- and
hosted-reference plugin builds and the exact semantic-versioned package
validation also pass locally; pushed CI remains part of the final gate.

**Remaining gate:** Human revalidation must confirm the compact panel clears
the right-side legend and Back control, the three columns and colors are
readable, wrapped cards contain no internal IDs or avoidable ellipses, and the
previously required lifecycle, cache-hit, cancellation, replacement, and
unsupported-state observations pass on the final installed candidate.

**Second human pass and repair:** The first three-column candidate was readable
and retained acceptable scan responsiveness, but repeated each context heading
inside independently flowing columns, used an opaque full-panel background,
and remained too small. The second pass also demonstrated that revisiting an
identical seed did not use the cache. Investigation found that cache admission
bounded every evaluator report before filtering to the complete-stage payload;
large but valid preview role/grouping sets could therefore reject a small
cacheable complete bundle.

The repaired renderer now treats each context as one card spanning the three
aligned outcome columns, centers one neutral context heading, orders cards
with all three outcomes first, and packs sparser contexts afterward. The full
surface has no background, while conclusion and class-heading fonts are
larger. The cache applies its report-count ceiling to the complete-stage
payload it actually serializes; a focused regression stores and reloads that
payload from a successful result containing more than 1,024 total reports.
Dark Fog conclusions remain unchanged pending broader seed sampling.

This second repair passed all 14 conclusion and 49 runtime-boundary checks and
the installed-game plugin build with zero warnings. Human revalidation and the
pushed package/CI gates remain required before PRES-07 acceptance.

## Roadmap coverage

| Accepted presentation requirement | Covered by |
| --- | --- |
| Completed-preview recognition, duplicate coalescing, and current-session identity | PRES-01 |
| Responsive main-thread progress and safe cancellation | PRES-02 |
| Versioned atomic bounded local cache and manual clearing | PRES-03 |
| One automatic resolution attempt, cache-or-scan behavior, no retry, and stale suppression | PRES-04 |
| Configurable four-corner panel, spinner, progress, and operational states | PRES-05 |
| Immediate and complete neutral conclusions grouped by every accepted context | PRES-06 |
| Deferred human validation, package verification, and accurate handoff documentation | PRES-07 |

## Roadmap-wide exclusions

This roadmap does not add adaptive panel placement, player scoring or required
preferences, manual scan or retry controls, seed comparison, batch search,
parallel generation, background DSP generation, shared caches, databases,
exports, telemetry, new conclusions, wider compatibility, publication, or
package icon and marketing-copy refinement. TD-003 remains independent and
does not block this roadmap.

## Completion

The roadmap is complete only when PRES-01 through PRES-07 are individually
accepted and PRES-07 records the sole human in-game validation of the installed
experience. Roadmap completion does not authorize external publication or
additional presentation features.
