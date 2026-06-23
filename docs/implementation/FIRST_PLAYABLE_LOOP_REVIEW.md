# First Playable Loop Review

Date: 2026-06-23
Status: Complete
Reviewed Scene: `Assets/_Project/Scenes/Prototype/PrototypeMineGrid.unity`
Reviewed Scope: 1-A through 1-M

## Purpose

Review whether the current prototype forms a playable first expedition loop before adding more content.

The reviewed loop is:

1. Generate a mine grid.
2. Render the mine grid with Tilemap.
3. Move the player on passable cells.
4. Mine adjacent wall cells.
5. Move through newly mined paths.
6. Encounter prototype enemies.
7. Resolve basic automatic attacks.
8. Take enemy contact damage.
9. Pick up treasure.
10. Return to the extraction marker.

## Explicit Exclusions

This review does not add or evaluate:

- Campaign progression.
- Expedition result screens.
- Persistent rewards.
- Weapon upgrades.
- Level-up choices.
- Final art, animation, VFX, or audio polish.
- Additional enemy types.
- Additional treasure types.
- Balance for a 15-minute session.

## Review Setup

Use the current `PrototypeMineGrid` scene.

Record any temporary Inspector changes here:

| Setting | Value | Notes |
|---|---:|---|
| Mine seed | 123321 |  |
| Width | 32 |  |
| Height | 26 |  |
| Random floor percent | 50 |  |
| Wall durability | 5 |  |
| Player move speed | 3 |  |
| Mining interval | 0.25 |  |
| Mining damage | 1 |  |
| Enemy count/settings | 3 |  |
| Required treasure value for extraction | 1 |  |

## Smoke Test Checklist

| Check | Result | Notes |
|---|---|---|
| Scene opens without Console compile errors | Pass |  |
| MineGrid is generated on Play Mode start | Pass |  |
| Tilemap terrain is visible and readable | Pass |  |
| Player starts on a passable cell | Pass |  |
| Player can move through floor cells | Pass |  |
| Player cannot move through wall cells | Pass |  |
| Player cannot move outside the MineGrid | Pass |  |
| Mining damages adjacent wall cells | Pass |  |
| Wall turns into floor at zero durability | Pass |  |
| Tilemap updates after mining | Pass |  |
| Player can move into mined cells | Pass |  |
| Enemies spawn visibly | Pass |  |
| Enemies move toward the player | Pass |  |
| Player auto-attack damages enemies | Pass |  |
| Enemies disappear or disable on defeat | Pass |  |
| Enemy contact damages player | Pass |  |
| Player defeat disables player actions | Pass |  |
| Treasures spawn visibly | Pass |  |
| Player picks up treasure on the same grid cell | Pass |  |
| Treasure count/value updates in Inspector | Pass |  |
| Extraction fails before required treasure is collected | Pass |  |
| Extraction succeeds after required treasure is collected | Pass |  |
| Extraction disables configured player actions | Pass |  |

Use one of:

- `Pass`
- `Fail`
- `Partial`
- `Not Tested`

## End-to-End Loop Runs

Run at least three short sessions.

| Run | Seed/Settings | Outcome | Notes |
|---:|---|---|---|
| 1 | Default scene settings | Pass | Completed treasure pickup and extraction. No blocking issue observed. |
| 2 | Alternate seed | Pass | Loop completed on a different layout. Mining remained useful. |
| 3 | Higher wall density or lower open space | Pass | Loop completed, but wall durability readability felt weak. |

## Review Questions

### Mining

| Question | Answer | Notes |
|---|---|---|
| Is mining readable? | Partial | The mined wall eventually breaks, but intermediate durability is not visible. |
| Does mining feel like a useful action instead of only a delay? | Yes |  |
| Does mining change movement decisions? | Yes |  |
| Is wall durability understandable enough for a prototype? | No | There is no clear feedback that repeated mining is making progress. |

### Movement

| Question | Answer | Notes |
|---|---|---|
| Does movement feel responsive enough for prototype validation? | Yes |  |
| Are blocked cells understandable? | Yes |  |
| Does collision feel predictable? | Yes |  |
| Does diagonal movement cause confusing cell transitions? | No |  |

### Combat

| Question | Answer | Notes |
|---|---|---|
| Is automatic attack compatible with mining movement? | Yes |  |
| Do enemies create enough pressure to make mining decisions matter? | Partial | Enemies create pressure, but movement/pathing can reduce pressure when walls block direct routes. |
| Is contact damage understandable? | No | Health changes are not readable without Inspector or defeat state. |
| Does player defeat stop the loop cleanly? | Partial | Player actions stop, but there is no clear defeat result presentation. |

### Treasure and Extraction

| Question | Answer | Notes |
|---|---|---|
| Is treasure pickup understandable? | Yes |  |
| Does extraction create a clear short-term objective? | Partial | The marker exists, but the requirement and unavailable state are not readable enough. |
| Is extraction failure before treasure clear enough? | No | There is no explanation about the goal or condition for extraction. |
| Is extraction completion clear enough? | Partial | Player actions stop, but there is no clear completion result presentation. |

## Confirmed Problems

List issues that were directly reproduced.

| Severity | Problem | Reproduction | Suggested Next Action |
|---|---|---|---|
| P2 | Wall durability is not readable while mining. | Mine a high-durability wall and observe that progress is not visible before the wall breaks. | Add minimal wall mining feedback, such as a durability overlay, hit flash, or temporary debug indicator. |
| P2 | Contact damage is not understandable during play. | Let an enemy touch the player and observe that damage is only clear through Inspector state or defeat. | Add minimal health UI and/or damage feedback. |
| P2 | Extraction failure before treasure pickup is not clear. | Stand on the extraction marker before collecting required treasure. | Add minimal extraction state feedback, such as "Need treasure" debug UI. |
| P2 | Extraction completion is only partially clear. | Complete extraction and observe that completion is not communicated through a dedicated loop result. | Add minimal extraction complete UI or scene message. |
| P2 | Current status UI is missing. | Play without watching Inspector values. | Add prototype HUD for health, treasure value, extraction requirement, and loop result. |

Severity values:

- `P0`: blocks Play Mode or basic loop completion.
- `P1`: breaks a core loop behavior.
- `P2`: confusing or weak but playable.
- `P3`: polish or follow-up.

## Likely Risks

List risks inferred from play, code structure, or current prototype limitations.

| Risk | Why It Matters | Suggested Mitigation |
|---|---|---|
| Player and enemy sprites can visually overlap. | It can be hard to read how many enemies are present and where the player is. | Improve sprite sorting, visual separation, or later grid occupancy rules. |
| Enemy movement is not smooth. | It can make combat pressure feel rough even when the loop technically works. | Defer to a tuning or enemy movement pass after feedback UI is added. |
| Enemies can stop applying pressure when walls block the shortest route. | Mining decisions are less meaningful if enemy pressure disappears unpredictably. | Consider wall-aware movement or simple repathing in a later combat/navigation pass. |
| Wall mining progress is invisible. | The player cannot tell whether repeated mining is working until the wall breaks. | Add durability overlay, hit feedback, or a simple wall damage state indicator. |
| There is no status UI for core loop state. | The player cannot read health, treasure value, extraction availability, or loop result without Inspector. | Make 1-O a prototype loop feedback pass. |

## Runtime Verification Needed

List behavior that still needs manual or PlayMode verification.

| Item | Reason | Suggested Verification |
|---|---|---|
| Player defeat and extraction completion should eventually have PlayMode coverage. | These are Unity lifecycle/component wiring behaviors, not pure domain rules. | Add PlayMode tests after the prototype UI/loop feedback pass stabilizes. |
| Enemy movement around walls needs focused verification. | Current behavior can make enemy pressure inconsistent. | Test several wall layouts and decide whether wall-aware movement is needed before combat tuning. |

## Review Decision

Choose one after the review.

| Decision Item | Value |
|---|---|
| Loop verdict | Playable with issues |
| Next implementation step | 1-O Prototype Loop Feedback Pass |
| Must-fix before content expansion | Minimal feedback for health, treasure value, extraction availability, extraction failure/completion, and wall mining progress. |
| Can defer until later prototype | Smooth enemy movement, wall-aware enemy navigation, final UI art, full result screen, campaign reward settlement, upgrade selection. |

Suggested loop verdict values:

- `Playable`: the loop can move to the next implementation sequence.
- `Playable with issues`: the loop works, but 1-2 focused fixes should come first.
- `Not yet playable`: core behavior blocks the loop and should be fixed before new systems.

## Recommended Next Step

Recommended next step:

### 1-O. Prototype Loop Feedback Pass

Goal:

- Add minimal on-screen feedback for health, treasure value, extraction state, player defeat, extraction completion, and wall mining progress.

Rationale:

- The current loop is playable end-to-end.
- The biggest blockers are readability and feedback, not missing core mechanics.
- Adding upgrades or more content now would make later tuning harder because the player still cannot read enough state during play.

Explicit exclusions:

- No final UI art.
- No full expedition result screen.
- No campaign reward settlement.
- No upgrade selection.
- No localization system.

## Verification Log

Record commands and results.

| Command | Result | Notes |
|---|---|---|
| `.\tools\verify-project.ps1` | Pass |  |
| `.\tools\test-editmode.ps1` | Pass | 133 tests passed. |
| `git diff --check` | Pass |  |

## Final Notes

What currently works:

- The prototype loop can be completed from mine generation through treasure pickup and extraction.
- Movement, mining, combat, player damage, treasure pickup, and extraction are connected well enough for prototype validation.

What feels weak:

- Wall durability is not readable while mining.
- Player health, treasure value, extraction availability, extraction failure, and extraction completion are not readable enough without Inspector checks.
- Contact damage is not clear during play.
- Enemy pressure and movement need later tuning.

What should be fixed next:

- Add minimal on-screen prototype feedback for health, treasure value, extraction state, player defeat, and extraction completion.

What should stay deferred:

- Full expedition result screen.
- Campaign rewards and persistence.
- Upgrade selection.
- Final art, animation, VFX, and audio polish.