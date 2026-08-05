using System;
using Common.UI;
using Common.UI.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIMenuScreen : MonoBehaviour
    {
        private Text _goldText;
        private Text _levelText;
        private Text _statsText;
        private Button _fightButton;
        private Button _trainingButton;
        private Button _statsButton;

        public Action FightClicked;
        public Action TrainingClicked;
        public Action StatsClicked;

        public static UIMenuScreen Create(Transform parent, UIWidgetFactory factory)
        {
            var panel = factory.CreatePanel(parent, "UIMenuScreen", ProxyUiPalette.Background);
            ProxyRect.Fill(panel.rectTransform);

            var screen = panel.gameObject.AddComponent<UIMenuScreen>();
            screen.Build(factory);

            return screen;
        }

        public void SetContent(string levelText, string statsText, string goldText)
        {
            _levelText.text = levelText;
            _statsText.text = statsText;
            _goldText.text = goldText;
        }

        private void Build(UIWidgetFactory factory)
        {
            var title = factory.CreateText(transform, "Title", "PUSHUP ARENA", 96, ProxyUiPalette.Gold, FontStyle.Bold);
            ProxyRect.SetAnchored(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -300f), new Vector2(1000f, 120f));

            var subtitle = factory.CreateText(transform, "Subtitle", "Фитнес-данж", 44, ProxyUiPalette.LightGray);
            ProxyRect.SetAnchored(subtitle.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -410f), new Vector2(1000f, 60f));

            _goldText = factory.CreateText(transform, "GoldText", "", 44, ProxyUiPalette.Gold, FontStyle.Bold);
            ProxyRect.SetAnchored(_goldText.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -510f), new Vector2(1000f, 60f));

            _levelText = factory.CreateText(transform, "LevelText", "", 56, ProxyUiPalette.White, FontStyle.Bold);
            ProxyRect.SetAnchored(_levelText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 160f), new Vector2(1000f, 80f));

            _statsText = factory.CreateText(transform, "StatsText", "", 40, ProxyUiPalette.LightGray);
            ProxyRect.SetAnchored(_statsText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 10f), new Vector2(1000f, 140f));

            _fightButton = factory.CreateButton(transform, "FightButton", "В БОЙ · ДАНЖ ОТЖИМАНИЙ", ProxyUiPalette.Green, ProxyUiPalette.White, 52);
            ProxyRect.SetAnchored((RectTransform)_fightButton.transform, new Vector2(0.5f, 0f), new Vector2(0f, 560f), new Vector2(820f, 160f));
            _fightButton.onClick.AddListener(OnFightClicked);

            _trainingButton = factory.CreateButton(transform, "TrainingButton", "ПРОСТО ОТЖИМАТЬСЯ", ProxyUiPalette.Gold, ProxyUiPalette.DarkText, 46);
            ProxyRect.SetAnchored((RectTransform)_trainingButton.transform, new Vector2(0.5f, 0f), new Vector2(0f, 380f), new Vector2(820f, 140f));
            _trainingButton.onClick.AddListener(OnTrainingClicked);

            _statsButton = factory.CreateButton(transform, "StatsButton", "СТАТИСТИКА", ProxyUiPalette.Gray, ProxyUiPalette.White, 44);
            ProxyRect.SetAnchored((RectTransform)_statsButton.transform, new Vector2(0.5f, 0f), new Vector2(0f, 220f), new Vector2(600f, 120f));
            _statsButton.onClick.AddListener(OnStatsClicked);
        }

        private void OnFightClicked()
        {
            FightClicked?.Invoke();
        }

        private void OnTrainingClicked()
        {
            TrainingClicked?.Invoke();
        }

        private void OnStatsClicked()
        {
            StatsClicked?.Invoke();
        }

        private void OnDestroy()
        {
            if (_fightButton != null)
            {
                _fightButton.onClick.RemoveListener(OnFightClicked);
            }

            if (_trainingButton != null)
            {
                _trainingButton.onClick.RemoveListener(OnTrainingClicked);
            }

            if (_statsButton != null)
            {
                _statsButton.onClick.RemoveListener(OnStatsClicked);
            }
        }
    }
}
