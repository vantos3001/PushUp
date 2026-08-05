using UnityEngine;

namespace Game.Reps
{
    [CreateAssetMenu(fileName = "TrackingConfig", menuName = "Game/TrackingConfig")]
    public class TrackingConfig : ScriptableObject
    {
        public bool CameraEnabled = true;
        public int CameraWidth = 176;
        public int CameraHeight = 144;
        [Range(0.02f, 0.5f)] public float DipThreshold = 0.12f;
        [Range(0.01f, 0.3f)] public float RiseThreshold = 0.05f;
        public float MinDownSeconds = 0.1f;
        public float MinCycleSeconds = 0.45f;
        [Range(0.01f, 1f)] public float SmoothFactor = 0.3f;
        [Range(0.001f, 0.2f)] public float BaselineFactor = 0.01f;

        public bool PoseEnabled = true;
        public string PoseModelResourcePath = "Models/MoveNet";
        public int PoseInputSize = 192;
        public int PoseFrameInterval = 1;
        [Range(0f, 1f)] public float PoseScoreThreshold = 0.3f;
        [Range(0.05f, 1f)] public float PoseSmoothFactor = 0.6f;
        public float ElbowDownAngle = 100f;
        public float ElbowUpAngle = 150f;
        public float BodyLineMaxAngle = 20f;
        [Range(0.03f, 0.5f)] public float MaxWristDrift = 0.12f;
        public bool StrictForm = true;
        public float FormWarningCooldownSeconds = 3f;
    }
}
