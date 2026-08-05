using UnityEngine;

namespace Game.Sounds
{
    public static class ProxySoundSynth
    {
        private const int SampleRate = 44100;

        public static AudioClip CreateTone(float fromFrequency, float toFrequency, float seconds)
        {
            var sampleCount = Mathf.Max(1, Mathf.RoundToInt(SampleRate * seconds));
            var samples = new float[sampleCount];
            var phase = 0f;

            for (var i = 0; i < sampleCount; i++)
            {
                var progress = i / (float)sampleCount;
                var frequency = Mathf.Lerp(fromFrequency, toFrequency, progress);
                phase += 2f * Mathf.PI * frequency / SampleRate;
                var envelope = Mathf.Sin(progress * Mathf.PI);
                samples[i] = Mathf.Sin(phase) * envelope * 0.9f;
            }

            var clip = AudioClip.Create($"tone_{(int)fromFrequency}_{(int)toFrequency}", sampleCount, 1, SampleRate, false);
            clip.SetData(samples, 0);

            return clip;
        }
    }
}
