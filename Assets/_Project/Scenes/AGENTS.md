# Scene scope

이 파일은 Unity scene authoring, hierarchy와 scene-level composition에 적용된다.

- Reusable content는 scene에 중복 authoring하지 않고 prefab reference를 사용한다.
- Tag/name lookup보다 serialized reference와 explicit composition을 우선한다.
- Scene proposal은 exact hierarchy, GameObject/component, serialized reference,
  layer/tag, camera/canvas, load와 lifetime을 명시한다.
- Domain rule을 scene object active state, visibility 또는 hierarchy로 추론하지 않는다.
- Additive loading, scene framework와 persistent global object는 승인된 requirement가
  있을 때만 도입한다.
- Scene 변경은 Unity compile/Console, 관련 PlayMode 또는 최소 manual smoke와 target
  resolution을 risk에 맞게 검증한다.
- Scene YAML은 사용자 주도 Unity Editor 작업이 기본이며 agent가 Unity를 열어
  unrelated object를 reserialize하지 않는다.
