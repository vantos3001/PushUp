using System.Collections;
using Common.UI;
using Common.UI.Services;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UIToastView : MonoBehaviour
    {
        private const float FadeSeconds = 0.3f;

        private CanvasGroup _canvasGroup;
        private Text _text;
        private Coroutine _routine;

        public static UIToastView Create(Transform parent, UIWidgetFactory factory)
        {
            var panel = factory.CreatePanel(parent, "UIToastView", ProxyUiPalette.Panel);
            ProxyRect.SetAnchored(panel.rectTransform, new Vector2(0.5f, 1f), new Vector2(0f, -660f), new Vector2(900f, 110f));

            var view = panel.gameObject.AddComponent<UIToastView>();
            view._canvasGroup = panel.gameObject.AddComponent<CanvasGroup>();
            view._canvasGroup.blocksRaycasts = false;
            view._canvasGroup.interactable = false;

            view._text = factory.CreateText(panel.transform, "Text", "", 42, ProxyUiPalette.Gold, FontStyle.Bold);
            ProxyRect.Fill(view._text.rectTransform);

            return view;
        }

        public void Show(string message, float seconds)
        {
            _text.text = message;
            gameObject.SetActive(true);
            _canvasGroup.alpha = 1f;

            if (_routine != null)
            {
                StopCoroutine(_routine);
            }

            _routine = StartCoroutine(HideRoutine(seconds));
        }

        private IEnumerator HideRoutine(float seconds)
        {
            yield return new WaitForSeconds(seconds);

            var time = 0f;
            while (time < FadeSeconds)
            {
                time += Time.deltaTime;
                _canvasGroup.alpha = 1f - time / FadeSeconds;
                yield return null;
            }

            gameObject.SetActive(false);
            _routine = null;
        }
    }
}
