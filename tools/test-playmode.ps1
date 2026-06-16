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

$logFile = "$ProjectPath\Logs\test-playmode.log"
$resultsFile = "$ProjectPath\TestResults\playmode-results.xml"

Write-Host "Running Unity PlayMode tests..."

& $UnityPath `
    -batchmode `
    -projectPath $ProjectPath `
    -runTests `
    -testPlatform PlayMode `
    -testResults $resultsFile `
    -logFile $logFile

$exitCode = $LASTEXITCODE

if ($exitCode -ne 0) {
    Write-Error "PlayMode tests failed with exit code $exitCode. See $logFile and $resultsFile"
}

if (-not (Test-Path $resultsFile)) {
    Write-Error "PlayMode test results file was not created. See $logFile"
}

Write-Host "PlayMode tests completed successfully. Results: $resultsFile"
