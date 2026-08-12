using System;
using System.Collections.Generic;
using System.Linq;
using DSPSeedScanner.Runtime;
using UnityEngine;

namespace DSPSeedScanner.Plugin
{
    internal sealed class PreviewStatusPanelRenderer
    {
        private GUIStyle? boxStyle;
        private GUIStyle? titleStyle;
        private GUIStyle? detailStyle;
        private GUIStyle? contextStyle;
        private GUIStyle? strengthStyle;
        private GUIStyle? preferenceStyle;
        private GUIStyle? limitationStyle;

        public void Draw(
            PreviewPanelView view,
            PreviewConclusionPresentation? conclusions,
            int screenWidth,
            int screenHeight)
        {
            if (!view.Visible)
                return;

            EnsureStyles();
            if (conclusions != null && conclusions.ImmediateGroups
                .Concat(conclusions.DetailGroups)
                .SelectMany(group => group.Cards)
                .Any())
            {
                DrawDocument(view, conclusions, screenWidth, screenHeight);
                return;
            }

            double scale = PreviewPanelLayout.ScaleForScreen(
                screenWidth,
                screenHeight,
                PreviewPanelLayout.Width,
                PreviewPanelLayout.Height);
            Matrix4x4 previousMatrix = BeginScaledDrawing(scale);
            try
            {
                DrawStatus(
                    view,
                    conclusions?.IdentityLine,
                    Logical(screenWidth, scale),
                    Logical(screenHeight, scale));
            }
            finally
            {
                GUI.matrix = previousMatrix;
            }
        }

        private void DrawStatus(
            PreviewPanelView view,
            string? identity,
            int screenWidth,
            int screenHeight)
        {
            PreviewPanelBounds bounds = PreviewPanelLayout.Place(
                view.Corner,
                screenWidth,
                screenHeight);
            var panel = new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height);
            GUI.Box(panel, GUIContent.none, boxStyle);

            string title = identity ?? (view.Spinner.HasValue
                ? view.Spinner.Value + "  " + view.Title
                : view.Title);
            string detail = identity == null
                ? view.Detail
                : (view.Spinner.HasValue ? view.Spinner.Value + "  " : String.Empty) +
                    view.Title + " - " + view.Detail;
            GUI.Label(
                new Rect(bounds.X + 20, bounds.Y + 16, bounds.Width - 40, 34),
                title,
                titleStyle);
            GUI.Label(
                new Rect(bounds.X + 20, bounds.Y + 58, bounds.Width - 40, 30),
                detail,
                detailStyle);
        }

        private void DrawDocument(
            PreviewPanelView view,
            PreviewConclusionPresentation conclusions,
            int screenWidth,
            int screenHeight)
        {
            ColumnContent[] columns =
            {
                BuildColumn(conclusions, PreviewConclusionColumn.Strength),
                BuildColumn(conclusions, PreviewConclusionColumn.PreferenceSensitive),
                BuildColumn(conclusions, PreviewConclusionColumn.Limitation)
            };
            float columnWidth = (PreviewPanelLayout.ConclusionWidth -
                PreviewPanelLayout.DocumentPadding * 2 - 24) / 3f;
            int contentHeight = (int)Math.Ceiling(columns.Max(column =>
                MeasureColumn(column, columnWidth)));
            int height = PreviewPanelLayout.DocumentPadding * 2 + 62 + contentHeight;
            double scale = PreviewPanelLayout.ScaleForScreen(
                screenWidth,
                screenHeight,
                PreviewPanelLayout.ConclusionWidth,
                height);
            Matrix4x4 previousMatrix = BeginScaledDrawing(scale);
            try
            {
                DrawDocumentAtScale(
                    view,
                    conclusions,
                    columns,
                    columnWidth,
                    height,
                    Logical(screenWidth, scale),
                    Logical(screenHeight, scale));
            }
            finally
            {
                GUI.matrix = previousMatrix;
            }
        }

        private void DrawDocumentAtScale(
            PreviewPanelView view,
            PreviewConclusionPresentation conclusions,
            IReadOnlyList<ColumnContent> columns,
            float columnWidth,
            int height,
            int screenWidth,
            int screenHeight)
        {
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

            float x = bounds.X + PreviewPanelLayout.DocumentPadding;
            float y = bounds.Y + PreviewPanelLayout.DocumentPadding;
            GUI.Label(
                new Rect(x, y, bounds.Width - PreviewPanelLayout.DocumentPadding * 2, 28),
                conclusions.IdentityLine,
                titleStyle);
            string status = view.Spinner.HasValue
                ? view.Spinner.Value + "  " + view.Title + " - " + view.Detail
                : view.Title + " - " + view.Detail;
            GUI.Label(
                new Rect(x, y + 28, bounds.Width - PreviewPanelLayout.DocumentPadding * 2, 24),
                status,
                detailStyle);

            float columnX = x;
            float columnY = y + 62;
            for (int index = 0; index < columns.Count; index++)
            {
                DrawColumn(columns[index], columnX, columnY, columnWidth);
                columnX += columnWidth + 12;
            }
        }

        private ColumnContent BuildColumn(
            PreviewConclusionPresentation presentation,
            PreviewConclusionColumn column)
        {
            var groups = presentation.ImmediateGroups
                .Concat(presentation.DetailGroups)
                .GroupBy(group => group.Context)
                .Select(group => new ColumnGroup(
                    group.First().Title,
                    group.SelectMany(value => value.Cards)
                        .Where(card => card.Column == column)
                        .GroupBy(card => card.Line, StringComparer.Ordinal)
                        .Select(cards => cards.First())
                        .ToArray()))
                .Where(group => group.Cards.Count != 0)
                .ToArray();
            return new ColumnContent(ColumnTitle(column), column, groups);
        }

        private float MeasureColumn(ColumnContent column, float width)
        {
            float height = 30f;
            GUIStyle cardStyle = CardStyle(column.Column);
            foreach (ColumnGroup group in column.Groups)
            {
                height += 25f;
                foreach (PresentedConclusionCard card in group.Cards)
                    height += Math.Max(22f, cardStyle.CalcHeight(
                        new GUIContent(card.Line),
                        width)) + 5f;
                height += 5f;
            }
            return height;
        }

        private void DrawColumn(
            ColumnContent column,
            float x,
            float y,
            float width)
        {
            GUIStyle cardStyle = CardStyle(column.Column);
            GUI.Label(new Rect(x, y, width, 26), column.Title, cardStyle);
            y += 30f;
            foreach (ColumnGroup group in column.Groups)
            {
                GUI.Label(new Rect(x, y, width, 22), group.Title, contextStyle);
                y += 25f;
                foreach (PresentedConclusionCard card in group.Cards)
                {
                    float cardHeight = Math.Max(22f, cardStyle.CalcHeight(
                        new GUIContent(card.Line),
                        width));
                    GUI.Label(new Rect(x, y, width, cardHeight), card.Line, cardStyle);
                    y += cardHeight + 5f;
                }
                y += 5f;
            }
        }

        private static Matrix4x4 BeginScaledDrawing(double scale)
        {
            Matrix4x4 previous = GUI.matrix;
            GUI.matrix = Matrix4x4.Scale(new Vector3(
                (float)scale,
                (float)scale,
                1f));
            return previous;
        }

        private static int Logical(int physical, double scale) =>
            (int)Math.Floor(physical / scale);

        private GUIStyle CardStyle(PreviewConclusionColumn column) => column switch
        {
            PreviewConclusionColumn.Strength => strengthStyle!,
            PreviewConclusionColumn.PreferenceSensitive => preferenceStyle!,
            PreviewConclusionColumn.Limitation => limitationStyle!,
            _ => throw new ArgumentOutOfRangeException(nameof(column))
        };

        private static string ColumnTitle(PreviewConclusionColumn column) => column switch
        {
            PreviewConclusionColumn.Strength => "Strengths",
            PreviewConclusionColumn.PreferenceSensitive => "Preference-sensitive",
            PreviewConclusionColumn.Limitation => "Limitations",
            _ => throw new ArgumentOutOfRangeException(nameof(column))
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
            contextStyle = new GUIStyle(detailStyle)
            {
                fontSize = 14,
                fontStyle = FontStyle.Bold
            };
            strengthStyle = new GUIStyle(detailStyle)
            {
                fontSize = 13,
                fontStyle = FontStyle.Normal,
                wordWrap = true,
                clipping = TextClipping.Overflow,
                normal = { textColor = new Color(0.50f, 0.95f, 0.58f) }
            };
            preferenceStyle = new GUIStyle(strengthStyle)
            {
                normal = { textColor = new Color(1.00f, 0.86f, 0.38f) }
            };
            limitationStyle = new GUIStyle(strengthStyle)
            {
                normal = { textColor = new Color(1.00f, 0.48f, 0.43f) }
            };
        }

        private sealed class ColumnContent
        {
            public ColumnContent(
                string title,
                PreviewConclusionColumn column,
                IEnumerable<ColumnGroup> groups)
            {
                Title = title;
                Column = column;
                Groups = groups.ToArray();
            }

            public string Title { get; }
            public PreviewConclusionColumn Column { get; }
            public IReadOnlyList<ColumnGroup> Groups { get; }
        }

        private sealed class ColumnGroup
        {
            public ColumnGroup(string title, IEnumerable<PresentedConclusionCard> cards)
            {
                Title = title;
                Cards = cards.ToArray();
            }

            public string Title { get; }
            public IReadOnlyList<PresentedConclusionCard> Cards { get; }
        }
    }
}
