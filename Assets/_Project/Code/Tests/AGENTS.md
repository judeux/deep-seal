# Test scope

이 파일은 EditMode와 PlayMode tests, fixtures와 test-only helper에 적용된다.

- Pure rule, deterministic algorithm, boundary와 data validation은 EditMode를 우선한다.
- Unity lifecycle, scene/prefab integration, physics, coroutine, timing과 rendering/input
  binding은 필요한 최소 PlayMode test를 사용한다.
- Empty, min/max, invalid input, ordering, connectivity, random determinism, rejection과
  partial-mutation boundary를 risk에 맞게 검증한다.
- Balance simulation은 deterministic unit test를 대체하지 않는다.
- Core로 검증할 수 있는 behavior에 broad fragile scene test를 추가하지 않는다.
- Test fixture/helper는 test assembly에 두고 production API를 test 편의를 위해
  노출하지 않는다.

Verification level은 변경 risk에 맞게 선택한다.

1. Static inspection과 diff
2. Unity script compilation, Console error 0
3. Relevant EditMode tests
4. Relevant PlayMode tests
5. Manual scene smoke
6. 필요 milestone의 Windows development build

한 review cycle에는 직접 관련된 검증 1~2개를 우선 제안한다. Batch mode가 Editor
lock, license, package restore, path 또는 cache 때문에 실패하면 exact blocker와 Unity
Editor Test Runner 대체 경로를 제공한다.
