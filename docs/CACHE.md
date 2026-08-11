# Complete-Cluster Cache

DSP Seed Scanner keeps successful complete-cluster presentation conclusions
under:

```text
BepInEx/config/DSPSeedScanner/cache
```

The cache is local to the installed game and is not a source of new evidence.
It only avoids repeating a scan whose complete semantic conclusions were
produced for the same supported runtime and generation identity.

## Trust boundary

An entry is reusable only when all of the following still match exactly:

- DSP version, galaxy algorithm, game-assembly digest, ordered theme catalogue,
  generation-method digest, and detected generation mods or patchers;
- seed, requested star count, creation version, resource multiplier, combat
  mode, and canonical pre-play combat settings;
- complete-cluster evidence stage, cache schema, scanner compatibility,
  conclusion definition, and conclusion contract versions; and
- successful, fully completed planet coverage with restored runtime state.

Partial, failed, cancelled, incompatible, corrupt, oversized, or obsolete
entries are cache misses. Each entry carries a payload checksum; corrupt or
obsolete files encountered at the current key are removed. A cache hit returns
only semantic conclusion reports attributed to complete-cluster evidence.
Live preview conclusions are inexpensive to regenerate and are not duplicated.
Normalized rare-resource evidence, rendered panel wording, elapsed time,
memory, per-planet progress, and execution diagnostics are never persisted.

Writes use a temporary file in the cache directory and atomically replace the
destination only after the complete entry has been flushed. Each entry is
limited to 256 KiB. The default cache retains the 256 most recently written or
read entries, establishing a 64 MiB worst-case payload bound. It does not
migrate old schemas or synchronize results between installations.

## Clear the cache

Close Dyson Sphere Program, then delete the
`BepInEx/config/DSPSeedScanner/cache` directory. The mod recreates it when a
later successful scan is stored. This removes only cached presentation
conclusions; it does not affect saves, configuration settings, or the installed
mod.

Integrations may perform the same operation through
`ClearCompleteClusterCache`. There is no cache-management panel in the current
scope.
