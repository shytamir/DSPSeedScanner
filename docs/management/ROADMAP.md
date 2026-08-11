# New Game Presentation Roadmap

**Status:** In progress. PRES-01 is implemented and pending acceptance.

**Active user story:** PRES-01 is at its acceptance gate. PRES-02 remains
inactive until PRES-01 is accepted.

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
  identity creates a new resolution attempt and may use its cached result.
- Immediate preview conclusions remain useful while complete raw evidence is
  pending. Raw completion produces a new attributed report and never silently
  rewrites the earlier report.
- Full raw scanning is automatic for this New Game workflow. It remains one
  serialized, bounded operation and may yield or cancel only at safe runtime
  boundaries.
- A result may update the panel only while its preview session and complete
  generation identity are still current.
- Only successful complete results may be persisted. Cache corruption,
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

**State:** Implemented on 2026-08-12; pending acceptance.

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

**State:** Approved; inactive.

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

### PRES-03: Reuse trustworthy local results

**State:** Approved; inactive.

As a player revisiting a preview, I want an already completed local scan reused
so that I do not repeatedly wait for identical evidence.

**Return:** A bounded local cache in the mod configuration area stores and
retrieves complete results by full generation identity, evidence coverage, and
the applicable scanner contract versions.

**Acceptance gate:** Automated storage tests prove deterministic key equality,
atomic replacement, bounded retention, successful round trips, and safe misses
for absent, partial, failed, cancelled, corrupt, incompatible, or obsolete
entries. A documented manual clear operation removes cached scanner results.

**Out of scope:** Shared or cross-machine caches, cloud storage, databases,
cache browsing UI, migration promises, incomplete-result resumption, and
changes to conclusion semantics.

## Phase 2 - Deliver the hands-off workflow

### PRES-04: Resolve every preview automatically once

**State:** Approved; inactive.

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

### PRES-05: Show current operational state

**State:** Approved; inactive. The corner-anchor requirement was resolved on
2026-08-12.

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

### PRES-06: Present concise neutral conclusions

**State:** Approved; inactive.

As a player deciding whether a seed suits an intended run, I want its supported
conclusions grouped by context so that I can understand strengths, limitations,
tradeoffs, and uncertainty without decoding raw statistics.

**Return:** The panel presents an identity header, immediate preview summary,
and context-grouped conclusion cards for the accepted fresh-start,
megafactory, Dark Fog farming, compact-expansion, sphere or energy, and
decision-relevant-trait contexts. The pending detailed section shows activity
until complete evidence replaces it, and cached results are identified.

**Acceptance gate:** Snapshot and mapping tests cover every accepted outcome,
tradeoff, unknown, not-applicable state, subject attribution, and supported
context using bounded copy. Immediate and complete reports remain visibly
distinct, all decisive conflicts survive, and no score, universal verdict,
unsupported claim, or raw-number wall is introduced.

**Out of scope:** New predicates or ranges, player preference controls,
discovery-mode controls, seed comparison, sorting, exports, charts, detailed
evidence browsing, localization, and package-branding refinement.

## Phase 3 - Accept the installed experience

### PRES-07: Validate the complete New Game experience

**State:** Approved; inactive.

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
