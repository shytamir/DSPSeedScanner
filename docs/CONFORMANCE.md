# Scanner Core Conformance Record

[PROJECT.md](PROJECT.md) is the sole authority for project steering and work
status, including historical dispositions. This document contains scope,
requirements, or technical evidence; it does not track status.

This record distinguishes pure checks, compilation, and evidence collected by
executing the scanner inside an isolated supported Dyson Sphere Program
runtime. It establishes conformance for the presentation-neutral core only.
Its evidence excludes product approval, package replacement, other game
builds, and modded generators.

## Unverified-identity scanning

Definition `0.7.0` retains the calibrated values and changes identity drift to
an advisory. Release solution and installed-game-reference plugin builds passed
with zero warnings/errors; Core checks passed 16/16 and Runtime checks 98/98.

Focused deterministic cases exercise older/newer game versions across preview,
single-planet and birth-system coordinators, and complete fresh/cache flows
for changed version, assembly hash, method hash, algorithm and theme order.
Identical fixture evidence retains identical conclusions and displays resource
statistics under its actual identity. Each changed fingerprint triggers a fresh scan, while
same-identity reloads reuse results with the notice intact. Both panel models
retain notices through scanning/cache states, clear them on verified-session
replacement/exit, and reject stale updates. Missing members, mismatched
creation versions, failed/incomplete/cancelled scans, obsolete scanner definitions and
existing star-count/resource-setting limits retain their failure boundaries.

Source review and game-linked compilation cover the shared renderer's red
border and red, wrapped warning above each scroll viewport. No installed-game
visual check, native generation sweep or recalibration was performed; these
checks do not establish arbitrary future-game compatibility or owner acceptance.

## DSP 0.10.35.29088 identity check

Read-only inspection on 2026-09-24 found DSP `0.10.35.29088` as the latest
entry in the installed `Updates/Versions.txt`, with Steam build `25503975`.
The original installed `Assembly-CSharp.dll` SHA-256 was
`C43A484F6ADF8A9E4B956156047070891B46860D5B5C707BA1377B6A2AF25732`.
Its combined generation-method SHA-256 was
`8E2B97B2C37DD9A3E1E441B0FCE0FE6ADAAAF8FF84EFF00A9F69E9CCEDE257ED`.
Capture used the original PE method bodies in production order: 948 bytes for
`UniverseGen.CreateGalaxy`, then 161 for
`PlanetData.RegenerateRawDataImmediately`, each preceded by its UTF-8
`DeclaringType.MethodName\n` label.

The identity-only source update passed Release solution and installed-game-
reference plugin builds with zero warnings/errors, 16/16 Core checks and
96/96 Runtime checks. These are compilation and deterministic checks, not
native generation or gameplay validation. No calibration cohort, native scan
or installed-game UI check was run for this hotfix; the dated evidence below
retains its original runtime identity.

## DSP 0.10.35.29057 evidence

The updated native assembly was rechecked on 2026-09-23 with SHA-256
`E75D3FE4B6A9CA822766189F826BA3A8348DFB7E301AA37FF6779DB29A83FD8D`.
Its combined generation-method SHA-256 is
`DAA3676DB1CCE742391387A75B8BCECCD02AEC0197A7C0BFF00A5022DB0B665E`.
The capture reads raw method bodies from the original PE metadata: 948 bytes
for `UniverseGen.CreateGalaxy`, then 161 for
`PlanetData.RegenerateRawDataImmediately`, each preceded by the production
UTF-8 `DeclaringType.MethodName\n` label. This is the production hash format,
not the investigation's resolved-IL comparison or a renamed private DLL hash.

The updated identity/calibration checkpoint passed Release solution and
game-linked Plugin builds with zero warnings/errors, 16 Core checks and
95 Runtime checks. The compiled evaluator reproduced the retained 96-seed
oil distributions recorded in the [predicate catalogue](specification/PREDICATE-RANGE-VALIDATION.md).
The initial sandbox build could not read Windows SDK discovery metadata;
the build with normal SDK access passed without a source workaround.

The naming/cache checkpoint additionally passed 96 Runtime checks (including
exact LCID identity, request mismatch rejection, cache round-trip and
Peace/Combat reuse, fresh/same-mode/switch-back labels, pending-work replacement
and preview exit/re-entry). Core remained 16/16. Release solution, game-linked
Plugin and hosted-reference Plugin builds passed with zero warnings/errors.
These deterministic naming fixtures exercise lifecycle and cache behavior;
the following private-player checks supply the separate native label evidence.

The updated production coordinators, compatibility policy, native gateway and
terrain-worker session were exercised in the existing private Unity player.
The original native DLL and exported current catalogue were reused. The
player supplied its own BepInEx paths, configuration and cache; it did not
launch `DSPGAME.exe`, attach to the installed game, or access a save.

| Check | Observed result |
| --- | --- |
| Runtime fingerprint | Production capture matched both assembly and method digests above, including the new required native member. |
| Native naming | Seeds `45772`, `73339583`, `96178012`, `16315224`, `61571387`, each with LCID `0`, `2052`, `1033`: all 960 star labels and 3,144 solid-planet target labels matched native generation; home labels and request diagnostics also matched. |
| Full default-name scans | Seeds `29519403`, `32395590`, `25064027`, `42424242`: 852/852 solid planets completed. Every projected statistic and conclusion matched the retained updated-game/mod-1.4.114 baseline, excluding session ID and cache flag. |
| Same-mode cache | All four complete semantic reports matched their fresh results exactly after those same two exclusions. |
| Chinese naming and reuse | Seed `29519403`: another 207/207 planets completed; physical home-resource payload matched default naming. Native labels matched fresh and cached results, including cached rare/unipolar locations. Same-LCID Peace/Combat reuse retained source LCID and source mode; switching back reproduced the default report exactly. |
| Replacement and cleanup | LCID `1033` did not reuse LCID `0` despite equal wording. Switching during pending work cancelled the old session; exit/re-entry reused the correct cache. Injected raw failure failed closed. Borrowed game-data/descriptor sentinels were restored after yields and terminal paths; incomplete work was not cached. |

Evidence is in the shared untracked update directory's `implementation-run3/`:
`integration.tsv`, `comparison.json`, `assemblies.json`, and the projected JSON
reports. The first two integration attempts stopped on harness setup/identifier
parsing errors and are not passing evidence. The probe build was pinned to its
declared assembly references after it initially found older copies in its
player directory; no production workaround was introduced.

### Candidate 1.4.119

Hosted [run 119](https://github.com/shytamir/DSPSeedScanner/actions/runs/35921054961)
built source `13f33bdeeeffc17c6719006d42216a3fb9223d5c` as `1.4.119`,
assembly/file version `1.4.119.0`, release label `1.4.119.13f33bd`.
Its downloaded package SHA-256 is
`5793A116A8C40B7E1E2601338F21BAF28C272AEE375AED9709727AC69A9579A9`.
The existing build-artifact and package validators passed locally. Generated
version-source validation used an ignored temporary copy, leaving the tracked
local-development version defaults intact.

The exact downloaded three candidate assemblies also passed the private native
check: all 15 naming cases, fresh/default and Chinese scans of seed `29519403`
(414 complete planets), baseline/cache/switch-back parity, Peace/Combat reuse,
pending replacement, exit/re-entry and injected-failure restoration. Those
records are in `candidate-1.4.119/` beside `implementation-run3/`.

The three installed scanner DLLs were replaced while DSP was stopped and their
hashes matched the validated candidate. The prior `1.4.114` DLLs and a before/
after hash manifest are retained in `before-installed-1.4.119/` under the shared
update directory. No configuration or cache purge was performed.

These automated checks do not establish installed BepInEx startup, the actual
New Game UI, Chinese interface rendering or resolution confirmation.
The owner-controlled recipe is in the
[scope document](archive/GAME-UPDATE-0.10.35.29057.md#owner-controlled-game-acceptance);
The owner's reported execution results and acceptance are recorded only in
[PROJECT.md](PROJECT.md).

The preceding updated-game private-player investigation covered 25 native
versus worker targets spanning algorithms 1-13, 24 settings comparisons and
success/cancellation/failure state restoration. Its evidence remains under
`D:\Shy\Shared Untracked Repo Resources\DSPSeedScanner-Update-0.10.35.29057`.
These observations do not establish installed BepInEx startup or UI acceptance
for the changed mod. The historical installed-runtime record below is retained
under its original identity and does not substitute for updated-game testing.

## Historical supported identity

The passing runtime was DSP `0.10.34.28529`, galaxy algorithm `20200403`,
Assembly-CSharp SHA-256
`AE0BA95F75BD879A62AA4CE253B2AB78EAA4FB3C7C595F5E1FEE75EBE0E0EF85`,
ordered theme IDs `1` through `25`, the accepted scanner contract versions,
and no other loaded BepInEx plugins or preloader assemblies. The loaded IL for
`UniverseGen.CreateGalaxy(GameDesc)` and
`PlanetData.RegenerateRawDataImmediately()` had combined SHA-256
`A0CC806F17FD8A88468AA8CF05CDD4C1A8728A33BA1A4C0FA967C2EF50775C9B`.

In the IMPL-08 probe, any exact identity mismatch, missing required member,
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
