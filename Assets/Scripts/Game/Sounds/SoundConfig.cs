using UnityEngine;

namespace Game.Sounds
{
    [CreateAssetMenu(fileName = "SoundConfig", menuName = "Game/SoundConfig")]
    public class SoundConfig : ScriptableObject
    {
        [Range(0f, 1f)] public float Volume = 0.8f;
        public AudioClip DownClip;
        public AudioClip UpClip;
        public AudioClip CancelClip;
    }
}
