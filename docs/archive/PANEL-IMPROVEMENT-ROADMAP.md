# Panel Improvement Roadmap — Historical Plan

[PROJECT.md](../PROJECT.md) is the sole authority for project steering and
all work state, including historical dispositions. This document records the
scope, requirements and procedures of the 2026-09-19 panel plan. Its criteria
are phrased as historical requirements, not as completion or acceptance claims.

The [owner findings](PANEL-VALIDATION-FINDINGS.md) and subsequent execution
constraints governed the plan. Older requests and archived plans supplied no
additional scope or prerequisites.

## Execution boundary

- Implementer execution of DSP was prohibited, including installed, isolated,
  headless or harness execution, game generation, Unity initialization, and
  execution through another tool or person acting on the implementer's behalf.
- Changes to the machine's game environment were prohibited: plugin deployment,
  BepInEx/config/cache edits, dependency replacement, mod toggling, installation
  changes and temporary changes followed by rollback. A rollback plan granted
  no exception.
- Existing saves were off limits for reading, copying, modifying, migrating,
  replacing or deleting. Human validation was limited to New Game preview.
- Read-only inspection of installed assembly metadata/native code, compilation
  against read-only references, repository-owned pure .NET tests and packaging
  were allowed. Outputs were restricted to repository build/ignored artifact
  or permitted temporary areas, never the game or saves.
- The owner-only final handoff exception allowed manual candidate installation
  and a New Game preview check, without starting gameplay or accessing a save.
  It did not authorize implementer installation/execution or earlier human
  gates, intermediate playtests or owner-run investigations.
- All investigations had to finish in Phase 1 before later phases began. A
  newly necessary investigation would have reopened Phase 1 and suspended later
  work until its gate passed again. Routine debugging remained within its story
  and could not conceal an unresolved native premise.

## Scope and design discipline

Every finding was required. Difficult cases, eligibility, thresholds and missing
coverage could not be weakened for expedience; unrelated behavior had to remain
unchanged. The plan excluded a new rare-resource ranking scheme, Aquatica
exclusion based on another displayed role, mandatory distance-origin labels,
extra theme views, scoring, configuration controls and population research.

The prescribed approach favored existing extraction, aggregation, selection,
projection, rendering, tests and packaging. Native units/calculations were to
be verified, and only necessary data was to be added. Generic rule engines,
compatibility frameworks, parallel evidence pipelines and speculative cache
redesign were excluded. Tests belonged with the changed behavior.

The findings supplied the numeric rules and presentation requirements. Story
references did not create a second threshold catalogue.

## Blocker procedure

The implementer could declare a blocker when evidence was unavailable within
the execution boundary, requirements conflicted, or a scope/permission change
was necessary. PROJECT.md was to record the affected story, evidence checked,
obstacle, impact and smallest additional input needed. Dependent work had to
wait for that input.

This permitted clarification, not delegated investigation or early human
validation. Owner probes, measurements and trial installations were excluded.
Settled findings and routine choices were not grounds for new approval gates.
An unresolved investigation blocked all later phases; other blockers stopped
their dependents. Dropping requested features was not an allowed workaround.

## Story completion rule C

Each story incorporated these closeout requirements:

1. PROJECT.md had to record its result and narrow supporting evidence. It alone
   held story, gate, milestone, blocker, handoff and acceptance state.
2. Affected contracts, user docs and the index were to change only where their
   meaning changed. Owner findings could not be rewritten for an easier result,
   and older tracked requests could not be enrolled or closed as a side effect.
3. Implementation stories required affected builds and relevant checks;
   investigation/documentation stories required applicable static checks.
   Skipped checks had to be identified; compilation did not prove runtime or
   visual behavior.
4. Status, scope and the final diff had to be inspected, with only intended
   paths staged. Management updates belonged in the closing commit and the
   authorized final push to main. Fetching and ordinary non-destructive Git
   operations, including fast-forward-only updates where applicable, were
   required. Force pushes and history rewriting were prohibited. A failed push
   would have left closeout incomplete.
5. The execution report had to identify the commit and actual push/CI outcome.
   A follow-up commit solely to record its own hash was excluded. Existing CI
   was sufficient; a new publishing pipeline was outside scope. Pre-handoff
   pushes recorded technical work, not owner acceptance or a published release.
   Store publication and tagging were outside this plan.

The plan kept story definitions free of progress entries. Epic completion was
to derive from child stories; gates and milestones required separate evidence
in PROJECT.md and were not additional stories.

## Phases, gates and milestones

| Phase | Assigned work | Required exit evidence and milestone | Validator |
| --- | --- | --- | --- |
| 1. Investigate | E1: I01-I03 only | G1 required native facts, data routes, safe checks and handoff-case preparation, with no unresolved investigation. M1: Implementation evidence ready. | Implementer, human-free |
| 2. Implement | E2-E5: F01-F04, M01-M02, S01-S03, T01-T04 | G2 required every finding, passing story/integrated checks, contract/cache handling and management commits on main. M2: Panels technically complete. | Implementer, human-free |
| 3. Prepare handoff | E6: D01 and D02 preparation | G3 required an identified package and concise owner checklist tied to the tested source/artifact. M3: Ready for owner preview. | Implementer, human-free |
| 4. Final handoff | D02 owner check and closure | G4 required the owner's minimal candidate preview result, resolution of reported defects and final management push. M4: Owner accepted. | Owner, sole human gate |

Each later phase depended on its preceding gate. Independent Phase 2 stories
needed no additional gates. A G4 failure would have returned affected work to
implementation and G2/G3 checks; only changed/failed behavior was to be rechecked
at the same final handoff. A new investigation would first have reopened G1.
A second full owner test programme was excluded.

## Epic E1: Verified evidence for the requested panels

The intended outcome was source-backed implementation without game execution
or guessed semantics. Its three investigation stories preceded all other work.
Short findings belonged in PROJECT.md; larger temporary evidence was untracked.

### PANEL-I01: Native resource units

- **Value:** Oil and Deuterium figures were intended to match the game's units.
- **Scope:** Inspection of native resource writers/fields, oil conversion,
  units, precision and settings treatment; tracing Deuterium through extraction
  to `/s` presentation.
- **Out of scope:** Native execution, live measurements, formula/threshold
  changes, recalibration and implementation.
- **Definition of done required:** Exact version/member references,
  transformations and pure numeric cases; retention of `>= 0.15/s` for
  Deuterium. The starter anecdote was not a separate threshold. Missing native
  evidence had to be treated as a blocker rather than guessed.
- **Approach:** Reuse of native calculations and formatting, without a new
  unit-conversion framework.
- **Closeout requirement:** C, including management updates and push to main.

### PANEL-I02: Planet and star attribution

- **Value:** Moon, containment, sphere and Aquatica facts were intended to refer
  to the correct bodies.
- **Scope:** Verification of parent/moon ordering, home identity, star-centric
  containment including moons, sphere units, Dyson luminosity, O/blue classes
  and Aquatica identity; mapping to system identities and pairwise distances.
- **Out of scope:** Game execution/generation, theme-value research, sphere
  recalibration and new duplication/eligibility policy.
- **Definition of done required:** Source-backed definitions and pure cases for
  F03, S01-S03 and T02; separation of stellar and sphere radii; no unresolved
  geometry or catalogue guess passed into implementation.
- **Approach:** Existing native topology/radius facts, without an orbit
  simulator, generalized astronomy model or localized-name heuristic.
- **Closeout requirement:** C, including management updates and push to main.

### PANEL-I03: Minimal data routes and verification inputs

- **Value:** Delivery was intended to avoid unnecessary collection, cache work
  or an owner investigation round.
- **Scope:** Mapping findings to Core/Runtime/Plugin routes, column conventions,
  selection bounds and cache fields; identifying necessary per-system totals
  or attribution; inspecting tests/CI and attributable existing seed evidence
  for the handoff. Real observations and synthetic fixtures had to stay distinct.
- **Dependencies:** I01 and I02.
- **Out of scope:** Reprioritization, new thresholds/scoring, broad seed search,
  native execution, saves and owner-supplied test labour.
- **Definition of done required:** An evidence route and human-free checks for
  every implementation story; scoped semantic/cache handling; no extra retained
  data for the four-row color intersection; a handoff recipe without owner seed
  hunting. Genuine unresolved gaps had to use the blocker procedure before G1,
  without reopening settled owner rules.
- **Approach:** Existing bounded aggregates, identities, tests and CI; no data
  collection solely for possible later use.
- **Closeout requirement:** C, including management updates and push to main.

## Epic E2: Useful fresh-start guidance

The intended outcome was useful starter guidance and consistent usable
Deuterium supply. G1 was its prerequisite.

### PANEL-F01: Single starter-giant conclusion

- **Value:** One clear starter-giant verdict was intended.
- **Scope:** Fresh start item 1: Fire Ice Strength, Deuterium Limitation, and
  removal of the duplicate opposite-column presence/absence presentation.
- **Out of scope:** Giant generation, gas rates and other contexts.
- **Definition of done required:** Product cases showing the correct polarity
  exactly once, with unrelated starter facts unchanged.
- **Approach:** Local predicate/projection changes, without scoring or a special
  game-generation path.
- **Closeout requirement:** C, including management updates and push to main.

### PANEL-F02: Home-system Fire Ice adequacy

- **Value:** Useful mineable Fire Ice was to be distinguished from poor supply.
- **Scope:** Fresh start items 3-4: system totals and both amount/group minima,
  retaining other scarcity conclusions and their polarity.
- **Out of scope:** Single-planet minima, other scarcity thresholds, gas-product
  coloring and resource generation.
- **Definition of done required:** Split deposits, both sides/equality of each
  minimum, missing-data handling without treating it as zero, unchanged scarcity
  regression outcomes and versioning of altered cached conclusion semantics.
- **Approach:** Existing home-system aggregate and outcome rules.
- **Closeout requirement:** C, including management updates and push to main.

### PANEL-F03: Sibling-moon power conclusions

- **Value:** Solar/Wind guidance was intended for useful neighboring moons.
- **Scope:** Fresh start item 2: omission of home-planet power advice, verified
  home-giant siblings, and independent Solar/Wind classification for each of
  one or two eligible siblings under the specified range.
- **Out of scope:** A combined moon verdict, other-system moon advice and
  changes to the statistics table's existing Solar/Wind values.
- **Definition of done required:** No-sibling/non-moon cases without sibling
  advice; one Solar and one Wind entry per eligible moon, each in one correct
  column, including equal boundaries and mixed outcomes.
- **Approach:** Verified parent/body inventory and existing range evaluation;
  display names were not a basis for inferring siblings.
- **Closeout requirement:** C, including management updates and push to main.

### PANEL-F04: Nearest qualifying Deuterium supply

- **Value:** Both panels were to identify the nearest usable supply without
  preferring a richer but farther giant.
- **Scope:** Fresh start item 5 and Cluster Deuterium: shared rate eligibility,
  conclusion distance classification, statistics title and retained search cap.
- **Out of scope:** New rate bands, a hard-coded starter exclusion or new gas
  color rule; the statistics cap was not a conclusion cap.
- **Definition of done required:** Below/equal/above rate cases, nearest versus
  richer candidates, no match, distance boundaries and inclusive statistics cap;
  attributed values with each panel's own distance treatment.
- **Approach:** Shared eligibility using gas candidates/system distances,
  retaining the panels' distinct rules.
- **Closeout requirement:** C, including management updates and push to main.

## Epic E3: Relevant megafactory resource destinations

The intended outcome was relevant resource systems instead of broad rare
summaries. G1 was its prerequisite.

### PANEL-M01: Targeted rare-resource presentation

- **Value:** Destinations for the three useful rare resources were intended.
- **Scope:** Megafactory items 2-3: Spiniform/Grating quantity qualifications,
  Organic presence, qualifying non-Aquatica deposits, nearest systems, removal
  of Many rares and preservation of existing rare-resource preference.
- **Out of scope:** New combination-versus-distance ranking, an Organic amount
  floor, statistics selection changes and theme generalization.
- **Definition of done required:** Correct resources/hosts, exact threshold and
  non-Aquatica cases, removal of unrelated rare summaries from Megafactory,
  existing column conventions, unchanged statistics rare rows and relevant
  data/contract/cache checks.
- **Approach:** Only necessary extensions to existing aggregation/projection
  identified by I03; no new ranking subsystem.
- **Closeout requirement:** C, including management updates and push to main.

### PANEL-M02: Plentiful resource-system conclusions

- **Value:** Nearby systems with substantial mixed supply were to be visible.
- **Scope:** Megafactory item 4: per-resource/system totals, strict amount
  threshold, at least three qualifying types and nearest-home selection.
- **Out of scope:** A universal score, throughput estimates, unrelated statistics
  columns and invented amount/distance bands.
- **Definition of done required:** Rejection of two types and exact amount
  equality, acceptance of three qualifying types, split-deposit aggregation
  with correct resource identities, applicable nearest-system conventions,
  separate amount/rate semantics and cache/fresh-result agreement.
- **Approach:** Bounded totals in the existing scan, retaining only necessary
  conclusion data without another scan or data dump.
- **Closeout requirement:** C, including management updates and push to main.

## Epic E4: Sphere and nearby Aquatica comparison

The intended outcome was correct geometry, facts and ordering for luminous
sphere candidates and Aquatica hosts. G1 was its prerequisite.

### PANEL-S01: Sphere-only geometry conclusions

- **Value:** Sphere conclusions were to identify nearby luminous candidates.
- **Scope:** Sphere / energy items 1-2 and Megafactory item 1 removal: verified
  star-centric containment, maximum radius, luminosity filter and distance order.
- **Out of scope:** Existing shell/count bands, other luminosity conclusions,
  receiver modelling and general sphere design advice.
- **Definition of done required:** Luminosity equality/below cases, nearer
  versus larger/farther choices, honest no-match behavior, no Megafactory
  containment duplicate, I02 moon geometry and semantic/cache checks together.
- **Approach:** Existing native topology/radius projection, shared with S02/T02.
- **Closeout requirement:** C, including management updates and push to main.

### PANEL-S02: Notable-star sphere rows

- **Value:** Blue giants, the brightest star and O stars were to be comparable.
- **Scope:** Notable stars items 1-2: blue giants first/brightest first/green,
  cluster brightest next, remaining O stars by home distance; Distance,
  Max Sphere and Contained columns.
- **Dependencies:** S01's shared geometry facts.
- **Out of scope:** Aquatica rows, replacing stellar Size, importing the
  conclusions luminosity cutoff or adding a global exclusion rule.
- **Definition of done required:** Group order, home distances, meter radii,
  counts, full blue-row color, no-blue cases and unchanged existing star facts.
- **Approach:** Existing table/renderer, local styles and distance matrix.
- **Closeout requirement:** C, including management updates and push to main.

### PANEL-S03: Final Aquatica group

- **Value:** Up to five relevant non-O hosts nearest the brightest star were
  intended to help players compare Aquatica locations.
- **Scope:** Notable stars item 3: theme eligibility, five-star cap, brightest-star
  selection/sort origin, final group and the two specified blank cells.
- **Dependencies:** S02's table and sphere-group order.
- **Out of scope:** Deposit-based theme eligibility, O hosts, a general themes
  panel, exclusion by another displayed role and extra labels.
- **Definition of done required:** Zero/fewer/exactly/more-than-five cases,
  one host per star despite multiple matching planets, different home/brightest
  distance ordering, last-group placement and blank Luminosity/Contained cells.
- **Approach:** Verified catalogue identity and pairwise distances, adding only
  the necessary attributed theme fact.
- **Closeout requirement:** C, including management updates and push to main.

## Epic E5: Clear home and cluster resource statistics

The intended outcome was accurate oil, moon, containment and resource emphasis
within the existing tables. G1 was its prerequisite.

### PANEL-T01: Native oil totals

- **Value:** The home table's oil rate was intended to match the game.
- **Scope:** Home system item 1: I01's native conversion/precision and retained
  wells count.
- **Out of scope:** Finite-ore changes, production estimates and scarcity rules
  consuming native amounts.
- **Definition of done required:** Multiple-well totals and rounding matching
  the native writer, rate presentation, unchanged other resource formatting
  and correct cached raw amount semantics.
- **Approach:** The existing oil-cell projection, without changing the shared
  finite-ore formatter for a different quantity.
- **Closeout requirement:** C, including management updates and push to main.

### PANEL-T02: Moon labels and contained-home highlight

- **Value:** Moon positions and the home orbit opportunity were to be explicit.
- **Scope:** Home system items 2/4: moon-only second lines in parent-relative
  nearest-first order, and a wholly green contained-home row.
- **Dependencies:** S01's verified containment.
- **Out of scope:** Extra lines on giants/non-moons, other contained-planet row
  highlights and changes to the body inventory.
- **Definition of done required:** Multiple parents/siblings, home moon/direct
  home planet and contained/uncontained cases; correct row-only effects while
  preserving body names and table values.
- **Approach:** Native parent/order/home identity and existing row wrapping,
  without a second topology model.
- **Closeout requirement:** C, including management updates and push to main.

### PANEL-T03: Rare-resource text emphasis

- **Value:** Useful deposits and nearby candidates were intended to stand out.
- **Scope:** Home system item 3 and all Cluster rare-resource rules, including
  the four displayed resource rows' system intersection and matching cells.
- **Out of scope:** Selection/preference changes, a distance cap, hidden-resource
  search, removal of other home resources, giant Fire Ice gas coloring and
  new retained scan/cache data.
- **Definition of done required:** Only the two specified home vein highlights,
  strict distance boundaries, ordinary cells, both displayed candidate columns
  in the intersection, no qualification through undisplayed resources, and
  correct matching-cell color without heading/unrelated-cell leakage.
- **Approach:** Existing cached host IDs and local text/cell styling, retaining
  two-candidate selection and payload.
- **Closeout requirement:** C, including management updates and push to main.

### PANEL-T04: Unipolar amount and distance colors

- **Value:** Supply and travel distance were to remain independently readable.
- **Scope:** Both Cluster Unipolar rules, independently in their named cells
  on every existing row.
- **Dependencies:** T03's local styling path.
- **Out of scope:** Row selection, ranks, whole-row coloring and number/unit changes.
- **Definition of done required:** Below/equal/above thresholds and mixed colors
  in the correct cells; unchanged other cells, quantities and order; comparisons
  using underlying values.
- **Approach:** T03's styling and direct comparisons, without configurable styling.
- **Closeout requirement:** C, including management updates and push to main.

## Epic E6: Reviewable candidate and final acceptance

The intended outcome was an identifiable candidate and one short owner preview.
G2 was its prerequisite; no new behavior or deferred feature tests were included.

### PANEL-D01: Verified candidate package

- **Value:** The owner was to receive the exact technically checked candidate.
- **Scope:** Existing build/test/version/package workflow, scanner-only ZIP,
  hashes, source/build identity and concise results.
- **Out of scope:** Installation, DSP execution, machine changes, store upload,
  new CI infrastructure and claims of in-game validation.
- **Definition of done required:** Passing technical checks for the identified
  source/artifact, the existing three-DLL package contract, no external binaries
  or saves, accurate docs and a package/reports requiring no owner build work.
- **Approach:** Existing scripts/artifacts; rebuilding only for changed final
  source/version or unresolved results.
- **Closeout requirement:** C and frozen artifact/source identity for D02.
  Later documentation commits were not to change the tested binary identity.

### PANEL-D02: Minimal owner preview and closure

- **Value:** Acceptance was intended to require no seed hunting or technical work.
- **Scope:** I03/D01 handoff preparation, readiness recording at G3, receipt of
  the brief G4 owner result, resolution of actual defects through their stories
  and acceptance recording in PROJECT.md.
- **Dependencies:** D01; human validation only after G3.
- **Out of scope:** Owner compilation, measurements, boundary matrices, logs
  without a failure, seed searches, other-mod changes or save use. Implementer
  installation/execution remained prohibited.
- **Definition of done required:** A complete candidate and source-backed recipe
  at the technical checkpoint, followed by the exact candidate's owner result,
  defect resolution and factual management closure pushed to main. Package
  readiness alone did not establish owner acceptance.
- **Approach:** Existing package/case evidence and only those visual/runtime
  checks that automated tests could not establish.
- **Closeout requirement:** C at final completion, preserving original binary
  source/hash separately from acceptance-documentation commits.

## Technical gate checks

Story checks were specified as pure repository-owned fixtures for boundaries,
selection, attribution and presentation, without executing installed DSP code.
The existing G2/D01 commands were:

```powershell
dotnet build DSPSeedScanner.sln --configuration Release
dotnet run --project tests/DSPSeedScanner.Core.Tests/DSPSeedScanner.Core.Tests.csproj --configuration Release --no-build
dotnet run --project tests/DSPSeedScanner.Runtime.Tests/DSPSeedScanner.Runtime.Tests.csproj --configuration Release --no-build
dotnet build src/DSPSeedScanner.Plugin/DSPSeedScanner.Plugin.csproj --configuration Release
```

The plugin build was permitted to read installed references and write repo
outputs, but not deploy/run DSP. CI declarations did not prove native runtime
compatibility. The plan required `Test-BuildArtifact.ps1`,
`New-ThunderstorePackage.ps1` and `Test-ThunderstorePackage.ps1` against the exact
workflow version/artifacts, without a new format. Documentation links, diff
scope, intended versions and exclusion of generated/dependency data from
commits were included. Passing checks on unchanged source were to be reused.

## Final handoff package and minimum owner check

The handoff note was required to link PROJECT.md for state and include:

- A direct package link, version, source and SHA-256, with binaries/reports kept
  untracked or in existing CI artifacts; documentation-only builds could not
  silently replace it, and a replacement required its own evidence.
- Short owner-only steps for the existing three-DLL layout, without changing
  other mods or configuration.
- One primary seed/settings recipe from attributable existing evidence; a
  second was permitted only for an essential visible case absent from the first.
  Synthetic fixtures could not be presented as real-seed observations.
- A few expected rows, text, colors, ordering and section locations, without
  requiring the owner to reconstruct findings or recalculate rules.
- Concise technical results and limitations, including no implementer game run
  or deployment.

The prescribed owner sequence was installation and New Game preview with the
supplied settings; one scroll through both panels after the scan; comparison
of marked conclusions, oil/moons/home highlight, rare/Unipolar colors and
notable stars; readability/overlap checks; then a return to the menu and a
Pass/problem reply. Screenshots were useful only for a problem. Gameplay and
existing saves were excluded; a second preview was conditional, not mandatory.

Numeric edges and rare combinations belonged to technical checks. The owner
was not to compensate for missing implementation evidence. An unprepared G3
case required a blocker, not an owner investigation. Final acceptance was
limited to runtime/readability, not broad compatibility or performance claims.
The [historical preview recipe](PANEL-PREVIEW-CHECK.md) preserves the supplied
input and expected cues.

## Coverage and ownership

| Findings section | Assigned stories |
| --- | --- |
| Conclusions: Fresh start, items 1-5 | F01; F02 (3-4); F03 (2); F04 (5) |
| Conclusions: Megafactory, item 1 | S01, single owner of containment relocation |
| Conclusions: Megafactory, items 2-4 | M01 (2-3); M02 (4) |
| Conclusions: Sphere / energy | S01 |
| Statistics: Home system, items 1-4 | T01; T02 (2, 4); T03 (3) |
| Statistics: Cluster Deuterium | F04, shared eligibility and distinct distance rules |
| Statistics: Cluster rare resources | T03 |
| Statistics: Cluster Unipolar Magnets | T04 |
| Statistics: Notable stars | S02 sphere facts/order; S03 Aquatica group |
| Native/data investigations | I01-I03, exclusively Phase 1 |
| Package and human acceptance | D01-D02, G3/G4 |

References from multiple sections did not duplicate a story. Management updates
and checks were included in closeout; no separate cleanup, population study or
documentation workstream was implied.

Return to the [findings](PANEL-VALIDATION-FINDINGS.md) or
[documentation index](../INDEX.md).
