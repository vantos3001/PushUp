using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Common.UI.Services
{
    public class UIWidgetFactory
    {
        private const int CircleTextureSize = 128;

        private Font _font;
        private Sprite _circleSprite;

        public Font Font
        {
            get
            {
                if (_font == null)
                {
                    _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                }

                return _font;
            }
        }

        public Sprite CircleSprite
        {
            get
            {
                if (_circleSprite == null)
                {
                    _circleSprite = CreateCircleSprite();
                }

                return _circleSprite;
            }
        }

        public Canvas CreateRoot(string name)
        {
            var canvasObject = new GameObject(name);
            var canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            var scaler = canvasObject.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            canvasObject.AddComponent<GraphicRaycaster>();
            CreateEventSystem();

            return canvas;
        }

        public Image CreatePanel(Transform parent, string name, Color color)
        {
            var panelObject = new GameObject(name);
            panelObject.transform.SetParent(parent, false);

            var image = panelObject.AddComponent<Image>();
            image.color = color;

            return image;
        }

        public RawImage CreateRawImage(Transform parent, string name)
        {
            var imageObject = new GameObject(name);
            imageObject.transform.SetParent(parent, false);

            var rawImage = imageObject.AddComponent<RawImage>();
            rawImage.raycastTarget = false;

            return rawImage;
        }

        public Image CreateCircle(Transform parent, string name, Color color)
        {
            var image = CreatePanel(parent, name, color);
            image.sprite = CircleSprite;

            return image;
        }

        public Text CreateText(Transform parent, string name, string content, int fontSize, Color color, FontStyle fontStyle = FontStyle.Normal)
        {
            var textObject = new GameObject(name);
            textObject.transform.SetParent(parent, false);

            var text = textObject.AddComponent<Text>();
            text.font = Font;
            text.text = content;
            text.fontSize = fontSize;
            text.color = color;
            text.fontStyle = fontStyle;
            text.alignment = TextAnchor.MiddleCenter;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;

            return text;
        }

        public Button CreateButton(Transform parent, string name, string label, Color background, Color textColor, int fontSize)
        {
            var image = CreatePanel(parent, name, background);
            var button = image.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            var text = CreateText(image.transform, "Label", label, fontSize, textColor, FontStyle.Bold);
            ProxyRect.Fill(text.rectTransform);

            return button;
        }

        private void CreateEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null)
            {
                return;
            }

            var eventSystemObject = new GameObject("EventSystem");
            eventSystemObject.AddComponent<EventSystem>();
            eventSystemObject.AddComponent<InputSystemUIInputModule>();
        }

        private static Sprite CreateCircleSprite()
        {
            var texture = new Texture2D(CircleTextureSize, CircleTextureSize, TextureFormat.RGBA32, false);
            var center = (CircleTextureSize - 1) / 2f;
            var radius = CircleTextureSize / 2f - 2f;

            for (var y = 0; y < CircleTextureSize; y++)
            {
                for (var x = 0; x < CircleTextureSize; x++)
                {
                    var distance = Mathf.Sqrt((x - center) * (x - center) + (y - center) * (y - center));
                    var alpha = Mathf.Clamp01(radius - distance + 1f);
                    texture.SetPixel(x, y, new Color(1f, 1f, 1f, alpha));
                }
            }

            texture.Apply();

            var rect = new Rect(0f, 0f, CircleTextureSize, CircleTextureSize);
            return Sprite.Create(texture, rect, new Vector2(0.5f, 0.5f), 100f);
        }
    }
}
