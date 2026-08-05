using System;
using Game.Battles.Services;
using Game.Campaigns;
using Game.Campaigns.Services;
using Game.Reps.Services;

namespace Game.UI.Services
{
    public class UIFlowService : IDisposable
    {
        private readonly UIMenuService _uiMenuService;
        private readonly UIBattleService _uiBattleService;
        private readonly UIStatsService _uiStatsService;
        private readonly UIToastService _uiToastService;
        private readonly UICampaignCompleteService _uiCampaignCompleteService;
        private readonly CampaignService _campaignService;
        private readonly BattleService _battleService;
        private readonly CameraRepCounterService _cameraRepCounterService;

        public UIFlowService(
            UIMenuService uiMenuService,
            UIBattleService uiBattleService,
            UIStatsService uiStatsService,
            UIToastService uiToastService,
            UICampaignCompleteService uiCampaignCompleteService,
            CampaignService campaignService,
            BattleService battleService,
            CameraRepCounterService cameraRepCounterService)
        {
            _uiMenuService = uiMenuService;
            _uiBattleService = uiBattleService;
            _uiStatsService = uiStatsService;
            _uiToastService = uiToastService;
            _uiCampaignCompleteService = uiCampaignCompleteService;
            _campaignService = campaignService;
            _battleService = battleService;
            _cameraRepCounterService = cameraRepCounterService;
        }

        public void Init()
        {
            _uiMenuService.FightClicked += OnFightClicked;
            _uiMenuService.TrainingClicked += OnTrainingClicked;
            _uiMenuService.StatsClicked += OnStatsClicked;
            _uiBattleService.GiveUpClicked += OnGiveUpClicked;
            _uiStatsService.BackClicked += OnStatsBackClicked;
            _uiCampaignCompleteService.ContinueClicked += OnContinueClicked;
            _campaignService.LevelCompleted += OnLevelCompleted;
            _campaignService.CampaignCompleted += OnCampaignCompleted;

            _uiMenuService.Show();
        }

        private void OnFightClicked()
        {
            StartTrack(ProxyTrackIds.Dungeon);
        }

        private void OnTrainingClicked()
        {
            StartTrack(ProxyTrackIds.PushUps);
        }

        private void StartTrack(string trackId)
        {
            _uiMenuService.Hide();
            _campaignService.Begin(trackId);
            _cameraRepCounterService.StartCamera();
            _uiBattleService.Show();
        }

        private void OnGiveUpClicked()
        {
            _campaignService.GiveUp();
            _cameraRepCounterService.StopCamera();
            _uiBattleService.Hide();
            _uiMenuService.Show();
        }

        private void OnStatsClicked()
        {
            _uiMenuService.Hide();
            _uiStatsService.Show();
        }

        private void OnStatsBackClicked()
        {
            _uiStatsService.Hide();
            _uiMenuService.Show();
        }

        private void OnLevelCompleted()
        {
            var level = _battleService.Level;
            var message = $"«{level.MonsterName}» повержен! +{_campaignService.LastGoldReward} золота";
            _uiToastService.Show(message);
            _campaignService.StartCurrentLevel();
        }

        private void OnCampaignCompleted()
        {
            var trackState = _campaignService.GetTrackState(_campaignService.ActiveTrack.Id);
            _uiCampaignCompleteService.Show(trackState.Prestige);
        }

        private void OnContinueClicked()
        {
            _uiCampaignCompleteService.Hide();
            _cameraRepCounterService.StopCamera();
            _uiBattleService.Hide();
            _uiMenuService.Show();
        }

        public void Dispose()
        {
            _uiMenuService.FightClicked -= OnFightClicked;
            _uiMenuService.TrainingClicked -= OnTrainingClicked;
            _uiMenuService.StatsClicked -= OnStatsClicked;
            _uiBattleService.GiveUpClicked -= OnGiveUpClicked;
            _uiStatsService.BackClicked -= OnStatsBackClicked;
            _uiCampaignCompleteService.ContinueClicked -= OnContinueClicked;
            _campaignService.LevelCompleted -= OnLevelCompleted;
            _campaignService.CampaignCompleted -= OnCampaignCompleted;
        }
    }
}
