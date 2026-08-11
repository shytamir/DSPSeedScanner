# DSP Seed Scanner - Project Definition

**Status:** SPEC-01 execution is complete and its candidate generation-identity
contract is pending acceptance. No user story is active.

The current roadmap is [docs/management/ROADMAP.md](management/ROADMAP.md).
It governs the remaining investigation that must precede implementation
planning. SPEC-01 produced a candidate
[generation-identity contract](specification/GENERATION-IDENTITY.md) and
[experiment record](specification/SPEC-01-EXPERIMENTS.md); neither has been
accepted as product scope merely because the work was completed.

## Purpose

DSP Seed Scanner will help players decide whether a procedurally generated
Dyson Sphere Program cluster suits the run they intend to play.

The project will ask the installed game runtime to generate candidate clusters,
extract only faithfully reproducible facts, and interpret those facts in the
player's stated context. It will not maintain an independent galaxy generator
or present one universal definition of a good seed.

## Current specification objective

Before selecting features or implementation stories, the project must answer
three questions. SPEC-01 established a candidate answer to the first; the
remaining roadmap must test and connect it to the other two.

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

Seed alone must not be assumed to identify a cluster. The investigation will
determine the full generation identity and record which settings affect galaxy,
resource, and Dark Fog outcomes.

## Player decision contexts

The research begins with four context families. They are hypotheses to test,
not implemented profiles.

### Fresh start

Concerned with the quality and convenience of progression from landing through
early interstellar expansion. Candidate themes include starter-system topology,
travel friction, usable early resources, power opportunities, nearby expansion,
and the interaction with chosen resource and combat settings.

### Megafactory

Concerned with sustained late-game scale. Candidate themes include stellar
energy potential, suitable planets and systems, rare-resource availability,
transport geometry, construction capacity, and performance-aware concentration
or distribution of production.

### Dark Fog farming

Concerned with establishing and sustaining deliberate combat farms. Candidate
themes include initial occupation, hive and planetary-base opportunity,
farmable planet and system topology, defensibility, replenishment, and access
to Dark Fog-exclusive drops. The research must separate seed-derived facts from
combat settings and state that develops only after play begins.

### Valid alternatives

The investigation will admit other materially distinct goals when evidence
supports them, such as speedrunning, scarce-resource play, high-difficulty
survival, compact exploration, visual or thematic preferences, and relaxed
first playthroughs. A preference does not become a first-class context merely
because it is measurable.

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
not a dump of cluster statistics. Its user story is deliberately inactive in
the roadmap until the evidence and conclusion contracts are settled.

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
