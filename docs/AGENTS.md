# AGENTS.md — Documentation Rules

This file applies to the `docs/` tree.

## Documentation Purpose

The documentation exists to keep design, architecture, decisions, testing, and licensing clear enough for a solo developer and Codex to work consistently over a long project.

## Status Tags

Use these tags when describing design elements:

- `LOCKED`: Current agreed direction. Do not change without a decision record.
- `DRAFT`: Direction is useful, but details are not final.
- `DEFERRED`: Not part of the current prototype, but may return later.
- `OUT`: Intentionally excluded from scope.

## Decision Records

Use `docs/decisions/` when a decision explains a meaningful change in direction.

Filename format:

```text
NNNN-short-kebab-case-title.md
```

Example:

```text
0002-codex-working-rules.md
```

Recommended structure:

```markdown
# 0002. Title

Date: YYYY-MM-DD

## Decision

## Rationale

## Consequences

## Status
```

## GDD Changes

Do not delete major design ideas silently.

If a feature is postponed, mark it as `DEFERRED` and explain why.

If a feature is removed, mark it as `OUT` and explain the reason.

If a feature is core, mark it as `LOCKED` only when the project owner explicitly agrees.

## Architecture Documents

Architecture documents should describe code boundaries, responsibilities, and dependencies.

Do not use architecture documents to introduce large new systems unless they are tied to the current MVP or a decision record.

## Licensing Documents

Whenever adding external assets or AI-generated assets, update:

```text
docs/licenses/ASSET_REGISTER.md
```

Never assume that a free asset is commercially usable without checking and recording the license.
