using DSPSeedScanner.Runtime;
using UnityEngine;

namespace DSPSeedScanner.Plugin
{
    internal sealed class PreviewStatusPanelRenderer
    {
        private GUIStyle? boxStyle;
        private GUIStyle? titleStyle;
        private GUIStyle? detailStyle;
        private GUIStyle? sectionStyle;
        private GUIStyle? contextStyle;
        private GUIStyle? cardStyle;

        public void Draw(
            PreviewPanelView view,
            PreviewConclusionPresentation? conclusions,
            int screenWidth,
            int screenHeight)
        {
            if (!view.Visible)
                return;

            EnsureStyles();
            if (conclusions != null)
            {
                DrawDocument(view, conclusions, screenWidth, screenHeight);
                return;
            }

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

        private void DrawDocument(
            PreviewPanelView view,
            PreviewConclusionPresentation conclusions,
            int screenWidth,
            int screenHeight)
        {
            PreviewPanelDocument document = PreviewConclusionPresenter.Compose(
                view,
                conclusions);
            int height = PreviewPanelLayout.DocumentPadding * 2 +
                document.Lines.Count * PreviewPanelLayout.DocumentLineHeight;
            PreviewPanelBounds bounds = PreviewPanelLayout.PlaceSized(
                view.Corner,
                screenWidth,
                screenHeight,
                PreviewPanelLayout.ConclusionWidth,
                height);
            GUI.Box(
                new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height),
                GUIContent.none,
                boxStyle);

            int y = bounds.Y + PreviewPanelLayout.DocumentPadding;
            foreach (PreviewPanelLine line in document.Lines)
            {
                GUIStyle style = StyleFor(line.Kind);
                int indent = line.Kind == PreviewPanelLineKind.Conclusion ? 22 :
                    line.Kind == PreviewPanelLineKind.Context ? 10 : 0;
                GUI.Label(
                    new Rect(
                        bounds.X + PreviewPanelLayout.DocumentPadding + indent,
                        y,
                        bounds.Width - PreviewPanelLayout.DocumentPadding * 2 - indent,
                        PreviewPanelLayout.DocumentLineHeight),
                    line.Text,
                    style);
                y += PreviewPanelLayout.DocumentLineHeight;
            }
        }

        private GUIStyle StyleFor(PreviewPanelLineKind kind) => kind switch
        {
            PreviewPanelLineKind.Identity => titleStyle!,
            PreviewPanelLineKind.Status => detailStyle!,
            PreviewPanelLineKind.Section => sectionStyle!,
            PreviewPanelLineKind.Context => contextStyle!,
            PreviewPanelLineKind.Conclusion => cardStyle!,
            _ => detailStyle!
        };

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
            sectionStyle = new GUIStyle(detailStyle)
            {
                fontSize = 15,
                fontStyle = FontStyle.Bold
            };
            contextStyle = new GUIStyle(detailStyle)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            cardStyle = new GUIStyle(detailStyle)
            {
                fontSize = 14
            };
        }
    }
}
