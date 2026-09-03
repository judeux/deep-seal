# Local Verification

Use these scripts from the repository root.

## Verification levels

Choose the smallest level that proves the changed behavior, then add higher levels when the
risk requires them.

1. Static inspection: targeted diff, path/reference and `git diff --check`
2. Unity script compilation: batch open or Editor Console error 0
3. EditMode tests: pure rule, deterministic algorithm and data validation
4. PlayMode tests: GameObject, scene/prefab, physics, coroutine, timing and Unity lifecycle
5. Manual scene smoke: player-visible flow and presentation
6. Windows development build: milestone/build integration gate

Launching or playing the application does not by itself prove script compilation or automated
test coverage. Record each level separately.

## Worktree and Unity process rule

- Run a verification command from the worktree that contains the exact changes under review.
- Do not run two Unity processes against the same worktree.
- A separate Codex/ZCode worktree has its own ignored `Library`, `Logs`, `TestResults` and build
  output. Do not copy these generated directories between worktrees as verification evidence.
- If Unity is already open in the target worktree, use the Editor/Test Runner or close it normally
  before batch mode. Do not terminate an unrelated owner process automatically.

## Script summary

| Command | Level | Expected success evidence | Default output |
| --- | --- | --- | --- |
| `.\tools\verify-project.ps1` | Unity compilation/open | Process exit code 0; no compile/package error | `Logs/verify-project.log` |
| `.\tools\test-editmode.ps1` | EditMode | XML result `Passed`, failed 0, command exit code 0 | `TestResults/editmode-results.xml`, `Logs/test-editmode.log` |
| `.\tools\test-playmode.ps1` | PlayMode | XML result `Passed`, failed 0, command exit code 0 | `TestResults/playmode-results.xml`, `Logs/test-playmode.log` |
| `.\tools\build-windows.ps1` | Windows build | Exit code 0 and expected executable exists | `Builds/Windows/DeepSeal.exe` |

These output directories are ignored and local. A result file proves only the commit/worktree and
date on which it was produced; do not carry an old pass forward after relevant changes.

## Evidence record

For each required verification record:

- Date and exact Unity version
- Worktree/branch and reviewed commit or diff
- Command or exact Unity Editor route
- Exit/result status and pass/fail count
- Result/log path
- Manual expected and observed behavior
- Warning, skipped/untested scope and blocker

## Documentation-only verification

For changes limited to repository Markdown and non-runtime instruction metadata, use at least:

```powershell
git diff --check
git status --short
```

Also validate internal Markdown links, renamed paths, status consistency and the exact staged
path list. Unity compilation is not required when no runtime-consumed asset or project setting
changed; state that exclusion in the completion report.

## Verify Unity Project Opens in Batch Mode

```powershell
.\tools\verify-project.ps1
```

This checks that Unity can open the project in batch mode without compilation or package errors.

## Run EditMode Tests

```powershell
.\tools\test-editmode.ps1
```

Use EditMode tests for pure C# rules and calculations.

## Run PlayMode Tests

```powershell
.\tools\test-playmode.ps1
```

Use PlayMode tests for GameObject, scene, coroutine, physics, and Unity lifecycle behavior.

## Build Windows Development Player

```powershell
.\tools\build-windows.ps1
```

The build script calls:

```text
DeepSeal.EditorBuild.BuildPlayer.PerformWindowsDevelopmentBuild
```

The generated build is written to:

```text
Builds/Windows/DeepSeal.exe
```

The `Builds/` folder is intentionally ignored by Git.

## Custom Unity Path

If Unity cannot be found automatically, pass the executable path manually:

```powershell
.\tools\verify-project.ps1 -UnityPath "C:\Program Files\Unity\Hub\Editor\6000.3.xxxx\Editor\Unity.exe"
```

## Pre-commit Trailing Whitespace Hook (Local Only)

This repository relies on a self-healing `pre-commit` hook. Before every commit it
strips trailing whitespace from staged text files (`*.cs`, `*.asmdef`, `*.json`,
`*.md`, `*.ps1`, `*.sh`, `*.txt`, `*.yml`, `*.yaml`, `.gitattributes`,
`.gitignore`; Unity `.meta` files are intentionally excluded because their
trailing spaces are Unity-standard) and re-stages them. If any whitespace error
still remains after the fix, the commit is aborted.

Git hooks live in `.git/hooks/`, which is **not tracked by Git**. After cloning
this repository on a new machine — or whenever commits start failing with
trailing whitespace errors again — recreate `.git/hooks/pre-commit` with the
following content (or copy it from a machine that already has it):

```sh
#!/bin/sh
# Strips trailing whitespace from staged text files and re-stages them.
# Staged files are always re-staged from the working copy, so review
# `git status` first when using partial staging.

patterns="*.cs *.asmdef *.json *.md *.ps1 *.sh *.txt *.yml *.yaml .gitattributes .gitignore"

git -c core.quotepath=off diff --cached --name-only -- $patterns | while IFS= read -r f; do
    [ -f "$f" ] || continue
    tmp="$f.whitespace-fix.tmp"
    if sed -e 's/[ \t]*$//' "$f" > "$tmp" 2>/dev/null; then
        if ! cmp -s "$f" "$tmp"; then
            mv -f "$tmp" "$f"
        else
            rm -f "$tmp"
        fi
        git add -- "$f"
    else
        rm -f "$tmp"
    fi
done

git diff --cached --check -- $patterns
```

The manual cleanup script `tools/strip-trailing-whitespace.ps1` is tracked by
Git and works without the hook, but it only cleans and re-stages files that it
changes; the hook is the safety net at commit time.
