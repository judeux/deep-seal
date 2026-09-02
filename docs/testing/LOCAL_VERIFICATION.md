# Local Verification

Use these scripts from the repository root.

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
