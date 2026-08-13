using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public enum HomeSystemBodyOrbitKind
    {
        Primary,
        Satellite
    }

    public enum HomeSystemBodyKind
    {
        Solid,
        GasGiant,
        IceGiant
    }

    public sealed record RuntimeHomeSystemBodyEvidence
    {
        public RuntimeHomeSystemBodyEvidence(
            int bodyId,
            string displayDesignation,
            int planetNumber,
            int orbitAround,
            int? resolvedParentBodyId,
            int stableGameOrder,
            HomeSystemBodyKind bodyKind = HomeSystemBodyKind.Solid,
            string? themeName = null,
            decimal? solarRatio = null,
            decimal? windRatio = null)
        {
            if (bodyId <= 0)
                throw new ArgumentOutOfRangeException(nameof(bodyId));
            if (String.IsNullOrWhiteSpace(displayDesignation))
                throw new ArgumentException(
                    "Body display designation is required.",
                    nameof(displayDesignation));
            if (planetNumber <= 0)
                throw new ArgumentOutOfRangeException(nameof(planetNumber));
            if (orbitAround < 0)
                throw new ArgumentOutOfRangeException(nameof(orbitAround));
            if (stableGameOrder < 0)
                throw new ArgumentOutOfRangeException(nameof(stableGameOrder));
            if (!Enum.IsDefined(typeof(HomeSystemBodyKind), bodyKind))
                throw new ArgumentOutOfRangeException(nameof(bodyKind));
            if (themeName != null && String.IsNullOrWhiteSpace(themeName))
                throw new ArgumentException(
                    "Theme name must be nonblank when supplied.",
                    nameof(themeName));
            if (solarRatio < 0)
                throw new ArgumentOutOfRangeException(nameof(solarRatio));
            if (windRatio < 0)
                throw new ArgumentOutOfRangeException(nameof(windRatio));
            if (bodyKind != HomeSystemBodyKind.Solid &&
                (themeName != null || solarRatio.HasValue || windRatio.HasValue))
            {
                throw new ArgumentException(
                    "Giant facts must not contain solid-planet fields.",
                    nameof(bodyKind));
            }

            BodyId = bodyId;
            DisplayDesignation = displayDesignation;
            PlanetNumber = planetNumber;
            OrbitAround = orbitAround;
            ResolvedParentBodyId = resolvedParentBodyId;
            StableGameOrder = stableGameOrder;
            BodyKind = bodyKind;
            ThemeName = themeName;
            SolarRatio = solarRatio;
            WindRatio = windRatio;
        }

        public int BodyId { get; }
        public string DisplayDesignation { get; }
        public int PlanetNumber { get; }
        public int OrbitAround { get; }
        public int? ResolvedParentBodyId { get; }
        public int StableGameOrder { get; }
        public HomeSystemBodyKind BodyKind { get; }
        public string? ThemeName { get; }
        public decimal? SolarRatio { get; }
        public decimal? WindRatio { get; }
    }

    public sealed record HomeSystemBody
    {
        internal HomeSystemBody(
            int bodyId,
            string displayDesignation,
            HomeSystemBodyOrbitKind orbitKind,
            int? parentBodyId,
            int stableGameOrder,
            HomeSystemBodyKind bodyKind,
            string? themeName,
            decimal? solarRatio,
            decimal? windRatio)
        {
            BodyId = bodyId;
            DisplayDesignation = displayDesignation;
            OrbitKind = orbitKind;
            ParentBodyId = parentBodyId;
            StableGameOrder = stableGameOrder;
            BodyKind = bodyKind;
            ThemeName = themeName;
            SolarRatio = solarRatio;
            WindRatio = windRatio;
        }

        public int BodyId { get; }
        public string DisplayDesignation { get; }
        public HomeSystemBodyOrbitKind OrbitKind { get; }
        public int? ParentBodyId { get; }
        public int StableGameOrder { get; }
        public HomeSystemBodyKind BodyKind { get; }
        public string? ThemeName { get; }
        public decimal? SolarRatio { get; }
        public decimal? WindRatio { get; }
    }

    public sealed class HomeSystemBodyInventory
    {
        private readonly HomeSystemBody[] bodies;
        private readonly IReadOnlyList<HomeSystemBody> bodyView;

        private HomeSystemBodyInventory(
            string homeSystemIdentifier,
            HomeSystemBody[] bodies)
        {
            HomeSystemIdentifier = homeSystemIdentifier;
            this.bodies = bodies;
            bodyView = Array.AsReadOnly(bodies);
        }

        public string HomeSystemIdentifier { get; }

        public IReadOnlyList<HomeSystemBody> Bodies => bodyView;

        public static HomeSystemBodyInventory? Project(
            string homeSystemIdentifier,
            IEnumerable<RuntimeHomeSystemBodyEvidence> evidence)
        {
            if (String.IsNullOrWhiteSpace(homeSystemIdentifier))
            {
                throw new ArgumentException(
                    "Home system identifier is required.",
                    nameof(homeSystemIdentifier));
            }
            if (evidence == null)
                throw new ArgumentNullException(nameof(evidence));

            RuntimeHomeSystemBodyEvidence[] values = evidence.ToArray();
            if (values.Length == 0 ||
                values.Select(value => value.BodyId).Distinct().Count() != values.Length ||
                values.Select(value => value.PlanetNumber).Distinct().Count() != values.Length ||
                values.Select(value => value.StableGameOrder).Distinct().Count() != values.Length)
            {
                return null;
            }

            IReadOnlyDictionary<int, RuntimeHomeSystemBodyEvidence> byId =
                values.ToDictionary(value => value.BodyId);
            var projected = new List<HomeSystemBody>(values.Length);
            foreach (RuntimeHomeSystemBodyEvidence value in values
                .OrderBy(item => item.StableGameOrder))
            {
                if (value.OrbitAround == 0)
                {
                    if (value.ResolvedParentBodyId.HasValue)
                        return null;
                    projected.Add(new HomeSystemBody(
                        value.BodyId,
                        value.DisplayDesignation,
                        HomeSystemBodyOrbitKind.Primary,
                        null,
                        value.StableGameOrder,
                        value.BodyKind,
                        value.ThemeName,
                        value.SolarRatio,
                        value.WindRatio));
                    continue;
                }

                if (!value.ResolvedParentBodyId.HasValue ||
                    !byId.TryGetValue(
                        value.ResolvedParentBodyId.Value,
                        out RuntimeHomeSystemBodyEvidence? parent) ||
                    parent.BodyId == value.BodyId ||
                    parent.OrbitAround != 0 ||
                    parent.ResolvedParentBodyId.HasValue ||
                    parent.PlanetNumber != value.OrbitAround)
                {
                    return null;
                }
                projected.Add(new HomeSystemBody(
                    value.BodyId,
                    value.DisplayDesignation,
                    HomeSystemBodyOrbitKind.Satellite,
                    parent.BodyId,
                    value.StableGameOrder,
                    value.BodyKind,
                    value.ThemeName,
                    value.SolarRatio,
                    value.WindRatio));
            }

            return new HomeSystemBodyInventory(homeSystemIdentifier, projected.ToArray());
        }
    }

    public static class HomeSystemBodyPresentation
    {
        public static string Format(HomeSystemBody body)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            if (body.BodyKind == HomeSystemBodyKind.GasGiant)
                return body.DisplayDesignation + " | Gas giant";
            if (body.BodyKind == HomeSystemBodyKind.IceGiant)
                return body.DisplayDesignation + " | Ice giant";

            var facts = new List<string>(3);
            if (body.ThemeName != null)
                facts.Add(body.ThemeName);
            if (body.SolarRatio.HasValue)
            {
                facts.Add("Solar " + FormatPercentage(body.SolarRatio.Value));
            }
            if (body.WindRatio.HasValue)
            {
                facts.Add("Wind " + FormatPercentage(body.WindRatio.Value));
            }
            return facts.Count == 0
                ? body.DisplayDesignation
                : body.DisplayDesignation + " | " + String.Join(" | ", facts);
        }

        public static string FormatPercentage(decimal ratio)
        {
            if (ratio < 0)
                throw new ArgumentOutOfRangeException(nameof(ratio));
            return (ratio * 100m).ToString(
                "0.############################",
                CultureInfo.InvariantCulture) + "%";
        }
    }

    public sealed record ClusterBodyLocation
    {
        public ClusterBodyLocation(
            string bodyIdentifier,
            string displayDesignation,
            string hostSystemIdentifier,
            decimal hostSystemDistanceAu,
            int stableGameOrder)
        {
            if (String.IsNullOrWhiteSpace(bodyIdentifier))
                throw new ArgumentException("Body identity is required.", nameof(bodyIdentifier));
            if (String.IsNullOrWhiteSpace(displayDesignation))
                throw new ArgumentException(
                    "Body display designation is required.",
                    nameof(displayDesignation));
            if (String.IsNullOrWhiteSpace(hostSystemIdentifier))
                throw new ArgumentException(
                    "Host system identity is required.",
                    nameof(hostSystemIdentifier));
            if (hostSystemDistanceAu < 0)
                throw new ArgumentOutOfRangeException(nameof(hostSystemDistanceAu));
            if (stableGameOrder < 0)
                throw new ArgumentOutOfRangeException(nameof(stableGameOrder));

            BodyIdentifier = bodyIdentifier;
            DisplayDesignation = displayDesignation;
            HostSystemIdentifier = hostSystemIdentifier;
            HostSystemDistanceAu = hostSystemDistanceAu;
            StableGameOrder = stableGameOrder;
        }

        public string BodyIdentifier { get; }
        public string DisplayDesignation { get; }
        public string HostSystemIdentifier { get; }
        public decimal HostSystemDistanceAu { get; }
        public int StableGameOrder { get; }

        public string FormattedDistance => DspAuFormatter.Format(HostSystemDistanceAu);
    }

    public static class DspAuFormatter
    {
        public static string Format(decimal astronomicalUnits)
        {
            if (astronomicalUnits < 0)
                throw new ArgumentOutOfRangeException(nameof(astronomicalUnits));
            return astronomicalUnits == 0
                ? "0 AU"
                : RoundToSignificantFigures(astronomicalUnits, 3).ToString(
                    "G29",
                    CultureInfo.InvariantCulture) + " AU";
        }

        public static IReadOnlyList<ClusterBodyLocation> StableOrder(
            IEnumerable<ClusterBodyLocation> locations)
        {
            if (locations == null)
                throw new ArgumentNullException(nameof(locations));
            return Array.AsReadOnly(locations
                .OrderBy(value => value.HostSystemDistanceAu)
                .ThenBy(value => value.StableGameOrder)
                .ThenBy(value => value.BodyIdentifier, StringComparer.Ordinal)
                .ToArray());
        }

        private static decimal RoundToSignificantFigures(decimal value, int digits)
        {
            int magnitude = (int)Math.Floor(Math.Log10((double)Math.Abs(value)));
            int decimalPlaces = digits - magnitude - 1;
            if (decimalPlaces >= 0)
                return Decimal.Round(value, Math.Min(decimalPlaces, 28));
            decimal factor = Convert.ToDecimal(Math.Pow(10, -decimalPlaces));
            return Decimal.Round(value / factor, 0) * factor;
        }
    }

    public sealed record PreviewStatisticItem
    {
        public PreviewStatisticItem(
            string key,
            string text,
            int stableOrder,
            string? subsectionKey = null,
            string? subsectionTitle = null,
            int subsectionOrder = 0)
        {
            if (String.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Statistic key is required.", nameof(key));
            if (String.IsNullOrWhiteSpace(text))
                throw new ArgumentException("Statistic text is required.", nameof(text));
            if (stableOrder < 0)
                throw new ArgumentOutOfRangeException(nameof(stableOrder));
            if ((subsectionKey == null) != (subsectionTitle == null))
            {
                throw new ArgumentException(
                    "A subsection key and title must be supplied together.");
            }
            if (subsectionKey != null && String.IsNullOrWhiteSpace(subsectionKey))
                throw new ArgumentException("Subsection key cannot be blank.", nameof(subsectionKey));
            if (subsectionTitle != null && String.IsNullOrWhiteSpace(subsectionTitle))
            {
                throw new ArgumentException(
                    "Subsection title cannot be blank.",
                    nameof(subsectionTitle));
            }
            if (subsectionOrder < 0)
                throw new ArgumentOutOfRangeException(nameof(subsectionOrder));

            Key = key;
            Text = text;
            StableOrder = stableOrder;
            SubsectionKey = subsectionKey;
            SubsectionTitle = subsectionTitle;
            SubsectionOrder = subsectionOrder;
        }

        public string Key { get; }
        public string Text { get; }
        public int StableOrder { get; }
        public string? SubsectionKey { get; }
        public string? SubsectionTitle { get; }
        public int SubsectionOrder { get; }
    }

    public sealed record PreviewStatisticSubsection
    {
        internal PreviewStatisticSubsection(
            string? key,
            string? title,
            IEnumerable<PreviewStatisticItem> items)
        {
            Key = key;
            Title = title;
            Items = Array.AsReadOnly(items.ToArray());
        }

        public string? Key { get; }
        public string? Title { get; }
        public IReadOnlyList<PreviewStatisticItem> Items { get; }
    }

    public sealed class PreviewClusterStatistics
    {
        public const int MaximumItems = 128;
        private readonly PreviewStatisticItem[] items;

        public PreviewClusterStatistics()
            : this(Array.Empty<PreviewStatisticItem>())
        {
        }

        private PreviewClusterStatistics(PreviewStatisticItem[] items)
        {
            this.items = items;
        }

        public IReadOnlyList<PreviewStatisticItem> Items =>
            Array.AsReadOnly((PreviewStatisticItem[])items.Clone());

        public PreviewClusterStatistics With(PreviewStatisticItem item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            PreviewStatisticItem[] result = items
                .Where(value => !String.Equals(value.Key, item.Key, StringComparison.Ordinal))
                .Append(item)
                .OrderBy(value => value.SubsectionKey == null ? 0 : 1)
                .ThenBy(value => value.SubsectionOrder)
                .ThenBy(value => value.StableOrder)
                .ThenBy(value => value.Key, StringComparer.Ordinal)
                .ToArray();
            if (result.Length > MaximumItems)
                throw new InvalidOperationException("Cluster statistics exceed their bound.");
            ValidateSubsections(result);
            return new PreviewClusterStatistics(result);
        }

        public IReadOnlyList<PreviewStatisticSubsection> Sections()
        {
            var sections = new List<PreviewStatisticSubsection>();
            PreviewStatisticItem[] untitled = items
                .Where(value => value.SubsectionKey == null)
                .ToArray();
            if (untitled.Length != 0)
                sections.Add(new PreviewStatisticSubsection(null, null, untitled));
            sections.AddRange(items
                .Where(value => value.SubsectionKey != null)
                .GroupBy(value => value.SubsectionKey!, StringComparer.Ordinal)
                .OrderBy(group => group.First().SubsectionOrder)
                .ThenBy(group => group.Key, StringComparer.Ordinal)
                .Select(group => new PreviewStatisticSubsection(
                    group.Key,
                    group.First().SubsectionTitle,
                    group)));
            return Array.AsReadOnly(sections.ToArray());
        }

        private static void ValidateSubsections(IEnumerable<PreviewStatisticItem> values)
        {
            foreach (IGrouping<string, PreviewStatisticItem> group in values
                .Where(value => value.SubsectionKey != null)
                .GroupBy(value => value.SubsectionKey!, StringComparer.Ordinal))
            {
                if (group.Select(value => value.SubsectionTitle)
                        .Distinct(StringComparer.Ordinal).Count() != 1 ||
                    group.Select(value => value.SubsectionOrder).Distinct().Count() != 1)
                {
                    throw new ArgumentException(
                        "One subsection key must have one title and stable order.");
                }
            }
        }
    }

    public sealed record PreviewStatisticsDocument
    {
        public const string HomeSystemTitle = "Home system";
        public const string ClusterTitle = "Cluster";

        internal PreviewStatisticsDocument(
            long sessionId,
            string identityLine,
            HomeSystemBodyInventory? homeSystem,
            PreviewClusterStatistics cluster)
        {
            if (sessionId <= 0)
                throw new ArgumentOutOfRangeException(nameof(sessionId));
            if (String.IsNullOrWhiteSpace(identityLine))
                throw new ArgumentException("Identity line is required.", nameof(identityLine));
            SessionId = sessionId;
            IdentityLine = identityLine;
            HomeSystem = homeSystem;
            Cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
        }

        public long SessionId { get; }
        public string IdentityLine { get; }
        public HomeSystemBodyInventory? HomeSystem { get; }
        public PreviewClusterStatistics Cluster { get; }
    }

    public static class PreviewIdentityPresentation
    {
        public static string Format(
            PreviewGenerationIdentity identity,
            string? homePlanetDisplayDesignation = null)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));
            if (homePlanetDisplayDesignation != null &&
                String.IsNullOrWhiteSpace(homePlanetDisplayDesignation))
            {
                throw new ArgumentException(
                    "Home planet display designation cannot be blank.",
                    nameof(homePlanetDisplayDesignation));
            }
            string home = homePlanetDisplayDesignation == null
                ? String.Empty
                : " | Home " + homePlanetDisplayDesignation;
            return "Seed " + identity.GalaxyIdentity.GalaxySeed.ToString(
                    "D8",
                    CultureInfo.InvariantCulture) +
                home +
                " | " + identity.GalaxyIdentity.RequestedStarCount.ToString(
                    CultureInfo.InvariantCulture) +
                " stars | resources x" + identity.ResourceMultiplier.ToString(
                    "G29",
                    CultureInfo.InvariantCulture) +
                " | " + (identity.CombatMode == CombatMode.Peace ? "Peace" : "Combat");
        }
    }

    public sealed class PreviewStatisticsPanelController
    {
        private long activeSessionId;

        public PreviewStatisticsDocument? Current { get; private set; }
        public double ScrollX { get; private set; }
        public double ScrollY { get; private set; }

        public void BeginSession(PreviewSession session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));
            activeSessionId = session.SessionId;
            ScrollX = 0;
            ScrollY = 0;
            Current = new PreviewStatisticsDocument(
                session.SessionId,
                PreviewIdentityPresentation.Format(
                    session.Identity,
                    session.HomePlanetDisplayDesignation),
                null,
                new PreviewClusterStatistics());
        }

        public bool Update(PreviewResolutionAttempt attempt)
        {
            if (attempt == null)
                throw new ArgumentNullException(nameof(attempt));
            if (attempt.Session.SessionId != activeSessionId || attempt.Session.IsRetired)
                return false;
            Current = new PreviewStatisticsDocument(
                activeSessionId,
                PreviewIdentityPresentation.Format(
                    attempt.Session.Identity,
                    attempt.Session.HomePlanetDisplayDesignation),
                attempt.HomeSystemBodyInventory,
                Current?.Cluster ?? new PreviewClusterStatistics());
            return true;
        }

        public bool Hide(long sessionId)
        {
            if (sessionId != activeSessionId)
                return false;
            HideCurrent();
            return true;
        }

        public bool SetScrollPosition(long sessionId, double x, double y)
        {
            if (sessionId != activeSessionId || Current == null)
                return false;
            if (Double.IsNaN(x) || Double.IsInfinity(x) || x < 0)
                throw new ArgumentOutOfRangeException(nameof(x));
            if (Double.IsNaN(y) || Double.IsInfinity(y) || y < 0)
                throw new ArgumentOutOfRangeException(nameof(y));
            ScrollX = x;
            ScrollY = y;
            return true;
        }

        public void HideCurrent()
        {
            activeSessionId = 0;
            Current = null;
            ScrollX = 0;
            ScrollY = 0;
        }
    }
}
