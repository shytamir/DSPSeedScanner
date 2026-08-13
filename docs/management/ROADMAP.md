# User Feedback Roadmap

**Status:** Active as of 2026-08-13.

**Active user story:** [FEED-06: Show home-system ore
availability](#feed-06-show-home-system-ore-availability) is undergoing
corrective owner validation.

**Source:** [GitHub issue #1: User List of Desired Features and
Fixes](https://github.com/shytamir/DSPSeedScanner/issues/1).

This active roadmap decomposes every independently valuable request in the
issue and records the approved presentation boundary for implementation.
Urgency describes how soon the work warrants attention; importance describes
its expected effect on product trust or player value. FEED-01 and FEED-02 were
accepted, `ready-for-new-panel` passed, and the **Clean slate for panel work**
milestone was established. FEED-03 and FEED-04 were accepted,
`ready-for-panel-population` passed, and the **Panel renders with all features
enabled even if not yet consumed** milestone was established. FEED-05 was
accepted. FEED-06 remains active while its narrow runtime and hosted-reference
corrections await owner validation. Later stories remain inactive behind their
documented gates.

## Source coverage

| Reported or derived need | Preserved in |
| --- | --- |
| Use literal tidal-lock and home-moon terminology | FEED-01 |
| Avoid a complete planet rescan after only the Dark Fog toggle changes | FEED-02 |
| Provide factual statistics without changing the conclusion panel | FEED-03 |
| Identify the current seed's home planet in both panel titles | FEED-04 |
| Show home-system giant type, planet themes, and exact Solar and Wind percentages | FEED-05 |
| Show exact home-system ore availability, including mineable Fire Ice | FEED-06 |
| Show the nearest sulfuric-acid ocean and nearest rare-resource locations | FEED-07 |
| Show per-planet Unipolar Magnet supply and distribution | FEED-08 |
| Locate the strongest nearby deuterium-producing gas giant | FEED-09 |
| Show blue-giant and O-star size and luminosity, including the cluster maximum | FEED-10 |
| Re-evaluate saturated contained-orbit and misleading shell conclusions | FEED-REJ-01 |
| Re-evaluate useful theme statistics beyond an Aquatica special case | FEED-REJ-02 |

## Presentation decision

The accepted conclusion panel remains the configured surface for bounded seed
conclusions. This roadmap does not replace its Strength, Preference-sensitive,
and Limitation lanes. FEED-01 may correct that panel within its explicit scope;
FEED-REJ-01 is tabled and authorizes no change. The factual stories do not
alter it.

A separate statistics panel will present neutral facts under `Home system` and
`Cluster`. Its corner is the horizontal opposite of the configured conclusion
corner: bottom right pairs with bottom left, and top right pairs with top left,
in either direction. Derive its bounds by horizontally reflecting the resolved
conclusion-panel bounds, preserving the exact vertical position, width, and
height. Mirror the horizontal UI anchor and pivot while retaining the same
top-or-bottom anchor. Do not apply the opposite corner's independent clearance
rules or add a second placement setting.

The statistics panel will have its own presentation document, renderer,
scrolling, and visual state. It will consume the same active preview session,
normalized evidence, and shared layout result as the conclusion panel, but
neither panel may own, query, or drive the other. Shared semantic values and
geometry belong at the runtime or presentation boundary rather than in either
renderer.

Both panels retire from the same preview lifecycle authority. Preview facts
may appear immediately; raw-scan facts appear only after completion or a valid
cache hit. Unavailable facts are omitted. Measurements use concise natural
labels and explicit game units. Cached statistics retain only their bounded
presentation payload, never raw planets or a complete cluster graph.

The configured conclusion corner remains authoritative for the pair. An
independent statistics-panel position, a generic multi-panel framework, and
changes to the accepted conclusion-panel dimensions or anchor are not part of
this roadmap.

## Phase gates

Document order defines implementation order. Each gate closes the phase named
in its definition and must pass before the next story begins. Passing a story's
acceptance gate does not by itself pass a phase gate or milestone. FEED-REJ-01
and FEED-REJ-02 remain tabled and unauthorized.

### `ready-for-new-panel`

**State:** Passed on 2026-08-13. The owner accepted FEED-01 and FEED-02 with no
unresolved residual or blocking issue, establishing the **Clean slate for
panel work** milestone.

The pre-new-panel phase is complete when FEED-01 and FEED-02 have passed their
acceptance gates. Their conclusion-copy, topology, reuse-key, active-preview
status, cache, replacement, cancellation, and exit behavior have no unresolved
regression that would contaminate new panel work. This gate establishes the
**Clean slate for panel work** milestone.

### `ready-for-panel-population`

**State:** Passed on 2026-08-13. The owner accepted FEED-03 and FEED-04,
establishing the **Panel renders with all features enabled even if not yet
consumed** milestone.

The panel-scaffold-and-prior-concern phase is complete when FEED-03 and FEED-04
have passed their acceptance gates. The statistics panel renders in every
configured corner pairing with its independent document, scrolling, lifecycle,
empty `Home system` and `Cluster` containers, home-system inventory, keyed
cluster collection, titled-subsection support, shared location formatting, and
shared home-planet title value functional and tested. No dummy statistic line
is required. This gate establishes the **Panel renders with all features
enabled even if not yet consumed** milestone.

### `ready-for-cluster-panel-population`

The home-system-panel phase is complete when FEED-05 and FEED-06 have passed
their acceptance gates. Every supported home-system body fact and exact raw-
resource fact is attached to the correct body row, cache and lifecycle behavior
remain valid, and the conclusion panel is unchanged. This gate establishes the
**Panel home system fully populated** milestone.

### `ready-for-subsection-consumer`

The cluster-panel phase excluding subsections is complete when FEED-07,
FEED-08, and FEED-09 have passed their acceptance gates. Their bounded cluster
selections, locations, distances, exact resource facts, cache payloads,
ordering, and absence behavior coexist without duplicate ownership or stale
presentation. This gate establishes the **Panel cluster populated excluding
subsection** milestone.

### `ready-for-end-to-end-testing`

The cluster-subsection phase is complete when FEED-10 has passed its acceptance
gate. The `Notable stars` subsection uses the scaffold's existing document and
scrollbar, remains correct at the full star-count bound, and coexists with every
earlier home-system and cluster item. This gate establishes the **Panel fully
populated** milestone and authorizes release-candidate work; it does not approve
a release candidate.

## Release-candidate milestones

**Release candidate ready:** All roadmap stories and phase gates have passed;
the full automated suite, release build, package validation, and installed DSP
end-to-end procedure pass on one candidate artifact; cache miss and hit, Peace
and Combat, Dark Fog toggle reuse, seed replacement, preview exit, scrolling,
supported panel corners, and coexistence with the normal plugin set are
covered; and residual issues are recorded with no release blocker open.

**Release candidate approved:** The owner completes the final human validation
against that exact candidate artifact and explicitly accepts it. Technical
checks, a successful package build, or absence of reported defects cannot infer
this milestone.

## FEED-01: Use literal home-system terminology

**State:** Accepted on 2026-08-13.

**Category:** bug-fix

**Urgency:** High

**Importance:** High

As a player reading Fresh start conclusions, I want familiar home-system
terminology so I can understand the result without interpreting mod-specific
phrases.

**Return:** In the conclusion panel only, replace `permanent solar source`
phrasing with literal tidal-lock wording. Name up to three known qualifying
home planets using `[Planet] is tidally locked` or `[Planets] are tidally
locked`; use `[N] home planets are tidally locked` above that display cap and
`No tidally locked home planets` for known absence.

Replace the ambiguous shared-giant count with explicit home-planet topology.
At preview normalization, verify that the home planet is a solid planet in the
home system. When it directly orbits the star, record that known topology, use
`Home planet is not a moon`, and do not count moons elsewhere in the system.
When it has a parent, resolve that exact same-system parent from the orbit
reference and require the parent to be a generated gas or ice giant. Count only
solid planets whose orbit reference identifies that same parent, including the
home planet. Use `1 moon orbits the home giant` or `[N] moons orbit the home
giant`. A missing, non-giant, cross-system, or otherwise inconsistent parent
makes only this conclusion unknown and therefore omitted. The presenter must
consume the verified topology rather than infer parentage from an aggregate
count.

**Acceptance gate:** Focused fixtures cover one through three named tidally
locked home planets, the above-cap count form, known absence, and incomplete
evidence. Topology fixtures cover a direct-orbit home planet; one through three
moons sharing its verified giant; another giant with moons while the home
planet directly orbits the star; moons of a different giant; gas giants with
no moons; unrelated star-orbiting planets; a home planet assigned to the wrong
system; and missing, non-giant, or cross-system parent references. Only the
verified same-parent moons are counted, including the home planet. Known
direct orbit is distinct from invalid or unknown parentage. No `permanent solar
source` or `gas giant neighbors` wording remains. The narrow topology-evidence
correction does not change generation, another predicate, a statistics line,
or unrelated conclusion copy.

**Out of scope:** Displaying giant type, topology elsewhere in the cluster,
moon rankings, power-output predictions, factual statistics, or conclusion
thresholds.

**Implemented:** Preview normalization now resolves the home planet's native
orbit parent, verifies that it is a generated giant in the same home system,
and records either known direct-star orbit or the exact same-parent solid-moon
count. Inconsistent topology omits only this conclusion. The presenter consumes
that verified value and uses literal tidal-lock and home-giant wording; the
existing topology predicate and every unrelated conclusion remain unchanged.

**Acceptance evidence:**

- tidal-lock fixtures covered one, two, and three named planets, a four-planet
  count, known absence, and incomplete attribution;
- topology fixtures covered direct orbit, one through three verified moons,
  unrelated star-orbiting planets, another giant with moons, a moon of another
  giant, a giant without moons, and wrong-system, missing, non-giant,
  cross-system, mismatched, and malformed parent evidence;
- only solid planets referencing the home planet's exact verified parent were
  counted, and invalid topology omitted only its presentation line;
- production-code inspection found no `permanent solar source` or `gas giant
  neighbors` wording and confirmed no raw-scan, cache, statistics-panel, or
  unrelated predicate change; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 Core checks passed, and all 68 Runtime checks passed.

**Produced at this gate:** Immutable normalized home-planet topology,
native-parent preview verification, literal Fresh start copy, and focused
acceptance fixtures. Interactive DSP presentation validation was not required
by this story's automated acceptance gate and was not performed.

## FEED-02: Reuse results across the Dark Fog toggle

**State:** Accepted on 2026-08-13.

**Category:** feature-request

**Urgency:** High

**Importance:** High

As a player toggling Dark Fog for an otherwise identical cluster, I want
compatible planet results reused so that single choice does not repeat the
long complete-cluster scan.

**Return:** Give the audited, toggle-invariant completed payload a reuse key
that omits only Peace/Combat mode. Keep the canonical identities distinct,
retain the reused payload's source identity and provenance, attach its
presentation to the active preview session, and resolve mode and Dark Fog
status only from that newly loaded preview. Admit only fields covered by the
[generation-identity applicability finding](../specification/GENERATION-IDENTITY.md#peacecombat-reuse-applicability);
later fields must establish the same invariance before joining the payload.

**Acceptance gate:** Peace-to-Combat and Combat-to-Peace fixtures reuse the
completed audited payload without starting raw planet generation. The active
preview supplies its own mode and Dark Fog status and owns the republished
presentation while the payload retains its source provenance. The two full
canonical identities remain unequal. Seed, star count, resource multiplier,
numeric combat values, generation provenance, scanner contract, conclusion
contract, or any other identity change still misses. Cache-hit, replacement,
cancellation, and exit fixtures prove no stale identity, status, or duplicate
publication. A fixture with an unaudited payload field proves it is not reused.

**Out of scope:** Reuse across changes to numeric Dark Fog or other combat
settings, reusing Dark Fog counts, weakening compatibility or canonical
generation identity, changing conclusion predicates, adding Dark Fog
judgments, or sharing results between game versions or materially different
generation environments.

**Implemented:** Cache schema 8 gives only the audited completed payload a
mode-neutral reuse key while preserving mode in the source identity stored
inside the entry. A hit attaches source-attributed reports to the newly loaded
preview session; that active preview remains authoritative for its identity,
mode, immediate facts, and Dark Fog status. The full canonical identities are
unchanged and still distinguish Peace from Combat.

**Acceptance evidence:**

- Combat-to-Peace and Peace-to-Combat fixtures reused one completed payload
  without a second raw-generation operation, while the active panel identity
  and Dark Fog status reflected the newly loaded mode;
- the cached payload and every reused report retained the original source mode
  and provenance instead of being rewritten as active-preview evidence;
- seed, star count, resource multiplier, numeric combat settings, runtime and
  generation fingerprints, scanner and conclusion contracts, and other key
  changes remained misses; incomplete cross-mode replacement cancelled and
  restarted rather than reusing partial work;
- persistence admitted only the audited Fresh start resource, cluster-resource,
  rare-access, derived-role, and compact-route report families; a synthetic
  future field was excluded until separately proved invariant;
- schema 7 entries became safe misses under schema 8, and existing corruption,
  bounds, atomic replacement, retention, filesystem-failure, cache-hit,
  cancellation, replacement, and exit fixtures remained green; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 Core checks passed, and all 70 Runtime checks passed.

**Produced at this gate:** A bounded cross-mode reuse identity, explicit source
provenance in persisted payloads and active attempts, an audited field
allowlist, schema 8 persistence, and focused two-direction lifecycle fixtures.
Interactive DSP validation was not required by this story's automated gate and
was not performed.

## FEED-03: Establish the statistics panel

**State:** Accepted on 2026-08-13.

**Category:** feature-request

**Urgency:** High

**Importance:** High

As a player comparing a seed's facts with its conclusions, I want a separate
statistics panel opposite the configured conclusion panel so factual detail
does not displace or dilute its conclusions.

**Return:** Add the complete visual scaffold needed by the later statistics
stories. Resolve the conclusion panel from its existing corner configuration,
then horizontally mirror those bounds for the statistics panel. Give the new
panel the mirrored horizontal anchor and its own presentation document,
renderer, scroll position, and visual state while subscribing it to the
existing preview-session lifecycle. At the runtime/presentation boundary,
establish one immutable, preview-session-owned home-system body inventory from
lightweight preview evidence. Preserve each body's runtime identity, display
designation, primary-or-satellite parent relationship, and stable game order
so later stories can project facts without rediscovering the system. Render the
current preview title and visibly separated empty `Home system` and `Cluster`
subcontainers with their headings. Give the presentation document an empty,
ordered `Cluster` statistics collection whose stable item keys allow later
stories to add or replace bounded presentation facts without sharing game
protos, raw runtime objects, or renderer state. Allow cluster items to belong
to an optional keyed, titled subsection with stable subsection and item order.
All subsections remain in the statistics panel's one document and one scroll
flow; they do not own nested scrolling, independent lifecycle, or renderer
state. Define one immutable bounded cluster-location value containing body
identity, display designation, host-system identity, measured host-system
distance from home, and stable game order. Provide its common DSP-AU formatter:
`0 AU` for the home system and three significant figures otherwise, without
changing measurement precision. Later stories construct locations only for
their selected results; the scaffold does not enumerate or retain the cluster.
Do not add body, subsection, or statistic lines. Treat DSP planet numbers by
their generated scope: primary numbers identify star-orbiting bodies, while
moon display ordinals may repeat primary or other moon numbers. Resolve a
satellite through its `orbitAround` primary number and use an available parent
object reference only to corroborate that relationship; body ID remains the
inventory identity.

**Acceptance gate:** Layout fixtures cover all four configured conclusion
corners and prove the statistics panel uses the horizontally opposite corner
with identical `y`, width, and height, a reflected `x`, a mirrored horizontal
anchor and pivot, and the same vertical anchor. Supported-resolution fixtures
prove the paired panels do not overlap. The calculation reuses one
authoritative conclusion layout and introduces no second corner setting. Both
empty subcontainers are visually distinct and headed exactly `Home system` and
`Cluster`. Inventory fixtures cover primary planets, satellites belonging to
different primaries, giants with and without satellites, repeated
primary/moon display numbers, and repeated moon ordinals. Primary numbers
remain unambiguous; repeated moon numbers do not reject the inventory. Every
home-system body appears exactly once in stable game order with its correct
body-ID identity and parent, and the inventory is replaced or retired with its
preview session. The empty cluster collection accepts independently keyed
presentation items in
stable order and replacement does not duplicate an item. Subsection fixtures
cover untitled items, multiple titled subsections, several items in one
subsection, stable ordering, empty-section omission, and keyed replacement
without duplicate headings or nested scrolling. Location fixtures cover
distinct bodies in one host system, equal-distance systems, `0 AU`, three-
significant-figure formatting, and stable game-order tie handling without
candidate selection or a cluster inventory. The scaffold appears with the
active preview; replacement resets its document and scroll position; exit
removes its visible state; and a retired session cannot restore it. Its
document, scrolling, home inventory, cluster collection, or failure cannot
change the conclusion panel's content or state. Existing conclusion-panel
snapshots remain unchanged.

**Out of scope:** Body, subsection, or statistic lines; gas-or-ice, theme,
energy, resource, or other body classifications; home-planet title attribution;
independent placement configuration, resizing or re-anchoring the conclusion
panel, cluster enumeration or candidate selection, shared renderer state, a
generic panel framework, nested scrolling, caching, or installed-game layout
approval.

**Implemented:** The plugin now renders an independently owned statistics
document opposite the configured conclusion panel using a horizontal reflection
of the one authoritative conclusion layout. It displays the active preview
identity and distinct empty `Home system` and `Cluster` containers in its own
scroll flow. Lightweight preview normalization supplies one immutable,
session-owned home-system body inventory. Runtime presentation contracts also
provide bounded keyed cluster items with optional ordered subsections and a
shared immutable cluster-body location with DSP-AU formatting for later
consumers; no statistic line was added.

**Acceptance evidence:**

- all four configured corner pairings at 1080p, 1440p, and 4K preserved the
  conclusion panel's `y`, width, height, and vertical anchor while reflecting
  `x` and the horizontal anchor without overlap or a second setting;
- inventory fixtures covered primary planets, satellites of different
  primaries, primaries with and without satellites, and DSP's separate primary
  and moon number sequences, preserving every body exactly once in stable game
  order with its resolved parent;
- cluster-document fixtures covered untitled items, multiple ordered titled
  subsections, multiple items per subsection, empty-subsection omission, stable
  item order, bounded keyed replacement, and rejection of conflicting headings;
- location fixtures covered separate bodies in one system, equal-distance
  stable-order ties, `0 AU`, and three-significant-figure DSP-AU formatting
  without introducing cluster enumeration or candidate selection;
- lifecycle fixtures proved new-session scroll reset, inventory replacement,
  exact exit retirement, stale-session rejection, and independent conclusion
  document and state ownership; existing conclusion snapshots remained
  unchanged; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 Core checks passed, and all 75 Runtime checks passed.

**Produced at this gate:** An independent statistics presentation document,
controller, renderer, and scroll state; horizontally mirrored panel geometry;
empty headed containers; session-owned home-body inventory; bounded cluster and
subsection contracts; a common cluster-location/AU value; and focused acceptance
fixtures. Installed-game visual layout approval remains explicitly outside this
story and was not performed.

## FEED-04: Share the home-planet title designation

**State:** Accepted on 2026-08-13.

**Category:** feature-request

**Urgency:** Medium

**Importance:** High

As a player viewing a seed preview, I want both panel titles to name the
current home planet so I can connect conclusions and statistics to the planet
where the game begins.

**Return:** Resolve the home planet's exact game-provided display
designation, such as `Aspidiske III`, once from the active lightweight preview.
Store it as one immutable, preview-session-owned presentation value. Compose
the shared identity title from that value and let both renderers consume the
title independently; neither panel may reference the other or reconstruct the
designation from an internal ID.

**Acceptance gate:** Both panel titles gain the same exact designation as soon
as it is available. Replacement clears the old title before the new preview is
resolved, exit removes both, and a late result from a retired session cannot
restore stale text. Re-entry and returning to a previously viewed seed resolve
the value from the active preview. Title composition has one authoritative
implementation, while each panel retains independent content, layout, scroll,
and visual state.

**Out of scope:** Renaming stars or planets, identifying other planets,
persisting the designation separately, changing title fields beyond the
home planet, coupling the renderers, or changing either panel's layout.

**Implemented:** Lightweight preview normalization resolves the exact display
designation of DSP's `birthPlanetId` from its home-system planet and carries
only that string across the runtime boundary. The active preview session accepts
it once as an immutable presentation value. The single identity-title formatter
adds `Home [designation]`, and each panel independently composes its own document
from the same session value. Session retirement destroys the designation.

**Acceptance evidence:**

- a successful lightweight preview gave both panel documents the identical
  exact designation and updated them in the completed-load turn;
- mutation after attachment was rejected, proving one immutable session-owned
  value rather than two renderer copies or title reconstruction from an ID;
- replacement retired the first session, destroyed its designation, reset both
  documents to the replacement identity, and rejected late updates from the old
  session;
- preview exit destroyed the current designation and prevented either retired
  document from returning, while re-entry to an earlier seed resolved its
  designation again from the newly active preview;
- title composition remained one shared formatter while conclusion content,
  statistics content, layout, scroll, renderer state, caching, and scan
  selection remained independent; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 Core checks passed, and all 76 Runtime checks passed.

**Produced at this gate:** Exact home-planet designation extraction, one
immutable preview-session presentation value, shared title composition, same-
turn independent document updates, retirement cleanup, and focused lifecycle
fixtures. Interactive DSP presentation validation was not required by this
story's automated gate and was not performed.

## FEED-05: Show home-system layout and energy facts

**State:** Accepted on 2026-08-13.

**Category:** feature-request

**Urgency:** Medium

**Importance:** High

As a player choosing a start, I want the home system's layout and renewable
energy facts shown directly so I can apply my own playthrough preferences.

**Return:** Populate the statistics panel for the first time. Under `Home
system`, project the FEED-03 body inventory once. Identify every giant as gas
or ice. For every solid planet, show its in-game display designation,
planet-theme name, and exact Solar and Wind percentages. Retain inventory order
and omit unavailable fields without suppressing the known body.

**Acceptance gate:** Fixtures cover gas and ice giants, every home-system solid
planet, distinct runtime planet themes, exact Solar and Wind formatting,
one displayed entry per inventory body, stable order, partial fields,
replacement, and exit. FEED-05 neither re-enumerates the galaxy nor creates a
second body collection. Facts appear only in the statistics panel, the complete
scan does not start, and conclusion-panel content remains unchanged.

**Out of scope:** Ore availability or amounts, cluster planet themes, energy
output predictions, preferred-planet selection, sorting by quality, or new
energy thresholds.

**Implemented:** The existing lightweight home-system body projection now
retains DSP's body kind, theme display name, and exact Solar and Wind ratios in
the same stable inventory entry. DSP's native per-planet ice flag distinguishes
ice giants from gas giants. The statistics renderer formats one row per body in
inventory order, omits unavailable fields, and sizes the `Home system`
container to its wrapped rows. No raw resource or cluster fact was added.

**Acceptance evidence:**

- fixtures covered solid planets with distinct themes, exact whole and
  fractional percentages, gas and ice giants, and partially available fields;
- the body-evidence source was enumerable only once, the immutable inventory
  remained the sole body collection, and formatting preserved one row per body
  in stable game order;
- statistics-document updates exposed the lightweight facts before any raw
  planet work advanced, caused no additional complete-scan session, replaced
  the prior seed's rows, and cleared them on preview exit;
- conclusion presentation and conclusion contracts were unchanged; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 Core checks passed, and all 77 Runtime checks passed.

**Produced at this gate:** One-pass home-body fact capture, exact percentage
formatting, gas/ice labels, dynamically sized and scrollable home-system rows,
compatibility-member checks for the newly consumed DSP fields, and focused
lifecycle fixtures. Interactive DSP visual validation was not required by
this story's automated gate and was not performed.

## FEED-06: Show home-system ore availability

**State:** Corrective build pending owner validation as of 2026-08-13.

**Category:** feature-request

**Urgency:** Medium

**Importance:** High

As a player choosing a start, I want mineable ores attributed to their home
planets and giant gas products identified separately so I can tell which local
resources can be extracted from each source.

**Return:** During existing home-system raw generation, retain the set of ore
types present on each solid planet as a bounded payload keyed to its FEED-03
body identity. Join that payload while composing the existing body row rather
than mutating the inventory or creating another row. Solid-body resource text
uses `Ores:`, names mineable Fire Ice as `Fire Ice veins`, and lists only ores
confirmed present. Giant rows use the separate label `Gas products:` and name
collected Fire Ice as `Fire Ice`; they never receive an ore field. Persist only
this presentation-safe resource payload for cache reuse. Preview gas products
may appear immediately; ore fields join the same rows only when their raw
evidence completes or a valid cache entry supplies it.

**Acceptance gate:** Fixtures cover present and absent common ores, mineable
Fire Ice, Fire Ice as a gas product, multiple solid planets, giants with and
without gas products, incomplete generation, scan completion, and cache reuse.
Each resource populates the correct existing body row exactly once without
changing inventory order or removing FEED-05 facts. Scan progress and
completion do not duplicate a body row or gas-product field. A giant never
displays an ore field, missing-ore value, `None`, `N/A`, or an equivalent nil
marker. `Ores: Fire Ice veins` and `Gas products: Fire Ice` remain visibly
distinct. An absent ore is not listed, and unavailable evidence is omitted
rather than presented as absence. Neither raw planet objects nor cluster ore
data are retained.

**Out of scope:** Ore amounts, node or vein-group counts, terrain access,
resource rankings, cluster-wide ore inventory, gas-product rates, or mining
throughput.

**Implemented:** Lightweight preview normalization attaches supported gas
product names to each giant's existing inventory entry. The complete scan
distills each fully generated home-system solid planet to a bounded body-ID and
present-ore set, discarding raw nodes, groups, amounts, and cluster ore data.
The statistics formatter joins that payload to the existing row only after
complete coverage or a valid cache hit. Cache schema 9 persists only this
presentation-safe home-resource payload alongside audited conclusions.

**Acceptance evidence:**

- fixtures covered common ores, absent ores, mineable Fire Ice, Fire Ice gas,
  multiple solid planets, and giants with and without products;
- gas products appeared from lightweight preview evidence, while ore fields
  remained absent during incomplete generation and appeared only after complete
  coverage or cache reuse;
- each payload joined its matching inventory body without adding, duplicating,
  suppressing, or reordering rows and without removing FEED-05 facts;
- giant rows never received `Ores:`, empty resource sets produced no nil marker,
  and `Ores: Fire Ice veins` remained distinct from `Gas products: Fire Ice`;
- cancellation and raw failure published no partial home-resource payload, and
  the cache round trip retained only bounded presentation data; and
- the Release solution and installed-game plugin built with zero warnings, all
  14 Core checks passed, and all 78 Runtime checks passed.

**Produced at this gate:** Immediate giant gas-product labels, complete-only
per-body ore sets, presentation-time row joining, cache schema 9 persistence,
and focused completion, cache, cancellation, replacement, and exit fixtures.
Interactive DSP visual validation was not required by this story's automated
gate and was not performed.

**Corrective finding:** The first interactive roadmap test found that the
hosted artifact referenced `ThemeProtoSet.dataArray`, while DSP declares that
field on `ProtoSet<ThemeProto>`, causing immediate scans to fail. Direct-build
testing then isolated empty rows to an invalid system-wide uniqueness rule for
`PlanetData.number`: DSP independently numbers primaries and each giant's
moons, so valid values repeat. The hosted compile contract now mirrors the real
declaring type. Home-body projection requires uniqueness only among primary
numbers, retains body ID as identity, resolves satellites from their
`orbitAround` primary number, and uses an available parent object reference
only as corroboration. Fixtures preserve the observed Menkent pattern of
primary numbers `1, 2` and moon numbers `1, 2` without weakening primary-parent
validation. The scan regression, cache/replacement lifecycle, and direct-build
home rows passed interactive checks; owner validation of the corrected hosted
artifact and the home-system population phase gate remains pending.

## FEED-07: Show nearest rare-resource access

**State:** Pending; inactive until `ready-for-cluster-panel-population` passes.

**Category:** feature-request

**Urgency:** Low

**Importance:** High

As a player planning expansion, I want two nearby candidates for sulfuric acid,
Fire Ice veins, Fractal Silicon, Kimberlite, Optical Grating Crystal, Organic
Crystal, and Spiniform Stalagmite Crystal so one unsuitable planet does not
make the statistic a dead end.

**Return:** Add bounded `Cluster` statistics for the two closest generated
unique planet candidates for each of these seven categories: sulfuric-acid
ocean, generated Fire Ice veins, Fractal Silicon, Kimberlite, Optical Grating
Crystal, Organic Crystal, and Spiniform Stalagmite Crystal. Fire Ice gas giant
products never qualify for the Fire Ice pair. Unipolar Magnets are explicitly
excluded and belong only to FEED-08. Multiple deposits on one planet do not
consume multiple slots. Construct FEED-03 cluster-location values for selected
planets and use their shared home-to-host-system DSP-AU formatter. Rank the
existing measurements without introducing a new tolerance or precision model.
If exactly two candidates share the closest measured distance, preserve and
display that pair without a secondary tie-breaker. If more than two share it,
retain the first two in stable game order without implying that one tied
candidate is better. A completed scan with no candidate displays `No
<resource> found`. Resolve natural resource names and planet designations
before adding the keyed presentation facts to the FEED-03 cluster collection;
do not expose game protos to its document, renderer, or cache.

**Acceptance gate:** Fixtures cover sulfuric-acid oceans and all six named rare
ores, including separate Fire Ice vein planets and Fire Ice gas giants; zero,
one, and two or more candidates; a two-way tie for closest; distinct planets in
one host system; multiple deposits on one planet; a tie larger than the result
bound; home-system candidates; planet attribution; shared location formatting;
scan completion; and cache reuse. Each named category owns one stable cluster
item containing at most two correctly ordered, unique planets. The Fire Ice
item contains only planets with generated veins; no gas giant or Unipolar
Magnet item is produced. Distances describe host-system separation rather than
an interplanetary route; no unselected location, game proto, or raw cluster
object is retained.

**Out of scope:** Resource amounts, vein counts, route quality, planet-orbit
distance, travel time, logistics advice, more than two selected locations per
resource, Fire Ice gas products, Unipolar Magnets, deuterium giants, or planet-
theme inventory.

## FEED-08: Show per-planet Unipolar Magnet supply

**State:** Pending; inactive until FEED-07 is accepted.

**Category:** feature-request

**Urgency:** Low

**Importance:** High

As a player evaluating a cluster's scarcest ore, I want every Unipolar Magnet
planet identified with its generated supply and distribution so I can compare
the actual locations directly.

**Return:** Populate `Cluster` with one Unipolar Magnet line per planet that
contains generated deposits. Each line shows the planet's in-game display
designation, its host system's distance from the home system, exact vein-node
count, exact total Unipolar Magnet amount, and exact vein-group count. Reuse
FEED-03's cluster-location value, DSP-AU formatter, and stable ordering. Treat
vein-group count as the existing factual distribution measure; do not rename
it density or derive a new ratio. The amount is the exact generated runtime
value under the active resource setting; do not normalize or estimate it. Use
the line form `<planet> - <distance> - <nodes> veins - <amount> magnets -
<groups> groups`, with natural singular forms and invariant grouped integers.
Planets in one system retain their distinct lines. A completed scan with no
qualifying planet displays `No Unipolar Magnets found`. Persist only these
bounded per-planet presentation facts for the active complete-scan identity
and cache reuse.

**Acceptance gate:** Fixtures distinguish vein nodes, vein groups, and resource
amount; cover no deposits, one planet, multiple planets in one and several
systems, multiple groups per planet, planet attribution, host-system distance,
equal-distance systems, singular and plural labels, grouped integer formatting,
active resource settings, stable order, scan completion, and cache reuse. Each
qualifying planet produces exactly one keyed cluster item with exact counts;
replacement and cache resolution cannot duplicate it, and changing a
resource-setting identity cannot reuse a mismatched amount. FEED-08 does not
depend on, replace, or suppress a FEED-07 item. No conclusion-panel text
changes. No individual vein position, raw planet, game proto, or other cluster
resource data is retained.

**Out of scope:** Mining yield, vein density, group geometry, Veins
Utilization, other resource counts, resource rankings, route quality, or
changes to Unipolar Magnet conclusions.

## FEED-09: Show the strongest nearby deuterium gas giant

**State:** Pending; inactive until FEED-08 is accepted.

**Category:** feature-request

**Urgency:** Low

**Importance:** Medium

As a player planning deuterium collection, I want the strongest Deuterium gas
giant within a practical distance of home identified so an extreme remote rate
does not displace the candidate I can reasonably use.

**Return:** Add one bounded `Cluster` statistic for the highest-rate gas giant
within 8.125 light-years of the home system whose lightweight preview products
include Deuterium. While traversing the existing lightweight preview, map each
matching giant to its body identity, designation, host system, stable order,
distance, and exact Deuterium rate; reject candidates beyond the existing
8.125-light-year close-candidate cutoff; and incrementally retain one winner.
This cutoff is one quarter of the accepted `[2.5, 10]` distance band below its
permissive upper bound: `10 - ((10 - 2.5) / 4)`. It does not change the shared
distance predicate or create a new conclusion class.
Highest exact game-provided rate wins. An exact-rate tie selects the nearer
host system, then stable game order. Do not introduce a rate threshold,
tolerance, or system-aggregated candidate. Construct the winner's FEED-03
cluster-location value and show its planet designation, shared DSP-AU distance,
and exact rate. A complete lightweight preview with no qualifying candidate
displays `No Deuterium gas giants within 8.125 ly`; incomplete attribution omits
the item rather than claiming absence.

**Acceptance gate:** Fixtures cover no Deuterium product; candidates inside,
at, and beyond 8.125 light-years; one and several in-range candidates; a farther
in-range giant with a higher rate; an exact-rate tie resolved by distance; an
exact-rate-and-distance tie resolved by stable game order; a home-system
candidate; exact planet/rate attribution and game display formatting;
incomplete attribution; replacement; exit; and returning to a prior seed. The
one keyed cluster item contains only the selected winner. System aggregation
cannot merge or fabricate a giant candidate, partial attribution never
produces a false absence, and no rejected candidate survives projection. No
raw scan starts, no game proto is retained, and conclusion-panel content does
not change.

**Out of scope:** A new `high-deuterium` rate threshold, candidates beyond
8.125 light-years, multiple displayed candidates, orbital-collector output,
logistics or travel advice, hydrogen ranking, Fire Ice gas products, or home-
system gas conclusions.

## FEED-10: Show notable-star measurements

**State:** Pending; inactive until `ready-for-subsection-consumer` passes.

**Category:** feature-request

**Urgency:** Medium

**Importance:** High

As a player comparing stellar candidates, I want the blue giants' and O-type
stars' sizes and luminosities shown directly so I can judge them without a
generic brightness verdict.

**Return:** Add `Cluster` statistics for every blue giant and O-type star in
the requested preview under one `Notable stars` subsection. Begin with the
compact summary `<O count> O stars - <blue-giant count> blue giants`. Then show
one compact line per qualifying star using its in-game display name, displayed
star type, and DSP's displayed stellar size and luminosity definitions and
rounding. Classify and count stars by DSP's displayed star type so each result
belongs to exactly one group; show O stars first and blue giants second, with
stable game order within each group. Identify the cluster's maximum luminosity:
append `Brightest` to its existing row when it already qualifies, otherwise add
one `Brightest` line for that non-notable star. Keep stellar size distinct from
maximum Dyson sphere radius and use the statistics panel's existing single
scroll flow for long lists.

**Acceptance gate:** The game fields and display units are verified before
implementation. Fixtures cover no notable stars, multiple O stars and blue
giants, the full configured star-count bound, singular and plural summary
forms, a giant with an O spectral class but a blue-giant display type, equal
luminosities, a maximum belonging to each notable type and to another type,
nonduplicated `Brightest` marking, stable grouping and order, partial evidence,
replacement, exit, and returning to a prior seed. Counts use the same exclusive
display classification as the rows. Every qualifying star appears exactly
once, `Brightest` appears at most once, and long content uses the parent
document's scrollbar without clipping or a nested scroll region. Displayed
values match DSP and the conclusion panel remains unchanged.

**Out of scope:** Sphere conclusions, star desirability scores, production
predictions, non-notable star catalogs beyond the maximum, planet themes, or
precision beyond DSP's display, item caps, `+N more` summaries, pagination,
nested scrolling, or another panel.

## FEED-REJ-01: Re-evaluate sphere conclusions

**State:** Unauthorized; tabled pending further evaluation.

**Category:** bug-fix

**Urgency:** Medium

**Importance:** High

As a player comparing sphere candidates, I want sphere conclusions to express
demonstrated player value rather than common geometry or a misleading verdict
based on one measurement.

**Reason tabled:** `Tiny shell` classifies maximum shell radius alone and can
present an O star as an overall limitation despite its independent energy
value. `Many contained orbits` may be saturated across ordinary clusters, and
the current calculation may compare moon-centric `orbitRadius` values with a
star-centric shell radius. A threshold or wording hotfix cannot safely resolve
these separate correctness, prevalence, and utility questions.

**Evaluation required before authorization:** Verify the star-centric
containment calculation for planets and moons, measure both conclusion
distributions against a fixed reference-identity sample, and define the player
value conveyed by any retained sphere conclusion. Then decide whether to
correct, replace, or remove each conclusion before specifying implementation
and acceptance criteria.

**Out of scope while tabled:** Predicate, threshold, wording, cache-contract,
or presentation changes; sphere design advice; receiver performance;
composite scores; factual star statistics; or unrelated conclusions.

## FEED-REJ-02: Define useful theme statistics

**State:** Unauthorized; tabled pending a future theme-statistics initiative.

**Category:** feature-request

**Urgency:** Low

**Importance:** Medium

As a player looking for useful worlds, I want nearby mechanically relevant
planet themes identified with their locations and generated resources so I can
judge concrete opportunities rather than an arbitrary theme count.

**Reason tabled:** The source request singled out Aquatica, but one user's
preferred theme does not justify a product-owned special case. A general theme
inventory also provides weak value: theme desirability depends on mechanics
such as ocean type, construction area, wind, geothermal opportunity, and
generated resources, while several direct resource results already belong to
FEED-06, FEED-07, and FEED-08. Theme-proto possibilities cannot substitute
for veins confirmed on a particular planet.

**Evaluation required before authorization:** Define the player questions and
bounded mechanically relevant theme set; decide how many nearby bodies to show
and how distance affects selection; choose which preview facts accompany each
theme; define the exact per-planet node, group, and amount statistics admitted
after raw completion; establish overlap rules for sulfuric acid and rare-
resource results; and bound cached presentation size. Decide whether theme
statistics use a third `Themes` subcontainer, a selectable view, or another
dedicated surface. A third subcontainer is the smallest recommended option; an
independent overlay would reopen placement, collision, configuration, and
lifecycle work.

**Out of scope while tabled:** An Aquatica exception, theme counts or absence
lines, FEED-03 scaffold changes, theme selection, theme or vein presentation,
raw-scan retention, cache changes, aesthetic rankings, or implementation of a
new panel surface.

## Delivery relationships

- FEED-01 and FEED-02 are independent of the statistics-panel work and of each
  other; both must pass the pre-new-panel phase gate.
- FEED-03 supplies the panel lifecycle, home-system inventory, keyed cluster
  collection, titled subsections, and shared cluster-location value required by
  the later panel stories.
- FEED-04 follows FEED-03 and supplies one authoritative title value to both
  panels without coupling their renderers.
- FEED-05 first projects the home-system inventory; FEED-06 then joins raw ore
  evidence to those body rows.
- FEED-07, FEED-08, and FEED-09 independently reuse FEED-03's cluster
  primitives for their own bounded selections. FEED-07 excludes Unipolar
  Magnets; FEED-08 owns them exclusively.
- FEED-10 is the first consumer of FEED-03's titled cluster subsections and
  remains in the statistics panel's single scroll flow.
- The rejected stories have no delivery relationship.

Accepting one story does not implicitly accept another.
