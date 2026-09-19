# Statistics Panel Presentation

[PROJECT.md](../PROJECT.md) owns steering and work status. This document
describes the factual table presentation contract.

## Home system

Oil is the planet's total native rate with two decimals in `/s`, followed by
the existing wells count. Sum native amounts before applying the captured
`VeinData.oilSpeedMultiplier`; use the native float and ties-to-even display
rounding described in [Panel evidence](PANEL-EVIDENCE.md#native-resource-units).
Resource settings are already reflected in the amounts. Missing multiplier
evidence displays Unavailable, never a guessed rate. Ore counts are unchanged.

Each solid moon's Body cell has a second line, `Moon 1`, `Moon 2`, and so on,
ordered from nearest to farthest around its own parent, including the home
moon. Primary planets and giants have no second line. The table's body order
is unchanged. Only the home planet's whole row is green when its entire
star-centric orbit is contained by the home star's maximum sphere radius.

Mineable Fire Ice and Spiniform entries use green text independently of that
whole-row highlight. Other ores retain normal formatting; giant gas-product
text is unchanged.

## Cluster resources

Rare-resource candidate cells are green below 3.5 ly. Sulfuric Acid ocean
cells are red above 8.5 ly; equality at either endpoint retains normal text.
If the same system is represented among the displayed candidates in all four
Spiniform, Fire Ice, Organic Crystal and Sulfuric Acid rows, its cells in those
rows are green. Both Closest and Alternative participate. Other cells and
headings are unchanged; resource presence outside the displayed candidates
does not qualify a system. Selection and cache contents are unchanged.

Unipolar Magnets amount cells are green above 2,000,000 and red below 900,000.
Their Distance cells are independently green below 15 ly and red above 21 ly.
Exact endpoints use normal text. Compare underlying quantities before display
rounding; the other cells and row selection remain unchanged.

## Notable stars

Sphere rows list blue giants first in descending Dyson luminosity, then the
brightest star unless already represented as a blue giant, then remaining
O stars by home-system distance. Blue giant rows use green text throughout.
The conclusions panel's luminosity cutoff does not apply to this table.

Columns are Star, Type, Size (stellar radius in R), Luminosity (three decimals
in L), Distance (from home), Max Sphere (native maximum radius in meters),
Contained (whole-orbit planet count), and Note. Brightest retains its note.
The shared native geometry definition is in [Panel evidence](PANEL-EVIDENCE.md).

The final group contains up to five non-O Aquatica host stars, selected and
sorted by distance from the brightest star. Each host appears once in this
group regardless of its matching planet count; another displayed role does
not disqualify it. Aquatica uses the native catalogue's invariant theme name
`Ocean 5`. O-spectrum blue giants are excluded from this group too.

Aquatica rows leave Luminosity and Contained empty, keep stellar Size and
Max Sphere, and show their distance from the brightest star. The existing
Note column identifies these rows as Aquatica.
