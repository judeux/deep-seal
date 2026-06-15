param(
    [string]$ProjectPath = (Resolve-Path "$PSScriptRoot\..").Path,
    [string]$UnityPath = ""
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($UnityPath)) {
    $UnityPath = & "$PSScriptRoot\find-unity-editor.ps1"
}

if (-not (Test-Path $UnityPath)) {
    Write-Error "Unity executable not found: $UnityPath"
}

New-Item -ItemType Directory -Force -Path "$ProjectPath\Logs" | Out-Null
New-Item -ItemType Directory -Force -Path "$ProjectPath\TestResults" | Out-Null

$logFile = "$ProjectPath\Logs\test-editmode.log"
$resultsFile = "$ProjectPath\TestResults\editmode-results.xml"

Write-Host "Running Unity EditMode tests..."

& $UnityPath `
    -batchmode `
    -quit `
    -projectPath $ProjectPath `
    -runTests `
    -testPlatform EditMode `
    -testResults $resultsFile `
    -logFile $logFile

$exitCode = $LASTEXITCODE

if ($exitCode -ne 0) {
    Write-Error "EditMode tests failed with exit code $exitCode. See $logFile and $resultsFile"
}

Write-Host "EditMode tests completed successfully. Results: $resultsFile"
