using System;
using Game.Reps;
using Game.Reps.Services;
using Unity.InferenceEngine;
using UnityEngine;
using Zenject;

namespace Game.Poses.Services
{
    public class PoseEstimationService : ITickable, IDisposable
    {
        private readonly TrackingConfig _trackingConfig;
        private readonly CameraRepCounterService _cameraRepCounterService;
        private readonly PoseData _poseData = new PoseData();

        private Model _sourceModel;
        private Worker _worker;
        private Tensor<float> _inputTensor;
        private bool _loadTried;
        private bool _hasPose;
        private int _builtRotation = -1;
        private bool _builtMirrored;
        private int _frameCounter;

        public event Action<PoseData> PoseUpdated;

        public bool IsAvailable => _worker != null;

        public PoseEstimationService(TrackingConfig trackingConfig, CameraRepCounterService cameraRepCounterService)
        {
            _trackingConfig = trackingConfig;
            _cameraRepCounterService = cameraRepCounterService;
        }

        public void Tick()
        {
            if (!_trackingConfig.PoseEnabled)
            {
                return;
            }

            var texture = _cameraRepCounterService.Texture;
            if (texture == null)
            {
                _hasPose = false;
                return;
            }

            TryLoadSource();

            if (_sourceModel == null)
            {
                return;
            }

            var webcam = texture as WebCamTexture;
            var rotation = webcam != null ? NormalizeRotation(webcam.videoRotationAngle) : 0;
            var mirrored = webcam != null && webcam.videoVerticallyMirrored;
            EnsureWorker(rotation, mirrored);

            if (_worker == null)
            {
                return;
            }

            _frameCounter += 1;
            if (_frameCounter < _trackingConfig.PoseFrameInterval)
            {
                return;
            }

            _frameCounter = 0;
            Estimate(texture);
        }

        private void TryLoadSource()
        {
            if (_loadTried)
            {
                return;
            }

            _loadTried = true;

            var modelAsset = Resources.Load<ModelAsset>(_trackingConfig.PoseModelResourcePath);
            if (modelAsset == null)
            {
                Debug.Log($"Pose model not found at Resources/{_trackingConfig.PoseModelResourcePath}, pose tracking disabled");
                return;
            }

            try
            {
                _sourceModel = ModelLoader.Load(modelAsset);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to load pose model: {exception.Message}");
                _sourceModel = null;
            }
        }

        private void EnsureWorker(int rotation, bool mirrored)
        {
            if (_worker != null && rotation == _builtRotation && mirrored == _builtMirrored)
            {
                return;
            }

            DisposeWorker();

            try
            {
                var model = BuildModel(rotation, mirrored);
                _worker = new Worker(model, BackendType.GPUCompute);

                var size = _trackingConfig.PoseInputSize;
                _inputTensor = new Tensor<float>(new TensorShape(1, size, size, 3));
                _builtRotation = rotation;
                _builtMirrored = mirrored;
                _hasPose = false;
            }
            catch (Exception exception)
            {
                Debug.LogError($"Failed to init pose model: {exception.Message}");
                DisposeWorker();
                _sourceModel = null;
            }
        }

        private Model BuildModel(int rotation, bool mirrored)
        {
            var size = _trackingConfig.PoseInputSize;
            var graph = new FunctionalGraph();
            var input = graph.AddInput(DataType.Float, new DynamicTensorShape(1, size, size, 3));
            var frame = input;

            if (mirrored)
            {
                frame = Functional.Flip(frame, new[] { 1 });
            }

            if (rotation == 90)
            {
                frame = Functional.Flip(Functional.Permute(frame, new[] { 0, 2, 1, 3 }), new[] { 2 });
            }
            else if (rotation == 180)
            {
                frame = Functional.Flip(frame, new[] { 1, 2 });
            }
            else if (rotation == 270)
            {
                frame = Functional.Flip(Functional.Permute(frame, new[] { 0, 2, 1, 3 }), new[] { 1 });
            }

            if (NeedsIntInput())
            {
                frame = Functional.Int(Functional.Round(frame * 255f));
            }

            var outputs = Functional.Forward(_sourceModel, frame);

            return graph.Compile(outputs[0]);
        }

        private bool NeedsIntInput()
        {
            return _sourceModel.inputs.Count > 0 && _sourceModel.inputs[0].dataType == DataType.Int;
        }

        private void Estimate(Texture texture)
        {
            try
            {
                TextureConverter.ToTensor(texture, _inputTensor, new TextureTransform().SetTensorLayout(TensorLayout.NHWC));
                _worker.Schedule(_inputTensor);

                var output = _worker.PeekOutput() as Tensor<float>;
                if (output == null)
                {
                    return;
                }

                var values = output.DownloadToArray();
                if (values.Length < PoseData.JointCount * 3)
                {
                    return;
                }

                ApplySmoothed(values);
                PoseUpdated?.Invoke(_poseData);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Pose estimation failed, disabling pose tracking: {exception.Message}");
                DisposeWorker();
                _sourceModel = null;
            }
        }

        private void ApplySmoothed(float[] values)
        {
            var factor = _trackingConfig.PoseSmoothFactor;

            for (var i = 0; i < PoseData.JointCount; i++)
            {
                var position = new Vector2(values[i * 3 + 1], 1f - values[i * 3]);
                var score = values[i * 3 + 2];

                if (_hasPose)
                {
                    position = Vector2.Lerp(_poseData.Positions[i], position, factor);
                    score = Mathf.Lerp(_poseData.Scores[i], score, factor);
                }

                _poseData.Positions[i] = position;
                _poseData.Scores[i] = score;
            }

            _hasPose = true;
        }

        private static int NormalizeRotation(int angle)
        {
            return ((angle % 360) + 360) % 360;
        }

        private void DisposeWorker()
        {
            if (_inputTensor != null)
            {
                _inputTensor.Dispose();
                _inputTensor = null;
            }

            if (_worker != null)
            {
                _worker.Dispose();
                _worker = null;
            }

            _builtRotation = -1;
        }

        public void Dispose()
        {
            DisposeWorker();
        }
    }
}
