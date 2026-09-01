$ErrorActionPreference = "Stop"

# pre-commit 훅과 같은 유형의 추적 파일에서 줄끝 공백을 제거한다.
# Unity .meta 파일은 표준 포맷에 trailing space가 있으므로 대상에서 제외한다.
$previousEncoding = [Console]::OutputEncoding

try {
    [Console]::OutputEncoding = New-Object System.Text.UTF8Encoding($false)
    $trackedFiles = @(
        git -c core.quotepath=off ls-files -- `
            "*.cs" "*.asmdef" "*.json" "*.md" "*.ps1" "*.sh" "*.txt" "*.yml" "*.yaml" `
            ".gitattributes" ".gitignore"
    )
}
finally {
    [Console]::OutputEncoding = $previousEncoding
}

$utf8NoBom = New-Object System.Text.UTF8Encoding($false)
$utf8Bom = New-Object System.Text.UTF8Encoding($true)
$cleanedCount = 0

foreach ($file in $trackedFiles) {
    if (-not (Test-Path -LiteralPath $file)) {
        continue
    }

    $bytes = [System.IO.File]::ReadAllBytes($file)

    if ($bytes.Length -eq 0) {
        continue
    }

    $hasBom = $bytes.Length -ge 3 -and $bytes[0] -eq 239 -and $bytes[1] -eq 187 -and $bytes[2] -eq 191
    $text = [System.Text.Encoding]::UTF8.GetString($bytes).TrimStart([char]0xFEFF)
    $clean = [regex]::Replace($text, "[ \t]+(?=\r?\n)", "")

    if ($clean -ne $text) {
        $encoding = if ($hasBom) { $utf8Bom } else { $utf8NoBom }
        [System.IO.File]::WriteAllText($file, $clean, $encoding)
        Write-Host "Cleaned: $file"
        $cleanedCount++
    }
}

Write-Host "Trailing whitespace removed from $cleanedCount file(s)."
