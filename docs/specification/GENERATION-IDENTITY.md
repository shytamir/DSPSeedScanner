# Generation Identity Contract

**Status:** Accepted on 2026-08-11 under the documented limits.

This contract defines the inputs that DSP Seed Scanner must retain when it
claims that generated evidence can be reproduced. It describes the installed
Dyson Sphere Program runtime examined on 2026-08-11 and was accepted because
the intended BepInEx mod can obtain the required identity from that runtime.

## Contract

A seed number alone does not identify a generated cluster. Reproduction must
bind the evidence to a layered identity so later work can request only the
inputs relevant to the conclusion being made.

### Runtime compatibility identity

Every layer requires:

- the full DSP version, including build number;
- the galaxy algorithm value supplied to `GameDesc.galaxyAlgo`;
- the installed generation implementation and data catalogue, including the
  ordered theme IDs captured in `GameDesc.savedThemeIds`;
- the scanner compatibility version and the presence of any mod or runtime
  patch capable of changing generation.

For the examined installation these values were DSP `0.10.34.28529`, Steam
build `23109513`, `UniverseGen.algoVersion` `20200403`, and 25 ordered theme
IDs. The examined `Assembly-CSharp.dll` SHA-256 was
`AE0BA95F75BD879A62AA4CE253B2AB78EAA4FB3C7C595F5E1FEE75EBE0E0EF85`.
These identifiers describe evidence provenance; they do not promise
compatibility with another build that reports the same galaxy algorithm.

### Galaxy structure identity

Reproducing stars, systems, planet topology, orbits, planet themes, and the
birth system requires the runtime compatibility identity plus:

- `galaxySeed`;
- requested `starCount`;
- the creation version recorded by `GameDesc`.

The ordered theme catalogue is an input, not merely descriptive metadata.
Reversing the same 25 theme IDs changed the generated structural hash in the
runtime experiment.

The actual generated star count must be recorded as output. The generator may
return the number of positions it could generate rather than blindly echoing
the requested count.

### Resource realization identity

Reproducing resource evidence requires the galaxy structure identity plus the
exact `resourceMultiplier` and the generation stage being reported.

At galaxy-preview generation, ordinary `0.5`, `1`, and infinite `100`
multipliers produced the same structural and gas-rate evidence for the test
seed. Rare-resource `0.1` retained the same structure but changed gas-giant
collection rates. Assembly inspection also confirmed that terrain/vein
generation reads `resourceMultiplier` later, so equal preview structure does
not imply equal resource deposits.

The exact multiplier must be retained. The rounded value embedded in DSP's
display string or numeric seed key is not a sufficient identity.

### Dark Fog identity

Reproducing the Dark Fog fields available in the New Game galaxy requires the
galaxy structure identity plus:

- `isPeaceMode`;
- `combatSettings.initialColonize`;
- `combatSettings.maxDensity`.

`initialColonize` changed initial hive counts and `maxDensity` changed both
initial and maximum hive counts in the runtime experiment. Toggling combat on
with otherwise default settings did not change the generated galaxy snapshot,
because the same default combat values were already present in `GameDesc`.

### Peace/Combat reuse applicability

A focused follow-up on 2026-08-13 established that `isPeaceMode` may be omitted
from the reuse key for explicitly toggle-invariant pre-play evidence when every
other identity input, including `initialColonize` and `maxDensity`, remains
equal. It may not be omitted from the canonical generation identity or from
Dark Fog status.

Assembly inspection of the supported `Assembly-CSharp.dll` found no direct
`isPeaceMode` read in galaxy, star, planet, terrain, or vein generation. Star
generation read `initialColonize` and `maxDensity`; raw algorithms did not read
the mode field or its `isCombatMode` property. Indirect property consumers were
UI, save, achievement, property-multiplier, factory, sector, and other gameplay
paths rather than the inspected pre-play generation path.

The controlled runtime comparison held seed, 64-star request, creation version,
ordered themes, resource multiplier `1`, galaxy algorithm, assembly, and
numeric combat values equal and changed only Peace/Combat mode. Seeds
`73339583`, `96178012`, and `45772` covered 630 solid planets. For each seed,
Peace and Combat produced identical hashes for the selected preview facts and
for every generated vein node and summarized vein group. The comparison covered
383,309 vein nodes and 21,351 groups, including planet attribution, theme and
algorithm, resource type and product, node/group counts, amounts, and positions.
The preview hash covered the star, planet, topology, theme, energy, gas-product,
ocean, and stellar facts consumed or planned by the product. Two independent
DSP processes produced byte-identical result files with SHA-256
`4F0889174EA9FDC90D6E8BB0A189D94B33840132C6A6B74BF9B1CBF0C8ABB33C`.

This finding authorizes reuse only for fields included in that audited payload.
An added field must establish the same invariance before joining it. The active
preview continues to supply its own mode and Dark Fog occupation, while the
reused payload retains its source identity and provenance. Any numeric combat,
resource, seed, catalogue, runtime, algorithm, contract, or evidence-stage
change remains a cache miss.

Reproducing the actual enemy state created when play begins additionally
requires `initialLevel` and `initialGrowth`. Reproducing or interpreting later
Dark Fog behavior requires the complete combat-settings value:

- aggressiveness;
- initial level;
- initial growth;
- initial colonization;
- maximum density;
- growth speed;
- power threat factor;
- battle threat factor;
- battle experience factor.

The latter behavior is not a static seed guarantee. Future conclusions must
state whether they concern the pre-play galaxy, initial enemy state, or
state that evolves during play.

## Inputs that did not identify cluster evidence

For the inspected call paths and runtime experiment:

- `playerProto`, `goalLevel`, achievement eligibility, and creation time were
  game/session metadata rather than generation inputs;
- sandbox mode changed game semantics and DSP's displayed seed key but did not
  change the generated galaxy snapshot;
- initial enemy level did not change the New Game galaxy snapshot, although it
  is consumed when enemy systems are created after generation;
- combat mode on/off did not independently change the snapshot when its
  combat values remained equal.

These findings limit the identity of pre-play cluster evidence only. They do
not claim that the settings are irrelevant to the player's run.

## Canonical identity requirements

The product must not use `GameDesc.clusterString`, `clusterStringLong`, or
`seedKey64` as its sole cache or comparison key. Those representations omit
the runtime implementation and ordered theme catalogue, quantize the resource
multiplier, and compress combat settings into a derived difficulty number.

A future normalized identity should therefore contain:

1. a runtime compatibility fingerprint;
2. seed, requested star count, and galaxy algorithm;
3. the full creation version and ordered theme identity;
4. exact resource settings when resource evidence is requested;
5. the Dark Fog fields appropriate to the evidence stage;
6. an explicit evidence-stage identifier.

Equality must be exact for stored inputs. Friendly DSP cluster strings may be
displayed alongside this identity but must not replace it.

## Failure behavior

Generation must be rejected as unsupported rather than silently reused when:

- the DSP build or generation catalogue is not recognized;
- the DSP game version is unsupported or a required runtime member is missing;
- required resource or combat inputs are unavailable;
- evidence from different generation stages is compared as if equivalent.

Loaded plugin and preloader inventories remain part of the cache identity but
their presence or detected generation changes do not reject a scan. The
observed assembly, algorithm, catalogue, and generation-method identities also
remain in the cache key. This permits BepInEx co-installation while preventing
cached conclusions from crossing different recorded environments. A plugin
may still alter generation or runtime behavior in a way the scanner cannot
identify; that interaction risk is accepted and does not imply that every mod
combination has been validated.

## Unresolved boundaries

SPEC-01 did not establish cross-machine floating-point equivalence, post-start
ground-base placement, or a durable fingerprint for all generation-relevant
LDB content. SPEC-02 subsequently confirmed representative exact raw terrain
and vein repeatability through the mod's intended runtime boundary and
classified post-start ground-base placement as ineligible pre-play evidence.
The remaining questions belong to compatibility and implementation planning.
They do not invalidate the accepted identity fields or broaden the contract
beyond its stated limits.
