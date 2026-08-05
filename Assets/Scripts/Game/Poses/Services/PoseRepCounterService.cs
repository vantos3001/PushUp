using System;
using Game.Reps;
using UnityEngine;

namespace Game.Poses.Services
{
    public class PoseRepCounterService : IRepCounter, IDisposable
    {
        private const float FrameEdgeMargin = 0.02f;
        private const float PlankMaxHeightDelta = 0.25f;

        private readonly TrackingConfig _trackingConfig;
        private readonly PoseEstimationService _poseEstimationService;

        private bool _isDown;
        private bool _bodyLineOk;
        private bool _depthReached;
        private bool _hasWristAnchor;
        private bool _handsMoved;
        private Vector2 _wristAnchor;
        private float _lowestAngle;
        private float _downTime;
        private float _lastRepTime;
        private float _lastWarningTime;

        public event Action Rep;
        public event Action DepthReached;
        public event Action CycleCanceled;
        public event Action<string> FormWarning;

        public PoseRepCounterService(TrackingConfig trackingConfig, PoseEstimationService poseEstimationService)
        {
            _trackingConfig = trackingConfig;
            _poseEstimationService = poseEstimationService;
        }

        public void Init()
        {
            _poseEstimationService.PoseUpdated += OnPoseUpdated;
        }

        private void OnPoseUpdated(PoseData pose)
        {
            var elbowAngle = GetElbowAngle(pose);
            if (elbowAngle < 0f)
            {
                return;
            }

            if (!_isDown)
            {
                if (elbowAngle >= _trackingConfig.ElbowUpAngle)
                {
                    return;
                }

                _isDown = true;
                _bodyLineOk = true;
                _depthReached = false;
                _handsMoved = false;
                _hasWristAnchor = TryGetWristCenter(pose, out _wristAnchor);
                _lowestAngle = elbowAngle;
                _downTime = Time.unscaledTime;
                CheckBodyLine(pose);
                CheckDepth();
                return;
            }

            _lowestAngle = Mathf.Min(_lowestAngle, elbowAngle);
            CheckBodyLine(pose);
            CheckDepth();
            CheckWristAnchor(pose);

            if (elbowAngle < _trackingConfig.ElbowUpAngle)
            {
                return;
            }

            _isDown = false;
            CompleteCycle();
        }

        private void CheckDepth()
        {
            if (_depthReached)
            {
                return;
            }

            if (_lowestAngle > _trackingConfig.ElbowDownAngle)
            {
                return;
            }

            _depthReached = true;
            DepthReached?.Invoke();
        }

        private void CompleteCycle()
        {
            var now = Time.unscaledTime;

            if (now - _downTime < _trackingConfig.MinDownSeconds)
            {
                Warn("Слишком быстро!");
                Cancel();
                return;
            }

            if (now - _lastRepTime < _trackingConfig.MinCycleSeconds)
            {
                Warn("Слишком быстро!");
                Cancel();
                return;
            }

            if (!_depthReached)
            {
                Warn("Опускайся ниже!");
                if (_trackingConfig.StrictForm)
                {
                    Cancel();
                    return;
                }
            }

            if (!_bodyLineOk)
            {
                Warn("Держи корпус ровно!");
                if (_trackingConfig.StrictForm)
                {
                    Cancel();
                    return;
                }
            }

            if (_handsMoved)
            {
                Warn("Руки не отрывай от пола!");
                if (_trackingConfig.StrictForm)
                {
                    Cancel();
                    return;
                }
            }

            _lastRepTime = now;
            _depthReached = false;
            Rep?.Invoke();
        }

        private void Cancel()
        {
            if (!_depthReached)
            {
                return;
            }

            _depthReached = false;
            CycleCanceled?.Invoke();
        }

        private void CheckWristAnchor(PoseData pose)
        {
            if (_handsMoved)
            {
                return;
            }

            if (!TryGetWristCenter(pose, out var wristCenter))
            {
                return;
            }

            if (!_hasWristAnchor)
            {
                _hasWristAnchor = true;
                _wristAnchor = wristCenter;
                return;
            }

            if (Vector2.Distance(_wristAnchor, wristCenter) <= _trackingConfig.MaxWristDrift)
            {
                return;
            }

            _handsMoved = true;
        }

        private bool TryGetWristCenter(PoseData pose, out Vector2 center)
        {
            var leftConfident = IsConfident(pose, ProxyPoseJoints.LeftWrist);
            var rightConfident = IsConfident(pose, ProxyPoseJoints.RightWrist);

            if (leftConfident && rightConfident)
            {
                center = (pose.Positions[ProxyPoseJoints.LeftWrist] + pose.Positions[ProxyPoseJoints.RightWrist]) * 0.5f;
                return true;
            }

            if (leftConfident)
            {
                center = pose.Positions[ProxyPoseJoints.LeftWrist];
                return true;
            }

            if (rightConfident)
            {
                center = pose.Positions[ProxyPoseJoints.RightWrist];
                return true;
            }

            center = Vector2.zero;
            return false;
        }

        private void CheckBodyLine(PoseData pose)
        {
            var deviation = GetBodyLineDeviation(pose);
            if (deviation < 0f)
            {
                return;
            }

            if (deviation <= _trackingConfig.BodyLineMaxAngle)
            {
                return;
            }

            _bodyLineOk = false;
        }

        private float GetBodyLineDeviation(PoseData pose)
        {
            var left = GetSideBodyLineDeviation(pose, ProxyPoseJoints.LeftShoulder, ProxyPoseJoints.LeftHip, ProxyPoseJoints.LeftAnkle);
            var right = GetSideBodyLineDeviation(pose, ProxyPoseJoints.RightShoulder, ProxyPoseJoints.RightHip, ProxyPoseJoints.RightAnkle);

            if (left < 0f)
            {
                return right;
            }

            if (right < 0f)
            {
                return left;
            }

            return Mathf.Min(left, right);
        }

        private float GetSideBodyLineDeviation(PoseData pose, int shoulder, int hip, int ankle)
        {
            if (!HasScores(pose, shoulder, hip, ankle))
            {
                return -1f;
            }

            var isHorizontal = Mathf.Abs(pose.Positions[shoulder].y - pose.Positions[hip].y) <= PlankMaxHeightDelta;
            if (!isHorizontal)
            {
                return -1f;
            }

            var angle = ProxyPoseMath.GetAngle(pose.Positions[shoulder], pose.Positions[hip], pose.Positions[ankle]);

            return 180f - angle;
        }

        private float GetElbowAngle(PoseData pose)
        {
            var left = GetSideElbowAngle(pose, ProxyPoseJoints.LeftShoulder, ProxyPoseJoints.LeftElbow, ProxyPoseJoints.LeftWrist);
            var right = GetSideElbowAngle(pose, ProxyPoseJoints.RightShoulder, ProxyPoseJoints.RightElbow, ProxyPoseJoints.RightWrist);

            if (left < 0f)
            {
                return right;
            }

            if (right < 0f)
            {
                return left;
            }

            return (left + right) * 0.5f;
        }

        private float GetSideElbowAngle(PoseData pose, int shoulder, int elbow, int wrist)
        {
            if (!HasScores(pose, shoulder, elbow, wrist))
            {
                return -1f;
            }

            return ProxyPoseMath.GetAngle(pose.Positions[shoulder], pose.Positions[elbow], pose.Positions[wrist]);
        }

        private bool HasScores(PoseData pose, int first, int second, int third)
        {
            return IsConfident(pose, first) && IsConfident(pose, second) && IsConfident(pose, third);
        }

        private bool IsConfident(PoseData pose, int joint)
        {
            if (pose.Scores[joint] < _trackingConfig.PoseScoreThreshold)
            {
                return false;
            }

            var position = pose.Positions[joint];

            return position.x > FrameEdgeMargin && position.x < 1f - FrameEdgeMargin
                && position.y > FrameEdgeMargin && position.y < 1f - FrameEdgeMargin;
        }

        private void Warn(string message)
        {
            var now = Time.unscaledTime;
            if (now - _lastWarningTime < _trackingConfig.FormWarningCooldownSeconds)
            {
                return;
            }

            _lastWarningTime = now;
            FormWarning?.Invoke(message);
        }

        public void Dispose()
        {
            _poseEstimationService.PoseUpdated -= OnPoseUpdated;
        }
    }
}
