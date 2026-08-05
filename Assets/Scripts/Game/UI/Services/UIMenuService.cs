using System;
using Common.Save;
using Common.UI.Services;
using Game.Campaigns;
using Game.Campaigns.Services;
using Game.Levels.Services;
using Game.Wallets.Services;

namespace Game.UI.Services
{
    public class UIMenuService : IDisposable
    {
        private readonly UIRootService _uiRootService;
        private readonly UIWidgetFactory _uiWidgetFactory;
        private readonly GameStateProvider _gameStateProvider;
        private readonly CampaignService _campaignService;
        private readonly LevelGenerationService _levelGenerationService;
        private readonly WalletService _walletService;

        private UIMenuScreen _screen;

        public event Action FightClicked;
        public event Action TrainingClicked;
        public event Action StatsClicked;

        public UIMenuService(
            UIRootService uiRootService,
            UIWidgetFactory uiWidgetFactory,
            GameStateProvider gameStateProvider,
            CampaignService campaignService,
            LevelGenerationService levelGenerationService,
            WalletService walletService)
        {
            _uiRootService = uiRootService;
            _uiWidgetFactory = uiWidgetFactory;
            _gameStateProvider = gameStateProvider;
            _campaignService = campaignService;
            _levelGenerationService = levelGenerationService;
            _walletService = walletService;
        }

        public void Init()
        {
            _screen = UIMenuScreen.Create(_uiRootService.Root, _uiWidgetFactory);
            _screen.FightClicked += OnFightClicked;
            _screen.TrainingClicked += OnTrainingClicked;
            _screen.StatsClicked += OnStatsClicked;
            _screen.gameObject.SetActive(false);
        }

        public void Show()
        {
            var state = _gameStateProvider.State;

            var levelText = GetTrackText("Данж", ProxyTrackIds.Dungeon);
            var trainingText = GetTrackText("Отжимания", ProxyTrackIds.PushUps);
            var statsText = $"{trainingText}\nВсего отжиманий: {state.TotalReps}\nМонстров побеждено: {state.TotalMonstersKilled}";
            var goldText = $"Золото: {_walletService.Gold}";

            _screen.SetContent(levelText, statsText, goldText);
            _screen.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _screen.gameObject.SetActive(false);
        }

        private string GetTrackText(string title, string trackId)
        {
            var level = _campaignService.GetCurrentLevel(trackId);
            if (level == null)
            {
                return $"{title}: —";
            }

            var text = $"{title}: уровень {level.Number} / {_levelGenerationService.TotalLevels}";

            var prestige = _campaignService.GetTrackState(trackId).Prestige;
            if (prestige > 0)
            {
                text += $" · Круг {prestige + 1}";
            }

            return text;
        }

        private void OnFightClicked()
        {
            FightClicked?.Invoke();
        }

        private void OnTrainingClicked()
        {
            TrainingClicked?.Invoke();
        }

        private void OnStatsClicked()
        {
            StatsClicked?.Invoke();
        }

        public void Dispose()
        {
            if (_screen == null)
            {
                return;
            }

            _screen.FightClicked -= OnFightClicked;
            _screen.TrainingClicked -= OnTrainingClicked;
            _screen.StatsClicked -= OnStatsClicked;
        }
    }
}
