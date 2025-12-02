using UnityEngine;
using UnityEngine.UI;

namespace LevelChecker {
    internal class UI {
        private GameObject obj;
        private Canvas canvas;

        private GameObject textObj;
        private Text text;

        internal RectTransform rect;

        internal UI() {
            obj = new GameObject(
                "Level Checker", typeof(Canvas),
                typeof(CanvasScaler),
                typeof(GraphicRaycaster)
            );
            GameObject.DontDestroyOnLoad(obj);

            canvas = obj.GetComponent<Canvas>();
            canvas.sortingOrder = 9999;
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler scaler = obj.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.Shrink;

            obj.GetComponent<GraphicRaycaster>();

            textObj = new GameObject("Text", typeof(Text), typeof(Outline));
            textObj.transform.SetParent(obj.transform);

            rect = textObj.GetComponent<RectTransform>();

            rect.anchoredPosition = Vector2.zero;
            rect.anchorMin = new Vector2(0.94f, 0f);
            rect.anchorMax = new Vector2(0.94f, 0f);
            rect.pivot     = new Vector2(0.94f, 0f);
            rect.sizeDelta = new Vector2(280f, 27f);

            text = textObj.GetComponent<Text>();
            text.fontSize = 16;
            text.alignByGeometry = true;
        }

        internal void Update() {
            if (obj == null) {
                return;
            }

            obj.SetActive(true);

            if (text.font == null && Cache.gameFont != null) {
                text.font = Cache.gameFont;
            }

            if (Cache.hash != null) {
                text.text = Cache.hash;
            }
            else {
                obj.SetActive(false);
                text.text = "Hash was null";
            }
        }
    }
}
