# Complete-Cluster Cache

DSP Seed Scanner keeps audited conclusions and bounded resource statistics
from successful complete scans under the active BepInEx configuration
directory, in `DSPSeedScanner/cache`. In a conventional installation this is:

```text
BepInEx/config/DSPSeedScanner/cache
```

The cache is local to the active BepInEx configuration and is not a source of
new evidence. On each completed New Game preview load, the mod checks it
automatically after evaluating the live preview. A valid hit avoids repeating
a complete scan whose payload satisfies the reuse rules below.

If the active BepInEx configuration directory cannot be selected or used
safely, the scanner continues without cache persistence. Read failures become
cache misses, and write failures leave the completed results visible but
uncached. The scanner never redirects cache writes to another DSP
installation, the managed game directory, or the plugin directory.

## Trust boundary

An entry is reusable only after a successful scan with fully completed planet
coverage and restored runtime state, and while these inputs still match exactly:

- DSP version, galaxy algorithm, game-assembly digest, ordered theme catalogue,
  generation-method digest, and detected generation mods or patchers;
- seed, requested star count, creation version, resource multiplier, and
  canonical pre-play combat settings, including `initialColonize` and
  `maxDensity`; and
- complete-cluster evidence stage, cache schema, scanner compatibility,
  conclusion definition, and conclusion contract versions.

Only Peace/Combat mode is omitted from the reuse key, under the accepted
[Peace/Combat reuse applicability rule](specification/GENERATION-IDENTITY.md#peacecombat-reuse-applicability)
implemented by [FEED-02](archive/USER-FEEDBACK-ROADMAP.md#feed-02-reuse-results-across-the-dark-fog-toggle).
Changing only that toggle can reuse the audited completed payload in either
direction. Canonical generation identities still distinguish Peace from
Combat. The payload and its reports retain their original source identity and
provenance; their presentation belongs to the newly loaded preview session.
The active preview supplies its own mode, immediate facts, and Dark Fog status.
Numeric combat settings and every other key input must still match. New
payload fields require the same invariance evidence before they can be reused;
partial work is never reused across the toggle.

Partial, failed, cancelled, incompatible, corrupt, oversized, or obsolete
entries are cache misses. Each entry carries a payload checksum; corrupt or
obsolete files encountered at the current key are removed.

## Cached payload

A cache hit returns only the audited presentation payload admitted from a
successful complete scan:

- semantic reports for Fresh start resources, cluster-resource strength,
  targeted rare/plentiful systems, rare access, derived system roles, and compact routes, retaining their
  original birth-system-raw or complete-cluster-raw evidence stages; and
- bounded resource statistics: per-body home-system amounts and vein-group
  counts with resource semantics and the native oil multiplier for oil-rate
  display; up to two nearest locations per supported
  sulfuric-acid-ocean or rare-resource category; and per-planet Unipolar Magnet
  locations, vein-node counts, amounts, and vein-group counts.

Live preview conclusions and statistics are regenerated, including home-system
layout and energy facts, nearby Deuterium giant selection, and notable-star
measurements. Dark Fog status is never cached. The bounded resource-statistics
payload does not retain raw planets, vein positions, full normalized resource
evidence, or a complete cluster graph. Rendered wording, elapsed time, memory,
per-planet progress, and execution diagnostics are not persisted.

Targeted system conclusions derive only from the same complete raw deposits
already covered by the Peace/Combat reuse rule. Oil formatting uses the
captured native multiplier and those raw amounts, with no player-history or
combat-mode term. Definition changes invalidate older conclusions; schema
changes invalidate older payloads rather than guessing missing rate inputs.

## Storage bounds

Writes use a temporary file in the cache directory and atomically replace the
destination only after the complete entry has been flushed. Each entry is
limited to 256 KiB. The default cache retains the 256 most recently written or
read entries, establishing a 64 MiB worst-case payload bound. It does not
migrate old schemas or synchronize results between installations.

## Clear the cache

Close Dyson Sphere Program, then delete `DSPSeedScanner/cache` under the active
BepInEx configuration directory (`BepInEx/config/DSPSeedScanner/cache` in a
conventional installation). The mod recreates it when a later successful scan
is stored. This removes only cached conclusions and resource statistics; it
does not affect saves, configuration settings, or the installed mod.

Integrations may perform the same operation through
`ClearCompleteClusterCache`. There is no cache-management panel in the current
scope.
