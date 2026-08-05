# Batch-генерация иконок монстров через Pollinations.ai (FLUX, бесплатно, без ключей).
# Файлы кладутся в Assets/Art/Monsters с именами tier{t}_{ii}.png / boss_{ii}.png —
# их автоматически подхватит MonsterIconAssigner (Tools/PushUp/Assign Monster Icons).
# Скрипт resume-able: уже скачанные файлы пропускает.

param(
    [string]$OutDir = "$PSScriptRoot\..\Assets\Art\Monsters",
    [int]$Size = 512
)

$ErrorActionPreference = "Continue"

$style = "fantasy RPG monster avatar icon, centered head and shoulders portrait, circular emblem, flat vector game art style, vibrant colors, dark navy blue background, high contrast, clean silhouette, no text, no letters, no watermark"

$tiers = @(
    @("green slime blob", "goblin", "spider", "horned beetle", "giant worm", "little imp", "vampire bat", "red crab", "frog warrior", "kobold", "rat man", "predatory mushroom"),
    @("grey wolf", "skeleton warrior", "zombie", "bandit rogue", "wild boar", "serpent snake", "vampiric undead ghoul", "mimic chest monster", "hobgoblin brute", "flesh-eating ghoul", "lizardman", "jackal beast"),
    @("orc warrior", "ghost specter", "cave troll", "ogre brute", "werewolf", "stone golem", "dark cultist", "basilisk", "minotaur", "griffin", "cyclops", "necromancer"),
    @("vampire lord", "horned demon", "dark sorcerer", "fire elemental", "revenant knight", "ifrit fire spirit", "djinn genie", "centaur warrior", "shapeshifter werebeast", "manticore", "gargoyle", "bone golem"),
    @("lich king", "dragon", "titan warrior", "kraken", "behemoth beast", "archdemon", "grim reaper", "stone colossus", "phoenix", "leviathan sea monster", "death knight", "abyssal horror")
)

$bosses = @(
    "slime king with golden crown", "goblin warchief", "spider queen broodmother", "emperor beetle",
    "great giant worm", "prince of imps", "lord of vampire bats", "giant armored crab",
    "swamp king", "elder mushroom patriarch",
    "alpha wolf pack leader", "bone king skeleton", "zombie goliath", "bandit chieftain",
    "destroyer war boar", "serpent king cobra", "ancient vampiric ghoul", "king of mimics",
    "hobgoblin warlord", "grave devourer undead",
    "orc warlord", "ghostly phantom lord", "troll bruiser giant", "ogre tyrant",
    "werewolf pack alpha", "colossal iron golem", "supreme cultist high priest", "basilisk king",
    "minotaur champion gladiator", "lord of the labyrinth",
    "ancient vampire elder", "duke of demons", "supreme dark warlock", "lord of elements",
    "avenger revenant knight", "sultan of fire ifrits", "despot djinn", "centaur warlord",
    "prince of werebeasts", "bone colossus giant",
    "lich emperor", "red dragon", "titan of war", "kraken of the depths",
    "behemoth of chaos", "archdemon of inferno", "soul reaper", "colossus of the void",
    "black phoenix", "lord of the abyss"
)

New-Item -ItemType Directory -Force $OutDir | Out-Null
$done = 0
$failed = 0

function Get-Icon([string]$prompt, [string]$outPath, [int]$seed, [int]$size, [string]$style)
{
    if (Test-Path $outPath)
    {
        return "skip"
    }

    $encoded = [uri]::EscapeDataString("$style, $prompt")
    $url = "https://image.pollinations.ai/prompt/$encoded" + "?width=$size&height=$size&seed=$seed&model=flux&nologo=true&enhance=true"

    for ($attempt = 1; $attempt -le 3; $attempt++)
    {
        try
        {
            Invoke-WebRequest -Uri $url -OutFile $outPath -TimeoutSec 180 -ErrorAction Stop
            return "ok"
        }
        catch
        {
            Write-Host "  retry $attempt failed: $($_.Exception.Message)"
            Start-Sleep -Seconds (5 * $attempt)
        }
    }

    return "fail"
}

for ($t = 0; $t -lt $tiers.Count; $t++)
{
    for ($i = 0; $i -lt $tiers[$t].Count; $i++)
    {
        $name = "tier{0}_{1:d2}.png" -f $t, $i
        $result = Get-Icon $tiers[$t][$i] (Join-Path $OutDir $name) (1000 + $t * 100 + $i) $Size $style
        Write-Host "$name -> $result ($($tiers[$t][$i]))"
        if ($result -eq "ok") { $done++; Start-Sleep -Seconds 1 }
        if ($result -eq "fail") { $failed++ }
    }
}

for ($i = 0; $i -lt $bosses.Count; $i++)
{
    $name = "boss_{0:d2}.png" -f $i
    $result = Get-Icon $bosses[$i] (Join-Path $OutDir $name) (5000 + $i) $Size $style
    Write-Host "$name -> $result ($($bosses[$i]))"
    if ($result -eq "ok") { $done++; Start-Sleep -Seconds 1 }
    if ($result -eq "fail") { $failed++ }
}

Write-Host ""
Write-Host "Downloaded: $done, failed: $failed. Rerun the script to retry failures."
