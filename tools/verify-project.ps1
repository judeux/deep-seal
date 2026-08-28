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

$logFile = "$ProjectPath\Logs\verify-project.log"

Write-Host "Unity: $UnityPath"
Write-Host "Project: $ProjectPath"
Write-Host "Log: $logFile"

$unityArguments = @(
    "-batchmode"
    "-quit"
    "-projectPath"
    "`"$ProjectPath`""
    "-logFile"
    "`"$logFile`""
)

$unityProcess = Start-Process -FilePath $UnityPath -ArgumentList $unityArguments -Wait -PassThru

$exitCode = $unityProcess.ExitCode

if ($null -eq $exitCode) {
    Write-Error "Unity process did not report an exit code. See $logFile"
}

if ($exitCode -ne 0) {
    Write-Error "Unity verification failed with exit code $exitCode. See $logFile"
}

Write-Host "Unity project verification completed successfully."
