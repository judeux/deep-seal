# Unity adapter scope

이 파일은 `DeepSeal.UnityAdapters.*`의 MonoBehaviour, ScriptableObject mapping,
rendering, input과 scene integration에 적용된다.

- Adapter는 Unity lifecycle, serialization, rendering, input과 scene/prefab binding을
  소유하고 reusable gameplay rule을 다시 계산하지 않는다.
- Current authoritative state와 rule은 domain 결과를 사용하고 Tilemap, Transform,
  visibility 또는 component enabled state로 역추론하지 않는다.
- `Update`와 coroutine은 작게 유지하고 non-trivial calculation을 named method 또는
  pure rule로 이동한다.
- Scene reference는 Inspector의 explicit serialized reference를 우선하고 broad
  `Find*`, tag/name lookup과 singleton registry를 피한다.
- Input System 전환이 승인되기 전의 direct keyboard input은 prototype limitation으로
  유지하며 새 gameplay contract로 확대하지 않는다.
- Retained player-facing UI는 scene/prefab으로 authoring하고 adapter는 binding한다.
  Debug/OnGUI prototype은 명시적으로 격리하고 production UI로 표시하지 않는다.
- Inspector field는 purpose, unit, valid range/default와 required-reference 여부를
  tooltip 또는 proposal 설명으로 제공한다.
- Adapter의 provisional tuning value와 production balance data를 구분한다.
