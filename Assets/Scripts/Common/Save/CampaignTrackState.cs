using System;

namespace Common.Save
{
    [Serializable]
    public class CampaignTrackState
    {
        public string Id;
        public int LevelIndex;
        public int RepsDoneInLevel;
        public int Prestige;
    }
}
