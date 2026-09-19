# Panel Validation Findings — Historical Requirements

[PROJECT.md](../PROJECT.md) is the sole authority for steering and all work
state, including historical dispositions. This document preserves the owner's
2026-09-19 observations and clarifications that informed the
[Panel Improvement Roadmap](PANEL-IMPROVEMENT-ROADMAP.md).

The owner reported extensive in-game use, personal experience and community
wisdom. These were owner claims and product-value judgments, not new scanner
probes, a measured seed-population distribution or independently sourced
community evidence. The conversation supplied no seed/settings sample or
captured rate measurements for these findings. Source notes described the
pre-change code; no new game reproduction or native oil-conversion validation
was performed for this findings record.

The owner clarified that resource-scarcity conclusions and their polarity were
to be retained, and corrected the rich Deuterium threshold to `>= 0.15/s` for
both panels. Earlier answers using `/min` were superseded by that clarification.

## Conclusions: Fresh start

1. **Single starter-giant classification.** The owner found starter Deuterium
   giants unfavorable because they lacked steady Fire Ice exploitation and
   reportedly produced little Deuterium (approximately `0.03/m`). The request
   was Fire Ice as a Strength and Deuterium as a Limitation, without duplicating
   the same giant across columns through presence/absence statements.

2. **Neighboring-moon power.** The owner found home-planet Solar/Wind advice
   unhelpful because the rates were similar across starts and did not affect
   gameplay decisions. When the home planet was a moon with one or two siblings
   around its starter giant, each sibling was to receive independent Solar and
   Wind advice. Both used `L = 40%`, `U = 115%`: below `40%` was a Limitation,
   `40%` to below `115%` Preference-sensitive, and `>= 115%` a Strength.
   Each metric/eligible moon was to appear once in its appropriate column;
   no combined moon verdict or mixed-metric placement was requested.

3. **Existing scarcity polarity.** The owner clarified that “Resource scarcity
   is good” meant keeping the existing conclusions and polarity, not making
   scarce resources a Strength.

4. **Mineable Fire Ice supply.** Useful presence was to be distinguished from
   scarce deposits or too few groups. Good supply required at least `900,000`
   units and at least `4` vein groups across the home system. Both minima were
   required; multiple planets could contribute, with no single-planet minimum.
   Inadequate supply belonged in Limitations.

5. **Usable Deuterium.** The nearest giant with rate `>= 0.15/s` was requested.
   Existing distance classification was retained: `<= 2.5 ly` Strength,
   `> 2.5 ly` through `10 ly` Preference-sensitive, and `> 10 ly` Limitation.
   This differed from the pre-change statistics selection, which favored the
   highest rate within `8.125 ly`.

## Conclusions: Megafactory

1. **Containment relocation.** Contained-orbit advice was requested only in
   Sphere / energy, without the Megafactory duplicate. The luminous-star and
   distance rules were the same as those recorded below.

2. **Targeted rare-resource destinations.** Attention was restricted to
   Spiniform Stalagmite Crystals, Optical Grating Crystals and Organic Crystals.
   The owner identified Aquatica as a source of large Spiniform deposits, but
   sufficiently large deposits on any theme were eligible. Large Spiniform
   meant `> 900,000` units; Grating required `> 600,000`; no Organic quantity
   floor was requested. Nearest matching systems were to be shown. The owner's
   preference for systems combining all three did not authorize new selection
   priority: existing preference was to remain, without a new
   combination-versus-distance ranking.

3. **Removal of Many rares.** The owner considered broad rare summaries
   redundant with statistics and requested their removal from this context,
   retaining the targeted conclusions above.

4. **Plentiful systems.** Systems with more than `40,000,000` units of each of
   more than two resource types were requested: at least three types each had
   to exceed the threshold independently. Totals were per resource across the
   system; presentation selection was by nearness to home.

## Conclusions: Sphere / energy

1. **Contained-orbit selection.** The owner considered containment useful for
   high-yield spheres only at luminosity `>= 2.0`. Lower-luminosity stars were
   to be excluded and matching candidates selected by home-system distance.
   This context alone was to emit containment advice.
2. **Shell-size selection.** The same luminosity filter and nearest-home order
   were requested for shell candidates. No replacement shell-radius or
   contained-orbit-count bands were supplied.

## Statistics: Home system

1. **Oil yield.** The owner reported oil counts in the table and requested the
   planet's in-game total oil rate with native units/precision. Removal of the
   accompanying wells count was not requested.
2. **Moon identity/order.** A second Body-cell line was requested for moons,
   giving nearest-to-farthest ordinal around the parent giant. Giants and
   non-moons were to have no extra line; exact wording was a presentation choice.
3. **Useful rare veins.** Fire Ice veins and Spiniform entries were to be green
   when present. The owner considered other rares unimportant for this view;
   neither further highlights nor removal of their entries was requested.
   Fire Ice here meant mineable veins, not the giant's gas product.
4. **Contained home planet.** The entire home row was to be green if its orbit
   fitted within the star's maximum sphere radius. Other planets and an
   uncontained home planet were unaffected by this rule; independent resource
   highlights still applied.

## Statistics: Cluster Deuterium

- The requested title was **Nearest Rich Deuterium Gas-Giant**.
- Eligibility required `>= 0.15/s`, following the owner's final clarification.
  The owner expected the starter giant never to meet it; that claim did not
  authorize a separate hard-coded starter exclusion.
- Selection was to favor the nearest qualifying giant within the retained
  inclusive `8.125 ly` home distance, replacing highest-rate-first selection.
  No new Deuterium font-color rule was supplied.

## Statistics: Cluster rare resources

The requested colors applied to candidate cells in Closest and Alternative,
not resource headings or unrelated candidates. Distances were from home.

| Candidate | Requested green | Requested red | Requested default |
| --- | --- | --- | --- |
| Sulfuric Acid ocean | `< 3.5 ly` | `> 8.5 ly` | `3.5 ly` through `8.5 ly`, inclusive |
| Other listed rares | `< 3.5 ly` | No far-distance red rule | `>= 3.5 ly` |

A system qualified for the four-resource green rule only if already-presented
candidates represented it in each of Spiniform, Fire Ice veins, Organic Crystal
and Sulfuric Acid ocean. Either candidate column could supply a match, and
multiple planets of one system could contribute. All matching cells in those
four rows were to be green. Undisplayed resources elsewhere in a system could
not qualify it.

The owner expected this intersection to rule out far-away sulfuric candidates,
because closer alternatives would fill the other rows. That expectation was
an owner claim, not permission for a distance cutoff or expanded search.
Existing rare-resource selection/preference was to remain unchanged; this
section requested presentation changes only.

## Statistics: Cluster Unipolar Magnets

Rules were independent for each row's named cells; other cells retained their
normal color. Distances were from home, and comparisons were to use underlying
values rather than rounded strings. Planet selection was not to change.

| Cell | Requested green | Requested red | Requested default |
| --- | --- | --- | --- |
| Magnets | `> 2,000,000` | `< 900,000` | `900,000` through `2,000,000`, inclusive |
| Distance | `< 15 ly` | `> 21 ly` | `15 ly` through `21 ly`, inclusive |

## Statistics: Notable stars

1. **Group order.** Blue giants were requested first, brightest first, with
   wholly green rows; the cluster's brightest star next; remaining O stars by
   home distance next; up to five Aquatica rows by brightest-star distance last.
2. **Sphere facts.** Distance, Max Sphere and Contained were additions to the
   existing table, without replacing stellar Size. Sphere-row distance was
   from home; Max Sphere was radius in meters; Contained was the count of
   planets with contained orbits. No statistics luminosity cutoff was supplied;
   importing the conclusions `>= 2.0` filter was explicitly excluded.
3. **Aquatica hosts.** Non-O stars with at least one Aquatica-theme planet were
   eligible. The five nearest to the brightest star were requested when more
   than five matched; otherwise all matches. Stars, not matching planets,
   were counted. Only Luminosity and Contained were to be empty. Distance,
   selection and ordering were all relative to the brightest star, not home.

## Pre-change implementation observations

These observations supported planning; they did not prescribe a new
architecture, additional product features or separate work packages. They
refer to the code before the panel changes, not its present implementation.

- **Oil:** `HomeSystemBodyPresentation.ProjectTableRow` passed oil Amount
  through the ore `FormatAmount` helper, while `CompleteClusterRawCoordinator`
  summed native groups. The native writer's formula, units, rounding and
  settings treatment required inspection; guessing a divisor or changing
  finite-resource semantics was excluded.
- **Moon/containment attribution:** `HomeSystemBodyInventory` held satellite
  status and parent identity but lacked radius/ordinal. Verification of native
  per-parent order, including the home moon, was needed. Home identity was to
  determine its unique highlight. `DspPreviewGateway.MaximumShellRadius`
  supplied sphere radius separately from stellar radius; its containment
  counter compared each local `orbitRadius * 40_000` with the maximum. Moon
  geometry therefore required star-centric verification and consistent use
  in both panels.
- **Color scope:** `PreviewStatisticsPanelRenderer` used a common body style
  and string cells. The requested resource fragments, rare/Unipolar cells and
  full home/blue-giant rows required scoped styling and checks for unrelated
  text leakage, subline fit and added-column fit.
- **Four-resource intersection:** Existing displayed candidates retained
  `HostSystemIdentifier`, and `WriteLocation` cached it. The intended approach
  was intersection across the four rows and matching-cell coloring, without
  new scan retention, system aggregation or payload fields for this highlight.
  Existing two-candidate selection and cache/fresh agreement were to remain.
- **Notable-star facts:** `RuntimeNotableStarEvidence` held name, class/type,
  stellar radius, Dyson luminosity and stable order. The requested sphere facts
  and Aquatica membership needed system attribution. Existing pairwise
  distances could supply both origins. Native catalogue identity was to define
  the Aquatica match rather than a localized name; this theme criterion was
  distinct from the conclusions' deposit criterion. No exclusion based on
  appearance in another star group was requested.
- **Suggested checks:** Native oil parity, strict colors, nearest eligible
  Deuterium, parent-relative moons, home-only containment, displayed-only
  four-row intersections, star order, five-host selection, mixed origins and
  cache/fresh agreement. Only the final owner handoff could check rendering;
  implementer game execution/deployment was prohibited. These were suggestions
  for the requested changes, not an extra research gate or tests performed for
  this findings record.

## Scope boundaries supplied to the roadmap

The settled inputs were `>= 0.15/s` for both panels, the statistics cap of
`8.125 ly`, the recorded thresholds/aggregation scopes, displayed-candidate
highlight and explicit star order. The starter `0.03/m` anecdote remained a
source claim, not an implementation cutoff. A new rare-resource preference
scheme or combined moon verdict was not an unresolved owner decision.

Unrequested behavior had to remain unchanged. Native formatting, geometry,
projection and cache implications were implementation investigations, not
additional product decisions. The four-resource color rule alone did not
require a cache-schema change. No additional Compact expansion or other
conclusion-family changes were inferred.

Source locations associated with these notes (the files now contain later code):
[preview extraction](../../src/DSPSeedScanner.Plugin/DspPreviewGateway.cs),
[statistics projection](../../src/DSPSeedScanner.Runtime/PreviewStatisticsPresentation.cs),
[table rendering](../../src/DSPSeedScanner.Plugin/PreviewStatisticsPanelRenderer.cs),
[complete-scan aggregation](../../src/DSPSeedScanner.Runtime/CompleteClusterRawCoordinator.cs),
and [cache payload](../../src/DSPSeedScanner.Runtime/CompleteClusterConclusionCache.cs).

Return to the [documentation index](../INDEX.md).
