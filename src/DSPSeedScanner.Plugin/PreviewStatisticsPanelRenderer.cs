using System;
using System.Linq;
using DSPSeedScanner.Runtime;
using UnityEngine;

namespace DSPSeedScanner.Plugin
{
    internal sealed class PreviewStatisticsPanelRenderer
    {
        private const float ScrollbarReserve = 18f;
        private const float SectionGap = 12f;
        private GUIStyle? titleStyle;
        private GUIStyle? sectionTitleStyle;
        private GUIStyle? bodyStyle;
        private GUIStyle? panelStyle;
        private GUIStyle? sectionStyle;
        private GUISkin? scrollSkin;
        private Vector2 scrollPosition;
        private long scrollSessionId;

        public void Draw(
            PreviewStatisticsPanelController controller,
            PreviewPanelCorner conclusionCorner,
            bool fullDocument,
            int screenWidth,
            int screenHeight)
        {
            if (controller == null)
                throw new ArgumentNullException(nameof(controller));
            PreviewStatisticsDocument? document = controller.Current;
            if (document == null)
                return;

            EnsureStyles();
            if (document.SessionId != scrollSessionId)
            {
                scrollSessionId = document.SessionId;
                scrollPosition = new Vector2(
                    (float)controller.ScrollX,
                    (float)controller.ScrollY);
            }

            double scale = PreviewPanelLayout.ScaleForScreen(
                screenWidth,
                screenHeight,
                PreviewPanelLayout.Width,
                PreviewPanelLayout.Height);
            Matrix4x4 previousMatrix = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3((float)scale, (float)scale, 1f));
            try
            {
                DrawAtScale(
                    document,
                    controller,
                    conclusionCorner,
                    fullDocument,
                    (int)Math.Floor(screenWidth / scale),
                    (int)Math.Floor(screenHeight / scale));
            }
            finally
            {
                GUI.matrix = previousMatrix;
            }
        }

        private void DrawAtScale(
            PreviewStatisticsDocument document,
            PreviewStatisticsPanelController controller,
            PreviewPanelCorner conclusionCorner,
            bool fullDocument,
            int screenWidth,
            int screenHeight)
        {
            PreviewPanelBounds bounds = PreviewPanelLayout.PlacePanelPair(
                conclusionCorner,
                screenWidth,
                screenHeight,
                fullDocument).StatisticsBounds;
            GUI.Box(
                new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height),
                GUIContent.none,
                panelStyle);

            float x = bounds.X + PreviewPanelLayout.DocumentPadding;
            float y = bounds.Y + PreviewPanelLayout.DocumentPadding;
            float viewportWidth = bounds.Width - PreviewPanelLayout.DocumentPadding * 2;
            GUI.Label(
                new Rect(x, y, viewportWidth, 30f),
                document.IdentityLine,
                titleStyle);

            float scrollY = y + 42f;
            float scrollHeight = bounds.Bottom - PreviewPanelLayout.DocumentPadding - scrollY;
            float contentWidth = viewportWidth - ScrollbarReserve;
            string[] homeLines = document.HomeSystem?.Bodies
                .Select(body => HomeSystemBodyPresentation.Format(
                    body,
                    document.HomeSystemResources))
                .ToArray() ?? Array.Empty<string>();
            float homeSectionHeight = SectionHeight(homeLines, contentWidth);
            float clusterSectionHeight = SectionHeight(Array.Empty<string>(), contentWidth);
            float contentHeight = homeSectionHeight + clusterSectionHeight + SectionGap;
            GUISkin previousSkin = GUI.skin;
            GUI.skin = scrollSkin!;
            scrollPosition = GUI.BeginScrollView(
                new Rect(x, scrollY, viewportWidth, scrollHeight),
                scrollPosition,
                new Rect(0f, 0f, contentWidth, Math.Max(scrollHeight, contentHeight)),
                false,
                contentHeight > scrollHeight);
            controller.SetScrollPosition(
                document.SessionId,
                scrollPosition.x,
                scrollPosition.y);
            try
            {
                DrawSection(
                    PreviewStatisticsDocument.HomeSystemTitle,
                    homeLines,
                    0f,
                    contentWidth,
                    homeSectionHeight);
                DrawSection(
                    PreviewStatisticsDocument.ClusterTitle,
                    Array.Empty<string>(),
                    homeSectionHeight + SectionGap,
                    contentWidth,
                    clusterSectionHeight);
            }
            finally
            {
                GUI.EndScrollView();
                GUI.skin = previousSkin;
            }
        }

        private float SectionHeight(string[] lines, float width)
        {
            float height = 46f;
            foreach (string line in lines)
            {
                height += Math.Max(
                    24f,
                    bodyStyle!.CalcHeight(
                        new GUIContent(line),
                        width - 24f)) + 4f;
            }
            return Math.Max(88f, height + 8f);
        }

        private void DrawSection(
            string title,
            string[] lines,
            float y,
            float width,
            float height)
        {
            GUI.Box(new Rect(0f, y, width, height), GUIContent.none, sectionStyle);
            GUI.Label(new Rect(0f, y + 8f, width, 30f), title, sectionTitleStyle);
            float lineY = y + 42f;
            foreach (string line in lines)
            {
                float lineHeight = Math.Max(
                    24f,
                    bodyStyle!.CalcHeight(new GUIContent(line), width - 24f));
                GUI.Label(
                    new Rect(12f, lineY, width - 24f, lineHeight),
                    line,
                    bodyStyle);
                lineY += lineHeight + 4f;
            }
        }

        private void EnsureStyles()
        {
            if (titleStyle != null)
                return;

            titleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip,
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                wordWrap = false
            };
            sectionTitleStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperCenter,
                clipping = TextClipping.Clip,
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                wordWrap = false
            };
            bodyStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.UpperLeft,
                clipping = TextClipping.Clip,
                fontSize = 16,
                fontStyle = FontStyle.Normal,
                wordWrap = true
            };
            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = MakeTexture(
                new Color(0.015f, 0.035f, 0.045f, 0.76f));
            sectionStyle = new GUIStyle(GUI.skin.box);
            sectionStyle.normal.background = MakeTexture(
                new Color(0.07f, 0.10f, 0.11f, 0.38f));
            scrollSkin = UnityEngine.Object.Instantiate(GUI.skin);
            scrollSkin.hideFlags = HideFlags.HideAndDontSave;
            scrollSkin.verticalScrollbar = new GUIStyle(GUI.skin.verticalScrollbar)
            {
                fixedWidth = 10f
            };
            scrollSkin.verticalScrollbar.normal.background = MakeTexture(
                new Color(0.07f, 0.10f, 0.11f, 0.38f));
            scrollSkin.verticalScrollbarThumb = new GUIStyle(
                GUI.skin.verticalScrollbarThumb)
            {
                fixedWidth = 8f
            };
            scrollSkin.verticalScrollbarThumb.normal.background = MakeTexture(
                new Color(0.50f, 0.62f, 0.64f, 0.68f));
        }

        private static Texture2D MakeTexture(Color color)
        {
            var texture = new Texture2D(1, 1)
            {
                hideFlags = HideFlags.HideAndDontSave
            };
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }
    }
}
