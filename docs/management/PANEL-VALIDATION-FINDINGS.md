# Panel Validation Findings

[PROJECT.md](../PROJECT.md) is the sole authority for project steering and work
status, including historical dispositions. This document contains scope,
requirements, or technical evidence; it does not track status.

**Scope:** Owner observations and clarifications from the 2026-09-19 session
about the conclusions and statistics panels, forming the requirements for the
[Panel Improvement Roadmap](ROADMAP.md). This remains a standalone
record of the new work.

**Source and date:** Owner feedback on 2026-09-19 after extensive in-game use
of the mod, incorporating personal experience and community wisdom as
reported by the owner. Follow-up statistics-panel findings are included below
for that roadmap. The owner clarified that the existing resource-
scarcity conclusions and their polarity should be retained and that the rich
Deuterium cutoff is `>= 0.15/s` for both panels.

**Evidence boundary:** These are owner-reported gameplay-validation findings
and product-value judgments. They are not newly reproduced scanner probes,
a measured seed-population distribution, or independently sourced community
claims. The conversation supplied no seed/settings sample or captured rate
measurements. Preserve the owner's observations as the planning basis while
keeping implementation details and unresolved choices explicit.
Implementation notes below come from repository source inspection; no new
in-game reproduction or native oil-rate conversion validation was performed
for this record.

## Conclusions: Fresh start

1. **Classify the starter giant once.** The owner finds starter Deuterium
   giants consistently unfavorable: they lack steady Fire Ice exploitation
   and their Deuterium output is low (reported as approximately `0.03/m`).
   Present a Fire Ice starter giant as a Strength and a Deuterium starter giant
   as a Limitation. Avoid the current duplication of the same giant across
   those columns through separate product-presence and product-absence lines.

2. **Evaluate power on neighboring moons, not the home planet.** The owner
   finds home-planet Solar/Wind information unhelpful because those rates are
   consistently similar and do not affect their gameplay decisions. If the
   home planet is a moon with one or two sibling moons around the same starter
   giant, evaluate each of those siblings. Use `L = 40%` and `U = 115%` for
   both Solar and Wind. Under the existing increasing-metric rule, each
   metric is a Limitation below `40%`, Preference-sensitive from `40%` up to
   but excluding `115%`, and a Strength at or above `115%`. Present a separate
   Solar and Wind conclusion for each eligible moon, each placed once in its
   appropriate column. There is no combined moon verdict or mixed-metric
   placement decision.

3. **Keep resource-scarcity conclusions.** The owner explicitly clarified
   that "Resource scarcity is good" means retain the existing conclusions
   and their current polarity. It does not mean scarcity becomes a Strength.

4. **Qualify mineable Fire Ice by supply.** Presence is useful, but scarce
   deposits or too few vein groups belong in Limitations. A good deposit must
   have at least `900,000` units across at least `4` vein groups; both minima
   must be met across the home system. The amounts and groups may be spread
   across multiple planets; no single-planet minimum is required.

5. **Locate usable Deuterium.** Present the nearest Deuterium giant with a
   rate of at least `0.15/s`. Assign its column by distance from the home
   system using the existing distance band: at most `2.5 ly` is a Strength,
   more than `2.5 ly` through `10 ly` is Preference-sensitive, and more than
   `10 ly` is a Limitation. This requests nearest qualifying supply rather
   than the statistics panel's current highest-rate selection within
   `8.125 ly`.

## Conclusions: Megafactory

1. **Remove contained-orbit conclusions from this context.** They belong in
   Sphere / energy and should not be emitted in both contexts. The owner's
   luminous-star filtering and distance ordering are recorded under that
   context below.

2. **Replace broad rare-access presentation with useful resource systems.**
   Focus on Spiniform Stalagmite Crystals, Optical Grating Crystals, and
   Organic Crystals only. The owner identifies Aquatica as a source of large
   Spiniform deposits, but any theme with sufficiently large deposits may
   qualify. A large Spiniform deposit means more than `900,000` units. Optical
   Grating Crystals require more than `600,000` units; no additional Organic
   Crystal quantity threshold was requested. Present the nearest matching
   systems. The owner's stated preference for systems combining all three
   does not authorize a new selection priority: preserve existing rare-resource
   preference, and do not introduce a combination-versus-distance ranking.

3. **Remove the "Many rares" presentation.** The owner considers the broad
   rare-resource summaries obsolete given the statistics panel. Retain only
   the targeted conclusions requested above rather than continuing the old
   all-rare-resource presentation.

4. **Present plentiful resource systems.** Identify potential megafactory
   systems with more than `40,000,000` units of each of more than two resource
   types: at least three types must independently exceed that amount. Select
   candidates for presentation by nearness to the home system, aggregating
   amounts per resource type across each system.

## Conclusions: Sphere / energy

1. **Filter and order contained-orbit candidates.** The owner considers
   containment relevant only for high-yield sphere stars with luminosity at
   least `2.0`. Exclude stars below that threshold and present matching stars,
   if any, by nearness to the home system. Emit this conclusion here only,
   removing its Megafactory duplicate.

2. **Apply the same candidate selection to shell size.** Exclude luminosity
   below `2.0` and select matching shell-size candidates by nearness to the
   home system. No replacement shell-radius or contained-orbit-count ranges
   were supplied in this feedback.

## Statistics: Home system

1. **Show native oil yield.** The owner reports that Oil displays resource
   counts. Replace that quantity with the planet's total oil rate as displayed
   in-game, using the native unit and precision. The request changes the rate
   value; it does not request removal of the accompanying wells count.

2. **Identify and order moons explicitly.** Add a second line in the Body
   cell for moons only, indicating their ordinal position from closest to
   farthest around their parent giant. Non-moons and the giant itself receive
   no additional line. Exact wording remains a presentation choice.

3. **Distinguish useful rare veins.** Render the Fire Ice veins and Spiniform
   Stalagmite Crystal entries in green when present. The owner considers
   other rare resources unimportant for the home-system view; no other rare
   highlights are requested. This refers to mineable Fire Ice, not the gas
   product. Removal of other resource entries was not explicitly requested.

4. **Highlight a contained home planet.** If the home planet's orbit is
   contained within the home star's maximum Dyson sphere radius, make the
   entire home-planet row's text green. This rule does not recolor other
   planets or an uncontained home planet. Independent resource highlights
   above still apply.

## Statistics: Cluster Deuterium

- Rename the section to **Nearest Rich Deuterium Gas-Giant**.
- Admit only giants with Deuterium rate `>= 0.15/s`, using the owner's final
  clarification for both panels. The owner expects the starter giant never
  to meet that cutoff; this is an owner claim, not a reason to hard-code its
  exclusion independently of the measured rate.
- Select the nearest qualifying giant while retaining the statistics panel's
  inclusive `8.125 ly` maximum distance from home. This replaces its current
  highest-rate-first selection within that bound. No new Deuterium font-color
  rule was supplied.

## Statistics: Cluster rare resources

Apply colors to the individual presented candidate cells in Closest and
Alternative, rather than to the resource heading or an unrelated candidate.
Distances are from the home system.

| Candidate | Green text | Red text | Default text under this distance rule |
| --- | --- | --- | --- |
| Sulfuric Acid ocean | Distance `< 3.5 ly` | Distance `> 8.5 ly` | `3.5 ly` through `8.5 ly`, inclusive |
| Other listed rare resources | Distance `< 3.5 ly` | No far-distance red rule requested | Distance `>= 3.5 ly` |

Additionally, a system qualifies for the four-resource green highlight only
when it is represented by an already-presented candidate in each of the
Spiniform, Fire Ice veins, Organic Crystal, and Sulfuric Acid ocean rows.
Either Closest or Alternative can supply that representation, and different
planets from the same system may supply different rows. Color all matching
candidate cells in those four rows green. A resource present elsewhere in the
system but absent from its displayed candidates does not satisfy this test.

The owner expects this displayed-candidate intersection to exclude far-away
sulfuric-acid candidates because closer candidates and alternatives will fill
the other rare-resource rows. Record that expectation as an owner claim; do
not add a distance cutoff or expand the candidate search to enforce it. Keep
existing rare-resource selection and preference unchanged. This section
requests presentation changes only.

## Statistics: Cluster Unipolar Magnets

Apply these rules independently to each presented row's named cells. Other
cells retain their normal text color.

| Cell | Green text | Red text | Default text |
| --- | --- | --- | --- |
| Magnets | Amount `> 2,000,000` | Amount `< 900,000` | `900,000` through `2,000,000`, inclusive |
| Distance | Distance `< 15 ly` | Distance `> 21 ly` | `15 ly` through `21 ly`, inclusive |

Distance is from the home system. Compare underlying values, not rounded
display strings. No change to which Unipolar planets appear was requested.

## Statistics: Notable stars

1. **Use the owner's explicit group order.** Present blue giants first, sorted
   brightest first, with fully green text rows. Present the cluster's brightest
   star next, followed by the remaining O stars sorted by distance to home.
   Up to five Aquatica rows come last, sorted by distance to the brightest
   star.

2. **Add sphere facts.** Add **Distance**, **Max Sphere**, and **Contained**
   columns. For sphere candidates, Distance is from the home system; Max
   Sphere is maximum sphere radius in meters; Contained is the number of
   planets with contained orbits in that system. These are additions to the
   existing table, not a replacement of stellar Size. No statistics-panel
   luminosity cutoff was supplied; do not silently import the conclusions
   panel's `>= 2.0` gate.

3. **Add Aquatica-hosting non-O stars.** Include non-O stars with at least one
   planet matching the Aquatica theme. When there are more than five matching
   stars, select the five nearest to the cluster's brightest star; otherwise
   include all matches. Count stars, not matching planets. For these rows,
   leave Luminosity and Contained empty and measure Distance from the
   brightest star, not from home. Only those two columns were requested blank.
   Both selection and ordering within this final group use distance to the
   brightest star.

## Implementation notes for the shared roadmap

These source observations and verification suggestions support the requested
behavior. They do not prescribe a new architecture, additional product
features, or separate prerequisite work packages.

- **Oil conversion needs the native writer.**
  `HomeSystemBodyPresentation.ProjectTableRow` currently passes oil Amount
  through the ore `FormatAmount` helper; `CompleteClusterRawCoordinator`
  sums native vein-group amounts. Before implementing a conversion, inspect
  the installed game's planet resource UI writer for its total-rate formula,
  units, rounding, and settings treatment. Do not guess a divisor from the
  label or change the finite-resource amount semantics alongside this fix.
- **Moon order and containment need attributed geometry.** The current
  `HomeSystemBodyInventory` retains satellite status and parent identity, but
  not an orbital radius or explicit nearest-first moon ordinal. Derive the
  ordinal from verified native orbital ordering within each parent, including
  the home moon. Retain the birth-planet identity for its unique row highlight.
  `DspPreviewGateway.MaximumShellRadius` already supplies the maximum radius
  separately from stellar radius. Its current contained-orbit counter compares
  every body's `orbitRadius * 40_000` directly with that maximum. For moons,
  verify that the comparison uses a star-centric orbit before reusing it for
  the row highlight or Contained. Verify the native geometry and use consistent
  containment semantics for both panels as an implementation responsibility.
- **Color scope exceeds the current plain string cells.**
  `PreviewStatisticsPanelRenderer` currently uses a common body style and
  string cells. The requested colors apply to individual resource text, rare
  and Unipolar cells, and entire home-planet and blue-giant rows. The rendering
  approach is an implementation choice; check that colors do not leak into
  unrelated text and that the requested sublines and columns fit the panel.
- **Four-resource coloring uses the existing retained candidates.** Intersect
  `HostSystemIdentifier` values across the displayed candidates in the four
  relevant rows, then color matching cells. `ClusterResourceStatistics`
  already retains those identifiers, and the cache serializes them through
  `WriteLocation`. No additional scan retention, system-resource aggregation,
  or cache payload extension is required for this highlight. Preserve the
  current two-candidate selection per category and cache/fresh-scan agreement.
- **Notable stars need identity and additional preview facts.**
  `RuntimeNotableStarEvidence` currently carries name, type/class, stellar
  radius, Dyson luminosity, and stable order. The new presentation needs the
  requested sphere facts and generated Aquatica theme membership associated
  with the correct systems.
  Preview normalization already produces all pairwise system distances, so
  reuse those for both home and brightest-star origins. Identify Aquatica
  through the runtime catalogue rather than a localized display-name guess;
  its theme match is the requested statistics criterion, distinct from the
  conclusions panel's generated-deposit criterion. The Aquatica group's blank
  cells and brightest-star distance follow the owner rules above. There is no
  additional eligibility exclusion based on appearance in another star group.
- **Verification suggestions.** Relevant focused checks include
  exact color boundaries and native rate parity; nearest qualifying Deuterium;
  per-parent moon ordering and home-only containment; four-row intersections
  using only presented candidates, with non-presented resources unable to
  qualify a system; the explicit notable-star group ordering;
  five-candidate selection and mixed distance origins; and cache/fresh-scan
  agreement. Owner-only inspection at the roadmap's final handoff can verify
  the rendering; the implementer must not run or deploy the game. These are
  suggestions for validating the requested changes, not a new research gate
  or checks performed for this documentation record.

## Scope boundaries for roadmap execution

- The qualifying Deuterium cutoff is `>= 0.15/s` for both panels; the statistics
  search retains its `8.125 ly` maximum. The separately reported starter
  observation (`0.03/m`) remains a source claim, not an implementation cutoff.
- The recorded thresholds, aggregation scopes, displayed-candidate highlight,
  and notable-star ordering are settled inputs. Do not reopen a rare-resource
  preference redesign or combined moon verdict as a planning requirement.
- Preserve existing behavior outside the changes explicitly requested above.
  Native oil formatting, geometry, evidence projection, and cache implications
  of actual semantic changes are technical investigations for implementation,
  not additional owner decisions. This four-resource presentation rule alone
  does not require a new cache schema.

These findings for both panels are the source for the active roadmap.
This record does not infer additional changes to Compact
expansion or other conclusion families from the proposals above.

Source pointers for the implementation notes:
[preview extraction](../../src/DSPSeedScanner.Plugin/DspPreviewGateway.cs),
[statistics models and projection](../../src/DSPSeedScanner.Runtime/PreviewStatisticsPresentation.cs),
[table rendering](../../src/DSPSeedScanner.Plugin/PreviewStatisticsPanelRenderer.cs),
[complete-scan aggregation](../../src/DSPSeedScanner.Runtime/CompleteClusterRawCoordinator.cs),
and [cache payload](../../src/DSPSeedScanner.Runtime/CompleteClusterConclusionCache.cs).

Return to the [documentation index](../INDEX.md).
