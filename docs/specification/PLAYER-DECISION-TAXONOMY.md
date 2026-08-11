# Player Seed-Decision Taxonomy

**Status:** Accepted on 2026-08-11.

This taxonomy records why players inspect Dyson Sphere Program seeds and which
cluster characteristics they use when making those decisions. It establishes
player vocabulary and disputed preferences; it does not select product
criteria, thresholds, profiles, or scores.

The research treated community reports as evidence of player intent, not proof
of game behavior. Existing seed tools supplied candidate vocabulary only. The
accepted [runtime evidence feasibility matrix](RUNTIME-EVIDENCE-FEASIBILITY.md)
remains authoritative for what this project can reproduce.

## How to read the taxonomy

A useful context identifies a decision that could change before the run or
before committing to a system. Characteristics are candidates when players
connect them to that decision, even when their claimed benefit is disputed.

Four qualifiers recur across contexts:

- **Horizon:** convenience before interstellar travel can conflict with
  abundance or geometry valued hundreds of hours later.
- **Settings:** resource multiplier, infinite resources, Dark Fog difficulty,
  and cluster size can reverse or erase a preference.
- **System role:** the best starter, production hub, sphere system, and combat
  farm need not be the same system.
- **Disclosure preference:** some players want an optimized route; others value
  discovery and would rather know only whether a severe anomaly exists.

These qualifiers are inputs to later interpretation, not properties of a seed.

## Shared characteristic vocabulary

| Characteristic family | Player decision it may change | Common claimed benefit | Important qualification or disagreement |
| --- | --- | --- | --- |
| Starter topology | Accept or reroll the birth system; choose the first off-world expansion | Multiple satellites around one giant reduce early interplanetary travel; a useful inner planet can simplify early power or industry | Many players report that every normal starter is viable, so convenience is not necessity. |
| Starter resources and placement | Choose an early recipe, power route, or expansion order | Accessible basic resources, mineable fire ice, silicon, titanium, or oil can shorten early progression | Total abundance and physical proximity are different claims; exact placement requires the heavier raw-generation layer. |
| Giant type and products | Choose an early hydrogen, deuterium, or fire-ice route | An ice giant can expose fire ice; a gas giant can favor hydrogen/deuterium supply | Players explicitly disagree over which product is preferable, and orbital collectors arrive later than mineable deposits. |
| Planet rotation, tilt, wind, and solar conditions | Choose a power planet or factory orientation | Tidal locking and favorable energy ratios can simplify continuous renewable power | Benefit depends on the particular planet and progression stage; some players value low obliquity while others treat it as minor. |
| Stellar type, luminosity, and sphere geometry | Select a sphere or power-export system | Bright O-type or giant stars and planets inside a shell can increase sphere opportunity | One excellent sphere system may be enough; multiple high-luminosity stars can exceed practical needs or hardware limits. |
| Planet count, type, and layout | Select production, receiver, mining, or showcase systems | More suitable planets can concentrate roles and reduce interstellar logistics | More planets do not guarantee usable terrain; satellite-only layouts can conflict with desired sphere geometry. |
| Resource totals, vein structure, and rare resources | Select mining systems and decide how long a cluster supports a plan | High totals, many veins, and rare recipe shortcuts can support scale or reduce transport | Nearby deposits are convenient but distant systems can be richer; infinite resources changes totals into a much weaker signal. |
| Cluster distances and grouping | Choose compact expansion or distributed specialization | Nearby useful systems reduce travel and warper friction | Distance alone does not establish travel time, throughput, or a universally preferable network. |
| Dark Fog preview occupation | Choose combat settings, tolerate a start, or nominate a future farm system | More hive opportunity may support combat-focused play and eventual farming | The same density is danger for one player and opportunity for another; later bases, levels, attacks, and yield are play state, not seed guarantees. |
| Rare or visually distinctive combinations | Choose a themed, challenge, or showcase run | Unusual stars, planet themes, giant colors, or orbital arrangements make a run memorable | Theme and geometry are facts; beauty, novelty, and challenge value are subjective. |

Community discussions repeatedly combine these families, while disagreeing
about their importance. A 2021 discussion, for example, described nearby rare
resources as useful in midgame but distant black holes and neutron stars as
better for late-game resource abundance. A 2026 discussion weighed starter
fire ice, tidal locking, obliquity, O stars, and unipolar magnets differently
within the same proposed seed. These are tradeoffs to retain, not votes to
average into one ranking.

## Context families

### Fresh start

**Decision:** whether to accept the generated start and how to route progress
from landing through the first interstellar expansion.

Players commonly inspect:

- the birth planet's basic-resource layout, oil, terrain-related convenience,
  wind and solar conditions, and axial behavior;
- the number and arrangement of starter-system planets and satellites;
- early access to silicon, titanium, mineable fire ice, or useful giant
  products;
- tidal locking or other power opportunities on a reachable planet;
- the distance to the first useful expansion, rare resource, or high-energy
  star; and
- initial Dark Fog pressure when combat is enabled.

The claimed value is reduced travel, fewer awkward early production steps, or
a clearer power route. The counter-position is material: players report that
ordinary starts contain enough resources to reach interstellar travel and that
seed optimization is unnecessary for a casual or first run. Consequently,
“viable start” and “convenient start” must remain separate questions.

Important disagreements include fire-ice versus deuterium-oriented giants,
mineable fire ice versus later collection, and whether tidal locking is a major
advantage or a short-lived convenience. Resource and combat settings must
qualify every conclusion.

### Megafactory and long-horizon scale

**Decision:** which systems should host major production, mining, energy
generation, receivers, or supporting logistics for a long-running save.

Players commonly inspect:

- high-luminosity stars and giant-star sphere capacity;
- planets inside a selected or maximum shell, planet count, and useful planet
  types;
- cluster and per-system common-resource totals;
- unipolar magnets and other rare resources, including vein count as well as
  amount;
- distance from the birth system and grouping of complementary systems; and
- whether energy, production, mining, and Dark Fog farming should be separated
  into different systems.

The claimed value is sustained material supply, strong energy generation, and
less logistics friction at scale. The central tradeoff is concentration versus
distribution: a compact hub can simplify transport, while separating roles can
avoid congestion, combat interference, or performance pressure.

Player reports also dispute whether many O stars matter, whether one exceptional
sphere system is sufficient, and whether resource totals remain important once
Veins Utilization or infinite resources dominate. “Megafactory” therefore does
not imply one maximum-resource or maximum-luminosity rule.

### Dark Fog farming

**Decision:** whether and where to cultivate sustained Dark Fog combat for
exclusive drops without imposing unwanted risk on the rest of the run.

Seed-selection vocabulary includes initial and maximum hive opportunity,
system and planet topology, proximity to support industry, and isolation from a
primary sphere or factory. Players also discuss planetary base count and
placement, enemy level, replenishment, threat, and loot yield, but those latter
facts develop after the game starts and cannot be promised by a seed scanner.

The claimed value is a dedicated, supportable combat farm. The same evidence
has opposite interpretations: greater occupation can mean more opportunity to
a farmer and more danger or interruption to a builder. Community strategies
range from early home-system farming to a separate farm planet or system, while
other players eradicate or disable Dark Fog entirely. Turret choice, damage,
shielding, signal towers, combat settings, and player activity materially shape
the result.

This context must therefore present preview opportunity separately from a
complete raw scan, and both separately from after-start farm performance.
It cannot conclude expected yield, base layout, or attack timing from the seed.

## Materially distinct alternatives

These alternatives survive the scope test because each changes a decision, not
merely because a tool can measure it.

| Context | Distinct decision | Candidate characteristics | Boundary |
| --- | --- | --- | --- |
| Set-seed speedrunning | Select a route-compatible seed that minimizes completion time under a defined category and game version | Exact starter resource proximity, satellite layout, mineable fire ice, giant products, tidal locking, and early travel distance | Route and ruleset define value; no community-reported seed or threshold is permanent. |
| Scarce-resource or maximum-difficulty play | Decide whether a start supports the chosen constraint and where substitutes or combat rewards fit | Starter reserves and placement, renewable inputs, rare recipe shortcuts, Dark Fog pressure, and reachable expansion | Settings are primary inputs; characteristics praised for abundance may be irrelevant or inverted. |
| Compact or low-travel expansion | Decide whether useful roles can remain geographically close | Distances among birth, rare-resource, mining, sphere, and special-star systems; starter satellite topology | Geometric distance is supportable; travel time and logistics throughput require progression assumptions. |
| Sphere showcase or energy-focused run | Choose a visually or energetically attractive sphere system rather than a production optimum | Star type and luminosity, maximum shell geometry, planets inside the shell, orbital arrangement, and planet themes | Factual geometry can support the choice; attractiveness and the player's sphere design remain subjective. |
| Themed, novelty, or self-imposed challenge | Choose a rare arrangement because the arrangement itself defines the run | Unusual star/planet combinations, giant colors, tidal-lock combinations, moon-heavy systems, or deliberately restrictive clusters | The scanner may describe rarity or facts only when defensible; it must not declare them beautiful or fun. |
| Relaxed or discovery-first play | Decide whether to inspect the seed at all | At most, severe setting-sensitive anomalies or a factual summary the player elects to reveal | “Any seed is fine” is a legitimate preference; disclosure controls remain outside the approved presentation roadmap. |

Achievement-specific goals may later qualify as contexts if they create a
repeatable seed-selection decision. They are not admitted merely because an
achievement mentions a measurable quantity.

## Reported threshold language

Players use both relative language—“nearby,” “many,” “bright,” “enough”—and
hard examples. Published discussions have proposed, among other things, a
nearby O star around 3–10 light-years with about 2.4 luminosity, 30 million or
more unipolar magnets, multiple O stars, or a particular count of tidal-locked
planets. Other players in the same discussions reject the need for those
targets or prefer distant rare-resource systems for greater abundance.

SPEC-03 records those numbers only as examples of how players express intent.
They are not adopted thresholds because they vary with game version, cluster
size, resource settings, progression horizon, hardware limits, and individual
plans. SPEC-04 may determine whether a relative comparison, configurable bound,
or no conclusion is defensible for each characteristic.

## Prior-art vocabulary review

- [DSP-Seed-Finder](https://github.com/DoubleUTH/DSP-Seed-Finder) exposes
  composable rules for luminosity, planet and giant traits, tidal locking,
  resource amounts, sphere containment, themes, and galaxy-wide matches. This
  supports a vocabulary of configurable questions, not their importance.
- [dsp_search_seed](https://github.com/botany233/dsp_search_seed) combines
  nested system, planet, resource, geometry, and distance conditions and
  distinguishes faster from more precise evidence. It reinforces that one
  context may require several facts and collection costs.
- [DSPSeedScanner](https://github.com/Selsion/DSPSeedScanner) published lists
  for multi-moon starts, tidal locking, unipolar magnets, bright stars, O-star
  arrangements, and unusual giants. Those lists are candidate intents and
  validation leads, not product criteria.

## Community evidence register

The strongest sources used to establish player language and disagreement were:

- [What makes a seed good?](https://www.reddit.com/r/Dyson_Sphere_Program/comments/mk6ywl/)
  contrasted nearby midgame rares with distant late-game abundance and tied
  “endgame” to the player's intended scale.
- [What makes a good map seed?](https://www.reddit.com/r/Dyson_Sphere_Program/comments/tqsp2r/)
  proposed starter topology, tidal locking, mineable fire ice, and unipolar
  magnet quantities while also reporting that no played seed felt bad.
- [Which star system is the best to start with?](https://www.reddit.com/r/Dyson_Sphere_Program/comments/1o1nfbk/)
  discussed starter satellites, fire ice versus deuterium, nearby special
  stars, speedrun routing, and large-factory system selection.
- [Could this be one of the best seeds?](https://www.reddit.com/r/Dyson_Sphere_Program/comments/1ry89ae/)
  supplied recent examples involving starter obliquity, tidal locking, O stars,
  wind/solar ratios, rare resources, and differing time horizons.
- [PSA: Your starting seed is perfectly fine](https://www.reddit.com/r/Dyson_Sphere_Program/comments/194bic3/)
  supplied the strong contrary view that normal starts are viable and extreme
  optimization is unnecessary for most players.
- [I made a seed finder for this game](https://www.reddit.com/r/Dyson_Sphere_Program/comments/18xmbvk/)
  connected configurable search criteria to player requests and exposed
  disagreement about near versus distant desirable systems.
- [A guide to choosing seeds based on star types](https://www.reddit.com/r/Dyson_Sphere_Program/comments/lxdp5r/)
  showed how players associate star types with resources and sphere plans while
  acknowledging sampling limits.
- [What is the point of farming Dark Fog?](https://www.reddit.com/r/Dyson_Sphere_Program/comments/1buqhp9/)
  tied farming to exclusive materials and showed sharply different appetite
  for high combat and scarce-resource settings.
- [Everything I learned about Dark Fog mechanics](https://www.reddit.com/r/Dyson_Sphere_Program/comments/18mr5a7/)
  and [Dark Fog farming, how to?](https://www.reddit.com/r/Dyson_Sphere_Program/comments/1j7wxr2/)
  demonstrated that threat, growth, planetary occupation, farm layout, and
  sustained yield are heavily shaped after the run starts.
- [Advice on Dark Fog defenses](https://www.reddit.com/r/Dyson_Sphere_Program/comments/1s9le3c/)
  supplied the recent preference for separating a farm from sphere production
  and illustrated settings-sensitive strategy.
- [Exceptional starting seeds](https://steamcommunity.com/sharedfiles/filedetails/?id=2378423594)
  documented recurring community labels such as speedrun candidates, tidal
  locking, giant products, satellites, and multiple O stars.

These sources span multiple game versions and are not a representative poll.
Their agreement establishes vocabulary; their conflict establishes required
context and cautions against popularity-based scoring.

## SPEC-03 conclusion

Players do not judge a seed along one stable axis. They judge whether a cluster
supports a planned decision over a particular time horizon, under chosen
resource and combat settings, with different systems assigned different roles.

The taxonomy supports three primary context families—fresh start, megafactory,
and Dark Fog farming—and six materially distinct alternatives. It preserves
viability versus convenience, nearby convenience versus distant abundance,
concentration versus distribution, farming opportunity versus combat risk, and
optimization versus discovery as explicit conflicts.

SPEC-04 crossed these player decisions with accepted runtime evidence in the
[context-to-evidence decision matrix](DECISION-EVIDENCE-MATRIX.md). It did not
infer that every measurable characteristic was valuable, adopt the reported
thresholds above, or collapse contexts into a universal seed score.
