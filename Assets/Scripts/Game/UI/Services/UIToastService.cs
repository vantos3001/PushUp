using Common.UI.Services;

namespace Game.UI.Services
{
    public class UIToastService
    {
        private const float DurationSeconds = 2.5f;

        private readonly UIRootService _uiRootService;
        private readonly UIWidgetFactory _uiWidgetFactory;

        private UIToastView _view;

        public UIToastService(UIRootService uiRootService, UIWidgetFactory uiWidgetFactory)
        {
            _uiRootService = uiRootService;
            _uiWidgetFactory = uiWidgetFactory;
        }

        public void Init()
        {
            _view = UIToastView.Create(_uiRootService.Root, _uiWidgetFactory);
            _view.gameObject.SetActive(false);
        }

        public void Show(string message)
        {
            _view.Show(message, DurationSeconds);
        }
    }
}
