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
    internal sealed class DspRawPlanetGateway : IRuntimeCompleteClusterRawGateway
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

        public CompleteClusterRawPlan DiscoverCompleteCluster(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<string> recordTrace)
        {
            cancellationToken.ThrowIfCancellationRequested();
            GameDesc descriptor = DspPreviewGateway.CreateDescriptor(request);
            GalaxyData? galaxy = null;
            try
            {
                recordTrace("UniverseGen.CreateGalaxy:cluster-plan:thread=" +
                    Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));
                galaxy = UniverseGen.CreateGalaxy(descriptor);
                RuntimePreviewSnapshot preview = DspPreviewGateway.NormalizePreview(
                    request,
                    galaxy,
                    recordTrace);
                StarData birthStar = galaxy.StarById(galaxy.birthStarId);
                var targets = new List<CompleteClusterPlanetTarget>();
                foreach (StarData star in galaxy.stars)
                {
                    bool isBirth = star.id == birthStar.id;
                    string systemId = DspPreviewGateway.SystemIdentifier(
                        request.GalaxySeed,
                        star,
                        isBirth);
                    decimal distance = isBirth
                        ? 0m
                        : preview.SystemDistances.Single(item => item.Connects(
                            preview.BirthSystemIdentifier,
                            systemId)).LightYears;
                    var subject = new ConclusionSubject(
                        isBirth ? SubjectKind.BirthSystem : SubjectKind.StarSystem,
                        systemId);
                    targets.AddRange(star.planets
                        .Where(planet => planet.type != EPlanetType.Gas)
                        .Select(planet => new CompleteClusterPlanetTarget(
                            planet.id,
                            planet.algoId,
                            subject,
                            distance)));
                }
                recordTrace("cluster-plan:declared=" +
                    targets.Count.ToString(CultureInfo.InvariantCulture));
                return new CompleteClusterRawPlan(preview, targets);
            }
            finally
            {
                if (galaxy != null)
                {
                    galaxy.Free();
                    recordTrace("cluster-plan:candidate:released");
                }
            }
        }

        public IRuntimeCompleteClusterRawSession OpenCompleteCluster(
            PreviewScanRequest request,
            CompleteClusterRawPlan plan,
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
            GameDesc descriptor = DspPreviewGateway.CreateDescriptor(request);
            GalaxyData? galaxy = null;
            try
            {
                recordTrace("UniverseGen.CreateGalaxy:cluster-raw:thread=" +
                    Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture));
                galaxy = UniverseGen.CreateGalaxy(descriptor);
                if (galaxy.starCount != plan.Preview.GeneratedStarCount)
                    throw new InvalidOperationException("The raw candidate cluster changed star count.");

                var planets = galaxy.stars
                    .SelectMany(star => star.planets.Select(planet => new { star, planet }))
                    .ToDictionary(
                        item => item.planet.id,
                        item => (Star: item.star, Planet: item.planet));
                int solidPlanetCount = planets.Values.Count(
                    item => item.Planet.type != EPlanetType.Gas);
                if (solidPlanetCount != plan.Targets.Count)
                {
                    throw new RawCompatibilityException(
                        "raw-target-count-mismatch",
                        "The raw candidate cluster changed its solid-planet count.",
                        "expected=" + plan.Targets.Count.ToString(CultureInfo.InvariantCulture) +
                        ";actual=" + solidPlanetCount.ToString(CultureInfo.InvariantCulture));
                }
                var candidateData = new GameData
                {
                    gameDesc = descriptor,
                    galaxy = galaxy
                };
                IRuntimeCompleteClusterRawSession session =
                    new DspCompleteClusterRawSession(
                        request.GalaxySeed,
                        descriptor,
                        galaxy,
                        candidateData,
                        planets,
                        recordTrace,
                        () => AtomicCompletedForProbe?.Invoke());
                galaxy = null;
                return session;
            }
            catch
            {
                if (galaxy != null)
                {
                    galaxy.Free();
                    recordTrace("cluster-raw:candidate:released");
                }
                throw;
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

        private sealed class DspCompleteClusterRawSession : IRuntimeCompleteClusterRawSession
        {
            private readonly int galaxySeed;
            private readonly GameDesc descriptor;
            private readonly GalaxyData galaxy;
            private readonly GameData candidateData;
            private readonly IReadOnlyDictionary<int, (StarData Star, PlanetData Planet)> planets;
            private readonly Action<string> sessionTrace;
            private readonly Action atomicCompleted;
            private readonly GameData? ambientData;
            private readonly GameDesc? ambientDescription;
            private CompleteClusterPlanetTarget? pendingTarget;
            private PlanetData? pendingPlanet;
            private PlanetAlgorithm? pendingAlgorithm;
            private Thread? terrainThread;
            private Exception? terrainFailure;
            private bool disposed;

            public DspCompleteClusterRawSession(
                int galaxySeed,
                GameDesc descriptor,
                GalaxyData galaxy,
                GameData candidateData,
                IReadOnlyDictionary<int, (StarData Star, PlanetData Planet)> planets,
                Action<string> sessionTrace,
                Action atomicCompleted)
            {
                this.galaxySeed = galaxySeed;
                this.descriptor = descriptor;
                this.galaxy = galaxy;
                this.candidateData = candidateData;
                this.planets = planets;
                this.sessionTrace = sessionTrace;
                this.atomicCompleted = atomicCompleted;
                ambientData = GameMain.data;
                ambientDescription = DSPGame.GameDesc;
                PrepareRawGeneration(sessionTrace);
                StateRestored = true;
                sessionTrace("cluster-raw:terrain-worker:prepared");
            }

            public bool StateRestored { get; private set; }

            public void StartPlanet(
                CompleteClusterPlanetTarget target,
                CancellationToken cancellationToken,
                Action<string> recordTrace)
            {
                if (disposed)
                    throw new ObjectDisposedException(nameof(DspCompleteClusterRawSession));
                if (pendingTarget != null)
                    throw new InvalidOperationException("A terrain generation step is already pending.");
                cancellationToken.ThrowIfCancellationRequested();
                PlanetData planet = ResolveTarget(target);
                PlanetAlgorithm algorithm = PlanetModelingManager.Algorithm(planet);
                planet.data = new PlanetRawData(planet.precision);
                planet.modData = planet.data.InitModData(planet.modData);
                planet.data.CalcVerts();
                planet.aux = new PlanetAuxData(planet);

                pendingTarget = target;
                pendingPlanet = planet;
                pendingAlgorithm = algorithm;
                terrainFailure = null;
                terrainThread = new Thread(() =>
                {
                    try
                    {
                        algorithm.GenerateTerrain(planet.mod_x, planet.mod_y);
                    }
                    catch (Exception exception)
                    {
                        terrainFailure = exception;
                    }
                })
                {
                    IsBackground = true,
                    Name = "DSP Seed Scanner Terrain"
                };
                recordTrace("raw:terrain-worker:start:planet=" +
                    planet.id.ToString(CultureInfo.InvariantCulture) +
                    ":algorithm=" + planet.algoId.ToString(CultureInfo.InvariantCulture));
                terrainThread.Start();
            }

            public bool TryCompletePlanet(
                CompleteClusterPlanetTarget target,
                CancellationToken cancellationToken,
                Action<string> recordTrace,
                out NormalizedRawPlanetEvidence? evidence)
            {
                evidence = null;
                if (disposed)
                    throw new ObjectDisposedException(nameof(DspCompleteClusterRawSession));
                cancellationToken.ThrowIfCancellationRequested();
                if (pendingTarget == null || pendingTarget.PlanetId != target.PlanetId)
                    throw new InvalidOperationException("The terrain completion target is not pending.");
                if (terrainThread == null || pendingPlanet == null || pendingAlgorithm == null)
                    throw new InvalidOperationException("The terrain worker state is incomplete.");
                if (terrainThread.IsAlive)
                    return false;
                terrainThread.Join();
                if (terrainFailure != null)
                    throw new InvalidOperationException(
                        "Background terrain generation failed.", terrainFailure);

                PlanetData planet = pendingPlanet;
                PlanetAlgorithm algorithm = pendingAlgorithm;
                recordTrace("raw:terrain-worker:complete:planet=" +
                    planet.id.ToString(CultureInfo.InvariantCulture));
                StateRestored = false;
                try
                {
                    GameMain.data = candidateData;
                    DSPGame.GameDesc = descriptor;
                    planet.data!.veinCursor = 1;
                    algorithm.GenerateVeins();
                    planet.SummarizeVeinGroups();
                    evidence = NormalizeGroups(galaxySeed, planet);
                }
                finally
                {
                    GameMain.data = ambientData;
                    DSPGame.GameDesc = ambientDescription;
                    StateRestored =
                        ReferenceEquals(GameMain.data, ambientData) &&
                        ReferenceEquals(DSPGame.GameDesc, ambientDescription);
                }
                if (!StateRestored)
                    throw new InvalidOperationException("Terrain completion state restoration failed.");

                recordTrace("raw:veins:complete:groups=" +
                    evidence.Groups.Count.ToString(CultureInfo.InvariantCulture));
                ReleasePendingPlanet();
                atomicCompleted();
                return true;
            }

            private PlanetData ResolveTarget(CompleteClusterPlanetTarget target)
            {
                if (!ReferenceEquals(GameMain.data, ambientData) ||
                    !ReferenceEquals(DSPGame.GameDesc, ambientDescription))
                {
                    throw new InvalidOperationException(
                        "Ambient DSP state changed during the complete-cluster session.");
                }
                if (!planets.TryGetValue(target.PlanetId, out var item))
                {
                    throw new RawCompatibilityException(
                        "raw-target-missing",
                        "A declared cluster planet was not generated.");
                }

                PlanetData planet = item.Planet;
                string systemId = DspPreviewGateway.SystemIdentifier(
                    galaxySeed,
                    item.Star,
                    item.Star.id == galaxy.birthStarId);
                if (planet.type == EPlanetType.Gas ||
                    planet.algoId != target.AlgorithmId ||
                    !String.Equals(systemId, target.System.Identifier, StringComparison.Ordinal))
                {
                    throw new RawCompatibilityException(
                        "raw-target-mismatch",
                        "A generated planet no longer matches its complete-cluster plan.",
                        "planet=" + target.PlanetId.ToString(CultureInfo.InvariantCulture));
                }
                return planet;
            }

            public void Dispose()
            {
                if (disposed)
                    return;
                disposed = true;
                try
                {
                    if (terrainThread != null && terrainThread.IsAlive)
                        terrainThread.Join();
                    ReleasePendingPlanet();
                    galaxy.Free();
                }
                finally
                {
                    GameMain.data = ambientData;
                    DSPGame.GameDesc = ambientDescription;
                    sessionTrace("cluster-raw:candidate:released");
                    StateRestored =
                        ReferenceEquals(GameMain.data, ambientData) &&
                        ReferenceEquals(DSPGame.GameDesc, ambientDescription);
                }
            }

            private void ReleasePendingPlanet()
            {
                if (pendingPlanet?.data != null)
                {
                    pendingPlanet.data.Free();
                    pendingPlanet.data = null;
                }
                if (pendingPlanet?.aux != null)
                {
                    pendingPlanet.aux.Free();
                    pendingPlanet.aux = null;
                }
                pendingPlanet = null;
                pendingAlgorithm = null;
                pendingTarget = null;
                terrainThread = null;
                terrainFailure = null;
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

        private static NormalizedRawPlanetEvidence NormalizeGroups(
            int galaxySeed,
            PlanetData planet)
        {
            if (planet.veinGroups == null)
                throw new InvalidOperationException("DSP returned no summarized vein groups.");
            var groups = new List<NormalizedRawVeinGroup>();
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
            return new NormalizedRawPlanetEvidence(
                galaxySeed,
                planet.id,
                planet.theme,
                planet.algoId,
                RawPlanetCoverage.Complete(),
                Array.Empty<NormalizedRawVeinNode>(),
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
                (typeof(PlanetData), "aux", MemberTypes.Field),
                (typeof(PlanetData), "veinGroups", MemberTypes.Field),
                (typeof(PlanetData), "SummarizeVeinGroups", MemberTypes.Method),
                (typeof(PlanetRawData), "InitModData", MemberTypes.Method),
                (typeof(PlanetRawData), "CalcVerts", MemberTypes.Method),
                (typeof(PlanetRawData), "Free", MemberTypes.Method),
                (typeof(PlanetRawData), "veinPool", MemberTypes.Field),
                (typeof(PlanetRawData), "veinCursor", MemberTypes.Field),
                (typeof(PlanetAuxData), "Free", MemberTypes.Method),
                (typeof(PlanetAlgorithm), "GenerateTerrain", MemberTypes.Method),
                (typeof(PlanetAlgorithm), "GenerateVeins", MemberTypes.Method),
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
                (typeof(PlanetModelingManager), "Algorithm", MemberTypes.Method),
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
