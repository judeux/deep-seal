# Runtime code scope

이 파일은 `DeepSeal.Runtime` assembly의 runtime code에 적용된다.

- `Core`, `Mining`, `ProceduralGeneration`, `Combat`, `Expedition`, `Upgrades`의 reusable
  rule과 state는 가능한 한 scene 없이 test 가능한 pure C#으로 유지한다.
- Pure domain namespace는 `UnityEngine`, `UnityEditor`, `MonoBehaviour`,
  `ScriptableObject`, `GameObject`, `Transform`, `Time` 또는 `UnityEngine.Random`에
  의존하지 않는다.
- Random-dependent rule은 seed 또는 random source ownership과 consumption order를
  명시하고 deterministic test가 가능해야 한다.
- Authoritative collection을 노출할 때 caller가 내부 상태를 우회해 변경할 수 없도록
  immutable/read-only contract를 사용한다.
- Domain state는 sprite, prefab, display text, scene object 또는 asset filename을 key로
  사용하지 않는다.
- 실패·rejection이 partial mutation을 남기지 않도록 validation과 commit 경계를
  명시한다.
- Unity integration은 `DeepSeal.UnityAdapters.*`에 두고 domain으로 역의존시키지 않는다.
