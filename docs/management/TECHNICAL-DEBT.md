# Technical Debt Register

**Status:** No active entries. TD-001 and TD-002 were closed by IMPL-08 on
2026-08-11.

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
replacement remain separately scoped to later work.

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
