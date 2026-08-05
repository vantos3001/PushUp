<#
.SYNOPSIS
    Generate 2D images for the Number Town project via Pollinations.ai (free, no API key).

.DESCRIPTION
    Sends a prompt to Pollinations (FLUX family models) and downloads a PNG into Assets/Art/Generated.
    This is a plain HTTPS GET -> PNG. No executable code comes back from the service.

.PARAMETER Prompt
    Image description. English works noticeably better.

.PARAMETER Name
    Output file name without extension. Defaults to a slug of the prompt + seed.

.PARAMETER Width / Height
    Image size. Default 1024x1024.

.PARAMETER Seed
    Seed for reproducibility. 0 = random (derived from current time).

.PARAMETER Model
    flux (default, general) | flux-realism | flux-anime | turbo (fast).

.PARAMETER NoStyle
    Do not prepend the project style prefix (generate strictly from the prompt).

.PARAMETER OutDir
    Output directory. Defaults to Assets/Art/Generated.

.EXAMPLE
    ./Tools/gen-image.ps1 -Prompt "a cozy house with number 7 on the door" -Name house_07
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)][string]$Prompt,
    [string]$Name,
    [int]$Width = 1024,
    [int]$Height = 1024,
    [long]$Seed = 0,
    [ValidateSet('flux', 'flux-realism', 'flux-anime', 'turbo')][string]$Model = 'flux',
    [ValidateSet('A', 'B')][string]$Style = 'A',
    [switch]$NoStyle,
    [string]$OutDir
)

# Style presets for Number Town (see Assets/Art/prompt-log.md and References/references.md).
#  A = soft flat / cozy illustration (warmer, painterly-ish, stable on FLUX).
#  B = strict flat vector (solid fills, no shadows; closer to ref 'flat-city-sideview').
$Styles = @{
    A = 'flat 2d vector illustration, side view, front-facing facade, simple shapes, ' +
        'minimal details, warm pastel colors, red orange rooftops, soft flat lighting, ' +
        'cozy town, childrens book style, '
    B = 'flat design vector art, side view, front-facing facade, solid fill colors, ' +
        'absolutely flat, no shadows, no gradients, no texture, bold simple geometric shapes, ' +
        'minimal details, adobe illustrator sticker style, warm pastel palette, red orange roof, '
}
$StylePrefix = $Styles[$Style]

$fullPrompt = if ($NoStyle) { $Prompt } else { $StylePrefix + $Prompt }

if ($Seed -le 0) { $Seed = [long](Get-Date -UFormat %s) }

if (-not $OutDir) {
    $OutDir = Join-Path $PSScriptRoot '..\Assets\Art\Generated'
}
if (-not (Test-Path $OutDir)) { New-Item -ItemType Directory -Force -Path $OutDir | Out-Null }

if (-not $Name) {
    $slug = ($Prompt.ToLower() -replace '[^a-z0-9]+', '-').Trim('-')
    if ($slug.Length -gt 40) { $slug = $slug.Substring(0, 40).Trim('-') }
    $Name = "$slug-$Seed"
}
$outPath = Join-Path $OutDir "$Name.png"

$encoded = [uri]::EscapeDataString($fullPrompt)
$url = "https://image.pollinations.ai/prompt/$encoded" +
    "?width=$Width&height=$Height&seed=$Seed&model=$Model&nologo=true&enhance=true"

Write-Host "Prompt : $fullPrompt"
Write-Host "Model  : $Model | ${Width}x${Height} | seed=$Seed"
Write-Host "Saving : $outPath"

try {
    Invoke-WebRequest -Uri $url -OutFile $outPath -TimeoutSec 120 -ErrorAction Stop
}
catch {
    Write-Error "Generation failed: $($_.Exception.Message). Retry, or try -Model turbo."
    exit 1
}

$sizeKb = [math]::Round((Get-Item $outPath).Length / 1KB, 1)
Write-Host "Done   : $outPath ($sizeKb KB)" -ForegroundColor Green
$outPath
