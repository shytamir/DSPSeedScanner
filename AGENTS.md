# AGENTS.md

This file defines how coding agents should work in this repository.

## 1. Core rule

Complete the requested task with the smallest coherent change that satisfies
its acceptance criteria. Optimize for correctness, maintainability, and
reviewability rather than activity.

## 2. Instruction order

Follow, in order:

1. The current user prompt.
2. This `AGENTS.md`.
3. Repository documentation and conventions.
4. Existing implementation patterns.
5. General engineering judgment.

A specific instruction overrides a general one.

## 3. Product contract

DSP Seed Scanner is a BepInEx-dependent mod that uses the Dyson Sphere Program
runtime to generate and inspect procedurally generated star clusters.

Preserve these invariants unless a task explicitly changes one:

- scanning is deterministic for the same seed, game version, and generation
  settings;
- scanning does not modify the player's factory, progression, or save data;
- game runtime data is treated as authoritative;
- generation, extraction, evaluation, and reporting remain separate concerns;
- long-running scans remain bounded, observable, and interruptible;
- failures identify the affected seed and stage without fabricating results;
- game, Unity, and BepInEx assemblies are dependencies and are never
  redistributed from this repository.

Read `docs/PROJECT.md` before changing scan behavior, result contracts,
runtime integration, or project scope.

## 4. Scope discipline

Inspect and modify only files named by the task and files directly required to
complete it. Do not perform unrelated cleanup, dependency upgrades, broad
refactors, or speculative feature work.

Preserve unexplained user changes. If they overlap the requested work and
cannot be safely retained, stop and ask for direction.

## 5. Before editing

1. Run `git status --short`.
2. Inspect the directly relevant source and documentation.
3. Identify existing behavior and local conventions.
4. Determine the smallest viable implementation and narrowest validation.
5. Ask a question only when a missing decision materially changes the result.

## 6. Architecture and runtime evidence

Keep these responsibilities separate as the implementation develops:

```text
Scan request
    |
DSP runtime generation
    |
Normalized cluster model
    |
Criteria evaluation
    |
Result reporting
```

Use the installed game and its assemblies as read-only development inputs.
Do not copy game assemblies into the repository or introduce replacement
generation logic when the task calls for authoritative runtime behavior.

When runtime members or behavior are uncertain, inspect the installed
assemblies or record runtime evidence. Distinguish confirmed behavior from
inference. Compatibility failures should be explicit and diagnostic rather
than silently producing incomplete scan results.

## 7. Implementation discipline

Prefer:

- small, local changes;
- direct and readable code compatible with the selected BepInEx toolchain;
- deterministic evaluation with explicit inputs;
- normalized data at the boundary between DSP runtime access and scan rules;
- cancellation and progress reporting for batch work;
- focused validation and documentation that matches behavior.

Avoid:

- hidden dependence on the active save;
- frame-by-frame work that can be scheduled or batched;
- unbounded allocations or retained game objects across seeds;
- scoring rules embedded in runtime extraction code;
- broad reflection scans when a focused runtime API is known;
- speculative abstractions and compatibility layers.

## 8. Validation

Run the narrowest relevant check first. For implementation changes, build the
affected project and repair failures caused by the change. Add focused tests
for deterministic normalization or evaluation logic when practical.

Compilation alone does not prove in-game behavior, runtime compatibility,
performance, or deterministic cluster generation. Report checks requiring a
running game as skipped unless they were actually performed.

Do not commit generated build output, copied assemblies, scan result dumps,
save data, temporary diagnostics, or editor and OS noise.

## 9. Documentation

Keep `README.md` focused on users and contributors. Keep `docs/PROJECT.md` as
the authority for product purpose, scope, invariants, architecture, and current
state. Update both when a change affects their respective contracts.

Do not present proposed behavior as implemented. Mark unsettled decisions and
future work plainly.

## 10. Git discipline

Before committing:

1. inspect `git status --short` and the final diff;
2. confirm that only intended files changed;
3. run the relevant validation;
4. check for secrets, generated output, copied assemblies, and scan data.

Commit only when requested. Push only when explicitly requested. Do not amend,
rebase, reset, clean, stash, force-push, or rewrite history unless explicitly
instructed.

If Git reports dubious ownership, scope the exception to each command instead
of changing global configuration:

```powershell
$repo = (Resolve-Path '.').Path.Replace('\', '/')
git -c "safe.directory=$repo" status --short
```

Before pushing a clean branch that may be behind its remote, fetch and use a
fast-forward-only update. Never resolve divergence with a force push.

## 11. Definition of done

A task is complete when:

- the requested behavior or artifact exists;
- the change stays within scope;
- relevant checks pass or skipped checks are reported accurately;
- documentation matches the implemented behavior;
- the final diff contains only intentional changes;
- required commit and push operations succeed.

Stop when the acceptance criteria are satisfied.

## 12. Final report

Report the completed result, changed files, checks actually run, Git branch
and commit details, push result, and any known residual issues. Keep the report
factual and concise.
