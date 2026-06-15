param(
    [string]$ProjectPath = (Resolve-Path "$PSScriptRoot\..").Path,
    [string]$UnityPath = "",
    [string]$BuildPath = ""
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($UnityPath)) {
    $UnityPath = & "$PSScriptRoot\find-unity-editor.ps1"
}

if ([string]::IsNullOrWhiteSpace($BuildPath)) {
    $BuildPath = Join-Path $ProjectPath "Builds\Windows\DeepSeal.exe"
}

if (-not (Test-Path $UnityPath)) {
    Write-Error "Unity executable not found: $UnityPath"
}

New-Item -ItemType Directory -Force -Path "$ProjectPath\Logs" | Out-Null
New-Item -ItemType Directory -Force -Path (Split-Path $BuildPath -Parent) | Out-Null

$logFile = "$ProjectPath\Logs\build-windows.log"

Write-Host "Building Windows player..."
Write-Host "BuildPath: $BuildPath"

& $UnityPath `
    -batchmode `
    -quit `
    -projectPath $ProjectPath `
    -executeMethod DeepSeal.EditorBuild.BuildPlayer.PerformWindowsDevelopmentBuild `
    -deepSealBuildPath $BuildPath `
    -logFile $logFile

$exitCode = $LASTEXITCODE

if ($exitCode -ne 0) {
    Write-Error "Windows build failed with exit code $exitCode. See $logFile"
}

Write-Host "Windows build completed successfully: $BuildPath"
