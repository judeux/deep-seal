# 0006. Prototype Player Movement

Date: 2026-06-18

## Decision

The first interactive prototype movement will be implemented as a Unity adapter over the existing pure C# MineGrid domain model.

The prototype player movement uses:

- `PrototypeMineGridBootstrap` as an explicit scene reference for the current generated MineGrid.
- `GridCoordinateConverter` for prototype GridPosition to Unity world coordinate conversion.
- `PrototypePlayerMovement` as a temporary MonoBehaviour movement adapter.
- `Keyboard.current` for minimal WASD and arrow key input.
- MineGrid terrain passability to block movement into walls and out-of-bounds cells.

The movement implementation does not use Rigidbody2D, CharacterController, NavMesh, Cinemachine, global singletons, service locators, or event buses.

## Rationale

The prototype needs the smallest interactive step after Tilemap rendering: verifying that generated terrain affects navigation.

Movement should depend on the domain grid state, not on Tilemap collider state, so later mining can update the MineGrid first and then refresh the Tilemap as a view.

Keeping input and Transform movement in Unity adapter code preserves the existing boundary: Core, Mining, and ProceduralGeneration remain independent from UnityEngine.

## Consequences

The current movement is prototype-only.

Known limitations:

- Input uses `Keyboard.current` directly and should later move to InputActions.
- Collision is point-based using the player's current world position.
- The coordinate rule assumes prototype Tilemap origin at `(0, 0)` and cell size `1`.
- No animation, physics, combat, mining input, treasure, or extraction behavior is included.

Future work should add mining input by applying `MiningRules` to the current MineGrid and refreshing the Tilemap through the Unity adapter layer.

## Status

Accepted.