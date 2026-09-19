# DSP Seed Scanner - Project Steering

This document is the sole authority for project steering and all work state
and status, including historical dispositions. Other documents define scope,
contracts, procedures, and technical evidence; they link here for authorization,
progress, acceptance, closure, and release state.

**Current status:** Active development. The owner accepted and activated the
[Panel Improvement Roadmap](management/ROADMAP.md) on 2026-09-19, authorizing
its execution and story-closing commits/pushes to `main`.

**Current phase:** Phase 2 - implementation.

**Next story:** `PANEL-S03: Final Aquatica group`. G1 passed and
M1 (Implementation evidence ready) is declared. G2-G4 remain pending.

| Panel story | State | Evidence |
| --- | --- | --- |
| PANEL-I01 | Technically complete | Read-only native unit inspection and numeric cases in [Panel evidence](specification/PANEL-EVIDENCE.md#native-resource-units); no game execution. |
| PANEL-I02 | Technically complete | Read-only source and catalogue inspection; attribution cases in [Panel evidence](specification/PANEL-EVIDENCE.md#planet-and-star-attribution). |
| PANEL-I03 | Technically complete | [Data routes and verification inputs](specification/PANEL-EVIDENCE.md#data-routes-and-verification-inputs); no unresolved investigation. |
| PANEL-F01 | Technically complete | Solution build and Runtime projection checks; owner preview awaits G4. |
| PANEL-F02 | Technically complete | Core boundary/coverage checks and Runtime split-deposit check; scarcity regressions retained. Owner preview awaits G4. |
| PANEL-F03 | Technically complete | Solution/plugin builds and sibling attribution/independent-boundary checks. Owner preview awaits G4. |
| PANEL-F04 | Technically complete | Rate/distance boundaries, nearest selection and scan/cache lifecycle checks; solution/plugin builds. Owner preview awaits G4. |
| PANEL-M01 | Technically complete | System-total qualification and cache parity checks; solution build and Core/Runtime suites. Owner preview awaits G4. |
| PANEL-M02 | Technically complete | Strict thresholds, split totals, finite-resource and nearest-selection checks; cache parity and solution build. Owner preview awaits G4. |
| PANEL-S01 | Technically complete | Geometry, luminosity and distance-selection checks; solution/plugin builds. Owner preview awaits G4. |
| PANEL-S02 | Technically complete | Star-group ordering, distance/column/color and lifecycle checks; solution/plugin builds. Owner preview awaits G4. |
| Remaining implementation and handoff stories | Not started | Await their roadmap sequence and gates. |

The accepted [owner findings](management/PANEL-VALIDATION-FINDINGS.md) define
the requested panel changes. The roadmap owns their delivery scope, gates,
and execution restrictions. Existing specifications describe the implemented
baseline until the relevant stories update them; roadmap acceptance does not
claim that the new behavior is already implemented or validated.

The previous [User Feedback Roadmap](archive/USER-FEEDBACK-ROADMAP.md) remains
completed history. Older feature requests and technical debt are not activated
by this work, and historical validation procedures do not override the active
roadmap's prohibition on implementer game execution, environment changes, or
save access.

## Product decision

DSP Seed Scanner helps players decide whether a procedurally generated Dyson
Sphere Program cluster suits the run they intend to play. It presents bounded,
context-specific conclusions and does not define a universally best seed.

## Accepted steering decisions

### Evidence authority

- The installed Dyson Sphere Program runtime is authoritative for generated
  cluster evidence.
- A seed is meaningful only with its complete generation identity and relevant
  settings.
- Community material and prior tools may identify player questions and test
  cases, but cannot override runtime-confirmed behavior.
- Unsupported compatibility, incomplete coverage, or unavailable evidence
  produces an explicit unknown rather than an approximation.
- Raw vein positions may use deterministic invariant-decimal normalization of
  DSP's single-precision values; preserving the source floating-point bit
  pattern is not required by the active conclusion contract.

The detailed boundaries are maintained in the accepted
[generation identity](specification/GENERATION-IDENTITY.md) and
[runtime evidence feasibility](specification/RUNTIME-EVIDENCE-FEASIBILITY.md)
documents.

### Decision contract

- The retained decision contexts are fresh start, megafactory, compact
  expansion, and sphere or energy. Dark Fog occupation is shown only as
  neutral status metadata. Redundant decision-relevant traits are not emitted.
- Neutral outcomes must survive the complete accepted preference range.
  Optional preferences may filter or explain an outcome but cannot create or
  reverse it.
- Components remain independent. Material conflicts remain visible as
  tradeoffs; no global score or hidden weighting may collapse them.
- Unsupported claims remain declined even when adjacent diagnostic facts are
  available.

The accepted semantics and thresholds are maintained in the
[conclusion contract](specification/CONCLUSION-CONTRACT.md) and
[predicate and validation catalogue](specification/PREDICATE-RANGE-VALIDATION.md).

### Delivery boundary

- Each completed New Game cluster-preview load creates exactly one resolution
  attempt for its complete generation identity.
- A valid local cache hit resolves without a new scan. Otherwise the mod
  evaluates immediate preview evidence and automatically runs at most one
  bounded full raw scan for that preview load.
- Replaced or exited previews cancel obsolete work at a safe boundary, and a
  stale result can never update the current panel.
- Only audited semantic conclusions and bounded resource-statistics payloads
  derived from a successful complete scan are persisted in a versioned,
  bounded local cache under the active BepInEx configuration area. Raw planets,
  full normalized resource evidence, execution diagnostics, and rendered
  wording are not cached. The [cache contract](CACHE.md) defines the retained
  payload and applies the accepted
  [Peace/Combat reuse rule](specification/GENERATION-IDENTITY.md#peacecombat-reuse-applicability)
  while preserving canonical generation identity and live Dark Fog status.
- The panel presents concise natural-language strengths,
  preference-sensitive results, and limitations without requiring player
  input. Unknown and not-applicable components remain omitted. Named candidates
  must retain evidence-backed attribution; the panel does not expose internal
  identifiers, raw runtime units, or mechanical evidence summaries.
- Its numeric corner setting defaults to `1` for
  bottom-right, then proceeds clockwise as `2` bottom-left, `3` top-left, and
  `4` top-right. Border-center placement is prohibited. The panel shows visible
  activity and terminal failure states. Its translucent scrollable conclusion
  viewport occupies 37% of resolution width and height and groups each player
  context once across the three outcome columns.
- The project resolves one current generation identity at a time. Batch search,
  parallel generation, unattended databases, shared caches, and exports
  require later steering decisions.

### Safety and responsibility boundaries

- Scanning must not modify player saves, factories, progression, or persistent
  game state.
- Generation, runtime extraction, normalization, evaluation, orchestration,
  and presentation remain separate responsibilities.
- Long-running work must be bounded, observable, cancellable at safe
  boundaries, and attributable to its seed and stage.
- DSP, Unity, and BepInEx assemblies remain external dependencies and are not
  redistributed.
- Co-installed BepInEx plugins and preloader assemblies do not by themselves
  make the scanner unsupported, including when they alter generation. Their
  inventory and the observed assembly, algorithm, catalogue, and generation-
  method identity remain part of the cache key. Unsupported game versions,
  missing required members, incomplete evidence, and runtime failures still
  fail closed. Plugin interactions are an accepted compatibility risk rather
  than a reason to require an isolated installation.

## Current scope exclusions

The active product scope does not include an independent galaxy generator,
universal seed ranking, subjective quality claims, post-start guarantees,
adaptive panel placement, player scoring or required preferences, manual scan
or retry controls, seed comparison, broad compatibility promises, telemetry,
or publication to an external service. New scope requires an explicit steering
decision and corresponding roadmap change.

## Management and documentation

The [Panel Improvement Roadmap](management/ROADMAP.md) defines story scope,
phase gates, milestones, and the final owner-only handoff. Their state is
recorded only in this document.
Its [source findings](management/PANEL-VALIDATION-FINDINGS.md) remain a separate
record of this session's owner claims and requirements. The
[feature-request register](management/FEATURE-REQUESTS.md) describes older
product requests, while the [technical-debt register](management/TECHNICAL-DEBT.md)
describes engineering obligations and evaluation criteria. Their dispositions
are recorded below. The [User Feedback Roadmap](archive/USER-FEEDBACK-ROADMAP.md)
preserves historical scope, implementation details, and evidence. The
[documentation index](INDEX.md) lists all current and archived documents with
their purpose.

## State-tracking rules

- Keep all project, roadmap, epic, story, gate, milestone, request, debt,
  blocker, handoff, acceptance, and release state here, including historical
  dispositions. Do not duplicate trackers, progress logs, status fields, or
  closure declarations in roadmaps, specifications, findings, handoff notes,
  indexes, contributor instructions, or archives.
- Update the affected entry here as work progresses. Record the result,
  supporting evidence or links, and relevant main-commit/push information.
  Keep technical completion, handoff readiness, owner acceptance, and
  publication distinct. Epics derive completion from their child stories;
  gates and milestones require their own evidence-backed declarations.
- For a blocker, record the affected story, evidence checked, exact obstacle,
  impact, and smallest additional input needed. Apply the roadmap's blocker
  procedure before dependent work proceeds.
- Include these updates with the story's closing commit and authorized final
  push to main. Report the resulting commit and actual push/CI result in the
  execution report; do not create a follow-up commit only to embed its own hash.
  Preserve the candidate's original build identity separately from later
  acceptance-documentation commits.
- Keep requirements, gate definitions, factual investigation findings, test
  results, and candidate identities in their appropriate documents. These
  describe scope or evidence, not project status. A passing test recorded
  there does not declare a story, phase, or owner-acceptance gate passed.

## Other tracked work

These entries remain separate from the Panel Improvement Roadmap. Its
acceptance does not enroll, close, or change them.

| Item | Disposition |
| --- | --- |
| [FR-001: Re-evaluate sphere conclusions](management/FEATURE-REQUESTS.md#fr-001-re-evaluate-sphere-conclusions) | Pending further evaluation; unauthorized, as recorded on 2026-08-14. |
| [FR-002: Define useful theme statistics](management/FEATURE-REQUESTS.md#fr-002-define-useful-theme-statistics) | Pending future theme-statistics evaluation; unauthorized, as recorded on 2026-08-14. |
| [TD-001: Non-success runtime isolation probes](management/TECHNICAL-DEBT.md#td-001-complete-non-success-runtime-isolation-probes) | Closed by IMPL-08 on 2026-08-11. Deferred at IMPL-03 acceptance and retained through IMPL-07; the IMPL-08 gate supplied the missing runtime proof. |
| [TD-002: Preloader and in-memory patch uncertainty](management/TECHNICAL-DEBT.md#td-002-detect-preloader-and-in-memory-generation-patch-uncertainty) | Closed by IMPL-08 on 2026-08-11. Deferred at IMPL-03 acceptance and retained through IMPL-07. The later coexistence policy is a separate steering decision above. |
| [TD-003: Single-assembly packaging](management/TECHNICAL-DEBT.md#td-003-evaluate-single-assembly-packaging) | Closed as declined on 2026-08-12 during the 0.9 release-candidate review. The three-DLL package was retained; this did not block presentation or publication readiness. |
| [TD-004: Developer-probe output failures](management/TECHNICAL-DEBT.md#td-004-contain-developer-probe-output-failures) | Pending evaluation since 2026-08-13. Not a blocker for ordinary player operation or the 1.0.85 candidate. |

The historical refinement roadmap's provisional `T`/`C` badge rule remains
deferred, with no active story, until a future conclusion emits tradeoff or
caution. It is not part of the Panel Improvement Roadmap.

## Historical delivery dispositions

This ledger consolidates the state formerly repeated across the linked
documents. Their scope and technical evidence remain there. Historical
execution or approval does not authorize actions under this roadmap.

| Work or artifact | Recorded disposition |
| --- | --- |
| [Product specification planning](archive/PLANNING-ROADMAP.md) | Completed and accepted on 2026-08-11. SPEC-01 through SPEC-07 were accepted; no story remained active. Presentation work remained inactive in that roadmap and was separately planned and approved on 2026-08-12. |
| [Generation identity](specification/GENERATION-IDENTITY.md) and [SPEC-01 experiments](specification/SPEC-01-EXPERIMENTS.md) | Investigation completed; contract accepted on 2026-08-11 under its documented limits after SPEC-02 confirmed the intended BepInEx evidence boundary. |
| [Runtime feasibility](specification/RUNTIME-EVIDENCE-FEASIBILITY.md), [player taxonomy](specification/PLAYER-DECISION-TAXONOMY.md), [evidence matrix](specification/DECISION-EVIDENCE-MATRIX.md), [conclusion contract](specification/CONCLUSION-CONTRACT.md), and [predicate catalogue](specification/PREDICATE-RANGE-VALIDATION.md) | Accepted on 2026-08-11. |
| [Implementation planning boundary](specification/IMPLEMENTATION-PLANNING-BOUNDARY.md) | Accepted on 2026-08-11 without semantic change, closing the specification phase; subsequently fulfilled by the scanner-core roadmap. Specification acceptance alone did not authorize implementation. |
| [Scanner core](archive/CORE-ROADMAP.md) | Completed and accepted on 2026-08-11; IMPL-01 through IMPL-09 accepted, no active story. IMPL-03 acceptance included TD-001/TD-002; IMPL-08 closed them. Completion allowed a presentation-roadmap proposal, not automatic activation of that work. |
| [Scanner core conformance](CONFORMANCE.md) | Accepted with IMPL-08 on 2026-08-11. IMPL-09 subsequently implemented packaging without changing the conformance result. |
| [New Game presentation](archive/PRESENTATION-ROADMAP.md) | PRES-01 through PRES-07 completed and accepted on 2026-08-12. Story activation and acceptance were separate from roadmap approval. PRES-05's corner-anchor requirement was resolved that day; PRES-07 passed the sole installed human gate. No active story remained; completion did not authorize publication or refinement work. |
| [Presentation refinement](archive/PRESENTATION-REFINEMENT-ROADMAP.md) | RFIN-01 through RFIN-10 completed and accepted on 2026-08-12; RFIN-01 through RFIN-09 were accepted without semantic change. All gates passed, no active story. RFIN-10's correction superseded RFIN-01's initial recovery-frame implementation. |
| [RFIN-10 human validation](archive/RFIN-10-HUMAN-VALIDATION.md) | All seven installed 4K checks passed on 2026-08-12; no residual human-validation blocker was recorded. |
| [Runtime identity maintenance](archive/RUNTIME-IDENTITY-MAINTENANCE-DRAFT.md) | HOTFIX-01 and FSOR-01 implemented and repository acceptance gates passed on 2026-08-13. COMPAT-02 retired without implementation. Original-reporter confirmation of HOTFIX-01 was not recorded before archival. |
| [User feedback](archive/USER-FEEDBACK-ROADMAP.md) | Completed and owner-accepted on 2026-08-14; no story remained active. FEED-01 through FEED-05 accepted on 2026-08-13; FEED-06 through FEED-10 accepted on 2026-08-14. FEED-06 included direct-build refinement followed by exact CI-artifact owner validation; FEED-08/09 included the cluster table workshop. All phase gates and release-candidate milestones passed as detailed below. |
| [Release Candidate 1.0.85](archive/RELEASE-CANDIDATE-1.0.85.md) | Readiness and final owner approval passed on 2026-08-14 for Build 85, source `ab1ad34f9a7d08a56455a5b92ad077f1b51ff0f9`. Later documentation-only builds did not replace that candidate. No release-blocking residual was identified. |

### User Feedback phase and milestone history

| Gate | Passed | Milestone established |
| --- | --- | --- |
| `ready-for-new-panel` | 2026-08-13, FEED-01/02 owner acceptance with no blocking residual | Clean slate for panel work |
| `ready-for-panel-population` | 2026-08-13, FEED-03/04 owner acceptance | Panel renders with all features enabled even if not yet consumed |
| `ready-for-cluster-panel-population` | 2026-08-14, FEED-05/06 and exact CI-artifact owner validation | Panel home system fully populated |
| `ready-for-subsection-consumer` | 2026-08-14, FEED-07/08/09 combined cluster presentation validation | Panel cluster populated excluding subsection |
| `ready-for-end-to-end-testing` | 2026-08-14, FEED-10 owner acceptance | Panel fully populated; release-candidate work authorized |
| Candidate readiness | 2026-08-14, exact 1.0.85 source/build/package/runtime evidence | Release candidate ready |
| Final owner validation | 2026-08-14, owner accepted that exact candidate | Release candidate approved |
