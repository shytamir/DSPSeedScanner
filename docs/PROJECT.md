# DSP Seed Scanner - Project Definition

**Status:** SPEC-01 and SPEC-02 are accepted. SPEC-03 execution is complete and
its player decision taxonomy is pending acceptance. No user story is active.

The current roadmap is [docs/management/ROADMAP.md](management/ROADMAP.md).
It governs the remaining investigation that must precede implementation
planning. SPEC-01 produced the accepted
[generation-identity contract](specification/GENERATION-IDENTITY.md) and
[experiment record](specification/SPEC-01-EXPERIMENTS.md). SPEC-02 produced the
accepted [runtime evidence feasibility
matrix](specification/RUNTIME-EVIDENCE-FEASIBILITY.md). SPEC-03 produced a
candidate [player seed-decision
taxonomy](specification/PLAYER-DECISION-TAXONOMY.md), which remains pending
acceptance.

## Purpose

DSP Seed Scanner will help players decide whether a procedurally generated
Dyson Sphere Program cluster suits the run they intend to play.

The project will ask the installed game runtime to generate candidate clusters,
extract only faithfully reproducible facts, and interpret those facts in the
player's stated context. It will not maintain an independent galaxy generator
or present one universal definition of a good seed.

## Current specification objective

Before selecting features or implementation stories, the project must answer
three questions. SPEC-01 established the accepted identity required to answer
the first, SPEC-02 established the accepted evidence boundary, and SPEC-03
documented how players describe the second. The remaining roadmap must cross
those accepted inputs and define supported conclusions.

1. Which cluster facts can the available DSP runtime reproduce reliably from
   the seed and all generation-affecting settings?
2. Which characteristics do players use to judge seeds for different kinds of
   runs?
3. Where do those sets intersect strongly enough to support a meaningful,
   context-specific decision?

The result will be a bounded decision contract: supported player questions,
the evidence required to answer them, known tradeoffs, and claims the product
must decline to make.

## Evidence available to the investigation

The current workspace provides:

- an installed Dyson Sphere Program runtime;
- the installed `Assembly-CSharp.dll` and Unity managed assemblies as read-only
  technical evidence;
- an installed BepInEx environment for focused runtime probes;
- the New Game flow and generated galaxy objects for behavioral observation;
- community discussions, guides, existing seed tools, and reported seeds as
  sources of player vocabulary and candidate preferences.

Game assemblies and repeatable runtime observations are authoritative for what
the scanner can reproduce. Community material is evidence of what players care
about, not proof of game behavior. Existing scanners may suggest questions and
test cases, but their calculations are not accepted as authoritative without
confirmation against the installed runtime.

## Faithful reproduction standard

A fact is eligible for the product only when the investigation establishes:

- the complete input identity needed to reproduce it, including every relevant
  generation setting and the DSP version;
- an authoritative runtime source or a deterministic derivation from
  runtime-sourced facts;
- repeatable results across independent generations of the same input;
- the point at which the fact becomes available during New Game selection;
- acceptable collection cost and no dependence on player save progression;
- explicit behavior when the source is unavailable or changes in a later game
  version.

Seed alone does not identify a cluster. SPEC-01 established the required
layered generation identity, and SPEC-02 mapped which galaxy, resource, and
Dark Fog facts were available at each runtime stage. The accepted delivery
boundary supports immediate low-cost preview evidence and reserves exact,
execution-heavy terrain and resource generation for an explicit on-demand
control.

## Player decision contexts

SPEC-03 established three primary context families and admitted materially
distinct alternatives. They remain research classifications, not implemented
profiles.

### Fresh start

Concerned with the quality and convenience of progression from landing through
early interstellar expansion. Players compare starter-system topology, travel
friction, usable early resources, power opportunities, nearby expansion, and
the interaction with chosen resource and combat settings.

### Megafactory

Concerned with sustained late-game scale. Players compare stellar energy
potential, suitable planets and systems, rare-resource availability, transport
geometry, construction capacity, and performance-aware concentration or
distribution of production.

### Dark Fog farming

Concerned with establishing and sustaining deliberate combat farms. Players
compare initial occupation, hive opportunity, farmable system topology,
defensibility, and access to Dark Fog-exclusive drops. SPEC-03 separated those
seed-selection concerns from planetary bases, replenishment, yield, and other
state that develops only after play begins.

### Valid alternatives

The taxonomy admitted speedrunning, scarce-resource or maximum-difficulty play,
compact expansion, sphere showcases, themed challenges, and discovery-first
play because each changes a recognizable decision. A preference did not become
a first-class context merely because it was measurable.

## Decision-value standard

The product should retain a candidate characteristic only when:

1. players use it to make a recognizable choice;
2. the runtime can reproduce the supporting evidence faithfully;
3. the characteristic changes a decision in at least one defined context;
4. its benefit and important tradeoffs can be stated without implying a
   universal ranking;
5. the conclusion can be validated against representative and counterexample
   seeds.

Raw availability is insufficient. A reproducible number with no material
player decision is diagnostic data, not a product feature. A popular heuristic
without reproducible evidence is research context, not a scanner conclusion.

## Product invariants

- The installed DSP runtime is authoritative for cluster generation.
- Results are deterministic for the same complete generation identity and
  supported game version.
- Scanning does not modify player factories, progression, or save files.
- Every conclusion is attributable to normalized evidence and a stated player
  context.
- Generation, extraction, normalization, interpretation, and presentation
  remain separate responsibilities.
- Context-specific tradeoffs are preserved; the product does not invent a
  context-free best seed.
- Unsupported or incomplete evidence produces an explicit unknown, not a
  fabricated match or conclusion.
- Batch work remains bounded, observable, cancellable, and isolated per seed.
- DSP, Unity, and BepInEx assemblies remain external dependencies and are not
  redistributed.

## Scope of the specification roadmap

The roadmap covers:

- runtime-source and generation-input discovery;
- reproducibility experiments and an evidence feasibility matrix;
- research into player seed-selection goals and disagreements;
- a cross-context decision matrix;
- a supported conclusion contract and validation seed set;
- a specification exit review suitable for implementation planning.

It does not authorize plugin implementation, batch-scanner optimization,
result-schema construction, UI design, or release work.

## Architecture hypothesis

The investigation may refine names and boundaries, but implementation planning
should preserve this separation:

```text
Complete generation identity
    |
DSP runtime generation
    |
Normalized cluster evidence
    |
Context-specific interpretation
    |
Decision conclusions
    |
Future New Game presentation
```

The eventual New Game panel should present conclusions and decisive tradeoffs,
not a dump of cluster statistics. It should show low-cost preview conclusions
immediately and place execution-heavy evidence behind an on-demand control.
Its user story is deliberately inactive in the roadmap until the evidence and
conclusion contracts are settled.

## Existing delivery infrastructure

The repository already establishes placeholder semantic versioning and generic
Thunderstore package validation. `VERSION` supplies major and minor values,
the GitHub Actions run number supplies the patch, assembly and file versions
append `.0`, and diagnostic labels append the short commit.

See [THUNDERSTORE-PACKAGE.md](THUNDERSTORE-PACKAGE.md). The current package is
intentionally non-installable and remains outside the specification roadmap.

## Specification exit condition

Implementation planning may begin when the roadmap has produced:

- a verified generation-identity contract;
- a runtime evidence feasibility matrix;
- a sourced player-goal and seed-characteristic taxonomy;
- a context-to-evidence decision matrix;
- an initial conclusion contract with explicit exclusions;
- representative positive, negative, and tradeoff validation seeds;
- recorded uncertainties and technical probes that must remain implementation
  prerequisites.

Until then, proposed criteria and profiles remain research hypotheses rather
than product requirements.
