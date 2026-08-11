# Product Specification Roadmap

**Status:** Draft. SPEC-01 through SPEC-04 are accepted; SPEC-05 execution is
complete and pending acceptance.

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

**State:** Completed; pending acceptance.

As a player in New Game selection, I want a small set of justified conclusions
and decisive tradeoffs so that I can accept, reject, or compare a seed without
interpreting a wall of statistics.

The first supported contexts, questions, evidence requirements, comparison
rules, allowed outcomes, tradeoff behavior, and mandatory unknown cases were
defined. The contract selected explainable categorical conclusions and
explicitly rejected a single opaque score.

**Produced:** Candidate [seed conclusion
contract](../specification/CONCLUSION-CONTRACT.md)
`0.1.0-candidate.1`, containing twelve conclusion families across six supported
contexts, explicit miss/unknown/not-applicable behavior, comparison semantics,
declined claims, and validation obligations.

**Prior-art used:** DSP-Seed-Finder's configurable match rules and
DSPSeedSearch's single-purpose largest-sphere result were treated as contrasting
conclusion models. The contract retained explicit predicates and declared
comparisons without inheriting either tool's criteria or treating a match as
universally good.

**Excluded:** Activating SPEC-06, panel layout, presentation copy, visual
design, result serialization, interaction behavior, and scan orchestration.

## Phase 5 - Validate that the specification discriminates usefully

### SPEC-06: Establish the validation seed set

**State:** Proposed.

As a player relying on a conclusion, I want it challenged by representative
good, bad, and mixed seeds so that it reflects meaningful tradeoffs rather than
the examples that inspired it.

Select runtime-confirmed seeds for each supported context, including positive
examples, clear negatives, threshold boundaries, conflicting strengths, and
settings-sensitive cases. Record expected evidence and conclusions without
turning named seeds into permanent special cases.

**Produces:** A reproducible validation catalogue and acceptance procedure for
the conclusion contract.

**Prior-art targets:** Draw candidate positive, negative, and boundary seeds
from DSPSeedScanner's published lists and DSPSeedSearch's largest-sphere
results. Re-generate every adopted case with the supported DSP runtime and add
counterexamples; prior-art labels are hypotheses, not expected truth.

**Excludes:** Exhaustive seed searches, performance benchmarking, and automated
test implementation.

## Phase 6 - Close product specification

### SPEC-07: Prepare the implementation-planning boundary

**State:** Proposed.

As a maintainer planning implementation, I want the accepted product contract,
remaining uncertainties, and required technical probes in one bounded handoff
so that engineering work begins from evidence rather than rediscovering scope.

Review the prior artifacts, resolve or explicitly defer contradictions, select
the minimum supported context and conclusion set, and identify runtime probes
that must precede production code.

**Produces:** An approved specification baseline, implementation constraints,
validation obligations, deferred questions, and inputs for a separate
implementation roadmap.

**Prior-art targets:** Use DSPSeedDatabase's pipeline separation,
DSPSeedSearch's operational controls, and dsp-csv-gen's runtime boundary as
architecture-review prompts. Record any borrowed constraint explicitly and
reject generator-reimplementation assumptions that conflict with the product
contract.

**Excludes:** Creating implementation stories inside this roadmap.

## Future planning - deliberately inactive

### FUTURE-UI-01: Present conclusions in New Game selection

**State:** Inactive; not eligible for activation in this roadmap.

As a player considering a generated seed, I want concise context-aware
conclusions integrated into the New Game seed-selection flow so that I can make
a decision without leaving the game or decoding raw statistics.

Future planning may address panel placement, context selection, comparison,
refresh behavior, accessibility, and failure states only after the generation,
evidence, and conclusion contracts are approved.

**Prior-art targets:** When this story becomes eligible, review
DSP-Seed-Finder's interactive exploration and DSPSeedSearch's New Game hook for
workflow lessons. They are not presentation requirements, and no UI research
is authorized by the current roadmap.

**Activation prerequisite:** Completion of SPEC-01 through SPEC-07 and adoption
of a separate presentation roadmap.

**Explicitly excluded now:** UI implementation, mockups, input interception,
New Game patches, styling, and presentation-specific telemetry.
