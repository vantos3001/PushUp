using System;
using Game.Poses.Services;

namespace Game.Reps.Services
{
    public class CompositeRepCounterService : IRepCounter, IDisposable
    {
        private readonly ManualRepCounterService _manualRepCounterService;
        private readonly CameraRepCounterService _cameraRepCounterService;
        private readonly PoseRepCounterService _poseRepCounterService;
        private readonly PoseEstimationService _poseEstimationService;

        public event Action Rep;

        public CompositeRepCounterService(
            ManualRepCounterService manualRepCounterService,
            CameraRepCounterService cameraRepCounterService,
            PoseRepCounterService poseRepCounterService,
            PoseEstimationService poseEstimationService)
        {
            _manualRepCounterService = manualRepCounterService;
            _cameraRepCounterService = cameraRepCounterService;
            _poseRepCounterService = poseRepCounterService;
            _poseEstimationService = poseEstimationService;
        }

        public void Init()
        {
            _manualRepCounterService.Rep += OnRep;
            _poseRepCounterService.Rep += OnRep;
            _cameraRepCounterService.Rep += OnCameraRep;
        }

        private void OnRep()
        {
            Rep?.Invoke();
        }

        private void OnCameraRep()
        {
            if (_poseEstimationService.IsAvailable)
            {
                return;
            }

            Rep?.Invoke();
        }

        public void Dispose()
        {
            _manualRepCounterService.Rep -= OnRep;
            _poseRepCounterService.Rep -= OnRep;
            _cameraRepCounterService.Rep -= OnCameraRep;
        }
    }
}
