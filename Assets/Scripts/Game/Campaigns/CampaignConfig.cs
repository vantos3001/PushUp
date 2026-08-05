using System.Collections.Generic;
using Game.Exercises;
using UnityEngine;

namespace Game.Campaigns
{
    [CreateAssetMenu(fileName = "CampaignConfig", menuName = "Game/CampaignConfig")]
    public class CampaignConfig : ScriptableObject
    {
        public int ChapterCount = 50;
        public int LevelsPerChapter = 10;
        public int MinHp = 3;
        public int FirstChapterCap = 4;
        public int MaxChapterCap = 20;
        [Range(0f, 1f)] public float RegularHpShare = 0.85f;
        public int BossRewardMultiplier = 2;

        public List<CampaignTrackData> Tracks = new List<CampaignTrackData>
        {
            new CampaignTrackData
            {
                Id = ProxyTrackIds.Dungeon,
                Title = "Данж",
                ExerciseId = ProxyExerciseIds.PushUps,
                GoldShare = 1f
            },
            new CampaignTrackData
            {
                Id = ProxyTrackIds.PushUps,
                Title = "Просто отжиматься",
                ExerciseId = ProxyExerciseIds.PushUps,
                GoldShare = 0.5f
            }
        };
    }
}
