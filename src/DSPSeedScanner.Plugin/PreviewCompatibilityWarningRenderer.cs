using DSPSeedScanner.Runtime;
using UnityEngine;

namespace DSPSeedScanner.Plugin
{
    internal sealed class PreviewCompatibilityWarningRenderer
    {
        private GUIStyle? textStyle;
        private Texture2D? borderTexture;

        public float Draw(
            PreviewPanelBounds bounds,
            float x,
            float y,
            float width,
            string? notice)
        {
            if (notice == null)
                return 0f;

            if (textStyle == null)
            {
                var red = new Color(1f, 0.32f, 0.32f, 1f);
                textStyle = new GUIStyle(GUI.skin.label)
                {
                    fontSize = 15,
                    fontStyle = FontStyle.Bold,
                    wordWrap = true,
                    richText = false,
                    normal = { textColor = red }
                };
                borderTexture = new Texture2D(1, 1)
                {
                    hideFlags = HideFlags.HideAndDontSave
                };
                borderTexture.SetPixel(0, 0, red);
                borderTexture.Apply();
            }

            const float thickness = 2f;
            GUI.DrawTexture(new Rect(bounds.X, bounds.Y, bounds.Width, thickness), borderTexture);
            GUI.DrawTexture(new Rect(bounds.X, bounds.Bottom - thickness, bounds.Width, thickness), borderTexture);
            GUI.DrawTexture(new Rect(bounds.X, bounds.Y, thickness, bounds.Height), borderTexture);
            GUI.DrawTexture(new Rect(bounds.Right - thickness, bounds.Y, thickness, bounds.Height), borderTexture);

            float height = textStyle.CalcHeight(new GUIContent(notice), width);
            GUI.Label(new Rect(x, y, width, height), notice, textStyle);
            return height + 8f;
        }
    }
}
