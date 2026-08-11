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

namespace DSPSeedScanner.Plugin
{
    internal sealed class DspPreviewGateway : IRuntimePreviewGateway
    {
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

        public RuntimeTopologySnapshot GeneratePreview(
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
                recordTrace("topology:extract");
                StarData birthStar = galaxy.StarById(galaxy.birthStarId);
                PlanetData birthPlanet = galaxy.PlanetById(galaxy.birthPlanetId);
                int sharedBodies = 0;
                foreach (PlanetData planet in birthStar.planets)
                {
                    int rawType = (int)planet.type;
                    if (!Enum.IsDefined(typeof(EPlanetType), planet.type))
                    {
                        return new RuntimeTopologySnapshot(
                            birthStar.id.ToString(CultureInfo.InvariantCulture),
                            1,
                            galaxy.starCount,
                            nameof(EPlanetType),
                            rawType);
                    }
                    if (birthPlanet.orbitAround > 0 &&
                        planet.orbitAround == birthPlanet.orbitAround)
                    {
                        sharedBodies++;
                    }
                }

                return new RuntimeTopologySnapshot(
                    birthStar.id.ToString(CultureInfo.InvariantCulture),
                    sharedBodies,
                    galaxy.starCount);
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

        private static GameDesc CreateDescriptor(PreviewScanRequest request)
        {
            var descriptor = new GameDesc();
            descriptor.SetForNewGame(
                UniverseGen.algoVersion,
                request.GalaxySeed,
                request.RequestedStarCount,
                1,
                (float)request.ResourceMultiplier);
            descriptor.isPeaceMode = request.CombatMode == CombatMode.Peace;
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
                (typeof(GameDesc), "savedThemeIds", MemberTypes.Field),
                (typeof(StarData), "planets", MemberTypes.Field),
                (typeof(PlanetData), "orbitAround", MemberTypes.Field),
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
