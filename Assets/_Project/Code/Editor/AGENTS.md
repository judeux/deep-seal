# Editor tooling scope

이 파일은 `DeepSeal.EditorBuild`, `DeepSeal.EditorTools`와 project-specific Unity Editor
tooling에 적용된다.

- Runtime assembly에서 Editor assembly/type을 reference하지 않는다.
- Tool은 authoring과 validation을 보조하지만 runtime validation이나 EditMode test를
  대체하지 않는다.
- Asset mutation tool은 exact target, Undo/dirty/save behavior, idempotency와 actionable
  failure message를 명시한다.
- Broad automatic reserialization 또는 unrelated asset import를 유발하는 tool을 만들지
  않는다.
- Batch build entry point는 deterministic path와 nonzero failure exit를 제공하고 secret,
  user-specific absolute path 또는 generated cache를 contract에 넣지 않는다.
