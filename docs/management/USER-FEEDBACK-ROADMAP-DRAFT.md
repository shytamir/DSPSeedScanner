# User Feedback Roadmap Draft

**Status:** Pending evaluation.

**Active user story:** None.

**Source:** [GitHub issue #1: User List of Desired Features and
Fixes](https://github.com/shytamir/DSPSeedScanner/issues/1).

This draft decomposes every independently valuable request in the issue.
Urgency describes how soon the work warrants attention; importance describes
its expected effect on product trust or player value. No story is authorized
or active.

## Source coverage

| Reported need | Preserved in |
| --- | --- |
| Avoid a complete planet rescan after a Dark Fog-only settings change | FEED-01 |
| Use literal tidal-lock, giant-type, and moon terminology | FEED-02 |
| Correct saturated contained-orbit and misleading shell classifications | FEED-03 |
| Show every starter planet's theme and exact Solar and Wind percentages | FEED-04 |
| Show exact starter ore availability, including mineable Fire Ice | FEED-05 |
| Show Unipolar Magnet vein count and nearest sulfuric-acid and rare access | FEED-06 |
| Locate the strongest deuterium-producing gas giants | FEED-07 |
| Inventory cluster planet themes, including Aquatica | FEED-08 |
| Show blue-giant and O-star size and luminosity, including the cluster maximum | FEED-09 |
| Replace judgment lanes with home-system and cluster characteristics | FEED-10 |

## FEED-01: Reuse planet results after Dark Fog-only changes

**State:** Proposed; inactive pending roadmap evaluation.

**Category:** bug-fix

**Urgency:** High

**Importance:** High

As a player adjusting Dark Fog settings for the same cluster, I want existing
planet conclusions reused so a settings-only change does not repeat the long
complete-cluster scan.

**Return:** Keep the full preview identity and current Dark Fog occupation
specific to the loaded settings, but give combat-independent complete results
a separately justified reuse key. Reuse is allowed only after proving the
retained planet evidence and conclusions are unchanged by the altered combat
inputs.

**Acceptance gate:** A Dark Fog-only change reuses a completed compatible
planet result without starting raw planet generation, while the neutral Dark
Fog status comes from the newly loaded preview. Seed, star count, resource
multiplier, generation provenance, scanner contract, or conclusion-contract
changes still miss. Cache-hit, replacement, cancellation, and preview-exit
fixtures pass without stale status or duplicate publication.

**Out of scope:** Reusing Dark Fog counts, weakening compatibility gates,
changing conclusion predicates, adding Dark Fog judgments, or sharing results
between game versions or materially different generation environments.

## FEED-02: Use literal starter-system terminology

**State:** Proposed; inactive pending roadmap evaluation.

**Category:** bug-fix

**Urgency:** High

**Importance:** High

As a player reading the Fresh start card, I want its labels to match the terms
used by the game so I can understand them without interpreting mod-specific
phrasing.

**Return:** Replace `No permanent solar sources` with a literal tidal-lock
statement. Describe each starter giant as gas or ice when known, and describe
the solid planets sharing each giant's orbit as moons with correct singular
and plural forms. Omit a fact when the required type or topology is not known.

**Acceptance gate:** Focused fixtures cover zero, one, two, and three starter
moons; gas and ice giants; more than one giant; tidal-lock presence and
absence; and incomplete evidence. No `permanent solar source` or `gas giant
neighbors` wording remains, and the correction does not change any underlying
predicate or scan.

**Out of scope:** New topology judgments, moon quality rankings, power-output
predictions, non-starter satellites, planet-theme catalogs, or changes to
conclusion thresholds.

## FEED-03: Restore discriminating sphere conclusions

**State:** Proposed; inactive pending roadmap evaluation.

**Category:** bug-fix

**Urgency:** Medium

**Importance:** High

As a player comparing sphere candidates, I want shell and contained-orbit
labels to distinguish materially different seeds so repeated or misleading
outcomes do not erode trust in the panel.

**Return:** Audit the accepted shell-size and contained-orbit classifications
against a representative reference-identity sample. Correct or suppress only
the outcomes shown to be saturated or semantically misleading, including the
reported `Many contained orbits` and O-star `Tiny shell` cases. Preserve the
neutral result when no defensible classification can be established.

**Acceptance gate:** The sample distribution, proposed boundaries, and named
edge cases are recorded before changing semantics. Deterministic fixtures cover
every retained class at both boundaries, ordinary seeds do not collapse into
one outcome, and the displayed result agrees with the underlying measured
radius or orbit count. Contract and cache versions change if semantics change.

**Out of scope:** Sphere design advice, receiver performance, aesthetics,
ranking star classes by reputation, new composite scores, or recalibrating
unrelated conclusions.

## FEED-04: Show starter layout and energy facts

**State:** Proposed; inactive pending roadmap evaluation.

**Category:** feature-request

**Urgency:** Medium

**Importance:** High

As a player choosing a start, I want the starter planets' types and renewable
energy factors shown directly so I can apply my own playthrough preferences.

**Return:** Add a bounded neutral starter-system facts section that lists every
solid starter planet once. Extend the bounded starter attribution with its
in-game planet-theme name, then show that name with the planet identifier and
exact Solar and Wind percentages. Use stable game order and current preview
data; omit unavailable fields rather than classifying or inventing them.

**Acceptance gate:** Fixtures cover all starter solid planets, distinct
in-game planet themes, exact Solar and Wind formatting, stable order, partial
evidence, and a preview reload. Facts do not appear under Strength,
Preference-sensitive, or Limitation headings and remain bounded by the starter
system.

**Out of scope:** Resource amounts, cluster-wide planet inventories, renewable
output predictions, preferred-planet selection, sorting by quality, or new
energy thresholds.

## FEED-05: Show exact starter ore availability

**State:** Proposed; inactive pending roadmap evaluation.

**Category:** feature-request

**Urgency:** Medium

**Importance:** High

As a player choosing a start, I want exact ore availability by starter planet
so I can see which local bodies provide each material, including Fire Ice.

**Return:** Preserve only the bounded per-planet starter resource aggregates
already produced during raw generation. Present each starter planet's known
ore amount and vein-group count with explicit units and the active resource
settings. A completed cache entry must retain only the presentation payload,
not raw planet or cluster objects.

**Acceptance gate:** Fixtures cover present and absent common ores, mineable
Fire Ice, multiple starter planets, resource multipliers, incomplete evidence,
scan completion, and cache reuse. Displayed amounts and vein groups match the
generated aggregates, and memory remains bounded to starter presentation data.

**Out of scope:** Cluster-wide ore retention, yield or throughput estimates,
terrain accessibility, gas-giant products, resource ranking, or claiming an
ore is usable when only a gas product is present.

## FEED-06: Add bounded cluster rare-access facts

**State:** Proposed; inactive pending roadmap evaluation.

**Category:** feature-request

**Urgency:** Low

**Importance:** High

As a player planning expansion, I want the nearest known source of each rare
resource and the cluster's Unipolar Magnet vein count so I can judge scarce
inputs from direct facts.

**Return:** Extend the existing incremental complete scan to retain one nearest
system and distance for each supported rare resource, the nearest sulfuric-acid
ocean, and the exact cluster Unipolar Magnet vein count. Resolve equal-distance
candidates deterministically and persist only the bounded presentation result.

**Acceptance gate:** Fixtures cover each supported rare, sulfuric-acid oceans,
known absence, equal-distance ties, exact Unipolar Magnet vein counts, current
resource settings, cache reuse, and deterministic display names. The scan does
not retain all planets or raw cluster objects, and unsupported or incomplete
facts are omitted.

**Out of scope:** Resource quality rankings, travel or logistics estimates,
all matching locations, raw cluster retention, high-deuterium gas giants,
planet-theme inventories, or O-star and giant-star measurements.

## FEED-07: Locate strong deuterium gas giants

**State:** Proposed; inactive pending roadmap evaluation.

**Category:** feature-request

**Urgency:** Low

**Importance:** Medium

As a player planning deuterium production, I want the strongest gas-giant
candidates identified by location so I can find them in the cluster preview.

**Return:** Extend the bounded preview projection with per-giant deuterium
collection rates. Present the three highest nonzero candidates using their
in-game planet and system identifiers and exact game-native rates. Call a
candidate `high-deuterium` only if a separately accepted threshold defines
that term; otherwise describe the ordered measurements neutrally.

**Acceptance gate:** Fixtures cover no deuterium, one and several candidates,
equal-rate ties, more than three candidates, incomplete attribution, stable
ordering, and preview replacement. Each displayed rate remains attached to
the correct planet and system, and the feature starts no raw planet scan and
retains no unbounded cluster objects.

**Out of scope:** Orbital-collector output predictions, logistics or travel
advice, hydrogen ranking, new gas-product thresholds, all-candidate display,
or starter-gas wording handled by FEED-02.

## FEED-08: Inventory cluster planet themes

**State:** Proposed; inactive pending roadmap evaluation.

**Category:** feature-request

**Urgency:** Low

**Importance:** Medium

As a player looking for particular worlds, I want to know which planet themes
exist in the cluster, including Aquatica, so I can judge whether the seed
contains the environments I want.

**Return:** Aggregate the lightweight generated preview's solid planets by the
runtime's in-game theme identifier and display name. Retain one exact count per
theme and up to three deterministic example systems per present theme. Report
known absence only for explicitly selected themes such as Aquatica; do not
start raw planet generation or retain planet objects after aggregation.

**Acceptance gate:** Fixtures cover multiple themes, repeated planets and
systems, Aquatica presence and absence, unknown theme identifiers, stable
example selection, preview replacement, and preview exit. Counts match the
loaded preview, names come from the active runtime catalog, no complete scan is
started, and retained memory is bounded by that catalog rather than cluster
planet count.

**Out of scope:** Terrain quality, buildable land, water or ocean resources,
theme rarity judgments, exhaustive planet locations, visual rankings,
sulfuric-acid access handled by FEED-06, or starter layout handled by FEED-04.

## FEED-09: Show notable-star measurements

**State:** Proposed; inactive pending roadmap evaluation.

**Category:** feature-request

**Urgency:** Medium

**Importance:** High

As a player comparing stellar candidates, I want the blue giants' and O-type
stars' sizes and luminosities shown directly so I can judge them without a
generic `No bright stars` verdict.

**Return:** Add a bounded neutral star-facts section for every blue giant and
O-type star in the requested cluster. Use the in-game display name and the
same physical-size and luminosity definitions and rounding shown by DSP.
Identify the cluster's maximum luminosity even when no listed star crosses an
existing brightness threshold. Keep stellar size distinct from maximum Dyson
sphere radius.

**Acceptance gate:** Fixtures cover no notable stars, multiple O stars,
multiple blue giants, equal luminosities, the cluster maximum belonging to
another type, stable game order, and incomplete preview evidence. Displayed
values match game-native fields and units, and `No bright stars` is absent
from the neutral facts output.

**Out of scope:** Reclassifying sphere conclusions, star desirability scores,
energy-production predictions, non-notable star catalogs, planet themes, or
raw precision beyond DSP's own display.

## FEED-10: Present neutral seed characteristics

**State:** Proposed; inactive pending roadmap evaluation.

**Category:** feature-request

**Urgency:** Low

**Importance:** High

As a player whose priorities vary by playthrough, I want home-system and
cluster characteristics presented without Strength, Preference-sensitive, or
Limitation judgments so I can make the tradeoffs myself.

**Return:** Replace the three judgment lanes with two concise factual groups:
`Home system` and `Cluster`. Compose them only from accepted, presentation-safe
facts delivered by the approved stories in this roadmap and existing neutral
status metadata. Use stable grouping, bounded entries, game terminology, and
explicit units where a measurement is shown. Omit unknown facts rather than
showing unknown or inferred judgments.

**Acceptance gate:** A copy matrix and representative panel snapshots are
approved before implementation. Automated presentation fixtures contain no
Strength, Preference-sensitive, Limitation, unsupported `No ...` verdict, or
internal conclusion role; preserve scan progress, Dark Fog status, scrolling,
cache reuse, seed replacement, and preview exit. Final installed-game human
validation confirms the factual panel remains readable at the accepted panel
size.

**Out of scope:** Changing evidence generation, thresholds, or compatibility;
adding a raw-data dump, alternate presentation modes, player preference
controls, seed scoring, comparisons, filters, charts, or localization.

## Evaluation boundary

No implementation sequence is approved. FEED-10 records the user's requested
replacement of conclusion lanes, but that product-contract change requires
explicit roadmap approval before any implementation. The issue's complete
request surface is preserved above; accepting one story does not implicitly
accept another.
