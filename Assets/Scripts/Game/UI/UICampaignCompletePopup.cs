using System;
using Common.UI;
using Common.UI.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UICampaignCompletePopup : MonoBehaviour
    {
        private Text _contentText;
        private Button _continueButton;

        public Action ContinueClicked;

        public static UICampaignCompletePopup Create(Transform parent, UIWidgetFactory factory)
        {
            var overlay = factory.CreatePanel(parent, "UICampaignCompletePopup", ProxyUiPalette.Overlay);
            ProxyRect.Fill(overlay.rectTransform);

            var popup = overlay.gameObject.AddComponent<UICampaignCompletePopup>();
            popup.Build(factory);

            return popup;
        }

        public void SetContent(int prestige, int totalLevels)
        {
            _contentText.text = $"Круг {prestige} завершён — {totalLevels} уровней позади!\nДанж начинается заново,\nмонстры уже ждут.";
        }

        private void Build(UIWidgetFactory factory)
        {
            var panel = factory.CreatePanel(transform, "Panel", ProxyUiPalette.Panel);
            ProxyRect.SetAnchored(panel.rectTransform, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(880f, 680f));

            var title = factory.CreateText(panel.transform, "Title", "КРУГ ПРОЙДЕН!", 80, ProxyUiPalette.Gold, FontStyle.Bold);
            ProxyRect.SetAnchored(title.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -110f), new Vector2(820f, 100f));

            _contentText = factory.CreateText(panel.transform, "Content", "", 46, ProxyUiPalette.White);
            ProxyRect.SetAnchored(_contentText.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0f, 10f), new Vector2(820f, 240f));

            _continueButton = factory.CreateButton(panel.transform, "ContinueButton", "В МЕНЮ", ProxyUiPalette.Green, ProxyUiPalette.White, 52);
            ProxyRect.SetAnchored((RectTransform)_continueButton.transform, new Vector2(0.5f, 0f), new Vector2(0f, 60f), new Vector2(560f, 140f));
            _continueButton.onClick.AddListener(OnContinueClicked);
        }

        private void OnContinueClicked()
        {
            ContinueClicked?.Invoke();
        }

        private void OnDestroy()
        {
            if (_continueButton == null)
            {
                return;
            }

            _continueButton.onClick.RemoveListener(OnContinueClicked);
        }
    }
}
