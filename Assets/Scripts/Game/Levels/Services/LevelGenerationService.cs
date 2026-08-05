using Common;
using Game.Campaigns;
using Game.Monsters.Services;
using UnityEngine;

namespace Game.Levels.Services
{
    public class LevelGenerationService
    {
        private readonly CampaignConfig _campaignConfig;
        private readonly MonsterNameService _monsterNameService;

        public int TotalLevels => _campaignConfig.ChapterCount * _campaignConfig.LevelsPerChapter;

        public LevelGenerationService(CampaignConfig campaignConfig, MonsterNameService monsterNameService)
        {
            _campaignConfig = campaignConfig;
            _monsterNameService = monsterNameService;
        }

        public LevelData GetLevel(int levelIndex, string exerciseId)
        {
            var totalLevels = Mathf.Max(1, TotalLevels);
            var index = ((levelIndex % totalLevels) + totalLevels) % totalLevels;
            var chapter = index / _campaignConfig.LevelsPerChapter;
            var step = index % _campaignConfig.LevelsPerChapter;
            var cap = GetChapterCap(chapter);
            var isBoss = step == _campaignConfig.LevelsPerChapter - 1;
            var monster = isBoss
                ? _monsterNameService.GetBossMonster(chapter)
                : _monsterNameService.GetRegularMonster(chapter, index);

            return new LevelData
            {
                Index = index,
                Number = index + 1,
                Chapter = chapter,
                MonsterName = monster.Name,
                MonsterIcon = monster.Icon,
                ExerciseId = exerciseId,
                Hp = isBoss ? cap : GetRegularHp(index, step, cap),
                IsBoss = isBoss
            };
        }

        public int GetChapterCap(int chapter)
        {
            if (_campaignConfig.ChapterCount <= 1)
            {
                return _campaignConfig.MaxChapterCap;
            }

            var progress = chapter / (float)(_campaignConfig.ChapterCount - 1);
            var capRange = _campaignConfig.MaxChapterCap - _campaignConfig.FirstChapterCap;

            return _campaignConfig.FirstChapterCap + Mathf.RoundToInt(capRange * progress);
        }

        private int GetRegularHp(int index, int step, int cap)
        {
            var lastRegularStep = Mathf.Max(1, _campaignConfig.LevelsPerChapter - 2);
            var progress = step / (float)lastRegularStep;
            var minHp = _campaignConfig.MinHp;
            var baseHp = minHp + (cap - minHp) * progress * _campaignConfig.RegularHpShare;
            var jitter = ProxyDeterministicRandom.Range(index, 3) - 1;

            return Mathf.Clamp(Mathf.RoundToInt(baseHp) + jitter, minHp, cap);
        }
    }
}
