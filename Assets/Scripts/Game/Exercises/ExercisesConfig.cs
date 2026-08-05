using System.Collections.Generic;
using UnityEngine;

namespace Game.Exercises
{
    [CreateAssetMenu(fileName = "ExercisesConfig", menuName = "Game/ExercisesConfig")]
    public class ExercisesConfig : ScriptableObject
    {
        public List<ExerciseData> Exercises = new List<ExerciseData>
        {
            new ExerciseData { Id = ProxyExerciseIds.PushUps, Title = "Отжимания", RepLabel = "+1 ОТЖИМАНИЕ" },
            new ExerciseData { Id = ProxyExerciseIds.Squats, Title = "Приседания", RepLabel = "+1 ПРИСЕДАНИЕ" },
            new ExerciseData { Id = ProxyExerciseIds.PullUps, Title = "Подтягивания", RepLabel = "+1 ПОДТЯГИВАНИЕ" },
            new ExerciseData { Id = ProxyExerciseIds.Plank, Title = "Планка", RepLabel = "+1 СЕКУНДА" },
            new ExerciseData { Id = ProxyExerciseIds.Walking, Title = "Ходьба", RepLabel = "+1 ШАГ" }
        };
    }
}
