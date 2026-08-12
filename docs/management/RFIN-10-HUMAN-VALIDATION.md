# RFIN-10 Human Validation

**Status:** Ready for the single installed-game validation phase. No human
result has been recorded.

Use the exact package produced by the successful RFIN-10 GitHub Actions run.
Run Dyson Sphere Program at 3840 by 2160 on the supported unmodified runtime
identity documented in [project steering](../PROJECT.md). Close the game before
replacing the three installed scanner DLLs or clearing the cache.

Record the seed, whether the load was cached, approximate scan duration, and a
pass/fail note for every step. Screenshots are useful for presentation defects
but are not required for timing or lifecycle observations.

## Seven-step sequence

1. **Uncached scan and pacing.** Clear `BepInEx/config/DSPSeedScanner/cache`,
   open New Game, and load a representative 64-star Combat seed. Confirm one
   scan starts, planet progress stays visible and monotonic, preview input
   remains acceptably responsive, and record the duration from first scanning
   status to complete results.
2. **Completed four-context panel.** Inspect the completed result. Confirm the
   only cards are Fresh start, Megafactory, Compact expansion, and Sphere /
   energy; each card is readable within the accepted 37% by 37% viewport; and
   no raw identifiers, `@`, `+N`, raw units, Dark Fog judgment, or trait section
   appears.
3. **Dark Fog metadata and scrolling.** Confirm exact initial-hive metadata is
   visible in Combat mode. Use both mouse-wheel and scrollbar-thumb input to
   reach the first and last conclusions; confirm the narrower scrollbar remains
   discoverable, draggable, clipped to the panel, and does not change its size.
4. **Cache reuse.** Leave the preview, revisit the same complete identity, and
   confirm a cache hit publishes the same completed conclusions without a full
   scan. A changed resource multiplier, combat mode/settings, star count, or
   runtime identity is not the same cache identity.
5. **Replacement safety.** Start an uncached seed, replace it through the seed
   input after progress begins, and wait for the new preview load. Confirm
   exactly one scan starts for the replacement and no status or conclusion from
   the retired seed appears on the new panel.
6. **Exit safety.** Start another uncached scan and leave the New Game preview
   while it is active. Confirm the panel hides and no obsolete panel or result
   appears after returning to the menu or loading another preview.
7. **Peace-mode and residual review.** Load a representative Peace identity.
   Confirm Dark Fog metadata is absent while all applicable retained contexts
   still render. Record any readability, wording, pacing, scrollbar, cache, or
   lifecycle defect without treating an unexplained difference as acceptance.

## Acceptance record

Do not mark RFIN-10 or the roadmap accepted until all seven steps have an
observed result. Automation establishes build, semantic, cache, package, and
lifecycle contracts; it does not establish perceived smoothness, installed 4K
readability, native input behavior, or the measured scan duration.
