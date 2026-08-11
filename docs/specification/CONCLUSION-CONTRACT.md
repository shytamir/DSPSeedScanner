# Seed Conclusion Contract

**Status:** Candidate contract produced by SPEC-05; pending product acceptance.

**Contract version:** `0.1.0-candidate.1`

This contract defines the first conclusions DSP Seed Scanner may draw from a
generated cluster. It turns accepted evidence into answers to bounded player
questions without inventing a universal definition of a good seed.

The contract depends on the accepted [generation identity](GENERATION-IDENTITY.md),
[runtime evidence boundary](RUNTIME-EVIDENCE-FEASIBILITY.md), [player decision
taxonomy](PLAYER-DECISION-TAXONOMY.md), and [context-to-evidence
matrix](DECISION-EVIDENCE-MATRIX.md).

It specifies meaning, not interface copy, storage layout, visual priority, or
implementation design.

## Contract inputs

A conclusion is valid only for a declared evaluation scope containing:

- the complete generation identity and supported DSP version;
- one named player context;
- the relevant time horizon and intended system or planet role;
- all generation and combat settings that affect the evidence;
- any user-supplied requirement or comparison reference;
- the evidence stage and exact generated coverage; and
- whether the player requested preview-only or on-demand evaluation.

The scanner must not silently assume a context, route, threshold, system role,
or comparison reference when one is required. A missing required input produces
**unknown**, not a default preference.

## Allowed outcome semantics

Every requested conclusion question resolves to one of these outcomes:

| Outcome | Meaning |
| --- | --- |
| **Supports** | Complete evidence satisfies an explicit structural predicate, user requirement, or declared comparison for the named context. |
| **Does not support** | Complete evidence proves that the same explicit predicate or requirement is not satisfied. This is not a claim that the seed is bad or unplayable. |
| **Tradeoff** | Complete evidence establishes material advantages and disadvantages that should remain visible together. It does not collapse them into a winner. |
| **Caution** | Complete evidence establishes a context-specific exposure or limitation, without predicting that harm will occur. |
| **Unknown** | Required evidence, scope, compatibility, coverage, predicate, or reference is absent or incomplete. Unknown is not equivalent to does not support. |
| **Not applicable** | The selected settings or context make the question irrelevant, such as Dark Fog farming in peace mode. |

No numeric confidence, letter grade, aggregate score, or universal verdict is
allowed by version `0.1.0-candidate.1`.

## Evidence and attribution rules

Every conclusion must be explainable from its evaluation scope:

1. Name the context and the planet, system, cluster, or comparison being
   evaluated.
2. Identify the decisive normalized facts and whether each came from preview,
   deterministic derivation, or on-demand raw generation.
3. Preserve the generation settings and evidence coverage needed to reproduce
   those facts.
4. State the material tradeoff or limiting assumption when the same fact can
   have an opposite interpretation in another context.
5. Distinguish factual absence from unavailable or incomplete evidence.
6. Keep simultaneous conclusions separate; one strength must not numerically
   cancel an unrelated caution.

Preview conclusions require a complete New Game galaxy snapshot. On-demand
conclusions require complete raw generation for the declared planet, system,
or cluster scope. Cancellation, generation failure, unsupported runtime
members, or partial coverage forces every dependent question to **unknown**.
Preview facts may remain available, but they may not substitute for the missing
raw evidence.

### Confidence semantics

This version permits no probabilistic or graduated confidence label. An outcome
other than **unknown** or **not applicable** requires the matrix's high-confidence
intersection: an accepted runtime source, a versioned derivation when used,
complete declared coverage, and an explicit player decision predicate.

Context dependence is not low confidence. It is expressed through the named
context, settings, role, comparison, and tradeoffs. When evidence is only a
proxy or the interpretation still has medium or low confidence, the dependent
question remains **unknown** until a later contract supplies and validates the
missing predicate.

## Supported contexts in the first contract

The first contract supports six bounded contexts:

| Context | Supported decision | Required qualification |
| --- | --- | --- |
| Fresh start | Identify structural conveniences, early power or gas-product opportunities, and exact starter-resource support | Convenience is not viability; exact deposits require on-demand generation. |
| Megafactory | Nominate systems for energy, sphere geometry, factual roles, and declared resource requirements | Production capacity, hardware suitability, and logistics throughput remain unknown. |
| Dark Fog farming | Identify initial farming opportunity or exposure and separation options | Combat must be enabled; farm performance and evolving state remain unknown. |
| Compact expansion | Compare geometric grouping of explicitly named system roles | Distance is not travel time or throughput. |
| Sphere showcase or energy-focused | Identify strong stellar and shell-geometry options | Receiver performance, realized output, and attractiveness remain unknown. |
| Explicit trait match | Determine whether a requested reproducible star, planet, orbit, theme, or resource combination exists | The contract answers the factual match, not whether it is rare, beautiful, fun, or challenging. |

Scarce-resource and maximum-difficulty play are supported as settings-sensitive
qualifiers to fresh-start, resource, and Dark Fog questions. They do not yet
receive a broad survival or viability conclusion.

Relaxed or discovery-first play is supported by permitting evaluation to be
disabled or limited to requested questions. It does not create an automatic
conclusion.

Set-seed speedrunning is deferred. Its resource and topology facts are
reproducible, but a useful route conclusion requires a named category, route,
game version, split objective, and route-specific predicates that this contract
does not define.

## Conclusion catalogue

Conclusion identifiers are stable semantic names. The wording eventually shown
to a player may change without changing their meaning.

### Fresh-start conclusions

| ID | Player question | Evidence stage | Supports when | Does not support, tradeoff, or unknown behavior |
| --- | --- | --- | --- | --- |
| `FS-TOPOLOGY` | Does the birth-system arrangement support the player's requested compact early expansion pattern? | Preview | Direct orbit and satellite topology satisfies an explicit requested arrangement. | **Does not support** only for a declared pattern miss. Mention moon/star-orbit tradeoffs. **Unknown** if no pattern was requested. |
| `FS-POWER` | Does a reachable birth-system planet provide the requested renewable-power opportunity? | Preview | Direct rotation flags and wind/solar fields satisfy an explicit factual power predicate, such as tidal locking. | **Does not support** for an explicit predicate miss. Preserve planet location and progression limits; do not predict build output. |
| `FS-GAS-ROUTE` | Does the birth system provide the requested giant product route? | Preview | Direct gas products include the requested item, with its setting-sensitive collection rate available. | **Does not support** if absent. State that collection requires progression and does not imply mineable deposits. |
| `FS-RESOURCES` | Do generated starter deposits support an explicit resource-presence or amount requirement? | On demand | Complete birth-system raw coverage proves the requested resource presence or bound. | **Does not support** only for a proven miss. **Unknown** for partial coverage. Never convert the result into general starter viability. |

### Megafactory and sphere conclusions

| ID | Player question | Evidence stage | Supports when | Does not support, tradeoff, or unknown behavior |
| --- | --- | --- | --- | --- |
| `MF-ENERGY-SYSTEM` | Which system best satisfies the declared stellar-energy comparison or requirement? | Preview | Direct stellar facts and runtime Dyson luminosity satisfy a user bound or identify an extremum within the declared cluster scope. | A comparative result must name its reference and ties. Do not infer realized generation, need, or hardware suitability. |
| `MF-SPHERE-GEOMETRY` | Does a system provide the requested shell radius or planet-containment option? | Preview | Runtime-derived allowed radius and an explicit containment predicate satisfy the request. | **Does not support** for a proven geometry miss. State the chosen shell and containment predicate; do not infer receiver performance. |
| `MF-SYSTEM-ROLE` | Does a system match an explicit factual role description? | Preview | Its star, planet, theme, topology, and distance facts satisfy all declared role predicates. | **Does not support** for a complete predicate miss. Buildable area, factory capacity, and logistics performance cannot be role predicates in this version. |
| `MF-RESOURCE-SCOPE` | Does a fully generated system or cluster satisfy a declared resource requirement or comparison? | On demand | Normalized exact totals, rare-resource presence, and/or vein structure satisfy the declared requirement under identical relevant settings. | **Unknown** for partial scope. Preserve amount, node/group structure, and distance as separate reasons; do not predict lifetime or throughput. |

### Dark Fog conclusion

| ID | Player question | Evidence stage | Supports when | Does not support, tradeoff, or unknown behavior |
| --- | --- | --- | --- | --- |
| `DF-OCCUPATION` | Does a system provide or avoid the requested initial Dark Fog opportunity? | Preview | Initial/max hive counts and orbit opportunity satisfy the requested direction under the complete combat settings. | Use **tradeoff** when opportunity also exposes a protected role. Use **caution** for unwanted exposure. **Not applicable** in peace mode. Never predict bases, levels, yield, threat, or attack timing. |

### Cross-context conclusions

| ID | Player question | Evidence stage | Supports when | Does not support, tradeoff, or unknown behavior |
| --- | --- | --- | --- | --- |
| `CX-GROUPING` | Does the cluster geometrically group the explicitly requested system roles? | Preview, plus on-demand evidence if a role requires exact resources | Every role is supported by eligible evidence and the declared distance predicate is satisfied. | **Does not support** for a complete miss; **unknown** if any role depends on incomplete raw evidence. Do not infer travel time or throughput. |
| `RR-ACCESS` | Does a generated scope contain the requested rare resource at the required geometric relationship? | On demand | Exact rare veins satisfy the presence or amount predicate and derived distance satisfies the separately declared relationship. | Report closeness and abundance separately and retain their possible conflict. **Unknown** for incomplete raw coverage. |
| `TRAIT-MATCH` | Does the cluster contain the explicitly requested reproducible combination? | Preview or on demand, according to the requested facts | Every requested factual predicate is satisfied within the declared scope. | **Does not support** for a complete miss. **Unknown** if any predicate is unsupported or ungenerated. Do not infer rarity or subjective quality. |

## Comparison semantics

The catalogue permits four comparison bases:

- a user-supplied bound with preserved units;
- another seed generated with the same relevant identity and settings;
- another system or planet within the same cluster; or
- an extremum within a declared, completely evaluated scope.

“High,” “low,” “near,” “many,” “best,” and similar relative terms are invalid
without one of those bases. Community-reported thresholds are not built-in
defaults. Comparisons across different resource settings, combat settings,
cluster sizes, game versions, or incomplete raw scopes must be declined unless
the conclusion explicitly concerns that difference.

An extremum is a factual comparison, not an endorsement. For example, the
highest Dyson luminosity in a cluster may be identified, but it is not “the
best system” without a declared sphere-energy question and its tradeoffs.

## Required tradeoff behavior

The following conflicts must remain visible whenever both sides are supported:

- starter convenience versus long-horizon importance;
- nearby access versus potentially greater distant resource abundance;
- resource amount versus vein/node distribution;
- concentrated roles versus deliberate system separation;
- Dark Fog farming opportunity versus exposure of protected infrastructure;
- sphere-energy potential versus the player's actual need and performance
  budget; and
- factual optimization versus discovery-first disclosure preference.

The contract does not decide these conflicts through weights. It may emit
multiple **supports**, **tradeoff**, or **caution** outcomes for the same seed
when they answer different questions.

## Explicitly declined claims

Version `0.1.0-candidate.1` must not produce:

- a universal best-seed verdict, aggregate score, or arbitrary weighting;
- a guarantee that a starter is viable, safe, easy, or impossible to lose;
- buildable area, foundation burden, factory capacity, or miners supported;
- travel time, logistics throughput, or transport operating cost;
- ray-receiver effectiveness or realized Dyson sphere output;
- future Dark Fog bases, level, growth, farm yield, threat, or attack timing;
- visual attractiveness, fun, novelty, challenge quality, or undefined rarity;
- speedrun suitability without a separately accepted route contract; or
- any raw-evidence conclusion from preview proxies or partial generation.

Requests for these claims resolve to **unknown** with the unsupported or
after-start dependency identified. They must not be approximated by an adjacent
supported fact.

## Validation obligations for SPEC-06

Before this contract can become an implementation baseline, SPEC-06 must test
each catalogue entry with:

- a positive case that produces **supports**;
- a complete negative case that produces **does not support** where allowed;
- a mixed case that preserves a material **tradeoff** or **caution**;
- a settings-sensitive case where the interpretation changes or becomes
  **not applicable**;
- an incomplete, cancelled, or unavailable evidence case that produces
  **unknown**; and
- a counterexample that would expose any prohibited proxy or universal claim.

Comparative entries also require ties, changed references, and mismatched
settings. On-demand entries require declared scope, complete-versus-partial
coverage, and deterministic repetition under the accepted generation identity.

SPEC-06 may reveal that a catalogue entry is not discriminating or testable.
Such an entry must be narrowed or removed rather than preserved for symmetry.

## Contract evolution

This contract uses semantic versioning independently of the mod package:

- a **major** change removes an outcome, changes an existing conclusion's
  meaning, or weakens unknown/declined behavior;
- a **minor** change adds a backward-compatible context, question, or
  conclusion identifier; and
- a **patch** clarifies wording or corrects an error without changing meaning.

Pre-release labels identify unaccepted candidates. Acceptance of this artifact
would establish contract version `0.1.0`; later changes must record their
compatibility impact.

## SPEC-05 conclusion

The first contract defines twelve attributable conclusion families across six
supported contexts. It permits contextual support, explicit misses, preserved
tradeoffs, cautions, unknowns, and not-applicable results. It never converts
evidence into a context-free grade.

The contract deliberately favors exact structural predicates, user-supplied
bounds, and declared comparisons. It defers speedrun routing, broad survival or
starter-viability judgments, subjective evaluation, and all after-start
performance claims.

No presentation copy, layout, result serialization, implementation priority,
or validation seed was selected in SPEC-05.
