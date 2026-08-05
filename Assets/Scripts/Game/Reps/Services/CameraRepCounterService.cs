using System;
using UnityEngine;
using Zenject;

namespace Game.Reps.Services
{
    public class CameraRepCounterService : IRepCounter, ITickable, IDisposable
    {
        private const int SampleStep = 16;

        private readonly TrackingConfig _trackingConfig;

        private WebCamTexture _texture;
        private Color32[] _buffer;
        private float _smooth;
        private float _baseline;
        private bool _hasBaseline;
        private bool _isDown;
        private bool _pendingStart;
        private float _downTime;
        private float _lastRepTime;

        public event Action Rep;

        public bool IsRunning => _texture != null && _texture.isPlaying;
        public Texture Texture => _texture;

        public CameraRepCounterService(TrackingConfig trackingConfig)
        {
            _trackingConfig = trackingConfig;
        }

        public void RequestPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera))
            {
                UnityEngine.Android.Permission.RequestUserPermission(UnityEngine.Android.Permission.Camera);
            }
#endif
        }

        public void StartCamera()
        {
            if (!_trackingConfig.CameraEnabled)
            {
                return;
            }

            if (IsRunning)
            {
                return;
            }

            if (!HasPermission())
            {
                _pendingStart = true;
                RequestPermission();
                return;
            }

            _pendingStart = false;

            var deviceName = GetFrontDeviceName();
            if (deviceName == null)
            {
                Debug.LogError("No camera device found, falling back to manual reps");
                return;
            }

            _texture = new WebCamTexture(deviceName, _trackingConfig.CameraWidth, _trackingConfig.CameraHeight, 30);
            _texture.Play();
            _hasBaseline = false;
            _isDown = false;
        }

        public void StopCamera()
        {
            _pendingStart = false;

            if (_texture == null)
            {
                return;
            }

            _texture.Stop();
            UnityEngine.Object.Destroy(_texture);
            _texture = null;
        }

        public void Tick()
        {
            if (_pendingStart && HasPermission())
            {
                StartCamera();
            }

            if (!IsRunning)
            {
                return;
            }

            if (!_texture.didUpdateThisFrame || _texture.width <= 16)
            {
                return;
            }

            var brightness = SampleBrightness();

            if (!_hasBaseline)
            {
                _smooth = brightness;
                _baseline = brightness;
                _hasBaseline = true;
                return;
            }

            _smooth = Mathf.Lerp(_smooth, brightness, _trackingConfig.SmoothFactor);
            _baseline = Mathf.Lerp(_baseline, _smooth, _trackingConfig.BaselineFactor);

            if (_baseline <= 0.001f)
            {
                return;
            }

            var relativeDip = (_baseline - _smooth) / _baseline;

            if (!_isDown && relativeDip > _trackingConfig.DipThreshold)
            {
                _isDown = true;
                _downTime = Time.unscaledTime;
                return;
            }

            if (!_isDown || relativeDip > _trackingConfig.RiseThreshold)
            {
                return;
            }

            _isDown = false;
            var now = Time.unscaledTime;

            if (now - _downTime < _trackingConfig.MinDownSeconds)
            {
                return;
            }

            if (now - _lastRepTime < _trackingConfig.MinCycleSeconds)
            {
                return;
            }

            _lastRepTime = now;
            Rep?.Invoke();
        }

        private float SampleBrightness()
        {
            var pixels = _texture.width * _texture.height;
            if (_buffer == null || _buffer.Length != pixels)
            {
                _buffer = new Color32[pixels];
            }

            _texture.GetPixels32(_buffer);

            var sum = 0L;
            for (var i = 0; i < _buffer.Length; i += SampleStep)
            {
                var pixel = _buffer[i];
                sum += pixel.r + pixel.g + pixel.b;
            }

            var samples = (_buffer.Length + SampleStep - 1) / SampleStep;

            return sum / (float)(samples * 3 * 255);
        }

        private static string GetFrontDeviceName()
        {
            var devices = WebCamTexture.devices;
            if (devices.Length == 0)
            {
                return null;
            }

            for (var i = 0; i < devices.Length; i++)
            {
                if (devices[i].isFrontFacing)
                {
                    return devices[i].name;
                }
            }

            return devices[0].name;
        }

        private static bool HasPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return UnityEngine.Android.Permission.HasUserAuthorizedPermission(UnityEngine.Android.Permission.Camera);
#else
            return true;
#endif
        }

        public void Dispose()
        {
            StopCamera();
        }
    }
}
