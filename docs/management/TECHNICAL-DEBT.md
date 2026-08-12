# Technical Debt Register

**Status:** No active entries. TD-001 and TD-002 were closed by IMPL-08;
TD-003 was reviewed and closed during the 0.9 release-candidate pass.

This register records deliberately deferred engineering obligations that are
too important to disappear into story prose. A debt entry does not weaken the
accepted product contract or convert missing evidence into a passing result.
Each entry states the temporary operating constraint and the gate that must
close it.

## TD-001: Complete non-success runtime isolation probes

**Introduced:** IMPL-03 acceptance review, 2026-08-11.

**State:** Closed by IMPL-08 on 2026-08-11.

**Deferred obligation:** Prove from the intended BepInEx lifecycle that shared
New Game, active save, factory, and progression state is unchanged after an
injected post-capture runtime failure and safe-boundary cancellation. Record a
re-entrant busy rejection in the same harness.

**Evidence before closure:** The isolated DSP probe had proved the supported
success path, main-thread call sequence, `GalaxyData.Free`, and successful
state restoration. Focused tests had proved failure, cancellation, and busy
orchestration against a fake runtime gateway.

**Closure evidence:** Two independent supported-runtime processes produced the
same conformance record. Success, an injected post-generation failure,
cancellation, and a re-entrant outer request together passed 32 before-and-
after comparisons across non-null New Game and `GameData` sentinels; the inner
request was rejected busy. All cases retained seed and stage, no non-success
case returned reports, each owned galaxy was freed, and every captured state
lease restored. The record SHA-256 was
`7ACF7AD82CB1A17C1C759922F92A6584F6DE5FAEC67C5CC0CD5A0FC7BACBF09A`.

**Closure gate:** Satisfied for IMPL-08. Player-facing invocation and package
replacement were separately scoped at closure and were completed by later
presentation work.

## TD-002: Detect preloader and in-memory generation patch uncertainty

**Introduced:** IMPL-03 acceptance review, 2026-08-11.

**State:** Closed by IMPL-08 on 2026-08-11.

**Deferred obligation:** Extend the compatibility fingerprint beyond ordinary
`Chainloader.PluginInfos` entries and the on-disk Assembly-CSharp hash so a
custom BepInEx preloader patcher or equivalent known in-memory generation patch
cannot pass as the supported runtime.

**Evidence before closure:** The adapter rejected every other ordinary loaded
BepInEx plugin and exact mismatches in game version, galaxy algorithm, assembly
hash, ordered themes, required members, and scanner contract versions. Policy
tests rejected explicitly reported patch uncertainty.

**Closure evidence:** The fingerprint now hashes the loaded IL bodies of
`UniverseGen.CreateGalaxy(GameDesc)` and
`PlanetData.RegenerateRawDataImmediately()` and inventories every assembly in
the BepInEx patcher directory by filename and SHA-256. The exact supported
method digest is
`A0CC806F17FD8A88468AA8CF05CDD4C1A8728A33BA1A4C0FA967C2EF50775C9B`.
A controlled external patcher fixture was detected and rejected as
`generation-patcher-uncertain` before generation; a focused test separately
rejected a changed in-memory method digest.

**Closure gate:** Satisfied for IMPL-08. Compatibility remains deliberately
limited to the one recorded runtime identity; no general mod compatibility is
claimed.

**Later policy:** The 1.0 coexistence correction retained plugin and preloader
inventory plus observed assembly, algorithm, catalogue, and method identity in
the cache key, but stopped treating plugin presence or generation changes as
incompatibility. Unsupported game versions and missing required members still
reject; undetected plugin interaction remains an accepted risk rather than an
isolation requirement.

## TD-003: Evaluate single-assembly packaging

**Introduced:** IMPL-09 acceptance, 2026-08-11.

**State:** Closed as declined on 2026-08-12.

**Deferred obligation:** Determine whether the three scanner-owned assemblies
can be merged into one delivered `DSPSeedScanner.dll` without collapsing the
source-project boundaries between the plugin adapter, runtime orchestration,
and pure conclusion core.

**Evidence at review:** The accepted three-assembly package loaded and executed
all core operations correctly. Separate assemblies keep the Core and Runtime
projects independently testable without DSP, Unity, or BepInEx. Assembly
merging would be a packaging refinement, not a functional requirement.

**Value if closed by implementation:** One installed DLL reduces visible
package clutter and simplifies manual inspection and removal.

**Cost boundary:** Do not adopt merging if it introduces significant build or
maintenance cost, weakens deterministic builds, changes public report types,
obscures stack traces, breaks BepInEx discovery, or risks including DSP, Unity,
BepInEx, or the CI reference assembly. The existing source and test boundaries
must remain intact.

**Closure evidence:** Either produce a one-DLL package that passes artifact and
archive validation, loads in the supported isolated runtime, and repeats
preview and raw invocation successfully, or document that the demonstrated
cost or risk is disproportionate and close the refinement as declined. The
three-DLL package remains acceptable until that decision.

**Closure decision:** The 0.9 release-candidate review retained the accepted
three-DLL package. Consolidation would add a new assembly-merging or dependency-
resolution step solely to reduce visible package clutter, while risking public
type identities, stack traces, deterministic output, BepInEx discovery, and the
independently tested project boundaries. The validated package contains only
the three scanner-owned assemblies and excludes game, framework, and CI
reference binaries. That cost and release risk were disproportionate to the
cosmetic benefit, so no publication blocker remained.

**Required by:** Satisfied by the 0.9 release-candidate review. The entry did
not block the completed presentation specification or implementation.

Return to the [maintenance roadmap](ROADMAP.md),
[project steering](../PROJECT.md), or the [documentation index](../INDEX.md).
