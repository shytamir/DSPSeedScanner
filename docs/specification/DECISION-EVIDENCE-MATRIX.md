# Context-to-Evidence Decision Matrix

**Status:** Accepted on 2026-08-11.

This matrix crosses the accepted [player seed-decision
taxonomy](PLAYER-DECISION-TAXONOMY.md) with the accepted [runtime evidence
feasibility matrix](RUNTIME-EVIDENCE-FEASIBILITY.md). It identifies which
player questions have a trustworthy pre-play evidence path and which claims
the product must defer or decline.

SPEC-04 selects candidates for the next specification phase. It does not set
thresholds, define final conclusion wording, implement profiles, or assign a
global seed score.

## Decision rules

Each candidate receives one disposition:

| Disposition | Meaning |
| --- | --- |
| **Advance** | Player value and reproducible evidence intersect strongly enough for SPEC-05 to define a bounded conclusion. |
| **Further research** | The facts are reproducible and the player question is recognizable, but a predicate, comparison basis, or interpretation remains unsettled. |
| **Diagnostic only** | The fact is reproducible but does not independently change a supported player decision. It may explain or troubleshoot another conclusion. |
| **Reject** | The claim depends on after-start state, unsupported inference, subjective judgment, or context-free scoring. It cannot become a seed conclusion under the current contract. |

Confidence describes the intersection, not merely the runtime source:

- **High:** accepted evidence directly supports a documented player decision,
  with its direction and limits expressible without a hidden assumption.
- **Medium:** accepted evidence exists, but its value depends on an unsettled
  definition, comparison, or strong contextual qualifier.
- **Low:** the desired judgment is supported only by a proxy, subjective
  preference, or state that does not exist during seed selection.

An **Advance** disposition means eligible for SPEC-05. It does not mean that
the characteristic is always beneficial.

## Context keys

| Key | Context |
| --- | --- |
| FS | Fresh start |
| MF | Megafactory and long-horizon scale |
| DF | Dark Fog farming |
| SR | Set-seed speedrunning |
| SC | Scarce-resource or maximum-difficulty play |
| CX | Compact or low-travel expansion |
| DS | Sphere showcase or energy-focused run |
| TH | Themed, novelty, or self-imposed challenge |
| RD | Relaxed or discovery-first play |

## Immediate preview candidates

These candidates use facts already present after DSP creates the New Game
galaxy, or low-cost deterministic derivations over those facts.

| Candidate | Contexts and decision | Direction and material tradeoff | Evidence and settings sensitivity | Confidence | Disposition and defensible boundary |
| --- | --- | --- | --- | --- | --- |
| Birth-system topology | FS, SR, CX: accept the start and choose early expansion order | More useful bodies sharing a giant can reduce early flight; a star-orbiting inner planet may offer a different power role. Moon-heavy layouts can constrain other geometry. | Direct planet and moon topology; low cost. Theme catalogue and generation identity apply. | High | **Advance.** Describe the generated arrangement and its likely reduction in early travel. Do not claim a travel time or viable route. |
| Planet rotation and energy opportunity | FS, SR, DS, TH: choose an early power planet or desired arrangement | Tidal locking, solar ratio, wind strength, and obliquity can favor a chosen power strategy. Their importance varies by planet, technology, and player preference. | Direct preview fields and singularity flags; low cost. Theme and runtime-version sensitive. | High | **Advance.** State the factual opportunity and relevant planet. Do not label the whole seed superior or predict power output from an unspecified build. |
| Giant products and collection rates | FS, SR, SC: choose an early-to-midgame hydrogen, deuterium, or fire-ice route | Different products support different recipes; no giant type is universally preferred, and collection requires progression. | Direct gas items and rates; low cost. Resource settings alter rates and must accompany the result. | High | **Advance.** Report route-relevant products and rates conditionally. Do not equate a gas product with mineable starter deposits. |
| Star type and luminosity | MF, DS, CX, TH: nominate sphere, energy, or themed systems | Higher Dyson luminosity can favor sphere energy; one strong system may suffice, while many can exceed the run's needs or performance budget. | Direct star fields and runtime-derived Dyson luminosity; low cost. Cluster size and intended scale qualify comparisons. | High | **Advance.** Compare or identify energy candidates for a stated role. Do not count O stars as a context-free quality score. |
| Sphere radius and orbital containment | MF, DS, TH: choose a sphere or receiver-oriented system | Larger allowed shells and useful orbits inside a selected shell create design options. Satellite layouts and player shell choices can change the value. | Deterministic runtime geometry; low cost. The containment predicate and selected shell radius must be explicit. | High | **Advance.** Support factual shell-capacity and containment conclusions under a named predicate. Do not predict receiver effectiveness. |
| Planet and system composition | MF, CX, DS, TH: assign production, mining, sphere, or showcase roles | More bodies and desired themes provide role options, but do not guarantee buildable land or low logistics cost. | Direct planet types, themes, counts, and topology; low cost. Theme catalogue is part of generation identity. | High | **Advance.** Identify systems matching explicit factual role requirements. Do not translate body count into construction capacity. |
| Star-system distance and grouping | FS, MF, DF, SR, SC, CX: choose an expansion, mining, sphere, or farm system | Near systems reduce geometric separation; distant systems may contain richer generated resources. Convenience and abundance can conflict. | Derived from star positions and DSP's light-year constant; low to moderate cost. Cluster size and the chosen reference system matter. | High | **Advance.** State distance and relative compactness for named roles. Do not infer travel time, vessel throughput, or operational cost. |
| Initial and maximum Dark Fog hive opportunity | FS, MF, DF, SC: tolerate a start, isolate infrastructure, or nominate a farm system | More occupation is farming opportunity for one player and risk for another. Peace mode removes the decision; combat settings alter the interpretation. | Direct initial/max hive counts, orbit slots, safety factor, and complete combat inputs; low cost. | High | **Advance.** Describe generated occupation opportunity or exposure under the selected combat settings. Do not predict farm yield, survival, or future attacks. |
| Factual theme and unusual arrangement match | DS, TH, RD: select or avoid a known factual motif | A requested theme, star type, color field, orbital arrangement, or singularity can define the run. Novelty and beauty remain personal. | Direct preview fields; low cost. Rarity requires a separate population and sampling definition. | Medium | **Advance** for an explicit factual match. **Further research** for rarity claims. Never conclude that an arrangement is attractive or fun. |
| Generated names, algorithm IDs, and internal coefficients | None independently; troubleshoot identity or compatibility | These values help explain or reproduce results but do not establish player value. Names can be localized or overridden. | Direct preview; low cost. Highly version or localization sensitive. | High for fact, low for decision value | **Diagnostic only.** Retain for provenance and failures, not seed recommendations. |

## On-demand raw-generation candidates

These candidates require DSP to generate terrain or veins for one or more
isolated planets. They must sit behind the accepted explicit control and report
progress, cancellation, partial coverage, and failure by seed and stage.

| Candidate | Contexts and decision | Direction and material tradeoff | Evidence and settings sensitivity | Confidence | Disposition and defensible boundary |
| --- | --- | --- | --- | --- | --- |
| Exact starter resources and totals | FS, SR, SC: accept a start and choose an early recipe or expansion route | More of a constrained input can extend a route, but ordinary-start viability is a separate claim and infinite resources changes amount semantics. | Exact generated veins and oil wells plus normalized totals; high cost. Resource multiplier, infinite mode, theme catalogue, and game version apply. | High | **Advance.** Support exact presence and amount conclusions on demand. Do not claim the start is viable or optimal without a later explicit contract. |
| Exact resource proximity to the birth point | FS, SR, SC: assess early collection friction or a route-specific layout | Shorter geometric distance can reduce movement, but terrain, miner placement, and the route's chosen deposits can dominate. | Generated birth point and vein positions; high cost. A resource-selection and distance predicate is still required. | Medium | **Further research.** Exact distances are defensible; “accessible,” “convenient,” and route-time conclusions are not yet defined. |
| System and cluster resource totals | MF, SC: select mining regions and assess long-horizon supply | Greater finite totals can support scale; distance, vein count, Veins Utilization, and infinite resources can reduce or reverse importance. | Full raw generation and aggregation across included planets; high and scope-dependent cost. Resource settings are decisive. | High | **Advance.** Compare normalized totals only with complete declared coverage and identical relevant settings. Avoid lifetime or production-capacity predictions. |
| Rare-resource presence and amount | FS, MF, SR, SC, CX: select recipe shortcuts, mining systems, or compact expansion | Early proximity favors recipe access; distant special systems may favor abundance. Amount and closeness answer different questions. | Exact generated rare veins, totals, and star distances; high cost. Resource settings and cluster size qualify results. | High | **Advance.** Support presence, amount, and geometric-distance conclusions separately. Do not merge them into one rare-resource score. |
| Vein node and group structure | MF, SC, SR: distinguish total stock from extraction layout | More nodes or groups may create mining options, but miner capacity depends on building placement and chosen technology. | Exact `VeinData` and normalized groups; high cost. Infinite mode changes the importance of amount versus node count. | Medium | **Advance** as factual node/group evidence. **Reject** any direct conversion to miners supported or extraction throughput. |
| Generated land percentage | MF, DF, DS, TH: screen planets for a role or factual theme | More generated land may be useful for some roles, but one aggregate does not establish contiguous or buildable area. | Runtime terrain generation and water calculation; high cost. Theme and algorithm sensitive. | Medium | **Further research.** May become a screening fact after validation against role decisions; it cannot stand in for buildable area. |
| Terrain, biome, vegetation, and birth-point details | TH and narrow FS diagnostics: inspect a specific planet arrangement | The facts may explain a start or theme, but no general player decision survived without an explicit predicate. | Direct raw data; high cost and high compatibility sensitivity. | Medium for fact, low for decision value | **Diagnostic only.** Advance only if a later story defines a distinct reproducible decision. |

## Claims that do not survive the cross-check

| Requested judgment | Why players might want it | Evidence failure | Disposition |
| --- | --- | --- | --- |
| Exact buildable area, foundation burden, or factory capacity | Select a large production planet | No accepted pre-play scalar or placement predicate covers terrain, water, grid, buildings, and player layout. Land percentage is only a proxy. | **Reject.** Return unknown rather than substitute land percentage. |
| Mining accessibility, miners supported, or extraction throughput | Estimate production capacity | Depends on collision, terrain, building choice, placement, technology, power, and layout. | **Reject.** Preserve nodes, groups, and amounts as separate evidence. |
| Travel time, logistics throughput, or operational transport cost | Choose a compact cluster or hub | Depends on progression, vessels, warpers, power, station configuration, and traffic. | **Reject.** Geometric distance may be reported without the operational claim. |
| Ray-receiver coverage or realized sphere output | Select a receiver or energy system | Depends on sphere design, receiver location, atmosphere, lenses, research, and operation after start. | **Reject.** Preserve stellar and shell geometry only. |
| Planetary Dark Fog base count or placement | Choose a combat-farm planet | Created during game initialization and altered by relay arrivals and play. | **Reject** as seed-selection evidence. |
| Dark Fog level, replenishment, loot yield, threat, or attack timing | Predict farm productivity or danger | Evolves with combat settings, player activity, time, technology, and kills. | **Reject.** Initial/max hive opportunity is the strongest eligible pre-play boundary. |
| Visual attractiveness, fun, novelty, or challenge quality | Choose an emotionally appealing run | Theme and geometry are reproducible; the judgment is subjective. Rarity additionally needs a defined comparison population. | **Reject** the judgment. Permit explicit factual matches; research rarity separately. |
| Universal starter viability | Reassure a new player that a seed cannot fail | “Viable” requires a progression contract, resource-use assumptions, difficulty settings, and player behavior. Community confidence is not runtime proof. | **Reject** under the current evidence contract. SPEC-05 may offer bounded factual reassurance, not a guarantee. |
| Universal best seed or global weighted score | Reduce every preference to one ranking | Preference direction changes by context, settings, horizon, role, and disclosure choice; weights would be arbitrary. | **Reject.** Comparisons must name their context and decisive evidence. |

## Context coverage after the cross-check

This is the candidate product surface exposed by the intersection. “Immediate”
and “on demand” describe evidence availability, not panel design.

| Context | Immediate evidence that can change the decision | On-demand evidence that can change the decision | Required unknowns or caution |
| --- | --- | --- | --- |
| Fresh start | Birth topology, rotation and power opportunities, giant products, nearby-system geometry, initial Dark Fog exposure | Exact starter resources; potentially exact resource proximity after its predicate is defined | Do not equate convenience with viability or predict route time. |
| Megafactory | Stellar energy and sphere geometry, system composition, distances, Dark Fog separation opportunity | System/cluster resources, rare resources, vein structure; land percentage remains research-only | Do not infer buildable capacity, throughput, or hardware suitability. |
| Dark Fog farming | Hive opportunity, orbit data, system topology, distance from support or protected roles | No accepted raw planet fact currently predicts farm performance; resources may describe support only | Base placement, level, yield, threat, and timing remain unknown. |
| Set-seed speedrunning | Starter topology, power traits, giant products, early system distance | Exact resources and, after definition, resource proximity | Every conclusion must name route, category, game version, and settings. |
| Scarce-resource or maximum-difficulty | Topology, renewable-power opportunities, giant products, expansion distance, Dark Fog exposure | Exact resources, rares, totals, and vein structure | Settings dominate interpretation; normal-mode heuristics cannot be reused silently. |
| Compact or low-travel expansion | Distances, system grouping, starter topology, role-compatible systems | Rare and common resource locations | Compactness is geometric, not a throughput claim. |
| Sphere showcase or energy-focused | Star facts, shell geometry, containment, system composition, factual themes | Land percentage remains research-only | No receiver-performance or aesthetic conclusion. |
| Themed, novelty, or challenge | Explicit factual matches in preview evidence | Explicit terrain/resource matches only when deliberately requested | Subjective quality is unknown; rarity lacks a comparison-population contract. |
| Relaxed or discovery-first | No mandatory conclusion; low-cost facts can remain undisclosed | None by default | The option not to inspect is part of the decision context, not a product failure. |

## Interpretation constraints for SPEC-05

The cross-check establishes the following requirements for any conclusion
contract:

1. Every conclusion must name its product-defined context, relevant settings,
   time horizon, and system role. The player need not supply them for neutral
   evaluation.
2. A direction such as “more” or “closer” is allowed only when the context
   establishes why that direction helps and the result retains its tradeoff.
3. Comparative claims must use a product-owned reference: another system in the
   same complete cluster, a cluster-relative extremum, an accepted bounded
   preference interval, or a versioned reference population.
4. Immediate conclusions may depend only on complete preview evidence.
   On-demand conclusions must declare generated coverage and remain unknown
   when generation is cancelled, partial, or incompatible.
5. Facts with opposite interpretations must remain separate evidence, not
   cancel each other inside a hidden weight.
6. Unsupported outcomes must return unknown or an explicit declined claim;
   a proxy may not silently replace the requested evidence.
7. Discovery-first play may limit disclosure, but it does not alter the neutral
   evaluation or its outcome.

These constraints favor small, attributable conclusion sets over broad
profiles. SPEC-05 remains responsible for selecting the initial set and
defining its exact semantics.

## Prior-art assessment

The configurable rules in
[DSP-Seed-Finder](https://github.com/DoubleUTH/DSP-Seed-Finder) and
[dsp_search_seed](https://github.com/botany233/dsp_search_seed) map well to
atomic evidence predicates and bounded comparisons. Their breadth does not
establish that every rule belongs in the first product contract. Revised
SPEC-05 made the adopted predicates and ranges product-owned rather than
requiring player-supplied queries.

[DSPSeedDatabase](https://github.com/SuperB3333/DSPSeedDatabase) reinforces the
useful separation between stored evidence, queries, and optional evaluation.
This project retains that separation but rejects a universal weighted score:
the accepted player taxonomy shows that preference direction changes with
context, settings, horizon, and system role.

## SPEC-04 conclusion

The accepted runtime can support meaningful pre-play decisions without
pretending to predict the run. Nine preview candidate families advance toward
SPEC-05, four raw-evidence families advance wholly or in bounded part, three
raw families remain research or diagnostic only, and nine requested judgments
are explicitly rejected.

The strongest initial surface is deliberately asymmetric:

- immediate evidence can explain structure, power opportunity, stellar and
  sphere geometry, distance, factual themes, and initial Dark Fog opportunity;
- a complete raw scan can add exact resource presence, amount, distribution,
  and declared coverage; and
- after-start performance, subjective quality, universal viability, and global
  quality scores remain outside the product's truthful boundary.

SPEC-05 used this accepted boundary to define the revised candidate [seed
conclusion contract](CONCLUSION-CONTRACT.md), including automatic context
evaluation and bounded preference invariance. No final presentation wording,
threshold, profile, or implementation priority was selected in SPEC-04.
