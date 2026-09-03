# Imported art scope

이 파일은 first-party image, sprite, tile, UI texture, VFX texture, material, atlas와
animation art에 적용된다. Root와 `Assets/_Project/AGENTS.md`의 proposal-only 경계를
유지한다.

- Asset spec 없이 global PPU, character canvas, tile size 또는 palette를 추정하지 않는다.
- Current prototype asset과 production-intended asset을 명시적으로 구분한다.
- Pixel-art spec은 exact dimensions, PPU, Point filter, mipmap, compression,
  transparency, mesh type, pivot와 native-scale readability를 다룬다.
- Repeated sprites는 실제 coherent group이 생긴 뒤 feature/scene 단위 atlas를 사용한다.
  빈 atlas나 하나의 global atlas를 미리 만들지 않는다.
- Source 교체 시 가능한 경우 path와 `.meta` GUID를 유지하고 importer, pivot, slice,
  atlas membership, prefab/scene reference를 다시 검증한다.
- AI output은 reference 또는 retained placeholder가 될 수 있지만 production pixel art는
  native grid에서 의도적으로 정리해야 한다.
- Player-facing UI는 retained scene/prefab hierarchy와 실제 sprite를 사용한다.
  초기의 명시적 debug/logic spike 밖에서 code-generated UI를 완성 UI로 유지하지 않는다.
- 시각 검증은 실제 game scale, target resolution, silhouette, contrast와 색상 외 구분
  수단을 포함한다.
