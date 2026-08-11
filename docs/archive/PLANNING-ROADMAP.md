# Product Specification Planning Roadmap

**Status:** Completed and accepted on 2026-08-11.

**Active user story:** None.

This roadmap orders the evidence needed to define DSP Seed Scanner before
implementation planning. Stories become active only through an explicit
project decision. Completing a story produces a reviewable specification
artifact; it does not authorize code or UI work.

## Working standards

- Separate runtime-confirmed facts, deterministic derivations, player reports,
  and project inference.
- Record the DSP version, generation settings, source, and confidence for every
  technical claim.
- Preserve disagreement and context instead of averaging preferences into one
  score.
- Use representative seeds and counterexamples, not anecdotes alone.
- Reject a candidate feature when it cannot change a player decision or cannot
  be reproduced faithfully.
- Keep research artifacts concise, source-linked, and explicit about unknowns.

## Prior-art use

Existing tools are inspiration targets and sources of candidate questions,
test cases, and architectural risks. Their reproduced generation logic and
reported outputs are not authoritative for this project until confirmed
against the installed DSP runtime.

- [DSP-Seed-Finder](https://github.com/DoubleUTH/DSP-Seed-Finder) demonstrates
  composable player-facing requirements and seed exploration.
- [dsp_search_seed](https://github.com/botany233/dsp_search_seed) demonstrates
  broad nested criteria, precise-versus-fast evidence modes, and result
  inspection.
- [DSPSeedScanner](https://github.com/Selsion/DSPSeedScanner) provides
  precomputed examples of characteristics players have requested and searched.
- [DSPSeedDatabase](https://github.com/SuperB3333/DSPSeedDatabase) demonstrates
  separating generation, indexed evidence, querying, and optional scoring.
- [DSPSeedSearch](https://github.com/HoneyTauOverTwo/DSPSeedSearch) demonstrates
  BepInEx integration, bounded long-running searches, and New Game inspection.
- [dsp-csv-gen](https://github.com/GreyHak/dsp-csv-gen) demonstrates extraction
  of cluster facts from the game runtime.
- [DSPSeedCalc](https://github.com/soarqin/DSPSeedCalc) provides an independent
  calculator, filters, viewer, tests, and performance comparison target.

## Phase 1 - Establish the reproducible evidence boundary

### SPEC-01: Identify the complete generation identity

**State:** Accepted on 2026-08-11.

As a player comparing seeds, I want every result tied to all inputs that shape
the cluster so that repeating the same selection produces the same evidence.

The investigation determined how seed, star count, resource settings, Dark Fog
settings, galaxy algorithm version, creation version, and the ordered theme
catalogue affected generated output. It confirmed the identity boundary
through assembly inspection and repeated, controlled runtime generation in two
independent game processes.

**Produced:** The accepted
[generation-identity contract](../specification/GENERATION-IDENTITY.md) and
[experiment record](../specification/SPEC-01-EXPERIMENTS.md), including inputs
confirmed relevant to each generation stage, inputs that did not affect the
pre-play snapshot, and unresolved boundaries.

**Prior-art used:** The input and algorithm-version assumptions in
DSP-Seed-Finder, dsp_search_seed, DSPSeedCalc, and DSPSeedSearch were used to
broaden the candidate input list. Their outputs did not settle the contract;
the installed runtime did.

**Excluded:** Performance optimization, batch scanning, scoring, and UI.

### SPEC-02: Map reproducible runtime facts

**State:** Accepted on 2026-08-11.

As a player evaluating a cluster, I want recommendations limited to facts the
installed game can reproduce before play begins so that the scanner never
overstates what the seed guarantees.

The investigation inventoried candidate star, system, planet, orbit, resource,
rare-resource, distance, sphere-geometry, and Dark Fog facts. It identified
their runtime sources, availability stages, settings dependencies, derivation
boundaries, collection costs, repeatability evidence, and compatibility risks.

**Produced:** The accepted
[runtime evidence feasibility matrix](../specification/RUNTIME-EVIDENCE-FEASIBILITY.md)
classifying facts as direct preview evidence, direct raw-generation evidence,
deterministic derivations, after-start state, or unsupported claims.

**Accepted delivery constraint:** Low-cost preview evidence can be presented
immediately. Exact terrain and resource evidence uses DSP's execution-heavy raw
generation path and must be initiated on demand through an explicit control.

**Prior-art used:** dsp-csv-gen supplied the primary runtime-extraction prompt.
dsp_search_seed, DSPSeedCalc, and DSPSeedScanner broadened the candidate fact
list and highlighted precision and generation-cost distinctions. The installed
runtime remained authoritative.

**Excluded:** Choosing which supported facts are valuable to players,
activating SPEC-03, scoring, profiles, UI, and implementation design.

## Phase 2 - Understand how players judge seeds

### SPEC-03: Build the player decision taxonomy

**State:** Accepted on 2026-08-11.

As a player choosing a seed, I want the scanner to understand the kind of run I
intend so that it evaluates relevant tradeoffs instead of applying a universal
definition of good.

The research examined seed-selection language, claimed benefits, thresholds,
tradeoffs, and disagreements for fresh starts, megafactories, and Dark Fog
farming. It admitted alternatives only when they represented a distinct
decision, including speedrunning, scarce-resource or maximum-difficulty play,
compact expansion, sphere showcases, themed challenges, and discovery-first
play.

**Produced:** The accepted [player seed-decision
taxonomy](../specification/PLAYER-DECISION-TAXONOMY.md) of contexts and
characteristics, with the affected player decisions, claimed benefits,
threshold language, tradeoffs, and contrary preferences retained.

**Prior-art used:** The configurable rules in DSP-Seed-Finder and
dsp_search_seed, and the requested seed lists in DSPSeedScanner, supplied
candidate player vocabulary. Important criteria were traced to player
discussions; tool support was not treated as proof of demand.

**Excluded:** Treating popularity as correctness, fixing thresholds, or
implementing profiles.

## Phase 3 - Find the decision-worthy intersection

### SPEC-04: Cross player value with reproducible evidence

**State:** Accepted on 2026-08-11.

As a player choosing among seeds, I want only characteristics that are both
trustworthy and relevant to my intended run so that the result reduces a real
decision rather than displaying trivia.

The accepted player taxonomy was crossed with the runtime feasibility matrix.
Every candidate characteristic was assessed for supported contexts, player
decision, evidence source and cost, direction and tradeoffs, settings
sensitivity, confidence, and defensible conclusion boundary.

**Produced:** The accepted [context-to-evidence decision
matrix](../specification/DECISION-EVIDENCE-MATRIX.md) marking candidates to
advance into SPEC-05, retain for further research or diagnostics, or reject.
It also established separate immediate-preview and on-demand evidence surfaces.

**Prior-art used:** The rule composition in DSP-Seed-Finder and dsp_search_seed
was compared with DSPSeedDatabase's separation of stored evidence, queries,
and optional scoring. Atomic predicates and evidence separation were retained
as useful patterns; universal weighted scoring was rejected because preference
direction changed by context, settings, horizon, and system role.

**Excluded:** Global seed scores, arbitrary weighting, presentation copy, and
implementation estimates.

## Phase 4 - Define the first conclusion contract

### SPEC-05: Specify bounded seed conclusions

**State:** Accepted on 2026-08-11.

As a player in New Game selection, I want a small set of justified conclusions
and decisive tradeoffs so that I can accept, reject, or compare a seed without
interpreting a wall of statistics.

The first supported contexts, neutral questions, evidence requirements,
comparison rules, outcome invariance, tradeoff behavior, mandatory unknown
cases, and ownership and validation requirements for predicates and preference
ranges were defined. Every supported context is evaluated without requiring
player input. Optional input may prioritize or refine a result inside its
accepted range but cannot reverse the neutral outcome.

**Produced:** The accepted [seed conclusion
contract](../specification/CONCLUSION-CONTRACT.md)
`0.1.0`, containing twelve automatically evaluated conclusion
families across six supported contexts. It defines robust whole-range outcomes,
`preference-sensitive` behavior for complete evidence that varies inside the
accepted range, strict unknown/not-applicable behavior, and validation
obligations for predicates and ranges.

**Prior-art used:** DSP-Seed-Finder's configurable match rules and
DSPSeedSearch's single-purpose largest-sphere result were treated as contrasting
conclusion models. The contract retained explicit predicates and declared
comparisons as product-owned, versioned definitions without inheriting either
tool's criteria or treating a match as universally good.

**Excluded at completion:** Panel layout, presentation copy, visual design,
result serialization, interaction behavior, and scan orchestration.

## Phase 5 - Validate that the specification discriminates usefully

### SPEC-06: Establish predicates, ranges, and the validation seed set

**State:** Accepted on 2026-08-11.

As a player relying on a neutral conclusion, I want its predicate and preference
range challenged by representative robust, sensitive, and mixed seeds so that
the result survives reasonable preferences rather than reflecting only the
examples that inspired it.

The product-owned predicate and preference-range set was established and
runtime-confirmed seeds were selected across the supported contexts. The
catalogue included robust positives, robust negatives, adjustable-range and
endpoint cases, conflicting strengths, settings-sensitive cases, and incomplete
evidence. Expected conclusions were recorded without turning named seeds into
permanent special cases.

**Produced:** The accepted [predicate, range, and validation
catalogue](../specification/PREDICATE-RANGE-VALIDATION.md)
`0.1.0`. It established deterministic fixed predicates,
setting-scoped cohort ranges, a research-anchored compact-distance range,
runtime-confirmed validation seeds, unknown/not-applicable cases, and an
accepted review resolution. Gas-rate, cluster-resource-strength, and
rare-abundance claims were purposefully left unknown because the evidence did
not support neutral ranges.

**Prior-art used:** Candidate cases were drawn from DSPSeedScanner's published
lists and regenerated with the supported DSP runtime. Seed `1369` confirmed a
nearby high-energy case, while `45772` and `82506644` confirmed relevant
birth-system traits. Cohort cases and counterexamples supplied the remaining
boundaries. Prior-art labels remained hypotheses until runtime confirmation;
DSPSeedSearch's single largest-sphere model informed the separate geometry
components without supplying their thresholds.

**Excluded at completion:** Exhaustive seed searches, performance benchmarking,
automated test implementation, and product code.

## Phase 6 - Close product specification

### SPEC-07: Prepare the implementation-planning boundary

**State:** Accepted on 2026-08-11.

As a maintainer planning implementation, I want the accepted product contract,
remaining uncertainties, and required technical probes in one bounded handoff
so that engineering work begins from evidence rather than rediscovering scope.

The prior artifacts were reconciled without reopening their product semantics.
The minimum active and explicitly unknown conclusion set was selected, and the
architecture, runtime probes, validation gates, and deferred work required for
implementation planning were recorded.

**Produced:** The accepted [implementation-planning
boundary](../specification/IMPLEMENTATION-PLANNING-BOUNDARY.md), containing the
normative baseline, single-request initial scope, immediate and on-demand
surfaces, required unknowns, runtime integration gates, conformance checks,
deferrals, and dependency-ordered inputs for a separate implementation roadmap.

**Prior-art used:** DSPSeedDatabase's pipeline separation, DSPSeedSearch's
bounded operational controls, and dsp-csv-gen's runtime extraction boundary
were retained as architecture-review prompts. No independent generator,
database architecture, UI behavior, or third-party threshold was adopted.

**Excluded:** Creating implementation stories inside this roadmap, activating
the future presentation story, and beginning product code.

## Future planning - deliberately inactive

### FUTURE-UI-01: Present conclusions in New Game selection

**State:** Inactive; outside this completed roadmap.

As a player considering a generated seed, I want concise context-aware
conclusions integrated into the New Game seed-selection flow so that I can make
a decision without leaving the game or decoding raw statistics.

A separate future roadmap may address panel placement, context selection,
comparison, refresh behavior, accessibility, and failure states. This completed
roadmap does not authorize that work.

**Prior-art targets:** A future presentation roadmap should review
DSP-Seed-Finder's interactive exploration and DSPSeedSearch's New Game hook for
workflow lessons. They are not presentation requirements, and no UI research
was authorized by this roadmap.

**Activation prerequisite:** SPEC-01 through SPEC-07 are complete. Adoption of
a separate presentation roadmap remains required.

**Explicitly excluded now:** UI implementation, mockups, input interception,
New Game patches, styling, and presentation-specific telemetry.
