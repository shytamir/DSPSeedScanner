# Runtime Identity Maintenance Draft

**Status:** HOTFIX-01 was implemented on 2026-08-13 and reached its external
confirmation gate. COMPAT-02 was retired without implementation. FSOR-01 is a
draft and is not authorized for implementation.

This draft records the completed path-resolution hotfix, the retired proposal
to change conclusion eligibility, and a separate filesystem-robustness story.
Filesystem recovery must not broaden conclusion eligibility.

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

## COMPAT-02: Apply conclusion gates by evidence basis

**State:** Retired without implementation.

The investigation found no demonstrated conclusion-gate defect behind the
reported failure. HOTFIX-01 addressed the actual empty-path filesystem failure
without changing conclusion semantics.

The proposed compatibility rewrite was not justified:

- cohort-calibrated solar, wind, starter-resource, energy, and maximum-shell
  ranges still require the exact reference identity under which their ranges
  were established;
- live-fact predicates already use complete runtime evidence without the exact
  reference-range gate;
- contained-orbit applicability remains coupled to a version-sensitive sphere
  radius derivation;
- rare-resource and compact distances use absolute player-facing ranges, but
  rare generation remains settings-sensitive and compact distance inherits the
  applicability of its upstream roles; and
- broadening any quantitative result beyond the accepted 64-star identity
  would reopen the specification boundary rather than repair compatibility.

The assembly, algorithm, ordered themes, generation-method digest, plugin
inventory, and patcher inventory therefore remain provenance and cache-
isolation inputs, and the existing conclusion boundary remains unchanged.
Any future narrowing must begin as a separately authorized specification
investigation for the affected predicate, not as this compatibility story.

## FSOR-01: Make runtime filesystem access resilient

**State:** Draft; not authorized for implementation.

**User story:** As a player, I want scanner-owned file access to recover from
missing, blank, stale, inaccessible, or concurrently changed paths, so a
filesystem problem cannot make the preview panel disappear or abort an
otherwise usable scan.

**Return:** Resolve one active-runtime filesystem context and give consumers
only its selected paths. Consumers must not receive candidate lists or repeat
path ranking themselves.

Establish the active game and BepInEx roots only from evidence loaded into the
current process: BepInEx host paths, this plugin DLL's canonical location under
`BepInEx/plugins/DSPSeedScanner`, and the loaded target assembly's canonical
location. Normalize and de-duplicate that evidence. Agreeing evidence selects
one context; conflicting evidence fails closed. A default Windows installation
may corroborate an already established active root but must never establish,
override, or compete with it. Mere existence or readability never qualifies a
file from another DSP installation.

From the selected context, resolve and validate the game root, BepInEx root,
managed `Assembly-CSharp.dll`, preloader directory, and configuration
directory together with bounded provenance for the selection. Apply the
selected read-only paths to game-assembly fingerprinting and BepInEx preloader
inventory. Preserve the existing SHA-256 and inventory formats on success.
Failure to read the required game assembly must produce a bounded `Scanner
unavailable` result. Missing or unreadable optional preloader inventory must
remain explicit unavailable provenance and must not abort the scan.

Keep the conclusion cache under the configuration directory selected from the
active BepInEx context. Accept BepInEx's reported configuration path only when
it belongs to that context; otherwise derive the canonical `BepInEx/config`
directory from the same root. Do not redirect writes to another DSP
installation, the managed directory, or the plugin directory. Cache path
construction, initialization, reads, writes, atomic replacement, timestamp
updates, trimming, deletion, and clearing must handle expected path, I/O,
access, security, and concurrent-change failures. A cache read failure becomes
a miss, a write failure leaves completed results usable but uncached, and an
unavailable cache must not prevent plugin startup or scanning.

Contain recognized filesystem failures raised while binding the panel-corner
setting through BepInEx: retain the default corner and continue without
inventing an alternate configuration destination. BepInEx configuration or
logging failures outside scanner-controlled calls remain external.

Every handled filesystem failure must emit only the operation, selected source
and concise exception type/message needed to diagnose it. User-facing text
stays bounded, and expected product-filesystem failures must not add full stack
traces to panel or scanner-log output. Unexpected programming and runtime
failures retain their existing diagnostic behavior.

**Acceptance gate:**

- focused fixtures prove that consumers receive one selected context rather
  than candidates; agreeing active-process evidence is normalized and
  de-duplicated, conflicts fail closed, and a readable default or second
  installation cannot override the active process;
- path fixtures cover blank and malformed evidence, missing and locked files,
  access denial, concurrent removal, canonical plugin and target-assembly
  derivation, and BepInEx host-path fallback;
- the selected game assembly retains the existing digest, optional patcher
  failures retain bounded unavailable provenance, and inability to select or
  read the active assembly produces a visible terminal panel state;
- cache fixtures cover construction, read, write, replacement, touch, trim,
  delete, and clear failures and prove scanning still completes without cache
  persistence;
- a recognized panel-setting persistence failure retains the default corner
  without preventing scanner initialization;
- diagnostics identify the failed operation and selected source without an
  expected-filesystem stack trace or unbounded path dump;
- conclusion eligibility, runtime identity fields, cache key/schema, and
  successful-path behavior remain unchanged; and
- core/runtime suites, the game-linked build, artifact checks, and exact
  Thunderstore-package validation pass.

**Out of scope:** Changing conclusion predicates or identity gates, accepting
another DSP version, changing cache contents or migration, writing outside the
active BepInEx config tree, searching Steam libraries or the registry,
repairing BepInEx-owned config/log persistence outside scanner-controlled
calls, developer-probe output hardening tracked as
[TD-004](TECHNICAL-DEBT.md#td-004-contain-developer-probe-output-failures),
changing build/package scripts, removing developer probes, UI redesign, or
supporting another plugin manager.

## Sequence

HOTFIX-01 retained the existing conclusion semantics. COMPAT-02 was retired
after investigation and will not follow it into implementation. FSOR-01 is an
unauthorized draft for a separate filesystem-robustness change.
