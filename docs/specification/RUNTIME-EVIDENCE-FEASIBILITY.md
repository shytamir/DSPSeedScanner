# Runtime Evidence Feasibility Matrix

**Status:** Candidate matrix produced by SPEC-02; pending product acceptance.

This matrix records what the installed Dyson Sphere Program runtime can
faithfully provide before a player starts a game. It establishes evidence
availability, not whether a characteristic is valuable or preferable. Those
decisions remain outside SPEC-02.

The examined runtime was DSP `0.10.34.28529` with galaxy algorithm `20200403`.
All supported facts inherit the accepted
[generation identity contract](GENERATION-IDENTITY.md).

## Classification and cost

| Class | Meaning |
| --- | --- |
| Direct-preview | DSP populated the fact in `GalaxyData` or its stars and planets during `UniverseGen.CreateGalaxy`. |
| Direct-raw | DSP populated the fact only after its raw planet algorithm generated terrain, vegetation, and veins. This can be requested before play but is not already present in the New Game galaxy. |
| Derived | A deterministic calculation over supported runtime facts; its formula and units must be versioned. |
| After-start | DSP created or changed the fact only while initializing or playing a game. It is not seed-selection evidence. |
| Unsupported | The available source was incomplete, subjective, dependent on player actions, or not yet defensible as a reproducible fact. |

Costs describe incremental collection after the galaxy exists:

- **Low:** direct field extraction or a small linear aggregation.
- **Moderate:** cluster-wide comparisons or calculations that should still be
  cheap relative to raw planet generation.
- **High:** explicit planet modeling with significant allocation and CPU work.
- **Ineligible:** requires starting or observing a game rather than inspecting
  a candidate cluster.

## Galaxy, system, and star evidence

| Candidate fact | Class | Runtime source | Availability and cost | Dependencies and limits |
| --- | --- | --- | --- | --- |
| Seed and actual generated star count | Direct-preview | `GalaxyData.seed`, `GalaxyData.starCount` | After galaxy creation; low | Record requested star count separately. |
| Birth star and birth planet | Direct-preview | `GalaxyData.birthStarId`, `birthPlanetId` | After galaxy creation; low | Theme catalogue and generation version sensitive. |
| Star type and spectral class | Direct-preview | `StarData.type`, `spectr` | After galaxy creation; low | Enum names are compatibility-sensitive; normalized values must preserve unknown members. |
| Stellar mass, lifetime, age, temperature, luminosity, color, and radius | Direct-preview | `StarData` scalar fields | After galaxy creation; low | Runtime units and formulas must not be silently reinterpreted across versions. |
| Dyson luminosity | Derived | `StarData.dysonLumino` property | After galaxy creation; low | Runtime-derived and version-sensitive; distinct from raw stellar luminosity. |
| Habitable and light-balance radii | Direct-preview | `habitableRadius`, `lightBalanceRadius` | After galaxy creation; low | Physical meaning is DSP-specific; no player-value conclusion is implied. |
| System radius and physical/view radii | Derived | `StarData.systemRadius`, `physicsRadius`, `viewRadius` | After galaxy creation; low | Useful geometry, not a guarantee of usable construction space. |
| Planet and asteroid-belt counts and radii | Direct-preview | `planetCount`, asteroid-belt fields, `planets` | After galaxy creation; low | A missing belt is represented by the runtime's generated values, not inferred from visuals. |
| Star position | Direct-preview | `StarData.position`, `uPosition` | After galaxy creation; low | Preserve source units; `GalaxyData.LY` is the runtime conversion constant. |
| Star-to-star and birth-to-star distance | Derived | Star positions and `GalaxyData.LY` | After galaxy creation; low to moderate | Geometric distance only; travel time requires progression and propulsion assumptions. |
| Generated star graph connections | Direct-preview | `GalaxyData.graphNodes`, `StarGraphNode.conns` and `lines` | After galaxy creation; low | The graph is a runtime visualization/navigation structure, not a logistics-route guarantee. |
| Internal resource coefficient | Direct-preview | `StarData.resourceCoef` | After galaxy creation; low | Diagnostic input to resource generation; it must not be presented as an exact resource total. |
| Generated star and planet names | Direct-preview | `name` and display-name properties | After galaxy creation; low | Localization, name-generation changes, and player overrides make names unsuitable as identity or ranking evidence. |

## Planet and orbit evidence

| Candidate fact | Class | Runtime source | Availability and cost | Dependencies and limits |
| --- | --- | --- | --- | --- |
| Planet type and theme | Direct-preview | `PlanetData.type`, `theme`; `LDB.themes` | After galaxy creation; low | Theme ID must be interpreted through the matching runtime catalogue. |
| Planet algorithm and style | Direct-preview | `PlanetData.algoId`, `style` | After galaxy creation; low | Primarily compatibility and diagnostic evidence. |
| Planet and moon topology | Direct-preview | `orbitAround`, `orbitAroundPlanet`, `orbitIndex`, `number` | After galaxy creation; low | Moon and satellite counts are deterministic aggregations over these fields. |
| Orbital radius, inclination, longitude, period, and phase | Direct-preview | Planet orbit fields | After galaxy creation; low | These are generated initial orbital parameters, not travel-time estimates. |
| Rotation period and phase, axial tilt, and obliquity | Direct-preview | Planet rotation fields | After galaxy creation; low | Preserve DSP units and sign conventions. |
| Tidal locking, resonances, horizontal rotation, reverse rotation, and multiple satellites | Direct-preview | `PlanetData.singularity` flags | After galaxy creation; low | Flags may be combined; consumers must not treat the enum as mutually exclusive. |
| Planet radius and star distance | Direct-preview | `radius`, `realRadius`, `sunDistance`, `orbitRadius` | After galaxy creation; low | Star-centric and moon-centric distances must not be conflated. |
| Solar ratio, wind strength, ionosphere height, water item, and ice flag | Direct-preview | Planet fields populated from its theme | After galaxy creation; low | These are runtime values; their usefulness and thresholds remain for later stories. |
| Gas giant products and collection rates | Direct-preview | `gasItems`, `gasSpeeds`, `gasHeatValues`, `gasTotalHeat` | After galaxy creation; low | Resource setting sensitive; rare-resource mode changed rates in SPEC-01. |
| Exact terrain heights, biome, and temperature samples | Direct-raw | `PlanetRawData.heightData`, `biomoData`, `temprData` | Explicit solid-planet generation; high | Algorithm, theme, seed, runtime data, and floating-point behavior sensitive. Gas giants do not use the same raw path. |
| Land percentage | Direct-raw | `PlanetAlgorithm.CalcWaterPercent`, `PlanetData.landPercent` | After raw terrain generation; high | A theme or preview estimate must not substitute for the generated value. |
| Birth and initial resource points | Direct-raw | `PlanetData.GenBirthPoints` and birth-point fields | After raw generation; high | Defined for the birth planet; not a general landing-quality conclusion. |
| Vegetation instances | Direct-raw | `PlanetRawData.vegePool` | After raw generation; high | Available but currently only diagnostic; asset catalogue changes carry high compatibility risk. |
| Exact buildable area or foundation cost | Unsupported | No single authoritative pre-play scalar | Not established | Requires a defined placement predicate over terrain, water, grid, and building rules. Defining that metric belongs to later specification work. |
| Visual attractiveness | Unsupported | Theme and rendering assets are only proxies | Not objectively reproducible | Theme identity is supported; an aesthetic judgment is not a runtime fact. |

## Resource evidence

| Candidate fact | Class | Runtime source | Availability and cost | Dependencies and limits |
| --- | --- | --- | --- | --- |
| Theme-declared common and possible rare resources | Direct-preview | `ThemeProto.VeinSpot`, `RareVeins`, and related settings | After galaxy creation; low | A possibility or generation weight is not proof that a deposit exists on a specific planet. |
| Exact vein node type, position, group, and product | Direct-raw | `PlanetRawData.veinPool`, `VeinData` | After full raw generation; high | Must use the runtime algorithm selected by `PlanetData.algoId`. |
| Exact vein group type, center, node count, and amount | Direct-raw | `PlanetData.SummarizeVeinGroups`, `VeinGroup` | After full raw generation; high | Group center is a normalized aggregate of generated node positions. |
| Planet totals by resource | Derived | Aggregation of generated `VeinData` or `VeinGroup` | After raw generation; high overall | Preserve oil and infinite-resource semantics instead of treating every amount as an ordinary finite ore count. |
| System and cluster totals by resource | Derived | Aggregation across raw-generated planets | After every included solid planet is generated; high | Work must be bounded, cancellable, and explicit about partially generated clusters. |
| Rare-resource presence, node count, and amount | Derived | Exact generated veins filtered by rare `EVeinType` members | After raw generation; high | Theme-declared rare resources alone are insufficient. |
| Oil wells and generated yield | Direct-raw | Oil `VeinData`, amounts, and oil-specific multiplier behavior | After raw generation; high | Oil uses different amount semantics and must remain a separate normalized resource kind. |
| Resource distance from the birth point | Derived | Generated vein positions and generated birth point | After raw generation; high | The distance is geometric; accessibility and mining convenience require additional definitions. |
| Mining accessibility or number of miners supported | Unsupported | No stable seed-only scalar | Not established | Depends on collision, terrain, building placement, chosen miner, and player layout. |

## Distance and Dyson sphere geometry

| Candidate fact | Class | Runtime source | Availability and cost | Dependencies and limits |
| --- | --- | --- | --- | --- |
| Default Dyson sphere radius | Derived | `StarData.dysonRadius` and the rounding used by `DysonSphere.Init` | After galaxy creation; low | Formula and units are version-sensitive. |
| Minimum and maximum allowed sphere radii | Derived | Stellar physical radius, type, `dysonRadius`, and `DysonSphere.Init` | After galaxy creation; low | Use the runtime formula, including rounding and giant-star behavior. |
| Whether a planet's orbit lies inside a specified shell | Derived | Planet orbital radius and selected shell radius | After galaxy creation; low | The predicate must say whether it compares orbit center, entire planet, or another margin. |
| Number of planets inside the maximum shell | Derived | Maximum radius plus planet orbital geometry | After galaxy creation; low | Valid only under an explicit containment predicate; not itself a recommendation. |
| Ray-receiver coverage or effectiveness | Unsupported as a seed fact | Requires sphere design, receiver position, lenses, atmosphere, and gameplay state | Not available as one pre-play fact | Geometry inputs are supported, but effectiveness is conditional behavior. |
| Travel time or logistics throughput | Unsupported as a seed fact | Requires technology, vessel, warper, power, and route assumptions | Not available as one pre-play fact | Geometric distance remains supported input for later contextual derivation. |

## Dark Fog evidence

| Candidate fact | Class | Runtime source | Availability and cost | Dependencies and limits |
| --- | --- | --- | --- | --- |
| Per-star initial and maximum hive counts | Direct-preview | `StarData.initialHiveCount`, `maxHiveCount` | After galaxy creation; low | Depends on initial colonization and maximum density as established by SPEC-01. |
| Hive orbit slots and patterns | Direct-preview | `hiveAstroOrbits`, `hivePatternLevel` | After galaxy creation; low | Represents generated opportunity and orbit data, not proof of later survival or development. |
| Stellar safety factor and epic-hive flag | Direct-preview or derived | `safetyFactor`, `epicHive` | After galaxy creation; low | DSP-specific internal semantics require versioned naming and must not be oversold as player safety. |
| Combat configuration | Direct input | `GameDesc.isPeaceMode`, `combatSettings` | Available before generation; low | Context-setting evidence, not a property caused by the seed. |
| Actual initialized hive level and growth state | After-start | `EnemyDFHiveSystem.SetForNewGame` and `SetForNewCreate` | Requires game initialization; ineligible for New Game evidence | Depends on initial level and growth settings in addition to preview fields. |
| Planetary base locations and counts | After-start | Ground enemy systems and relay arrival logic | Requires initialized or evolving game state; ineligible | Not guaranteed by the New Game galaxy snapshot. |
| Future expansion, threat, or attack timing | After-start | Dark Fog tick logic | Evolving play; ineligible | Depends on player power, combat, time, and the complete combat configuration. |
| Drop rates and farm yield | After-start | Enemy level, loot tables, combat settings, and player kill rate | Evolving play; ineligible | Dark Fog-exclusive resources may be contextual goals, but yield is not a static seed fact. |

## Runtime probe evidence

An isolated BepInEx probe demonstrated that the planned mod can invoke the
authoritative raw-generation path without creating or loading a save. For seed
`12345678`, the birth planet was planet `103`, theme `1`, algorithm `1`.

Across two independent DSP processes:

- terrain, vein positions, vein counts, vein groups, amounts, and land
  percentage repeated exactly;
- the planet contained 581 vein nodes in 53 groups;
- full raw generation took approximately 100–140 ms per run on the examined
  machine after runtime initialization;
- changing resource multiplier left terrain, vein positions, 581 nodes, and
  53 groups unchanged while changing amounts;
- total generated amounts were 20,159,270 at `1`, 10,718,605 at `0.5`,
  2,527,094 at rare `0.1`, and 562,001,277,953 under infinite `100` semantics.

This was a representative feasibility and repeatability probe, not a benchmark
or exhaustive test of all planet algorithms. Raw diagnostic files and probe
binaries remained outside the repository.

## Mod-support conclusion

The accepted generation identity is implementable by the intended BepInEx
mod. The mod can read preview evidence directly from the same `GameDesc`,
`GalaxyData`, `StarData`, `PlanetData`, and `LDB` objects used by DSP. It can
also request exact raw evidence through DSP's selected `PlanetAlgorithm` before
play begins.

That support has constraints:

- raw generation mutates and allocates planet data and must operate on isolated
  candidate objects rather than the player's active factory or save;
- DSP exposes generation-wide static state, including the active `GameDesc`,
  so scans must be serialized or otherwise isolated and must restore state on
  failure or cancellation;
- raw per-planet evidence is materially more expensive than preview evidence
  and cannot be assumed suitable for every seed without later orchestration and
  performance work;
- internal member and algorithm changes require explicit compatibility
  failure, not partial results.

These are implementation constraints, not blockers to the accepted contract.

## SPEC-02 conclusion

The faithfully reproducible pre-play evidence boundary contains:

1. direct galaxy, star, planet, orbit, gas, sphere-input, and Dark Fog preview
   fields;
2. deterministic geometry and aggregation derived from those fields;
3. exact terrain and resource evidence when DSP's raw planet generation is
   explicitly requested and its higher cost is accepted.

Actual initialized or evolving Dark Fog state, travel and logistics outcomes,
ray-receiver performance, mining-layout capacity, and subjective judgments are
not pre-play seed facts. They must remain conditional, after-start, or unknown.

No fact was selected for product presentation or assigned player value in this
story.
