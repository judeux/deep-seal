# Art and audio specification scope

이 파일은 asset specification, generation prompt, provenance와 import-pipeline 문서에
적용된다. 실제 Unity asset에는 `Assets/_Project/AGENTS.md`와 Art/Audio의 scoped
지침을 함께 적용한다.

## Common asset specification

모든 retained asset 요청은 필요한 범위에서 다음을 명시한다.

- Asset ID, status, exact filename과 target path
- Gameplay purpose, trigger 또는 screen context
- Source format, dimensions/duration와 technical quality
- Orientation, pivot/contact point 또는 UI anchor
- Variants, states, animation/loop requirement
- Unity import setting과 prefab/scene/atlas/mixer connection
- 실행 가능한 generation prompt와 forbidden output
- Acceptance criteria와 replacement condition
- Creator/tool, date, source relationship, commercial-use/license와 attribution

Asset status는 `Spec`, `Placeholder`, `Candidate`, `Approved`, `Deprecated` 중 하나다.
Prototype placeholder를 production-quality final로 표시하지 않는다.

## Image and sprite requests

- Pixel dimensions, per-frame cell, sheet layout, PPU, expected display/world size를
  구분해 기록한다.
- Transparency, safe margin, crop, pivot, facing, palette, lighting, outline,
  anti-aliasing, filter와 compression을 명시한다.
- Deep Seal의 global PPU와 character canvas는 아직 승인된 고정 계약이 아니다.
  기존 prototype tile이 16x16이라는 사실만으로 새 asset 규격을 추론하지 않는다.
- AI prompt는 기본적으로 한 request에 subject 하나만 요구한다.
- Alpha가 불안정한 도구에는 실제 palette와 충돌하지 않는 완전한 단색 chroma
  background를 fallback으로 지정한다.
- Checkerboard, floor, ground shadow, frame, decoration, UI card, text, watermark,
  extra object와 cropped output을 명시적으로 금지한다.
- Tool이 주장한 alpha와 dimensions를 신뢰하지 않고 실제 파일을 검사한다.
- Production-quality pixel art는 high-resolution AI output의 단순 축소만으로 승인하지
  않는다. Native grid에서 silhouette, line weight와 readability를 확인한다.

## UI image requests

Native size, reference display size, anchor/stretch, safe content rectangle, 9-slice
border, interaction states, minimum control size와 text expansion allowance를 명시한다.
일반 UI texture에 Korean/English text를 굽지 않는다.

## Audio requests

- Trigger, gameplay meaning, emotional/physical intent, duration, transient/body/tail,
  leading silence, loop, overlap와 variation을 명시한다.
- 별도 spec이 없으면 generated SFX source 기준은 WAV PCM, 48 kHz, 24-bit다.
- Positionable world SFX는 기본 mono이며 UI, music과 ambience의 stereo는 spec에서
  결정한다.
- 반복 효과음은 variation 또는 통제된 pitch/volume variation을 정의한다.
- AudioMixer group과 Unity load/compression은 usage를 확인한 뒤 결정한다.
- Copyrighted melody/sample, voice, clipping, heavy limiting, unintended silence/noise와
  과도한 baked reverb를 금지한다.

## Provenance and integration

- AI/third-party asset은 `ArtSource/`의 prompt/source와
  `docs/licenses/ASSET_REGISTER.md` 기록을 연결한다.
- 호환 가능한 품질 교체는 Unity path와 `.meta` GUID 보존을 우선한다.
- Import setting, slice, prefab/scene reference, atlas/mixer, target scale/resolution과
  known defect를 확인한 뒤 integrated로 기록한다.
- 상업적 이용 조건이 확인되지 않은 asset은 project에 사용하지 않는다.
