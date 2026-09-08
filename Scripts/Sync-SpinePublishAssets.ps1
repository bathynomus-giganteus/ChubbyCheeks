param(
    [string]$ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path,
    [string]$SourceRoot = "E:\work\Cult_leader_mod\SPINE_4_2_TEST",
    [string]$LocalModDir = "",
    [string]$WorkshopSubscriptionDir = "",
    [string]$WorkshopContentDir = "",
    [switch]$Clean
)

$ErrorActionPreference = "Stop"

function Get-PublishHash([string]$PathValue) {
    $stream = [System.IO.File]::OpenRead($PathValue)
    $sha = [System.Security.Cryptography.SHA256]::Create()
    try { return [System.BitConverter]::ToString($sha.ComputeHash($stream)) }
    finally { $sha.Dispose(); $stream.Dispose() }
}

function Resolve-SafePath([string]$PathValue) {
    if ([string]::IsNullOrWhiteSpace($PathValue)) {
        return $null
    }

    $parent = Split-Path -Parent $PathValue
    if ($parent -and (Test-Path -LiteralPath $parent)) {
        return [System.IO.Path]::GetFullPath($PathValue)
    }

    return [System.IO.Path]::GetFullPath($PathValue)
}

function Assert-SafeSpineDestination([string]$PathValue) {
    $full = [System.IO.Path]::GetFullPath($PathValue)
    if (-not $full.EndsWith("spine_assets", [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Refusing to clean/copy unexpected Spine destination: $full"
    }

    if ($full.Length -lt 20) {
        throw "Refusing suspiciously short Spine destination: $full"
    }

    return $full
}

function Convert-CategoryPath([string]$Category) {
    $otherCategory = "$([char]0x5176)$([char]0x4F59)"
    $summonCategory = "$([char]0x53EC)$([char]0x5524)$([char]0x7269)"
    if ($Category.Contains($otherCategory) -and $Category.Contains($summonCategory)) {
        return [System.IO.Path]::Combine($otherCategory, $summonCategory)
    }

    $parts = $Category -split "[\\/]" | Where-Object { -not [string]::IsNullOrWhiteSpace($_) }
    return [System.IO.Path]::Combine($parts)
}

function Add-Asset([System.Collections.Generic.HashSet[string]]$Set, [string]$Category, [string]$ResourceCode) {
    if ([string]::IsNullOrWhiteSpace($Category) -or [string]::IsNullOrWhiteSpace($ResourceCode)) {
        return
    }

    [void]$Set.Add("$Category|$ResourceCode")
}

$projectRootFull = [System.IO.Path]::GetFullPath($ProjectRoot)
$profilePath = Join-Path $projectRootFull "CultLeaderModCode\Vfx\ApostleSpinePrototype.cs"
if (-not (Test-Path -LiteralPath $profilePath)) {
    throw "Cannot find ApostleSpinePrototype.cs at $profilePath"
}

if (-not (Test-Path -LiteralPath $SourceRoot)) {
    throw "Spine source root not found: $SourceRoot"
}

$text = [System.Text.Encoding]::UTF8.GetString([System.IO.File]::ReadAllBytes($profilePath))
$normalApostleCategory = "$([char]0x6B63)$([char]0x5E38)$([char]0x4F7F)$([char]0x5F92)"
$battleModelCategory = "$([char]0x6218)$([char]0x6597)$([char]0x6A21)$([char]0x578B)"
$start = $text.IndexOf("private static readonly Dictionary<string, SpineApostleProfile> Profiles")
$end = $text.IndexOf("public static bool CanUseSpineBattle")
if ($start -lt 0 -or $end -le $start) {
    throw "Cannot locate Spine profile table in $profilePath"
}

$profiles = $text.Substring($start, $end - $start)
$assetKeys = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)

$primaryPattern = [regex]'\]\s*=\s*new\([^,]+,\s*"([^"]+)"\s*,\s*"Normal"'
foreach ($match in $primaryPattern.Matches($profiles)) {
    $resourceCode = $match.Groups[1].Value
    Add-Asset $assetKeys $normalApostleCategory $resourceCode

    $lineEnd = $profiles.IndexOf("`n", $match.Index)
    if ($lineEnd -lt 0) {
        $lineEnd = $profiles.Length
    }
    $line = $profiles.Substring($match.Index, $lineEnd - $match.Index)

    $battleCodeMatch = [regex]::Match($line, 'BattleResourceCode:\s*"([^"]+)"')
    $battleCategoryMatch = [regex]::Match($line, 'BattleCategory:\s*@?"([^"]+)"')
    $battleResourceCode = if ($battleCodeMatch.Success) { $battleCodeMatch.Groups[1].Value } else { $resourceCode }
    $battleCategory = if ($battleCategoryMatch.Success) { $battleCategoryMatch.Groups[1].Value } else { $battleModelCategory }
    Add-Asset $assetKeys $battleCategory $battleResourceCode
}

$secondaryPattern = [regex]'new\("([^"]+)"\s*,\s*@?"([^"]+)"'
foreach ($match in $secondaryPattern.Matches($profiles)) {
    Add-Asset $assetKeys $match.Groups[2].Value $match.Groups[1].Value
}

$destinations = New-Object System.Collections.Generic.List[string]
if (-not [string]::IsNullOrWhiteSpace($LocalModDir)) {
    $destinations.Add((Join-Path $LocalModDir "CultLeaderMod\spine_assets"))
}
if (-not [string]::IsNullOrWhiteSpace($WorkshopContentDir)) {
    $destinations.Add((Join-Path $WorkshopContentDir "CultLeaderMod\spine_assets"))
}
if (-not [string]::IsNullOrWhiteSpace($WorkshopSubscriptionDir)) {
    $destinations.Add((Join-Path $WorkshopSubscriptionDir "CultLeaderMod\spine_assets"))
}

if ($destinations.Count -eq 0) {
    Write-Host "No Spine destinations provided; nothing to sync."
    exit 0
}

$filesToCopy = New-Object System.Collections.Generic.List[object]
$missing = New-Object System.Collections.Generic.List[string]

foreach ($key in $assetKeys) {
    $category, $resourceCode = $key.Split("|", 2)
    $categoryPath = Convert-CategoryPath $category
    $sourceDir = Join-Path (Join-Path $SourceRoot $categoryPath) $resourceCode.ToLowerInvariant()
    if (-not (Test-Path -LiteralPath $sourceDir -PathType Container)) {
        $missing.Add($sourceDir)
        continue
    }

    # JSON is the actual skeleton for most profiles, and also supplies skin and
    # animation metadata when a binary skeleton is used. Never omit it globally.
    $binaryPath = Join-Path $sourceDir "$resourceCode.skel"
    $jsonPath = Join-Path $sourceDir "$resourceCode.spine-json"
    if (-not (Test-Path -LiteralPath $binaryPath -PathType Leaf) -and
        -not (Test-Path -LiteralPath $jsonPath -PathType Leaf)) {
        $missing.Add("Missing skeleton: $sourceDir / $resourceCode")
    }
    foreach ($extension in @('atlas', 'png')) {
        $requiredPath = Join-Path $sourceDir "$resourceCode.$extension"
        if (-not (Test-Path -LiteralPath $requiredPath -PathType Leaf)) {
            $missing.Add($requiredPath)
        }
    }
    $runtimeFiles = Get-ChildItem -LiteralPath $sourceDir -File | Where-Object {
        $_.Extension -in @('.skel', '.spine-json', '.atlas', '.png')
    }

    foreach ($file in $runtimeFiles) {
        $relativeDir = Join-Path $categoryPath $resourceCode.ToLowerInvariant()
        $filesToCopy.Add([pscustomobject]@{
            Source = $file.FullName
            RelativeDir = $relativeDir
        })
    }
}

if ($missing.Count -gt 0) {
    throw ("Incomplete Spine source assets ({0}); no destinations modified:`n{1}" -f $missing.Count, ($missing -join "`n"))
}

foreach ($destination in $destinations) {
    $destFull = Assert-SafeSpineDestination $destination
    if ($Clean -and (Test-Path -LiteralPath $destFull)) {
        Remove-Item -LiteralPath $destFull -Recurse -Force
    }

    New-Item -ItemType Directory -Force -Path $destFull | Out-Null
    Set-Content -LiteralPath (Join-Path $destFull ".gdignore") -Value "" -NoNewline -Encoding ASCII

    foreach ($item in $filesToCopy) {
        $targetDir = Join-Path $destFull $item.RelativeDir
        New-Item -ItemType Directory -Force -Path $targetDir | Out-Null
        Copy-Item -LiteralPath $item.Source -Destination (Join-Path $targetDir (Split-Path -Leaf $item.Source)) -Force
    }

    foreach ($item in $filesToCopy) {
        $target = Join-Path (Join-Path $destFull $item.RelativeDir) (Split-Path -Leaf $item.Source)
        if ((Get-PublishHash $item.Source) -ne (Get-PublishHash $target)) {
            throw "Spine publish verification failed: $target"
        }
    }

    Get-ChildItem -LiteralPath $destFull -Recurse -File -Filter "*.import" -ErrorAction SilentlyContinue |
        Remove-Item -Force
}

$totalBytes = 0L
foreach ($item in $filesToCopy) {
    $totalBytes += (Get-Item -LiteralPath $item.Source).Length
}

Write-Host ("Synced {0} Spine asset directories, {1} runtime files, {2:N2} MB to {3} destination(s)." -f `
    $assetKeys.Count, $filesToCopy.Count, ($totalBytes / 1MB), $destinations.Count)
