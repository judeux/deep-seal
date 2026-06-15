# 0003. Manual Application Workflow

Date: 2026-06-15

## Decision

The default development workflow is proposal-only. Codex proposes code, file contents, diffs, commands, and review steps, but the project owner manually applies code and project changes.

Codex reviews the manually applied result before the project moves to the next task.

Direct file edits by Codex are allowed only when the project owner explicitly asks Codex to apply edits for that specific task.

## Rationale

This project is a solo Unity project where the owner wants to understand and control each implementation step. Unity assets, `.meta` files, scenes, prefabs, and project settings can be fragile when edited automatically. A proposal-only workflow reduces accidental structural changes and keeps the owner familiar with the codebase.

## Expected Workflow

1. Define the goal and scope of a step.
2. Codex reads relevant documents and current files.
3. Codex proposes code or exact edit instructions.
4. The project owner applies the changes manually.
5. The project owner runs Unity or verification scripts.
6. Codex reviews the resulting code, logs, test output, or Git diff.
7. The next task is selected only after the review is complete.

## Consequences

Codex should prioritize clarity, complete snippets, file paths, and explanations over direct automation.

The project `.codex/config.toml` uses a read-only default sandbox to reinforce this workflow.

If a task requires direct edits or command execution that writes files, the owner must explicitly approve that exception.

## Status

Accepted.
