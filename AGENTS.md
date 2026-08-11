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

## 3. Scope and preservation

Inspect and modify only files named by the task and files directly required to
complete it. Do not perform unrelated cleanup, dependency upgrades, broad
refactors, or speculative feature work.

Preserve unexplained user changes. If they overlap the requested work and
cannot be safely retained, stop and ask for direction.

Treat external dependencies and installed software as read-only inputs unless
the task explicitly requires changing them. Do not copy dependency binaries,
credentials, private data, or machine-specific state into the repository.

## 4. Before editing

1. Run `git status --short`.
2. Inspect the directly relevant files and governing documentation.
3. Identify existing behavior and local conventions.
4. Determine the smallest viable change and narrowest useful validation.
5. Ask a question only when a missing decision materially changes the result.

## 5. Change discipline

Prefer small, local, direct, and readable changes compatible with the existing
toolchain. Reuse established patterns when they fit, but do not preserve a
pattern that conflicts with the current requirements.

Avoid speculative abstractions, compatibility layers, broad reflection,
unbounded work, or behavior that depends silently on ambient machine state.
When external behavior is uncertain, distinguish confirmed evidence from
inference and fail explicitly rather than fabricating a result.

## 6. Validation

Run the narrowest relevant check first. For implementation changes, build the
affected project and repair failures caused by the change. Add focused tests
when they provide practical protection for the changed behavior.

Report checks that require unavailable software, credentials, hardware, or an
interactive runtime as skipped unless they were actually performed. Do not
claim that compilation proves runtime behavior, compatibility, performance, or
other properties it did not exercise.

Do not commit generated build output, dependency binaries, data dumps,
temporary diagnostics, secrets, credentials, or editor and OS noise.

## 7. Documentation

`docs/PROJECT.md` is authoritative for project steering decisions and current
tracked status. Do not duplicate that authority in this file or elsewhere.

Update user, contributor, project, and management documentation only when the
change affects their respective contracts. Keep authoritative information in
the repository document designated for it instead of duplicating it here.

Do not present proposed behavior as implemented. Mark unsettled decisions,
skipped validation, and future work plainly.

## 8. Git discipline

Before committing:

1. inspect `git status --short` and the final diff;
2. confirm that only intended files changed;
3. run the relevant validation;
4. check for secrets, generated output, dependency binaries, and data dumps.

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

## 9. Definition of done

A task is complete when:

- the requested behavior or artifact exists;
- the change stays within scope;
- relevant checks pass or skipped checks are reported accurately;
- affected documentation matches the result;
- the final diff contains only intentional changes;
- required commit and push operations succeed.

Stop when the acceptance criteria are satisfied.

## 10. Final report

Report the completed result, changed files, checks actually run, Git branch and
commit details, push result, and any known residual issues. Keep the report
factual and concise.
