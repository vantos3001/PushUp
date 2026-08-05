using System;
using System.Collections.Generic;

namespace Common.Save
{
    [Serializable]
    public class DayState
    {
        public string Date;
        public List<ExerciseDayState> Exercises = new List<ExerciseDayState>();
    }
}
