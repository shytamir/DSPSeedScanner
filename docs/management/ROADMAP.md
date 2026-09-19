# Panel Improvement Roadmap

[PROJECT.md](../PROJECT.md) is the sole authority for project steering and work
status, including historical dispositions. This document contains scope,
requirements, or technical evidence; it does not track status.

**Product scope:** The standalone [Panel Validation Findings](PANEL-VALIDATION-FINDINGS.md)
and the owner's subsequent execution constraints govern this roadmap. Older
feature requests, archived plans, and their evaluation gates supply no scope
or prerequisites here.

## Execution boundary

- The implementer must not launch or run DSP, in the installed environment,
  an isolated copy, a headless process, a test harness, or through another tool
  or person acting on the implementer's behalf. Do not execute game generation
  or initialize game/Unity runtime code as a substitute for launching DSP.
- The implementer must not alter the machine's game environment: no plugin
  deployment, BepInEx/config/cache edits, dependency replacement, mod toggling,
  installation changes, or temporary changes followed by rollback. A rollback
  plan does not authorize an otherwise forbidden action.
- Existing saves are off limits: do not open, read, copy, modify, migrate,
  replace, or delete them. Validation uses New Game preview only.
- Read-only inspection of installed assembly metadata and native code is
  allowed. Build against read-only references; run repository-owned pure .NET
  tests and packaging tools. Write outputs only within the repository's build,
  ignored artifact, or permitted temporary areas, never into the game or saves.
- **Owner-only exception:** At the final handoff gate, the owner may manually
  install the prepared scanner candidate and run its New Game preview check.
  This does not authorize the implementer to perform either action. The check
  does not start gameplay or access an existing save. There are no earlier
  human validation gates, intermediate playtests, or owner-run investigations.
- Every investigation must finish in Phase 1 before any later-phase work
  begins. A newly necessary investigation reopens Phase 1 and suspends later
  phases until its gate passes again. Routine implementation debugging stays
  within the relevant story; it cannot conceal a new unresolved native premise.

## Scope and design discipline

Implement every finding; do not omit difficult cases, narrow eligibility,
weaken thresholds, or silently turn missing evidence into absence to make a
story pass. Keep existing behavior outside the requested changes. In particular,
do not add a rare-resource ranking scheme, an Aquatica eligibility exclusion
based on another displayed role, a required distance-origin label, additional
theme views, a score, configuration controls, or a population research project.

Prefer the existing extraction, aggregation, selection, projection, rendering,
tests, and packaging paths. Use verified native units and calculations where
they provide the requested fact. Extend only the data actually required; avoid
generic rule engines, compatibility frameworks, parallel evidence pipelines,
or speculative cache redesign. Tests belong with the changed behavior.

The findings own the numeric rules and presentation requirements. The stories
below identify bounded delivery units and cite their source sections rather
than maintaining a second threshold catalogue.

## Blocker procedure

An implementer may stop and declare a blocker when necessary evidence is
unavailable under the execution boundary, requirements genuinely conflict,
or proceeding would require a scope or permission change. Record the affected
story, evidence checked, exact obstacle, impact, and smallest additional input
needed in [PROJECT.md](../PROJECT.md). Request that input before dependent
work proceeds.

This exception permits clarification, not delegation of investigation or an
early human validation task. Investigations remain human-free: do not ask the
owner to run probes, gather game measurements, or perform trial installations.
Do not reopen settled findings or escalate routine implementation choices.
An unresolved investigation blocks all later phases; other blockers stop their
dependent work. A blocker cannot be bypassed by dropping a requested feature.

## Story completion rule C

Every story's Closeout field incorporates all of C:

1. Update [PROJECT.md](../PROJECT.md) following its tracking rules, with the
   story result and narrow relevant evidence or evidence links. It alone records
   story, gate, milestone, blocker, handoff, and acceptance state.
2. Update affected contracts, user documentation, and the documentation index
   only where that story changes their meaning. Preserve the findings as owner
   input; do not rewrite them to match an easier implementation. Do not enroll
   or close older tracked requests as a side effect.
3. Build affected projects for implementation stories and run their relevant
   checks. Investigations and documentation-only changes use their applicable
   static checks. Report skipped checks accurately; a build is not gameplay
   or visual validation.
4. Inspect status and the final diff; stage only the story's intended paths.
   Include management updates in its closing commit and finish the story with
   the authorized final push to `main`. Fetch first; reconcile through normal
   non-destructive Git operations and a fast-forward-only update where applicable.
   Never force-push or rewrite history. A failed push leaves closeout incomplete.
5. Report the resulting commit and actual push/CI outcome in the execution
   report; do not create a follow-up commit merely to embed a commit's own hash.
   Existing build/test/package CI is sufficient; do not create a new publishing
   pipeline. A push
   before final handoff records technically completed work awaiting owner
   acceptance, not an accepted release. Store publication/tagging is outside
   this roadmap.

Keep these story definitions free of progress entries. Epics complete when
their child stories complete; phase gates and milestones require supporting
evidence and are not extra stories. Record their dispositions only in
[PROJECT.md](../PROJECT.md).

## Phases, gates, and milestones

| Phase | Work | Exit gate and declarable milestone | Who validates |
| --- | --- | --- | --- |
| 1. Investigate | E1: I01-I03 only | G1: native facts, evidence routes, safe checks, and handoff-case preparation are established; no unresolved investigation. Declare M1: Implementation evidence ready. | Implementer, human-free |
| 2. Implement | E2-E5: F01-F04, M01-M02, S01-S03, T01-T04 | G2: every requested behavior is implemented, story checks pass, integrated builds/tests pass, required contract/cache handling is in place, and management commits are on main. Declare M2: Panels technically complete. | Implementer, human-free |
| 3. Prepare handoff | E6: D01; D02 through its Ready for handoff checkpoint | G3: one identified candidate package and the concise owner checklist are complete and linked to the exact tested source/artifact. Declare M3: Ready for owner preview. | Implementer, human-free |
| 4. Final handoff | D02 owner check and closure | G4: owner completes the minimal preview check on that candidate; any reported defects are resolved and the final management update is pushed to main. Declare M4: Owner accepted. | Owner, only human validation gate |

No later phase starts before its preceding gate passes. Within Phase 2, the
listed story dependencies apply; independent stories need no extra gates.
G4 failures return the affected story to implementation and the affected
technical/package checks to G2/G3. Recheck only the changed or failed behavior
at the same final handoff gate; do not impose a second full owner test programme.
Reopen G1 first if a failure reveals a genuinely new investigation.

## Epic E1: Verified evidence for the requested panels

**Outcome:** Implementation can use known native facts and existing repository
paths without launching the game or guessing semantics. All three stories are
investigations; record their short findings and evidence references in
[PROJECT.md](../PROJECT.md), with larger temporary inspection output kept
untracked.

### PANEL-I01: Native resource units

- **Value:** Oil and Deuterium figures mean what the player sees in-game.
- **Scope:** Read the native oil resource UI writer and rate fields; establish
  the total-rate formula, units, precision, and relevant settings treatment.
  Trace Deuterium source units through current extraction to `/s` presentation.
- **Out of scope:** Running native methods, measuring a live game, changing
  formulas or thresholds, recalibration, or implementation.
- **Definition of done:** Exact inspected version/member references and the
  transformations are recorded; pure numeric check cases are specified. The
  agreed Deuterium cutoff remains `>= 0.15/s`; the starter anecdote is not a
  separate runtime threshold. Missing native evidence is a blocker, not a guess.
- **Approach:** Reuse native calculations and existing quantity formatting
  where appropriate; do not design a unit-conversion framework.
- **Closeout:** C, including management updates and final push to `main`.

### PANEL-I02: Planet and star attribution

- **Value:** Moon order, containment, sphere size, and Aquatica rows refer to
  the correct physical objects.
- **Scope:** Verify parent/moon ordering, birth-planet identity, star-centric
  containment including moons, maximum sphere radius units, Dyson luminosity,
  O/blue classifications, and Aquatica identity from read-only native sources.
  Map them to existing system identities and pairwise distances.
- **Out of scope:** Game execution, live generation, theme-value research,
  recalibrating sphere ranges, or adding a duplicate/eligibility policy.
- **Definition of done:** The fields/calculations needed by F03, S01-S03 and
  T02 have source-backed definitions and pure boundary/attribution cases.
  Stellar radius and maximum sphere radius remain distinct. No unresolved
  geometry or catalogue guess is passed into implementation.
- **Approach:** Use existing native topology and radius facts; avoid an orbit
  simulator, generalized astronomy model, or localized-name heuristic.
- **Closeout:** C, including management updates and final push to `main`.

### PANEL-I03: Minimal data routes and verification inputs

- **Value:** All findings can be delivered without unnecessary collection,
  cache work, or an owner investigation round.
- **Scope:** Map findings to existing Core/Runtime/Plugin paths, applicable
  column conventions, selection bounds, and cache fields. Identify only the
  additional per-system totals or attribution actually required. Inspect the
  repository tests/CI and prepare the source of the final seed/settings recipe
  from existing attributable evidence; distinguish real evidence from synthetic
  test fixtures. Resolve ordinary choices from existing behavior and the findings.
- **Dependencies:** I01 and I02 for the verified native premises.
- **Out of scope:** Product reprioritization, new thresholds or scoring, broad
  seed searches, native execution, save inspection, or owner-supplied test labour.
- **Definition of done:** Each implementation story has an evidence route and
  human-free check strategy. Required semantic/cache version handling is scoped;
  the four-row color intersection needs no new retained data. The handoff can
  be prepared without asking the owner to find suitable seeds. Genuine gaps
  that cannot be resolved within these rules are reported through the blocker
  procedure before G1; settled owner rules are not reopened.
- **Approach:** Reuse current bounded aggregates, identities, tests, and CI.
  Do not add data simply because it may be useful later.
- **Closeout:** C, including management updates and final push to `main`.

## Epic E2: Useful fresh-start guidance

**Outcome:** Starting-system conclusions identify the requested opportunities
and limitations, with usable Deuterium supply reported consistently. Requires G1.

### PANEL-F01: Single starter-giant conclusion

- **Value:** The player sees one clear starter-giant verdict.
- **Scope:** Implement Conclusions / Fresh start item 1: Fire Ice is a
  Strength; Deuterium is a Limitation; remove the duplicate opposite-column
  presence/absence presentation for that giant.
- **Out of scope:** Changing giant generation, gas rates, or other contexts.
- **Definition of done:** Focused cases for each starter product show the
  required polarity exactly once. Unrelated starter facts remain unchanged.
- **Approach:** Adjust existing predicates/projection locally; add no scoring
  or special game-generation path.
- **Closeout:** C, including management updates and final push to `main`.

### PANEL-F02: Home-system Fire Ice adequacy

- **Value:** Useful mineable Fire Ice is distinguished from inadequate supply.
- **Scope:** Apply Conclusions / Fresh start items 3-4 using system totals
  and both stated amount/group minima; preserve other scarcity conclusions
  and their polarity.
- **Out of scope:** Single-planet minima, other scarcity threshold changes,
  gas-product coloring, or resource generation changes.
- **Definition of done:** Split-across-planets cases and each side/equality of
  both minima produce the required result; missing data is not treated as zero.
  Existing scarcity regression cases retain their outcomes. Any changed cached
  conclusion semantics are versioned with this change.
- **Approach:** Reuse the home-system raw aggregate and existing outcome rules.
- **Closeout:** C, including management updates and final push to `main`.

### PANEL-F03: Sibling-moon power conclusions

- **Value:** Power guidance describes the relevant neighbouring moons.
- **Scope:** Apply Conclusions / Fresh start item 2 to one or two sibling
  moons of the home moon, using the two specified power limits independently
  for Solar and Wind. Remove home-planet power conclusions.
- **Out of scope:** A combined moon verdict, other-system moon advice, or
  changing the statistics table's existing Solar/Wind values.
- **Definition of done:** Direct-star homes and homes without siblings emit no
  sibling advice. Each eligible sibling has one Solar and one Wind conclusion,
  each in exactly one correct column, including equality and mixed-outcome cases.
- **Approach:** Reuse the parent/body inventory verified by I02 and existing
  range evaluation; do not infer siblings from display names.
- **Closeout:** C, including management updates and final push to `main`.

### PANEL-F04: Nearest qualifying Deuterium supply

- **Value:** Both panels identify usable supply without favouring a richer
  but farther giant over the nearest qualifying one.
- **Scope:** Implement Conclusions / Fresh start item 5 and Statistics /
  Cluster Deuterium together: shared rate eligibility, conclusion distance
  classification, and the statistics title and retained search cap.
- **Out of scope:** New gas-rate bands, a hard-coded starter exclusion, or a
  new Deuterium color rule. The statistics cap does not restrict the conclusion.
- **Definition of done:** Cases cover below/equal/above the rate cutoff,
  nearer qualifying versus farther richer giants, no match, distance-band
  boundaries, and the inclusive statistics cap. Each panel applies its own
  stated distance treatment and shows correctly attributed values.
- **Approach:** Reuse the gas candidate and system-distance data; share the
  eligibility calculation without merging the two panels' different rules.
- **Closeout:** C, including management updates and final push to `main`.

## Epic E3: Relevant megafactory resource destinations

**Outcome:** Megafactory conclusions present the requested resource systems
instead of broad rare-resource summaries. Requires G1.

### PANEL-M01: Targeted rare-resource presentation

- **Value:** Players see destinations for the three useful rare resources.
- **Scope:** Implement Conclusions / Megafactory items 2-3: the specified
  Spiniform and Grating quantity qualifications, Organic presence, qualifying
  non-Aquatica deposits, nearest matching systems, and removal of Many rares.
  Preserve the owner's existing rare-resource preference rule.
- **Out of scope:** A new combination-versus-distance ranking, an Organic
  quantity floor, statistics candidate selection changes, or theme generalization.
- **Definition of done:** The three requested resources and their actual
  qualifying systems are presented; exact threshold and non-Aquatica cases
  pass; unrelated rare summaries are removed from this context. Applicable
  existing column conventions are retained. The statistics rare rows are
  unchanged by this story; required data/contract/cache changes are validated.
- **Approach:** Extend the existing resource aggregation/projection only where
  I03 establishes missing facts; do not implement a new ranking subsystem.
- **Closeout:** C, including management updates and final push to `main`.

### PANEL-M02: Plentiful resource-system conclusions

- **Value:** Players can identify nearby systems with substantial mixed supply.
- **Scope:** Implement Conclusions / Megafactory item 4 using per-resource,
  per-system totals, the strict amount threshold, and at least three qualifying
  types; select for presentation by nearness to home.
- **Out of scope:** A universal megafactory score, throughput estimates,
  unrelated statistics columns, or invented amount/distance bands.
- **Definition of done:** Two qualifying types fail eligibility; three pass;
  exact amount equality does not pass. Split deposits aggregate across the
  system and retain correct resource identity. Nearest qualifying systems are
  presented using the applicable conventions established in I03. Amount/rate
  semantics are not mixed; cache hits and fresh results agree for this feature.
- **Approach:** Use bounded per-system totals in the existing complete scan.
  Retain only data needed by the conclusion, without a second scan or data dump.
- **Closeout:** C, including management updates and final push to `main`.

## Epic E4: Sphere and nearby Aquatica comparison

**Outcome:** Luminous sphere candidates and the requested Aquatica hosts have
correct geometry, facts, and ordering. Requires G1.

### PANEL-S01: Sphere-only geometry conclusions

- **Value:** Sphere conclusions refer to nearby qualifying luminous stars.
- **Scope:** Implement Conclusions / Sphere / energy items 1-2 and the
  Megafactory item 1 removal. Apply verified star-centric containment and
  maximum-radius facts, the luminosity filter, and distance selection.
- **Out of scope:** Changing existing shell/count ranges, altering other
  luminosity conclusions, receiver modelling, or general sphere design advice.
- **Definition of done:** Below/equal luminosity cases and nearer versus
  larger/farther candidates select correctly; no-match behavior remains honest.
  The Megafactory containment duplicate is gone. Moon geometry follows I02,
  and required conclusion/cache semantic changes are checked together.
- **Approach:** Reuse native radius/topology facts and existing geometry
  projection; expose the same verified facts for S02 and T02.
- **Closeout:** C, including management updates and final push to `main`.

### PANEL-S02: Notable-star sphere rows

- **Value:** Players can compare blue giants, the brightest star, and O stars.
- **Scope:** Apply Statistics / Notable stars items 1-2 for the sphere rows:
  blue giants first/brightest first/green, cluster brightest next, remaining
  O stars by home distance; add Distance, Max Sphere, and Contained.
- **Dependencies:** S01 for verified shared geometry facts.
- **Out of scope:** Aquatica rows (S03), replacing stellar Size, applying the
  conclusions luminosity cutoff to this table, or a new global exclusion rule.
- **Definition of done:** The requested group order, home-distance values,
  meter radius, contained counts, and whole-blue-row color pass projection
  checks. No-blue cases are covered; existing star facts remain intact.
- **Approach:** Extend the current star table and renderer with the required
  facts and local styles; use the existing distance matrix.
- **Closeout:** C, including management updates and final push to `main`.

### PANEL-S03: Final Aquatica group

- **Value:** Players see up to five relevant Aquatica-hosting non-O stars
  nearest to the brightest star.
- **Scope:** Apply Statistics / Notable stars item 3 and its final-group
  ordering: theme eligibility, five-star cap, brightest-star distance for both
  selection and sorting, and the two required blank cells.
- **Dependencies:** S02 for the table structure and sphere-group ordering.
- **Out of scope:** Deposit-based Aquatica eligibility, O hosts, a general
  themes panel, exclusion because another role is displayed, or extra labels.
- **Definition of done:** Zero, fewer than five, five, and more than five
  matches behave correctly; multiple Aquatica planets do not inflate the star
  count. A case with different home/brightest distances verifies the origin.
  The group is last; Luminosity and Contained are blank exactly as requested.
- **Approach:** Use verified catalogue identities and pairwise distances.
  Add only the attributed theme fact needed for this selection.
- **Closeout:** C, including management updates and final push to `main`.

## Epic E5: Clear home and cluster resource statistics

**Outcome:** The existing factual tables display the requested oil, moon,
containment, and resource emphasis accurately. Requires G1.

### PANEL-T01: Native oil totals

- **Value:** The home table shows the same oil rate as the game.
- **Scope:** Apply Statistics / Home system item 1 using I01's verified
  native conversion and display precision, retaining the wells count.
- **Out of scope:** Finite-ore amount changes, production estimates, or
  changing scarcity conclusions that consume native amounts.
- **Definition of done:** Pure cases for multiple wells, total conversion,
  and rounding match the inspected native writer. Oil is displayed as a rate;
  other resource formatting and cached raw amount semantics remain correct.
- **Approach:** Correct the existing oil cell projection; do not change the
  shared ore formatter to accommodate a different quantity.
- **Closeout:** C, including management updates and final push to `main`.

### PANEL-T02: Moon labels and contained-home highlight

- **Value:** Players can identify moon positions and the home orbit opportunity.
- **Scope:** Apply Statistics / Home system items 2 and 4: moon-only second
  body lines in nearest-to-farthest parent order, and full green text only for
  the contained home planet.
- **Dependencies:** S01 for shared verified containment facts.
- **Out of scope:** Extra lines on giants/non-moons, coloring other contained
  planets through this rule, or changing the body inventory.
- **Definition of done:** Multiple parents/siblings, the home moon, a direct
  home planet, and contained/uncontained cases label and color only the right
  rows. The body name and existing table values remain intact.
- **Approach:** Reuse native parent/order and birth identity, plus the current
  row-height/wrapping path; do not create another topology model.
- **Closeout:** C, including management updates and final push to `main`.

### PANEL-T03: Rare-resource text emphasis

- **Value:** Useful deposits and nearby rare candidates stand out in place.
- **Scope:** Apply Statistics / Home system item 3 and all Statistics /
  Cluster rare resources rules, including the four-row displayed-system
  intersection and its matching candidate cells.
- **Out of scope:** Changing closest/alternative preference, adding a distance
  cap, searching hidden system resources, removing other home resources,
  coloring giant Fire Ice gas, or adding retained scan/cache data.
- **Definition of done:** Only the two requested home vein entries get the
  resource highlight. Exact distance boundaries and ordinary cells pass;
  both displayed candidate columns participate in the four-row intersection.
  Non-displayed resources cannot qualify a system. Required green matching
  cells and other distance colors do not leak into headings or unrelated cells.
- **Approach:** Use cached candidate host IDs and local text/cell coloring.
  Keep the existing two-candidate selection and payload.
- **Closeout:** C, including management updates and final push to `main`.

### PANEL-T04: Unipolar amount and distance colors

- **Value:** Players can judge Unipolar supply and travel distance separately.
- **Scope:** Apply both Statistics / Cluster Unipolar Magnets color rules
  independently to their named cells on every existing row.
- **Dependencies:** T03 for the local resource-cell styling path.
- **Out of scope:** Row selection, new ranks, whole-row coloring, or changed
  numbers/units.
- **Definition of done:** Below/equal/above each threshold and mixed red/green
  outcomes color the correct cells. Other cells, exact quantities, and row
  order retain current behavior; comparisons use underlying values.
- **Approach:** Reuse T03's table styling path; use direct comparisons rather
  than a configurable styling framework.
- **Closeout:** C, including management updates and final push to `main`.

## Epic E6: Reviewable candidate and final acceptance

**Outcome:** The owner receives an identifiable working candidate and can
accept it with one short preview session. Requires G2; it introduces no new
product behavior or deferred feature testing.

### PANEL-D01: Verified candidate package

- **Value:** The owner receives the exact candidate supported by technical checks.
- **Scope:** Run/reuse the existing build, test, versioning, and package
  workflow for the completed source; collect the scanner-only package,
  hashes, source commit, build/run identity, and concise check results.
- **Out of scope:** Installation, launching DSP, machine changes, uploading
  to a store, new CI infrastructure, or claiming in-game validation.
- **Definition of done:** The required technical checks below pass for the
  identified source/artifact. Package contents match the three scanner DLLs
  and existing package contract; no external game/dependency binaries or saves
  are included. Required docs describe the delivered behavior. The candidate
  and reports are available without the owner building or assembling anything.
- **Approach:** Use existing scripts and CI artifacts; rebuild only where the
  final source/version or unresolved results require it.
- **Closeout:** C, including management updates and final push to `main`.
  Freeze the artifact hash and its build-source commit for D02; a later
  documentation commit does not change the identity of the tested binary.

### PANEL-D02: Minimal owner preview and closure

- **Value:** The owner can accept the panels without seed hunting or technical work.
- **Scope:** Prepare the handoff package/checklist below from I03 and D01;
  record Ready for handoff in `PROJECT.md` at G3. At G4 receive the owner's brief result, resolve
  any actual defects through the relevant stories, and record acceptance in
  `PROJECT.md`.
- **Dependencies:** D01. The human step occurs only after G3.
- **Out of scope:** Asking the owner to compile, collect measurements, run
  boundary matrices, inspect logs without a failure, search for seeds, change
  other mods, or use an existing save. The implementer never installs or runs it.
- **Definition of done:** At the technical checkpoint the complete candidate,
  source-backed recipe/expectations, and concise instructions are ready. Final
  completion additionally requires the owner's G4 result for that exact
  candidate, resolution of reported failures, and factual management closure
  pushed to main. A ready package alone is not owner acceptance.
- **Approach:** Reuse the existing package and case evidence; keep instructions
  to the few visual/runtime checks automated tests cannot establish.
- **Closeout:** C at final completion. In `PROJECT.md`, record the candidate's
  original build commit/hash separately from the later acceptance-documentation
  commit.

## Technical gate checks

Story checks use pure repository-owned fixtures for numeric boundaries,
selection, attribution, and presentation. They do not execute installed DSP
code. At G2/D01, use the existing commands as applicable to the final source:

```powershell
dotnet build DSPSeedScanner.sln --configuration Release
dotnet run --project tests/DSPSeedScanner.Core.Tests/DSPSeedScanner.Core.Tests.csproj --configuration Release --no-build
dotnet run --project tests/DSPSeedScanner.Runtime.Tests/DSPSeedScanner.Runtime.Tests.csproj --configuration Release --no-build
dotnet build src/DSPSeedScanner.Plugin/DSPSeedScanner.Plugin.csproj --configuration Release
```

The plugin build reads installed reference assemblies and writes repository
outputs; it does not deploy or run the game. CI's compile-only references are
not proof of native runtime compatibility. Use the existing
`Test-BuildArtifact.ps1`, `New-ThunderstorePackage.ps1`, and
`Test-ThunderstorePackage.ps1` with the exact version/artifacts produced by the
current workflow. No new package format is needed. Check documentation links,
diff scope, intended version changes, and absence of generated/dependency data
from commits. Reuse passing checks on the same unchanged source rather than
repeating work without a reason.

## Final handoff package and minimum owner check

D02 prepares one small handoff note containing instructions and evidence,
linking to `PROJECT.md` for handoff and acceptance status, with:

- A direct candidate-package link, version, build-source commit, and SHA-256;
  keep generated binaries/reports untracked or in the existing CI artifacts.
  Keep that candidate fixed even if a later management-only push creates
  another CI artifact; a replacement candidate needs its own check evidence.
- Exact short owner-only installation steps for the three scanner DLLs using
  the existing package layout, with no changes to other mods or configuration.
- One primary seed/settings recipe selected by the implementer from attributable
  existing evidence, plus a second only if needed for an essential visible case
  absent from the first. State what is expected on each; do not label synthetic
  fixture results as observations of a real seed.
- A few expected rows/text/color/order examples and the location of changed
  sections, so the owner need not reconstruct the findings or recalculate rules.
- A concise technical result and limitation statement: automated/static checks
  passed as listed; the implementer has not run or deployed the game.

The delivered owner instructions must be short and concrete, following this
shape with the actual package/seed details filled in:

1. Install the supplied scanner candidate using the included short steps.
   Open **New Game** and enter the supplied seed/settings. Do not start
   gameplay or load an existing save.
2. Wait for the scan to finish and scroll both panels once. Compare the marked
   examples: the changed conclusions, oil rate/moon labels/home highlight,
   rare/Unipolar colors, and notable-star columns/order. Check readability and
   absence of overlap or truncation. Use the supplied expected values; no
   resource measurements or recalculation are required.
3. Use the supplied second preview only if the checklist requires it, then
   return to the menu. Reply **Pass**, or name the mismatched section; a
   screenshot is useful only if something is wrong.

The owner is not asked to validate every numeric edge or rare combination;
those are technical story checks. Do not expand human work to compensate for
missing implementation evidence. If G3 cannot provide an honest prepared
case/checklist under the restrictions, declare the blocker rather than asking
the owner to investigate. Owner-only preview acceptance settles the final
runtime/readability check, not a broad compatibility or performance claim.

## Coverage and ownership

| Findings section | Delivery stories |
| --- | --- |
| Conclusions: Fresh start, items 1-5 | F01; F02 (items 3-4); F03 (item 2); F04 (item 5) |
| Conclusions: Megafactory, item 1 | S01 (single owner of containment relocation) |
| Conclusions: Megafactory, items 2-4 | M01 (items 2-3); M02 (item 4) |
| Conclusions: Sphere / energy | S01 |
| Statistics: Home system, items 1-4 | T01; T02 (items 2, 4); T03 (item 3) |
| Statistics: Cluster Deuterium | F04 (shared eligibility, distinct panel rules) |
| Statistics: Cluster rare resources | T03 |
| Statistics: Cluster Unipolar Magnets | T04 |
| Statistics: Notable stars | S02 (sphere facts/order); S03 (final Aquatica group) |
| Necessary native/data investigations | I01-I03, exclusively Phase 1 |
| Technical package and minimal human acceptance | D01-D02, G3/G4 |

References to a story from two source sections do not create duplicate work.
Management updates and checks are included in story completion. No additional
cleanup, population study, or documentation workstream is implied.

Return to the [findings](PANEL-VALIDATION-FINDINGS.md) or
[documentation index](../INDEX.md).
