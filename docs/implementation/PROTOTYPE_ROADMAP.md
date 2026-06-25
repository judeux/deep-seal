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
| 2-C  | Terrain Wall Type Pass | Planned | Introduce terrain distinctions needed for mineable walls, unmineable walls, boundary walls, and future wall variants. |
| 2-D  | Procedural Preset Placement Pass | Planned | Blend hand-authored terrain presets into generated mine maps after base shape and terrain semantics are stable. |

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

---

## Current Step

### 2-C. Terrain Wall Type Pass

Goal:

* Expand terrain semantics enough to distinguish mineable walls, unmineable walls, boundary walls, and future wall durability/material variants.
* Keep the connected mine shape generator compatible with the expanded terrain model.
* Preserve movement, mining, enemy navigation, spawning, reward drops, treasure pickup, extraction, and upgrade behavior.

Explicit exclusions:

* No hand-authored preset or vault injection yet.
* No biome system.
* No minimap or exploration UI yet.
* No final terrain art pass.

---

## Next Planned Step

### 2-D. Procedural Preset Placement Pass

Goal:

* Blend hand-authored terrain presets, rooms, or vault-like chunks into seed-based generated maps after terrain semantics are stable.

Notes:

* Preset placement should wait until terrain wall semantics are stable enough to describe mineable, unmineable, and boundary cells.
* Minimap or exploration map UI remains deferred until generation, terrain semantics, and discovery rules are more stable.

---

## Later Prototype Steps

### 1-H. Basic Enemy Domain

Goal:

* Add pure C# enemy state and simple movement rules.

Explicit exclusions:

* No Unity rendering yet.
* No combat damage yet.
* No animation.

---

### 1-I. Enemy Unity Adapter

Goal:

* Spawn and display simple enemies in the prototype scene.

Explicit exclusions:

* No complex AI.
* No pathfinding.
* No named elites.
* No boss logic.

---

### 1-J. Basic Automatic Attack

Goal:

* Add a minimal automatic attack loop against nearby enemies.

Explicit exclusions:

* No weapon evolution.
* No upgrade selection.
* No VFX polish.
* No audio.

---

### 1-K. Damage and Death Loop

Goal:

* Add minimal health, damage, enemy death, and player damage.

Explicit exclusions:

* No campaign injury or missing miner system yet.
* No permanent death yet.

---

### 1-L. Treasure Pickup

Goal:

* Add a simple collectible treasure or resource marker.

Explicit exclusions:

* No full inventory.
* No economy.
* No museum.
* No campaign persistence.

---

### 1-M. Extraction Marker

Goal:

* Add a simple extraction/return marker to complete the first prototype loop.

Explicit exclusions:

* No final dungeon.
* No sealstone campaign.
* No expedition result screen beyond a prototype debug result.

---

### 1-N. First Playable Loop Review

Goal:

* Review the first playable prototype and decide whether mining, movement, combat, pickup, and extraction are worth expanding.

Review questions:

* Is mining readable?
* Does mining change movement decisions?
* Does the player understand passable and blocked terrain?
* Is automatic combat compatible with mining?
* Is the prototype loop worth expanding?
* Which systems should be cut, simplified, or expanded?
