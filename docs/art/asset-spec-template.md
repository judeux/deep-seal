# Asset Specification: <Asset ID / Batch Name>

적용되지 않는 section은 삭제할 수 있지만, 삭제 전에 현재 asset에 정말 불필요한지
확인한다. 하나의 batch에는 같은 technical contract와 acceptance gate를 공유하는
asset만 포함한다.

## Common metadata

| Field | Value |
| --- | --- |
| Asset ID | `TBD` |
| Status | `Spec` |
| Feature/work unit | `TBD` |
| Gameplay purpose | `TBD` |
| Exact filename | `TBD` |
| ArtSource path | `ArtSource/...` |
| Exact Unity path | `Assets/_Project/...` |
| Owner | `TBD` |
| Source creator/tool/model | `TBD` |
| Creation/acquisition date | `TBD` |
| License/commercial-use status | `TBD` |
| Attribution required | `TBD` |
| Replacement/GUID policy | Preserve compatible Unity path and `.meta` GUID |

## Image or sprite specification

### Visual brief

- Subject:
- Gameplay role and screen context:
- Camera/view/perspective:
- Required silhouette:
- Art style and quality target:
- Mood/material cues:
- Palette and contrast:
- Global light direction:
- Facing/orientation:
- Required variants/states:
- Details required at game scale:
- Details to omit:

### Canvas and pixel contract

| Field | Value |
| --- | --- |
| File format | `PNG/TBD` |
| Canvas size | `TBD px x TBD px` |
| Per-frame cell | `N/A or TBD px x TBD px` |
| Sheet columns x rows | `N/A or TBD` |
| Frame order | `N/A or TBD` |
| PPU | `TBD; no global default is approved` |
| Expected world/display size | `TBD` |
| Background | Transparent/opaque/chroma: `TBD` |
| Anti-aliasing | Off for pixel art |
| Safe margin | `TBD px` |
| Pivot | `TBD` |
| Contact/ground point | `TBD` |
| Mirroring allowed | `TBD` |

### Animation

| State | Frames | FPS | Loop | Contact/event frame | Current/deferred |
| --- | ---: | ---: | --- | --- | --- |
| Idle | `TBD` | `TBD` | Yes/No | N/A | `TBD` |
| Move | `TBD` | `TBD` | Yes/No | N/A | `TBD` |
| Attack | `TBD` | `TBD` | No | `TBD` | `TBD` |
| Hit | `TBD` | `TBD` | No | N/A | `TBD` |
| Death | `TBD` | `TBD` | No | `TBD` | `TBD` |
| Special | `TBD` | `TBD` | `TBD` | `TBD` | `TBD` |

Exact frame naming/order:

```text
TBD
```

### Unity import

| Setting | Value |
| --- | --- |
| Texture Type | Sprite (2D and UI) |
| Sprite Mode | Single/Multiple: `TBD` |
| Pixels Per Unit | `TBD` |
| Mesh Type | Full Rect/Tight: `TBD` |
| Filter Mode | Point/Bilinear: `TBD` |
| Compression | `TBD` |
| Generate Mip Maps | `TBD` |
| Alpha Is Transparency | `TBD` |
| Read/Write | Off unless justified |
| Sprite Atlas | `N/A or TBD` |
| Sorting layer/order | `N/A or TBD` |

### Generation prompt

```text
Create one image asset for the 2D URP game "Deep Seal".

Purpose and context: TBD
Subject: TBD. Generate this subject only; do not add other characters or assets.
Canvas and layout: TBD
Style and quality target: TBD
View and orientation: TBD
Palette, contrast, material and lighting: TBD
Required states/frames: TBD
Pixel constraints when applicable: exact grid intent, hard pixel edges, no
anti-aliasing, no subpixel texture and no blur.
Background: genuinely transparent. If reliable alpha cannot be produced, use one
perfectly uniform chroma color that does not occur in the subject: TBD.
Safe margins and pivot/contact intent: TBD
Output: one source image or one sheet for this single subject in the specified layout.
```

### Negative prompt

```text
No copied game asset, logo, watermark, signature, unintended text, extra character
or extra asset, transparency checkerboard, floor, ground shadow, frame, decorative
element, UI card, anti-aliasing, blurred pixels, inconsistent frame dimensions,
cropped content or perspective change between animation frames.
```

### Image acceptance

- [ ] Exact dimensions and format
- [ ] Correct alpha/opaque/chroma background
- [ ] Crisp intended pixel grid and no unintended smoothing
- [ ] Silhouette readable at actual game scale
- [ ] Correct orientation, pivot and contact point
- [ ] Animation cells and order match the contract
- [ ] No crop, bleed, watermark, copied logo or unintended text
- [ ] Unity importer and slice verified
- [ ] Prefab/animation/atlas connection verified
- [ ] Source, prompt and license recorded

## Tile or tileset extension

| Field | Value |
| --- | --- |
| Logical tile size | `TBD px` |
| World cell size | `TBD` |
| Terrain/domain role | `TBD` |
| Passability/mining semantics | Existing domain type: `TBD` |
| Tile origin/pivot | `TBD` |
| Fixed/neighbor/autotile | `TBD` |
| Required edge/corner/transition variants | `TBD` |
| Atlas layout | `TBD` |
| Collider owner | `TBD` |
| Seam/repetition acceptance | `TBD` |
| Durability/destruction overlay | Separate/combined: `TBD` |

## UI image specification

| Field | Value |
| --- | --- |
| Native size | `TBD` |
| Reference display size | `TBD` |
| Anchor/stretch | `TBD` |
| Safe content rectangle | `TBD` |
| 9-slice L/T/R/B | `N/A or TBD` |
| Icon size and padding | `TBD` |
| Minimum interactive size | `TBD` |
| Korean expansion allowance | `TBD` |
| English expansion allowance | `TBD` |
| Target prefab/hierarchy | `TBD` |

Required states:

- [ ] Normal
- [ ] Hover
- [ ] Pressed
- [ ] Selected
- [ ] Disabled
- [ ] Focused
- [ ] Warning/error

Text is separate from the image unless an approved localized-image requirement exists.

## SFX specification

### Sound brief

| Field | Value |
| --- | --- |
| Trigger | `TBD` |
| Gameplay meaning | `TBD` |
| Emotional/physical intent | `TBD` |
| Duration | `TBD ms/s` |
| Transient/body/tail | `TBD` |
| Leading silence maximum | `TBD ms` |
| Loop and loop points | No/Yes: `TBD` |
| Variations | `TBD` |
| Expected simultaneous overlap | `TBD` |
| Runtime pitch variation | `TBD` |
| Runtime volume variation | `TBD` |
| AudioMixer group | Master/Music/SFX/UI/Ambience/TBD |

### Source and import

| Setting | Value |
| --- | --- |
| Source format | WAV PCM unless approved otherwise |
| Sample rate | 48 kHz default |
| Bit depth | 24-bit default |
| Channels | Mono/Stereo: `TBD` |
| Normalize | `TBD` |
| Unity Load Type | `TBD after usage review` |
| Compression Format/Quality | `TBD after usage review` |
| Preload Audio Data | `TBD` |

### Audio generation prompt

```text
Create one clean game sound effect for the 2D expedition survival roguelite
"Deep Seal".

Trigger and gameplay purpose: TBD
Physical/emotional character: TBD
Duration: TBD
Transient, body and tail: TBD
Channels and spatial intent: TBD
Variations: TBD
Technical output: 48 kHz, 24-bit PCM WAV unless specified otherwise, no clipping,
no unintended leading silence and no long baked-in reverb unless requested.
```

### Audio negative prompt

```text
No copyrighted melody or recognizable sampled game sound, voice or speech unless
requested, watermark, clipping, limiter pumping, noisy background, unintended long
silence or reverb tail longer than specified.
```

### SFX acceptance

- [ ] Format, sample rate, bit depth and channel count match
- [ ] Duration and leading silence are within contract
- [ ] No clipping, unwanted noise or excessive limiting
- [ ] Trigger reads at gameplay volume
- [ ] Tail and overlap remain clear under repeated playback
- [ ] Variations are distinct and coherent
- [ ] No recognizable copyrighted source
- [ ] Import, mixer group and playback owner verified
- [ ] Source, prompt and license recorded

## Integration record

| Check | Result |
| --- | --- |
| Imported with Unity | `6000.3.17f1 / TBD` |
| Import settings verified by/date | `TBD` |
| Prefab/scene/UI reference | `TBD` |
| Atlas/mixer assignment | `N/A or TBD` |
| Target scale/resolution or playback test | `TBD` |
| Known defects | `TBD` |
| Replacement plan | `TBD` |
| Asset register entry | `TBD` |
