# RFIN-10 Human Validation

[PROJECT.md](../PROJECT.md) is the sole authority for project steering and work
status, including historical dispositions. This document contains scope,
requirements, or technical evidence; it does not track status.

The 2026-08-12 validation at 3840 by 2160 used successive local candidate
builds while resolving observed defects. The final tested build used the
supported unmodified runtime identity and retained the three scanner-owned
DLL boundary.

The sequence below records the preview observations. Screenshots and direct
runtime feedback informed the repair iterations.

## Seven-step sequence

1. **Uncached scan and pacing passed.** A representative uncached 64-star
   Combat seed started one scan. Planet progress remained visible and
   monotonic, and preview rendering showed no observable frame drops.
2. **Completed four-context panel passed.** Fresh start, Megafactory, Compact
   expansion, and Sphere / energy remained readable within the accepted 37% by
   37% viewport, with prohibited mechanical output absent.
3. **Dark Fog metadata and scrolling passed.** Combat metadata was present,
   and the conclusion viewport reached its complete result through the accepted
   scrolling behavior.
4. **Cache reuse passed.** Revisiting a previously scanned complete identity
   reused its cached conclusions without starting another full scan.
5. **Replacement safety passed.** Replacing the seed produced one scan for the
   new loaded preview and no stale publication from the retired seed.
6. **Exit safety passed.** Leaving an active preview hid the panel and no
   obsolete result appeared afterward.
7. **Peace-mode and residual review passed.** Applicable contexts remained
   available without Combat-only Dark Fog metadata, and no blocking residual
   presentation, pacing, cache, or lifecycle defect was recorded.

## Observed behavior

The final uncached scan retained visible,
monotonic planet progress and completed without observable frame drops. The
four-context panel, Dark Fog metadata boundary, scrolling, cache reuse, seed
replacement, preview exit, and Peace-mode behavior worked as specified.

Return to the
[presentation refinement roadmap](PRESENTATION-REFINEMENT-ROADMAP.md),
[project steering](../PROJECT.md), or the [documentation index](../INDEX.md).
