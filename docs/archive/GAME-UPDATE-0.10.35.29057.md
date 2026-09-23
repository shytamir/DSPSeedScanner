# DSP 0.10.35.29057 implementation plan

This document defines the implementation scope for adapting the mod to DSP
`0.10.35.29057`, using bounded stories instead of a new roadmap. It describes
proposed implementation and acceptance criteria, not implemented behavior.
[PROJECT.md](../PROJECT.md) remains the sole authority for authorization,
work status, decisions requiring owner direction, acceptance, and release.
Implementation, deployment, validation and publication authorization are
recorded there rather than inferred from the requirements below.

## Intended result

A player opening New Game on the updated DSP build can scan with the current
mod's conclusions and presentation, with calibrated resource judgments that
reflect the changed generation and location names that match the native naming
choice. Fresh results and cached results must agree. Changing naming mode must
not retain labels from the previous mode or allow obsolete work to replace the
current preview.

The source baseline is `main` at `51c3a447d7802dbe90a7eaf1935ac87c54db2fa5`.
Its `src` tree matches published mod `1.4.114`. The changes are confined to
runtime identity, four calibrated ranges, native naming input and its cache
consequences, and the checks and documentation needed for those contracts.

## Evidence to reuse

The investigation and offline updates already establish the following:

- The old version gate rejects the updated game. The reference assembly hash
  also needs updating before the new runtime can receive calibrated conclusions.
- All 518 recorded preview rows retain their measured metrics. The original
  starter cohort was regenerated twice with identical results between runs.
  Four calibrated ranges need revision under the existing calibration rule.
- The existing production native and terrain-worker paths agree on 25 sampled
  planets spanning solid-planet algorithms 1-13. The tested success,
  cancellation, and failure paths restore borrowed game state. Another 24
  native/worker comparisons cover 32/64 stars, 0.1/1/100 resources and
  Peace/Combat. This supports retaining the extraction path.
- The new `GameDesc.starNameLCID` input is omitted by the mod. Across five
  sampled seeds, default versus Chinese naming changes 60-62 star names and
  234-250 planet names per cluster while measured geometry remains equal.
  Cache payloads persist names, so fixing regeneration alone is insufficient.
- Four previously cached seeds (`29519403`, `32395590`, `25064027`,
  `42424242`) completed fresh offline scans of 852 solid planets. Each gained
  three starter oil wells; their recorded other statistics and conclusions
  retained the preceding baseline's values. This does not imply that every
  unreported vein is unchanged: the separate native investigation found finite
  and rare-resource changes, including starter seed `26240403`.
- The offline utility now uses the actual `1.4.114` evaluator and presenter.
  Its later conclusion changes reflect catching up to the mod, not additional
  mod requirements. Its English/default naming tests do not validate the new
  native naming choice or the installed BepInEx UI.

The local evidence is retained under
`D:\Shy\Shared Untracked Repo Resources`:

| Record | Use |
| --- | --- |
| `DSPSeedScanner-Update-0.10.35.29057/COMPATIBILITY-EVIDENCE.md` | Native identity, generation comparison, calibration, naming and limits; adjacent `evidence-manifest.json` identifies raw evidence. |
| `DSPSeedScanner-Offline/update-0.10.35.29057/BASELINE-COMPARISON.md` | Four-seed old/new game comparison using the prior offline presentation. |
| `DSPSeedScanner-Offline/presentation-1.4.114/PRESENTATION-UPDATE.md` | Published-mod provenance, current presentation, fresh/repeat/cache parity. |

Keep these existing probes, private runtime files and raw outputs outside Git.
Use their retained manifests and fixtures rather than recreating the research.
The rejected `underwaterOilGroups` diagnostic in `settings-home-run1` must not
be used: group positions are normalized directions, not terrain altitudes.
The independent raw-node measurement in `oil-run1/oil.tsv` is the relevant
underwater-oil evidence.

## Story 1: recognize the updated runtime accurately

As a player on the updated game, I want the scanner to accept the supported
runtime and identify its evidence correctly, while preserving the existing
limits on calibrated conclusions.

### Implementation

- Update `ConclusionDefinition.ReferenceGameVersion` from `0.10.34.28529`
  to `0.10.35.29057`, including the version in `ReferenceCombatSettingsKey`
  (`GameDesc.SetForNewGame:0.10.35.29057`). Request creation-version checks,
  fixtures and current documentation must use the same reference.
- Replace `ReferenceAssemblySha256` with the measured original native
  `Assembly-CSharp.dll` SHA-256:
  `E75D3FE4B6A9CA822766189F826BA3A8348DFB7E301AA37FF6779DB29A83FD8D`.
  The former value is
  `AE0BA95F75BD879A62AA4CE253B2AB78EAA4FB3C7C595F5E1FEE75EBE0E0EF85`.
- Capture the updated generation-method digest using the exact production
  `DspPreviewGateway.CaptureGenerationMethodHash` algorithm: UTF-8 declaring
  type/method labels followed by raw method-body bytes for
  `UniverseGen.CreateGalaxy(GameDesc)` and
  `PlanetData.RegenerateRawDataImmediately`, in that order, then SHA-256.
  The new value has not been established by the retained investigation.
  Replace `ReferenceGenerationMethodIlSha256` if it differs from the old
  `A0CC806F17FD8A88468AA8CF05CDD4C1A8728A33BA1A4C0FA967C2EF50775C9B`;
  otherwise retain it with measured provenance. Neither the resolved-IL
  comparison nor the renamed private game DLL's hash substitutes for this.
- Version the changed calibration definition as `0.6.0` from `0.5.0`.
  Retain conclusion contract `0.1.0`, galaxy algorithm `20200403`, ordered
  theme IDs `1..25`, and the 64-star reference setting.
- Preserve the distinction between the version/member support gate, exact
  reference identity for calibrated conclusions, and observed fingerprints
  used for cache separation. The current method digest is cache provenance,
  not an additional hard whitelist. Do not introduce a blanket mod or assembly
  rejection policy as part of this update.

Primary surfaces: `ConclusionDefinition.cs`, `DspPreviewGateway.cs`,
`CompatibilityPolicy.cs`, and their existing test fixtures. The policy may need
only updated tests if its existing use of the reference constants is sufficient.

### Acceptance criteria

- The exact updated runtime and creation version pass the intended support
  checks; old or unsupported versions remain explicitly unsupported.
- Calibrated components still decline non-reference identities/settings as
  required. An assembly mismatch is not accidentally promoted into a new
  global coexistence restriction.
- The observed method fingerprint and reference fixture agree with a capture
  from the original updated assembly. Both assembly and method hash provenance
  are explicit, with no fabricated or placeholder digest treated as measured.

## Story 2: recalibrate the affected starter resources

As a player comparing fresh starts, I want oil and other affected resource
judgments to retain their established meaning after generation changes.

### Implementation

Change only these four ranges in `ConclusionDefinition`:

| Component | Previous lower / upper | Proposed lower / upper |
| --- | ---: | ---: |
| Iron amount | 9,151,265 / 26,773,650 | 9,100,885 / 26,773,650 |
| Coal amount | 9,495,641 / 10,938,129 | 9,539,996 / 10,938,129 |
| Oil amount | 1,196,959 / 1,304,446 | 1,419,033 / 1,511,204 |
| Oil groups/wells | 17 / 19 | 20 / 22 |

Amounts are native amount units. These values come from the original 96-seed
cohort, `seed(i) = (45772 + i * 982451653) mod 100000000`, `i = 0..95`,
using nearest-rank positions 24 and 72. They are not fitted to the four offline
examples. Retain all other bands, including the common finite-resource total
`[74,788,292, 105,667,431]`, and existing applicability guards.

Keep the native oil multiplier `0.00004` and existing player-facing rate
conversion. Generated supply includes underwater wells. Preserve that meaning
in the relevant evidence/presentation documentation; do not promise immediate
extractor access or add accessibility classification from group positions.
Measured changes to other individual deposits are native outputs, not reasons
to force old seed quantities or recalibrate unrelated fixed thresholds.

### Acceptance criteria

- Focused evaluator tests cover `L-1`, `L`, `U-1` and `U` for each changed
  increasing range: below `L` is limited, `[L,U)` is preference-sensitive,
  and `U` supports. Optional preferences still cannot reverse neutral outcomes.
- Evaluating the retained new cohort with the new bands reproduces oil amount
  counts of 25 supports / 48 preference-sensitive / 23 limited, and oil wells
  of 34 / 62 / 0. Applying the old bands would incorrectly yield 96 supports
  for each component. No new full cohort generation is needed for this check.
- Unchanged bands and the existing oil-rate conversion remain unchanged.
  The new bands do not broaden calibrated claims to other star counts or
  resource settings.

## Story 3: preserve native names through preview and cache reuse

As a player changing the native naming option, I want every displayed star
and planet designation to match that preview, including after a cache hit.

### Implementation

- Copy the exact integer `GameDesc.starNameLCID` into `PreviewScanRequest`
  when `DSPSeedScannerPlugin.OnPreviewLoadCompleted` receives the descriptor.
  Carry it into `PreviewGenerationIdentity`, request/identity validation,
  equality and hashing. Keep this a naming input rather than a new condition
  on the calibrated physical metrics.
- Restore the captured value after `SetForNewGame` in
  `DspPreviewGateway.CreateDescriptor`. Require the native member in the
  existing member check and add it to `ci/DSPGame.Reference/RuntimeApi.cs`.
  Do not infer it later from ambient localization, force English, or limit
  supported values to a hardcoded Chinese/default pair.
- Include the exact value in `CompleteClusterCacheKey`'s canonical identity.
  Use separate cache entries for different naming values, preserving existing
  bounded-cache behavior. This is the proposed minimal design; it avoids a
  new layer that rewrites every cached location label.
- Preserve the input when reconstructing cached source identities and checking
  Peace/Combat reuse. Reuse across Peace/Combat remains permitted only when
  all other required inputs, including naming, match. The active preview still
  supplies its own mode/status metadata.
- Let a naming change retire obsolete preview work through the existing
  lifecycle. Audit session deduplication and publication as well as disk cache
  lookup, so the previous mode cannot supply stale labels after a toggle.
  Native naming changes already call `SetStarmapGalaxy`; retain the existing
  hook unless integration evidence demonstrates a missing transition.
- Retain cache schema `13` if the change only adds canonical key content and
  restores the naming field from that validated key. The serialized canonical
  identity, game identity and definition version already separate old entries.
  A binary payload change would require the corresponding schema revision;
  do not add such a change unnecessarily. No blanket cache purge or migration
  of old labels is required.

Primary surfaces are the Plugin request/gateway, Runtime request and preview
identity, `PreviewResolutionCoordinator`, `CompleteClusterConclusionCache`,
the CI game reference and focused tests. Audit any diagnostic identity output
that claims to reproduce a preview so it also records the naming input.

### Acceptance criteria

- An otherwise identical request with LCID `0`, `2052`, or `1033` retains its
  exact value end to end. Different values have different preview/cache
  identities even where native names happen to be equal.
- Native and scanner labels agree for default and Chinese naming on the
  retained naming fixtures, while their measured physical facts agree.
- Fresh scan, same-mode cache hit, switch to another naming mode, and switch
  back all show the correct labels. This covers the home system, rare-resource
  candidates, unipolar and gas locations, and notable stars.
- Cache round-trips retain naming in source identities; same-mode Peace/Combat
  reuse still works, different naming modes cannot reuse stale names, and
  old-version/old-definition entries miss without a destructive cache reset.
- Toggling during pending raw work or leaving/re-entering preview cannot publish
  an obsolete session. Existing cancellation, failure and state-restoration
  guarantees remain intact.

## Story 4: verify the integrated mod and update its contracts

As the maintainer, I want a small, reviewable change supported by reproducible
checks and clear limits, without repeating completed research.

### Implementation and validation sequence

1. Recheck the source baseline and the exact target runtime identity before
   implementation. If the installed build differs, do not silently transplant
   this calibration. Capture the method digest required by Story 1 and reuse
   the existing evidence when its inputs match.
2. Implement the stories with focused Core and Runtime tests, then run the
   existing test executables and Release solution build, plus the Plugin build
   against actual updated game references and the existing CI reference build
   path. The earlier unchanged-source bootstrap passed 15 Core and 95 Runtime
   checks; those are historical baselines, not validation of the future patch.
3. Exercise the changed production adapter in the existing private harness:
   the retained four-seed default-name comparison, at least one retained
   Chinese naming fixture, fresh/cache parity, naming transitions, and
   cancellation/failure restoration. Preserve the updated native resource
   outputs and current mod presentation. Reuse full-cohort and algorithm
   coverage evidence unless inputs/path changes or a discrepancy justify
   repeating those expensive probes. The offline utility's 32-worker results
   do not imply a new worker-count requirement for the mod.
4. Update the affected authoritative contracts:
   [generation identity](../specification/GENERATION-IDENTITY.md) (target runtime
   and naming), [predicate/range catalogue](../specification/PREDICATE-RANGE-VALIDATION.md)
   (definition, endpoints and evidence provenance), and [cache documentation](../CACHE.md)
   (naming separation and reuse). Update [conformance](../CONFORMANCE.md)
   and user-facing compatibility/release notes where the
   implemented behavior changes their claims. Clarify generated oil supply in
   the existing presentation/evidence contract only where needed. Preserve
   dated historical evidence as historical; do not globally replace old
   version strings or rewrite archives. Record implementation authorization
   and subsequent work/acceptance state only in `PROJECT.md`.
5. Prepare the normal local package candidate using existing packaging checks
   when implementation is authorized. Ensure only intended mod outputs enter
   it, with no private harness, native dependency copies or investigation data.
   Product release numbering and publication remain subject to the existing
   release procedure and explicit owner direction.

### Owner-controlled game acceptance

Before release acceptance, exercise installed BepInEx startup and the actual
New Game panel on the target build. Cover fresh and cached scans, the native
naming toggle in English and Chinese, a toggle during pending work, preview
exit/re-entry, and Peace/Combat reuse with matching naming. Check displayed
names against the starmap and inspect the panel after a resolution change and
the native confirmation popup. A rendering change is required only if this
check finds a regression.

These checks require a separately authorized game/deployment session or owner
execution. Private-player evidence and successful compilation do not establish
installed lifecycle or visual behavior. Report unperformed checks as skipped;
do not equate technical checks with owner acceptance or publication.

### Delivery criteria

The implementation is technically ready for owner review when the changed
identity, calibration, naming and cache behavior pass their focused tests,
affected builds and bounded runtime checks, documentation matches the result,
and the final diff contains only the required changes. Any remaining installed
game acceptance must be stated explicitly in `PROJECT.md` and the handoff.
Commit, push and publication require their own explicit instructions.

## Scope boundaries

This plan does not replace native generation, introduce a new compatibility
framework, expand supported settings, redesign conclusions or presentation,
or transfer offline HTML/reporting, private loading/patching, cache namespaces
or worker policies into the mod. The mod already owns the latest conclusion
and presentation standards used to update the offline tool.

Vegetation transplantation/harvesting, post-start terrain and blueprint edits,
factories, logistics and technology queues do not require scanner changes on
the retained evidence. Dark Fog Lens does not enter the current calculations;
sphere geometry and luminosity remain distinct from receiver-efficiency
predictions. Underwater-oil accessibility and arbitrary mod coexistence are
not newly promised. Additional work belongs in this maintenance change only
when a focused check demonstrates that it is required for the stated result.
