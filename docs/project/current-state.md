# Current Project State

- Status: Active evidence baseline
- Snapshot date: 2026-09-03
- Snapshot implementation commit: `46cd937`
- Target Unity version: `6000.3.17f1` (`cf0352b38e81`)
- Active gameplay track: 3-E Movement Pacing and Map Openness Pass
- Current gameplay owner: ZCode
- Active plan: [`../roadmap/next-milestone.md`](../roadmap/next-milestone.md)
- Historical step detail: [`../implementation/PROTOTYPE_ROADMAP.md`](../implementation/PROTOTYPE_ROADMAP.md)

이 문서는 현재 repository에서 확인되는 구현, verification과 active-work snapshot만
소유한다. Game rule과 future design은 GDD와 approved ADR/roadmap이 소유한다.

## Active work and Git state

| Item | State |
| --- | --- |
| 3-E-1 | Verified and integrated through `0cb910f`; findings recorded in `6aec973` |
| Remaining 3-E | `Awaiting Proposal`; exact next work-unit scope is not recorded in repository docs |
| 3-F | Planned after reviewed 3-E completion |
| Integration checkout | `G:/unity_workspace/deep-seal`, clean at the W0-C branch point |
| Codex worktree | `G:/unity_workspace/deep-seal-codex`, W0-C owner worktree |
| ZCode worktree | Not registered in `git worktree list`; required before the next ZCode-owned write task |

W0-A agent workflow governance was integrated as `0f38c40`. W0-B documentation lifecycle
was integrated as `d38cb90`. W0-C evidence synchronization was integrated as `46cd937`;
later metadata-only documentation commits do not change that gameplay implementation baseline.

## Implemented prototype scope

### Pure runtime rules

| Area | Confirmed implementation |
| --- | --- |
| `DeepSeal.Core` | Integer grid position and direction primitives |
| `DeepSeal.Mining` | Terrain semantics, mutable mine-grid state, passability and mining result rules |
| `DeepSeal.ProceduralGeneration` | Seeded scatter/cavern generation, irregular `Void` footprint, validation, terrain presets, four prototype biome definitions and deterministic selection |
| `DeepSeal.Combat` | Targeting, health/damage, enemy movement/pathfinding/spawn selection, projectile/area attack traces, charger/ranged counterplay and time-based threat scaling |
| `DeepSeal.Expedition` | Treasure, reward drop and extraction state/rules, reachable spawn placement and depth-tier treasure scaling |
| `DeepSeal.Upgrades` | Prototype reward-funded upgrade options, spending state and purchase rules |

Pure rule source is under `Assets/_Project/Code/Runtime/` and remains independent from scene
and prefab ownership except through the `DeepSeal.UnityAdapters.*` boundary.

### Unity adapters and player-visible loop

- Seeded mine generation and Tilemap rendering
- Biome tint presentation and biome/seed HUD display
- Keyboard-based prototype movement and adjacent-wall mining
- Camera follow and prototype-scale framing
- Automatic nearest/projectile/area attacks
- Enemy runtime spawning, pathfinding, charger/ranged behavior, first named elite and nameplate
- Player health/contact damage and defeat handling
- Treasure pickup, reward drops, extraction and temporary upgrade selection
- Threat, treasure, extraction, elite and mining-progress feedback through prototype OnGUI/UI adapters

The repository contains two tracked prototype scenes:

- `Assets/_Project/Scenes/Prototype/ExpeditionPrototype.unity`
- `Assets/_Project/Scenes/Prototype/PrototypeMineGrid.unity`

It contains four tracked prototype prefabs for enemy, treasure, reward drop and extraction
marker presentation.

## Tracked inventory at snapshot

| Item | Count | Meaning |
| --- | ---: | --- |
| Runtime C# files | 86 | Pure rule and Unity adapter source combined |
| EditMode test C# files | 31 | No tracked PlayMode test source/assembly |
| `[Test]`/`[TestCase]` declaration lines | 263 | Source inventory, not an executed test count |
| Runtime asmdef | 1 | `DeepSeal.Runtime.asmdef` |
| Test asmdef | 1 | `DeepSeal.Tests.EditMode.asmdef` |
| Prototype scenes | 2 | Both tracked under `Scenes/Prototype` |
| Prototype prefabs | 4 | Enemy, treasure, reward drop and extraction marker |
| Tracked prototype PNG outputs | 11 | Includes tile, character, item, VFX/UI placeholders |
| Tracked prototype WAV outputs | 1 | Mining-hit placeholder |

These counts are a navigation aid and must be refreshed when the corresponding tree changes.

## Verification baseline

### Confirmed evidence

| Date | Level | Evidence | Result | Scope limitation |
| --- | --- | --- | --- | --- |
| 2026-09-01 | Unity batch open | `Logs/verify-project.log` in the integration checkout | Return code 0 | Predates later progression and 3-E-1 commits |
| 2026-09-01 | EditMode | `TestResults/editmode-results.xml` | 241/241 passed, 0 failed | Predates later source/test changes; not current-tree proof |
| 2026-09-02 | Manual play | 3-E-1 interim findings in the historical roadmap | Map scale, relative sizing, movement pacing, spawn grace and pathfinding budget observed as working | Does not replace current compilation/EditMode evidence or long-run performance testing |
| 2026-09-03 | Documentation static review | W0-A/W0-B/W0-C staged paths, links, whitespace, source-count and `.meta` GUID checks | Passed | Documentation/workflow scope only |

### Verification still needed

- Current `origin/main` Unity compilation and full EditMode run after the latest gameplay changes
- PlayMode coverage for scene lifecycle, defeat/extraction, spawning and presentation wiring
- Longer/harsher 3-E performance runs for spawn pressure
- Windows development build evidence for a future milestone gate

Application launch or a short manual play session alone must not be recorded as full compile/test
verification.

## Current constraints and known risks

### Confirmed prototype constraints

- Input still uses direct keyboard polling in prototype adapters rather than an approved Input
  Action contract.
- Player-facing status and upgrade selection still depend on temporary OnGUI/prototype UI.
- Tuning values remain distributed across scene/prefab Inspector fields rather than validated
  balance data assets.
- Runtime code currently uses one broad `DeepSeal.Runtime` assembly; further splitting is not
  justified until dependency or compile-time evidence requires it.
- No tracked PlayMode test assembly exists.
- Treasure/reward/extraction/upgrades do not implement inventory, campaign settlement, save or
  permanent progression.
- Current art/audio files are explicit prototype placeholders; global PPU and character canvas
  are not approved production contracts.

### 3-E findings awaiting disposition

- Camera framing shows too much of the enlarged map and needs a later local-view adjustment.
- Enemy spawn style requires a dedicated review; tested spawn ticks showed no frame drop, but
  long-run pressure is unverified.
- Treasure placement and balance are deferred to a later tuning pass.

### Documentation boundary

- `docs/implementation/PROTOTYPE_ROADMAP.md` is now a historical detailed index, not the active
  execution source.
- Existing completed steps have not been retroactively converted into milestone archives because
  their full required evidence was not revalidated.
- GDD `LOCKED` rules remain authoritative for game direction; W0 did not change gameplay or
  concept.

## Next gate

Before ZCode starts the remaining 3-E work:

1. Create or select a dedicated clean ZCode worktree based on current `origin/main`.
2. Use a new `zcode/<short-scope>` branch for one bounded work unit.
3. Read root `AGENTS.md`, then the nearest scoped instruction files.
4. Propose the exact 3-E goal, affected files, exclusions and verification gate.
5. Obtain user approval before code, scene, prefab, asset or ProjectSettings application.

Do not begin 3-F until 3-E completion and unresolved findings have been reviewed and documented.
