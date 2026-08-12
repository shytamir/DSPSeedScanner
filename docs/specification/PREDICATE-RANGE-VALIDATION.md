# Predicate, Range, and Validation Catalogue

**Status:** Accepted on 2026-08-11.

**Definition version:** `0.1.0`

**Compatible conclusion contract:**
[Seed Conclusion Contract](CONCLUSION-CONTRACT.md) `0.1.0`

This catalogue supplies the first product-owned predicates, bounded preference
ranges, and runtime-confirmed validation cases for the conclusion contract. It
defines evaluable questions; it does not rank seeds, prescribe presentation
copy, or authorize implementation.

## Decision rules

For an increasing metric with threshold range `[L, U]`:

- `x >= U` is **supports**;
- `x < L` is **does not support** or **caution**, as declared by the family;
- `L <= x < U` is **preference-sensitive**.

For a decreasing metric:

- `x <= L` is **supports**;
- `x > U` is **does not support** or **caution**;
- `L < x <= U` is **preference-sensitive**.

Both endpoints are valid optional preference values. Selecting either endpoint
or any value between them may explain a preference-sensitive result but cannot
change the stored neutral outcome. Fixed predicates use a singleton range and
therefore cannot be preference-sensitive.

Families emit component conclusions instead of averaging unrelated metrics.
Two invariant, materially conflicting component conclusions produce a visible
**tradeoff**. Missing range, compatibility, or required coverage produces
**unknown** only for the dependent component. Peace mode makes Dark Fog
occupation **not applicable**.

## Evidence basis and limitations

The reference runtime was DSP `0.10.34.28529`, galaxy algorithm `20200403`, 64
stars, resource multiplier `1`, and the combat settings created by
`GameDesc.SetForNewGame` with combat enabled. The theme catalogue and remaining
identity fields are those recorded by the accepted
[generation-identity contract](GENERATION-IDENTITY.md).

The preview reference cohort contained 512 deterministic seeds:

```text
seed(i) = (45772 + i * 982451653) mod 100000000, i = 0..511
```

The raw starter cohort was the first 96 seeds from the same sequence. Range
endpoints are nearest-rank positions 128 and 384 for the preview cohort and 24
and 72 for the starter cohort: the observed first and third quartiles. The
middle half is intentionally preference-sensitive; the outer quarters provide
counterexamples on both sides. These are versioned reference-cohort bands, not
claims about the distribution of all 100 million seeds or other settings.

Preview evidence for all 512 seeds and raw starter evidence for all 96 seeds
matched in independent runs, excluding raw-generation elapsed time. The
preview run's evidence file SHA-256 was
`BEC6EA7094BB53D9AFA2DE11AD8124B6C41DF09E8B81A7AD258FEB784AA83A73`.
The repeated starter run's file SHA-256, including its non-contract timing
column and added named cases, was
`6F5CF7853D474E1ABC5F690A095F48584EECC2052A5D7E23D75A92ACE7DEB00F`.

Eight deliberately selected clusters were generated fully to challenge raw
resource and rare-access behavior: `45772`, `73339583`, `96178012`,
`48823053`, `16315224`, `12074390`, `82506644`, and `61571387`. That catalogue
is not a random reference population and therefore does not establish
cluster-scale amount ranges. Raw probe output remains outside the repository,
as required by the project rules.

Runtime probing also established two implementation prerequisites:

- call `RandomTable.Init()` before isolated raw planet generation; without it,
  at least one installed terrain algorithm failed;
- compute the runtime maximum shell radius as
  `round(star.dysonRadius * 40000 * 2 / 100) * 100`, and compute interstellar
  distance from `StarData.uPosition / GalaxyData.LY`.

These details are compatibility evidence, not permission to replace DSP's
generation routines.

## Structural and derived predicate registry

| ID | Predicate and subject | Outcome | Required evidence and boundary |
| --- | --- | --- | --- |
| `FS-TOPOLOGY.shared-satellites` | Count solid planets sharing the birth planet's giant parent, including the birth planet | At least 2 **supports** concentrated early expansion; 1 **does not support** that arrangement | Complete preview birth-system topology. It does not predict travel time or buildable area. |
| `FS-POWER.birth-tidal` | Any solid planet in the birth system has DSP's tidal-lock singularity | Present **supports** a continuous-daylight opportunity; absent **does not support** that specific opportunity | Complete preview rotation data. It does not predict realized power. |
| `FS-GAS-ROUTE.product` | Birth-system giant contains each registered runtime product ID | Present **supports** that product opportunity; absent **does not support** it | Evaluate hydrogen, deuterium, and fire ice automatically and separately. Collection prerequisites remain explicit. Rates have no accepted range. |
| `FS-RESOURCES.fire-ice` | Complete birth-system raw generation contains fire-ice veins | Present **supports** local deposit access; absent **does not support** it | Exact raw coverage. Presence is separate from abundance. |
| `MF-SPHERE-GEOMETRY.containment` | Number of planetary orbit radii no greater than the system's derived maximum shell radius | Uses adjustable count range `[1, 2]`: 2 or more **supports**, 0 **does not support**, and 1 is **preference-sensitive** | Complete preview orbits and versioned radius derivation. It does not imply receiver effectiveness. |
| `MF-SYSTEM-ROLE.role` | A system satisfies an upstream accepted component predicate | The upstream outcome is preserved for `strong-energy`, `large-shell`, `orbit-containment`, or `rare-access`; the birth system is the fixed `starter-anchor` | A role adds no threshold and cannot upgrade sensitive or unknown evidence. Multiple roles remain separate. |

The containment count uses an interval because player research distinguished
one contained orbit from multiple contained orbits. It is not aggregated with
maximum shell radius.

## Quantitative range registry

All increasing ranges use the increasing rule above unless stated otherwise.

| ID | Metric | Accepted range | Scope and settings |
| --- | --- | --- | --- |
| `FS-POWER.solar` | Maximum solid-planet solar ratio in the birth system | `[1.16, 1.35]` | Preview; reference identity only |
| `FS-POWER.wind` | Maximum solid-planet wind ratio in the birth system | `[1.0, 1.5]` | Preview; reference identity only |
| `FS-RESOURCES.common-total` | Sum of finite common deposits (iron, copper, silicon, titanium, stone, and coal) in the birth system | `[74,788,292, 105,667,431]` | Complete raw birth system; resource multiplier `1` only; oil flow remains a separate component |
| `MF-ENERGY-SYSTEM.output` | Highest Dyson luminosity in the cluster | `[2.4489998817, 2.4900000095]` | Complete preview cluster; reference identity only |
| `MF-ENERGY-SYSTEM.separation` | Highest Dyson luminosity divided by the second highest | `[1.1104599329, 1.2183275480]` | Complete preview cluster; reference identity only |
| `MF-SPHERE-GEOMETRY.radius` | Largest derived maximum shell radius in the cluster | `[76,200, 191,400]` radius units | Complete preview cluster; reference identity only |
| `CX-GROUPING.distance` | Distance between the starter anchor and, or between, independently supported roles | `[2.5, 10]` light-years, decreasing | Complete role evidence; all role subjects must be known |
| `RR-ACCESS.distance` | Birth-system distance to a system containing a specified rare resource | `[2.5, 10]` light-years, decreasing | Complete cluster raw coverage for that resource; evaluate each resource separately |

The compact-distance range is research-anchored rather than cohort-derived.
Published seed searches repeatedly treated `2.5` light-years as an exceptional
nearby target, while player research used broader nearby-system language. The
contract adopts `10` light-years as a deliberately permissive upper bound to
make disagreement visible. It must be revisited if implementation evidence
shows the band does not discriminate useful choices.

### Starter resource component ranges

`FS-RESOURCES` also emits separate amount and vein-group conclusions for each
common resource. Values are runtime vein amount units; oil must not be
described as ore. No component cancels another.

| Resource | Amount range | Vein-group range |
| --- | ---: | ---: |
| Iron | `[9,151,265, 26,773,650]` | `[16, 27]` |
| Copper | `[12,078,923, 29,497,621]` | `[18, 28]` |
| Silicon | `[3,355,497, 12,453,357]` | `[4, 11]` |
| Titanium | `[11,403,989, 21,808,706]` | `[8, 15]` |
| Stone | `[8,939,801, 20,925,618]` | `[14, 21]` |
| Coal | `[9,495,641, 10,938,129]` | `[13, 15]` |
| Oil | `[1,196,959, 1,304,446]` | `[17, 19]` |

These ranges apply only to the reference identity and multiplier `1`. SPEC-02
confirmed for a controlled seed that resource multipliers changed amounts but
not vein positions, nodes, or groups. That single-seed result is insufficient
to publish cross-setting ranges, so other multipliers remain **unknown** in
this version rather than being silently rescaled.

## Deferred strength claims

The following accepted conclusion families remain available but resolve the
listed component to **unknown** under this definition version:

| Family or component | Reason for deferral | Facts still eligible |
| --- | --- | --- |
| `FS-GAS-ROUTE.rate` | No player-grounded, setting-aware rate interval was established | Exact products and runtime rates as attributed diagnostics |
| `MF-RESOURCE-SCOPE` | Eight selected full clusters cannot establish a neutral population range for long-horizon amount or concentration | Exact totals, vein structure, per-system totals, and cluster-relative leaders after complete generation |
| `RR-ACCESS.amount` | The selected full clusters cannot establish abundance ranges for each rare resource and resource setting | Exact amount, groups, and distance, kept separate |

This is a deliberate contraction of evaluable claims, not evidence that those
features lack value. SPEC-07 may retain them as prerequisites for later
research; implementation must not invent fallback thresholds.

## Runtime-confirmed validation catalogue

Every row is a normal 64-star, multiplier-1 cluster unless a settings variant
is stated. Named seeds are fixtures for their evidence, never special cases in
evaluation code.

| Family | Seed and confirmed evidence | Expected contract result |
| --- | --- | --- |
| `FS-TOPOLOGY` | `16315224`: 3 shared birth-giant satellites; `73339583`: 1 | **Supports**; **does not support** |
| `FS-POWER` | `45772`: a birth-system tidal lock; `73339583`: none | Tidal **supports**; tidal **does not support** |
| `FS-POWER` | `57213558`: solar `1.16`; `89864814`: solar `1.35`; `16315224`: solar `0.92` | Lower endpoint **preference-sensitive**; upper endpoint **supports**; below range **does not support** |
| `FS-GAS-ROUTE` | `45772`: fire ice and hydrogen; `73339583`: deuterium and hydrogen | Each present product **supports** and the absent alternative **does not support**; no product ranking |
| `FS-RESOURCES` | `73339583`: common total `60,569,720`; `63015198`: `74,788,292`; `48823053`: `105,667,431`; `96178012`: `124,175,637` | **Does not support**, **preference-sensitive**, **supports**, **supports** |
| `FS-RESOURCES` | `45772`: birth-system fire ice present; `73339583`: absent | Local fire-ice access **supports**; **does not support** |
| `MF-ENERGY-SYSTEM` | `63925962`: maximum luminosity `2.404`; `50245375`: `2.4489998817`; `8692056`: `2.4900000095`; `64181741`: `2.698` | Below-range **does not support**; lower endpoint **preference-sensitive**; upper endpoint and high case **support** |
| `MF-ENERGY-SYSTEM` | `61571387`: maximum `2.489`, second `2.486`, ratio about `1.001` | Output is **preference-sensitive** and distinct-leader component **does not support**; ten O stars do not upgrade either result |
| `MF-SPHERE-GEOMETRY` | `86764391`: 0 contained orbits; `45772`: 1; `48823053`: 4 | **Does not support**, **preference-sensitive**, **supports** containment |
| `MF-SPHERE-GEOMETRY` | `52322682`: radius `76,200`; `74250347`: `191,400`; `64181741`: `234,200` | Lower endpoint **preference-sensitive**; upper endpoint and high case **support** |
| `MF-SYSTEM-ROLE` | `64181741`: strong-energy component supports; `61571387`: energy output is sensitive | Emit a `strong-energy` role only for `64181741`; do not upgrade `61571387` |
| `MF-RESOURCE-SCOPE` | `61571387`: selected-set-high common total `32,048,044,700`; `96178012`: `19,481,451,769` | Strength remains **unknown** for both because the selected set supplies no accepted range |
| `CX-GROUPING` | `1369`: supported strong-energy system `2.274181` ly from birth; `61224745`: `4.621132` ly; `64181741`: `19.521508` ly | **Supports**, **preference-sensitive**, **does not support** compact grouping |
| `RR-ACCESS` | `73339583`: several rare resources about `2.028` ly away; `96178012`: unipolar magnets `7.353` ly away; `45772`: unipolar magnets `38.495` ly away | Distance **supports**, is **preference-sensitive**, and **does not support**, respectively; abundance remains **unknown** |

Seed `1369` was independently suggested by
[DSPSeedScanner's published seed lists](https://github.com/Selsion/DSPSeedScanner/tree/main/seed_lists).
The installed runtime confirmed luminosity about `2.504` at `2.274181`
light-years. Seeds `45772` and `82506644` came from its ashen-gelisol, fire-ice,
and tidal-lock list; the current runtime confirmed the relevant birth-system
facts. [DSPSeedSearch](https://github.com/HoneyTauOverTwo/DSPSeedSearch) supplied
an independent largest-sphere comparison target. These uses adopt seeds and
questions as test inputs, not either tool's generator, labels, or thresholds.

## Settings, tradeoff, and failure cases

| Case | Required result |
| --- | --- |
| Seed `12345678`, resource multipliers `1`, `0.5`, `0.1`, and infinite | Multiplier `1` may use starter ranges. Other amount conclusions are **unknown**; exact attributed facts remain available. No silent scaling. |
| Seed `12345678`, default combat versus altered initial colonization or maximum density | The default occupation range is not reused. Exact counts remain facts; opportunity strength is **unknown** until a matching settings range exists. |
| Seed `67937149`, default combat | Farming opportunity supports while birth-system exposure is a caution. Preserve both as a **tradeoff** when those decisions are compared. |
| Seed `96178012` | Starter common total supports, while its fully generated cluster total was lower than the other selected full-cluster cases. Preserve the early strength and long-horizon **unknown**; do not infer or score a reversal. |
| Complete preview with one required raw planet omitted | Preview-only families remain eligible. Every conclusion depending on the omitted raw scope is **unknown**. |
| Cancellation or raw-generation failure naming a seed and stage | Dependent conclusions are **unknown**; completed unrelated evidence remains attributable and cannot substitute for the missing scope. |
| Unsupported DSP version, changed member, absent `RandomTable` preparation, or failed derivation check | Every dependent conclusion is **unknown** with a compatibility diagnostic. No cached or approximate result is presented as current evidence. |
| Optional preference at `L`, inside `(L,U)`, at `U`, or outside the range | The neutral result follows the stored evidence and full range. An in-range value only explains it; an out-of-range value is reported as not applied. |

## Prohibited implementations exposed by the cases

The validation set must fail an implementation that:

- labels the highest value in one cluster universally high without the matching
  reference range;
- turns O-star count into an energy conclusion;
- treats gas products as mineable veins or presence as abundant supply;
- uses preview proxies for exact resources;
- rescales amount thresholds for another resource multiplier;
- lets a strong starter total cancel a weak individual resource or sparse
  vein-group result;
- treats a preference-sensitive endpoint as a stable positive;
- assigns a role from sensitive or unknown upstream evidence;
- replaces a missing raw planet with partial totals; or
- converts **unknown** or **not applicable** into a negative result.

## Accepted resolution

The product review accepted these four points without semantic change:

1. the fixed predicates answer recognizable decisions without prohibited
   inference;
2. the cohort construction and quartile method are adequate as the first
   bounded reference, with their settings and population limitations retained;
3. the `[2.5, 10]` light-year research-anchored range is acceptable for compact
   grouping and rare access; and
4. the three deferred strength components remain **unknown** until later
   evidence establishes defensible ranges.

This definition is therefore version `0.1.0`. A changed endpoint, direction,
predicate, role, or outcome requires a new minor candidate. Editorial
corrections alone increment the patch candidate.

## SPEC-06 conclusion

The accepted definition establishes deterministic fixed predicates,
setting-scoped reference ranges, endpoint semantics, representative
runtime-confirmed seeds, and explicit failure behavior for the first
implementation baseline. It
narrows unsupported rate and abundance claims to **unknown**, keeps component
results separate, and demonstrates positive, negative, preference-sensitive,
tradeoff, not-applicable, and unknown behavior without requiring player input.
