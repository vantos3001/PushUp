using System;
using Common.UI.Services;
using Game.Levels.Services;

namespace Game.UI.Services
{
    public class UICampaignCompleteService : IDisposable
    {
        private readonly UIRootService _uiRootService;
        private readonly UIWidgetFactory _uiWidgetFactory;
        private readonly LevelGenerationService _levelGenerationService;

        private UICampaignCompletePopup _popup;

        public event Action ContinueClicked;

        public UICampaignCompleteService(
            UIRootService uiRootService,
            UIWidgetFactory uiWidgetFactory,
            LevelGenerationService levelGenerationService)
        {
            _uiRootService = uiRootService;
            _uiWidgetFactory = uiWidgetFactory;
            _levelGenerationService = levelGenerationService;
        }

        public void Init()
        {
            _popup = UICampaignCompletePopup.Create(_uiRootService.Root, _uiWidgetFactory);
            _popup.ContinueClicked += OnContinueClicked;
            _popup.gameObject.SetActive(false);
        }

        public void Show(int prestige)
        {
            _popup.SetContent(prestige, _levelGenerationService.TotalLevels);
            _popup.gameObject.SetActive(true);
        }

        public void Hide()
        {
            _popup.gameObject.SetActive(false);
        }

        private void OnContinueClicked()
        {
            ContinueClicked?.Invoke();
        }

        public void Dispose()
        {
            if (_popup == null)
            {
                return;
            }

            _popup.ContinueClicked -= OnContinueClicked;
        }
    }
}
