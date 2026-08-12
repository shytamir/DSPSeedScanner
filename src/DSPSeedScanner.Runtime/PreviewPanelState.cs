using System;

namespace DSPSeedScanner.Runtime
{
    public enum PreviewPanelCorner
    {
        BottomRight = 1,
        BottomLeft = 2,
        TopLeft = 3,
        TopRight = 4
    }

    public enum PreviewPanelOperationalState
    {
        Hidden,
        Waiting,
        Cached,
        Scanning,
        Complete,
        Cancelled,
        Unsupported,
        Failed
    }

    public sealed record PreviewPanelBounds
    {
        public PreviewPanelBounds(int x, int y, int width, int height)
        {
            if (x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
            if (width <= 0)
                throw new ArgumentOutOfRangeException(nameof(width));
            if (height <= 0)
                throw new ArgumentOutOfRangeException(nameof(height));
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public int X { get; }
        public int Y { get; }
        public int Width { get; }
        public int Height { get; }
        public int Right => X + Width;
        public int Bottom => Y + Height;
    }

    public sealed record PreviewPanelView
    {
        internal PreviewPanelView(
            bool visible,
            long sessionId,
            PreviewPanelCorner corner,
            PreviewPanelOperationalState state,
            string title,
            string detail,
            char? spinner)
        {
            Visible = visible;
            SessionId = sessionId;
            Corner = corner;
            State = state;
            Title = title;
            Detail = detail;
            Spinner = spinner;
        }

        public bool Visible { get; }
        public long SessionId { get; }
        public PreviewPanelCorner Corner { get; }
        public PreviewPanelOperationalState State { get; }
        public string Title { get; }
        public string Detail { get; }
        public char? Spinner { get; }

        public static PreviewPanelView Hidden { get; } = new PreviewPanelView(
            false,
            0,
            PreviewPanelCorner.BottomRight,
            PreviewPanelOperationalState.Hidden,
            String.Empty,
            String.Empty,
            null);
    }

    public static class PreviewPanelLayout
    {
        public const int DefaultCornerCode = 1;
        public const int Width = 520;
        public const int Height = 116;
        public const int Margin = 24;
        public const int MaximumTitleCharacters = 32;
        public const int MaximumDetailCharacters = 64;

        public static PreviewPanelCorner ParseCorner(int value) => value switch
        {
            1 => PreviewPanelCorner.BottomRight,
            2 => PreviewPanelCorner.BottomLeft,
            3 => PreviewPanelCorner.TopLeft,
            4 => PreviewPanelCorner.TopRight,
            _ => PreviewPanelCorner.BottomRight
        };

        public static PreviewPanelBounds Place(
            PreviewPanelCorner corner,
            int screenWidth,
            int screenHeight)
        {
            if (!Enum.IsDefined(typeof(PreviewPanelCorner), corner))
                throw new ArgumentOutOfRangeException(nameof(corner));
            if (screenWidth < Width + Margin * 2)
                throw new ArgumentOutOfRangeException(nameof(screenWidth));
            if (screenHeight < Height + Margin * 2)
                throw new ArgumentOutOfRangeException(nameof(screenHeight));

            bool right = corner == PreviewPanelCorner.BottomRight ||
                corner == PreviewPanelCorner.TopRight;
            bool bottom = corner == PreviewPanelCorner.BottomRight ||
                corner == PreviewPanelCorner.BottomLeft;
            return new PreviewPanelBounds(
                right ? screenWidth - Margin - Width : Margin,
                bottom ? screenHeight - Margin - Height : Margin,
                Width,
                Height);
        }
    }

    public static class PreviewPanelStateMapper
    {
        private static readonly char[] SpinnerFrames = { '|', '/', '-', '\\' };

        public static PreviewPanelView Waiting(
            long sessionId,
            PreviewPanelCorner corner,
            int spinnerStep)
        {
            ValidateSession(sessionId);
            return View(
                sessionId,
                corner,
                PreviewPanelOperationalState.Waiting,
                "Preparing scanner",
                "Reading the loaded cluster preview",
                Spinner(spinnerStep));
        }

        public static PreviewPanelView Project(
            long sessionId,
            PreviewResolutionState state,
            int expectedPlanets,
            int completedPlanets,
            PreviewPanelCorner corner,
            int spinnerStep)
        {
            ValidateSession(sessionId);
            if (expectedPlanets < 0)
                throw new ArgumentOutOfRangeException(nameof(expectedPlanets));
            if (completedPlanets < 0 || completedPlanets > expectedPlanets)
                throw new ArgumentOutOfRangeException(nameof(completedPlanets));

            return state switch
            {
                PreviewResolutionState.Scanning when expectedPlanets == 0 => Waiting(
                    sessionId,
                    corner,
                    spinnerStep),
                PreviewResolutionState.Scanning => View(
                    sessionId,
                    corner,
                    PreviewPanelOperationalState.Scanning,
                    "Scanning complete cluster",
                    "Planets " + completedPlanets + " / " + expectedPlanets,
                    Spinner(spinnerStep)),
                PreviewResolutionState.Cached => View(
                    sessionId,
                    corner,
                    PreviewPanelOperationalState.Cached,
                    "Complete results ready",
                    "Loaded from the local cache",
                    null),
                PreviewResolutionState.Complete => View(
                    sessionId,
                    corner,
                    PreviewPanelOperationalState.Complete,
                    "Complete results ready",
                    "Cluster scan completed",
                    null),
                PreviewResolutionState.Cancelled => View(
                    sessionId,
                    corner,
                    PreviewPanelOperationalState.Cancelled,
                    "Scan cancelled",
                    "The preview is no longer current",
                    null),
                PreviewResolutionState.Incompatible => View(
                    sessionId,
                    corner,
                    PreviewPanelOperationalState.Unsupported,
                    "Unsupported runtime",
                    "No complete result was published",
                    null),
                PreviewResolutionState.Busy => View(
                    sessionId,
                    corner,
                    PreviewPanelOperationalState.Failed,
                    "Scanner unavailable",
                    "Another runtime operation was active",
                    null),
                PreviewResolutionState.Failed => View(
                    sessionId,
                    corner,
                    PreviewPanelOperationalState.Failed,
                    "Scan failed",
                    "No complete result was published",
                    null),
                _ => throw new ArgumentOutOfRangeException(nameof(state))
            };
        }

        private static PreviewPanelView View(
            long sessionId,
            PreviewPanelCorner corner,
            PreviewPanelOperationalState state,
            string title,
            string detail,
            char? spinner)
        {
            if (!Enum.IsDefined(typeof(PreviewPanelCorner), corner))
                throw new ArgumentOutOfRangeException(nameof(corner));
            if (title.Length > PreviewPanelLayout.MaximumTitleCharacters)
                throw new InvalidOperationException("Panel title exceeds its presentation bound.");
            if (detail.Length > PreviewPanelLayout.MaximumDetailCharacters)
                throw new InvalidOperationException("Panel detail exceeds its presentation bound.");
            return new PreviewPanelView(
                true,
                sessionId,
                corner,
                state,
                title,
                detail,
                spinner);
        }

        private static char Spinner(int step)
        {
            if (step < 0)
                throw new ArgumentOutOfRangeException(nameof(step));
            return SpinnerFrames[step % SpinnerFrames.Length];
        }

        private static void ValidateSession(long sessionId)
        {
            if (sessionId <= 0)
                throw new ArgumentOutOfRangeException(nameof(sessionId));
        }
    }

    public sealed class PreviewPanelController
    {
        private long activeSessionId;

        public PreviewPanelView Current { get; private set; } = PreviewPanelView.Hidden;

        public void BeginSession(
            long sessionId,
            PreviewPanelCorner corner,
            int spinnerStep)
        {
            activeSessionId = sessionId;
            Current = PreviewPanelStateMapper.Waiting(sessionId, corner, spinnerStep);
        }

        public bool Update(
            PreviewResolutionAttempt attempt,
            PreviewPanelCorner corner,
            int spinnerStep)
        {
            if (attempt == null)
                throw new ArgumentNullException(nameof(attempt));
            if (attempt.Session.SessionId != activeSessionId || attempt.Session.IsRetired)
                return false;
            Current = PreviewPanelStateMapper.Project(
                activeSessionId,
                attempt.State,
                attempt.ExpectedPlanets,
                attempt.CompletedPlanets,
                corner,
                spinnerStep);
            return true;
        }

        public bool Hide(long sessionId)
        {
            if (sessionId != activeSessionId)
                return false;
            activeSessionId = 0;
            Current = PreviewPanelView.Hidden;
            return true;
        }

        public void HideCurrent()
        {
            activeSessionId = 0;
            Current = PreviewPanelView.Hidden;
        }
    }
}
