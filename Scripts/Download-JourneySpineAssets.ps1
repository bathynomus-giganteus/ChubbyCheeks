param(
    [string]$AssetRoot = "E:\work\Cult_leader_mod",
    [string]$OutputRoot = "E:\work\Cult_leader_mod\SPINE",
    [string]$ManifestUrl = "https://journey.927927927.xyz/spine-manifest.json",
    [string]$AssetBaseUrl = "https://assets.927927927.xyz/spine",
    [string[]]$ResourceCode = @(),
    [switch]$IncludeDataFiles,
    [switch]$Force,
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

function Join-UrlPath {
    param(
        [Parameter(Mandatory = $true)][string]$BaseUrl,
        [Parameter(Mandatory = $true)][string[]]$Segments
    )

    $base = $BaseUrl.TrimEnd("/")
    $encoded = $Segments |
        Where-Object { -not [string]::IsNullOrWhiteSpace($_) } |
        ForEach-Object { [Uri]::EscapeDataString($_) }

    return $base + "/" + ($encoded -join "/")
}

function Get-EntryOutputDirectory {
    param(
        [Parameter(Mandatory = $true)]$Entry,
        [Parameter(Mandatory = $true)][string]$Root
    )

    $parts = $Entry.folderName -split "[\\/]" | Where-Object { $_ }
    return Join-Path $Root (Join-Path -Path $parts[0] -ChildPath (($parts | Select-Object -Skip 1) -join [IO.Path]::DirectorySeparatorChar))
}

function Get-ProjectReferences {
    param([Parameter(Mandatory = $true)][string]$Root)

    if (!(Test-Path -LiteralPath $Root)) {
        Write-Warning "AssetRoot not found: $Root"
        return @()
    }

    $jsonFiles = Get-ChildItem -LiteralPath $Root -Recurse -File -Filter "*.json" -ErrorAction SilentlyContinue

    $refs = @()
    foreach ($file in $jsonFiles) {
        try {
            $project = Get-Content -LiteralPath $file.FullName -Raw -Encoding UTF8 | ConvertFrom-Json
            if ($project.schema -ne "trickcal-studio-project") { continue }
            foreach ($layer in @($project.modelLayers)) {
                if (!$layer.resourceCode) { continue }

                $folderName = $null
                if ($layer.resourceId) {
                    $parts = [string]$layer.resourceId -split "\|"
                    if ($parts.Count -ge 3) {
                        $folderName = [Uri]::UnescapeDataString($parts[2])
                    }
                }

                $refs += [pscustomobject]@{
                    SourceFile   = $file.FullName
                    ResourceCode = [string]$layer.resourceCode
                    FolderName   = $folderName
                    Animation    = [string]$layer.animationName
                    Skin         = [string]$layer.skinName
                }
            }
        } catch {
            Write-Warning "Failed to read project json: $($file.FullName) :: $($_.Exception.Message)"
        }
    }

    return $refs
}

function Save-RemoteFile {
    param(
        [Parameter(Mandatory = $true)][string]$Url,
        [Parameter(Mandatory = $true)][string]$Destination,
        [switch]$Force,
        [switch]$DryRun
    )

    if ((Test-Path -LiteralPath $Destination) -and !$Force) {
        Write-Host "skip existing: $Destination"
        return "skipped"
    }

    if ($DryRun) {
        Write-Host "would download: $Url"
        Write-Host "           to: $Destination"
        return "dry-run"
    }

    $parent = Split-Path -Parent $Destination
    New-Item -ItemType Directory -Path $parent -Force | Out-Null
    Invoke-WebRequest -Uri $Url -OutFile $Destination -UseBasicParsing -TimeoutSec 120
    Write-Host "downloaded: $Destination"
    return "downloaded"
}

Write-Host "Loading manifest: $ManifestUrl"
$manifest = Invoke-RestMethod -Uri $ManifestUrl -TimeoutSec 60
$entries = @($manifest.entries | Where-Object { $_.isComplete })
$ResourceCode = @(
    foreach ($code in $ResourceCode) {
        foreach ($part in ([string]$code -split ",")) {
            $trimmed = $part.Trim()
            if ($trimmed) { $trimmed }
        }
    }
)

$wanted = New-Object System.Collections.Generic.List[object]

if ($ResourceCode.Count -gt 0) {
    foreach ($code in $ResourceCode) {
        $matches = @($entries | Where-Object { $_.resourceCode -eq $code })
        if ($matches.Count -eq 0) {
            Write-Warning "No manifest entry for ResourceCode: $code"
            continue
        }
        foreach ($match in $matches) { $wanted.Add($match) }
    }
} else {
    $refs = Get-ProjectReferences -Root $AssetRoot
    if ($refs.Count -eq 0) {
        Write-Warning "No trickcal-studio-project json references found under $AssetRoot. Use -ResourceCode ErpinRoyale to download manually."
    }

    foreach ($ref in $refs) {
        $matches = @($entries | Where-Object {
            $_.resourceCode -eq $ref.ResourceCode -and (!$ref.FolderName -or $_.folderName -eq $ref.FolderName)
        })
        if ($matches.Count -eq 0) {
            $matches = @($entries | Where-Object { $_.resourceCode -eq $ref.ResourceCode })
        }
        if ($matches.Count -eq 0) {
            Write-Warning "No manifest entry for $($ref.ResourceCode) from $($ref.SourceFile)"
            continue
        }
        foreach ($match in $matches) { $wanted.Add($match) }
    }
}

$unique = $wanted |
    Group-Object { "$($_.resourceCode)|$($_.folderName)" } |
    ForEach-Object { $_.Group[0] } |
    Sort-Object resourceCategory, resourceCode, folderName

Write-Host "Matched complete Spine resources: $($unique.Count)"
New-Item -ItemType Directory -Path $OutputRoot -Force | Out-Null

$summary = New-Object System.Collections.Generic.List[object]
foreach ($entry in $unique) {
    $folderParts = $entry.folderName -split "[\\/]" | Where-Object { $_ }
    $entryOutDir = Get-EntryOutputDirectory -Entry $entry -Root $OutputRoot

    $files = New-Object System.Collections.Generic.List[string]
    if ($entry.skeletonFileName) { $files.Add([string]$entry.skeletonFileName) }
    if ($entry.atlasFileName) { $files.Add([string]$entry.atlasFileName) }
    foreach ($texture in @($entry.textureFileNames)) { if ($texture) { $files.Add([string]$texture) } }
    if ($IncludeDataFiles) {
        foreach ($data in @($entry.dataFileNames)) { if ($data) { $files.Add([string]$data) } }
    }

    foreach ($name in ($files | Sort-Object -Unique)) {
        $url = Join-UrlPath -BaseUrl $AssetBaseUrl -Segments @($folderParts + $name)
        $dest = Join-Path $entryOutDir $name
        $status = Save-RemoteFile -Url $url -Destination $dest -Force:$Force -DryRun:$DryRun
        $summary.Add([pscustomobject]@{
            ResourceCode = $entry.resourceCode
            Category     = $entry.resourceCategory
            FolderName   = $entry.folderName
            FileName     = $name
            Status       = $status
            Destination  = $dest
        })
    }
}

$summaryPath = Join-Path $OutputRoot "download-summary.csv"
$summary | Export-Csv -LiteralPath $summaryPath -NoTypeInformation -Encoding UTF8
Write-Host "Summary written: $summaryPath"

if (!$DryRun) {
    $total = Get-ChildItem -LiteralPath $OutputRoot -Recurse -File -ErrorAction SilentlyContinue |
        Measure-Object -Property Length -Sum
    Write-Host ("Output size: {0} files, {1} MB" -f $total.Count, [math]::Round($total.Sum / 1MB, 2))
}
