using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading;
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

        public DspPreviewGateway(int mainThreadId, string ownPluginGuid)
        {
            MainThreadId = mainThreadId;
            this.ownPluginGuid = ownPluginGuid;
        }

        public int MainThreadId { get; }

        public RuntimeFingerprint CaptureFingerprint(PreviewScanRequest request)
        {
            string? missingMember = FindMissingMember();
            GameDesc descriptor = CreateDescriptor(request);
            string[] themes = descriptor.savedThemeIds
                .Select(value => value.ToString(CultureInfo.InvariantCulture))
                .ToArray();
            string[] otherPlugins = Chainloader.PluginInfos.Keys
                .Where(id => !String.Equals(id, ownPluginGuid, StringComparison.Ordinal))
                .OrderBy(id => id, StringComparer.Ordinal)
                .ToArray();

            return new RuntimeFingerprint(
                GameConfig.gameVersion.ToFullString(),
                UniverseGen.algoVersion,
                HashAssembly(typeof(UniverseGen).Assembly.Location),
                themes,
                ConclusionDefinition.DefinitionVersion,
                ConclusionDefinition.ContractVersion,
                missingMember == null,
                missingMember,
                otherPlugins);
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
            PlanetData birthPlanet = galaxy.PlanetById(galaxy.birthPlanetId);
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

            NormalizedSystemEvidence[] systems = galaxy.stars
                .Select(star => NormalizeSystem(
                    request.GalaxySeed,
                    star,
                    birthStar,
                    birthPlanet))
                .ToArray();
            NormalizedSystemDistance[] distances = NormalizeDistances(
                request.GalaxySeed,
                galaxy.birthStarId,
                galaxy.stars);
            recordTrace("preview:normalized");
            return new RuntimePreviewSnapshot(
                SystemIdentifier(request.GalaxySeed, birthStar, true),
                galaxy.starCount,
                systems,
                distances);
        }

        private static NormalizedSystemEvidence NormalizeSystem(
            int seed,
            StarData star,
            StarData birthStar,
            PlanetData birthPlanet)
        {
            bool isBirth = star.id == birthStar.id;
            int sharedBodies = 0;
            bool tidal = false;
            bool hasSolid = false;
            decimal maximumSolar = 0m;
            decimal maximumWind = 0m;
            var gasRates = new SortedDictionary<string, decimal>(StringComparer.Ordinal);
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
                    if (isBirth && birthPlanet.orbitAround > 0 &&
                        planet.orbitAround == birthPlanet.orbitAround)
                    {
                        sharedBodies++;
                    }
                }
                if (planet.orbitRadius * 40_000f <= maximumShellRadius)
                    containedOrbits++;

                if (planet.gasItems == null)
                    continue;
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
            }

            return new NormalizedSystemEvidence(
                new ConclusionSubject(
                    isBirth ? SubjectKind.BirthSystem : SubjectKind.StarSystem,
                    SystemIdentifier(seed, star, isBirth)),
                isBirth,
                isBirth ? sharedBodies : null,
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
                star.initialHiveCount);
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

        private static string SystemIdentifier(int seed, StarData star, bool isBirth)
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
                (typeof(GalaxyData), "PlanetById", MemberTypes.Method),
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
                (typeof(PlanetData), "orbitRadius", MemberTypes.Field),
                (typeof(PlanetData), "singularity", MemberTypes.Field),
                (typeof(PlanetData), "luminosity", MemberTypes.Field),
                (typeof(PlanetData), "windStrength", MemberTypes.Field),
                (typeof(PlanetData), "gasItems", MemberTypes.Field),
                (typeof(PlanetData), "gasSpeeds", MemberTypes.Field),
                (typeof(PlanetData), "type", MemberTypes.Field)
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

        private static string HashAssembly(string path)
        {
            using (FileStream stream = File.OpenRead(path))
            using (SHA256 hash = SHA256.Create())
                return BitConverter.ToString(hash.ComputeHash(stream)).Replace("-", String.Empty);
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
