# Panel preview check

[PROJECT.md](PROJECT.md) owns handoff and acceptance status. This check is for
the owner only; the implementer does not install or run DSP.

[Download candidate and reports](https://nightly.link/shytamir/DSPSeedScanner/actions/runs/35429534551/DSPSeedScanner-1.3.110.zip).
The download contains the installable `packages/DSPSeedScanner-1.3.110.zip`,
build identity and verification reports. No build or package assembly is needed.

- Version: **1.3.110**; source: `4726a344ebe9e5b3fa40ab5046a05cb24d3f9b6d`.
- Installable package SHA-256:
  `0ca31a412208b319f740665781d198c7a183b510549ac6da9e5e8155376a1323`.
- Outer Actions download SHA-256:
  `ed288579e494996f3989ddecdaaf824ede277fd208ac35035fbb1d4b48ef864b`.

1. With DSP closed, open the installable ZIP. Copy its three DLLs
   (`DSPSeedScanner.dll`, `DSPSeedScanner.Core.dll`, `DSPSeedScanner.Runtime.dll`)
   from `BepInEx/plugins/DSPSeedScanner/` into the same scanner folder in your
   existing active BepInEx installation/profile, replacing the scanner DLLs.
   Leave other mods and configuration alone. Launch normally and open
   **New Game**: seed **16315224**, **64 stars**, **1x resources**, default
   Combat settings, DSP **0.10.34.28529**. Stay in preview; do not start
   gameplay or load an existing save.
2. Wait for the scan to finish, then scroll both panels once using the cues
   below. Check that text, colors and added columns are readable without
   overlap or clipping. No measurements or extra seeds are needed.
3. Return to the menu. Reply **Pass for 1.3.110**, or name the mismatched
   section. Send a screenshot only if something is wrong.

| Where to look | Expected cue |
| --- | --- |
| Conclusions: Fresh start | One “Starter gas giant supplies low Deuterium” limitation. Solar and wind entries name the two sibling moons, not the home planet (four power entries altogether). |
| Conclusions: Megafactory / Sphere | Megafactory uses resource-system guidance; “many rares” and its duplicate contained-orbit advice are absent. Contained-orbit advice belongs to Sphere / energy. Qualifying resource/sphere cards appear only when candidates exist. |
| Statistics: Home system | Three moon rows have a second line, “Moon 1”, “Moon 2”, “Moon 3”; the giant does not. Oil uses a two-decimal `/s` rate plus wells. Fire Ice/Spiniform fragments are green if present; a contained home row is wholly green. |
| Statistics: Cluster | “Nearest Rich Deuterium Gas-Giant” is the heading. Rare-resource and Unipolar highlights remain readable and confined to their relevant cells. A qualifying nearby Deuterium candidate need not exist. |
| Statistics: Notable stars | Blue giants are wholly green at the top, brightest first; the brightest star follows unless already in that group, then the remaining O stars. Distance, Max Sphere and Contained columns are visible. Aquatica rows, if present, come last with empty Luminosity/Contained cells. |

The seed's three home-giant moons and Deuterium starter are supported by an
[earlier native preview capture](specification/PANEL-EVIDENCE.md#data-routes-and-verification-inputs).
It selects the recipe; it is not an observation of this candidate. Conditional
highlights need not all appear in this seed; numeric edges and rare combinations
are covered by automated checks, not additional owner work.

Technical evidence: Core **15/15** and Runtime **95/95** checks passed;
solution and native-reference builds had zero warnings/errors.
[Actions run 110](https://github.com/shytamir/DSPSeedScanner/actions/runs/35429534551)
built and verified this package. Its downloaded digest, DLL versions and ZIP
contents were checked locally. These checks do not establish Unity rendering
or in-game compatibility. The implementer did not run/deploy DSP, change its
environment or access saves.
