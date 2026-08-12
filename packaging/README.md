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

This release supports Dyson Sphere Program `0.10.34.28529`. If the game or
another mod changes cluster generation, DSP Seed Scanner may report that the
runtime is unsupported instead of showing unreliable results.

## Useful links

- [View the source and report problems](https://github.com/shytamir/DSPSeedScanner)

DSP Seed Scanner is an unofficial community project. Dyson Sphere Program and
its assets belong to their respective owners.
