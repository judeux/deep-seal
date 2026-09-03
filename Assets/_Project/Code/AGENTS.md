# Source and test code scope

이 파일은 `Assets/_Project/Code/` 아래 source와 test code에 적용된다. Root의
proposal-only 권한을 유지한다.

## Proposal format

- 새 작은 파일은 repository-relative exact path와 전체 내용을 제공한다.
- 기존 파일은 충분한 context의 unified diff 또는 exact bounded replacement를 제공한다.
- 적용 순서, namespace/import, reference, serialized field, test와 Editor wiring을
  생략하지 않는다.
- 제안 전 target module/package 구조를 검사하고 responsibility owner에 맞는 기존
  package를 우선한다.
- 새 package가 필요하면 기존 package가 부적합한 이유와 함께 성장할 책임 경계를
  설명한다. 한 개의 작은 class를 피하기 위한 package는 만들지 않는다.

새 class 제안에는 다음을 포함한다.

1. Target assembly/module
2. Target folder와 namespace
3. Class name
4. 이 위치가 책임에 맞는 이유
5. 새 package 필요 여부와 근거
6. Class 목적을 요약한 Korean XML documentation comment

## Coding rules

- Runtime root namespace는 `DeepSeal`이다.
- Identifier, runtime log와 commit message는 English를 사용한다.
- Comment와 XML documentation은 Korean을 기본으로 하며 문법 반복보다 목적,
  invariant, ownership, unit, coordinate, timing과 randomness를 설명한다.
- 명시적 access modifier와 불변 field의 `readonly`를 우선한다.
- Serialized private field는 `[SerializeField] private Type fieldName;` 형식을 사용하고
  public field를 serialization 편의로 노출하지 않는다.
- Hidden side effect가 있는 property getter와 불필요한 inheritance를 피하고 composition을
  우선한다.
- `Update`, `FixedUpdate`와 gameplay hot loop에서 반복 allocation과 LINQ를 피한다.
- Broad singleton, service locator와 global event bus는 승인 없이 도입하지 않는다.

## Compatibility and data

- Public API, assembly reference, serialized field/schema, stable ID, Input Action contract와
  ScriptableObject data schema 변경은 영향을 먼저 설명한다.
- Field rename/removal은 existing serialized data의 migration 또는 re-authoring을 다룬다.
- Tunable data framework, localization, Addressables, save system 또는 content database를
  현재 milestone보다 앞서 구축하지 않는다.
- `.meta`는 file 이동과 함께 보존하고 GUID를 임의로 다시 만들지 않는다.

## Verification

- 가장 작은 의미 있는 compile/syntax 검증부터 시작한다.
- Pure rule은 EditMode test를 우선하고 Unity lifecycle/scene/physics/timing은 PlayMode를
  사용한다.
- 사용자가 적용하기 전 proposal을 compile/test passed로 표시하지 않는다.
- 적용 후 실제 diff와 relevant test를 검토한 다음에만 verified로 판정한다.
