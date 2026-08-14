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
            decimal? windRatio = null,
            IEnumerable<string>? gasProducts = null)
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
            string[] products = ResourcePresentation.Normalize(gasProducts);
            if (bodyKind == HomeSystemBodyKind.Solid && products.Length != 0)
            {
                throw new ArgumentException(
                    "Solid planets must not contain giant gas products.",
                    nameof(gasProducts));
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
            GasProducts = Array.AsReadOnly(products);
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
        public IReadOnlyList<string> GasProducts { get; }
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
            decimal? windRatio,
            IEnumerable<string> gasProducts)
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
            GasProducts = Array.AsReadOnly(gasProducts.ToArray());
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
        public IReadOnlyList<string> GasProducts { get; }
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
                values.Where(value => value.OrbitAround == 0)
                    .Select(value => value.PlanetNumber).Distinct().Count() !=
                    values.Count(value => value.OrbitAround == 0) ||
                values.Select(value => value.StableGameOrder).Distinct().Count() != values.Length)
            {
                return null;
            }

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
                        value.WindRatio,
                        value.GasProducts));
                    continue;
                }

                RuntimeHomeSystemBodyEvidence? parent = values.SingleOrDefault(candidate =>
                    candidate.PlanetNumber == value.OrbitAround &&
                    candidate.OrbitAround == 0);
                if (parent == null ||
                    parent.BodyId == value.BodyId ||
                    parent.ResolvedParentBodyId.HasValue ||
                    (value.ResolvedParentBodyId.HasValue &&
                        value.ResolvedParentBodyId.Value != parent.BodyId))
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
                    value.WindRatio,
                    value.GasProducts));
            }

            return new HomeSystemBodyInventory(homeSystemIdentifier, projected.ToArray());
        }
    }

    public static class HomeSystemBodyPresentation
    {
        public static HomeSystemBodyTableRow ProjectTableRow(
            HomeSystemBody body,
            HomeSystemResourceStatistics? resources = null)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            HomeSystemBodyResources? bodyResources = body.BodyKind ==
                HomeSystemBodyKind.Solid
                    ? resources?.ForBody(body.BodyId)
                    : null;
            HomeSystemResource[] values = bodyResources?.Resources.ToArray() ??
                Array.Empty<HomeSystemResource>();
            return new HomeSystemBodyTableRow(
                body.DisplayDesignation,
                body.BodyKind == HomeSystemBodyKind.Solid
                    ? body.ThemeName ?? String.Empty
                    : body.BodyKind == HomeSystemBodyKind.IceGiant
                        ? "Ice giant"
                        : "Gas giant",
                body.SolarRatio.HasValue
                    ? FormatPercentage(body.SolarRatio.Value)
                    : String.Empty,
                body.WindRatio.HasValue
                    ? FormatPercentage(body.WindRatio.Value)
                    : String.Empty,
                String.Join(
                    ", ",
                    values.Where(resource =>
                            resource.Semantics == RawResourceSemantics.FiniteDeposit)
                        .Select(FormatResourceValues)),
                String.Join(
                    "\n",
                    values.Where(resource =>
                            resource.Semantics == RawResourceSemantics.OilFlow)
                        .Select(resource =>
                            FormatAmount(resource.Amount) + " / " +
                            resource.VeinGroups.ToString(CultureInfo.InvariantCulture))),
                String.Join(
                    "\n",
                    body.GasProducts.Select(ResourcePresentation.GasProductName)));
        }

        public static string Format(
            HomeSystemBody body,
            HomeSystemResourceStatistics? resources = null)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body));

            var facts = new List<string>(5);
            if (body.BodyKind != HomeSystemBodyKind.Solid)
            {
                facts.Add(body.BodyKind == HomeSystemBodyKind.IceGiant
                    ? "Ice giant"
                    : "Gas giant");
                if (body.GasProducts.Count != 0)
                {
                    facts.Add("Gas products: " + String.Join(
                        ", ",
                        body.GasProducts.Select(ResourcePresentation.GasProductName)));
                }
            }
            else
            {
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
                HomeSystemBodyResources? bodyResources = resources?.ForBody(body.BodyId);
                if (bodyResources != null && bodyResources.Resources.Count != 0)
                {
                    HomeSystemResource[] ores = bodyResources.Resources
                        .Where(resource =>
                            resource.Semantics == RawResourceSemantics.FiniteDeposit)
                        .ToArray();
                    if (ores.Length != 0)
                    {
                        facts.Add("Ores (units / vein groups): " + String.Join(
                            "; ",
                            ores.Select(FormatResourceValues)));
                    }
                    HomeSystemResource? oil = bodyResources.Resources.SingleOrDefault(
                        resource => resource.Semantics == RawResourceSemantics.OilFlow);
                    if (oil != null)
                    {
                        facts.Add("Crude Oil (flow units / groups): " +
                            FormatAmount(oil.Amount) + " / " +
                            oil.VeinGroups.ToString(CultureInfo.InvariantCulture));
                    }
                }
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

        private static string FormatResourceValues(HomeSystemResource resource) =>
            ResourcePresentation.OreName(resource.ResourceId) + " " +
            FormatAmount(resource.Amount) + " / " +
            resource.VeinGroups.ToString(CultureInfo.InvariantCulture);

        internal static string FormatAmount(long amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            if (amount < 1_000)
                return amount.ToString(CultureInfo.InvariantCulture);

            decimal scaled = amount >= 1_000_000
                ? amount / 1_000_000m
                : amount / 1_000m;
            int decimals = scaled >= 100m ? 0 : scaled >= 10m ? 1 : 2;
            decimal rounded = Math.Round(
                scaled,
                decimals,
                MidpointRounding.AwayFromZero);
            if (amount < 1_000_000 && rounded >= 1_000m)
                return "1M";
            return rounded.ToString("0.##", CultureInfo.InvariantCulture) +
                (amount >= 1_000_000 ? "M" : "K");
        }
    }

    public sealed record HomeSystemBodyTableRow
    {
        private readonly IReadOnlyList<string> cells;

        internal HomeSystemBodyTableRow(
            string body,
            string world,
            string solar,
            string wind,
            string ores,
            string oil,
            string gasProducts)
        {
            Body = body;
            World = world;
            Solar = solar;
            Wind = wind;
            Ores = ores;
            Oil = oil;
            GasProducts = gasProducts;
            cells = Array.AsReadOnly(new[]
            {
                Body,
                World,
                Solar,
                Wind,
                Ores,
                Oil,
                GasProducts
            });
        }

        public string Body { get; }
        public string World { get; }
        public string Solar { get; }
        public string Wind { get; }
        public string Ores { get; }
        public string Oil { get; }
        public string GasProducts { get; }

        public IReadOnlyList<string> Cells => cells;
    }

    public sealed record HomeSystemResource
    {
        public HomeSystemResource(
            string resourceId,
            RawResourceSemantics semantics,
            long amount,
            int veinGroups)
        {
            if (!ResourcePresentation.Supports(resourceId))
                throw new ArgumentException("A resource identifier is unsupported.", nameof(resourceId));
            if (!Enum.IsDefined(typeof(RawResourceSemantics), semantics))
                throw new ArgumentOutOfRangeException(nameof(semantics));
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            if (veinGroups <= 0)
                throw new ArgumentOutOfRangeException(nameof(veinGroups));

            ResourceId = resourceId;
            Semantics = semantics;
            Amount = amount;
            VeinGroups = veinGroups;
        }

        public string ResourceId { get; }
        public RawResourceSemantics Semantics { get; }
        public long Amount { get; }
        public int VeinGroups { get; }
    }

    public sealed record HomeSystemBodyResources
    {
        public HomeSystemBodyResources(
            int bodyId,
            IEnumerable<HomeSystemResource> resources)
        {
            if (bodyId <= 0)
                throw new ArgumentOutOfRangeException(nameof(bodyId));
            if (resources == null)
                throw new ArgumentNullException(nameof(resources));
            BodyId = bodyId;
            HomeSystemResource[] values = resources
                .OrderBy(value => ResourcePresentation.Order(value.ResourceId))
                .ToArray();
            if (values.Select(value => value.ResourceId)
                .Distinct(StringComparer.Ordinal).Count() != values.Length)
            {
                throw new ArgumentException(
                    "Home-system resources must be unique by resource identifier.",
                    nameof(resources));
            }
            Resources = Array.AsReadOnly(values);
        }

        public int BodyId { get; }
        public IReadOnlyList<HomeSystemResource> Resources { get; }
    }

    public sealed class HomeSystemResourceStatistics
    {
        public const int MaximumBodies = 256;
        private readonly HomeSystemBodyResources[] bodies;
        private readonly IReadOnlyDictionary<int, HomeSystemBodyResources> byBodyId;

        public HomeSystemResourceStatistics(IEnumerable<HomeSystemBodyResources> bodies)
        {
            if (bodies == null)
                throw new ArgumentNullException(nameof(bodies));
            this.bodies = bodies.OrderBy(value => value.BodyId).ToArray();
            if (this.bodies.Length > MaximumBodies ||
                this.bodies.Select(value => value.BodyId).Distinct().Count() !=
                    this.bodies.Length)
            {
                throw new ArgumentException(
                    "Home-system resource bodies must be bounded and unique.",
                    nameof(bodies));
            }
            byBodyId = this.bodies.ToDictionary(value => value.BodyId);
        }

        public IReadOnlyList<HomeSystemBodyResources> Bodies =>
            Array.AsReadOnly((HomeSystemBodyResources[])bodies.Clone());

        public HomeSystemBodyResources? ForBody(int bodyId) =>
            byBodyId.TryGetValue(bodyId, out HomeSystemBodyResources? value)
                ? value
                : null;
    }

    public static class ResourcePresentation
    {
        private static readonly IReadOnlyDictionary<string, (int Order, string Ore, string Gas)>
            Names = new Dictionary<string, (int, string, string)>(StringComparer.Ordinal)
            {
                { "iron", (0, "Iron", "Iron") },
                { "copper", (1, "Copper", "Copper") },
                { "silicon", (2, "Silicon", "Silicon") },
                { "titanium", (3, "Titanium", "Titanium") },
                { "stone", (4, "Stone", "Stone") },
                { "coal", (5, "Coal", "Coal") },
                { "oil", (6, "Crude Oil", "Crude Oil") },
                { "fire-ice", (7, "Fire Ice veins", "Fire Ice") },
                { "kimberlite", (8, "Kimberlite", "Kimberlite") },
                { "fractal-silicon", (9, "Fractal Silicon", "Fractal Silicon") },
                { "organic-crystal", (10, "Organic Crystal", "Organic Crystal") },
                { "optical-grating-crystal", (11, "Optical Grating Crystal", "Optical Grating Crystal") },
                { "spiniform-stalagmite-crystal", (12, "Spiniform Stalagmite Crystal", "Spiniform Stalagmite Crystal") },
                { "unipolar-magnet", (13, "Unipolar Magnet", "Unipolar Magnet") },
                { "hydrogen", (14, "Hydrogen", "Hydrogen") },
                { "deuterium", (15, "Deuterium", "Deuterium") }
            };

        internal static string[] Normalize(IEnumerable<string>? resourceIds)
        {
            if (resourceIds == null)
                return Array.Empty<string>();
            string[] values = resourceIds.ToArray();
            if (values.Any(String.IsNullOrWhiteSpace) ||
                values.Any(value => !Names.ContainsKey(value)))
            {
                throw new ArgumentException("A resource identifier is unsupported.", nameof(resourceIds));
            }
            return values.Distinct(StringComparer.Ordinal)
                .OrderBy(value => Names[value].Order)
                .ToArray();
        }

        public static string OreName(string resourceId) => Name(resourceId).Ore;

        public static string GasProductName(string resourceId) => Name(resourceId).Gas;

        public static bool Supports(string resourceId) =>
            resourceId != null && Names.ContainsKey(resourceId);

        internal static int Order(string resourceId) => Name(resourceId).Order;

        private static (int Order, string Ore, string Gas) Name(string resourceId)
        {
            if (resourceId == null)
                throw new ArgumentNullException(nameof(resourceId));
            if (!Names.TryGetValue(resourceId, out (int, string, string) value))
                throw new ArgumentException("A resource identifier is unsupported.", nameof(resourceId));
            return value;
        }
    }

    public sealed record ClusterBodyLocation
    {
        public ClusterBodyLocation(
            string bodyIdentifier,
            string displayDesignation,
            string hostSystemIdentifier,
            decimal hostSystemDistanceLy,
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
            if (hostSystemDistanceLy < 0)
                throw new ArgumentOutOfRangeException(nameof(hostSystemDistanceLy));
            if (stableGameOrder < 0)
                throw new ArgumentOutOfRangeException(nameof(stableGameOrder));

            BodyIdentifier = bodyIdentifier;
            DisplayDesignation = displayDesignation;
            HostSystemIdentifier = hostSystemIdentifier;
            HostSystemDistanceLy = hostSystemDistanceLy;
            StableGameOrder = stableGameOrder;
        }

        public string BodyIdentifier { get; }
        public string DisplayDesignation { get; }
        public string HostSystemIdentifier { get; }
        public decimal HostSystemDistanceLy { get; }
        public int StableGameOrder { get; }

        public string FormattedDistance => DspLyFormatter.Format(HostSystemDistanceLy);
    }

    public static class DspLyFormatter
    {
        public static string Format(decimal lightYears)
        {
            if (lightYears < 0)
                throw new ArgumentOutOfRangeException(nameof(lightYears));
            return lightYears == 0
                ? "0 ly"
                : RoundToSignificantFigures(lightYears, 3).ToString(
                    "G29",
                    CultureInfo.InvariantCulture) + " ly";
        }

        public static IReadOnlyList<ClusterBodyLocation> StableOrder(
            IEnumerable<ClusterBodyLocation> locations)
        {
            if (locations == null)
                throw new ArgumentNullException(nameof(locations));
            return Array.AsReadOnly(locations
                .OrderBy(value => value.HostSystemDistanceLy)
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

    public sealed record ClusterResourceCandidate
    {
        public ClusterResourceCandidate(string categoryId, ClusterBodyLocation location)
        {
            if (!ClusterResourcePresentation.Supports(categoryId))
                throw new ArgumentException("A cluster-resource category is unsupported.", nameof(categoryId));
            CategoryId = categoryId;
            Location = location ?? throw new ArgumentNullException(nameof(location));
        }

        public string CategoryId { get; }
        public ClusterBodyLocation Location { get; }
    }

    public sealed record UnipolarMagnetPlanetStatistics
    {
        public UnipolarMagnetPlanetStatistics(
            ClusterBodyLocation location,
            int veinNodes,
            long amount,
            int veinGroups)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
            if (veinNodes <= 0)
                throw new ArgumentOutOfRangeException(nameof(veinNodes));
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            if (veinGroups <= 0)
                throw new ArgumentOutOfRangeException(nameof(veinGroups));
            VeinNodes = veinNodes;
            Amount = amount;
            VeinGroups = veinGroups;
        }

        public ClusterBodyLocation Location { get; }
        public int VeinNodes { get; }
        public long Amount { get; }
        public int VeinGroups { get; }
    }

    public sealed record NearbyDeuteriumGasGiantCandidate
    {
        public NearbyDeuteriumGasGiantCandidate(
            ClusterBodyLocation location,
            decimal collectionRate)
        {
            Location = location ?? throw new ArgumentNullException(nameof(location));
            if (collectionRate < 0)
                throw new ArgumentOutOfRangeException(nameof(collectionRate));
            CollectionRate = collectionRate;
        }

        public ClusterBodyLocation Location { get; }
        public decimal CollectionRate { get; }
    }

    public sealed record NearbyDeuteriumTableRow
    {
        private readonly IReadOnlyList<string> cells;

        internal NearbyDeuteriumTableRow(
            string gasGiant,
            string distance,
            string rate)
        {
            GasGiant = gasGiant;
            Distance = distance;
            Rate = rate;
            cells = Array.AsReadOnly(new[] { GasGiant, Distance, Rate });
        }

        public string GasGiant { get; }
        public string Distance { get; }
        public string Rate { get; }
        public IReadOnlyList<string> Cells => cells;
    }

    public sealed class NearbyDeuteriumGasGiantSelection
    {
        public const decimal MaximumDistanceLy = 8.125m;
        private const string StatisticKey = "deuterium:strongest-nearby";

        private NearbyDeuteriumGasGiantSelection(
            bool attributionComplete,
            NearbyDeuteriumGasGiantCandidate? candidate)
        {
            AttributionComplete = attributionComplete;
            Candidate = candidate;
        }

        public bool AttributionComplete { get; }
        public NearbyDeuteriumGasGiantCandidate? Candidate { get; }

        public NearbyDeuteriumTableRow? ProjectTableRow()
        {
            if (!AttributionComplete)
                return null;
            return Candidate == null
                ? new NearbyDeuteriumTableRow(
                    "Not found within 8.125 ly",
                    String.Empty,
                    String.Empty)
                : new NearbyDeuteriumTableRow(
                    Candidate.Location.DisplayDesignation,
                    Candidate.Location.FormattedDistance,
                    FormatRate(Candidate.CollectionRate));
        }

        public static NearbyDeuteriumGasGiantSelection Select(
            IEnumerable<NearbyDeuteriumGasGiantCandidate> candidates,
            bool attributionComplete = true)
        {
            if (candidates == null)
                throw new ArgumentNullException(nameof(candidates));
            if (!attributionComplete)
                return new NearbyDeuteriumGasGiantSelection(false, null);

            NearbyDeuteriumGasGiantCandidate? winner = null;
            foreach (NearbyDeuteriumGasGiantCandidate candidate in candidates)
            {
                if (candidate == null)
                    throw new ArgumentException("A Deuterium candidate cannot be null.", nameof(candidates));
                winner = Prefer(winner, candidate);
            }
            return new NearbyDeuteriumGasGiantSelection(true, winner);
        }

        internal static NearbyDeuteriumGasGiantCandidate? Prefer(
            NearbyDeuteriumGasGiantCandidate? current,
            NearbyDeuteriumGasGiantCandidate candidate)
        {
            if (candidate == null)
                throw new ArgumentNullException(nameof(candidate));
            if (candidate.Location.HostSystemDistanceLy > MaximumDistanceLy)
                return current;
            return current == null || Better(candidate, current)
                ? candidate
                : current;
        }

        internal static NearbyDeuteriumGasGiantSelection FromWinner(
            NearbyDeuteriumGasGiantCandidate? candidate,
            bool attributionComplete)
        {
            if (candidate?.Location.HostSystemDistanceLy > MaximumDistanceLy)
                throw new ArgumentException("The selected Deuterium giant is outside the distance bound.", nameof(candidate));
            return new NearbyDeuteriumGasGiantSelection(
                attributionComplete,
                attributionComplete ? candidate : null);
        }

        internal static bool IsTableItemKey(string key) =>
            String.Equals(key, StatisticKey, StringComparison.Ordinal);

        public PreviewClusterStatistics Apply(PreviewClusterStatistics statistics)
        {
            if (statistics == null)
                throw new ArgumentNullException(nameof(statistics));
            if (!AttributionComplete)
                return statistics;

            string text = Candidate == null
                ? "No Deuterium gas giants within 8.125 ly"
                : Candidate.Location.DisplayDesignation + " - " +
                    Candidate.Location.FormattedDistance + " - " +
                    "Deuterium " + FormatRate(Candidate.CollectionRate);
            return statistics.With(new PreviewStatisticItem(
                StatisticKey,
                text,
                400));
        }

        private static bool Better(
            NearbyDeuteriumGasGiantCandidate candidate,
            NearbyDeuteriumGasGiantCandidate current)
        {
            int rate = candidate.CollectionRate.CompareTo(current.CollectionRate);
            if (rate != 0)
                return rate > 0;
            int distance = candidate.Location.HostSystemDistanceLy.CompareTo(
                current.Location.HostSystemDistanceLy);
            if (distance != 0)
                return distance < 0;
            int order = candidate.Location.StableGameOrder.CompareTo(
                current.Location.StableGameOrder);
            if (order != 0)
                return order < 0;
            return StringComparer.Ordinal.Compare(
                candidate.Location.BodyIdentifier,
                current.Location.BodyIdentifier) < 0;
        }

        private static string FormatRate(decimal value) =>
            Decimal.Round(value, 4, MidpointRounding.AwayFromZero)
                .ToString("0.0000", CultureInfo.InvariantCulture) + "/s";
    }

    public sealed class ClusterResourceStatistics
    {
        public const int MaximumCategories = 7;
        public const int MaximumCandidatesPerCategory = 2;
        public const int MaximumUnipolarPlanets =
            CompleteClusterRawCoordinator.MaximumSolidPlanets;
        private readonly ClusterResourceCandidate[] candidates;
        private readonly UnipolarMagnetPlanetStatistics[] unipolarMagnets;

        public ClusterResourceStatistics(
            IEnumerable<ClusterResourceCandidate> candidates,
            IEnumerable<UnipolarMagnetPlanetStatistics>? unipolarMagnets = null)
        {
            if (candidates == null)
                throw new ArgumentNullException(nameof(candidates));
            this.candidates = candidates
                .OrderBy(value => ClusterResourcePresentation.Order(value.CategoryId))
                .ThenBy(value => value.Location.HostSystemDistanceLy)
                .ThenBy(value => value.Location.StableGameOrder)
                .ThenBy(value => value.Location.BodyIdentifier, StringComparer.Ordinal)
                .ToArray();
            if (this.candidates.GroupBy(value => value.CategoryId, StringComparer.Ordinal)
                    .Count() > MaximumCategories ||
                this.candidates.GroupBy(value => value.CategoryId, StringComparer.Ordinal)
                    .Any(group => group.Count() > MaximumCandidatesPerCategory) ||
                this.candidates.GroupBy(value => value.CategoryId, StringComparer.Ordinal)
                    .Any(group => group.Select(value => value.Location.BodyIdentifier)
                        .Distinct(StringComparer.Ordinal).Count() != group.Count()))
            {
                throw new ArgumentException("Cluster-resource candidates exceed their bounds.", nameof(candidates));
            }
            this.unipolarMagnets = (unipolarMagnets ??
                    Array.Empty<UnipolarMagnetPlanetStatistics>())
                .OrderBy(value => value.Location.HostSystemDistanceLy)
                .ThenBy(value => value.Location.StableGameOrder)
                .ThenBy(value => value.Location.BodyIdentifier, StringComparer.Ordinal)
                .ToArray();
            if (this.unipolarMagnets.Length > MaximumUnipolarPlanets ||
                this.unipolarMagnets.Select(value => value.Location.BodyIdentifier)
                    .Distinct(StringComparer.Ordinal).Count() != this.unipolarMagnets.Length)
            {
                throw new ArgumentException(
                    "Unipolar Magnet planet statistics exceed their bounds.",
                    nameof(unipolarMagnets));
            }
        }

        public IReadOnlyList<ClusterResourceCandidate> Candidates =>
            Array.AsReadOnly((ClusterResourceCandidate[])candidates.Clone());

        public IReadOnlyList<ClusterResourceCandidate> ForCategory(string categoryId) =>
            Array.AsReadOnly(candidates.Where(value => String.Equals(
                value.CategoryId,
                categoryId,
                StringComparison.Ordinal)).ToArray());

        public IReadOnlyList<UnipolarMagnetPlanetStatistics> UnipolarMagnets =>
            Array.AsReadOnly((UnipolarMagnetPlanetStatistics[])unipolarMagnets.Clone());
    }

    public sealed record ClusterRareResourceTableRow
    {
        private readonly IReadOnlyList<string> cells;

        internal ClusterRareResourceTableRow(
            string resource,
            string closest,
            string alternative)
        {
            Resource = resource;
            Closest = closest;
            Alternative = alternative;
            cells = Array.AsReadOnly(new[] { Resource, Closest, Alternative });
        }

        public string Resource { get; }
        public string Closest { get; }
        public string Alternative { get; }
        public IReadOnlyList<string> Cells => cells;
    }

    public sealed record ClusterUnipolarMagnetTableRow
    {
        private readonly IReadOnlyList<string> cells;

        internal ClusterUnipolarMagnetTableRow(
            string planet,
            string distance,
            string veins,
            string magnets,
            string groups)
        {
            Planet = planet;
            Distance = distance;
            Veins = veins;
            Magnets = magnets;
            Groups = groups;
            cells = Array.AsReadOnly(new[]
            {
                Planet,
                Distance,
                Veins,
                Magnets,
                Groups
            });
        }

        public string Planet { get; }
        public string Distance { get; }
        public string Veins { get; }
        public string Magnets { get; }
        public string Groups { get; }
        public IReadOnlyList<string> Cells => cells;
    }

    public static class ClusterResourcePresentation
    {
        public const string SulfuricAcidOcean = "sulfuric-acid-ocean";
        private static readonly IReadOnlyDictionary<string, (int Order, string Name)> Names =
            new Dictionary<string, (int, string)>(StringComparer.Ordinal)
            {
                { SulfuricAcidOcean, (0, "Sulfuric Acid ocean") },
                { "fire-ice", (1, "Fire Ice veins") },
                { "fractal-silicon", (2, "Fractal Silicon") },
                { "kimberlite", (3, "Kimberlite") },
                { "optical-grating-crystal", (4, "Optical Grating Crystal") },
                { "organic-crystal", (5, "Organic Crystal") },
                { "spiniform-stalagmite-crystal", (6, "Spiniform Stalagmite Crystal") }
            };

        public static IReadOnlyList<string> CategoryIds =>
            Array.AsReadOnly(Names.OrderBy(pair => pair.Value.Order)
                .Select(pair => pair.Key).ToArray());

        public static PreviewClusterStatistics Project(ClusterResourceStatistics statistics)
        {
            if (statistics == null)
                throw new ArgumentNullException(nameof(statistics));
            var result = new PreviewClusterStatistics();
            foreach (string categoryId in CategoryIds)
            {
                IReadOnlyList<ClusterResourceCandidate> candidates =
                    statistics.ForCategory(categoryId);
                string name = Names[categoryId].Name;
                string text = candidates.Count == 0
                    ? "No " + name + " found"
                    : name + ": " + String.Join(", ", candidates.Select(value =>
                        value.Location.DisplayDesignation + " - " +
                        value.Location.FormattedDistance));
                result = result.With(new PreviewStatisticItem(
                    "resource:" + categoryId,
                    text,
                    Names[categoryId].Order));
            }
            if (statistics.UnipolarMagnets.Count == 0)
            {
                result = result.With(new PreviewStatisticItem(
                    "unipolar:none",
                    "No Unipolar Magnets found",
                    100));
            }
            else
            {
                foreach (UnipolarMagnetPlanetStatistics planet in
                    statistics.UnipolarMagnets)
                {
                    result = result.With(new PreviewStatisticItem(
                        "unipolar:" + planet.Location.BodyIdentifier,
                        FormatUnipolar(planet),
                        100 + planet.Location.StableGameOrder));
                }
            }
            return result;
        }

        public static IReadOnlyList<ClusterRareResourceTableRow>
            ProjectRareResourceTableRows(ClusterResourceStatistics statistics)
        {
            if (statistics == null)
                throw new ArgumentNullException(nameof(statistics));
            return Array.AsReadOnly(CategoryIds.Select(categoryId =>
            {
                IReadOnlyList<ClusterResourceCandidate> candidates =
                    statistics.ForCategory(categoryId);
                return new ClusterRareResourceTableRow(
                    Names[categoryId].Name,
                    candidates.Count == 0
                        ? "Not found"
                        : FormatLocation(candidates[0].Location),
                    candidates.Count < 2
                        ? String.Empty
                        : FormatLocation(candidates[1].Location));
            }).ToArray());
        }

        public static IReadOnlyList<ClusterUnipolarMagnetTableRow>
            ProjectUnipolarTableRows(ClusterResourceStatistics statistics)
        {
            if (statistics == null)
                throw new ArgumentNullException(nameof(statistics));
            if (statistics.UnipolarMagnets.Count == 0)
            {
                return Array.AsReadOnly(new[]
                {
                    new ClusterUnipolarMagnetTableRow(
                        "Not found",
                        String.Empty,
                        String.Empty,
                        String.Empty,
                        String.Empty)
                });
            }
            return Array.AsReadOnly(statistics.UnipolarMagnets.Select(planet =>
                new ClusterUnipolarMagnetTableRow(
                    planet.Location.DisplayDesignation,
                    planet.Location.FormattedDistance,
                    planet.VeinNodes.ToString("N0", CultureInfo.InvariantCulture),
                    planet.Amount.ToString("N0", CultureInfo.InvariantCulture),
                    planet.VeinGroups.ToString("N0", CultureInfo.InvariantCulture)))
                .ToArray());
        }

        internal static bool IsTableItemKey(string key) =>
            key != null &&
            (key.StartsWith("resource:", StringComparison.Ordinal) ||
                key.StartsWith("unipolar:", StringComparison.Ordinal));

        public static bool Supports(string categoryId) =>
            categoryId != null && Names.ContainsKey(categoryId);

        internal static int Order(string categoryId) => Names.TryGetValue(
            categoryId,
            out (int Order, string Name) value)
                ? value.Order
                : throw new ArgumentException(
                    "A cluster-resource category is unsupported.",
                    nameof(categoryId));

        private static string FormatUnipolar(UnipolarMagnetPlanetStatistics planet) =>
            planet.Location.DisplayDesignation + " - " +
            planet.Location.FormattedDistance + " - " +
            Count(planet.VeinNodes, "vein", "veins") + " - " +
            Count(planet.Amount, "magnet", "magnets") + " - " +
            Count(planet.VeinGroups, "group", "groups");

        private static string FormatLocation(ClusterBodyLocation location) =>
            location.DisplayDesignation + " · " + location.FormattedDistance;

        private static string Count(long value, string singular, string plural) =>
            value.ToString("N0", CultureInfo.InvariantCulture) + " " +
            (value == 1 ? singular : plural);
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
        public const int MaximumItems = 512;
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
            HomeSystemResourceStatistics? homeSystemResources,
            ClusterResourceStatistics? clusterResources,
            NearbyDeuteriumGasGiantSelection? nearbyDeuteriumGasGiant,
            PreviewClusterStatistics cluster)
        {
            if (sessionId <= 0)
                throw new ArgumentOutOfRangeException(nameof(sessionId));
            if (String.IsNullOrWhiteSpace(identityLine))
                throw new ArgumentException("Identity line is required.", nameof(identityLine));
            SessionId = sessionId;
            IdentityLine = identityLine;
            HomeSystem = homeSystem;
            HomeSystemResources = homeSystemResources;
            RareResourceRows = clusterResources == null
                ? Array.Empty<ClusterRareResourceTableRow>()
                : ClusterResourcePresentation.ProjectRareResourceTableRows(
                    clusterResources);
            UnipolarMagnetRows = clusterResources == null
                ? Array.Empty<ClusterUnipolarMagnetTableRow>()
                : ClusterResourcePresentation.ProjectUnipolarTableRows(
                    clusterResources);
            NearbyDeuteriumRow = nearbyDeuteriumGasGiant?.ProjectTableRow();
            Cluster = cluster ?? throw new ArgumentNullException(nameof(cluster));
        }

        public long SessionId { get; }
        public string IdentityLine { get; }
        public HomeSystemBodyInventory? HomeSystem { get; }
        public HomeSystemResourceStatistics? HomeSystemResources { get; }
        public IReadOnlyList<ClusterRareResourceTableRow> RareResourceRows { get; }
        public IReadOnlyList<ClusterUnipolarMagnetTableRow> UnipolarMagnetRows { get; }
        public NearbyDeuteriumTableRow? NearbyDeuteriumRow { get; }
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
                null,
                null,
                null,
                new PreviewClusterStatistics());
        }

        public bool Update(PreviewResolutionAttempt attempt)
        {
            if (attempt == null)
                throw new ArgumentNullException(nameof(attempt));
            if (attempt.Session.SessionId != activeSessionId || attempt.Session.IsRetired)
                return false;
            PreviewClusterStatistics cluster = attempt.ClusterResources == null
                ? Current?.Cluster ?? new PreviewClusterStatistics()
                : ClusterResourcePresentation.Project(attempt.ClusterResources);
            if (attempt.NearbyDeuteriumGasGiant != null)
                cluster = attempt.NearbyDeuteriumGasGiant.Apply(cluster);
            Current = new PreviewStatisticsDocument(
                activeSessionId,
                PreviewIdentityPresentation.Format(
                    attempt.Session.Identity,
                    attempt.Session.HomePlanetDisplayDesignation),
                attempt.HomeSystemBodyInventory,
                attempt.HomeSystemResources,
                attempt.ClusterResources,
                attempt.NearbyDeuteriumGasGiant,
                cluster);
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
