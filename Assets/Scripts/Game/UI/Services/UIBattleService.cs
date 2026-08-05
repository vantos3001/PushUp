using System;
using Common.Save;
using Common.UI.Services;
using Game.Battles.Services;
using Game.Exercises.Services;
using Game.Monsters.Services;
using Game.Poses;
using Game.Poses.Services;
using Game.Reps;
using Game.Reps.Services;

namespace Game.UI.Services
{
    public class UIBattleService : IDisposable
    {
        private readonly UIRootService _uiRootService;
        private readonly UIWidgetFactory _uiWidgetFactory;
        private readonly GameStateProvider _gameStateProvider;
        private readonly BattleService _battleService;
        private readonly ManualRepCounterService _manualRepCounterService;
        private readonly CameraRepCounterService _cameraRepCounterService;
        private readonly PoseEstimationService _poseEstimationService;
        private readonly PoseRepCounterService _poseRepCounterService;
        private readonly TrackingConfig _trackingConfig;
        private readonly MonsterNameService _monsterNameService;
        private readonly ExerciseInfoService _exerciseInfoService;

        private UIBattleScreen _screen;

        public event Action GiveUpClicked;

        public UIBattleService(
            UIRootService uiRootService,
            UIWidgetFactory uiWidgetFactory,
            GameStateProvider gameStateProvider,
            BattleService battleService,
            ManualRepCounterService manualRepCounterService,
            CameraRepCounterService cameraRepCounterService,
            PoseEstimationService poseEstimationService,
            PoseRepCounterService poseRepCounterService,
            TrackingConfig trackingConfig,
            MonsterNameService monsterNameService,
            ExerciseInfoService exerciseInfoService)
        {
            _uiRootService = uiRootService;
            _uiWidgetFactory = uiWidgetFactory;
            _gameStateProvider = gameStateProvider;
            _battleService = battleService;
            _manualRepCounterService = manualRepCounterService;
            _cameraRepCounterService = cameraRepCounterService;
            _poseEstimationService = poseEstimationService;
            _poseRepCounterService = poseRepCounterService;
            _trackingConfig = trackingConfig;
            _monsterNameService = monsterNameService;
            _exerciseInfoService = exerciseInfoService;
        }

        public void Init()
        {
            _screen = UIBattleScreen.Create(_uiRootService.Root, _uiWidgetFactory);
            _screen.RepClicked += OnRepClicked;
            _screen.GiveUpClicked += OnGiveUpClicked;
            _battleService.HpChanged += OnHpChanged;
            _poseEstimationService.PoseUpdated += OnPoseUpdated;
            _poseRepCounterService.FormWarning += OnFormWarning;
            _screen.gameObject.SetActive(false);
        }

        public void Show()
        {
            Refresh();
            _screen.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _screen.gameObject.SetActive(false);
        }

        private void Refresh()
        {
            var level = _battleService.Level;
            if (level == null)
            {
                return;
            }

            _screen.SetLevel(level, _monsterNameService.GetColor(level.MonsterName), _exerciseInfoService.GetRepLabel(level.ExerciseId));
            _screen.SetHp(_battleService.RemainingHp, level.Hp, _battleService.PendingDamage);
            _screen.SetReps(_battleService.RepsDone);
            _screen.SetTotal(_gameStateProvider.State.TotalReps);
            _screen.SetCameraTexture(_cameraRepCounterService.Texture);

            if (_cameraRepCounterService.Texture == null)
            {
                _screen.Skeleton.gameObject.SetActive(false);
            }
        }

        private void OnPoseUpdated(PoseData pose)
        {
            if (!_screen.gameObject.activeSelf)
            {
                return;
            }

            _screen.Skeleton.gameObject.SetActive(true);
            _screen.Skeleton.SetPose(pose, _trackingConfig.PoseScoreThreshold);
        }

        private void OnFormWarning(string message)
        {
            if (!_screen.gameObject.activeSelf)
            {
                return;
            }

            _screen.ShowFormWarning(message);
        }

        private void OnHpChanged()
        {
            if (!_screen.gameObject.activeSelf)
            {
                return;
            }

            Refresh();
        }

        private void OnRepClicked()
        {
            _manualRepCounterService.AddRep();
        }

        private void OnGiveUpClicked()
        {
            GiveUpClicked?.Invoke();
        }

        public void Dispose()
        {
            _battleService.HpChanged -= OnHpChanged;
            _poseEstimationService.PoseUpdated -= OnPoseUpdated;
            _poseRepCounterService.FormWarning -= OnFormWarning;

            if (_screen == null)
            {
                return;
            }

            _screen.RepClicked -= OnRepClicked;
            _screen.GiveUpClicked -= OnGiveUpClicked;
        }
    }
}
