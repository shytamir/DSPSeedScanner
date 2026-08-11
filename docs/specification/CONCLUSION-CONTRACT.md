# Seed Conclusion Contract

**Status:** Accepted on 2026-08-11.

**Contract version:** `0.1.0`

This contract defines the first conclusions DSP Seed Scanner may draw from a
generated cluster. It turns accepted evidence into neutral, useful answers
without requiring the player to formulate queries and without inventing a
universal definition of a good seed.

The contract depends on the accepted [generation identity](GENERATION-IDENTITY.md),
[runtime evidence boundary](RUNTIME-EVIDENCE-FEASIBILITY.md), [player decision
taxonomy](PLAYER-DECISION-TAXONOMY.md), and [context-to-evidence
matrix](DECISION-EVIDENCE-MATRIX.md). Its accepted predicates, ranges, and
validation cases are defined by the [predicate, range, and validation
catalogue](PREDICATE-RANGE-VALIDATION.md) `0.1.0`.

It specifies conclusion meaning, not interface copy, storage layout, visual
priority, or implementation design.

## Neutrality and robustness rule

The product owns every conclusion's question, evidence dependencies, predicate,
comparison basis, and supported preference range. A player does not have to
supply a context, threshold, desired arrangement, role definition, or reference
before the scanner can evaluate the cluster.

For a conclusion family `C`, let `E` be its complete normalized evidence and
`P(C)` be the full product-defined range of admissible preferences. The neutral
outcome is evaluated across the whole range:

```text
Evaluate C(E, p) for every p in P(C)
    |
All admissible preferences agree positively -> Supports
All admissible preferences agree negatively -> Does not support or Caution
Admissible preferences disagree             -> Preference-sensitive
Invariant strengths and drawbacks coexist   -> Tradeoff
Evidence incomplete or unsupported           -> Unknown
Settings remove the question                  -> Not applicable
```

The core outcome must survive every permitted preference value. An individual
preference may refine strength, relevance, or explanation inside that outcome;
it may not replace or reverse it.

Structural predicates with no adjustable value use a singleton preference
range. Quantitative predicates use product-owned intervals validated and
versioned with the contract. Until such an interval is accepted, the dependent
conclusion is **unknown** rather than silently using a community threshold or a
developer guess.

## Evaluation inputs

Neutral evaluation requires only authoritative product inputs:

- the complete generation identity and supported DSP version;
- all generation and combat settings that affect the evidence;
- a complete New Game galaxy snapshot for preview conclusions;
- exact generated coverage for conclusions that require raw evidence; and
- the contract version and its matching predicate and preference-range set.

The scanner evaluates every supported context that is applicable to those
inputs. Contexts are product-owned interpretive lenses, not questions the
player must construct.

### Optional player influence

Optional player input may:

- prioritize or filter already evaluated contexts and conclusions;
- select a value inside a published preference interval;
- refine the displayed strength of a stable conclusion;
- show the selected interpretation of a **preference-sensitive** conclusion;
  or
- reduce disclosure for discovery-first play.

Optional player input may not:

- alter evidence, predicates, admissible ranges, or the neutral outcome;
- turn **supports** into **does not support**, or the reverse;
- hide a decisive tradeoff inside a score;
- make incomplete evidence appear complete; or
- create an unsupported context or conclusion.

An input outside the accepted range is not clamped or treated as authoritative.
The neutral conclusion remains valid, and the unsupported preference is
reported as not applied.

## Allowed outcome semantics

Every applicable conclusion family resolves without player input:

| Outcome | Meaning |
| --- | --- |
| **Supports** | Complete evidence satisfies the context objective for every preference in the accepted range. |
| **Does not support** | Complete evidence fails the same bounded objective for every preference in the accepted range. This does not make the seed bad or unplayable. |
| **Preference-sensitive** | Complete evidence produces different interpretations within the accepted preference range. The evidence is known; its value is legitimately variable. |
| **Tradeoff** | Complete evidence establishes invariant material strengths and drawbacks that must remain visible together. |
| **Caution** | Complete evidence establishes an invariant context-specific exposure or limitation without predicting that harm will occur. |
| **Unknown** | Required evidence, compatibility, coverage, predicate, or accepted range is missing or incomplete. Unknown is not a negative result or preference ambiguity. |
| **Not applicable** | Authoritative settings make the context question irrelevant, such as Dark Fog farming in peace mode. |

No numeric confidence, letter grade, aggregate score, or universal verdict is
allowed by version `0.1.0`.

## Predicate and preference-range requirements

Every conclusion family must own a versioned evaluation definition containing:

1. a neutral question and the context decision it informs;
2. the subject scope: planet, system, birth region, or complete cluster;
3. its normalized evidence and required evidence stage;
4. a fixed structural predicate or quantitative function;
5. the complete admissible preference interval, including units and endpoints;
6. invariant positive, negative, preference-sensitive, tradeoff, unknown, and
   not-applicable behavior where relevant;
7. settings that alter evidence or applicability;
8. material tradeoffs and prohibited inferences; and
9. validation cases supporting every reachable outcome.

Ranges are owned by the contract, not by UI defaults. They must be justified
from the accepted player taxonomy, challenged by runtime-confirmed examples,
and approved through SPEC-06. Different settings may select different published
ranges when their effect is understood; they may not silently rescale one range.

An interval may express tolerable variation in distance, amount, concentration,
energy opportunity, or context intensity. Its endpoints are contract data and
change under the versioning rules below.

## Evidence, confidence, and attribution

Every conclusion must:

- name its context and planet, system, birth region, or cluster subject;
- identify the decisive normalized preview, derived, or raw facts;
- preserve generation settings, contract version, and generated coverage;
- state material tradeoffs and limiting assumptions;
- distinguish factual absence from unavailable evidence; and
- remain separate from unrelated conclusions rather than participating in a
  hidden weight.

Preview conclusions require a complete galaxy snapshot. On-demand conclusions
require complete raw generation for the declared scope. Cancellation, failure,
unsupported runtime members, or partial coverage forces every dependent family
to **unknown**. Preview facts remain usable by preview families but cannot stand
in for missing raw evidence.

This version permits no probabilistic or graduated confidence label. An outcome
other than **unknown** or **not applicable** requires an accepted runtime source,
a versioned derivation when used, complete declared coverage, an accepted
predicate, and an accepted preference range. Context dependence is represented
by the outcome and its interval, not by lowering confidence.

## Automatically evaluated contexts

The first contract evaluates six bounded contexts:

| Context | Neutral decision supported | Required boundary |
| --- | --- | --- |
| Fresh start | Identify robust structural conveniences, early power and gas-product opportunities, and starter-resource limitations or strengths | Convenience is not viability; exact deposits require on-demand generation. |
| Megafactory | Identify robust energy, sphere-geometry, system-role, and resource-scale candidates | Production capacity, hardware suitability, and logistics throughput remain unknown. |
| Dark Fog farming | Identify initial farming opportunity, unwanted exposure, and role-separation tradeoffs | Combat must be enabled; farm performance and evolving state remain unknown. |
| Compact expansion | Identify robust or preference-sensitive geometric grouping of supported system roles | Distance is not travel time or throughput. |
| Sphere showcase or energy-focused | Identify strong stellar and shell-geometry options under product-owned comparisons | Receiver performance, realized output, and attractiveness remain unknown. |
| Decision-relevant traits | Identify supported factual arrangements that materially affect at least one accepted context | The contract may describe the arrangement, not whether it is rare, beautiful, fun, or challenging. |

Scarce-resource and maximum-difficulty play qualify the fresh-start, resource,
and Dark Fog evaluations using their authoritative game settings. They do not
receive a broad survival or viability conclusion.

Discovery-first preferences affect disclosure only. Set-seed speedrunning
remains deferred because a neutral route conclusion requires a separately
accepted category, route, game version, split objective, and predicate set.

## Conclusion catalogue

The catalogue retains twelve semantic families. Each evaluates automatically;
the player is never required to provide its predicate.

| ID | Neutral question | Evidence | Product-owned evaluation | Required boundary |
| --- | --- | --- | --- | --- |
| `FS-TOPOLOGY` | Does the birth system contain a materially concentrated early-expansion arrangement? | Preview topology | Fixed structural predicates identify shared-giant satellite groups and other accepted concentration patterns. | Emit **tradeoff** when the same topology constrains an accepted sphere or role objective. Never predict travel time. |
| `FS-POWER` | Does a reachable birth-system planet provide a robust renewable-power opportunity? | Preview rotation, wind, solar, and orbit facts | Fixed traits such as tidal locking are invariant; quantitative energy ratios use accepted ranges. | Identify the planet and progression limits. Never predict realized build output. |
| `FS-GAS-ROUTE` | Which early-to-midgame giant-product opportunities exist? | Preview gas products and rates | Fixed product presence is evaluated automatically; rates use setting-specific accepted ranges. | Preserve collection prerequisites and never imply mineable deposits. Multiple products may coexist without ranking. |
| `FS-RESOURCES` | Are starter deposits robustly strong, limited, or preference-sensitive for early progression? | Complete on-demand birth-system veins and oil | Per-resource, setting-specific amount and distribution intervals determine the stable outcome. | **Unknown** until ranges and complete coverage exist. Never convert the result into starter viability. |
| `MF-ENERGY-SYSTEM` | Which systems are robust stellar-energy candidates within this cluster? | Preview stellar facts and Dyson luminosity | Cluster-relative extrema and accepted separation intervals identify leaders, ties, or a preference-sensitive leading group. | An extremum is not a universal best system and does not predict realized output or hardware suitability. |
| `MF-SPHERE-GEOMETRY` | Which systems provide robust shell-radius or orbital-containment opportunities? | Preview and derived sphere geometry | Fixed runtime containment predicates and accepted geometry ranges identify opportunities and ties. | State the containment predicate. Never infer receiver effectiveness. |
| `MF-SYSTEM-ROLE` | Which systems robustly satisfy the contract's supported factual roles? | Preview composition, topology, distance, and eligible conclusions | Versioned role predicates are evaluated for every system without a player-supplied role. | Buildable area, factory capacity, and logistics performance cannot define a role in this version. |
| `MF-RESOURCE-SCOPE` | Is system or cluster supply robustly strong, limited, or preference-sensitive for long-horizon scale? | Complete on-demand resources and vein structure | Setting-specific amount, distribution, and coverage intervals produce separate conclusions. | Keep amount, node/group structure, and distance separate. Never predict lifetime or throughput. |
| `DF-OCCUPATION` | Does generated Dark Fog occupation create robust farming opportunity, exposure, or a role-separation tradeoff? | Preview hive counts, orbits, safety fields, and combat settings | Product-owned occupation ranges are evaluated separately for farming opportunity and protected-role exposure. | **Not applicable** in peace mode. Never predict bases, levels, yield, threat, or attack timing. |
| `CX-GROUPING` | Are supported system roles robustly compact, dispersed, or preference-sensitive? | Preview distances and role conclusions; raw evidence when a role requires it | Product-owned roles and distance intervals evaluate the complete cluster automatically. | **Unknown** when a dependent role lacks evidence. Never infer travel time or throughput. |
| `RR-ACCESS` | Is rare-resource access robustly close, abundant, limited, or preference-sensitive? | Complete on-demand rare veins and derived distances | Separate setting-specific amount and distance intervals are evaluated together without merging their outcomes. | Preserve nearby convenience versus distant abundance as a possible **tradeoff**. |
| `TRAIT-SUMMARY` | Which accepted decision-relevant structural traits materially distinguish this cluster's options? | Eligible preview or raw evidence | A versioned registry emits only factual traits connected to another supported context. | No subjective quality or undefined rarity. Diagnostic facts without a decision remain excluded. |

## Robust quantitative behavior

For a monotonically favorable metric with an accepted player-preference
threshold interval `[L, U]`:

| Evidence value | Neutral outcome |
| --- | --- |
| Value satisfies the objective even at `U` | **Supports** across the full range. |
| Value fails the objective even at `L` | **Does not support** or **caution**, according to the family contract. |
| Value satisfies some thresholds but not others | **Preference-sensitive**. |
| Value or accepted interval is unavailable | **Unknown**. |

Direction may be reversed for metrics where lower is favorable, such as
distance. Non-monotonic metrics require an explicit evaluation function and
must demonstrate the same whole-range invariance. Multi-metric conclusions may
emit **tradeoff**; they may not collapse disagreement into a weighted average.

An optional player selection inside `[L, U]` may explain which side of a
**preference-sensitive** result fits that player. The neutral result remains
**preference-sensitive** and must remain recoverable.

## Comparison semantics

Neutral comparisons may use only product-owned references:

- another system or planet within the completely evaluated cluster;
- a cluster-relative extremum or tie;
- an accepted quantitative preference interval; or
- a versioned reference population established by an accepted later contract.

Community thresholds, arbitrary defaults, and unbounded user-supplied values
are not comparison bases. “High,” “low,” “near,” “many,” “best,” and similar
terms require one of the references above and must preserve units and ties.

Comparisons across different relevant settings, cluster sizes, game versions,
or incomplete raw scopes are declined unless the conclusion explicitly and
validly concerns that difference.

## Required tradeoff behavior

The following conflicts remain visible whenever both sides are established:

- starter convenience versus long-horizon importance;
- nearby access versus potentially greater distant resource abundance;
- resource amount versus vein/node distribution;
- concentrated roles versus deliberate system separation;
- Dark Fog farming opportunity versus protected-infrastructure exposure;
- sphere-energy potential versus practical need and performance budget; and
- factual optimization versus discovery-first disclosure.

The contract may emit multiple conclusions about one seed. It never decides
these conflicts through weights or lets one strength cancel an unrelated
caution.

## Explicitly declined claims

Version `0.1.0` must not produce:

- a universal best-seed verdict, aggregate score, or arbitrary weighting;
- a guarantee that a starter is viable, safe, easy, or impossible to lose;
- buildable area, foundation burden, factory capacity, or miners supported;
- travel time, logistics throughput, or transport operating cost;
- ray-receiver effectiveness or realized Dyson sphere output;
- future Dark Fog bases, level, growth, farm yield, threat, or attack timing;
- visual attractiveness, fun, novelty, challenge quality, or undefined rarity;
- speedrun suitability without a separately accepted route contract;
- a raw-evidence conclusion from preview proxies or partial generation; or
- a neutral positive or negative result whose polarity changes inside the
  accepted preference range.

Unsupported or after-start requests resolve to **unknown** with the dependency
identified. Complete but preference-dependent evidence resolves to
**preference-sensitive**. Neither may be approximated by an adjacent fact.

## Validation obligations addressed by SPEC-06

This contract became an implementation baseline with the accepted matching
predicate and preference-range definition. SPEC-06 addressed these required
cases:

- stable positive evidence across the full accepted range;
- stable negative or caution evidence where that outcome is allowed;
- evidence inside each adjustable interval that produces
  **preference-sensitive**;
- a mixed case that preserves an invariant **tradeoff**;
- settings-sensitive cases, including **not applicable** behavior;
- incomplete, cancelled, unavailable, and incompatible cases producing
  **unknown**;
- optional preference values at both endpoints and inside the interval,
  demonstrating that the neutral outcome survives; and
- counterexamples exposing prohibited proxies, hidden weighting, or polarity
  changes.

Quantitative ranges must record units, inclusive/exclusive endpoints, direction,
settings applicability, research basis, runtime-confirmed boundary examples,
and compatibility with the contract version. On-demand entries also require
complete-versus-partial coverage and deterministic repetition.

SPEC-06 was allowed to narrow or remove a family that could not obtain a
defensible range or discriminate usefully. Symmetry was not a reason to
preserve it.

## Contract evolution

This contract uses semantic versioning independently of the mod package:

- a **major** change removes an outcome, changes an existing conclusion's
  meaning, expands player influence beyond its accepted bounds, or weakens
  unknown/declined behavior;
- a **minor** change adds a backward-compatible context, family, predicate, or
  preference range; and
- a **patch** clarifies wording or corrects an error without changing meaning.

Changing an accepted range endpoint, direction, or associated outcome requires
at least a minor version. Pre-release labels identify unaccepted candidates.
Acceptance of this artifact established contract version `0.1.0`; later
changes must record their compatibility impact.

## SPEC-05 conclusion

The revised contract defines twelve automatically evaluated conclusion families
across six contexts. The product owns their predicates, comparisons, and
bounded preference ranges. Players may prioritize, filter, or refine results,
but they are not required to supply the questions and cannot reverse a neutral
outcome.

Stable evidence produces **supports**, **does not support**, **caution**, or
**tradeoff**. Complete evidence whose interpretation changes within the accepted
range produces **preference-sensitive**. Missing or unsupported evidence alone
produces **unknown**.

No presentation copy, layout, result serialization, implementation priority,
accepted quantitative range, or validation seed was selected in SPEC-05.
