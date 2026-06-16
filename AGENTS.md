# AGENTS.md — Deep Seal Project Instructions

This file defines how Codex and human contributors should work in this repository.

## Project Identity

Deep Seal is a single-player expedition-management survival roguelite made with Unity 6.3 LTS.

Core concept:
- Automatic combat similar to Vampire Survivors.
- Procedurally generated mining maps.
- Campaign progression through repeated expeditions.
- Sealstones accumulated across multiple expeditions.
- Miners can be injured, go missing, be rescued, or die permanently only in limited high-risk cases.
- The implementation target is a 2D tile-based Unity game with URP 2D visuals.
- Multiplayer is out of scope for the initial project.
- Complex fluid simulation such as spreading lava or physically flowing water is out of scope for the initial project.


## Manual Application Workflow — Locked

The default workflow for this repository is **proposal-only implementation**.

Codex must not directly create, modify, delete, or move project files unless the project owner explicitly asks for direct file edits in that specific task.

For normal development tasks, Codex should follow this loop:

1. Read the relevant design and architecture documents.
2. Inspect the current files needed to understand the task.
3. Propose the implementation as code blocks, file paths, diffs, or step-by-step edit instructions.
4. Explain where each file should be placed and why the structure fits the project hierarchy.
5. Wait for the project owner to manually apply the changes.
6. After the owner reports the changes are applied, review the resulting code, Unity Console output, Git diff, or verification logs.
7. Only then suggest the next development step.

Codex may still read files, review diffs, explain errors, design APIs, write sample code, and recommend commands. Running verification commands is allowed only when it does not require modifying project files, or when the project owner explicitly permits it for that review step.

When proposing code, include complete file contents for new small files, or clearly bounded replacement snippets for existing files. Avoid vague instructions such as “update the class accordingly.”

If direct editing would be useful, Codex must ask first and state exactly which files it wants to change.

## Asset and Image Request Rule — Locked

When an implementation step requires new visual, audio, UI, font, animation, VFX, or other external assets, Codex must not use placeholders silently and must not import random assets.

Codex must first provide an explicit asset request specification for the project owner.

Each asset request must include:

* Purpose: where and why the asset is needed.
* Asset type: sprite, tile, tileset, portrait, icon, UI panel, VFX texture, sound effect, music loop, font, shader, material, or other.
* Required file format: for example PNG, WAV, OGG, TTF, FBX, PSD source, or Unity prefab.
* Recommended resolution or size:

  * sprites: pixel size and pixels-per-unit assumption;
  * tiles: tile size, tile count, and atlas layout;
  * UI: target canvas size and scaling expectation;
  * audio: sample rate, bit depth, mono/stereo, loop requirement.
* Orientation and pivot:

  * character sprite pivot;
  * projectile direction;
  * tile origin;
  * UI anchor expectation.
* Animation requirements, if any:

  * frame count;
  * directions;
  * frame rate;
  * loop or one-shot behavior.
* Style constraints:

  * visual mood;
  * palette direction;
  * outline or no outline;
  * readability constraints;
  * relationship to the current GDD art direction.
* Implementation constraints:

  * expected folder path under `Assets/_Project`;
  * whether the asset is temporary prototype content or intended production content;
  * whether a placeholder is acceptable.
* Licensing constraints:

  * commercial use must be allowed;
  * attribution requirements must be recorded;
  * third-party or AI-generated assets must be documented in `docs/licenses/ASSET_REGISTER.md`.

Codex must present the asset request as a proposal only. The project owner will create, purchase, generate, or import the asset manually.

If a placeholder is acceptable for a prototype step, Codex must explicitly state the placeholder requirements and the later replacement criteria.


## Primary Source Documents

Before changing design-sensitive code, read these documents:

- `docs/gdd/GDD_MASTER.md`
- `docs/gdd/01_design_pillars.md`
- `docs/gdd/03_expedition_loop.md`
- `docs/gdd/04_combat_and_mining.md`
- `docs/gdd/05_encounters_and_counters.md`
- `docs/gdd/06_sub_dungeons.md`
- `docs/gdd/07_miners_failure_and_death.md`
- `docs/gdd/11_scope_mvp_roadmap.md`
- `docs/decisions/0001-initial-project-direction.md`

If code and design documents conflict, pause and report the conflict before changing the design direction.

## Current Development Phase

The project is in bootstrap/prototype preparation.

The first playable prototype should validate:

1. Player movement.
2. Cell-based mining.
3. Basic procedural map generation.
4. Automatic combat.
5. Enemy spawning.
6. Treasure pickup.
7. Extraction/return flow.

Do not implement campaign-scale complexity before the first prototype loop works.

## Repository Structure

Expected high-level structure:

- `Assets/_Project/`: First-party Unity assets and source code.
- `Assets/ThirdParty/`: Asset Store or external Unity packages/assets.
- `ArtSource/`: Source art, AI generations, prompts, editable originals, and references.
- `docs/`: Design, architecture, decisions, testing, and licensing documents.
- `tools/`: PowerShell scripts for local verification, testing, and builds.
- `.codex/`: Project-scoped Codex configuration.

Do not move Unity-generated folders such as `Assets`, `Packages`, or `ProjectSettings`.

## Unity Rules

Use Unity 6.3 LTS.

Keep these project settings intact unless explicitly asked:

- Version Control Mode: Visible Meta Files.
- Asset Serialization Mode: Force Text.
- Render pipeline: URP 2D.
- Main target platform: Windows x86_64.

Never commit or intentionally edit these generated/local folders:

- `Library/`
- `Temp/`
- `Obj/`
- `Logs/`
- `UserSettings/`
- `Build/`
- `Builds/`
- `TestResults/`
- `CoverageResults/`

Do not manually edit Unity scene, prefab, material, or ProjectSettings YAML unless the task explicitly requires it and the diff is small and explainable.

Prefer generating or modifying Unity assets through Unity Editor scripts when GUID/fileID correctness matters.

Do not import large third-party packages, change render pipelines, add Steam integration, or add new package dependencies without explicit approval.

## Git Rules

Work in small, reviewable changes.

Before starting a task, inspect the current branch and status:

```powershell
git status
git branch --show-current
```

Do not overwrite user changes. If there are unrelated uncommitted changes, report them.

Use conventional commit-style messages when suggesting commits:

- `chore:` project setup, scripts, configuration.
- `docs:` documentation-only changes.
- `feat:` new player-visible or developer-facing functionality.
- `fix:` bug fixes.
- `refactor:` structure changes without behavior change.
- `test:` tests only.

## Coding Rules

First-party runtime code should live under `Assets/_Project/Code/Runtime/`.

Editor-only code should live under `Assets/_Project/Code/Editor/`.

Tests should live under `Assets/_Project/Code/Tests/`.

Use the root namespace `DeepSeal` for runtime code.

Use `DeepSeal.EditorBuild` or `DeepSeal.EditorTools` for editor-only build and tooling scripts.

Prefer small pure C# classes for core logic, with MonoBehaviours used as Unity adapters. This keeps procedural generation, mining logic, combat calculations, and campaign rules testable outside scenes.

Avoid unnecessary inheritance hierarchies. Prefer composition and explicit data structures.

Avoid per-frame allocations in gameplay loops. Do not use LINQ in `Update`, `FixedUpdate`, tight enemy loops, or procedural generation hot paths unless the allocation cost is measured and acceptable.

Do not introduce global singletons or service locators during the prototype phase unless explicitly approved.

## Design Implementation Rules

When implementing gameplay, preserve these design principles:

- Mining must affect combat and navigation.
- Combat upgrades and mining upgrades should not feel like separate disconnected systems.
- Named elites and bosses should use soft counters, not hard counters.
- Unfavorable preparation should make combat harder, not impossible.
- Retreat and extraction are valid outcomes.
- Failure should create injury, missing, rescue, or recovery states before permanent death in normal expeditions.
- Randomness should be controlled and readable through scouting information, traces, encounter hints, or local adaptation opportunities.

## Documentation Rules

If a task changes design direction, update one of:

- `docs/gdd/*`
- `docs/decisions/*`
- `docs/architecture/*`

Use `docs/decisions/` for decisions that explain why a direction changed.

Do not silently remove design scope. Mark it as `OUT`, `DEFERRED`, `DRAFT`, or `LOCKED` instead.

## Verification Rules

After code or Unity configuration changes, run the most relevant script from `tools/` if available:

```powershell
.\tools\verify-project.ps1
.\tools\test-editmode.ps1
.\tools\test-playmode.ps1
.\tools\build-windows.ps1
```

If Unity is not installed at the expected path or a script cannot run locally, report that clearly and include what was not verified.

## Response Rules for Codex

When finishing a task, report:

1. What changed.
2. Why the change was made.
3. Files changed.
4. Verification performed.
5. Any risks, assumptions, or follow-up work.

For code changes, explain the purpose of new classes, important methods, and important fields/variables so the project owner can follow the implementation.
