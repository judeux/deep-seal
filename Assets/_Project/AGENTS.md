# Unity project content scope

이 파일은 `Assets/_Project/` 아래 first-party code, tests, scenes, prefabs,
ScriptableObject와 imported asset에 적용된다. Root의 proposal-only 권한을 유지한다.

## User-led Unity work

사용자가 해당 work unit의 직접 수정을 명시적으로 승인하지 않으면 다음은 Unity
Editor에서 사용자가 적용한다.

- GameObject hierarchy, component, Transform와 Inspector reference
- Scene, prefab, ScriptableObject instance, material, animation과 Input Action
- Imported image/audio setting, sprite slicing, atlas와 mixer connection
- Unity가 serialize하는 project content의 생성, 이동, rename과 삭제

Agent는 exact hierarchy, object/component/property, value, asset path, reference와 검증
순서를 제공한다. Unity를 자동으로 열어 broad reserialization하지 않는다.

## Serialized and binary inspection

1. `git status`, `git diff --name-status`, exact path와 size로 대상을 한정한다.
2. `.meta`는 생성·삭제·이동, GUID, importer 또는 reference 문제가 있을 때만 관련
   field를 읽는다.
3. Scene, prefab, ScriptableObject, material, controller와 atlas YAML은 `rg`와 filtered
   diff로 target object/property/GUID를 먼저 확인한다.
4. Binary는 dimensions, format, duration/channel, hash와 import metadata부터 확인하고
   실제 시각·청각 판단이 필요할 때만 preview한다.
5. Generated payload 전문은 issue가 직접 가리킬 때만 읽는다.

Scene/prefab/controller/imported asset YAML은 serialized 형식과 exact target을 확인하지
않고 수작업하지 않는다. Asset 이동·교체 시 `.meta` GUID와 reference를 보존한다.

## Shared authoring rules

- Runtime/player-facing reusable content는 scene 복제보다 prefab을 우선한다.
- Visual-only asset에 authoritative gameplay state를 넣지 않는다.
- Inspector field proposal은 purpose, unit, range/default와 required reference를 설명한다.
- Placeholder와 production-intended asset을 구분하고 replacement condition을 기록한다.
- Asset source, prompt와 license는 `docs/licenses/ASSET_REGISTER.md`에 연결한다.
