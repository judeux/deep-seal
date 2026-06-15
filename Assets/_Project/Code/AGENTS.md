# AGENTS.md — Code Rules

This file applies to source code under `Assets/_Project/Code/`.


## Manual Code Application Rule

The default mode for code work is proposal-only.

Codex should normally provide code as:

- Full file contents for new files.
- Exact replacement snippets for small edits.
- Unified diffs only when they are easier for the owner to review.
- Clear file paths relative to the repository root.

The project owner applies the code manually. After application, Codex reviews the actual changed code, errors, test output, or Git diff before moving to the next task.

Do not directly edit files under `Assets/_Project/Code/` unless the project owner explicitly asks Codex to apply the edits itself for that task.

## Namespace

Use `DeepSeal` as the root namespace for runtime code.

Examples:

- `DeepSeal.Core`
- `DeepSeal.Mining`
- `DeepSeal.Combat`
- `DeepSeal.Expedition`
- `DeepSeal.ProceduralGeneration`
- `DeepSeal.Campaign`
- `DeepSeal.UI`

Editor-only code should use:

- `DeepSeal.EditorBuild`
- `DeepSeal.EditorTools`

## Folder Intent

Recommended folder structure when the relevant systems are created:

```text
Assets/_Project/Code/
├─ Runtime/
│  ├─ Bootstrap/
│  ├─ Campaign/
│  ├─ Combat/
│  ├─ Core/
│  ├─ Encounters/
│  ├─ Expedition/
│  ├─ Mining/
│  ├─ ProceduralGeneration/
│  └─ UI/
├─ Editor/
└─ Tests/
   ├─ EditMode/
   └─ PlayMode/
```

Do not create all folders preemptively. Create folders when a real file belongs there.

## Design of Runtime Code

Prefer pure C# domain classes for rules and calculations.

Use MonoBehaviours as Unity-facing adapters for:

- Scene references.
- Unity lifecycle methods.
- Rendering.
- Input hookup.
- Prefab and component wiring.

This split makes code easier to test without relying on scenes.

## Unity Lifecycle Rules

Keep `Update`, `FixedUpdate`, and per-frame loops small.

Move non-trivial logic into named methods or pure classes.

Avoid hidden side effects in property getters.

Do not allocate repeatedly in gameplay hot paths unless the code is clearly prototype-only and marked as such.

## Data Rules

Use simple serializable data structures first.

Do not introduce a complex data pipeline, Addressables setup, localization system, or database layer until the prototype needs it.

ScriptableObjects are acceptable for definitions once the first systems are stable, but avoid building a large content framework before gameplay is proven.

## Testing Rules

Prefer EditMode tests for pure gameplay logic.

Use PlayMode tests only when Unity scene behavior, coroutines, GameObjects, physics, or timing must be verified.

New pure systems should include at least basic EditMode tests when practical.

## Assembly Definition Rules

Do not add many assembly definitions early.

If assembly definitions are needed, start with only:

- `DeepSeal.Runtime.asmdef`
- `DeepSeal.Editor.asmdef`
- `DeepSeal.Tests.EditMode.asmdef`
- `DeepSeal.Tests.PlayMode.asmdef`

Do not split by every system until dependencies are clearer.

## Style Rules

Use clear, descriptive names.

Avoid abbreviations unless they are standard in Unity or game development.

Prefer explicit access modifiers.

Prefer `readonly` for fields that should not change after construction.

For serialized private fields, use:

```csharp
[SerializeField] private Type fieldName;
```

Do not expose public fields just to make Unity serialization easy.

## Explanation Requirement

When adding or changing code, summarize the purpose of important classes, methods, fields, and variables in the final response.

Explain why the chosen structure is appropriate for this Unity project and how it fits the current project hierarchy.
