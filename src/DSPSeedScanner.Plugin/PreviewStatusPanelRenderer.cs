using System;
using System.Collections.Generic;
using System.Linq;
using DSPSeedScanner.Core;
using DSPSeedScanner.Runtime;
using UnityEngine;

namespace DSPSeedScanner.Plugin
{
    internal sealed class PreviewStatusPanelRenderer
    {
        private GUIStyle? titleStyle;
        private GUIStyle? detailStyle;
        private GUIStyle? contextStyle;
        private GUIStyle? strengthStyle;
        private GUIStyle? preferenceStyle;
        private GUIStyle? limitationStyle;
        private GUIStyle? strengthHeaderStyle;
        private GUIStyle? preferenceHeaderStyle;
        private GUIStyle? limitationHeaderStyle;

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
            ContextCard[] cards = BuildContextCards(conclusions);
            float columnWidth = (PreviewPanelLayout.ConclusionWidth -
                PreviewPanelLayout.DocumentPadding * 2 - 24) / 3f;
            int contentHeight = (int)Math.Ceiling(cards.Sum(card =>
                MeasureContextCard(card, columnWidth)));
            int height = PreviewPanelLayout.DocumentPadding * 2 + 98 + contentHeight;
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
                    cards,
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
            IReadOnlyList<ContextCard> cards,
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

            float columnY = y + 62;
            foreach (PreviewConclusionColumn column in Enum.GetValues(
                typeof(PreviewConclusionColumn)))
            {
                float columnX = x + (int)column * (columnWidth + 12);
                GUIStyle style = HeaderStyle(column);
                GUI.Label(
                    new Rect(columnX, columnY, columnWidth, 30),
                    ColumnTitle(column),
                    style);
            }
            columnY += 36;
            foreach (ContextCard card in cards)
            {
                float cardHeight = MeasureContextCard(card, columnWidth);
                DrawContextCard(card, x, columnY, columnWidth, cardHeight);
                columnY += cardHeight;
            }
        }

        private static ContextCard[] BuildContextCards(
            PreviewConclusionPresentation presentation)
        {
            return presentation.ImmediateGroups
                .Concat(presentation.DetailGroups)
                .GroupBy(group => group.Context)
                .Select(group => new ContextCard(
                    group.Key,
                    group.First().Title,
                    group.SelectMany(value => value.Cards)
                        .GroupBy(card => card.Column)
                        .ToDictionary(
                            cards => cards.Key,
                            cards => (IReadOnlyList<PresentedConclusionCard>)cards
                                .GroupBy(card => card.Line, StringComparer.Ordinal)
                                .Select(values => values.First())
                                .ToArray())))
                .OrderByDescending(card => card.PopulatedColumnCount == 3)
                .ThenByDescending(card => card.PopulatedColumnCount)
                .ThenBy(card => card.Context)
                .ToArray();
        }

        private float MeasureContextCard(ContextCard card, float columnWidth)
        {
            float maximum = 0f;
            foreach (PreviewConclusionColumn column in Enum.GetValues(
                typeof(PreviewConclusionColumn)))
            {
                float columnHeight = 0f;
                GUIStyle style = CardStyle(column);
                foreach (PresentedConclusionCard conclusion in card.Cards(column))
                {
                    columnHeight += Math.Max(24f, style.CalcHeight(
                        new GUIContent(conclusion.Line),
                        columnWidth)) + 7f;
                }
                maximum = Math.Max(maximum, columnHeight);
            }
            return 36f + maximum + 14f;
        }

        private void DrawContextCard(
            ContextCard card,
            float x,
            float y,
            float columnWidth,
            float height)
        {
            GUI.Label(
                new Rect(x, y, PreviewPanelLayout.ConclusionWidth -
                    PreviewPanelLayout.DocumentPadding * 2, 28),
                card.Title,
                contextStyle);
            float contentY = y + 34f;
            foreach (PreviewConclusionColumn column in Enum.GetValues(
                typeof(PreviewConclusionColumn)))
            {
                float columnX = x + (int)column * (columnWidth + 12);
                float itemY = contentY;
                GUIStyle style = CardStyle(column);
                foreach (PresentedConclusionCard conclusion in card.Cards(column))
                {
                    float itemHeight = Math.Max(24f, style.CalcHeight(
                        new GUIContent(conclusion.Line),
                        columnWidth));
                    GUI.Label(
                        new Rect(columnX, itemY, columnWidth, itemHeight),
                        conclusion.Line,
                        style);
                    itemY += itemHeight + 7f;
                }
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

        private GUIStyle HeaderStyle(PreviewConclusionColumn column) => column switch
        {
            PreviewConclusionColumn.Strength => strengthHeaderStyle!,
            PreviewConclusionColumn.PreferenceSensitive => preferenceHeaderStyle!,
            PreviewConclusionColumn.Limitation => limitationHeaderStyle!,
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
            detailStyle = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleLeft,
                clipping = TextClipping.Clip,
                fontSize = 17,
                wordWrap = false
            };
            contextStyle = new GUIStyle(detailStyle)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 17,
                fontStyle = FontStyle.Bold,
                wordWrap = false
            };
            strengthStyle = new GUIStyle(detailStyle)
            {
                fontSize = 15,
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
            strengthHeaderStyle = new GUIStyle(strengthStyle)
            {
                alignment = TextAnchor.MiddleCenter,
                fontSize = 18,
                fontStyle = FontStyle.Bold,
                wordWrap = false
            };
            preferenceHeaderStyle = new GUIStyle(strengthHeaderStyle)
            {
                normal = { textColor = preferenceStyle.normal.textColor }
            };
            limitationHeaderStyle = new GUIStyle(strengthHeaderStyle)
            {
                normal = { textColor = limitationStyle.normal.textColor }
            };
        }

        private sealed class ContextCard
        {
            private readonly IReadOnlyDictionary<PreviewConclusionColumn,
                IReadOnlyList<PresentedConclusionCard>> cards;

            public ContextCard(
                ConclusionContext context,
                string title,
                IReadOnlyDictionary<PreviewConclusionColumn,
                    IReadOnlyList<PresentedConclusionCard>> cards)
            {
                Context = context;
                Title = title;
                this.cards = cards;
            }

            public ConclusionContext Context { get; }
            public string Title { get; }
            public int PopulatedColumnCount => cards.Count;

            public IReadOnlyList<PresentedConclusionCard> Cards(
                PreviewConclusionColumn column) =>
                cards.TryGetValue(column, out IReadOnlyList<PresentedConclusionCard>? values)
                    ? values
                    : Array.Empty<PresentedConclusionCard>();
        }
    }
}
