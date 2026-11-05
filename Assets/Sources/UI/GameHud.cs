using System;
using UnityEngine;
using UnityEngine.UI;

namespace ShrinkShot.UI
{
    /// <summary>Win / lose screen with a "play again" button, built from code. Input goes through the EventSystem in the scene.</summary>
    public class GameHud : MonoBehaviour
    {
        private static readonly Color PanelColor = new Color(0.08f, 0.12f, 0.08f, 0.78f);
        private static readonly Color AccentColor = new Color(1f, 0.62f, 0.2f);
        private static readonly Color LoseColor = new Color(1f, 0.4f, 0.35f);

        private Font _font;
        private GameObject _resultPanel;
        private Text _resultTitle;
        private Text _resultReason;

        public event Action PlayAgainClicked;

        public static GameHud Create()
        {
            var root = new GameObject("HUD", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 10;

            var scaler = root.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080f, 1920f);
            scaler.matchWidthOrHeight = 0.5f;

            var hud = root.AddComponent<GameHud>();
            hud.Build();
            return hud;
        }

        public void ShowResult(bool won, string reason)
        {
            _resultPanel.SetActive(true);
            _resultTitle.text = won ? "LEVEL COMPLETE!" : "YOU LOST";
            _resultTitle.color = won ? AccentColor : LoseColor;
            _resultReason.text = reason;
        }

        public void HideResult() => _resultPanel.SetActive(false);

        private void Build()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            Image panel = CreateImage((RectTransform)transform, "Result", PanelColor);
            panel.raycastTarget = true;
            _resultPanel = panel.gameObject;
            Place(panel.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            _resultTitle = CreateText(panel.rectTransform, string.Empty, 96, AccentColor);
            _resultTitle.fontStyle = FontStyle.Bold;
            Place(_resultTitle.rectTransform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(40f, 160f), new Vector2(-40f, 320f));

            _resultReason = CreateText(panel.rectTransform, string.Empty, 48, Color.white);
            Place(_resultReason.rectTransform, new Vector2(0f, 0.5f), new Vector2(1f, 0.5f), new Vector2(80f, -20f), new Vector2(-80f, 150f));

            Image button = CreateImage(panel.rectTransform, "PlayAgain", AccentColor);
            button.raycastTarget = true;
            button.gameObject.AddComponent<Button>().onClick.AddListener(() => PlayAgainClicked?.Invoke());
            Place(button.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(-260f, -230f), new Vector2(260f, -90f));

            Text label = CreateText(button.rectTransform, "PLAY AGAIN", 56, Color.white);
            label.fontStyle = FontStyle.Bold;
            Place(label.rectTransform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);

            _resultPanel.SetActive(false);
        }

        private Text CreateText(RectTransform parent, string text, int size, Color color)
        {
            var go = new GameObject("Text", typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var label = go.AddComponent<Text>();
            label.font = _font;
            label.text = text;
            label.fontSize = size;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = color;
            label.raycastTarget = false;
            label.horizontalOverflow = HorizontalWrapMode.Wrap;
            label.verticalOverflow = VerticalWrapMode.Overflow;
            return label;
        }

        private static Image CreateImage(RectTransform parent, string name, Color color)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            var image = go.AddComponent<Image>();
            image.color = color;
            image.raycastTarget = false;
            return image;
        }

        private static void Place(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }
    }
}
