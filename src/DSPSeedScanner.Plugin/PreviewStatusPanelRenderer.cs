using DSPSeedScanner.Runtime;
using UnityEngine;

namespace DSPSeedScanner.Plugin
{
    internal sealed class PreviewStatusPanelRenderer
    {
        private GUIStyle? boxStyle;
        private GUIStyle? titleStyle;
        private GUIStyle? detailStyle;

        public void Draw(PreviewPanelView view, int screenWidth, int screenHeight)
        {
            if (!view.Visible)
                return;

            EnsureStyles();
            PreviewPanelBounds bounds = PreviewPanelLayout.Place(
                view.Corner,
                screenWidth,
                screenHeight);
            var panel = new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            GUI.Box(panel, GUIContent.none, boxStyle);

            string title = view.Spinner.HasValue
                ? view.Spinner.Value + "  " + view.Title
                : view.Title;
            GUI.Label(
                new Rect(bounds.X + 20, bounds.Y + 16, bounds.Width - 40, 34),
                title,
                titleStyle);
            GUI.Label(
                new Rect(bounds.X + 20, bounds.Y + 58, bounds.Width - 40, 30),
                view.Detail,
                detailStyle);
        }

        private void EnsureStyles()
        {
            if (boxStyle != null)
                return;

            boxStyle = new GUIStyle(GUI.skin.box)
            {
                alignment = TextAnchor.UpperLeft,
                padding = new RectOffset(12, 12, 12, 12)
            };
            titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip,
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                wordWrap = false
            };
            detailStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip,
                fontSize = 15,
                wordWrap = false
            };
        }
    }
}
