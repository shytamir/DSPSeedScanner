# Technical Debt Register

**Status:** Active.

This register records deliberately deferred engineering obligations that are
too important to disappear into story prose. A debt entry does not weaken the
accepted product contract or convert missing evidence into a passing result.
Each entry states the temporary operating constraint and the gate that must
close it.

## TD-001: Complete non-success runtime isolation probes

**Introduced:** IMPL-03 acceptance review, 2026-08-11.

**Deferred obligation:** Prove from the intended BepInEx lifecycle that shared
New Game, active save, factory, and progression state is unchanged after an
injected post-capture runtime failure and safe-boundary cancellation. Record a
re-entrant busy rejection in the same harness.

**Current evidence:** The isolated DSP probe proved the supported success path,
main-thread call sequence, `GalaxyData.Free`, and successful state restoration.
Focused tests proved failure, cancellation, and busy orchestration against a
fake runtime gateway, and code inspection confirmed that every post-capture
exit uses the same restoration block. Those checks do not replace the missing
in-game failure and cancellation evidence.

**Risk while open:** A DSP-specific side effect on a non-success path could
escape the pure harness and remain undiscovered.

**Temporary constraint:** Runtime operations remain developer-invoked and are
not release-ready. Later implementation may reuse the boundary, but may not
cite non-success in-game isolation as proven.

**Closure evidence:** In an isolated supported runtime, capture before-and-after
state identities, inject a failure after state capture, cancel at a supported
boundary, attempt a re-entrant request, and show cleanup, unchanged tracked
state, and precise seed-and-stage results for every case.

**Required by:** IMPL-08 acceptance. It also blocks player-facing invocation
and replacement of the dummy package.
## TD-002: Detect preloader and in-memory generation patch uncertainty

**Introduced:** IMPL-03 acceptance review, 2026-08-11.

**Deferred obligation:** Extend the compatibility fingerprint beyond ordinary
`Chainloader.PluginInfos` entries and the on-disk Assembly-CSharp hash so a
custom BepInEx preloader patcher or equivalent known in-memory generation patch
cannot pass as the supported runtime.

**Current evidence:** The adapter rejects every other ordinary loaded BepInEx
plugin and exact mismatches in game version, galaxy algorithm, assembly hash,
ordered themes, required members, and scanner contract versions. Policy tests
prove rejection when patch uncertainty is reported; they do not prove that the
adapter discovers preloader patch uncertainty.

**Risk while open:** A patched in-memory generator could retain the accepted
on-disk identity and produce a conclusion under a false supported fingerprint.

**Temporary constraint:** Only the controlled isolated runtime used by project
probes is verified. No broader modded-runtime compatibility claim is allowed.

**Closure evidence:** Add a deterministic patcher or loaded-patch inventory to
the fingerprint, conservatively reject unrecognized entries, and demonstrate
the rejection with a controlled patcher fixture without weakening the exact
supported-runtime case.

**Required by:** IMPL-08 acceptance. It also blocks player-facing invocation
and replacement of the dummy package.
