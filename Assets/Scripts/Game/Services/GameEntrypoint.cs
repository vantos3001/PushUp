using Common.Save;
using Game.Campaigns.Services;
using Game.Poses.Services;
using Game.Reps.Services;
using Game.Sounds.Services;
using Game.UI.Services;
using Zenject;

namespace Game.Services
{
    public class GameEntrypoint : IInitializable
    {
        private readonly InitGameStateService _initGameStateService;
        private readonly CameraRepCounterService _cameraRepCounterService;
        private readonly PoseRepCounterService _poseRepCounterService;
        private readonly CompositeRepCounterService _compositeRepCounterService;
        private readonly SoundService _soundService;
        private readonly CampaignService _campaignService;
        private readonly UIRootService _uiRootService;
        private readonly UIMenuService _uiMenuService;
        private readonly UIBattleService _uiBattleService;
        private readonly UIStatsService _uiStatsService;
        private readonly UICampaignCompleteService _uiCampaignCompleteService;
        private readonly UIToastService _uiToastService;
        private readonly UIFlowService _uiFlowService;

        public GameEntrypoint(
            InitGameStateService initGameStateService,
            CameraRepCounterService cameraRepCounterService,
            PoseRepCounterService poseRepCounterService,
            CompositeRepCounterService compositeRepCounterService,
            SoundService soundService,
            CampaignService campaignService,
            UIRootService uiRootService,
            UIMenuService uiMenuService,
            UIBattleService uiBattleService,
            UIStatsService uiStatsService,
            UICampaignCompleteService uiCampaignCompleteService,
            UIToastService uiToastService,
            UIFlowService uiFlowService)
        {
            _initGameStateService = initGameStateService;
            _cameraRepCounterService = cameraRepCounterService;
            _poseRepCounterService = poseRepCounterService;
            _compositeRepCounterService = compositeRepCounterService;
            _soundService = soundService;
            _campaignService = campaignService;
            _uiRootService = uiRootService;
            _uiMenuService = uiMenuService;
            _uiBattleService = uiBattleService;
            _uiStatsService = uiStatsService;
            _uiCampaignCompleteService = uiCampaignCompleteService;
            _uiToastService = uiToastService;
            _uiFlowService = uiFlowService;
        }

        public void Initialize()
        {
            _initGameStateService.Init();
            _poseRepCounterService.Init();
            _compositeRepCounterService.Init();
            _soundService.Init();
            _campaignService.Init();
            _uiRootService.Init();
            _uiMenuService.Init();
            _uiBattleService.Init();
            _uiStatsService.Init();
            _uiCampaignCompleteService.Init();
            _uiToastService.Init();
            _uiFlowService.Init();
            _cameraRepCounterService.RequestPermission();
        }
    }
}
