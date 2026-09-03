# Prefab scope

이 파일은 reusable runtime/player-facing prefab의 authoring, reference와 lifetime에
적용된다.

- Runtime-spawned player, enemy, projectile, treasure, reward, effect와 reusable UI는
  prefab을 origin으로 사용한다.
- Scene마다 같은 hierarchy를 복제하지 않고 prefab 또는 필요한 경우 nested prefab으로
  공유한다.
- Proposal은 owner, exact hierarchy, required component/reference, layer/tag/sorting,
  spawn/despawn lifetime와 variant boundary를 명시한다.
- Visual-only prefab에 authoritative gameplay rule이나 broad manager를 넣지 않는다.
- Prefab replacement/rename은 `.meta` GUID, root/component, nested reference와 serialized
  target을 함께 검증한다.
- Prefab YAML은 사용자가 Unity Editor에서 적용하는 것이 기본이며 agent가 임의로
  재작성하지 않는다.
