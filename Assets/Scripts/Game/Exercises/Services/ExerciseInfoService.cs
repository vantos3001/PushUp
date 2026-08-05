using System.Collections.Generic;

namespace Game.Exercises.Services
{
    public class ExerciseInfoService
    {
        private readonly ExercisesConfig _exercisesConfig;

        public ExerciseInfoService(ExercisesConfig exercisesConfig)
        {
            _exercisesConfig = exercisesConfig;
        }

        public List<ExerciseData> GetAll()
        {
            return _exercisesConfig.Exercises;
        }

        public string GetTitle(string exerciseId)
        {
            var exercise = Find(exerciseId);
            if (exercise == null)
            {
                return exerciseId;
            }

            return exercise.Title;
        }

        public string GetRepLabel(string exerciseId)
        {
            var exercise = Find(exerciseId);
            if (exercise == null)
            {
                return "+1";
            }

            return exercise.RepLabel;
        }

        private ExerciseData Find(string exerciseId)
        {
            var exercises = _exercisesConfig.Exercises;

            for (var i = 0; i < exercises.Count; i++)
            {
                if (exercises[i].Id != exerciseId)
                {
                    continue;
                }

                return exercises[i];
            }

            return null;
        }
    }
}
