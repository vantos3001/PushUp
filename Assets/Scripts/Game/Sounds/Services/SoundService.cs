using System;
using Game.Poses.Services;
using Game.Reps;
using UnityEngine;

namespace Game.Sounds.Services
{
    public class SoundService : IDisposable
    {
        private readonly SoundConfig _soundConfig;
        private readonly IRepCounter _repCounter;
        private readonly PoseRepCounterService _poseRepCounterService;

        private AudioSource _audioSource;
        private AudioClip _downClip;
        private AudioClip _upClip;
        private AudioClip _cancelClip;

        public SoundService(
            SoundConfig soundConfig,
            IRepCounter repCounter,
            PoseRepCounterService poseRepCounterService)
        {
            _soundConfig = soundConfig;
            _repCounter = repCounter;
            _poseRepCounterService = poseRepCounterService;
        }

        public void Init()
        {
            var audioObject = new GameObject("SoundService");
            _audioSource = audioObject.AddComponent<AudioSource>();
            _audioSource.playOnAwake = false;

            _downClip = _soundConfig.DownClip != null ? _soundConfig.DownClip : ProxySoundSynth.CreateTone(520f, 440f, 0.09f);
            _upClip = _soundConfig.UpClip != null ? _soundConfig.UpClip : ProxySoundSynth.CreateTone(660f, 990f, 0.16f);
            _cancelClip = _soundConfig.CancelClip != null ? _soundConfig.CancelClip : ProxySoundSynth.CreateTone(330f, 180f, 0.25f);

            _repCounter.Rep += OnRep;
            _poseRepCounterService.DepthReached += OnDepthReached;
            _poseRepCounterService.CycleCanceled += OnCycleCanceled;
        }

        public void PlayDownSound()
        {
            Play(_downClip);
        }

        public void PlayRepSound()
        {
            Play(_upClip);
        }

        public void PlayCancelSound()
        {
            Play(_cancelClip);
        }

        private void OnDepthReached()
        {
            PlayDownSound();
        }

        private void OnRep()
        {
            PlayRepSound();
        }

        private void OnCycleCanceled()
        {
            PlayCancelSound();
        }

        private void Play(AudioClip clip)
        {
            if (_audioSource == null || clip == null)
            {
                return;
            }

            _audioSource.PlayOneShot(clip, _soundConfig.Volume);
        }

        public void Dispose()
        {
            _repCounter.Rep -= OnRep;
            _poseRepCounterService.DepthReached -= OnDepthReached;
            _poseRepCounterService.CycleCanceled -= OnCycleCanceled;
        }
    }
}
