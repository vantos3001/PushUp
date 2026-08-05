using UnityEngine;

namespace Game.Poses
{
    public static class ProxyPoseMath
    {
        public static float GetAngle(Vector2 a, Vector2 center, Vector2 b)
        {
            return Vector2.Angle(a - center, b - center);
        }
    }
}
