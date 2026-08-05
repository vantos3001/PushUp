using Common.UI;
using Common.UI.Services;
using Game.Poses;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UISkeletonView : MonoBehaviour
    {
        private const float JointSize = 26f;
        private const float BoneWidth = 10f;

        private const float VisibilityHysteresis = 0.05f;
        private const float FrameEdgeMargin = 0.02f;

        private RectTransform _rect;
        private Image[] _joints;
        private RectTransform[] _bones;
        private bool[] _jointVisible;

        public static UISkeletonView Create(Transform parent, UIWidgetFactory factory)
        {
            var containerObject = new GameObject("UISkeletonView");
            containerObject.transform.SetParent(parent, false);

            var view = containerObject.AddComponent<UISkeletonView>();
            view._rect = containerObject.AddComponent<RectTransform>();
            ProxyRect.SetAnchored(view._rect, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            view.Build(factory);

            return view;
        }

        public void SetPose(PoseData pose, float scoreThreshold)
        {
            var size = _rect.rect.size;

            for (var i = 0; i < _joints.Length; i++)
            {
                var threshold = _jointVisible[i]
                    ? scoreThreshold - VisibilityHysteresis
                    : scoreThreshold + VisibilityHysteresis;
                _jointVisible[i] = pose.Scores[i] >= threshold && IsInsideFrame(pose.Positions[i]);
                _joints[i].gameObject.SetActive(_jointVisible[i]);

                if (!_jointVisible[i])
                {
                    continue;
                }

                _joints[i].rectTransform.anchoredPosition = ToLocal(pose.Positions[i], size);
            }

            for (var i = 0; i < _bones.Length; i++)
            {
                var bone = ProxyPoseJoints.Bones[i];
                var visible = _jointVisible[bone[0]] && _jointVisible[bone[1]];
                _bones[i].gameObject.SetActive(visible);

                if (!visible)
                {
                    continue;
                }

                var start = ToLocal(pose.Positions[bone[0]], size);
                var end = ToLocal(pose.Positions[bone[1]], size);
                var delta = end - start;
                _bones[i].anchoredPosition = start;
                _bones[i].sizeDelta = new Vector2(delta.magnitude, BoneWidth);
                _bones[i].localRotation = Quaternion.Euler(0f, 0f, Mathf.Atan2(delta.y, delta.x) * Mathf.Rad2Deg);
            }
        }

        private void Build(UIWidgetFactory factory)
        {
            var bonePairs = ProxyPoseJoints.Bones;
            _bones = new RectTransform[bonePairs.Length];

            for (var i = 0; i < bonePairs.Length; i++)
            {
                var bone = factory.CreatePanel(transform, $"Bone{i}", ProxyUiPalette.White);
                bone.raycastTarget = false;
                _bones[i] = bone.rectTransform;
                _bones[i].pivot = new Vector2(0f, 0.5f);
                _bones[i].gameObject.SetActive(false);
            }

            _joints = new Image[PoseData.JointCount];
            _jointVisible = new bool[PoseData.JointCount];

            for (var i = 0; i < PoseData.JointCount; i++)
            {
                var joint = factory.CreateCircle(transform, $"Joint{i}", ProxyUiPalette.Gold);
                joint.raycastTarget = false;
                joint.rectTransform.sizeDelta = new Vector2(JointSize, JointSize);
                joint.gameObject.SetActive(false);
                _joints[i] = joint;
            }
        }

        private static bool IsInsideFrame(Vector2 position)
        {
            return position.x > FrameEdgeMargin && position.x < 1f - FrameEdgeMargin
                && position.y > FrameEdgeMargin && position.y < 1f - FrameEdgeMargin;
        }

        private static Vector2 ToLocal(Vector2 normalized, Vector2 size)
        {
            return new Vector2((normalized.x - 0.5f) * size.x, (normalized.y - 0.5f) * size.y);
        }
    }
}
