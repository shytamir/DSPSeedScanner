# Continue scanning after game updates

[PROJECT.md](../PROJECT.md) owns authorization, work status and acceptance.

## User story and value

As a player choosing a cluster, I want the scanner to use my current game and
show provisional results when its identity has not been verified, so I can
keep using fresh statistics and conclusions without waiting for a mod release
after every update. The uncertainty must remain visible while I assess them.

## Bounded scope

Turn game-version and runtime-fingerprint drift into an advisory state.
Evaluate current evidence with the existing rules and calibrated bands. Keep
the actual identity throughout generation, reports and cache keys. Preserve
star-count, resource/combat-setting, coverage and runtime failure boundaries.
Show the current and reference game versions in a persistent red warning,
with a red border on both result panels, for fresh and cached results.

## Definition of done

- The reference identity retains normal scanning and presentation.
- An unrecognized identity can complete a scan and display applicable
  statistics and conclusions using current evidence.
- Both unverified panels have a red border and red warning outside scrolling
  content, identifying current/reference versions and possible inaccuracy.
  The warning persists on cache hits and clears when the session changes to
  a verified identity or exits.
- Missing required members, mismatched request/runtime identity, invalid or
  incomplete evidence and runtime failures still prevent successful results.
- Existing star-count and resource/combat-setting limits remain enforced.
- Changed identities cannot reuse one another's cache entries; results are
  never relabelled with the reference identity. Earlier definition results
  cannot suppress conclusions under the new policy.
- Focused regressions and Release solution/game-linked plugin builds pass;
  affected current documentation agrees and the reviewed change reaches main.
  Owner review remains separate from automated technical validation.

## Out of scope

Recalibration, new conclusions, generation changes, compatibility guarantees,
new settings or dialogs, deployment, publication, exhaustive native/gameplay
testing, and unrelated management or backlog work.
