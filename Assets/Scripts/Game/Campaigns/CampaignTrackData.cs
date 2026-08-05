using System;

namespace Game.Campaigns
{
    [Serializable]
    public class CampaignTrackData
    {
        public string Id;
        public string Title;
        public string ExerciseId;
        public float GoldShare = 1f;
    }
}
