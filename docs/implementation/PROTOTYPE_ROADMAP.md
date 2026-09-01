# Prototype Implementation Roadmap

This document defines the current step-by-step implementation plan for the Deep Seal playable prototype.

Codex must use this document as the primary source of truth when proposing the next implementation task.

## Current Prototype Goal

Build the smallest playable expedition prototype that proves the following loop:

1. Generate a mine grid.
2. Render the mine grid with Unity Tilemap.
3. Move a player on passable cells.
4. Mine adjacent wall cells.
5. Update terrain after mining.
6. Allow the player to move through newly mined paths.
7. Validate basic enemies, player damage, and automatic combat.
8. Add treasure pickup and extraction later.

This roadmap is not the full game roadmap. It is the first playable prototype roadmap.

---

## Working Rules

* Follow the proposal-only workflow.
* Codex must not create, edit, delete, or move files unless the project owner explicitly asks for direct edits in that task.
* Each implementation step should be small enough to review manually.
* Each step should have a clear goal, explicit exclusions, verification steps, and a suggested commit message.
* Do not skip ahead to later systems unless the project owner explicitly changes the plan.
* Do not introduce multiplayer, Steamworks, Addressables, complex AI, advanced animation, campaign management, or meta progression during this prototype sequence.
* Keep pure domain code independent from `UnityEngine`.
* Use Unity adapters only for scene, input, rendering, Tilemap, and MonoBehaviour integration.
* If assets are required, provide a detailed asset request specification before assuming or importing assets.

---

## Step Status

| Step | Name                           | Status      | Summary                                                                               |
| ---- | ------------------------------ | ----------- | ------------------------------------------------------------------------------------- |
| 1-A  | Core Grid Primitives           | Done        | Added basic grid position and direction primitives with EditMode tests.               |
| 1-B  | Mining Domain                  | Done        | Added terrain cell, mine grid, mining rules, and mining tests.                        |
| 1-C  | Procedural Mine Generation     | Done        | Added seed-based mine grid generation and validation.                                 |
| 1-D  | Tilemap Rendering              | Done        | Added Unity adapter to render generated mine grids through Tilemap.                   |
| 1-E  | Prototype Player Movement      | Done        | Added grid-aware prototype player movement.                                           |
| 1-F  | Player Mining Input            | Done        | Connected player mining input to MiningRules, MineGrid mutation, and Tilemap refresh. |
| 1-G  | Camera and Prototype Feel Pass | Done        | Added minimal camera follow and tuned prototype scene readability.                    |
| 1-H  | Basic Enemy Domain             | Done        | Added pure C# enemy state and simple MineGrid-based movement rules.                   |
| 1-I  | Enemy Unity Adapter            | Done        | Rendered and moved simple prototype enemies in the scene.                             |
| 1-J  | Basic Automatic Attack         | Done        | Added nearest-target automatic attack and prototype enemy defeat/removal.             |
| 1-K  | Player Damage and Health Loop  | Done        | Added player health, enemy contact damage, and prototype defeat handling.             |
| 1-L  | Treasure Pickup                | Done        | Added visible prototype treasures and grid-position pickup tracking.                  |
| 1-M  | Extraction Marker              | Done        | Added a visible prototype extraction marker and simple return completion trigger.     |
| 1-N  | First Playable Loop Review     | Done        | Recorded first playable loop findings and selected the next prototype direction.      |
| 1-O  | Prototype Loop Feedback Pass   | Done        | Added minimal readable feedback for health, treasure value, extraction, loop result, and wall mining progress. |
| 1-P  | Prototype Tuning Pass          | Done        | Tuned the first playable prototype baseline and identified enemy navigation, runtime spawning, and reward feedback as the next bottlenecks. |
| 1-Q  | Enemy Navigation and Spawn Pressure Pass | Done | Added grid pathfinding for enemies, movement variation, and runtime enemy spawning based on active enemy pressure. |
| 1-R  | Prototype Reward Drops         | Done        | Added reward drops from enemy defeat and selected mining actions, with short-range pickup. |
| 2-A  | Upgrade Selection Prototype    | Done        | Added temporary reward-funded upgrade choices that modify attack, mining, and movement during a run. |
| 2-B  | Procedural Mine Shape Pass | Done | Added connected cavern-style mine generation with invisible Void footprint cells, irregular visible silhouettes, and optional mineable internal wall obstacles. |
| 2-C  | Terrain Wall Type Pass | Done | Added explicit mineable, unmineable, boundary, and void terrain semantics while preserving the prototype mining and movement loop. |
| 2-D  | Procedural Preset Placement Pass | Done | Added seed-stable pure C# terrain preset placement for generated mine maps while preserving terrain semantics and passable connectivity. |
| 2-E  | Generation Spawn Rule Review Pass | Done | Added pure expedition spawn placement rules and connected treasure/extraction fallback placement to irregular generated maps. |
| 2-F  | Generation and Spawn Tuning Review | Deferred   | Applied the runtime enemy pressure ramp only; detailed spawn, treasure, and reward tuning is deferred until core feature breadth is validated. |
| 3-A  | Attack Pattern Variety             | Done        | Added projectile and area auto attack patterns with pure trace rules and EditMode tests. |
| 3-B  | Map Variety Pass                   | Planned    | Vary map scale and shape, with a first simple biome flavor layer. |
| 3-C  | Enemy Variety Pass                 | Planned    | Add distinct enemy behaviors and a first named elite. |
| 3-D  | Progression and Difficulty Pass    | Planned    | Add in-run progression structure and a readable difficulty gradient. |
| 3-E  | Sub-Dungeon Prototype              | Planned    | Add one optional sub-dungeon and diversify biome flavor. |

---

## Completed Steps

### 1-A. Core Grid Primitives

Goal:

* Establish basic grid coordinate primitives for all later map, mining, movement, and combat logic.

Completed:

* `DeepSeal.Core.GridPosition`
* `DeepSeal.Core.GridDirection`
* EditMode tests for core grid behavior.

Notes:

* These types should remain small and stable.
* They should not depend on Unity types.

---

### 1-B. Mining Domain

Goal:

* Create the pure C# mining domain used by both procedural generation and gameplay.

Completed:

* Terrain cell type definitions.
* Terrain cell state.
* Mine grid storage and safe access.
* Mining rules and mining result logic.
* EditMode tests for mining behavior.

Notes:

* Mining logic should remain independent from Unity Tilemap.
* Unity adapters may call mining rules, but mining rules must not know about Unity objects.

---

### 1-C. Procedural Mine Generation

Goal:

* Generate deterministic prototype mine maps from seed and settings.

Completed:

* Seed-based mine generation.
* Basic generation settings.
* Generation result.
* Mine grid validation.
* EditMode tests for deterministic generation and validation.

Notes:

* Generation is intentionally simple at this stage.
* Full cave generation, biome rules, sub-dungeons, named encounters, and campaign map logic are deferred.

---

### 1-D. Tilemap Rendering

Goal:

* Display generated `MineGrid` data in the Unity prototype scene.

Completed:

* Terrain cell type to TileBase mapping.
* Tilemap renderer adapter.
* Prototype bootstrap integration.
* Prototype terrain tiles or placeholder tile assets.

Notes:

* Rendering code is allowed to depend on Unity.
* Domain code must remain independent from Unity.
* Prototype tiles are temporary and may be replaced later.

---

### 1-E. Prototype Player Movement

Goal:

* Allow a visible player object to move on the generated mine grid while respecting passable and blocked cells.

Completed:

* Prototype player movement adapter.
* Grid-aware collision or passability check.
* Basic keyboard movement.
* Manual scene setup in `ExpeditionPrototype`.

Notes:

* This is prototype input, not final input architecture.
* Movement should not include mining, combat, animation, or campaign logic.
* Keyboard-based temporary input may later be replaced with InputActions.

### 2-D. Procedural Preset Placement Pass

Goal:

* Blend hand-authored terrain patterns into seed-based generated mine maps.

Completed:

* Added pure C# terrain preset data.
* Added deterministic preset placement rules.
* Added placement validation and rollback when a preset blocks start area or disconnects passable cells.
* Integrated preset placement into connected cavern generation before boundary/rind shell construction.
* Added EditMode tests for direct placement, deterministic placement, generator validity, and connectivity preservation.

Notes:

* Presets are currently code-authored prototype data, not Unity assets or Tilemap chunks.
* Presets place only Floor, MineableWall, and UnmineableWall cells.
* Void and BoundaryWall remain controlled by the generator footprint and shell rules.

### 2-E. Generation Spawn Rule Review Pass

Goal:

* Keep treasure, extraction, reward, and enemy spawn placement compatible with irregular generated maps and terrain presets.

Completed:

* Added pure C# expedition spawn placement rules.
* Added passable, occupied, distance, and reachability checks for expedition object placement.
* Updated treasure and extraction marker spawners to fall back to valid reachable floor cells.
* Reused the same passable spawn validation for reward drops.
* Added terrain semantics coverage for enemy spawn rules.

Notes:

* Enemy runtime spawn remains owned by `DeepSeal.Combat.EnemySpawnRules`.
* Treasure, extraction marker, and reward drop placement are expedition loop concerns and use `DeepSeal.Expedition` rules.
* Spawn tables, biome-specific weighting, and final content authoring remain deferred.

---

### 3-A. Attack Pattern Variety

Goal:

* Add distinct automatic attack patterns beyond the adjacent auto-attack.
* Keep new attack patterns compatible with mining terrain, movement, and the existing upgrade loop.

Completed:

* Added pure projectile trace rules that decide a straight cardinal flight at fire time and stop at the first wall, the first enemy on the path, or the range limit.
* Added pure area attack rules that affect every enemy within Manhattan range.
* Added a traveling projectile view with an explicit white-square placeholder sprite.
* Extended the prototype auto attack adapter with a Nearest, Projectile, and Area pattern selector.
* Added EditMode tests for projectile tracing and area collection.

Notes:

* Projectile flights are blocked by walls, so mining opens firing lines; the combat-mining link is validated.
* The Nearest and Area patterns have no visible attack motion, effect, or range display yet; a combat feedback pass is deferred until after the feature breadth steps.
* The projectile placeholder sprite must be replaced by a real asset before production content.

---

## Current Step

### 3-B. Map Variety Pass

Goal:

* Vary map scale and cavern shape across runs.
* Add a first simple biome flavor layer over the existing terrain generation.

Explicit exclusions:

* No biome-specific spawn tables or encounters.
* No minimap or exploration UI.
* No multi-floor or depth descent structure yet.

---

## Next Planned Step

### 3-C. Enemy Variety Pass

Goal:

* Add distinct enemy behaviors such as ranged or charging enemies.
* Add a first named elite with soft counterplay.

Explicit exclusions:

* No boss logic yet.
* No hand-made combination pools or forbidden combo validation yet.

---

## Later Prototype Steps

### 3-D. Progression and Difficulty Pass

Goal:

* Add in-run progression structure and a readable difficulty gradient.

Explicit exclusions:

* No campaign persistence or meta progression.
* No injury, missing, or rescue states yet.

### 3-E. Sub-Dungeon Prototype

Goal:

* Add one optional sub-dungeon prototype.
* Diversify biome flavor beyond 3-B.

Explicit exclusions:

* No full sub-dungeon rotation from the GDD.
* No counter-play item rewards yet.
