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

if (Test-Path $resultsFile) {
    Remove-Item -LiteralPath $resultsFile -Force
}

Write-Host "Running Unity EditMode tests..."

& $UnityPath `
    -batchmode `
    -projectPath $ProjectPath `
    -runTests `
    -testPlatform EditMode `
    -testResults $resultsFile `
    -logFile $logFile

$exitCode = $LASTEXITCODE

if ($exitCode -ne 0) {
    Write-Error "EditMode tests failed with exit code $exitCode. See $logFile and $resultsFile"
}

$resultsDeadline = (Get-Date).AddSeconds(60)

do {
    if (Test-Path -LiteralPath $resultsFile) {
        break
    }

    Start-Sleep -Milliseconds 500
} while ((Get-Date) -lt $resultsDeadline)

if (-not (Test-Path -LiteralPath $resultsFile)) {
    Write-Error "EditMode test results file was not created within 60 seconds. See $logFile"
}

[xml]$resultsXml = Get-Content -LiteralPath $resultsFile
$testRun = $resultsXml.'test-run'

if ($testRun.result -ne "Passed") {
    Write-Error "EditMode tests did not pass. Result=$($testRun.result), Failed=$($testRun.failed), Total=$($testRun.total). See $resultsFile"
}

Write-Host "EditMode tests completed successfully. Results: $resultsFile"
