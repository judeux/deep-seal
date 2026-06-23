## Current Development Phase

Interactive prototype foundation.

Implemented:
- Core grid primitives.
- Basic mining domain model.
- Seed-based procedural mine grid generation.
- Basic generated mine grid validation.
- Unity Tilemap rendering adapter for generated MineGrid data.
- Prototype scene that displays a seed-based mine grid through Tilemap.
- Prototype player movement over passable MineGrid cells.
- Wall collision based on MineGrid terrain passability.
- Player mining input and runtime Tilemap refresh after terrain changes.
- Prototype camera follow.
- Pure C# enemy state and MineGrid-based enemy movement rules.
- Prototype enemy spawning and Unity view adapter.
- Basic automatic attack targeting and prototype enemy defeat/removal.
- EditMode tests for core, mining, procedural generation, enemy movement, and attack targeting rules.
- Prototype player health, enemy contact damage, and player defeat handling.
- Prototype treasure spawning and grid-position pickup tracking.
- Prototype extraction marker and simple return completion tracking.

Not implemented yet:
- Weapon definitions, upgrades, combat UI, and polished combat feedback.
- Full expedition result screen, campaign rewards, and persistence.

Next:
- Add minimal prototype loop feedback for health, treasure value, extraction state, player defeat, extraction completion, and wall mining progress.