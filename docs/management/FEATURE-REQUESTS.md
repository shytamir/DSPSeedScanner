# Feature Request Register

**Status:** Two requests are pending evaluation as of 2026-08-14; neither is
authorized or active.

This register preserves product requests that were deliberately excluded from
the completed [User Feedback Roadmap](../archive/USER-FEEDBACK-ROADMAP.md).
Recording a request does not authorize implementation or weaken an accepted
product contract.

## FR-001: Re-evaluate sphere conclusions

**Origin:** `FEED-REJ-01` in the completed User Feedback Roadmap.

**Category:** bug-fix

**Urgency:** Medium

**Importance:** High

**State:** Pending further evaluation; unauthorized.

As a player comparing sphere candidates, I want sphere conclusions to express
demonstrated player value rather than common geometry or a misleading verdict
based on one measurement.

**Reason deferred:** `Tiny shell` classified maximum shell radius alone and
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

**Out of scope until authorized:** Predicate, threshold, wording, cache, or
presentation changes; sphere-design advice; receiver performance; composite
scores; factual star statistics; or unrelated conclusions.

## FR-002: Define useful theme statistics

**Origin:** `FEED-REJ-02` in the completed User Feedback Roadmap.

**Category:** feature-request

**Urgency:** Low

**Importance:** Medium

**State:** Pending a future theme-statistics evaluation; unauthorized.

As a player looking for useful worlds, I want nearby mechanically relevant
planet themes identified with their locations and generated resources so I can
judge concrete opportunities rather than an arbitrary theme count.

**Reason deferred:** The source request singled out Aquatica, but one player's
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

**Out of scope until authorized:** An Aquatica exception, theme counts or
absence lines, scaffold changes, theme or vein presentation, raw-scan
retention, cache changes, aesthetic rankings, or a new panel surface.

Return to the [maintenance roadmap](ROADMAP.md),
[project steering](../PROJECT.md), or the [documentation index](../INDEX.md).
