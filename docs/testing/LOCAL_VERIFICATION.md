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
