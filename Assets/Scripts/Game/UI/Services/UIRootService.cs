using Common.UI.Services;
using UnityEngine;

namespace Game.UI.Services
{
    public class UIRootService
    {
        private readonly UIWidgetFactory _uiWidgetFactory;

        public Transform Root { get; private set; }

        public UIRootService(UIWidgetFactory uiWidgetFactory)
        {
            _uiWidgetFactory = uiWidgetFactory;
        }

        public void Init()
        {
            Root = _uiWidgetFactory.CreateRoot("UIRoot").transform;
        }
    }
}
