# Product Specification Roadmap

**Status:** Draft and still being worked on.

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

**State:** Proposed.

As a player comparing seeds, I want every result tied to all inputs that shape
the cluster so that repeating the same selection produces the same evidence.

Determine how seed, star count, resource settings, Dark Fog settings, galaxy
algorithm version, and any other discovered inputs affect generated output.
Confirm the identity through repeated runtime generation and controlled
single-setting changes.

**Produces:** A generation-identity contract, experiment record, and list of
settings proven relevant, irrelevant, or still unknown.

**Prior-art targets:** Compare the input and algorithm-version assumptions in
DSP-Seed-Finder, dsp_search_seed, DSPSeedCalc, and DSPSeedSearch. Use their
differences to design controlled runtime experiments, not to settle the
identity contract.

**Excludes:** Performance optimization, batch scanning, scoring, and UI.

### SPEC-02: Map reproducible runtime facts

**State:** Proposed.

As a player evaluating a cluster, I want recommendations limited to facts the
installed game can reproduce before play begins so that the scanner never
overstates what the seed guarantees.

Inventory candidate star, system, planet, orbit, resource, rare-resource,
distance, sphere-geometry, and Dark Fog facts. For each fact, identify its
runtime source, availability point, settings dependency, derivation, collection
cost, repeatability, and compatibility risk.

**Produces:** A runtime evidence feasibility matrix classifying each candidate
as directly supported, deterministically derived, available only after play,
unreliable, or unavailable.

**Prior-art targets:** Use dsp-csv-gen as the primary inventory prompt for
runtime-extractable facts. Use dsp_search_seed, DSPSeedCalc, and DSPSeedScanner
to identify additional claims, precision levels, and expensive generation
stages that require runtime confirmation.

**Excludes:** Choosing which supported facts are valuable to players.

## Phase 2 - Understand how players judge seeds

### SPEC-03: Build the player decision taxonomy

**State:** Proposed.

As a player choosing a seed, I want the scanner to understand the kind of run I
intend so that it evaluates relevant tradeoffs instead of applying a universal
definition of good.

Research seed-selection language, claimed benefits, thresholds, tradeoffs, and
disagreements for fresh starts, megafactories, and Dark Fog farming. Admit
additional contexts only when they represent a distinct decision, such as
speedrunning, scarce-resource or maximum-difficulty play, compact exploration,
or aesthetic goals.

**Produces:** A sourced taxonomy of contexts and candidate characteristics,
with the player decision each characteristic changes and contrary evidence or
preferences retained.

**Prior-art targets:** Treat the configurable rules in DSP-Seed-Finder and
dsp_search_seed, and the requested seed lists in DSPSeedScanner, as candidate
player vocabulary. Trace important criteria back to player discussions and
preserve missing contexts and disagreements rather than treating tool support
as proof of demand.

**Excludes:** Treating popularity as correctness, fixing thresholds, or
implementing profiles.

## Phase 3 - Find the decision-worthy intersection

### SPEC-04: Cross player value with reproducible evidence

**State:** Proposed.

As a player choosing among seeds, I want only characteristics that are both
trustworthy and relevant to my intended run so that the result reduces a real
decision rather than displaying trivia.

Cross the player taxonomy with the runtime feasibility matrix. For every
candidate characteristic, record the supported contexts, required evidence,
direction of preference, material tradeoffs, settings sensitivity, confidence,
and whether a defensible conclusion is possible.

**Produces:** A context-to-evidence decision matrix with candidates marked for
adoption, further research, diagnostic-only retention, or rejection.

**Prior-art targets:** Compare the rule composition in DSP-Seed-Finder and
dsp_search_seed with DSPSeedDatabase's separation of stored evidence, queries,
and weighted scoring. Retain explainable context-specific decisions while
recording why opaque or universal scoring approaches are accepted or rejected.

**Excludes:** Global seed scores, arbitrary weighting, presentation copy, and
implementation estimates.

## Phase 4 - Define the first conclusion contract

### SPEC-05: Specify bounded seed conclusions

**State:** Proposed.

As a player in New Game selection, I want a small set of justified conclusions
and decisive tradeoffs so that I can accept, reject, or compare a seed without
interpreting a wall of statistics.

Define the first supported contexts, the questions answered for each context,
the evidence and confidence required, the allowed conclusion forms, and cases
that must remain unknown. Prefer explainable categorical conclusions over a
single opaque score.

**Produces:** A versioned conclusion contract with context inputs, conclusion
semantics, evidence dependencies, conflicts, unknown behavior, and explicit
non-goals.

**Prior-art targets:** Review DSP-Seed-Finder's match rules and
DSPSeedSearch's single-purpose largest-sphere result as contrasting conclusion
models. Define what our product can conclude from runtime evidence without
inheriting either tool's criteria or presenting a match as universally good.

**Excludes:** Panel layout, visual design, interaction behavior, and scan
orchestration.

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
