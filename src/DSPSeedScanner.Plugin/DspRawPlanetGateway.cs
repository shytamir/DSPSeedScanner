using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using DSPSeedScanner.Core;
using DSPSeedScanner.Runtime;

namespace DSPSeedScanner.Plugin
{
    internal sealed class DspRawPlanetGateway : IRuntimeBirthSystemRawGateway
    {
        private static readonly IReadOnlyDictionary<int, string> ResourceIds =
            new Dictionary<int, string>
            {
                { (int)EVeinType.Iron, "iron" },
                { (int)EVeinType.Copper, "copper" },
                { (int)EVeinType.Silicium, "silicon" },
                { (int)EVeinType.Titanium, "titanium" },
                { (int)EVeinType.Stone, "stone" },
                { (int)EVeinType.Coal, "coal" },
                { (int)EVeinType.Oil, "oil" },
                { (int)EVeinType.Fireice, "fire-ice" },
                { (int)EVeinType.Diamond, "kimberlite" },
                { (int)EVeinType.Fractal, "fractal-silicon" },
                { (int)EVeinType.Crysrub, "organic-crystal" },
                { (int)EVeinType.Grat, "optical-grating-crystal" },
                { (int)EVeinType.Bamboo, "spiniform-stalagmite-crystal" },
                { (int)EVeinType.Mag, "unipolar-magnet" }
            };
        private static readonly (Type Type, string Member)[] PreparationStaticFields =
        {
            (typeof(RandomTable), "sphericNormal"),
            (typeof(RandomTable), "sphericInside"),
            (typeof(RandomTable), "integers"),
            (typeof(PlanetModelingManager), "vegeHps"),
            (typeof(PlanetModelingManager), "vegeScaleRanges"),
            (typeof(PlanetModelingManager), "vegeProtos"),
            (typeof(PlanetModelingManager), "veinProducts"),
            (typeof(PlanetModelingManager), "veinModelIndexs"),
            (typeof(PlanetModelingManager), "veinModelCounts"),
            (typeof(PlanetModelingManager), "veinProtos")
        };

        private readonly DspPreviewGateway sharedGateway;

        public DspRawPlanetGateway(DspPreviewGateway sharedGateway)
        {
            this.sharedGateway = sharedGateway ?? throw new ArgumentNullException(nameof(sharedGateway));
        }

        public int MainThreadId => sharedGateway.MainThreadId;

        internal Action? AtomicCompletedForProbe { get; set; }

        public RuntimeFingerprint CaptureFingerprint(PreviewScanRequest request) =>
            sharedGateway.CaptureFingerprint(request);

        public RuntimeStateLease CaptureState()
        {
            KeyValuePair<FieldInfo, object?>[] preparationState =
                CapturePreparationStaticState();
            return new DspRawStateLease(
                sharedGateway.CaptureState(),
                preparationState);
        }

        public IReadOnlyList<int> ReachableSolidAlgorithmIds(PreviewScanRequest request)
        {
            var allowedThemes = new HashSet<int>(
                DspPreviewGateway.CreateDescriptor(request).savedThemeIds);
            return LDB.themes.dataArray
                .Where(theme => allowedThemes.Contains(theme.ID) &&
                    theme.PlanetType != EPlanetType.Gas)
                .SelectMany(theme => theme.Algos ?? Array.Empty<int>())
                .Where(algorithm => algorithm > 0)
                .Distinct()
                .OrderBy(algorithm => algorithm)
                .ToArray();
        }

        public IReadOnlyDictionary<int, int> DiscoverCandidatePlanets(PreviewScanRequest request)
        {
            GameDesc descriptor = DspPreviewGateway.CreateDescriptor(request);
            GalaxyData? galaxy = null;
            try
            {
                galaxy = UniverseGen.CreateGalaxy(descriptor);
                return galaxy.stars
                    .SelectMany(star => star.planets)
                    .Where(planet => planet.type != EPlanetType.Gas && planet.algoId > 0)
                    .GroupBy(planet => planet.algoId)
                    .ToDictionary(group => group.Key, group => group.First().id);
            }
            finally
            {
                galaxy?.Free();
            }
        }

        public BirthSystemRawPlan DiscoverBirthSystem(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<string> recordTrace)
        {
            cancellationToken.ThrowIfCancellationRequested();
            GameDesc descriptor = DspPreviewGateway.CreateDescriptor(request);
            GalaxyData? galaxy = null;
            try
            {
                recordTrace("UniverseGen.CreateGalaxy:birth-plan:thread=" +
                    Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));
                galaxy = UniverseGen.CreateGalaxy(descriptor);
                RuntimePreviewSnapshot preview = DspPreviewGateway.NormalizePreview(
                    request,
                    galaxy,
                    recordTrace);
                StarData birthStar = galaxy.StarById(galaxy.birthStarId);
                BirthSystemPlanetTarget[] targets = birthStar.planets
                    .Where(planet => planet.type != EPlanetType.Gas)
                    .Select(planet => new BirthSystemPlanetTarget(planet.id, planet.algoId))
                    .OrderBy(target => target.PlanetId)
                    .ToArray();
                recordTrace("birth-plan:declared=" +
                    targets.Length.ToString(CultureInfo.InvariantCulture));
                return new BirthSystemRawPlan(preview, targets);
            }
            finally
            {
                if (galaxy != null)
                {
                    galaxy.Free();
                    recordTrace("birth-plan:candidate:released");
                }
            }
        }

        public NormalizedRawPlanetEvidence GenerateRawPlanet(
            RawPlanetRequest request,
            CancellationToken cancellationToken,
            Action<string> recordTrace)
        {
            string? missingMember = FindMissingRawMember();
            if (missingMember != null)
            {
                throw new RawCompatibilityException(
                    "missing-raw-runtime-member",
                    "A required raw-generation member is unavailable.",
                    missingMember);
            }

            cancellationToken.ThrowIfCancellationRequested();
            GameDesc descriptor = DspPreviewGateway.CreateDescriptor(request.Identity);
            GalaxyData? galaxy = null;
            try
            {
                recordTrace("UniverseGen.CreateGalaxy:thread=" + Thread.CurrentThread.ManagedThreadId);
                galaxy = UniverseGen.CreateGalaxy(descriptor);
                PlanetData planet = galaxy.PlanetById(request.PlanetId);
                if (planet == null)
                    throw new RawCompatibilityException("raw-target-missing", "The requested candidate planet was not generated.");
                if (planet.type == EPlanetType.Gas)
                    throw new RawCompatibilityException("raw-target-gas", "Gas giants do not use the supported solid raw path.");
                if (planet.algoId != request.ExpectedAlgorithmId)
                {
                    throw new RawCompatibilityException(
                        "raw-algorithm-mismatch",
                        "The candidate planet selected a different raw algorithm.",
                        "expected=" + request.ExpectedAlgorithmId.ToString(CultureInfo.InvariantCulture) +
                        ";actual=" + planet.algoId.ToString(CultureInfo.InvariantCulture));
                }

                var candidateData = new GameData
                {
                    gameDesc = descriptor,
                    galaxy = galaxy
                };
                GameMain.data = candidateData;
                DSPGame.GameDesc = descriptor;

                PrepareRawGeneration(recordTrace);
                cancellationToken.ThrowIfCancellationRequested();
                recordTrace("raw:atomic:start:planet=" + planet.id.ToString(CultureInfo.InvariantCulture) +
                    ":algorithm=" + planet.algoId.ToString(CultureInfo.InvariantCulture));
                planet.RegenerateRawDataImmediately();
                recordTrace("raw:atomic:complete");
                AtomicCompletedForProbe?.Invoke();
                cancellationToken.ThrowIfCancellationRequested();

                NormalizedRawPlanetEvidence evidence = Normalize(request.Identity.GalaxySeed, planet);
                recordTrace("raw:normalized:nodes=" + evidence.Nodes.Count.ToString(CultureInfo.InvariantCulture) +
                    ":groups=" + evidence.Groups.Count.ToString(CultureInfo.InvariantCulture));
                return evidence;
            }
            finally
            {
                if (galaxy != null)
                {
                    galaxy.Free();
                    recordTrace("raw:candidate:released");
                }
            }
        }

        private static void PrepareRawGeneration(Action<string> recordTrace)
        {
            MethodInfo? prepare = typeof(PlanetModelingManager).GetMethod(
                "PrepareWorks",
                BindingFlags.Static | BindingFlags.NonPublic);
            if (prepare == null)
                throw new RawCompatibilityException("missing-raw-runtime-member", "PlanetModelingManager.PrepareWorks is unavailable.");
            prepare.Invoke(null, null);
            RandomTable.Init();
            recordTrace("raw:prepare:PlanetModelingManager.PrepareWorks");
            recordTrace("raw:prepare:RandomTable.Init");
        }

        private static NormalizedRawPlanetEvidence Normalize(int galaxySeed, PlanetData planet)
        {
            PlanetRawData? data = planet.data;
            if (data == null)
                throw new InvalidOperationException("DSP returned no raw planet data.");

            var nodes = new List<NormalizedRawVeinNode>();
            for (int index = 1; index < data.veinCursor; index++)
            {
                VeinData vein = data.veinPool[index];
                int type = (int)vein.type;
                string resourceId = ResourceId(type);
                RawResourceSemantics semantics = Semantics(type);
                nodes.Add(new NormalizedRawVeinNode(
                    index,
                    vein.id,
                    type,
                    resourceId,
                    vein.productId,
                    semantics,
                    vein.amount,
                    vein.groupIndex,
                    Convert.ToDecimal(vein.pos.x),
                    Convert.ToDecimal(vein.pos.y),
                    Convert.ToDecimal(vein.pos.z),
                    semantics == RawResourceSemantics.OilFlow
                        ? Convert.ToDecimal(VeinData.oilSpeedMultiplier)
                        : null));
            }

            var groups = new List<NormalizedRawVeinGroup>();
            if (planet.veinGroups != null)
            {
                for (int index = 1; index < planet.veinGroups.Length; index++)
                {
                    VeinGroup group = planet.veinGroups[index];
                    if (group.type == EVeinType.None)
                        continue;
                    int type = (int)group.type;
                    groups.Add(new NormalizedRawVeinGroup(
                        index,
                        type,
                        ResourceId(type),
                        Semantics(type),
                        group.count,
                        group.amount,
                        Convert.ToDecimal(group.pos.x),
                        Convert.ToDecimal(group.pos.y),
                        Convert.ToDecimal(group.pos.z)));
                }
            }

            return new NormalizedRawPlanetEvidence(
                galaxySeed,
                planet.id,
                planet.theme,
                planet.algoId,
                RawPlanetCoverage.Complete(),
                nodes,
                groups);
        }

        private static string ResourceId(int type)
        {
            if (!Enum.IsDefined(typeof(EVeinType), (byte)type) ||
                type <= (int)EVeinType.None || type >= (int)EVeinType.Max ||
                !ResourceIds.TryGetValue(type, out string? resourceId))
            {
                throw new RawCompatibilityException(
                    "unknown-raw-resource-type",
                    "DSP returned a vein type outside the supported contract.",
                    "EVeinType=" + type.ToString(CultureInfo.InvariantCulture));
            }
            return resourceId;
        }

        private static RawResourceSemantics Semantics(int type) =>
            type == (int)EVeinType.Oil
                ? RawResourceSemantics.OilFlow
                : RawResourceSemantics.FiniteDeposit;

        private static string? FindMissingRawMember()
        {
            var required = new (Type Type, string Member, MemberTypes Kind)[]
            {
                (typeof(PlanetData), "RegenerateRawDataImmediately", MemberTypes.Method),
                (typeof(PlanetData), "data", MemberTypes.Field),
                (typeof(PlanetData), "veinGroups", MemberTypes.Field),
                (typeof(PlanetRawData), "Free", MemberTypes.Method),
                (typeof(PlanetRawData), "veinPool", MemberTypes.Field),
                (typeof(PlanetRawData), "veinCursor", MemberTypes.Field),
                (typeof(VeinData), "type", MemberTypes.Field),
                (typeof(VeinData), "productId", MemberTypes.Field),
                (typeof(VeinData), "amount", MemberTypes.Field),
                (typeof(VeinData), "groupIndex", MemberTypes.Field),
                (typeof(VeinData), "pos", MemberTypes.Field),
                (typeof(VeinData), "oilSpeedMultiplier", MemberTypes.Field),
                (typeof(VeinGroup), "type", MemberTypes.Field),
                (typeof(VeinGroup), "count", MemberTypes.Field),
                (typeof(VeinGroup), "amount", MemberTypes.Field),
                (typeof(VeinGroup), "pos", MemberTypes.Field),
                (typeof(RandomTable), "Init", MemberTypes.Method)
            };
            foreach ((Type type, string member, MemberTypes kind) in required)
            {
                if (type.GetMember(
                    member,
                    kind,
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static).Length == 0)
                {
                    return type.FullName + "." + member;
                }
            }
            foreach ((Type type, string member) in PreparationStaticFields)
            {
                if (FindStaticField(type, member) == null)
                    return type.FullName + "." + member;
            }
            return null;
        }

        private static KeyValuePair<FieldInfo, object?>[] CapturePreparationStaticState()
        {
            var values = new List<KeyValuePair<FieldInfo, object?>>();
            foreach ((Type type, string member) in PreparationStaticFields)
            {
                FieldInfo? field = FindStaticField(type, member);
                if (field == null)
                {
                    throw new RawCompatibilityException(
                        "missing-raw-runtime-member",
                        "A required raw preparation field is unavailable.",
                        type.FullName + "." + member);
                }
                values.Add(new KeyValuePair<FieldInfo, object?>(field, field.GetValue(null)));
            }
            return values.ToArray();
        }

        private static FieldInfo? FindStaticField(Type type, string member)
        {
            return type.GetField(
                member,
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
        }

        private sealed class DspRawStateLease : RuntimeStateLease
        {
            private readonly RuntimeStateLease sharedLease;
            private readonly KeyValuePair<FieldInfo, object?>[] preparationState;
            private bool restored;

            public DspRawStateLease(
                RuntimeStateLease sharedLease,
                KeyValuePair<FieldInfo, object?>[] preparationState)
            {
                this.sharedLease = sharedLease ??
                    throw new ArgumentNullException(nameof(sharedLease));
                this.preparationState = preparationState ??
                    throw new ArgumentNullException(nameof(preparationState));
            }

            public override bool Restored => restored;

            public override void Dispose()
            {
                Exception? failure = null;
                bool preparationRestored = true;
                foreach (KeyValuePair<FieldInfo, object?> pair in preparationState)
                {
                    try
                    {
                        pair.Key.SetValue(null, pair.Value);
                    }
                    catch (Exception exception)
                    {
                        preparationRestored = false;
                        if (failure == null)
                            failure = exception;
                    }
                }
                foreach (KeyValuePair<FieldInfo, object?> pair in preparationState)
                {
                    try
                    {
                        preparationRestored &=
                            ReferenceEquals(pair.Key.GetValue(null), pair.Value);
                    }
                    catch (Exception exception)
                    {
                        preparationRestored = false;
                        if (failure == null)
                            failure = exception;
                    }
                }

                try
                {
                    sharedLease.Dispose();
                }
                catch (Exception exception)
                {
                    if (failure == null)
                        failure = exception;
                }
                restored = preparationRestored && sharedLease.Restored;
                if (failure != null)
                {
                    throw new InvalidOperationException(
                        "Raw runtime state restoration failed.",
                        failure);
                }
            }
        }
    }
}
