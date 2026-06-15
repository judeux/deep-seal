# 0002. Codex Working Rules

Date: 2026-06-15

## Decision

The repository will use project-scoped Codex instructions and local verification scripts before gameplay implementation begins.

Files introduced:

- `AGENTS.md`
- `docs/AGENTS.md`
- `Assets/_Project/Code/AGENTS.md`
- `.codex/config.toml`
- `tools/*.ps1`
- `docs/testing/LOCAL_VERIFICATION.md`
- `docs/architecture/CODE_STRUCTURE.md`

## Rationale

The project is a Unity game with generated project files, `.meta` GUIDs, scene assets, prefab assets, and third-party assets. Codex needs explicit rules to avoid unsafe changes such as modifying `Library/`, importing large packages, changing render pipelines, or directly editing large Unity YAML files.

Local verification scripts provide repeatable commands for checking that Unity can open, run tests, and create a Windows development build.

## Consequences

Codex should work in smaller tasks and report verification results after each implementation.

Project-specific behavior belongs in `.codex/config.toml` and repository instructions. Personal model preferences and account-level Codex settings should remain outside the repository.

See also: `0003-manual-application-workflow.md`.

## Status

Accepted.
