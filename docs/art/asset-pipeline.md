# Deep Seal Asset Pipeline

- Status: Active workflow
- Date established: 2026-09-03
- Applies to: visual, UI, animation, VFX, audio, font and other external assets
- Related decision: [ADR-0004](../adr/0004-asset-request-specification.md)

## Purpose

이 pipeline은 prototype placeholder부터 production candidate까지 asset 요구사항,
source, Unity import, integration, license와 replacement를 추적한다. Asset을 code-only
작업에 암묵적으로 끼워 넣거나 출처가 불명확한 파일을 무작위로 import하지 않는다.

## Authority boundary

- Asset requirement와 generation/import spec은 승인된 work unit에서 owner agent가
  문서로 직접 관리한다.
- Image/audio generation, file conversion, Unity import, slicing, animation, prefab/scene
  연결은 사용자가 해당 task에서 직접 실행을 승인하지 않는 한 proposal-only다.
- Asset이 game concept, art direction, player-readable rule 또는 global technical standard를
  바꾸면 관련 GDD/ADR 승인을 먼저 받는다.
- 다른 image/audio agent에 전달하는 prompt도 이 repository의 asset spec과 같은
  source-of-truth를 사용한다.

## Asset status lifecycle

모든 retained asset은 다음 중 하나의 상태를 가진다.

- `Spec`: 요구사항만 승인되었고 usable file은 없음
- `Placeholder`: 기술적으로 유효하고 통합되었지만 시각·음향 품질은 임시
- `Candidate`: 현재 quality target 후보로 review 중
- `Approved`: 현재 milestone 또는 production quality target에 승인됨
- `Deprecated`: reference migration 또는 삭제 전까지만 유지

`Placeholder`를 final 또는 production-complete로 표현하지 않는다. 상태를 올릴 때
acceptance criteria, Unity integration과 provenance evidence를 확인한다.

## Placeholder policy

### Disposable logic spike

명시적으로 승인된 짧은 logic spike에서는 primitive, single-color sprite, debug label,
code-generated visualization 또는 temporary tone을 사용할 수 있다. Work unit에 disposable
scope와 제거 gate를 기록한다.

### Retained player-facing feature

Milestone 완료 전에 다음을 만족한다.

- 실제 PNG, WAV, font 또는 요구된 source file 사용
- Unity import setting 확인
- Prefab, scene, retained UI 또는 presentation event에 연결
- 필요한 animation/atlas/mixer 구성
- Runtime-generated player-facing placeholder 제거 또는 debug-only 격리
- Status, source, prompt/license, known defect와 replacement plan 기록

## Folder ownership

현재 first-party 구조를 유지한다.

```text
ArtSource/
├─ <Feature>/Prompts/
├─ <Feature>/References/
├─ <Feature>/Generated/
└─ <Feature>/Editable/

Assets/_Project/
├─ Art/
│  ├─ Characters/
│  ├─ Environment/
│  ├─ Items/
│  ├─ Tiles/
│  ├─ UI/
│  └─ Prototype/
├─ Audio/
│  ├─ Music/
│  ├─ SFX/
│  └─ Prototype/
└─ Prefabs/
```

Feature/faction/biome subfolder는 실제 content가 생길 때만 추가한다. 같은 source를
여러 feature folder에 복제하지 않는다.

`ArtSource/`는 original/reference/editable source를, `Assets/_Project/`는 Unity에
실제로 import하는 output을 소유한다. 두 경로의 관계를 spec과 asset register에 남긴다.

## Naming

기본 filename은 lowercase snake case와 stable English technical name을 사용한다.
Type prefix는 검색성을 실제로 높일 때 사용한다.

```text
spr_player_miner_idle.png
spr_enemy_charger_placeholder.png
spr_tile_mine_wall.png
spr_ui_extraction_icon.png
vfx_mining_hit_sheet.png
sfx_mining_stone_hit_01.wav
sfx_enemy_charger_impact_01.wav
mus_mine_exploration_01.ogg
pf_enemy_charger.prefab
atlas_mine_environment.spriteatlasv2
```

- Variant suffix는 `01`, `02`처럼 두 자리로 통일한다.
- Display text, mutable balance, locale text를 filename에 넣지 않는다.
- Technical asset path에는 space와 non-ASCII name을 새로 추가하지 않는다.
- 호환 가능한 revision은 기존 path와 `.meta` GUID를 유지한다.

## Specification before creation

Asset을 만들거나 요청하기 전에 [asset-spec-template.md](asset-spec-template.md)를 사용한다.
하나의 coherent batch는 같은 technical contract와 review gate를 공유할 때만 묶는다.

공통 필수 항목:

- Asset ID, status, feature/work unit와 gameplay purpose
- Exact filename, source path와 Unity target path
- Dimensions/duration/format와 display or trigger context
- Orientation, pivot/contact point 또는 UI anchor
- Required states, variants, animation/loop
- Unity import와 integration target
- Executable generation brief, negative prompt와 acceptance checklist
- Creator/tool/date, license/commercial use와 attribution
- Replacement path/GUID policy

## Image and sprite workflow

### Technical contract

- Exact canvas dimensions와 format
- Per-frame cell, columns/rows와 frame order
- PPU와 expected world/display size
- Transparent/opaque background와 alpha edge rule
- Safe margin, crop, pivot/contact point와 orientation
- Palette, contrast, outline, lighting와 material cue
- Anti-aliasing, filter, compression, mipmap와 mesh type
- Required variants/states와 atlas group

Deep Seal의 global PPU와 character base canvas는 아직 accepted ADR로 고정되지 않았다.
기존 16x16 prototype tile을 모든 새 asset의 전역 기준으로 사용하지 않는다. 새 family의
규격은 prototype 또는 production intent와 함께 spec에서 정하고, 전역 표준으로 승격할
때는 camera, collider, UI scale, animation과 기존 asset 재작업 영향을 ADR로 승인한다.

### AI generation handoff

- 기본적으로 한 request에 subject 하나만 생성한다.
- Existing style을 잇는 경우 approved reference와 유지할 silhouette, palette,
  perspective와 direction을 명시한다.
- Transparent output을 요청하되 alpha가 불안정하면 subject palette와 충돌하지 않는
  완전한 단색 chroma background를 지정한다.
- Transparency checkerboard, floor, ground shadow, frame, decoration, UI card, text,
  watermark, signature와 extra object를 금지한다.
- Generated dimensions와 alpha를 실제 file inspection으로 확인한다.
- Retained placeholder는 exact canvas 변환, background extraction, nearest sampling과
  최소 pixel cleanup을 허용한다.
- Production-quality pixel art는 native exact grid에서 silhouette, line weight, pixel
  cluster와 readability를 의도적으로 정리한다.

### Tile and tileset requirements

Tile spec은 일반 sprite 항목에 더해 다음을 포함한다.

- Logical tile size, PPU와 world cell size
- Terrain role과 mining/passability semantics
- Neighbor/autotile rule 또는 fixed tile 여부
- Edge/corner/inner-corner/transition variant 수와 atlas layout
- Tile origin과 pivot
- Seam, repetition, readability와 biome palette rule
- Collider or Tilemap Collider ownership
- Destruction/durability overlay가 base tile과 분리되는지 여부

Visual tile variant가 `TerrainCellType` 같은 domain semantics를 임의로 추가하거나
바꾸지 않는다.

### Unity import baseline

Pixel-art asset은 spec에서 다르게 승인하지 않는 한 다음을 검토한다.

- Texture Type: `Sprite (2D and UI)`
- Sprite Mode: spec에 따른 Single/Multiple
- PPU: asset family spec의 값
- Mesh Type: predictable bounds가 필요하면 `Full Rect`
- Filter Mode: `Point`
- Compression: prototype pixel sprite는 `None`
- Generate Mip Maps: `Off`
- Alpha Is Transparency: transparent source에 적절히 `On`
- Read/Write: proven runtime requirement가 없으면 `Off`

Serialized importer field를 추정해 YAML로 직접 제안하지 않고 실제 Unity 6.3 Inspector
경로와 값을 제공한다.

## Animation

현재 work unit에 필요한 state만 요청하고 모든 future state를 미리 만들지 않는다.
Animated asset은 필요한 경우 idle, move, attack anticipation/contact/recovery, hit, death와
special을 구분하고 frame count, FPS, loop, exact event/contact frame을 기록한다.

Animation event가 authoritative damage/mining result를 단독 결정하지 않는다. Presentation
event는 승인된 runtime result와 timing을 표현해야 하며 playback failure가 gameplay
결과를 중복하거나 제거하지 않아야 한다.

## UI asset workflow

UI spec에는 다음을 추가한다.

- Native size와 reference-screen display size
- Anchor/stretch와 safe content rectangle
- 9-slice border
- Normal/hover/pressed/selected/disabled/focus/warning state
- Icon padding과 minimum interactive size
- Korean/English text expansion allowance
- Target prefab/hierarchy와 atlas

일반 UI texture에 language text를 굽지 않는다. Retained player-facing UI는 real Unity UI
object와 prefab/scene hierarchy로 authoring하고 code는 reference와 behavior만 binding한다.

## Audio workflow

### Source baseline

Generated SFX master는 별도 spec이 없으면 다음을 사용한다.

- WAV PCM, 48 kHz, 24-bit
- Positionable world sound는 mono 우선
- Intentional UI/music/ambience는 spec에 따라 stereo 허용
- Unintended leading silence, clipping, background noise와 long baked reverb 없음

Music delivery format과 Unity compression은 duration, looping과 platform memory를 검토한
후 정한다. 모든 clip에 같은 load/compression setting을 적용하지 않는다.

### SFX contract

- Trigger와 gameplay meaning
- Physical/emotional intent
- Duration, transient, body, tail와 silence tolerance
- Loop point, simultaneous overlap와 variation count
- Runtime pitch/volume variation
- Mixer group과 playback owner
- Unity load/compression recommendation
- Generation/negative prompt와 acceptance criteria

채굴, 공격, 피격, projectile, enemy cue와 UI confirm 같은 반복 sound는 variation 또는
controlled variation으로 피로도를 줄인다. 시야 밖 위험, 지형, named enemy 접근과 extraction
단계는 GDD의 정보 전달 목적을 보존한다.

AudioMixer가 필요한 milestone에서는 Master, Music, SFX, UI, Ambience 책임을 기본 후보로
검토한다. 필요 전에는 빈 mixer 구조를 미리 만들지 않는다.

## Generation, import and review flow

1. Feature proposal에서 필요한 asset과 placeholder 허용 여부를 식별한다.
2. Asset 또는 coherent batch마다 spec을 승인한다.
3. User/approved agent가 source를 생성·구매·제작한다.
4. Raw file의 dimensions/format/alpha 또는 duration/channel/peak를 확인한다.
5. Unity에 recorded setting으로 import한다.
6. 필요한 slice, animation, material, atlas/mixer와 prefab을 구성한다.
7. Scene 또는 retained UI에 연결한다.
8. Target scale/resolution과 실제 trigger/overlap에서 확인한다.
9. Status, source, prompt, license, known defect와 replacement plan을 기록한다.
10. 품질 향상 시 가능한 한 동일 path/GUID에서 교체한다.

## Provenance and licensing

External 또는 AI-generated source마다 다음을 `docs/licenses/ASSET_REGISTER.md`에 기록한다.

- Exact asset/output file
- Creator, store 또는 tool/model
- Source URL/order reference 또는 prompt record
- Creation/acquisition date와 version
- License/service terms와 commercial-use 확인
- Attribution requirement
- Modification/conversion
- Source and imported storage path
- Current status, known defect와 replacement condition

다른 게임의 sprite, UI, sound, logo, effect 또는 exact screen composition을 복사하지
않는다. 허가가 불명확하면 import와 integration을 중단한다.

## Acceptance checklist

- 역할과 silhouette/sonic cue가 실제 game context에서 읽히는가?
- Exact dimensions/duration, format, alpha/channel이 spec과 일치하는가?
- Pivot, tile seam, animation cell 또는 audio loop가 안정적인가?
- Filtering/compression으로 blur, bleeding 또는 artifact가 생기지 않는가?
- UI가 target resolution과 text expansion에서 유지되는가?
- 반복 sound가 clipping이나 과도한 피로를 만들지 않는가?
- Correct prefab/scene, atlas/mixer와 trigger에 연결되었는가?
- Source, prompt/license와 commercial-use가 기록되었는가?
- 더 높은 품질의 교체가 path/GUID를 보존할 수 있는가?

## Related documents

- [Asset specification template](asset-spec-template.md)
- [World, Art and Audio GDD](../gdd/08_world_art_and_audio.md)
- [Asset register](../licenses/ASSET_REGISTER.md)
- [Current milestone](../roadmap/next-milestone.md)
