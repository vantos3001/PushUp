using System;
using System.Collections.Generic;

namespace Common.Save
{
    [Serializable]
    public class GameState
    {
        public int Version = 3;
        public int TotalReps = 0;
        public int TotalMonstersKilled = 0;
        public int Gold = 0;
        public List<CampaignTrackState> Tracks = new List<CampaignTrackState>();
        public List<DayState> Days = new List<DayState>();
    }
}
