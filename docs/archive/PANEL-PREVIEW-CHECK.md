# Panel Preview Check — Historical Recipe

[PROJECT.md](../PROJECT.md) owns handoff and acceptance state. This note
preserves the recipe supplied for the owner-only panel preview. It is not a
request to repeat validation or install an older build.

The [original Actions artifact](https://github.com/shytamir/DSPSeedScanner/actions/runs/35429534551)
contained `packages/DSPSeedScanner-1.3.110.zip`, identity and verification
reports. Artifact availability is subject to CI retention.

- Version: **1.3.110**; source: `4726a344ebe9e5b3fa40ab5046a05cb24d3f9b6d`.
- Installable ZIP SHA-256:
  `0ca31a412208b319f740665781d198c7a183b510549ac6da9e5e8155376a1323`.
- Outer Actions download SHA-256:
  `ed288579e494996f3989ddecdaaf824ede277fd208ac35035fbb1d4b48ef864b`.

The supplied instructions asked the owner to:

1. Close DSP and replace only the scanner's three DLLs from the package's
   `BepInEx/plugins/DSPSeedScanner/` folder in the existing active installation
   or profile. Other mods and configuration were to remain untouched. The
   recipe was New Game preview, seed **16315224**, **64 stars**, **1x resources**,
   default Combat settings, DSP **0.10.34.28529**, without gameplay or save use.
2. Wait for the scan, scroll both panels once and compare the cues below for
   readability, overlap and clipping. Measurements and extra seeds were not
   requested.
3. Return to the menu and reply **Pass for 1.3.110**, or name a mismatched
   section. A screenshot was requested only for a problem.

| Section | Cue supplied for the check |
| --- | --- |
| Fresh start | One low-Deuterium starter limitation; independent Solar/Wind entries for two sibling moons, without home-planet power advice: four entries. |
| Megafactory / Sphere | Resource-system guidance, no Many rares or duplicate Megafactory containment; containment under Sphere / energy; cards conditional on qualifying candidates. |
| Home system | Moon 1/2/3 second lines, none on the giant; two-decimal oil `/s` plus wells; green Fire Ice/Spiniform fragments when present and a wholly green contained-home row. |
| Cluster | Nearest Rich Deuterium Gas-Giant heading; readable rare/Unipolar highlights limited to relevant cells; a qualifying nearby giant was not guaranteed. |
| Notable stars | Wholly green blue giants first/brightest first, brightest star next unless already included, then remaining O stars; visible Distance/Max Sphere/Contained; Aquatica last with empty Luminosity/Contained. |

An [earlier native capture](../specification/PANEL-EVIDENCE.md#data-routes-and-verification-inputs)
supported the seed's three home-giant moons and Deuterium starter. It supplied
recipe selection, not a candidate observation. Conditional highlights were not
all required in that seed; numeric edges and rare combinations were covered by
automated checks, without extra owner work.

Technical evidence accompanying the recipe comprised Core **15/15**, Runtime
**95/95**, zero-warning/error solution/native-reference builds and the successful
[Actions run 110](https://github.com/shytamir/DSPSeedScanner/actions/runs/35429534551).
Downloaded digest, DLL versions and ZIP contents were checked locally. These
checks did not establish Unity rendering or in-game compatibility. The
implementer did not run/deploy DSP, change its environment or access saves.
