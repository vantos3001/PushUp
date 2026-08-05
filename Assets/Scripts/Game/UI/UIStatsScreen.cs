using System;
using Common.UI;
using Common.UI.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIStatsScreen : MonoBehaviour
    {
        private Text _summaryText;
        private Text _daysText;
        private Button _backButton;

        public Action BackClicked;

        public static UIStatsScreen Create(Transform parent, UIWidgetFactory factory)
        {
            var panel = factory.CreatePanel(parent, "UIStatsScreen", ProxyUiPalette.Background);
            ProxyRect.Fill(panel.rectTransform);

            var screen = panel.gameObject.AddComponent<UIStatsScreen>();
            screen.Build(factory);

            return screen;
        }

        public void SetContent(string summary, string days)
        {
            _summaryText.text = summary;
            _daysText.text = days;
        }

        private void Build(UIWidgetFactory factory)
        {
            var title = factory.CreateText(transform, "Title", "СТАТИСТИКА", 80, ProxyUiPalette.Gold, FontStyle.Bold);
            ProxyRect.SetAnchored(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -200f), new Vector2(1000f, 100f));

            var summaryPanel = factory.CreatePanel(transform, "SummaryPanel", ProxyUiPalette.Panel);
            ProxyRect.SetAnchored(summaryPanel.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -330f), new Vector2(920f, 360f));

            _summaryText = factory.CreateText(summaryPanel.transform, "SummaryText", "", 42, ProxyUiPalette.White);
            ProxyRect.Fill(_summaryText.rectTransform);

            var daysPanel = factory.CreatePanel(transform, "DaysPanel", ProxyUiPalette.Panel);
            ProxyRect.SetAnchored(daysPanel.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -740f), new Vector2(920f, 560f));

            _daysText = factory.CreateText(daysPanel.transform, "DaysText", "", 40, ProxyUiPalette.LightGray);
            ProxyRect.Fill(_daysText.rectTransform);

            _backButton = factory.CreateButton(transform, "BackButton", "НАЗАД", ProxyUiPalette.Gray, ProxyUiPalette.White, 48);
            ProxyRect.SetAnchored((RectTransform)_backButton.transform, new Vector2(0.5f, 0f), new Vector2(0f, 160f), new Vector2(560f, 130f));
            _backButton.onClick.AddListener(OnBackClicked);
        }

        private void OnBackClicked()
        {
            BackClicked?.Invoke();
        }

        private void OnDestroy()
        {
            if (_backButton == null)
            {
                return;
            }

            _backButton.onClick.RemoveListener(OnBackClicked);
        }
    }
}
