using System;
using System.Text;
using Common.Save;
using Common.UI.Services;
using Game.Exercises;
using Game.Exercises.Services;
using Game.Stats.Services;

namespace Game.UI.Services
{
    public class UIStatsService : IDisposable
    {
        private const int LastDaysCount = 7;

        private readonly UIRootService _uiRootService;
        private readonly UIWidgetFactory _uiWidgetFactory;
        private readonly GameStateProvider _gameStateProvider;
        private readonly StatsService _statsService;
        private readonly ExerciseInfoService _exerciseInfoService;

        private UIStatsScreen _screen;

        public event Action BackClicked;

        public UIStatsService(
            UIRootService uiRootService,
            UIWidgetFactory uiWidgetFactory,
            GameStateProvider gameStateProvider,
            StatsService statsService,
            ExerciseInfoService exerciseInfoService)
        {
            _uiRootService = uiRootService;
            _uiWidgetFactory = uiWidgetFactory;
            _gameStateProvider = gameStateProvider;
            _statsService = statsService;
            _exerciseInfoService = exerciseInfoService;
        }

        public void Init()
        {
            _screen = UIStatsScreen.Create(_uiRootService.Root, _uiWidgetFactory);
            _screen.BackClicked += OnBackClicked;
            _screen.gameObject.SetActive(false);
        }

        public void Show()
        {
            _screen.SetContent(BuildSummary(), BuildDays());
            _screen.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _screen.gameObject.SetActive(false);
        }

        private string BuildSummary()
        {
            var state = _gameStateProvider.State;
            var builder = new StringBuilder();

            var exercises = _exerciseInfoService.GetAll();
            for (var i = 0; i < exercises.Count; i++)
            {
                var exerciseId = exercises[i].Id;
                var total = _statsService.GetTotalReps(exerciseId);

                if (total == 0 && exerciseId != ProxyExerciseIds.PushUps)
                {
                    continue;
                }

                var average = _statsService.GetAverageRepsPerDay(exerciseId);
                var activeDays = _statsService.GetActiveDaysCount(exerciseId);
                builder.AppendLine($"{exercises[i].Title}: {total}");
                builder.AppendLine($"Среднее в день: {average} · Дней с тренировками: {activeDays}");
                builder.AppendLine();
            }

            builder.Append($"Монстров побеждено: {state.TotalMonstersKilled} · Кругов: {GetTotalPrestige()}");

            return builder.ToString();
        }

        private int GetTotalPrestige()
        {
            var tracks = _gameStateProvider.State.Tracks;
            var total = 0;

            for (var i = 0; i < tracks.Count; i++)
            {
                total += tracks[i].Prestige;
            }

            return total;
        }

        private string BuildDays()
        {
            var days = _statsService.GetLastDays(LastDaysCount);

            if (days.Count == 0)
            {
                return "Тренировок пока не было.\nСамое время отжаться!";
            }

            var builder = new StringBuilder();
            builder.AppendLine("Последние дни:");
            builder.AppendLine();

            for (var i = days.Count - 1; i >= 0; i--)
            {
                var day = days[i];
                var reps = _statsService.GetReps(day, ProxyExerciseIds.PushUps);
                builder.AppendLine($"{FormatDate(day.Date)} — {reps} отж.");
            }

            return builder.ToString();
        }

        private static string FormatDate(string date)
        {
            if (string.IsNullOrEmpty(date) || date.Length < 10)
            {
                return date;
            }

            return $"{date.Substring(8, 2)}.{date.Substring(5, 2)}";
        }

        private void OnBackClicked()
        {
            BackClicked?.Invoke();
        }

        public void Dispose()
        {
            if (_screen == null)
            {
                return;
            }

            _screen.BackClicked -= OnBackClicked;
        }
    }
}
