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
