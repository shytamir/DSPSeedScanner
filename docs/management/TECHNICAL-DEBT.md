# Technical Debt Register

[PROJECT.md](../PROJECT.md) is the sole authority for project steering and work
status, including historical dispositions. This document contains scope,
requirements, or technical evidence; it does not track status.

This register describes engineering obligations, evaluation criteria, and
technical evidence. Their dispositions belong only in
[PROJECT.md](../PROJECT.md). An entry does not weaken the product contract or
convert missing evidence into a passing result.

## TD-001: Complete non-success runtime isolation probes

**Origin:** IMPL-03 runtime-boundary review, 2026-08-11.

**Obligation:** Prove from the intended BepInEx lifecycle that shared
New Game, active save, factory, and progression state is unchanged after an
injected post-capture runtime failure and safe-boundary cancellation. Record a
re-entrant busy rejection in the same harness.

**Initial evidence:** The isolated DSP probe had proved the supported
success path, main-thread call sequence, `GalaxyData.Free`, and successful
state restoration. Focused tests had proved failure, cancellation, and busy
orchestration against a fake runtime gateway.

**Validation evidence:** Two independent supported-runtime processes produced the
same conformance record. Success, an injected post-generation failure,
cancellation, and a re-entrant outer request together passed 32 before-and-
after comparisons across non-null New Game and `GameData` sentinels; the inner
request was rejected busy. All cases retained seed and stage, no non-success
case returned reports, each owned galaxy was freed, and every captured state
lease restored. The record SHA-256 was
`7ACF7AD82CB1A17C1C759922F92A6584F6DE5FAEC67C5CC0CD5A0FC7BACBF09A`.

**Evidence boundary:** These probes concern the runtime boundary; player-facing
invocation and package replacement require separate evidence.

## TD-002: Detect preloader and in-memory generation patch uncertainty

**Origin:** IMPL-03 runtime-boundary review, 2026-08-11.

**Obligation:** Extend the compatibility fingerprint beyond ordinary
`Chainloader.PluginInfos` entries and the on-disk Assembly-CSharp hash so a
custom BepInEx preloader patcher or equivalent known in-memory generation patch
cannot pass as the supported runtime.

**Initial evidence:** The adapter rejected every other ordinary loaded
BepInEx plugin and exact mismatches in game version, galaxy algorithm, assembly
hash, ordered themes, required members, and scanner contract versions. Policy
tests rejected explicitly reported patch uncertainty.

**Validation evidence:** The fingerprint now hashes the loaded IL bodies of
`UniverseGen.CreateGalaxy(GameDesc)` and
`PlanetData.RegenerateRawDataImmediately()` and inventories every assembly in
the BepInEx patcher directory by filename and SHA-256. The exact supported
method digest is
`A0CC806F17FD8A88468AA8CF05CDD4C1A8728A33BA1A4C0FA967C2EF50775C9B`.
A controlled external patcher fixture was detected and rejected as
`generation-patcher-uncertain` before generation; a focused test separately
rejected a changed in-memory method digest.

**Evidence boundary:** The probes cover the recorded runtime identity and do
not establish general mod compatibility.

**Later policy:** The 1.0 coexistence correction retained plugin and preloader
inventory plus observed assembly, algorithm, catalogue, and method identity in
the cache key, but stopped treating plugin presence or generation changes as
incompatibility. Unsupported game versions and missing required members still
reject; undetected plugin interaction remains an accepted risk rather than an
isolation requirement.

## TD-003: Evaluate single-assembly packaging

**Origin:** IMPL-09 packaging review, 2026-08-11.

**Obligation:** Determine whether the three scanner-owned assemblies
can be merged into one delivered `DSPSeedScanner.dll` without collapsing the
source-project boundaries between the plugin adapter, runtime orchestration,
and pure conclusion core.

**Evidence at review:** The accepted three-assembly package loaded and executed
all core operations correctly. Separate assemblies keep the Core and Runtime
projects independently testable without DSP, Unity, or BepInEx. Assembly
merging would be a packaging refinement, not a functional requirement.

**Implementation value:** One installed DLL reduces visible
package clutter and simplifies manual inspection and removal.

**Cost boundary:** Do not adopt merging if it introduces significant build or
maintenance cost, weakens deterministic builds, changes public report types,
obscures stack traces, breaks BepInEx discovery, or risks including DSP, Unity,
BepInEx, or the CI reference assembly. The existing source and test boundaries
must remain intact.

**Evaluation criteria:** Either produce a one-DLL package that passes artifact and
archive validation, loads in the supported isolated runtime, and repeats
preview and raw invocation successfully, or document that the demonstrated
cost or risk is disproportionate. Record the disposition only in
[PROJECT.md](../PROJECT.md).

**Review rationale:** Consolidation would add a new assembly-merging or
dependency-resolution step solely to reduce visible package clutter, while risking public
type identities, stack traces, deterministic output, BepInEx discovery, and the
independently tested project boundaries. The validated package contains only
the three scanner-owned assemblies and excludes game, framework, and CI
reference binaries. That cost and release risk were disproportionate to the
cosmetic benefit.

## TD-004: Contain developer-probe output failures

**Origin:** FSOR-01 scope review, 2026-08-13.

**Obligation:** Decide whether the environment-selected developer
probe output paths warrant a shared bounded-write helper. If implemented, each
probe result receives one write attempt. A failed write is reported once
through concise fallback logging, never retried through the failed path, and
the probe always terminates cleanly while preserving the original failure.

**Implementation value:** Probe failures become deterministic and
retain their useful original diagnostic instead of risking a second escaping
write exception. This improves development and installed-runtime validation;
it does not change ordinary player behavior.

**Cost boundary:** Do not add path discovery, alternate output destinations,
general probe refactoring, or player-facing behavior. The change is justified
only if its small validation-pipeline benefit outweighs touching every probe
writer and adding failure-injection coverage.

**Evaluation gate:** Inventory the remaining direct probe writes and determine
whether one shared helper and focused injected-writer tests can close the
failure pattern without widening production filesystem policy. Close as
declined if the maintenance and test surface is disproportionate.

Return to the [current roadmap](ROADMAP.md),
[project steering](../PROJECT.md), or the [documentation index](../INDEX.md).
