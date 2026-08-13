# Runtime Identity Maintenance Draft

**Status:** HOTFIX-01 was implemented on 2026-08-13 and reached its external
confirmation gate. COMPAT-02 remains proposed and is not authorized for
implementation.

These stories separate restoration of the reported installation from the
larger decision about which runtime facts make calibrated conclusions
applicable. The hotfix must not broaden conclusion eligibility. The gate
replacement must not be smuggled into error handling.

## HOTFIX-01: Resolve pathless game assemblies safely

**State:** Implemented; automated, game-linked build, and package gates passed.
Confirmation on the reporting user's affected installation remains pending.

**User story:** As a player using the supported DSP version, I want the New
Game scanner to start when the loaded game assembly exposes no filesystem
location, so a loader implementation detail does not make the panel disappear.

**Return:** Make runtime fingerprint capture non-throwing for the reported
empty `Assembly.Location` case while preserving the current conclusion gate.
For `Assembly-CSharp`, use the loaded assembly location only when it names a
readable file; otherwise try the canonical file under the BepInEx game root,
`DSPGAME_Data/Managed/Assembly-CSharp.dll`. Hash the selected readable file
exactly as today. Apply the same defensive file checks to preloader inventory
hashing so an absent, inaccessible, or concurrently removed file cannot abort
the preview callback.

If no assembly file can be read, fingerprint capture must return an explicit
nonblank unavailable identity or a typed incompatibility result; it must not
throw an incidental `ArgumentException`, `IOException`, or access exception.
The preview panel must remain visible and show a concise terminal scanner-
unavailable message when identity capture cannot safely continue. Technical
path and exception details remain in the BepInEx log.

**Acceptance gate:**

- focused tests cover an empty loaded-assembly location with a readable
  canonical game file, proving that its SHA-256 is used;
- missing, blank, inaccessible, and disappearing candidate paths produce a
  controlled fingerprint outcome rather than an escaped file exception;
- an unreadable preloader inventory item cannot abort the whole preview load;
- panel-state coverage proves the remaining identity failure is visible and
  bounded instead of calling `HideCurrent()`;
- existing readable-path behavior and hash output remain unchanged;
- core/runtime suites, the game-linked build, and exact package validation
  pass; and
- the reporting user confirms that a New Game preview reaches scanning or a
  cache result on the affected installation.

**Out of scope:** Changing `IsReferencePreviewIdentity`, accepting a new
assembly hash, changing any conclusion outcome or threshold, redefining cache
identity, migrating existing cache entries, supporting another DSP version,
or claiming compatibility with every loader or plugin combination.

**Implemented:** Runtime fingerprinting now prefers a readable loaded-assembly
path, falls back to BepInEx's canonical managed `Assembly-CSharp.dll`, and
records `unavailable` instead of throwing when neither file can be read.
Preloader inventory retains readable hashes and records an unavailable item
when expected filesystem failures occur. A remaining preview-resolution
exception leaves a bounded `Scanner unavailable` panel visible while the full
exception is logged.

**Validation evidence:** Focused file fixtures covered blank, missing,
preferred, fallback, access-denied, disappearing, and exclusively locked paths.
The existing readable path won over fallback and retained the same SHA-256
format. The panel fixture kept the identity failure visible and bounded. The
Release solution built without warnings; 14 core checks and 61 runtime checks
passed; the installed-game-linked plugin build and exact artifact and
Thunderstore-package validators passed. The reporting user's confirmation is
still required to close the final gate.

## COMPAT-02: Replace the exact-reference conclusion gate

**User story:** As a player on the supported DSP release, I want conclusions
to depend on the runtime facts and evidence their predicates actually require,
so a machine-specific assembly digest does not suppress otherwise valid live
results.

**Return:** Replace the coarse `IsReferencePreviewIdentity` gate with explicit,
named applicability rules shared by the conclusion families. The replacement
must be based on dependencies of the accepted predicates, not on whether a
runtime happens to reproduce the development machine's complete fingerprint.

The common applicability boundary must require:

- the exact supported DSP version and matching request creation version;
- the current scanner conclusion contract and definition versions;
- available required runtime members and successful runtime compatibility;
- complete evidence for the family being evaluated; and
- the settings and scope already required by that family, including 64-star
  scope only for predicates whose calibrated range depends on it.

The exact `Assembly-CSharp` SHA-256 must no longer decide conclusion
eligibility. Assembly digest, module identity, observed galaxy algorithm,
ordered themes, generation-method digest, loaded plugin IDs and versions, and
preloader identities remain provenance and cache-isolation inputs. Changes to
those values must produce a different cache key, but do not alone turn live,
complete evidence into unknown. This deliberately accepts that co-installed
plugins may alter generation; runtime exceptions, missing members, partial
coverage, unsupported game versions, and unsupported settings continue to fail
closed.

Audit every current `IsReferencePreviewIdentity` call site and replace it with
the narrowest applicable rule. Do not replace one global boolean with another
equally broad alias. Record which families depend on star count, resource
multiplier, combat settings, or other calibrated inputs, and preserve their
existing unknown behavior outside accepted ranges.

Because this broadens the circumstances in which accepted predicates may
publish conclusions, advance the conclusion definition's semantic version as
required by its contract. Let that version change invalidate old semantic
cache entries through the existing cache identity; do not write a cache
migration.

**Acceptance gate:**

- a dependency table maps every affected conclusion family to its game,
  creation-version, evidence, settings, and scope gates;
- focused fixtures vary assembly digest, module/provenance identity,
  generation-method digest, plugin inventory, patcher inventory, algorithm,
  and ordered themes independently and prove they isolate cache keys without
  suppressing otherwise complete supported-version conclusions;
- fixtures independently vary game version, creation version, required-member
  availability, evidence completeness, star count, resource settings, and
  combat settings and prove only the dependent families publish or remain
  unknown;
- existing threshold endpoints, outcomes, attribution, cancellation, state
  restoration, and incomplete-result publication rules remain unchanged;
- obsolete cache entries miss cleanly after the definition-version change;
- all core/runtime suites, game-linked build, artifact checks, and exact
  Thunderstore-package validation pass; and
- installed validation covers one cache miss and subsequent hit on the
  affected environment plus one ordinary multi-plugin environment.

**Out of scope:** New predicates or thresholds, support for a different DSP
version, automatic correction of third-party generation changes, universal
mod compatibility, shared caches, cache migration, scoring, UI redesign, or
changes to scan scheduling and runtime-state restoration.

## Sequence

HOTFIX-01 may ship independently after its gate and must retain today's
conclusion semantics. COMPAT-02 follows as a separately reviewable semantic
change, using the hotfix's reliable provenance capture but not reopening its
error-handling scope.
