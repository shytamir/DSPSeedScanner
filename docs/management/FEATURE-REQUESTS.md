# Feature Request Register

[PROJECT.md](../PROJECT.md) is the sole authority for project steering and work
status, including historical dispositions. This document contains scope,
requirements, or technical evidence; it does not track status.

This register preserves the scope and rationale of requests originating in the
[User Feedback Roadmap](../archive/USER-FEEDBACK-ROADMAP.md). Implementation
authority and request dispositions belong only in [PROJECT.md](../PROJECT.md).

## FR-001: Re-evaluate sphere conclusions

**Origin:** `FEED-REJ-01` in the User Feedback Roadmap.

**Category:** bug-fix

**Urgency:** Medium

**Importance:** High

As a player comparing sphere candidates, I want sphere conclusions to express
demonstrated player value rather than common geometry or a misleading verdict
based on one measurement.

**Evaluation rationale:** `Tiny shell` classified maximum shell radius alone and
could present an O star as an overall limitation despite its independent energy
value. `Many contained orbits` might be saturated across ordinary clusters,
and the calculation might compare moon-centric `orbitRadius` values with a
star-centric shell radius. A wording or threshold hotfix could not safely
resolve the separate correctness, prevalence, and utility questions.

**Evaluation gate:** Verify star-centric containment for planets and moons,
measure both conclusion distributions against a fixed reference-identity
sample, and define the player value of any retained conclusion. Then decide
whether to correct, replace, or remove each conclusion before authorizing an
implementation story.

**Outside evaluation scope:** Predicate, threshold, wording, cache, or
presentation changes; sphere-design advice; receiver performance; composite
scores; factual star statistics; or unrelated conclusions.

## FR-002: Define useful theme statistics

**Origin:** `FEED-REJ-02` in the User Feedback Roadmap.

**Category:** feature-request

**Urgency:** Low

**Importance:** Medium

As a player looking for useful worlds, I want nearby mechanically relevant
planet themes identified with their locations and generated resources so I can
judge concrete opportunities rather than an arbitrary theme count.

**Evaluation rationale:** The source request singled out Aquatica, but one player's
preferred theme did not justify a product-owned special case. A general theme
inventory also offered weak value without mechanics such as ocean type,
construction area, wind, geothermal opportunity, and generated resources.
Theme-proto possibilities could not substitute for veins confirmed on a
particular planet, and several direct resource results were already delivered.

**Evaluation gate:** Define the player questions and bounded mechanically
relevant theme set; decide the body and distance bounds; select accompanying
preview and exact generated-resource facts; establish overlap rules with
existing sulfuric-acid and rare-resource results; bound cached presentation
size; and choose a third `Themes` subcontainer, selectable view, or other
surface. A third subcontainer remained the smallest recommended option.

**Outside evaluation scope:** An Aquatica exception, theme counts or
absence lines, scaffold changes, theme or vein presentation, raw-scan
retention, cache changes, aesthetic rankings, or a new panel surface.

Return to the [roadmap placeholder](ROADMAP.md),
[project steering](../PROJECT.md), or the [documentation index](../INDEX.md).
