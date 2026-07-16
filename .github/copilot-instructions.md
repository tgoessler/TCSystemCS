# Copilot Instructions for TCSystemCS

These instructions apply to coding agents working in this repository.

## Before Editing

- Inspect `git status` and preserve unrelated working-tree changes.
- Read the root [`README.md`](../README.md) for repository structure, dependencies, development commands, packaging, and
  CI/CD.
- Read [`CodingStyle.md`](../CodingStyle.md) for C# conventions and project coding patterns.
- Read the project and test-project READMEs for every affected project; they contain package, usage, tool, and data-safety
  details.
- Read [`SECURITY.md`](../SECURITY.md) before dependency or security-related work.

## Repository Rules

- Keep external dependencies minimal and preserve the project-reference direction documented in the root README.
- Follow the existing logging, database lifecycle, immutable-model, resource-management, and threading patterns in
  `CodingStyle.md`.
- Add or update matching NUnit tests for behavior changes.
- Run affected tests during development and use the root README validation commands before finishing when practical.
- Do not use `--no-build` unless the same configuration and target framework have already been built.
- Do not edit or commit generated `bin/`, `obj/`, `.sonarqube/`, coverage, DocFX output, IDE, or ReSharper cache files.
- Keep each topic in its canonical document. Update that document and link to it instead of duplicating instructions.
