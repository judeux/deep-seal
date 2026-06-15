param(
    [string]$UnityVersionPattern = "6000.3*"
)

$ErrorActionPreference = "Stop"

$candidateRoots = @(
    "$env:ProgramFiles\Unity\Hub\Editor",
    "${env:ProgramFiles(x86)}\Unity\Hub\Editor"
) | Where-Object { $_ -and (Test-Path $_) }

$editors = @()

foreach ($root in $candidateRoots) {
    $editors += Get-ChildItem -Path $root -Directory -ErrorAction SilentlyContinue |
        Where-Object { $_.Name -like $UnityVersionPattern } |
        ForEach-Object {
            $exe = Join-Path $_.FullName "Editor\Unity.exe"
            if (Test-Path $exe) {
                [PSCustomObject]@{
                    Version = $_.Name
                    Path = $exe
                }
            }
        }
}

$selected = $editors |
    Sort-Object Version -Descending |
    Select-Object -First 1

if (-not $selected) {
    Write-Error "Could not find Unity Editor matching pattern '$UnityVersionPattern'. Pass -UnityPath explicitly to the tool script."
}

$selected.Path
