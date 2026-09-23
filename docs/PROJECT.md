# DSP Seed Scanner - Project Steering

This document is the sole authority for project steering and all work state
and status, including historical dispositions. Other documents define scope,
contracts, procedures, and technical evidence; they link here for authorization,
progress, acceptance, closure, and release state.

**Current status:** Maintenance mode. The
[DSP 0.10.35.29057 scope](management/GAME-UPDATE-0.10.35.29057.md) is complete.
On 2026-09-23 the owner confirmed that installed candidate **1.4.119** passed
all owner validations and granted acceptance. No outstanding finding was
reported. Implementation, technical validation and owner acceptance are closed.

**Published baseline:** The owner published package **1.4.114**
on 2026-09-19 to [Thunderstore](https://thunderstore.io/c/dyson-sphere-program/p/DSPSeedScanner/DSPSeedScanner/)
and [GitHub release/tag 1.4](https://github.com/shytamir/DSPSeedScanner/releases/tag/1.4).
Both publication artifacts matched the verified package hash recorded below.

**Current phase:** None. Candidate publication remains a separate owner decision.

**Next action:** Await explicit release direction or new work. Keep the accepted
`1.4.119` candidate identity distinct from later documentation builds. This
closeout does not authorize publication, further game/environment changes or
save access. Older requests and archives do not activate work.

## DSP 0.10.35.29057 work state

The single [implementation plan](management/GAME-UPDATE-0.10.35.29057.md)
defines requirements and acceptance criteria instead of a new roadmap.
The approved starting source is `51c3a447d7802dbe90a7eaf1935ac87c54db2fa5`.

| Work | State and next boundary |
| --- | --- |
| Scope approval | Accepted by the owner on 2026-09-23; scope and management commit `7c8e42e` pushed to `main` before implementation; hosted run 117 passed. |
| Stories 1-2: runtime identity and calibration | Pushed in `e79029d`; hosted run 118 passed. Technically complete: exact native assembly/method hashes, definition 0.6.0 and four bands updated. Release solution/game-linked Plugin builds passed with zero warnings/errors; Core 16/16 and Runtime 95/95 passed; compiled evaluator reproduced the retained 96-seed oil distributions. Evidence: [conformance](CONFORMANCE.md#dsp-0103529057-evidence) and [predicate catalogue](specification/PREDICATE-RANGE-VALIDATION.md). |
| Story 3: naming and cache | Pushed in `13f33bd`; hosted run 119 passed. Exact LCID propagation, session/request equality, canonical cache separation and source identity retained. Core 16/16 and Runtime 96/96 passed; Release solution, game-linked Plugin and hosted-reference Plugin builds passed with zero warnings/errors. Native integration passed as recorded below. |
| Story 4: integration and candidate | Complete. Production native checks passed 15 naming cases, four fresh/cache baselines (852 planets), Chinese fresh/reuse (207 planets), cancellation/failure/restoration and full presentation parity. Exact candidate DLLs passed a focused repeat (414 planets), existing package validators and installed-file hash verification. Evidence and handoff were pushed in `97818fc`; [conformance](CONFORMANCE.md#dsp-0103529057-evidence) records provenance and limits. Owner acceptance below closes the installed-game gate. |
| Owner acceptance | Passed on 2026-09-23 for installed candidate 1.4.119. After confirming they would run the scope's [New Game checks](management/GAME-UPDATE-0.10.35.29057.md#owner-controlled-game-acceptance), the owner reported: "Owner acceptance passed on all validations." This covers installed startup, fresh/cache behavior, English/Chinese naming and pending toggles, Peace/Combat, preview exit/re-entry and resolution confirmation. These are owner-reported results, not agent-operated UI checks. |
| Publication | Not authorized by this implementation request. |

**Accepted candidate:** `1.4.119` from
`13f33bdeeeffc17c6719006d42216a3fb9223d5c`,
[hosted run 119](https://github.com/shytamir/DSPSeedScanner/actions/runs/35921054961).
ZIP SHA-256:
`5793A116A8C40B7E1E2601338F21BAF28C272AEE375AED9709727AC69A9579A9`.
The local ZIP is
`artifacts/game-update-1.4.119/download/packages/DSPSeedScanner-1.4.119.zip`.
Installed location: `BepInEx/plugins/DSPSeedScanner` in the inspected DSP
installation. The previous `1.4.114` DLLs are backed up at
`D:\Shy\Shared Untracked Repo Resources\DSPSeedScanner-Update-0.10.35.29057\before-installed-1.4.119`.
Later documentation builds do not replace this candidate identity.

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
decision and corresponding scope-document change.

## Management and documentation

The [game-update plan](management/GAME-UPDATE-0.10.35.29057.md) defines the
approved maintenance scope. The [roadmap placeholder](management/ROADMAP.md)
remains available for larger future work. Neither contains independent state
tracking. The
[archived panel roadmap](archive/PANEL-IMPROVEMENT-ROADMAP.md),
[owner findings](archive/PANEL-VALIDATION-FINDINGS.md) and
[preview recipe](archive/PANEL-PREVIEW-CHECK.md) preserve historical scope and
evidence, not instructions to resume the work.

The [feature-request register](management/FEATURE-REQUESTS.md) and
[technical-debt register](management/TECHNICAL-DEBT.md) describe older requests
and obligations. Their dispositions below are unchanged; the game-update work
does not activate them. The [documentation index](INDEX.md) links current contracts
and historical records.

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

These entries remained separate from the Panel Improvement Roadmap. Its
acceptance did not enroll, close, or change them; their dispositions are unchanged.

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
caution. It was not part of the Panel Improvement Roadmap.

## Historical delivery dispositions

This ledger consolidates the state formerly repeated across the linked
documents. Their scope and technical evidence remain there. Historical
execution or approval does not authorize new work in maintenance mode.

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

## Panel improvement and 1.4 release history

**Release review pass 1:** Source version routing and player-facing copy
were reviewed. Obsolete package screenshots were removed; product documentation
and error wording were clarified. The existing 15 Core and 95 Runtime checks
passed; solution and native-reference builds had zero warnings/errors.

**Release review pass 2:** The downloaded CI artifact matched its published
digest. Compiled/plugin/package versions agreed, and package contents, DLL
integrity, README/icon fidelity and player-facing copy checks passed. All
203 local document links resolved. No new in-game validation was performed;
the owner preview acceptance below applied to `1.3.110`.

**Published artifact:** `1.4.114`, source
`9a0c5c9d6dd9837768da13d275e3b6ef320341ec`, built by successful
[Actions run 114](https://github.com/shytamir/DSPSeedScanner/actions/runs/35430985790).
[Published ZIP](https://github.com/shytamir/DSPSeedScanner/releases/download/1.4/DSPSeedScanner-1.4.114.zip).
Its SHA-256 is
`6d6938e3c23ed9ee0094c3851bd1004fdf663be87054aaf906928d9df29a2001`.
On 2026-09-19 the owner published this package to Thunderstore and attached
the same ZIP to GitHub release/tag `1.4`. The Thunderstore download and GitHub
asset digest matched the SHA-256 above. Tag `1.4` pointed to documentation
commit `6f71920d62c3c9bc4ef1d28cf7bb9a0210302ccd`; the binary source remained
`9a0c5c9d6dd9837768da13d275e3b6ef320341ec`. Later documentation builds did not
replace the published artifact.

The [Panel Improvement Roadmap](archive/PANEL-IMPROVEMENT-ROADMAP.md) was
completed and owner-accepted. The owner authorized its execution and story-closing pushes
on 2026-09-19. All stories and epics E1-E6 were
completed; G1-G4 passed. M1 (Implementation evidence ready), M2 (Panels
technically complete), M3 (Ready for owner preview), and M4 (Owner accepted)
were declared. Release preparation followed separately from that roadmap.

**G4 evidence:** On 2026-09-19 the owner replied **“Pass for 1.3.110.”** to the
[final preview checklist](archive/PANEL-PREVIEW-CHECK.md). This accepted the exact
candidate identified below and closed `PANEL-D02`; no defects were reported.
Store publication and tagging were outside that roadmap and followed under
the separate owner-led release preparation.

**G2 evidence:** Solution and native-reference plugin builds: zero warnings
or errors; Core tests 15/15 and Runtime tests 95/95; 196 repository document
links resolved; diff/scope review clean. Story commits were pushed to `main` and their
hosted build/test/package runs through `4726a34` succeeded. No implementer game
execution, environment changes, deployment, or save access occurred.

**Accepted panel candidate:** `1.3.110`, built from
`4726a344ebe9e5b3fa40ab5046a05cb24d3f9b6d`; ZIP SHA-256
`0ca31a412208b319f740665781d198c7a183b510549ac6da9e5e8155376a1323`.
The downloaded CI package passed the existing build-artifact and package
checks locally; the versioned native-reference build also had zero warnings
or errors.
[Hosted run 110](https://github.com/shytamir/DSPSeedScanner/actions/runs/35429534551)
passed. Its downloaded artifact matched GitHub's published digest. The inner
package ZIP hash above identified the owner preview candidate; later
documentation commits and artifacts did not replace that acceptance identity.

| Panel story | State | Evidence |
| --- | --- | --- |
| PANEL-I01 | Complete | Read-only native unit inspection and numeric cases in [Panel evidence](specification/PANEL-EVIDENCE.md#native-resource-units); no game execution. |
| PANEL-I02 | Complete | Read-only source and catalogue inspection; attribution cases in [Panel evidence](specification/PANEL-EVIDENCE.md#planet-and-star-attribution). |
| PANEL-I03 | Complete | [Data routes and verification inputs](specification/PANEL-EVIDENCE.md#data-routes-and-verification-inputs); no unresolved investigation. |
| PANEL-F01 | Complete | Solution build and Runtime projection checks. |
| PANEL-F02 | Complete | Core boundary/coverage checks and Runtime split-deposit check; scarcity regressions retained. |
| PANEL-F03 | Complete | Solution/plugin builds and sibling attribution/independent-boundary checks. |
| PANEL-F04 | Complete | Rate/distance boundaries, nearest selection and scan/cache lifecycle checks; solution/plugin builds. |
| PANEL-M01 | Complete | System-total qualification and cache parity checks; solution build and Core/Runtime suites. |
| PANEL-M02 | Complete | Strict thresholds, split totals, finite-resource and nearest-selection checks; cache parity and solution build. |
| PANEL-S01 | Complete | Geometry, luminosity and distance-selection checks; solution/plugin builds. |
| PANEL-S02 | Complete | Star-group ordering, distance/column/color and lifecycle checks; solution/plugin builds. |
| PANEL-S03 | Complete | Aquatica eligibility, caps, distance origin, blank cells and overlapping-role checks; solution/plugin builds. |
| PANEL-T01 | Complete | Native-rate rounding, multiple-well and cache round-trip checks; solution/plugin builds. |
| PANEL-T02 | Complete | Per-parent moon order and home-only highlight checks; solution/plugin builds. |
| PANEL-T03 | Complete | Home resource fragments, strict distances and four-row displayed-candidate checks; solution build and Runtime regressions. |
| PANEL-T04 | Complete | Independent amount/distance boundary combinations and unchanged-cell checks; solution build and Runtime regressions. |
| PANEL-D01 | Complete | Frozen candidate and package/build verification reports prepared; identity recorded above. |
| PANEL-D02 | Complete | Package, reports and [concise checklist](archive/PANEL-PREVIEW-CHECK.md); 202 document links resolved at G3. Owner acceptance at G4 is recorded above. |

The [owner findings](archive/PANEL-VALIDATION-FINDINGS.md) defined the panel
requests; the roadmap defined their delivery scope, gates and restrictions.
Specifications describe product behavior. Technical checks, preview acceptance
and publication were distinct steps, recorded separately above.

The [User Feedback Roadmap](archive/USER-FEEDBACK-ROADMAP.md) remained separate
history. The panel work did not activate older requests or technical debt,
and historical procedures did not override its game/environment/save
restrictions. Archival likewise creates no new work authorization.
