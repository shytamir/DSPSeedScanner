# Release Candidate 1.0.85 Validation

**Status:** Approved by the owner on 2026-08-14 after final human validation.

This record identified the single candidate that established the roadmap's
**Release candidate ready** milestone. Later documentation-only builds did not
supersede it.

## Candidate identity

| Field | Value |
| --- | --- |
| Version | `1.0.85` |
| Source commit | `ab1ad34f9a7d08a56455a5b92ad077f1b51ff0f9` |
| CI run | [Build 85](https://github.com/shytamir/DSPSeedScanner/actions/runs/31782121233) |
| Package | `DSPSeedScanner-1.0.85.zip` |
| Reconstructed package SHA-256 | `D84D9DA65BF386008DEAA17041AA6862DC1713B124CD6EEA70A08EECE62E7DEC` |
| Supported game build | `0.10.34.28529` |

The installed candidate DLLs reported assembly and file version `1.0.85.0`
and product version `1.0.85.ab1ad34`. Their SHA-256 values were:

- `DSPSeedScanner.dll`:
  `1A9DCA99F87BA03B036CD747C93FE598B2DAACC96A4E8BF0E8B7AC9C7988D8D4`
- `DSPSeedScanner.Core.dll`:
  `780F9F4887E4B783ABB753B2ABEC14D16431BF055BFED6BB91CC8EA3CE12AC91`
- `DSPSeedScanner.Runtime.dll`:
  `5F5BBBFC41A74D19D5197074936327DF6D65935698300223945657FD8FD34584`

## Evidence

- Build 85 completed the release build, 14 core checks, 82 runtime checks,
  hosted-reference plugin build, build-artifact validation, Thunderstore
  package construction, and package validation successfully.
- A clean local release build of the candidate source completed with no
  warnings or errors; the same 14 core and 82 runtime checks passed.
- An independent package reconstruction used the exact installed candidate
  DLLs and repository packaging scripts. Build-artifact and Thunderstore
  validators passed the DLL allowlist, non-empty payload, semantic, assembly,
  file, and product versions, dependency boundary, manifest, README, icon, and
  package-content checks. The resulting package was 196,489 bytes and produced
  the reconstructed-package hash recorded above. GitHub permitted inspection
  of Build 85's successful steps but required authentication to download its
  uploaded artifact directly; the installed DLL hashes therefore provide the
  exact binary identity independently of the reconstruction.
- The exact candidate loaded under BepInEx `5.4.17` with the normal three-mod
  development set: DSP Seed Scanner, DSP Mirror Blueprint, and DSP Guide
  Check. Its captured runtime log contained no warning, error, or exception.
- Owner validation accepted FEED-10 on this candidate after confirming the
  completed statistics panel and its Notable stars subsection. Runtime
  fixtures covered cache miss and hit, Peace and Combat reuse, Dark Fog toggle
  reuse, seed replacement, preview exit, return to a prior seed, scrolling,
  every supported corner pairing, cancellation, and multi-plugin inventory.
  The earlier passed seven-step installed-game procedure served as the human
  regression baseline for those lifecycle paths.

## Release boundary

No release-blocking residual was identified. TD-004 concerns containment for a
developer-only probe write and does not affect ordinary player operation. A
Unity player build, scene validation, and prefab validation are not applicable
to this BepInEx package.

The **Release candidate ready** milestone passed for the exact candidate above.
The owner then completed final human validation against that candidate and
explicitly accepted it, passing the **Release candidate approved** milestone.
