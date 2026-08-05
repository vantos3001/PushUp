using System;
using System.Collections.Generic;
using Common.Save;

namespace Game.Stats.Services
{
    public class StatsService
    {
        private readonly GameStateProvider _gameStateProvider;

        public StatsService(GameStateProvider gameStateProvider)
        {
            _gameStateProvider = gameStateProvider;
        }

        public void AddReps(string exerciseId, int reps)
        {
            var day = GetOrCreateToday();
            var exercise = GetOrCreateExercise(day, exerciseId);
            exercise.Reps += reps;
        }

        public int GetReps(DayState day, string exerciseId)
        {
            for (var i = 0; i < day.Exercises.Count; i++)
            {
                if (day.Exercises[i].ExerciseId != exerciseId)
                {
                    continue;
                }

                return day.Exercises[i].Reps;
            }

            return 0;
        }

        public int GetTotalReps(string exerciseId)
        {
            var days = _gameStateProvider.State.Days;
            var total = 0;

            for (var i = 0; i < days.Count; i++)
            {
                total += GetReps(days[i], exerciseId);
            }

            return total;
        }

        public int GetActiveDaysCount(string exerciseId)
        {
            var days = _gameStateProvider.State.Days;
            var count = 0;

            for (var i = 0; i < days.Count; i++)
            {
                if (GetReps(days[i], exerciseId) <= 0)
                {
                    continue;
                }

                count += 1;
            }

            return count;
        }

        public int GetAverageRepsPerDay(string exerciseId)
        {
            var activeDays = GetActiveDaysCount(exerciseId);
            if (activeDays == 0)
            {
                return 0;
            }

            return GetTotalReps(exerciseId) / activeDays;
        }

        public int GetTodayReps(string exerciseId)
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var days = _gameStateProvider.State.Days;

            for (var i = days.Count - 1; i >= 0; i--)
            {
                if (days[i].Date != today)
                {
                    continue;
                }

                return GetReps(days[i], exerciseId);
            }

            return 0;
        }

        public List<DayState> GetLastDays(int count)
        {
            var days = _gameStateProvider.State.Days;
            var result = new List<DayState>();
            var start = Math.Max(0, days.Count - count);

            for (var i = start; i < days.Count; i++)
            {
                result.Add(days[i]);
            }

            return result;
        }

        private DayState GetOrCreateToday()
        {
            var today = DateTime.Now.ToString("yyyy-MM-dd");
            var days = _gameStateProvider.State.Days;

            for (var i = 0; i < days.Count; i++)
            {
                if (days[i].Date != today)
                {
                    continue;
                }

                return days[i];
            }

            var day = new DayState { Date = today };
            days.Add(day);

            return day;
        }

        private static ExerciseDayState GetOrCreateExercise(DayState day, string exerciseId)
        {
            for (var i = 0; i < day.Exercises.Count; i++)
            {
                if (day.Exercises[i].ExerciseId != exerciseId)
                {
                    continue;
                }

                return day.Exercises[i];
            }

            var exercise = new ExerciseDayState { ExerciseId = exerciseId };
            day.Exercises.Add(exercise);

            return exercise;
        }
    }
}
