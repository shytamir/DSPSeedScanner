using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using BepInEx;
using BepInEx.Bootstrap;
using DSPSeedScanner.Core;
using DSPSeedScanner.Runtime;
using UnityEngine;

namespace DSPSeedScanner.Plugin
{
    internal sealed class DspPreviewGateway : IRuntimePreviewGateway
    {
        private static readonly IReadOnlyDictionary<int, string> GasProductIds =
            new Dictionary<int, string>
            {
                { 1011, "fire-ice" },
                { 1120, "hydrogen" },
                { 1121, "deuterium" }
            };

        private readonly string ownPluginGuid;
        private readonly RuntimeFilesystemResolution filesystemResolution;
        private readonly Action<string> reportFilesystemFailure;

        public DspPreviewGateway(
            int mainThreadId,
            string ownPluginGuid,
            RuntimeFilesystemResolution filesystemResolution,
            Action<string> reportFilesystemFailure)
        {
            MainThreadId = mainThreadId;
            this.ownPluginGuid = ownPluginGuid;
            this.filesystemResolution = filesystemResolution ??
                throw new ArgumentNullException(nameof(filesystemResolution));
            this.reportFilesystemFailure = reportFilesystemFailure ??
                throw new ArgumentNullException(nameof(reportFilesystemFailure));
        }

        public int MainThreadId { get; }

        internal Action? AfterGalaxyCreatedForProbe { get; set; }

        public RuntimeFingerprint CaptureFingerprint(PreviewScanRequest request)
        {
            RuntimeFilesystemContext filesystem = filesystemResolution.Context ??
                throw filesystemResolution.ToException();
            string? missingMember = FindMissingMember();
            GameDesc descriptor = CreateDescriptor(request);
            string[] themes = descriptor.savedThemeIds
                .Select(value => value.ToString(CultureInfo.InvariantCulture))
                .ToArray();
            string[] otherPlugins = Chainloader.PluginInfos
                .Where(pair => !String.Equals(pair.Key, ownPluginGuid, StringComparison.Ordinal))
                .Select(pair => pair.Key + "@" + pair.Value.Metadata.Version)
                .OrderBy(value => value, StringComparer.Ordinal)
                .ToArray();
            string[] patchers = CapturePatcherInventory(filesystem);
            string methodHash = CaptureGenerationMethodHash();

            return new RuntimeFingerprint(
                GameConfig.gameVersion.ToFullString(),
                UniverseGen.algoVersion,
                RuntimeFileFingerprint.RequiredSha256(
                    filesystem.ManagedAssemblyPath,
                    "active-managed-assembly"),
                themes,
                ConclusionDefinition.DefinitionVersion,
                ConclusionDefinition.ContractVersion,
                missingMember == null,
                missingMember,
                otherPlugins,
                methodHash,
                patchers);
        }

        public RuntimeStateLease CaptureState()
        {
            return new DspStateLease(GameMain.data, DSPGame.GameDesc);
        }

        public RuntimePreviewSnapshot GeneratePreview(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<string> recordTrace)
        {
            cancellationToken.ThrowIfCancellationRequested();
            GameDesc descriptor = CreateDescriptor(request);
            GalaxyData? galaxy = null;
            try
            {
                recordTrace("UniverseGen.CreateGalaxy:thread=" + Thread.CurrentThread.ManagedThreadId);
                galaxy = UniverseGen.CreateGalaxy(descriptor);
                AfterGalaxyCreatedForProbe?.Invoke();
                cancellationToken.ThrowIfCancellationRequested();
                return NormalizePreview(request, galaxy, recordTrace);
            }
            finally
            {
                if (galaxy != null)
                {
                    galaxy.Free();
                    recordTrace("GalaxyData.Free");
                }
            }
        }

        internal static RuntimePreviewSnapshot NormalizePreview(
            PreviewScanRequest request,
            GalaxyData galaxy,
            Action<string> recordTrace)
        {
            recordTrace("preview:extract");
            StarData birthStar = galaxy.StarById(galaxy.birthStarId);
            foreach (StarData star in galaxy.stars)
            {
                foreach (PlanetData planet in star.planets)
                {
                    int rawType = (int)planet.type;
                    if (!Enum.IsDefined(typeof(EPlanetType), planet.type))
                    {
                        return new RuntimePreviewSnapshot(
                            SystemIdentifier(request.GalaxySeed, birthStar, true),
                            galaxy.starCount,
                            Array.Empty<NormalizedSystemEvidence>(),
                            Array.Empty<NormalizedSystemDistance>(),
                            nameof(EPlanetType),
                            rawType);
                    }
                }
            }

            string birthSystemIdentifier = SystemIdentifier(
                request.GalaxySeed,
                birthStar,
                true);
            var generatedPlanets = new HashSet<PlanetData>(
                galaxy.stars.SelectMany(star => star.planets));
            PlanetData? homePlanet = birthStar.planets.SingleOrDefault(planet =>
                planet.id == galaxy.birthPlanetId);
            IReadOnlyDictionary<int, ThemeProto> themesById = LDB.themes.dataArray
                .Where(theme => theme != null)
                .ToDictionary(theme => theme.ID);
            HomeSystemBodyInventory? homeSystemBodyInventory =
                HomeSystemBodyInventory.Project(
                    birthSystemIdentifier,
                    birthStar.planets.Select((planet, stableGameOrder) =>
                        new RuntimeHomeSystemBodyEvidence(
                            planet.id,
                            planet.displayName,
                            planet.number,
                            planet.orbitAround,
                            planet.orbitAroundPlanet != null &&
                                generatedPlanets.Contains(planet.orbitAroundPlanet)
                                ? planet.orbitAroundPlanet.id
                                : null,
                            stableGameOrder,
                            HomeBodyKind(planet),
                            HomeThemeName(planet, themesById),
                            HomeEnergyRatio(planet, planet.luminosity),
                            HomeEnergyRatio(planet, planet.windStrength),
                            planet.type == EPlanetType.Gas && planet.gasItems != null
                                ? planet.gasItems.Select(NormalizeGasProductId)
                                    .Where(ResourcePresentation.Supports)
                                : null)));
            NormalizedHomePlanetTopology? homePlanetTopology =
                PreviewHomeTopologyNormalizer.Normalize(
                    birthSystemIdentifier,
                    galaxy.birthPlanetId,
                    galaxy.stars.SelectMany(star => star.planets.Select(planet =>
                        new RuntimePlanetOrbitEvidence(
                            planet.id,
                            SystemIdentifier(
                                request.GalaxySeed,
                                star,
                                star.id == galaxy.birthStarId),
                            planet.number,
                            planet.type != EPlanetType.Gas,
                            planet.type == EPlanetType.Gas,
                            planet.orbitAround,
                            planet.orbitAroundPlanet != null &&
                                generatedPlanets.Contains(planet.orbitAroundPlanet)
                                ? planet.orbitAroundPlanet.id
                                : null))));

            NormalizedSystemEvidence[] systems = galaxy.stars
                .Select(star => NormalizeSystem(
                    request.GalaxySeed,
                    star,
                    birthStar,
                    homePlanetTopology))
                .ToArray();
            NormalizedSystemDistance[] distances = NormalizeDistances(
                request.GalaxySeed,
                galaxy.birthStarId,
                galaxy.stars);
            RuntimeSystemDisplay[] displays = galaxy.stars
                .Select(star => new RuntimeSystemDisplay(
                    SystemIdentifier(
                        request.GalaxySeed,
                        star,
                        star.id == galaxy.birthStarId),
                    star.displayName,
                    star.typeString))
                .ToArray();
            NearbyDeuteriumGasGiantSelection nearbyDeuteriumGasGiant =
                NormalizeNearbyDeuteriumGasGiant(
                    request.GalaxySeed,
                    birthStar,
                    galaxy.stars,
                    distances);
            recordTrace("preview:normalized");
            return new RuntimePreviewSnapshot(
                birthSystemIdentifier,
                galaxy.starCount,
                systems,
                distances,
                systemDisplays: displays,
                homeSystemBodyInventory: homeSystemBodyInventory,
                homePlanetDisplayDesignation: homePlanet?.displayName,
                nearbyDeuteriumGasGiant: nearbyDeuteriumGasGiant);
        }

        private static NearbyDeuteriumGasGiantSelection
            NormalizeNearbyDeuteriumGasGiant(
                int seed,
                StarData birthStar,
                StarData[] stars,
                IReadOnlyList<NormalizedSystemDistance> distances)
        {
            NearbyDeuteriumGasGiantCandidate? winner = null;
            bool complete = true;
            int stableGameOrder = 0;
            foreach (StarData star in stars)
            {
                bool isBirth = star.id == birthStar.id;
                string systemIdentifier = SystemIdentifier(seed, star, isBirth);
                decimal distance = isBirth
                    ? 0m
                    : distances.Single(item => item.Connects(
                        SystemIdentifier(seed, birthStar, true),
                        systemIdentifier)).LightYears;
                foreach (PlanetData planet in star.planets)
                {
                    int order = stableGameOrder++;
                    if (planet.type != EPlanetType.Gas)
                        continue;
                    if (planet.gasItems == null || planet.gasSpeeds == null ||
                        planet.gasItems.Length != planet.gasSpeeds.Length ||
                        String.IsNullOrWhiteSpace(planet.displayName))
                    {
                        complete = false;
                        continue;
                    }
                    for (int index = 0; index < planet.gasItems.Length; index++)
                    {
                        if (planet.gasItems[index] != 1121)
                            continue;
                        decimal rate = Convert.ToDecimal(planet.gasSpeeds[index]);
                        var candidate = new NearbyDeuteriumGasGiantCandidate(
                            new ClusterBodyLocation(
                                planet.id.ToString(CultureInfo.InvariantCulture),
                                planet.displayName,
                                systemIdentifier,
                                distance,
                                order),
                            rate);
                        winner = NearbyDeuteriumGasGiantSelection.Prefer(
                            winner,
                            candidate);
                        break;
                    }
                }
            }
            return NearbyDeuteriumGasGiantSelection.FromWinner(winner, complete);
        }

        private static HomeSystemBodyKind HomeBodyKind(PlanetData planet)
        {
            if (planet.type != EPlanetType.Gas)
                return HomeSystemBodyKind.Solid;
            return planet.iceFlag > 0
                ? HomeSystemBodyKind.IceGiant
                : HomeSystemBodyKind.GasGiant;
        }

        private static string? HomeThemeName(
            PlanetData planet,
            IReadOnlyDictionary<int, ThemeProto> themesById)
        {
            if (planet.type == EPlanetType.Gas ||
                !themesById.TryGetValue(planet.theme, out ThemeProto? theme) ||
                String.IsNullOrWhiteSpace(theme.displayName))
            {
                return null;
            }
            return theme.displayName;
        }

        private static decimal? HomeEnergyRatio(PlanetData planet, float ratio)
        {
            if (planet.type == EPlanetType.Gas ||
                Single.IsNaN(ratio) ||
                Single.IsInfinity(ratio) ||
                ratio < 0f)
            {
                return null;
            }
            return Convert.ToDecimal(ratio);
        }

        private static NormalizedSystemEvidence NormalizeSystem(
            int seed,
            StarData star,
            StarData birthStar,
            NormalizedHomePlanetTopology? homePlanetTopology)
        {
            bool isBirth = star.id == birthStar.id;
            int? sharedBodies = isBirth
                ? homePlanetTopology?.OrbitKind == HomePlanetOrbitKind.DirectStar
                    ? 1
                    : homePlanetTopology?.HomeGiantMoonCount
                : null;
            bool tidal = false;
            bool hasSolid = false;
            decimal maximumSolar = 0m;
            decimal maximumWind = 0m;
            var gasRates = new SortedDictionary<string, decimal>(StringComparer.Ordinal);
            var birthPlanets = isBirth
                ? new List<NormalizedBirthPlanetEvidence>()
                : null;
            bool birthPlanetAttributionComplete = isBirth;
            long maximumShellRadius = MaximumShellRadius(star);
            int containedOrbits = 0;

            foreach (PlanetData planet in star.planets)
            {
                bool solid = planet.type != EPlanetType.Gas;
                if (solid)
                {
                    hasSolid = true;
                    maximumSolar = Math.Max(maximumSolar, Convert.ToDecimal(planet.luminosity));
                    maximumWind = Math.Max(maximumWind, Convert.ToDecimal(planet.windStrength));
                    tidal |= (planet.singularity & EPlanetSingularity.TidalLocked) != 0;
                    birthPlanets?.Add(new NormalizedBirthPlanetEvidence(
                        planet.id,
                        planet.displayName,
                        false,
                        Convert.ToDecimal(planet.luminosity),
                        Convert.ToDecimal(planet.windStrength),
                        (planet.singularity & EPlanetSingularity.TidalLocked) != 0,
                        null));
                }
                if (planet.orbitRadius * 40_000f <= maximumShellRadius)
                    containedOrbits++;

                if (planet.gasItems == null)
                {
                    if (!solid)
                        birthPlanetAttributionComplete = false;
                    continue;
                }
                if (planet.gasSpeeds == null || planet.gasItems.Length != planet.gasSpeeds.Length)
                    throw new InvalidOperationException("Gas item and rate arrays do not match.");
                for (int index = 0; index < planet.gasItems.Length; index++)
                {
                    string productId = NormalizeGasProductId(planet.gasItems[index]);
                    decimal rate = Convert.ToDecimal(planet.gasSpeeds[index]);
                    gasRates[productId] = gasRates.TryGetValue(productId, out decimal current)
                        ? current + rate
                        : rate;
                }
                if (!solid)
                {
                    birthPlanets?.Add(new NormalizedBirthPlanetEvidence(
                        planet.id,
                        planet.displayName,
                        true,
                        null,
                        null,
                        null,
                        planet.gasItems.Select(NormalizeGasProductId)
                            .Distinct(StringComparer.Ordinal)));
                }
            }

            return new NormalizedSystemEvidence(
                new ConclusionSubject(
                    isBirth ? SubjectKind.BirthSystem : SubjectKind.StarSystem,
                    SystemIdentifier(seed, star, isBirth)),
                isBirth,
                sharedBodies,
                isBirth ? tidal : null,
                isBirth && hasSolid ? maximumSolar : null,
                isBirth && hasSolid ? maximumWind : null,
                isBirth
                    ? gasRates.Select(pair => new NormalizedGasProduct(pair.Key, pair.Value))
                    : null,
                Decimal.Round(
                    (decimal)(double)star.dysonLumino,
                    10,
                    MidpointRounding.AwayFromZero),
                maximumShellRadius,
                containedOrbits,
                star.initialHiveCount,
                birthPlanetAttributionComplete ? birthPlanets : null,
                isBirth ? homePlanetTopology : null);
        }

        private static NormalizedSystemDistance[] NormalizeDistances(
            int seed,
            int birthStarId,
            StarData[] stars)
        {
            var distances = new List<NormalizedSystemDistance>();
            for (int first = 0; first < stars.Length; first++)
            {
                for (int second = first + 1; second < stars.Length; second++)
                {
                    double x = stars[first].uPosition.x - stars[second].uPosition.x;
                    double y = stars[first].uPosition.y - stars[second].uPosition.y;
                    double z = stars[first].uPosition.z - stars[second].uPosition.z;
                    double lightYears = Math.Sqrt(x * x + y * y + z * z) / GalaxyData.LY;
                    distances.Add(new NormalizedSystemDistance(
                        SystemIdentifier(seed, stars[first], stars[first].id == birthStarId),
                        SystemIdentifier(seed, stars[second], stars[second].id == birthStarId),
                        Convert.ToDecimal(lightYears)));
                }
            }
            return distances.ToArray();
        }

        private static long MaximumShellRadius(StarData star)
        {
            return Convert.ToInt64(
                Mathf.Round(star.dysonRadius * 40_000f * 2f / 100f) * 100f);
        }

        private static string NormalizeGasProductId(int itemId)
        {
            return GasProductIds.TryGetValue(itemId, out string? productId)
                ? productId
                : "item-" + itemId.ToString(CultureInfo.InvariantCulture);
        }

        internal static string SystemIdentifier(int seed, StarData star, bool isBirth)
        {
            return seed.ToString(CultureInfo.InvariantCulture) + ":star:" +
                star.id.ToString(CultureInfo.InvariantCulture) +
                (isBirth ? ":birth" : String.Empty);
        }

        internal static GameDesc CreateDescriptor(PreviewScanRequest request)
        {
            var descriptor = new GameDesc();
            descriptor.SetForNewGame(
                UniverseGen.algoVersion,
                request.GalaxySeed,
                request.RequestedStarCount,
                1,
                (float)request.ResourceMultiplier);
            descriptor.isPeaceMode = request.CombatMode == CombatMode.Peace;
            descriptor.combatSettings.initialColonize = (float)request.InitialColonize;
            descriptor.combatSettings.maxDensity = (float)request.MaxDensity;
            return descriptor;
        }

        private static string? FindMissingMember()
        {
            var required = new (Type Type, string Member, MemberTypes Kind)[]
            {
                (typeof(GameDesc), "SetForNewGame", MemberTypes.Method),
                (typeof(UniverseGen), "CreateGalaxy", MemberTypes.Method),
                (typeof(GalaxyData), "Free", MemberTypes.Method),
                (typeof(GalaxyData), "StarById", MemberTypes.Method),
                (typeof(GalaxyData), "stars", MemberTypes.Field),
                (typeof(GalaxyData), "starCount", MemberTypes.Field),
                (typeof(GameDesc), "savedThemeIds", MemberTypes.Field),
                (typeof(CombatSettings), "initialColonize", MemberTypes.Field),
                (typeof(CombatSettings), "maxDensity", MemberTypes.Field),
                (typeof(StarData), "planets", MemberTypes.Field),
                (typeof(StarData), "dysonLumino", MemberTypes.Property),
                (typeof(StarData), "uPosition", MemberTypes.Field),
                (typeof(StarData), "initialHiveCount", MemberTypes.Field),
                (typeof(PlanetData), "orbitAround", MemberTypes.Field),
                (typeof(PlanetData), "orbitAroundPlanet", MemberTypes.Field),
                (typeof(PlanetData), "number", MemberTypes.Field),
                (typeof(PlanetData), "displayName", MemberTypes.Property),
                (typeof(PlanetData), "orbitRadius", MemberTypes.Field),
                (typeof(PlanetData), "singularity", MemberTypes.Field),
                (typeof(PlanetData), "luminosity", MemberTypes.Field),
                (typeof(PlanetData), "windStrength", MemberTypes.Field),
                (typeof(PlanetData), "theme", MemberTypes.Field),
                (typeof(PlanetData), "waterItemId", MemberTypes.Field),
                (typeof(PlanetData), "iceFlag", MemberTypes.Field),
                (typeof(PlanetData), "gasItems", MemberTypes.Field),
                (typeof(PlanetData), "gasSpeeds", MemberTypes.Field),
                (typeof(PlanetData), "type", MemberTypes.Field),
                (typeof(ThemeProto), "displayName", MemberTypes.Property)
            };
            foreach ((Type type, string member, MemberTypes kind) in required)
            {
                MemberInfo[] matches = type.GetMember(
                    member,
                    kind,
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static);
                if (matches.Length == 0)
                    return type.FullName + "." + member;
            }
            return null;
        }

        private string[] CapturePatcherInventory(RuntimeFilesystemContext filesystem)
        {
            if (filesystem.PatcherDirectoryPath == null)
                return new[] { "inventory:" + RuntimeFileFingerprint.Unavailable };
            return RuntimeFileFingerprint.Inventory(
                    filesystem.PatcherDirectoryPath,
                    "*.dll",
                    reportFilesystemFailure,
                    "active-patchers")
                .ToArray();
        }

        private static string CaptureGenerationMethodHash()
        {
            MethodInfo? preview = typeof(UniverseGen).GetMethod(
                "CreateGalaxy",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static,
                null,
                new[] { typeof(GameDesc) },
                null);
            MethodInfo? raw = typeof(PlanetData).GetMethod(
                "RegenerateRawDataImmediately",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance,
                null,
                Type.EmptyTypes,
                null);
            if (preview?.GetMethodBody() == null || raw?.GetMethodBody() == null)
                return "unavailable";

            var bytes = new List<byte>();
            AddMethod(bytes, preview);
            AddMethod(bytes, raw);
            using (System.Security.Cryptography.SHA256 hash =
                System.Security.Cryptography.SHA256.Create())
            {
                return BitConverter.ToString(hash.ComputeHash(bytes.ToArray()))
                    .Replace("-", String.Empty);
            }
        }

        private static void AddMethod(ICollection<byte> destination, MethodInfo method)
        {
            string identity = method.DeclaringType?.FullName + "." + method.Name + "\n";
            foreach (byte value in Encoding.UTF8.GetBytes(identity))
                destination.Add(value);
            foreach (byte value in method.GetMethodBody()?.GetILAsByteArray() ?? Array.Empty<byte>())
                destination.Add(value);
        }

        private sealed class DspStateLease : RuntimeStateLease
        {
            private static readonly string[] TrackedGameDataFields =
            {
                "gameDesc",
                "galaxy",
                "factories",
                "factoryCount",
                "history",
                "statistics",
                "mainPlayer"
            };

            private readonly GameData? gameData;
            private readonly GameDesc? gameDesc;
            private readonly Dictionary<FieldInfo, object?> gameDataValues;
            private bool restored;

            public DspStateLease(GameData? gameData, GameDesc? gameDesc)
            {
                this.gameData = gameData;
                this.gameDesc = gameDesc;
                gameDataValues = new Dictionary<FieldInfo, object?>();
                if (gameData != null)
                {
                    foreach (string name in TrackedGameDataFields)
                    {
                        FieldInfo? field = typeof(GameData).GetField(
                            name,
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                        if (field != null)
                            gameDataValues.Add(field, field.GetValue(gameData));
                    }
                }
            }

            public override bool Restored => restored;

            public override void Dispose()
            {
                GameMain.data = gameData;
                DSPGame.GameDesc = gameDesc;
                if (gameData != null)
                {
                    foreach (KeyValuePair<FieldInfo, object?> pair in gameDataValues)
                        pair.Key.SetValue(gameData, pair.Value);
                }
                restored = ReferenceEquals(GameMain.data, gameData) &&
                    ReferenceEquals(DSPGame.GameDesc, gameDesc) &&
                    gameDataValues.All(pair => ValuesEqual(pair.Key.GetValue(gameData), pair.Value));
            }

            private static bool ValuesEqual(object? current, object? expected)
            {
                if (current == null || expected == null)
                    return current == expected;
                Type type = current.GetType();
                return type.IsValueType
                    ? current.Equals(expected)
                    : ReferenceEquals(current, expected);
            }
        }
    }
}
