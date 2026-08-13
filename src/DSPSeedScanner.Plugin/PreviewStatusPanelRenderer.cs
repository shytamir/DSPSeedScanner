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
        private const float ColumnGap = 12f;
        private const float ScrollbarReserve = 18f;
        private const float CardGap = 10f;

        private GUIStyle? titleStyle;
        private GUIStyle? detailStyle;
        private GUIStyle? contextStyle;
        private GUIStyle? strengthStyle;
        private GUIStyle? preferenceStyle;
        private GUIStyle? limitationStyle;
        private GUIStyle? strengthHeaderStyle;
        private GUIStyle? preferenceHeaderStyle;
        private GUIStyle? limitationHeaderStyle;
        private GUIStyle? panelStyle;
        private GUIStyle? contextCardStyle;
        private GUISkin? scrollSkin;
        private Vector2 scrollPosition;
        private long scrollSessionId;

        public void Draw(
            PreviewPanelView view,
            PreviewConclusionPresentation? conclusions,
            int screenWidth,
            int screenHeight)
        {
            if (!view.Visible)
                return;

            EnsureStyles();
            if (view.SessionId != scrollSessionId)
            {
                scrollSessionId = view.SessionId;
                scrollPosition = Vector2.zero;
            }
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
                    conclusions?.DarkFogStatusLine,
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
            string? darkFogStatus,
            int screenWidth,
            int screenHeight)
        {
            PreviewPanelBounds bounds = PreviewPanelLayout.PlacePanelPair(
                view.Corner,
                screenWidth,
                screenHeight,
                false).ConclusionBounds;
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
            if (darkFogStatus != null)
            {
                GUI.Label(
                    new Rect(bounds.X + 20, bounds.Y + 86, bounds.Width - 40, 30),
                    darkFogStatus,
                    detailStyle);
            }
        }

        private void DrawDocument(
            PreviewPanelView view,
            PreviewConclusionPresentation conclusions,
            int screenWidth,
            int screenHeight)
        {
            ContextCard[] cards = BuildContextCards(conclusions);
            double scale = PreviewPanelLayout.ScaleForScreen(
                screenWidth,
                screenHeight,
                PreviewPanelLayout.Width,
                PreviewPanelLayout.Height);
            Matrix4x4 previousMatrix = BeginScaledDrawing(scale);
            try
            {
                DrawDocumentAtScale(
                    view,
                    conclusions,
                    cards,
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
            int screenWidth,
            int screenHeight)
        {
            PreviewPanelBounds bounds = PreviewPanelLayout.PlacePanelPair(
                view.Corner,
                screenWidth,
                screenHeight).ConclusionBounds;
            GUI.Box(
                new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height),
                GUIContent.none,
                panelStyle);
            float x = bounds.X + PreviewPanelLayout.DocumentPadding;
            float y = bounds.Y + PreviewPanelLayout.DocumentPadding;
            float viewportWidth = bounds.Width -
                PreviewPanelLayout.DocumentPadding * 2;
            float contentWidth = viewportWidth - ScrollbarReserve;
            float columnWidth = (contentWidth - ColumnGap * 2) / 3f;
            GUI.Label(
                new Rect(x, y, viewportWidth, 28),
                conclusions.IdentityLine,
                titleStyle);
            string status = view.Spinner.HasValue
                ? view.Spinner.Value + "  " + view.Title + " - " + view.Detail
                : view.Title + " - " + view.Detail;
            GUI.Label(
                new Rect(x, y + 28, viewportWidth, 24),
                status,
                detailStyle);

            float columnY = y + 62;
            if (conclusions.DarkFogStatusLine != null)
            {
                GUI.Label(
                    new Rect(x, y + 52, viewportWidth, 24),
                    conclusions.DarkFogStatusLine,
                    detailStyle);
                columnY += 24f;
            }

            foreach (PreviewConclusionColumn column in Enum.GetValues(
                typeof(PreviewConclusionColumn)))
            {
                float columnX = x + (int)column * (columnWidth + ColumnGap);
                GUIStyle style = HeaderStyle(column);
                GUI.Label(
                    new Rect(columnX, columnY, columnWidth, 30),
                    ColumnTitle(column),
                    style);
            }
            float scrollY = columnY + 36f;
            float scrollHeight = bounds.Bottom -
                PreviewPanelLayout.DocumentPadding - scrollY;
            PackedLayout layout = PackContextCards(cards, columnWidth);
            Rect viewport = new Rect(x, scrollY, viewportWidth, scrollHeight);
            Rect document = new Rect(
                0f,
                0f,
                contentWidth,
                Math.Max(scrollHeight, layout.Height));
            GUISkin previousSkin = GUI.skin;
            GUI.skin = scrollSkin!;
            scrollPosition = GUI.BeginScrollView(
                viewport,
                scrollPosition,
                document,
                false,
                layout.Height > scrollHeight);
            try
            {
                foreach (PackedContextCard card in layout.Cards)
                    DrawContextCard(card, columnWidth);
            }
            finally
            {
                GUI.EndScrollView();
                GUI.skin = previousSkin;
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

        private PackedLayout PackContextCards(
            IReadOnlyList<ContextCard> cards,
            float columnWidth)
        {
            var packed = new List<PackedContextCard>();
            float y = 0f;
            ContextCard? freshStart = cards.FirstOrDefault(value =>
                value.Context == ConclusionContext.FreshStart);
            if (freshStart != null)
            {
                float height = MeasureContextCard(freshStart, columnWidth);
                packed.Add(new PackedContextCard(freshStart, 0, 2, y, height));
                y += height + CardGap;
            }

            foreach (ContextCard card in cards.Where(value =>
                value.Context != ConclusionContext.FreshStart &&
                value.PopulatedColumnCount == 3))
            {
                float height = MeasureContextCard(card, columnWidth);
                packed.Add(new PackedContextCard(card, 0, 2, y, height));
                y += height + CardGap;
            }

            var rows = new List<PackedRow>();
            foreach (ContextCard card in cards.Where(value =>
                value.Context != ConclusionContext.FreshStart &&
                value.PopulatedColumnCount < 3))
            {
                int first = card.FirstColumn;
                int last = card.LastColumn;
                int spanMask = ((1 << (last - first + 1)) - 1) << first;
                PackedRow? row = rows.FirstOrDefault(value =>
                    (value.OccupiedMask & spanMask) == 0);
                if (row == null)
                {
                    row = new PackedRow();
                    rows.Add(row);
                }
                float height = MeasureContextCard(card, columnWidth);
                row.Add(card, first, last, height, spanMask);
            }

            foreach (PackedRow row in rows)
            {
                foreach (PackedRowCard card in row.Cards)
                {
                    packed.Add(new PackedContextCard(
                        card.Card,
                        card.FirstColumn,
                        card.LastColumn,
                        y,
                        card.Height));
                }
                y += row.Height + CardGap;
            }
            return new PackedLayout(
                packed,
                Math.Max(0f, y - CardGap));
        }

        private void DrawContextCard(
            PackedContextCard packed,
            float columnWidth)
        {
            ContextCard card = packed.Card;
            float x = packed.FirstColumn * (columnWidth + ColumnGap);
            float width = (packed.LastColumn - packed.FirstColumn + 1) *
                columnWidth + (packed.LastColumn - packed.FirstColumn) * ColumnGap;
            float y = packed.Y;
            GUI.Box(
                new Rect(x, y, width, packed.Height),
                GUIContent.none,
                contextCardStyle);
            GUI.Label(
                new Rect(x + 8f, y + 4f, width - 16f, 28f),
                card.Title,
                contextStyle);
            float contentY = y + 34f;
            foreach (PreviewConclusionColumn column in Enum.GetValues(
                typeof(PreviewConclusionColumn)))
            {
                if (!card.HasColumn(column))
                    continue;
                float columnX = (int)column * (columnWidth + ColumnGap);
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
                clipping = TextClipping.Clip,
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
            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.background = MakeTexture(
                new Color(0.015f, 0.035f, 0.045f, 0.76f));
            contextCardStyle = new GUIStyle(GUI.skin.box);
            contextCardStyle.normal.background = MakeTexture(
                new Color(0.07f, 0.10f, 0.11f, 0.38f));
            scrollSkin = UnityEngine.Object.Instantiate(GUI.skin);
            scrollSkin.hideFlags = HideFlags.HideAndDontSave;
            scrollSkin.verticalScrollbar = new GUIStyle(GUI.skin.verticalScrollbar)
            {
                fixedWidth = 10f
            };
            scrollSkin.verticalScrollbar.normal.background = MakeTexture(
                new Color(0.07f, 0.10f, 0.11f, 0.38f));
            scrollSkin.verticalScrollbar.hover.background =
                scrollSkin.verticalScrollbar.normal.background;
            scrollSkin.verticalScrollbar.active.background =
                scrollSkin.verticalScrollbar.normal.background;
            scrollSkin.verticalScrollbarThumb = new GUIStyle(
                GUI.skin.verticalScrollbarThumb)
            {
                fixedWidth = 8f
            };
            scrollSkin.verticalScrollbarThumb.normal.background = MakeTexture(
                new Color(0.50f, 0.62f, 0.64f, 0.68f));
            scrollSkin.verticalScrollbarThumb.hover.background = MakeTexture(
                new Color(0.62f, 0.76f, 0.78f, 0.82f));
            scrollSkin.verticalScrollbarThumb.active.background =
                scrollSkin.verticalScrollbarThumb.hover.background;
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
            public int FirstColumn => cards.Keys.Min(value => (int)value);
            public int LastColumn => cards.Keys.Max(value => (int)value);

            public bool HasColumn(PreviewConclusionColumn column) =>
                cards.ContainsKey(column);

            public IReadOnlyList<PresentedConclusionCard> Cards(
                PreviewConclusionColumn column) =>
                cards.TryGetValue(column, out IReadOnlyList<PresentedConclusionCard>? values)
                    ? values
                    : Array.Empty<PresentedConclusionCard>();
        }

        private sealed class PackedRow
        {
            private readonly List<PackedRowCard> cards = new List<PackedRowCard>();

            public IReadOnlyList<PackedRowCard> Cards => cards;
            public int OccupiedMask { get; private set; }
            public float Height { get; private set; }

            public void Add(
                ContextCard card,
                int firstColumn,
                int lastColumn,
                float height,
                int spanMask)
            {
                cards.Add(new PackedRowCard(
                    card,
                    firstColumn,
                    lastColumn,
                    height));
                OccupiedMask |= spanMask;
                Height = Math.Max(Height, height);
            }
        }

        private sealed class PackedRowCard
        {
            public PackedRowCard(
                ContextCard card,
                int firstColumn,
                int lastColumn,
                float height)
            {
                Card = card;
                FirstColumn = firstColumn;
                LastColumn = lastColumn;
                Height = height;
            }

            public ContextCard Card { get; }
            public int FirstColumn { get; }
            public int LastColumn { get; }
            public float Height { get; }
        }

        private sealed class PackedContextCard
        {
            public PackedContextCard(
                ContextCard card,
                int firstColumn,
                int lastColumn,
                float y,
                float height)
            {
                Card = card;
                FirstColumn = firstColumn;
                LastColumn = lastColumn;
                Y = y;
                Height = height;
            }

            public ContextCard Card { get; }
            public int FirstColumn { get; }
            public int LastColumn { get; }
            public float Y { get; }
            public float Height { get; }
        }

        private sealed class PackedLayout
        {
            public PackedLayout(
                IReadOnlyList<PackedContextCard> cards,
                float height)
            {
                Cards = cards;
                Height = height;
            }

            public IReadOnlyList<PackedContextCard> Cards { get; }
            public float Height { get; }
        }
    }
}
