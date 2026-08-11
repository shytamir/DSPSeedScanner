# SPEC-01 Evidence and Experiment Record

**Status:** Completed investigation record; findings pending acceptance.

## Question

The investigation asked which inputs had to accompany a DSP seed so the
installed runtime could reproduce the same pre-play galaxy evidence.

## Examined environment

- Date: 2026-08-11
- DSP: `0.10.34.28529`
- Steam build: `23109513`
- Unity: `2022.3.62.1451004`
- BepInEx: `5.4.17.0`
- Galaxy algorithm: `20200403`
- Theme count: 25
- `Assembly-CSharp.dll` SHA-256:
  `AE0BA95F75BD879A62AA4CE253B2AB78EAA4FB3C7C595F5E1FEE75EBE0E0EF85`

The probe ran from an isolated BepInEx root. It referenced the installed game
and data directories read-only and did not load the user's installed plugins.
No game save was created or modified.

## Assembly evidence

Focused metadata and IL inspection established the following call-path facts:

- `GameDesc.SetForNewGame` recorded galaxy algorithm, seed, star count, player
  prototype, resource multiplier, creation version, ordered theme IDs, and
  default combat settings.
- `UniverseGen.CreateGalaxy` read galaxy algorithm, seed, star count, the rare
  resource mode, and passed `GameDesc` into star and planet creation.
- `StarGen.CreateStarPlanets` consumed `savedThemeIds` and contained a
  creation-build compatibility branch.
- `StarGen.CreateBirthStar` and `StarGen.CreateStar` consumed Dark Fog maximum
  density and initial colonization to derive per-star hive counts.
- `PlanetAlgorithm.GenerateVeins` and algorithms 7, 11, 12, and 13 consumed
  `resourceMultiplier` during the later raw-resource stage.
- `EnemyDFHiveSystem.SetForNewGame` and `SetForNewCreate` consumed initial
  growth and initial level after the galaxy snapshot was constructed.
- `GameDesc.seedKey64` clamped and rounded several fields, while the cluster
  strings omitted the runtime and theme catalogue.

This inspection selected the controlled variables and distinguished direct
runtime evidence from later gameplay state.

## Runtime method

The probe generated a 64-star baseline for seed `12345678` twice, then changed
one input per case. It serialized stable star and planet fields in fixed order
and computed:

- a **core hash** for galaxy, star, planet, orbit, theme, and birth-system
  evidence;
- a **preview hash** that additionally included hive counts and gas rates;
- aggregate initial and maximum hive counts;
- a separate gas-rate hash.

The complete matrix was executed in two independent DSP processes. Both
output files were byte-identical with SHA-256
`C55CE1754C396C873A5B10C4EF70C69FEBA9EE8CE8ED1F4A1900F7892E5DB76E`.
The raw diagnostic files and probe binaries were kept outside the repository.

## Results

| Controlled case | Core | Preview | Observable result |
| --- | --- | --- | --- |
| Baseline repeated | Same | Same | Both calls and both game processes matched exactly. |
| Seed `12345678` to `12345679` | Changed | Changed | Galaxy and preview evidence changed. |
| Requested stars 64 to 32 | Changed | Changed | The generated cluster contained 32 stars instead of 64. |
| Resources `1` to `0.5` | Same | Same | No pre-play structural or gas-rate change for this seed. |
| Resources `1` to rare `0.1` | Same | Changed | Gas-rate hash changed; structure remained equal. |
| Resources `1` to infinite `100` | Same | Same | No pre-play structural or gas-rate change for this seed. |
| Peace to combat, defaults retained | Same | Same | Mode alone did not change the snapshot. |
| Dark Fog maximum density `1` to `3` | Same | Changed | Initial hives changed 37 to 80; maximum hives 66 to 198. |
| Initial colonization `1` to `2` | Same | Changed | Initial hives changed 37 to 50; maximum remained 66. |
| Initial enemy level `0` to `10` | Same | Same | The value was not consumed by preview generation. |
| Sandbox off to on | Same | Same | The pre-play galaxy snapshot did not change. |
| Theme ID order reversed | Changed | Changed | Ordered catalogue identity affected generation. |

The baseline core hash was
`ACB8028F4BFEC23F7C0B98712B61526EF574C2C269FF720A099FCD3294C2BCF5`;
the baseline preview hash was
`5CA8F4264675E8F31415FAC6F29779A92C5860B4A1C372DF367D2F41553BCA05`.

## Interpretation limits

- Hash equality covered the selected pre-play fields, not every internal or
  presentation field.
- Single-variable cases used one representative seed. Assembly call paths
  supported the classifications, but the experiment was not a statistical
  survey.
- Ordinary resource multipliers were not declared irrelevant: their effect
  occurs during later vein realization even when preview hashes match.
- Initial enemy level and growth were not declared irrelevant: they are used
  when actual enemy systems are initialized after galaxy generation.
- Cross-machine and cross-build equivalence was not tested.
- No prior-art generator output was used as authoritative evidence.

## Outcome

The investigation produced the candidate
[generation identity contract](GENERATION-IDENTITY.md). The story reached its
defined evidence deliverables, but neither the contract nor any downstream
product behavior was accepted by completing the investigation.
