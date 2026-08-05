namespace Game.Poses
{
    public static class ProxyPoseJoints
    {
        public const int Nose = 0;
        public const int LeftEye = 1;
        public const int RightEye = 2;
        public const int LeftEar = 3;
        public const int RightEar = 4;
        public const int LeftShoulder = 5;
        public const int RightShoulder = 6;
        public const int LeftElbow = 7;
        public const int RightElbow = 8;
        public const int LeftWrist = 9;
        public const int RightWrist = 10;
        public const int LeftHip = 11;
        public const int RightHip = 12;
        public const int LeftKnee = 13;
        public const int RightKnee = 14;
        public const int LeftAnkle = 15;
        public const int RightAnkle = 16;

        public static readonly int[][] Bones =
        {
            new[] { LeftShoulder, RightShoulder },
            new[] { LeftShoulder, LeftElbow },
            new[] { LeftElbow, LeftWrist },
            new[] { RightShoulder, RightElbow },
            new[] { RightElbow, RightWrist },
            new[] { LeftShoulder, LeftHip },
            new[] { RightShoulder, RightHip },
            new[] { LeftHip, RightHip },
            new[] { LeftHip, LeftKnee },
            new[] { LeftKnee, LeftAnkle },
            new[] { RightHip, RightKnee },
            new[] { RightKnee, RightAnkle },
            new[] { Nose, LeftShoulder },
            new[] { Nose, RightShoulder }
        };
    }
}
