param(
    [string]$ProjectRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path,
    [string]$LocalModDir = "E:\SteamLibrary\steamapps\common\Slay the Spire 2\mods\CultLeaderMod",
    [string]$WorkshopContentDir = ""
)

$ErrorActionPreference = "Stop"

$projectRootFull = [System.IO.Path]::GetFullPath($ProjectRoot)
if ([string]::IsNullOrWhiteSpace($WorkshopContentDir)) {
    $WorkshopContentDir = Join-Path $projectRootFull "release\workshop\CultLeaderModWorkspace\content"
}

$localModFull = [System.IO.Path]::GetFullPath($LocalModDir)
$workshopContentFull = [System.IO.Path]::GetFullPath($WorkshopContentDir)
$expectedWorkshopRoot = [System.IO.Path]::GetFullPath(
    (Join-Path $projectRootFull "release\workshop\CultLeaderModWorkspace")
)

if (-not $workshopContentFull.StartsWith(
    $expectedWorkshopRoot + [System.IO.Path]::DirectorySeparatorChar,
    [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "Refusing unexpected Workshop content directory: $workshopContentFull"
}

$sourceLocalization = Join-Path $projectRootFull "CultLeaderMod\localization"
$targetLocalization = Join-Path $workshopContentFull "CultLeaderMod\localization"
if (-not (Test-Path -LiteralPath $sourceLocalization -PathType Container)) {
    throw "Localization source not found: $sourceLocalization"
}
if (-not (Test-Path -LiteralPath $localModFull -PathType Container)) {
    throw "Local mod directory not found: $localModFull"
}

New-Item -ItemType Directory -Force -Path $workshopContentFull | Out-Null

$rootFiles = @(
    "CultLeaderMod.dll",
    "CultLeaderMod.pdb",
    "CultLeaderMod.pck",
    "CultLeaderMod.json",
    "external_vfx_manifest.json"
)

foreach ($name in $rootFiles) {
    $source = Join-Path $localModFull $name
    if (-not (Test-Path -LiteralPath $source -PathType Leaf)) {
        throw "Required local mod file not found: $source"
    }
    Copy-Item -LiteralPath $source -Destination (Join-Path $workshopContentFull $name) -Force
}

$sourceFiles = Get-ChildItem -LiteralPath $sourceLocalization -Recurse -File
foreach ($source in $sourceFiles) {
    $relative = [System.IO.Path]::GetRelativePath($sourceLocalization, $source.FullName)
    $target = Join-Path $targetLocalization $relative
    New-Item -ItemType Directory -Force -Path (Split-Path -Parent $target) | Out-Null
    Copy-Item -LiteralPath $source.FullName -Destination $target -Force
}

# Remove stale loose files one by one, after constraining every path to the localization tree.
if (Test-Path -LiteralPath $targetLocalization -PathType Container) {
    foreach ($target in Get-ChildItem -LiteralPath $targetLocalization -Recurse -File) {
        $relative = [System.IO.Path]::GetRelativePath($targetLocalization, $target.FullName)
        $source = Join-Path $sourceLocalization $relative
        if (-not (Test-Path -LiteralPath $source -PathType Leaf)) {
            $targetFull = [System.IO.Path]::GetFullPath($target.FullName)
            if (-not $targetFull.StartsWith(
                $targetLocalization + [System.IO.Path]::DirectorySeparatorChar,
                [System.StringComparison]::OrdinalIgnoreCase)) {
                throw "Refusing to remove unexpected file: $targetFull"
            }
            Remove-Item -LiteralPath $targetFull -Force
        }
    }
}

foreach ($name in $rootFiles) {
    $source = Join-Path $localModFull $name
    $target = Join-Path $workshopContentFull $name
    if ((Get-FileHash -LiteralPath $source).Hash -ne (Get-FileHash -LiteralPath $target).Hash) {
        throw "Workshop root file verification failed: $name"
    }
}

foreach ($source in $sourceFiles) {
    $relative = [System.IO.Path]::GetRelativePath($sourceLocalization, $source.FullName)
    $target = Join-Path $targetLocalization $relative
    if ((Get-FileHash -LiteralPath $source.FullName).Hash -ne (Get-FileHash -LiteralPath $target).Hash) {
        throw "Workshop localization verification failed: $relative"
    }
    Get-Content -Raw -LiteralPath $target | ConvertFrom-Json | Out-Null
}

$extraFiles = Get-ChildItem -LiteralPath $targetLocalization -Recurse -File | Where-Object {
    $relative = [System.IO.Path]::GetRelativePath($targetLocalization, $_.FullName)
    -not (Test-Path -LiteralPath (Join-Path $sourceLocalization $relative) -PathType Leaf)
}
if ($extraFiles) {
    throw "Unexpected localization files remain in Workshop content."
}

Write-Host ("Workshop content verified: {0} root files and {1} localization files." -f `
    $rootFiles.Count, $sourceFiles.Count)
