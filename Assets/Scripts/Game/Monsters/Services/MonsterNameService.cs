using Common;
using Game.Campaigns;
using UnityEngine;

namespace Game.Monsters.Services
{
    public class MonsterNameService
    {
        private readonly MonstersConfig _monstersConfig;
        private readonly CampaignConfig _campaignConfig;

        public MonsterNameService(MonstersConfig monstersConfig, CampaignConfig campaignConfig)
        {
            _monstersConfig = monstersConfig;
            _campaignConfig = campaignConfig;
        }

        public MonsterData GetRegularMonster(int chapter, int levelIndex)
        {
            var tiers = _monstersConfig.Tiers;
            if (tiers.Count == 0)
            {
                return CreateFallback();
            }

            var chapterCount = Mathf.Max(1, _campaignConfig.ChapterCount);
            var tierIndex = Mathf.Clamp(chapter * tiers.Count / chapterCount, 0, tiers.Count - 1);
            var monsters = tiers[tierIndex].Monsters;
            if (monsters.Count == 0)
            {
                return CreateFallback();
            }

            var monster = monsters[ProxyDeterministicRandom.Range(levelIndex * 7 + 1, monsters.Count)];

            return new MonsterData
            {
                Name = ApplyPrefix(monster.Name, levelIndex),
                Icon = monster.Icon
            };
        }

        public MonsterData GetBossMonster(int chapter)
        {
            var bosses = _monstersConfig.Bosses;
            if (bosses.Count == 0)
            {
                return CreateFallback();
            }

            var index = Mathf.Clamp(chapter, 0, bosses.Count - 1);

            return bosses[index];
        }

        public Color GetColor(string monsterName)
        {
            if (string.IsNullOrEmpty(monsterName))
            {
                return Color.gray;
            }

            var hash = 0;
            for (var i = 0; i < monsterName.Length; i++)
            {
                hash = hash * 31 + monsterName[i];
            }

            var hue = ProxyDeterministicRandom.Range(hash, 360) / 360f;

            return Color.HSVToRGB(hue, 0.55f, 0.85f);
        }

        private string ApplyPrefix(string baseName, int levelIndex)
        {
            var prefixes = _monstersConfig.Prefixes;
            if (prefixes.Count == 0)
            {
                return baseName;
            }

            var prefix = prefixes[ProxyDeterministicRandom.Range(levelIndex * 13 + 5, prefixes.Count)];
            if (string.IsNullOrEmpty(prefix))
            {
                return baseName;
            }

            return prefix + " " + baseName;
        }

        private static MonsterData CreateFallback()
        {
            return new MonsterData { Name = "Монстр" };
        }
    }
}
