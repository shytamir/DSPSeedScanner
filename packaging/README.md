<p align="center"><img alt="Before you choose a star, let the galaxy tell you what kind of future it holds." src="https://shytamir.github.io/DSP_Guide/assets/images/mod/before-you-choose-a-star-let-the-galaxy-tell-you-what-kind-of-future-it-holds.png"><br><small><em><span style="color:#b8b8b8">Before you choose a star, let the galaxy tell you what kind of future it holds.</span></em></small></p>

# DSP Seed Scanner

DSP Seed Scanner helps you choose a cluster before starting a new game. It
automatically scans the seed shown in the New Game preview and gives you a
simple summary of its strengths, tradeoffs, and limitations.

## Quick start

Install the mod and open the **New Game** cluster preview. That is all you need
to do.

The scanner starts automatically whenever a new cluster preview loads. While it
works, the panel shows its progress. When the scan finishes, scroll through the
results to see how the seed looks for:

- A fresh start
- A megafactory
- Compact expansion
- Dyson sphere construction and energy

Previously scanned seeds load from the local cache automatically.

Fresh-start guidance covers the starter giant, useful Fire Ice deposits,
power on neighboring moons and nearby rich Deuterium supply. Megafactory guidance
highlights systems with useful rare resources and plentiful deposits;
sphere candidates favor nearby luminous stars.

The statistics panel shows oil rates, moon numbers and resource colors.
Its notable-star table shows distance, maximum sphere radius and the number
of planets whose orbits fit inside it, followed by systems with Aquatica
planets nearest to the brightest star.

The panel appears in the bottom-right corner by default. You can move it by
changing `Presentation.PanelCorner` in the generated configuration file:

- `1` — Bottom right
- `2` — Bottom left
- `3` — Top left
- `4` — Top right

## Installation

The simplest option is a Thunderstore-compatible mod manager. Install
DSP Seed Scanner and launch the game with mods enabled; its BepInEx dependency
will be handled for you.

For a manual installation, install
[BepInEx 5](https://thunderstore.io/c/dyson-sphere-program/p/xiaoye97/BepInEx/)
first, then copy the package's `BepInEx` folder into the Dyson Sphere Program
game folder.

## Compatibility

DSP Seed Scanner supports Dyson Sphere Program `0.10.35.29057`. Other game
versions are reported as unsupported. The scanner also stops if it cannot
read the information it needs from the game.

Star and planet names follow the game's native naming option. Changing that
option may trigger a fresh scan; returning to a previously scanned choice can
reuse its cached results.

DSP Seed Scanner can run alongside other BepInEx plugins. Their changes may
affect the generated cluster, scan results, or runtime behavior, and conflicts
cannot always be detected automatically.

## Useful links

- [View the source and report problems](https://github.com/shytamir/DSPSeedScanner)

DSP Seed Scanner is an unofficial community project. Dyson Sphere Program and
its assets belong to their respective owners.

*From your home system to the most exotic corners of your cluster.*
