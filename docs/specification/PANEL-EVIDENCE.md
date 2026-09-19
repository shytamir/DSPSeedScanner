# Panel Evidence and Verification

[PROJECT.md](../PROJECT.md) owns steering and work status. This note records
read-only source evidence and repository verification cases for the panel
contracts. No game process, generation method, or Unity initialization was run.

## Native resource units

The inspected installed `Assembly-CSharp.dll` has SHA-256
`AE0BA95F75BD879A62AA4CE253B2AB78EAA4FB3C7C595F5E1FEE75EBE0E0EF85`, matching
the DSP `0.10.34.28529` reference in the conformance record. ILSpy 9.1.0.7988
read the assembly; decompiled output is an ignored inspection artifact.

- `UIPlanetDetail.OnPlanetDataSet` labels oil and unfiltered gas collection
  with `/s`. `UIPlanetDetail._OnUpdate` converts the planet's summed oil amount
  by multiplying it in double precision by `VeinData.oilSpeedMultiplier`,
  then casts to float for display. The native multiplier field is `4E-05f`.
  The unfiltered total uses no mining-speed multiplier. The separate mined
  resource filter applies player mining speed and is not a New Game total.
- `StringBuilderUtility.WritePositiveFloat` defaults to two decimal places
  and rounds the float scaled by 100 with `Mathf.RoundToInt`. Keep this numeric
  sequence, including float precision, rather than formatting ore units or
  treating oil as a finite deposit. Resource settings already affect generated
  amounts; do not multiply the displayed rate by the resource setting again.
- Gas item `1121` is Deuterium. The native writer displays its matching
  `PlanetData.gasSpeeds` element in `/s` with four decimal places. The adapter
  already normalizes that value into `CollectionRate` without a time conversion.
  Eligibility compares the unrounded rate against `0.15/s`.
- The complete scan retains per-planet group amounts but does not retain the
  oil multiplier in its group-only raw path. The rate presentation must receive
  the inspected native multiplier from extraction; it must not consult player
  history or a different installation. The wells/group count is independent.

Pure numeric cases: amounts `0`, `25000`, and `1000000` at the native multiplier
display `0.00/s`, `1.00/s`, and `40.00/s`; test float rounding around half-cent
rates and a different supplied multiplier. For Deuterium test `0.14999`,
`0.15`, and `0.15001` before display rounding. These are synthetic check inputs,
not observations from generated seeds.

## Planet and star attribution

- `PlanetGen.CreatePlanet` assigns `orbitAround` to the parent's primary
  `number`; it resolves that parent only among bodies whose `orbitAround` is
  zero. Moon numbers are not globally unique. Keep body ID as identity and
  sort moons by their local `orbitRadius` within each parent to derive the
  nearest-first ordinal, including the home moon. `GalaxyData.birthPlanetId`
  identifies the one home row. Existing topology checks provide the parent
  and sibling set; display names do not establish relationships.
- `PlanetData.UpdateRuntimePose` adds the parent's position to the moon's
  local orbital vector, then scales by 40,000 meters per AU. For complete
  orbit containment, use the primary radius, or the parent primary radius
  plus the moon radius, compared inclusively with the maximum sphere radius.
  This is the outer envelope of the native circular orbits, not the moon's
  small parent-centric radius or one instantaneous position. `sunDistance`
  alone is the parent's radius for a moon and omits its local orbit extent.
- `DysonSphere.Init` computes the default radius as
  `(float)((double)star.dysonRadius * 40000.0)`, doubles it, and rounds the
  maximum to 100-meter increments with `Mathf.Round` (ties to even). Use that
  sequence in the shared geometry calculation. `StarData.radius` remains
  stellar size in solar radii and is not the sphere radius.
- `UIStarDetail` uses `StarData.dysonLumino` with three decimal places.
  `StarData.typeString` classifies a `GiantStar` hotter than A as blue before
  main-sequence spectral classification. Keep a separate spectral-O flag for
  excluding O hosts from Aquatica rows, including an O-spectrum blue giant.
  Sphere-row grouping retains the existing exclusive blue/O display classes.
- The existing captured native catalogue (`DSPSeedScanner-Offline/LdbSnapshot`,
  25 themes, manifest 2026-08-04 and live manifest 2026-08-16) identifies the
  Aquatica/water-world theme as ID 16, invariant `Proto.Name` `Ocean 5`, display
  key `水世界`. Match the runtime catalogue's invariant name and resolve its
  actual ID rather than comparing a localized display string. `Name` and `ID`
  are fields declared on native `Proto`, which `ThemeProto` inherits; hosted
  compile declarations must preserve this declaring type. Catalogue data was
  read from that existing capture; no new catalogue export was run.
- Pairwise `NormalizedSystemDistance` already supplies home and brightest-star
  origins. Count a host once within the Aquatica group even if several planets
  match. Do not exclude that host because it appears in another display group.

Pure checks cover shuffled moons, repeated numbers under different parents,
no sibling, one/two siblings, exact radius equality, and a moon whose parent
orbit lies outside the sphere. A moon crossing the boundary must not count as
contained. Star cases cover luminosity 2.0, O-spectrum blue giants, an ordinary
brightest star, and differing home/brightest distance order. These inputs are
synthetic geometry/projection fixtures, not game-generated seed observations.
