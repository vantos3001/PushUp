using UnityEngine;

namespace Game.Poses
{
    public class PoseData
    {
        public const int JointCount = 17;

        public Vector2[] Positions = new Vector2[JointCount];
        public float[] Scores = new float[JointCount];
    }
}
