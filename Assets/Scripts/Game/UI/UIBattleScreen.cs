using System;
using System.Collections;
using Common.UI;
using Common.UI.Services;
using Game.Levels;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIBattleScreen : MonoBehaviour
    {
        private Sprite _fallbackIconSprite;
        private RawImage _cameraImage;
        private Image _cameraDim;
        private Image _monsterIcon;
        private Text _monsterLetter;
        private Text _levelLabel;
        private Text _monsterName;
        private RectTransform _hpPendingFill;
        private RectTransform _hpFill;
        private Text _hpText;
        private Text _repsText;
        private Text _totalText;
        private Text _formText;
        private Button _repButton;
        private Text _repButtonLabel;
        private Button _giveUpButton;
        private Coroutine _formRoutine;

        public Action RepClicked;
        public Action GiveUpClicked;

        public UISkeletonView Skeleton { get; private set; }

        public static UIBattleScreen Create(Transform parent, UIWidgetFactory factory)
        {
            var panel = factory.CreatePanel(parent, "UIBattleScreen", ProxyUiPalette.Background);
            ProxyRect.Fill(panel.rectTransform);

            var screen = panel.gameObject.AddComponent<UIBattleScreen>();
            screen.Build(factory);

            return screen;
        }

        public void SetLevel(LevelData level, Color fallbackColor, string repLabel)
        {
            if (level.MonsterIcon != null)
            {
                _monsterIcon.sprite = level.MonsterIcon;
                _monsterIcon.color = Color.white;
                _monsterLetter.gameObject.SetActive(false);
            }
            else
            {
                _monsterIcon.sprite = _fallbackIconSprite;
                _monsterIcon.color = fallbackColor;
                _monsterLetter.gameObject.SetActive(true);
                _monsterLetter.text = GetMonsterLetter(level.MonsterName);
            }

            var bossTag = level.IsBoss ? " · БОСС" : "";
            _levelLabel.text = $"УРОВЕНЬ {level.Number}{bossTag}";
            _monsterName.text = level.MonsterName;
            _monsterName.color = level.IsBoss ? ProxyUiPalette.Gold : ProxyUiPalette.White;
            _repButtonLabel.text = repLabel;
        }

        public void SetCameraTexture(Texture texture)
        {
            if (texture == null)
            {
                _cameraImage.gameObject.SetActive(false);
                _cameraDim.gameObject.SetActive(false);
                return;
            }

            _cameraImage.texture = texture;
            FitCameraImage(texture);
            _cameraImage.gameObject.SetActive(true);
            _cameraDim.gameObject.SetActive(true);
        }

        private void FitCameraImage(Texture texture)
        {
            var webcam = texture as WebCamTexture;
            var angle = webcam != null ? webcam.videoRotationAngle : 0;
            var mirrored = webcam != null && webcam.videoVerticallyMirrored;
            var rect = _cameraImage.rectTransform;

            rect.localEulerAngles = new Vector3(0f, 0f, -angle);
            _cameraImage.uvRect = mirrored ? new Rect(0f, 1f, 1f, -1f) : new Rect(0f, 0f, 1f, 1f);

            var parentSize = ((RectTransform)rect.parent).rect.size;
            var rotated = Mathf.Abs(angle) % 180 != 0;
            var containerWidth = rotated ? parentSize.y : parentSize.x;
            var containerHeight = rotated ? parentSize.x : parentSize.y;
            var textureAspect = texture.height > 0 ? (float)texture.width / texture.height : 1f;

            var width = containerWidth;
            var height = width / textureAspect;
            if (height < containerHeight)
            {
                height = containerHeight;
                width = height * textureAspect;
            }

            rect.sizeDelta = new Vector2(width, height);

            var skeletonRect = (RectTransform)Skeleton.transform;
            skeletonRect.sizeDelta = rotated ? new Vector2(height, width) : new Vector2(width, height);
        }

        public void ShowFormWarning(string message)
        {
            _formText.text = message;
            _formText.gameObject.SetActive(true);

            if (_formRoutine != null)
            {
                StopCoroutine(_formRoutine);
            }

            _formRoutine = StartCoroutine(HideFormRoutine());
        }

        public void SetHp(int remainingHp, int maxHp, int pendingDamage)
        {
            var shownHp = Mathf.Max(0, remainingHp - pendingDamage);
            SetFill(_hpPendingFill, remainingHp, maxHp);
            SetFill(_hpFill, shownHp, maxHp);
            _hpText.text = $"{shownHp} / {maxHp} HP";
        }

        private static void SetFill(RectTransform fill, int value, int maxValue)
        {
            var share = maxValue > 0 ? value / (float)maxValue : 0f;
            fill.anchorMax = new Vector2(Mathf.Clamp01(share), 1f);
            fill.offsetMin = new Vector2(6f, 6f);
            fill.offsetMax = new Vector2(-6f, -6f);
        }

        public void SetReps(int reps)
        {
            _repsText.text = reps.ToString();
        }

        public void SetTotal(int totalReps)
        {
            _totalText.text = $"Всего отжиманий: {totalReps}";
        }

        private void Build(UIWidgetFactory factory)
        {
            _cameraImage = factory.CreateRawImage(transform, "CameraImage");
            ProxyRect.SetAnchored(_cameraImage.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero);
            _cameraImage.gameObject.SetActive(false);

            _cameraDim = factory.CreatePanel(transform, "CameraDim", new Color(0f, 0f, 0f, 0.45f));
            ProxyRect.Fill(_cameraDim.rectTransform);
            _cameraDim.raycastTarget = false;
            _cameraDim.gameObject.SetActive(false);

            Skeleton = UISkeletonView.Create(transform, factory);
            Skeleton.gameObject.SetActive(false);

            var plate = factory.CreatePanel(transform, "MonsterPlate", ProxyUiPalette.Panel);
            ProxyRect.SetAnchored(plate.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -60f), new Vector2(920f, 380f));

            _monsterIcon = factory.CreateCircle(plate.transform, "MonsterIcon", ProxyUiPalette.Gray);
            ProxyRect.SetAnchored(_monsterIcon.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -20f), new Vector2(150f, 150f));
            _fallbackIconSprite = _monsterIcon.sprite;

            _monsterLetter = factory.CreateText(_monsterIcon.transform, "MonsterLetter", "", 72, ProxyUiPalette.White, FontStyle.Bold);
            ProxyRect.Fill(_monsterLetter.rectTransform);

            _levelLabel = factory.CreateText(plate.transform, "LevelLabel", "", 40, ProxyUiPalette.Red, FontStyle.Bold);
            ProxyRect.SetAnchored(_levelLabel.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -200f), new Vector2(880f, 50f));

            _monsterName = factory.CreateText(plate.transform, "MonsterName", "", 60, ProxyUiPalette.White, FontStyle.Bold);
            ProxyRect.SetAnchored(_monsterName.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -270f), new Vector2(880f, 70f));

            var hpBar = factory.CreatePanel(transform, "HpBar", ProxyUiPalette.BarBackground);
            ProxyRect.SetAnchored(hpBar.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -470f), new Vector2(920f, 80f));

            var hpPendingImage = factory.CreatePanel(hpBar.transform, "HpPendingFill", ProxyUiPalette.PendingRed);
            _hpPendingFill = hpPendingImage.rectTransform;
            _hpPendingFill.anchorMin = Vector2.zero;
            _hpPendingFill.anchorMax = Vector2.one;

            var hpFillImage = factory.CreatePanel(hpBar.transform, "HpFill", ProxyUiPalette.Red);
            _hpFill = hpFillImage.rectTransform;
            _hpFill.anchorMin = Vector2.zero;
            _hpFill.anchorMax = Vector2.one;

            _hpText = factory.CreateText(hpBar.transform, "HpText", "", 44, ProxyUiPalette.White, FontStyle.Bold);
            ProxyRect.Fill(_hpText.rectTransform);

            _totalText = factory.CreateText(transform, "TotalText", "", 36, ProxyUiPalette.LightGray);
            ProxyRect.SetAnchored(_totalText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -590f), new Vector2(900f, 50f));

            _formText = factory.CreateText(transform, "FormText", "", 48, ProxyUiPalette.Gold, FontStyle.Bold);
            ProxyRect.SetAnchored(_formText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 260f), new Vector2(900f, 60f));
            _formText.gameObject.SetActive(false);

            var medal = factory.CreateCircle(transform, "Medal", ProxyUiPalette.Gold);
            ProxyRect.SetAnchored(medal.rectTransform, new Vector2(0.5f, 0f), new Vector2(0f, 700f), new Vector2(220f, 220f));

            _repsText = factory.CreateText(medal.transform, "RepsText", "0", 96, ProxyUiPalette.DarkText, FontStyle.Bold);
            ProxyRect.Fill(_repsText.rectTransform);

            _repButton = factory.CreateButton(transform, "RepButton", "+1 ОТЖИМАНИЕ", ProxyUiPalette.Green, ProxyUiPalette.White, 56);
            ProxyRect.SetAnchored((RectTransform)_repButton.transform, new Vector2(0.5f, 0f), new Vector2(0f, 320f), new Vector2(760f, 170f));
            _repButtonLabel = _repButton.GetComponentInChildren<Text>();
            _repButton.onClick.AddListener(OnRepClicked);

            _giveUpButton = factory.CreateButton(transform, "GiveUpButton", "СДАТЬСЯ", ProxyUiPalette.Gray, ProxyUiPalette.White, 40);
            ProxyRect.SetAnchored((RectTransform)_giveUpButton.transform, new Vector2(0.5f, 0f), new Vector2(0f, 140f), new Vector2(500f, 110f));
            _giveUpButton.onClick.AddListener(OnGiveUpClicked);
        }

        private static string GetMonsterLetter(string monsterName)
        {
            if (string.IsNullOrEmpty(monsterName))
            {
                return "?";
            }

            var lastSpace = monsterName.LastIndexOf(' ');
            var baseStart = lastSpace + 1;

            return monsterName.Substring(baseStart, 1).ToUpperInvariant();
        }

        private IEnumerator HideFormRoutine()
        {
            yield return new WaitForSeconds(2f);
            _formText.gameObject.SetActive(false);
            _formRoutine = null;
        }

        private void OnRepClicked()
        {
            RepClicked?.Invoke();
        }

        private void OnGiveUpClicked()
        {
            GiveUpClicked?.Invoke();
        }

        private void OnDestroy()
        {
            if (_repButton != null)
            {
                _repButton.onClick.RemoveListener(OnRepClicked);
            }

            if (_giveUpButton != null)
            {
                _giveUpButton.onClick.RemoveListener(OnGiveUpClicked);
            }
        }
    }
}
