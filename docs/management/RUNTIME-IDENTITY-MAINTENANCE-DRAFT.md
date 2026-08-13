# Runtime Identity Maintenance Draft

**Status:** HOTFIX-01 was implemented on 2026-08-13 and reached its external
confirmation gate. COMPAT-02 was retired without implementation. FSOR-01 was
implemented and passed its acceptance gate on 2026-08-13.

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

**State:** Implemented; acceptance gate passed on 2026-08-13.

**User story:** As a player, I want scanner-owned file access to recover from
missing, blank, stale, inaccessible, or concurrently changed paths, so a
filesystem problem cannot make the preview panel disappear or abort an
otherwise usable scan.

**Return:** Add one resolver that selects an active-runtime filesystem context
and supplies its resolved paths to all consumers. Candidate selection must not
be repeated outside the resolver.

Use the current DSP process executable location as the authoritative game
root. Fall back to BepInEx's reported game root only when the executable path
is unavailable. Normalize the selected root and require the expected DSP and
BepInEx directory structure. A conflicting nonblank BepInEx game root or
loaded `Assembly-CSharp` location fails closed. The loaded plugin location may
corroborate the context but cannot select or displace it. Ignore unrelated
installations, including the default Windows installation when it is not the
active root.

Derive the managed assembly, BepInEx root, preloader directory, and canonical
configuration directory from the selected root. Preserve existing assembly
and preloader digest formats. An unavailable or unreadable required assembly
produces a bounded `Scanner unavailable` result. An invalid or unreadable
preloader path records unavailable optional provenance without aborting the
scan.

Keep the conclusion cache under the active BepInEx configuration tree. A
blank configuration path may use the canonical derived directory; a nonblank
path outside the selected BepInEx root disables persistence instead of being
followed or replaced. Cache initialization, read, write, replacement, touch,
trim, delete, and clear failures must be contained. Reads degrade to misses;
writes leave completed results usable but uncached; no cache failure may
prevent plugin startup or scanning.

If scanner-controlled panel-setting binding raises a recognized filesystem
failure, retain the default corner and continue without an alternate config
destination. Report handled filesystem failures with a bounded operation,
source, and exception type/message, without full stack traces. Preserve normal
diagnostics for unexpected non-filesystem failures.

**Acceptance gate:**

- resolver fixtures prove the process executable selects one normalized
  context, the BepInEx game root is only a fallback, required conflicts fail
  closed, and another readable installation can never be selected;
- blank, malformed, missing, locked, inaccessible, and concurrently removed
  paths produce the defined required, optional, or persistence-only outcome;
- the selected game assembly and preloader inventory retain their successful
  digest formats, while required identity exhaustion remains visibly terminal;
- cache fixtures cover construction, read, write, replacement, touch, trim,
  delete, and clear failures and prove scanning still completes without cache
  persistence;
- invalid external config paths are never written, and a panel-setting
  persistence failure retains the default corner without blocking startup;
- diagnostics identify the failed operation and selected source without an
  expected-filesystem stack trace or unbounded path disclosure;
- conclusion eligibility, runtime identity fields, cache key/schema, and
  successful-path behavior remain unchanged; and
- core/runtime suites, the game-linked build, artifact checks, and exact
  Thunderstore-package validation pass.

**Out of scope:** Changing conclusion predicates or identity gates, accepting
another DSP version, changing cache contents or migration, writing outside the
active BepInEx config tree, searching other installations or the registry,
repairing BepInEx-owned config/log persistence outside scanner-controlled
calls, developer-probe output hardening tracked as
[TD-004](TECHNICAL-DEBT.md#td-004-contain-developer-probe-output-failures),
changing build/package scripts, removing developer probes, UI redesign, or
supporting another plugin manager.

**Implemented:** One resolver now selects the active runtime context from the
current DSP executable, with BepInEx game-root fallback only when that path is
unavailable. Required root and assembly conflicts fail closed; plugin,
preloader, and configuration paths cannot select another installation.
Required assembly reads produce a visible bounded failure, optional preloader
failures retain unavailable provenance, and cache or panel-setting persistence
failures leave scanning operational without writing outside the active
BepInEx configuration tree. Expected filesystem diagnostics are bounded and
omit stack traces.

**Validation evidence:** Resolver and failure-injection fixtures covered
authoritative and fallback roots, conflicting installations, blank and
malformed paths, missing and locked files, external optional paths, required
hashing, optional inventory, cache initialization, read, write, replacement,
touch, trim, delete, clear, and guarded setting failures. The Release solution
built without warnings; 14 core checks and 66 runtime checks passed; the
installed-game-linked plugin build passed; and exact versioned artifact and
Thunderstore package validation passed for `1.0.0`.

## Sequence

HOTFIX-01 retained the existing conclusion semantics. COMPAT-02 was retired
after investigation and will not follow it into implementation. FSOR-01 is an
implemented filesystem-robustness correction that preserved the existing
identity and conclusion contracts.
