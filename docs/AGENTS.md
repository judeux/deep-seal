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

## Asset Request Documentation Rule

When a task requires new assets, Codex must document the request clearly instead of assuming that the asset already exists.

Asset-related requests should be written in a reusable format so the project owner can later create the asset manually, purchase it from an asset store, generate it with AI, or commission it.

If the asset affects design direction, Codex should also suggest whether the relevant GDD document should be updated.

Examples:

* A new miner sprite requirement may affect `docs/gdd/08_world_art_and_audio.md`.
* A terrain tileset requirement may affect `docs/gdd/10_procedural_generation.md`.
* A UI icon requirement may affect future UI documentation.
* A third-party or AI-generated asset must be recorded in `docs/licenses/ASSET_REGISTER.md`.

Asset requests should not be mixed into code-only implementation instructions without a clear heading.

## Architecture Documents

Architecture documents should describe code boundaries, responsibilities, and dependencies.

Do not use architecture documents to introduce large new systems unless they are tied to the current MVP or a decision record.

## Licensing Documents

Whenever adding external assets or AI-generated assets, update:

```text
docs/licenses/ASSET_REGISTER.md
```

Never assume that a free asset is commercially usable without checking and recording the license.

## Prototype Roadmap Maintenance Rule

`docs/implementation/PROTOTYPE_ROADMAP.md` must be updated when a prototype step is completed, skipped, split, merged, or re-scoped.

Each roadmap update should include:

* current step status;
* completed behavior;
* explicit exclusions that remain out of scope;
* next planned step;
* verification results or expected verification method;
* suggested commit message if relevant.

Do not rewrite the full roadmap for small updates. Prefer small, targeted edits.

