# Scanner Core Conformance Record

**Status:** Accepted with IMPL-08 on 2026-08-11; packaging was subsequently
implemented by IMPL-09 without changing the conformance result.

This record distinguishes pure checks, compilation, and evidence collected by
executing the scanner inside an isolated supported Dyson Sphere Program
runtime. It establishes conformance for the presentation-neutral core only.
IMPL-08 itself did not approve the product or replace the then-current dummy
package, and it made no claim for another game build or modded generator.

## Supported identity

The passing runtime was DSP `0.10.34.28529`, galaxy algorithm `20200403`,
Assembly-CSharp SHA-256
`AE0BA95F75BD879A62AA4CE253B2AB78EAA4FB3C7C595F5E1FEE75EBE0E0EF85`,
ordered theme IDs `1` through `25`, the accepted scanner contract versions,
and no other loaded BepInEx plugins or preloader assemblies. The loaded IL for
`UniverseGen.CreateGalaxy(GameDesc)` and
`PlanetData.RegenerateRawDataImmediately()` had combined SHA-256
`A0CC806F17FD8A88468AA8CF05CDD4C1A8728A33BA1A4C0FA967C2EF50775C9B`.

At IMPL-08 acceptance, any exact identity mismatch, missing required member,
other plugin, preloader assembly, or changed generation-method digest was
rejected before evaluation. The later 1.0 coexistence correction retained
those observed values as cache identity but stopped treating plugin presence
or generation changes as incompatibility. Unsupported game versions and
missing required members remain rejected; runtime failures and incomplete
evidence still publish no complete result. The accepted isolated probes do not
prove compatibility with every plugin combination.

## Validation layers

| Layer | What passed | What it establishes |
| --- | --- | --- |
| Pure core suite | 14 deterministic conclusion checks | Accepted outcomes, thresholds, endpoints, preference range, tradeoffs, unknowns, not-applicable results, attribution, independence, and prohibited proxies |
| Pure runtime-boundary suite | 29 identity, member, plugin, patcher, catalogue, enum, raw failure, cancellation, partial coverage, busy, cleanup, bounds, and no-game-object checks | Fail-closed orchestration and normalized boundary behavior without claiming DSP execution |
| Release compilation | Solution and game-linked plugin, with zero warnings | Source compatibility with the selected toolchain and installed references |
| In-game probes | Preview, raw algorithms, birth-system resources, complete-cluster rare access, failure, cancellation, busy, cleanup, and isolation | Actual behavior in the supported DSP and BepInEx lifecycle |
| Generic package validation | Semantic-versioned real scanner package | Archive structure, scanner-owned DLL integrity, and exclusion of external runtime assemblies |

Compilation and pure tests did not substitute for the in-game probes.

## Runtime gate evidence

| Required gate | Recorded result |
| --- | --- |
| Compatibility fingerprint | The supported identity passed. Deliberate game, algorithm, assembly, theme, member, plugin, patcher, method-IL, and request mismatches were rejected before evaluation. A controlled preloader fixture was inventoried as `ControlledPatcherFixture.dll:6D614438824CACE7DB98AD9F68109BE7D4039B731C09CE0781FED2551628267E` and rejected as `generation-patcher-uncertain` without reaching generation. |
| Lifecycle and thread affinity | Generation ran on the captured Unity main thread behind one shared operation gate. Two independent conformance processes covered success, injected post-generation failure, cancellation, and re-entrant busy behavior. All 32 state comparisons passed; every owned galaxy was freed and every captured lease restored. |
| Reachable raw algorithms | Raw algorithms `1` through `13` were exercised through DSP's selected preparation and generation path. Exact normalized repetition and explicit failure behavior were recorded during IMPL-05. |
| Full-cluster repeat and cleanup | Seeds `73339583`, `96178012`, and `45772` repeated exactly across independent processes with 218, 196, and 216 solid planets. Cancellation stopped at `3/218`; injected incompatibility stopped at `1/218`; neither exposed conclusions or candidate objects. |
| Derivation parity | Light-year conversion and maximum-shell rounding matched the supported runtime boundary fixtures established in IMPL-04. |
| Compatibility failure paths | Missing members, changed catalogues, unknown resource enums, raw planet failure, incomplete coverage, and altered settings retained seed, stage, provenance, subject, component independence, and explicit unknown or not-applicable outcomes. |

The controlled preloader rejection in the table above records the IMPL-08 gate
as it was executed. Under the 1.0 coexistence policy, plugin inventory and
observed generation changes are accepted and isolate cache entries instead of
rejecting the scan.

The 1.0 coexistence correction was then validated in the normal multi-plugin
installation on 2026-08-12. One cache-miss scan completed, one cache hit was
reused, preview replacement retired obsolete work, and preview exit hid and
retired the active session. This validates that installed combination; it does
not claim compatibility with every possible plugin set.

The two independent IMPL-08 conformance records were byte-identical with
SHA-256
`7ACF7AD82CB1A17C1C759922F92A6584F6DE5FAEC67C5CC0CD5A0FC7BACBF09A`.
For seed `16315224`, success and the re-entrant outer request each returned 374
reports. Injected failure returned `Failed/runtime-exception`, cancellation
returned `Cancelled/cancelled`, and the inner request returned `Busy/busy`;
all three returned zero reports and preserved `galaxy-preview` attribution.

Earlier accepted independent-process normalized evidence remains part of this
record: preview SHA-256
`CDD47CDF2142FBBD494EB19DE108A93142FEA38E0667712873492233EB59A969`,
raw-algorithm SHA-256
`47DC2C493A02FAB0E249E934C6E96D094520C427860029ED82484F43BBCE81E8`,
birth-system SHA-256
`8CFDA61B9A356C80F2C38E7D4B61F3634B0169BE1F38F8C5C7D6D1B0310F2E98`,
and complete-cluster SHA-256
`B67A4D824DD784A8D0FE53156E6172FD76974147FDA8C26E17A1B44CE94C8936`.

## Enforced operating bounds

- All runtime generation is serialized; a concurrent or re-entrant request is
  rejected busy.
- Preview is limited to one explicitly requested, supported 64-star identity.
- Birth-system raw work declares coverage and advances between solid planets,
  where cancellation is safe.
- Complete-cluster work rejects more than 256 declared solid planets before
  raw generation and otherwise processes planets sequentially.
- Partial or failed raw coverage publishes no complete-scope conclusion.

Observed complete-cluster operations took 23,670 to 28,143 ms. Peak temporary
managed heap growth was 1,295,376,384 bytes; developer-only post-collection
retention was at most 2,510,848 bytes including returned results and progress.
These measurements justify the current single-operation bound but are not
performance guarantees and do not authorize queues or parallel scans.

## Isolation and residual limits

The in-game conformance harness used isolated non-null sentinels and compared
`GameMain.data`, `DSPGame.GameDesc`, and the captured description, galaxy,
factories, factory count, history, and statistics fields before and after each
applicable path. It did not load or mutate a player save. Exact probe outputs,
controlled fixture binaries, copied game-linked builds, and scan data remain
outside the repository.

Conformance is local to the recorded runtime identity. Cross-machine byte
equivalence, another DSP build, generation-altering mods, batch scanning,
parallel generation, player-facing controls, the New Game panel, publication,
and the real Thunderstore package remained outside IMPL-08. IMPL-09 later
implemented packaging without changing this runtime evidence.
