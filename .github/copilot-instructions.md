# Copilot Instructions for TCSystemCS

## Canonical Documentation

Use these documents as the source of truth instead of duplicating their content:

- [`README.md`](../README.md) — solution structure, frameworks, dependencies, prerequisites, build/test/coverage commands, packaging, and CI/CD.
- [`CodingStyle.md`](../CodingStyle.md) — C# formatting, naming, analyzers, architecture patterns, resource handling, logging, threading, and NUnit conventions.
- Project-specific READMEs linked from [`README.md`](../README.md#projects) — APIs, tool behavior, dependencies, and targeted-test guidance.
- [`SECURITY.md`](../SECURITY.md) — vulnerability checks and reporting.

Read the root README and coding style before editing, followed by the README for every affected project and test project.

## Repository Workflow

- Inspect `git status` before editing and preserve unrelated working-tree changes.
- Keep external dependencies minimal and preserve the project-reference direction documented in the root README.
- Use existing TCSystem logging, database factory/lifecycle, immutable model, and threading patterns defined in [`CodingStyle.md`](../CodingStyle.md#project-design-patterns).
- Add or update matching NUnit tests for behavior changes.
- Run affected tests during development and use the validation commands in [`README.md`](../README.md#run-unit-tests) before finishing when practical.
- Do not use `--no-build` unless the same configuration and target framework have already been built.
- Do not edit or commit generated `bin/`, `obj/`, `.sonarqube/`, coverage XML, IDE, or ReSharper cache files.
- When behavior or project configuration changes, update its canonical document and link to it elsewhere rather than copying it.
