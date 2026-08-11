using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using BepInEx;
using DSPSeedScanner.Core;
using DSPSeedScanner.Runtime;
using UnityEngine;

namespace DSPSeedScanner.Plugin
{
    [BepInPlugin(PluginGuid, PluginName, BuildVersion.BepInPluginVersion)]
    public sealed class DSPSeedScannerPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "io.github.shytamir.dspseedscanner";
        public const string PluginName = "DSP Seed Scanner";
        public const string PluginVersion = BuildVersion.PluginVersion;

        private PreviewScanCoordinator? coordinator;
        private DspPreviewGateway? previewGateway;
        private RawPlanetCoordinator? rawCoordinator;
        private BirthSystemRawCoordinator? birthSystemCoordinator;
        private CompleteClusterRawCoordinator? completeClusterCoordinator;
        private CompleteClusterResultCache? completeClusterCache;
        private DspRawPlanetGateway? rawGateway;
        private bool probeAttempted;
        private CompleteClusterRawOperation? cooperativeProbeOperation;
        private CompleteClusterRawResult? cooperativeProbeReference;
        private DspStateSnapshot? cooperativeProbeInitialState;
        private readonly List<int> cooperativeProbeFrames = new List<int>();
        private bool cooperativeProbeYieldsRestored = true;
        private bool cooperativeReferenceRestored;

        private void Awake()
        {
            var operationGate = new RuntimeOperationGate();
            previewGateway = new DspPreviewGateway(
                Thread.CurrentThread.ManagedThreadId,
                PluginGuid);
            rawGateway = new DspRawPlanetGateway(previewGateway);
            coordinator = new PreviewScanCoordinator(previewGateway, operationGate);
            rawCoordinator = new RawPlanetCoordinator(rawGateway, operationGate);
            birthSystemCoordinator = new BirthSystemRawCoordinator(rawGateway, operationGate);
            completeClusterCoordinator = new CompleteClusterRawCoordinator(
                rawGateway,
                operationGate);
            completeClusterCache = new CompleteClusterResultCache(
                Path.Combine(Paths.ConfigPath, "DSPSeedScanner", "cache"));
            Logger.LogInfo("Runtime boundary initialized on managed thread " +
                Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture) + ".");
        }

        private void Update()
        {
            string? output = Environment.GetEnvironmentVariable("DSP_SEED_SCANNER_PROBE_OUTPUT");
            if (String.IsNullOrWhiteSpace(output) ||
                LDB.themes == null || LDB.themes.Length == 0)
            {
                return;
            }

            string? mode = Environment.GetEnvironmentVariable("DSP_SEED_SCANNER_PROBE_MODE");
            if (String.Equals(mode, "cooperative-cluster", StringComparison.Ordinal))
            {
                AdvanceCooperativeProbe(output);
                return;
            }
            if (probeAttempted)
                return;

            probeAttempted = true;
            try
            {
                if (String.Equals(mode, "conformance", StringComparison.Ordinal))
                {
                    WriteConformanceProbe(output);
                    Logger.LogInfo("IMPL-08 conformance probe completed: " + output);
                }
                else if (String.Equals(mode, "rare-access", StringComparison.Ordinal))
                {
                    WriteRareAccessProbe(output);
                    Logger.LogInfo("IMPL-07 rare-access probe completed: " + output);
                }
                else if (String.Equals(mode, "birth-resources", StringComparison.Ordinal))
                {
                    WriteBirthResourceProbe(output);
                    Logger.LogInfo("IMPL-06 birth-resource probe completed: " + output);
                }
                else if (String.Equals(mode, "raw-certification", StringComparison.Ordinal))
                {
                    WriteRawCertificationProbe(output);
                    Logger.LogInfo("IMPL-05 raw certification probe completed: " + output);
                }
                else
                {
                    var results = new List<RuntimeScanResult>();
                    foreach (int seed in ParseSeeds())
                        results.Add(ScanPreview(Request(seed), CancellationToken.None));
                    WriteProbe(output, results);
                    Logger.LogInfo("IMPL-04 preview probe completed: " + output);
                }
            }
            catch (Exception exception)
            {
                File.WriteAllText(output, "probe-error\t" + exception, new UTF8Encoding(false));
                Logger.LogError(exception);
            }
            finally
            {
                Application.Quit();
            }
        }

        private void AdvanceCooperativeProbe(string path)
        {
            try
            {
                if (!probeAttempted)
                {
                    probeAttempted = true;
                    PreviewScanRequest request = Request(ParseSeeds().First());
                    cooperativeProbeInitialState = DspStateSnapshot.Capture();
                    cooperativeProbeReference = GenerateCompleteClusterResources(
                        request,
                        CancellationToken.None);
                    cooperativeReferenceRestored =
                        cooperativeProbeInitialState.CompareCurrent().All(check => check.Value);
                    cooperativeProbeOperation = StartCompleteClusterResources(
                        request,
                        CancellationToken.None);
                    cooperativeProbeYieldsRestored &=
                        cooperativeProbeOperation.IsYieldStateRestored;
                    return;
                }

                if (cooperativeProbeOperation == null ||
                    cooperativeProbeReference == null ||
                    cooperativeProbeInitialState == null)
                {
                    throw new InvalidOperationException(
                        "The cooperative probe was not initialized.");
                }
                if (cooperativeProbeOperation.State == CompleteClusterRawOperationState.Ready)
                {
                    cooperativeProbeFrames.Add(Time.frameCount);
                    cooperativeProbeOperation.Advance();
                    cooperativeProbeYieldsRestored &=
                        cooperativeProbeOperation.IsYieldStateRestored;
                    if (cooperativeProbeOperation.State == CompleteClusterRawOperationState.Ready)
                        return;
                }

                CompleteClusterRawResult incremental =
                    cooperativeProbeOperation.Result ?? throw new InvalidOperationException(
                        "The cooperative probe completed without a result.");
                bool finalStateRestored = cooperativeProbeInitialState.CompareCurrent()
                    .All(check => check.Value);
                bool progressMonotonic = HasMonotonicPlanetProgress(incremental.Progress);
                bool distinctFrames = cooperativeProbeFrames.Distinct().Count() ==
                    cooperativeProbeFrames.Count;
                bool rareEquivalent = cooperativeProbeReference.RareResources
                    .SequenceEqual(incremental.RareResources);
                bool reportsEquivalent = cooperativeProbeReference.Reports
                    .SequenceEqual(incremental.Reports);

                var lines = new StringBuilder();
                lines.Append("cooperative-result\t")
                    .Append(incremental.GalaxySeed).Append('\t')
                    .Append(cooperativeProbeReference.Status).Append('\t')
                    .Append(incremental.Status).Append('\t')
                    .Append(reportsEquivalent).Append('\t')
                    .Append(rareEquivalent).Append('\t')
                    .Append(progressMonotonic).Append('\t')
                    .Append(distinctFrames).Append('\t')
                    .Append(cooperativeProbeYieldsRestored).Append('\t')
                    .Append(cooperativeReferenceRestored).Append('\t')
                    .Append(finalStateRestored).Append('\t')
                    .Append(cooperativeProbeFrames.Count).Append('\t')
                    .Append(incremental.Coverage.ExpectedPlanets).AppendLine();
                foreach (KeyValuePair<string, bool> check in
                    cooperativeProbeInitialState.CompareCurrent())
                {
                    lines.Append("cooperative-state\t")
                        .Append(check.Key).Append('\t').Append(check.Value).AppendLine();
                }
                File.WriteAllText(path, lines.ToString(), new UTF8Encoding(false));
                Logger.LogInfo("PRES-02 cooperative-cluster probe completed: " + path);
                cooperativeProbeOperation.Dispose();
                cooperativeProbeOperation = null;
                Application.Quit();
            }
            catch (Exception exception)
            {
                cooperativeProbeOperation?.Dispose();
                cooperativeProbeOperation = null;
                File.WriteAllText(path, "probe-error\t" + exception, new UTF8Encoding(false));
                Logger.LogError(exception);
                Application.Quit();
            }
        }

        private static bool HasMonotonicPlanetProgress(
            IReadOnlyList<CompleteClusterRawProgress> progress)
        {
            if (progress.Count == 0 ||
                progress[0].State != CompleteClusterProgressState.Planned ||
                progress[0].CompletedPlanets != 0)
            {
                return false;
            }
            int completed = 0;
            for (int index = 1; index < progress.Count; index += 2)
            {
                if (index + 1 >= progress.Count)
                    return false;
                CompleteClusterRawProgress started = progress[index];
                CompleteClusterRawProgress finished = progress[index + 1];
                if (started.State != CompleteClusterProgressState.PlanetStarted ||
                    started.CompletedPlanets != completed ||
                    finished.State != CompleteClusterProgressState.PlanetCompleted ||
                    finished.CompletedPlanets != completed + 1 ||
                    started.PlanetId != finished.PlanetId)
                {
                    return false;
                }
                completed++;
            }
            return completed == progress[0].ExpectedPlanets;
        }

        public RuntimeScanResult ScanPreview(
            PreviewScanRequest request,
            CancellationToken cancellationToken)
        {
            if (coordinator == null)
                throw new InvalidOperationException("The plugin has not completed Awake.");
            return coordinator.TryScan(request, cancellationToken);
        }

        public RawPlanetResult GenerateRawPlanet(
            RawPlanetRequest request,
            CancellationToken cancellationToken)
        {
            if (rawCoordinator == null)
                throw new InvalidOperationException("The plugin has not completed Awake.");
            return rawCoordinator.TryGenerate(request, cancellationToken);
        }

        public BirthSystemRawResult GenerateBirthSystemResources(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<BirthSystemRawProgress>? reportProgress = null)
        {
            if (birthSystemCoordinator == null)
                throw new InvalidOperationException("The plugin has not completed Awake.");
            return birthSystemCoordinator.TryGenerate(request, cancellationToken, reportProgress);
        }

        public CompleteClusterRawResult GenerateCompleteClusterResources(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<CompleteClusterRawProgress>? reportProgress = null)
        {
            if (completeClusterCoordinator == null)
                throw new InvalidOperationException("The plugin has not completed Awake.");
            return completeClusterCoordinator.TryGenerate(
                request,
                cancellationToken,
                reportProgress);
        }

        public CompleteClusterRawOperation StartCompleteClusterResources(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<CompleteClusterRawProgress>? reportProgress = null)
        {
            if (completeClusterCoordinator == null)
                throw new InvalidOperationException("The plugin has not completed Awake.");
            return completeClusterCoordinator.TryStart(
                request,
                cancellationToken,
                reportProgress);
        }

        public bool TryGetCachedCompleteCluster(
            PreviewGenerationIdentity identity,
            RuntimeFingerprint fingerprint,
            out CompleteClusterRawResult? result)
        {
            if (completeClusterCache == null)
                throw new InvalidOperationException("The plugin has not completed Awake.");
            return completeClusterCache.TryRead(identity, fingerprint, out result);
        }

        public bool TryStoreCompleteCluster(
            PreviewGenerationIdentity identity,
            CompleteClusterRawResult result)
        {
            if (completeClusterCache == null)
                throw new InvalidOperationException("The plugin has not completed Awake.");
            return completeClusterCache.TryStore(identity, result);
        }

        public bool ClearCompleteClusterCache()
        {
            if (completeClusterCache == null)
                throw new InvalidOperationException("The plugin has not completed Awake.");
            return completeClusterCache.Clear();
        }

        private void WriteConformanceProbe(string path)
        {
            if (previewGateway == null)
                throw new InvalidOperationException("The preview gateway is unavailable.");

            GameData? originalData = GameMain.data;
            GameDesc? originalDescription = DSPGame.GameDesc;
            var sentinelDescription = new GameDesc();
            var sentinelData = new GameData
            {
                gameDesc = sentinelDescription,
                factories = new PlanetFactory[1]
            };
            try
            {
                GameMain.data = sentinelData;
                DSPGame.GameDesc = sentinelDescription;
                WriteConformanceCases(path);
            }
            finally
            {
                GameMain.data = originalData;
                DSPGame.GameDesc = originalDescription;
            }
        }

        private void WriteConformanceCases(string path)
        {
            if (previewGateway == null)
                throw new InvalidOperationException("The preview gateway is unavailable.");

            int seed = ParseSeeds().First();
            var cases = new List<ConformanceCase>();
            cases.Add(RunPreviewCase(
                "preview-success",
                () => ScanPreview(Request(seed), CancellationToken.None)));

            try
            {
                previewGateway.AfterGalaxyCreatedForProbe = () =>
                    throw new InvalidOperationException("injected preview failure");
                cases.Add(RunPreviewCase(
                    "preview-failure",
                    () => ScanPreview(Request(seed), CancellationToken.None)));
            }
            finally
            {
                previewGateway.AfterGalaxyCreatedForProbe = null;
            }

            using (var cancellation = new CancellationTokenSource())
            {
                try
                {
                    previewGateway.AfterGalaxyCreatedForProbe = cancellation.Cancel;
                    cases.Add(RunPreviewCase(
                        "preview-cancellation",
                        () => ScanPreview(Request(seed), cancellation.Token)));
                }
                finally
                {
                    previewGateway.AfterGalaxyCreatedForProbe = null;
                }
            }

            RuntimeScanResult? nested = null;
            try
            {
                previewGateway.AfterGalaxyCreatedForProbe = () =>
                    nested = ScanPreview(Request(seed), CancellationToken.None);
                cases.Add(RunPreviewCase(
                    "preview-reentrant-outer",
                    () => ScanPreview(Request(seed), CancellationToken.None)));
            }
            finally
            {
                previewGateway.AfterGalaxyCreatedForProbe = null;
            }
            if (nested == null)
                throw new InvalidOperationException("The re-entrant preview probe did not run.");
            cases.Add(new ConformanceCase(
                "preview-reentrant-inner",
                nested,
                Array.Empty<KeyValuePair<string, bool>>()));

            var lines = new StringBuilder();
            foreach (ConformanceCase item in cases)
            {
                RuntimeScanResult result = item.Result;
                lines.Append("conformance-result\t").Append(item.Name).Append('\t')
                    .Append(result.GalaxySeed).Append('\t').Append(result.Stage).Append('\t')
                    .Append(result.Status).Append('\t').Append(result.Code).Append('\t')
                    .Append(result.StateRestored).Append('\t')
                    .Append(result.Reports.Count).AppendLine();
                if (result.Fingerprint != null)
                {
                    lines.Append("conformance-fingerprint\t").Append(item.Name).Append('\t')
                        .Append(result.Fingerprint.GameVersion).Append('\t')
                        .Append(result.Fingerprint.GalaxyAlgorithm).Append('\t')
                        .Append(result.Fingerprint.AssemblySha256).Append('\t')
                        .Append(result.Fingerprint.OrderedThemeIdsKey).Append('\t')
                        .Append(result.Fingerprint.GenerationMethodIlSha256).Append('\t')
                        .Append(String.Join(",", result.Fingerprint.LoadedGenerationModIds)).Append('\t')
                        .Append(String.Join(",", result.Fingerprint.LoadedPatcherIds)).AppendLine();
                }
                foreach (KeyValuePair<string, bool> check in item.StateChecks)
                {
                    lines.Append("conformance-state\t").Append(item.Name).Append('\t')
                        .Append(check.Key).Append('\t').Append(check.Value).AppendLine();
                }
                foreach (string trace in result.Trace.Where(value =>
                    value == "GalaxyData.Free" ||
                    value.StartsWith("state:restore=", StringComparison.Ordinal) ||
                    value == "request:cancelled" || value == "request:failed"))
                {
                    lines.Append("conformance-trace\t").Append(item.Name).Append('\t')
                        .Append(trace).AppendLine();
                }
            }
            File.WriteAllText(path, lines.ToString(), new UTF8Encoding(false));
        }

        private ConformanceCase RunPreviewCase(
            string name,
            Func<RuntimeScanResult> operation)
        {
            DspStateSnapshot before = DspStateSnapshot.Capture();
            RuntimeScanResult result = operation();
            return new ConformanceCase(name, result, before.CompareCurrent());
        }

        private void WriteRareAccessProbe(string path)
        {
            if (rawGateway == null)
                throw new InvalidOperationException("The raw gateway is unavailable.");

            int[] seeds = ParseSeeds().ToArray();
            var results = new List<(CompleteClusterRawResult Result, long RetainedBytes)>();
            foreach (int seed in seeds)
            {
                results.Add(MeasureCompleteCluster(
                    Request(seed),
                    CancellationToken.None));
            }

            using (var cancellation = new CancellationTokenSource())
            {
                results.Add(MeasureCompleteCluster(
                    Request(seeds[0]),
                    cancellation.Token,
                    progress =>
                    {
                        if (progress.State == CompleteClusterProgressState.PlanetCompleted &&
                            progress.CompletedPlanets == 3)
                        {
                            cancellation.Cancel();
                        }
                    }));
            }

            int atomicCompletions = 0;
            try
            {
                rawGateway.AtomicCompletedForProbe = () =>
                {
                    atomicCompletions++;
                    if (atomicCompletions == 2)
                    {
                        throw new RawCompatibilityException(
                            "injected-rare-incompatibility",
                            "Injected complete-cluster compatibility failure.",
                            "EVeinType=99");
                    }
                };
                results.Add(MeasureCompleteCluster(
                    Request(seeds[0]),
                    CancellationToken.None));
            }
            finally
            {
                rawGateway.AtomicCompletedForProbe = null;
            }
            WriteRareProbe(path, results);
        }

        private (CompleteClusterRawResult Result, long RetainedBytes) MeasureCompleteCluster(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<CompleteClusterRawProgress>? reportProgress = null)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            long before = GC.GetTotalMemory(true);
            CompleteClusterRawResult result = GenerateCompleteClusterResources(
                request,
                cancellationToken,
                reportProgress);
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            return (result, GC.GetTotalMemory(true) - before);
        }

        private static void WriteRareProbe(
            string path,
            IEnumerable<(CompleteClusterRawResult Result, long RetainedBytes)> results)
        {
            var lines = new StringBuilder();
            foreach ((CompleteClusterRawResult result, long retainedBytes) in results)
            {
                string seed = result.GalaxySeed.ToString(CultureInfo.InvariantCulture);
                lines.Append("cluster-result\t").Append(seed).Append('\t')
                    .Append(result.Status).Append('\t').Append(result.Code).Append('\t')
                    .Append(result.StateRestored).Append('\t')
                    .Append(result.Coverage.State).Append('\t')
                    .Append(result.Coverage.ExpectedPlanets).Append('\t')
                    .Append(result.Coverage.CompletedPlanets).Append('\t')
                    .Append(result.AffectedPlanetId).Append('\t')
                    .Append(result.RawDiagnostic).AppendLine();
                lines.Append("cluster-observation\t").Append(seed).Append('\t')
                    .Append(result.ElapsedMilliseconds.ToString(CultureInfo.InvariantCulture))
                    .Append('\t')
                    .Append(result.ManagedMemoryDeltaBytes.ToString(CultureInfo.InvariantCulture))
                    .AppendLine();
                lines.Append("cluster-retained\t").Append(seed).Append('\t')
                    .Append(retainedBytes.ToString(CultureInfo.InvariantCulture))
                    .AppendLine();
                foreach (NormalizedRareResourceEvidence rare in result.RareResources)
                {
                    lines.Append("cluster-rare\t").Append(seed).Append('\t')
                        .Append(rare.ResourceId).Append('\t')
                        .Append(rare.IsPresent).Append('\t')
                        .Append(rare.NearestSystem?.Identifier).Append('\t')
                        .Append(rare.DistanceFromBirthLy?.ToString(
                            "0.############################",
                            CultureInfo.InvariantCulture)).Append('\t')
                        .Append(rare.Amount).Append('\t')
                        .Append(rare.VeinGroups).AppendLine();
                }
                foreach (ConclusionReport report in result.Reports.Where(report =>
                    report.ConclusionId.StartsWith("RR-ACCESS.", StringComparison.Ordinal) ||
                    report.ConclusionId.StartsWith("MF-RESOURCE-SCOPE.", StringComparison.Ordinal) ||
                    report.SourceConclusionId?.StartsWith("RR-ACCESS.", StringComparison.Ordinal) == true))
                {
                    lines.Append("cluster-report\t").Append(seed).Append('\t')
                        .Append(report.ConclusionId).Append('\t')
                        .Append(report.Outcome).Append('\t')
                        .Append(report.Subject.Identifier).Append('\t')
                        .Append(report.DecisiveFact?.Value).Append('\t')
                        .Append(report.DecisiveFact?.Unit).Append('\t')
                        .Append(report.DiagnosticCause?.Code).Append('\t')
                        .Append(report.SourceConclusionId).AppendLine();
                }
                for (int index = 0; index < result.Trace.Count; index++)
                {
                    string trace = result.Trace[index];
                    if (trace == "cluster-plan:candidate:released" ||
                        trace == "cluster-raw:candidate:released" ||
                        trace.StartsWith("state:restore=", StringComparison.Ordinal))
                    {
                        lines.Append("cluster-trace\t").Append(seed).Append('\t')
                            .Append(index.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(trace).AppendLine();
                    }
                }
            }
            File.WriteAllText(path, lines.ToString(), new UTF8Encoding(false));
        }

        private void WriteBirthResourceProbe(string path)
        {
            if (rawGateway == null)
                throw new InvalidOperationException("The raw gateway is unavailable.");

            int[] seeds = ParseSeeds().ToArray();
            var results = new List<BirthSystemRawResult>();
            foreach (int seed in seeds)
                results.Add(GenerateBirthSystemResources(Request(seed), CancellationToken.None));

            results.Add(GenerateBirthSystemResources(
                Request(seeds[0], 0.5m),
                CancellationToken.None));

            using (var cancellation = new CancellationTokenSource())
            {
                results.Add(GenerateBirthSystemResources(
                    Request(seeds[0]),
                    cancellation.Token,
                    progress =>
                    {
                        if (progress.State == BirthSystemProgressState.PlanetCompleted)
                            cancellation.Cancel();
                    }));
            }

            int atomicCompletions = 0;
            try
            {
                rawGateway.AtomicCompletedForProbe = () =>
                {
                    atomicCompletions++;
                    if (atomicCompletions == 2)
                        throw new InvalidOperationException("injected birth-system planet failure");
                };
                results.Add(GenerateBirthSystemResources(Request(seeds[0]), CancellationToken.None));
            }
            finally
            {
                rawGateway.AtomicCompletedForProbe = null;
            }
            WriteBirthProbe(path, results);
        }

        private static void WriteBirthProbe(
            string path,
            IEnumerable<BirthSystemRawResult> results)
        {
            var lines = new StringBuilder();
            foreach (BirthSystemRawResult result in results)
            {
                string seed = result.GalaxySeed.ToString(CultureInfo.InvariantCulture);
                lines.Append("birth-result\t").Append(seed).Append('\t')
                    .Append(result.Status).Append('\t').Append(result.Code).Append('\t')
                    .Append(result.StateRestored).Append('\t')
                    .Append(result.Coverage.State).Append('\t')
                    .Append(result.Coverage.ExpectedPlanets).Append('\t')
                    .Append(result.Coverage.CompletedPlanets).AppendLine();
                foreach (BirthSystemRawProgress progress in result.Progress)
                {
                    lines.Append("birth-progress\t").Append(seed).Append('\t')
                        .Append(progress.State).Append('\t')
                        .Append(progress.ExpectedPlanets).Append('\t')
                        .Append(progress.CompletedPlanets).Append('\t')
                        .Append(progress.PlanetId).AppendLine();
                }
                foreach (ConclusionReport report in result.Reports.Where(report =>
                    report.ConclusionId.StartsWith("FS-RESOURCES.", StringComparison.Ordinal)))
                {
                    lines.Append("birth-report\t").Append(seed).Append('\t')
                        .Append(report.ConclusionId).Append('\t').Append(report.Outcome).Append('\t')
                        .Append(report.DecisiveFact?.Value).Append('\t')
                        .Append(report.DecisiveFact?.Unit).Append('\t')
                        .Append(report.DiagnosticCause?.Code).AppendLine();
                }
                for (int index = 0; index < result.Trace.Count; index++)
                {
                    lines.Append("birth-trace\t").Append(seed).Append('\t')
                        .Append(index.ToString(CultureInfo.InvariantCulture)).Append('\t')
                        .Append(result.Trace[index]).AppendLine();
                }
            }
            File.WriteAllText(path, lines.ToString(), new UTF8Encoding(false));
        }

        private void WriteRawCertificationProbe(string path)
        {
            if (rawGateway == null)
                throw new InvalidOperationException("The raw gateway is unavailable.");

            int[] seeds = ParseSeeds().Distinct().ToArray();
            var targets = new SortedDictionary<int, (PreviewScanRequest Request, int PlanetId)>();
            int[] reachable = rawGateway.ReachableSolidAlgorithmIds(Request(seeds[0])).ToArray();
            foreach (int seed in seeds)
            {
                PreviewScanRequest request = Request(seed);
                foreach (KeyValuePair<int, int> pair in rawGateway.DiscoverCandidatePlanets(request))
                {
                    if (reachable.Contains(pair.Key) && !targets.ContainsKey(pair.Key))
                        targets.Add(pair.Key, (request, pair.Value));
                }
                if (targets.Count == reachable.Length)
                    break;
            }

            int[] missing = reachable.Where(algorithm => !targets.ContainsKey(algorithm)).ToArray();
            if (missing.Length != 0)
            {
                throw new InvalidOperationException(
                    "No certification candidate was found for algorithms: " +
                    String.Join(",", missing));
            }

            var results = new List<RawPlanetResult>();
            foreach (KeyValuePair<int, (PreviewScanRequest Request, int PlanetId)> target in targets)
            {
                results.Add(GenerateRawPlanet(
                    new RawPlanetRequest(target.Value.Request, target.Value.PlanetId, target.Key),
                    CancellationToken.None));
            }
            KeyValuePair<int, (PreviewScanRequest Request, int PlanetId)> exitTarget = targets.First();
            var exitRequest = new RawPlanetRequest(
                exitTarget.Value.Request,
                exitTarget.Value.PlanetId,
                exitTarget.Key);
            try
            {
                rawGateway.AtomicCompletedForProbe = () =>
                    throw new InvalidOperationException("injected post-atomic raw failure");
                results.Add(GenerateRawPlanet(exitRequest, CancellationToken.None));
            }
            finally
            {
                rawGateway.AtomicCompletedForProbe = null;
            }
            using (var cancelledAfterAtomic = new CancellationTokenSource())
            {
                try
                {
                    rawGateway.AtomicCompletedForProbe = cancelledAfterAtomic.Cancel;
                    results.Add(GenerateRawPlanet(exitRequest, cancelledAfterAtomic.Token));
                }
                finally
                {
                    rawGateway.AtomicCompletedForProbe = null;
                }
            }
            using (var cancelledBeforeRaw = new CancellationTokenSource())
            {
                cancelledBeforeRaw.Cancel();
                results.Add(GenerateRawPlanet(exitRequest, cancelledBeforeRaw.Token));
            }
            WriteRawProbe(path, reachable, results);
        }

        private static PreviewScanRequest Request(int seed)
        {
            return Request(seed, 1m);
        }

        private static PreviewScanRequest Request(int seed, decimal resourceMultiplier)
        {
            return new PreviewScanRequest(
                seed,
                ConclusionDefinition.ReferenceStarCount,
                ConclusionDefinition.ReferenceGameVersion,
                resourceMultiplier,
                CombatMode.Combat,
                ConclusionDefinition.ReferenceCombatSettingsKey);
        }

        private static IEnumerable<int> ParseSeeds()
        {
            string? values = Environment.GetEnvironmentVariable("DSP_SEED_SCANNER_PROBE_SEEDS");
            if (String.IsNullOrWhiteSpace(values))
                values = Environment.GetEnvironmentVariable("DSP_SEED_SCANNER_PROBE_SEED");
            if (!String.IsNullOrWhiteSpace(values))
            {
                foreach (string value in values.Split(','))
                {
                    if (Int32.TryParse(
                        value.Trim(),
                        NumberStyles.Integer,
                        CultureInfo.InvariantCulture,
                        out int seed))
                    {
                        yield return seed;
                    }
                    else
                    {
                        throw new InvalidOperationException("Invalid probe seed: " + value);
                    }
                }
                yield break;
            }
            yield return 16_315_224;
        }

        private static void WriteProbe(string path, IEnumerable<RuntimeScanResult> results)
        {
            var lines = new StringBuilder();
            foreach (RuntimeScanResult result in results)
            {
                string seed = result.GalaxySeed.ToString(CultureInfo.InvariantCulture);
                lines.Append("result\t").Append(seed).Append('\t')
                    .Append(result.Status).Append('\t').Append(result.Code).Append('\t')
                    .Append(result.StateRestored).Append('\t')
                    .Append(result.GeneratedStarCount?.ToString(CultureInfo.InvariantCulture))
                    .AppendLine();
                if (result.Fingerprint != null)
                {
                    lines.Append("fingerprint\t").Append(seed).Append('\t')
                        .Append(result.Fingerprint.GameVersion).Append('\t')
                        .Append(result.Fingerprint.GalaxyAlgorithm.ToString(CultureInfo.InvariantCulture)).Append('\t')
                        .Append(result.Fingerprint.AssemblySha256).Append('\t')
                        .Append(result.Fingerprint.OrderedThemeIdsKey).Append('\t')
                        .Append(String.Join(",", result.Fingerprint.LoadedGenerationModIds)).Append('\t')
                        .Append(result.Fingerprint.GenerationMethodIlSha256).Append('\t')
                        .Append(String.Join(",", result.Fingerprint.LoadedPatcherIds))
                        .AppendLine();
                }
                foreach (ConclusionReport report in result.Reports)
                {
                    lines.Append("report\t").Append(seed).Append('\t')
                        .Append(report.ConclusionId).Append('\t')
                        .Append(report.Subject.Identifier).Append('\t')
                        .Append(report.Outcome).Append('\t')
                        .Append(report.DecisiveFact?.FactId).Append('\t')
                        .Append(report.DecisiveFact?.Value).Append('\t')
                        .Append(report.DecisiveFact?.Unit).Append('\t')
                        .Append(report.DiagnosticCause?.Code).Append('\t')
                        .Append(report.SourceConclusionId).Append('\t')
                        .Append(report.Stage).Append('\t')
                        .Append(report.Coverage.Scope).Append('\t')
                        .Append(report.Settings.ResourceMultiplier.ToString(CultureInfo.InvariantCulture)).Append('\t')
                        .Append(report.Settings.CombatMode).Append('\t')
                        .Append(report.Settings.CombatSettingsKey).Append('\t')
                        .Append(report.ContractVersion).Append('\t')
                        .Append(report.DefinitionVersion).AppendLine();
                }
                for (int index = 0; index < result.Trace.Count; index++)
                    lines.Append("trace\t").Append(seed).Append('\t').Append(index)
                        .Append('\t').Append(result.Trace[index]).AppendLine();
            }
            File.WriteAllText(path, lines.ToString(), new UTF8Encoding(false));
        }

        private static void WriteRawProbe(
            string path,
            IEnumerable<int> reachableAlgorithms,
            IEnumerable<RawPlanetResult> results)
        {
            var lines = new StringBuilder();
            lines.Append("catalogue\t").Append(String.Join(",", reachableAlgorithms)).AppendLine();
            foreach (RawPlanetResult result in results)
            {
                lines.Append("raw-result\t")
                    .Append(result.GalaxySeed.ToString(CultureInfo.InvariantCulture)).Append('\t')
                    .Append(result.PlanetId.ToString(CultureInfo.InvariantCulture)).Append('\t')
                    .Append(result.Status).Append('\t').Append(result.Code).Append('\t')
                    .Append(result.Coverage.State).Append('\t')
                    .Append(result.Coverage.CompletedSubjects.ToString(CultureInfo.InvariantCulture)).Append('\t')
                    .Append(result.StateRestored).Append('\t').Append(result.Stage).Append('\t')
                    .Append(result.Request.Identity.RequestedStarCount.ToString(CultureInfo.InvariantCulture)).Append('\t')
                    .Append(result.Request.Identity.CreationVersion).Append('\t')
                    .Append(result.Request.Identity.ResourceMultiplier.ToString(CultureInfo.InvariantCulture)).Append('\t')
                    .Append(result.Request.Identity.CombatMode).Append('\t')
                    .Append(result.Request.Identity.CombatSettingsKey).Append('\t')
                    .Append(result.RawDiagnostic).AppendLine();
                if (result.Fingerprint != null)
                {
                    lines.Append("raw-fingerprint\t")
                        .Append(result.GalaxySeed.ToString(CultureInfo.InvariantCulture)).Append('\t')
                        .Append(result.PlanetId.ToString(CultureInfo.InvariantCulture)).Append('\t')
                        .Append(result.Fingerprint.GameVersion).Append('\t')
                        .Append(result.Fingerprint.GalaxyAlgorithm.ToString(CultureInfo.InvariantCulture)).Append('\t')
                        .Append(result.Fingerprint.AssemblySha256).Append('\t')
                        .Append(result.Fingerprint.OrderedThemeIdsKey).Append('\t')
                        .Append(result.Fingerprint.ScannerCompatibilityVersion).Append('\t')
                        .Append(result.Fingerprint.ScannerContractVersion).AppendLine();
                }
                NormalizedRawPlanetEvidence? evidence = result.Evidence;
                if (evidence != null)
                {
                    lines.Append("raw-planet\t")
                        .Append(evidence.GalaxySeed.ToString(CultureInfo.InvariantCulture)).Append('\t')
                        .Append(evidence.PlanetId.ToString(CultureInfo.InvariantCulture)).Append('\t')
                        .Append(evidence.ThemeId.ToString(CultureInfo.InvariantCulture)).Append('\t')
                        .Append(evidence.AlgorithmId.ToString(CultureInfo.InvariantCulture)).Append('\t')
                        .Append(evidence.Nodes.Count.ToString(CultureInfo.InvariantCulture)).Append('\t')
                        .Append(evidence.Groups.Count.ToString(CultureInfo.InvariantCulture)).AppendLine();
                    foreach (NormalizedRawVeinNode node in evidence.Nodes)
                    {
                        lines.Append("raw-node\t")
                            .Append(evidence.PlanetId.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(node.SourceIndex.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(node.NodeId.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(node.ResourceType.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(node.ResourceId).Append('\t')
                            .Append(node.ProductItemId.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(node.Semantics).Append('\t')
                            .Append(node.Amount.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(node.AmountUnit).Append('\t')
                            .Append(node.GroupIndex.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(F(node.PositionX)).Append('\t').Append(F(node.PositionY)).Append('\t')
                            .Append(F(node.PositionZ)).Append('\t').Append(node.PositionUnit).Append('\t')
                            .Append(node.OilSpeedMultiplier.HasValue
                                ? F(node.OilSpeedMultiplier.Value)
                                : String.Empty).AppendLine();
                    }
                    foreach (NormalizedRawVeinGroup group in evidence.Groups)
                    {
                        lines.Append("raw-group\t")
                            .Append(evidence.PlanetId.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(group.GroupIndex.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(group.ResourceType.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(group.ResourceId).Append('\t').Append(group.Semantics).Append('\t')
                            .Append(group.NodeCount.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(group.Amount.ToString(CultureInfo.InvariantCulture)).Append('\t')
                            .Append(group.AmountUnit).Append('\t')
                            .Append(F(group.PositionX)).Append('\t').Append(F(group.PositionY)).Append('\t')
                            .Append(F(group.PositionZ)).Append('\t').Append(group.PositionUnit).AppendLine();
                    }
                }
                for (int index = 0; index < result.Trace.Count; index++)
                    lines.Append("raw-trace\t").Append(result.PlanetId).Append('\t').Append(index)
                        .Append('\t').Append(result.Trace[index]).AppendLine();
            }
            File.WriteAllText(path, lines.ToString(), new UTF8Encoding(false));
        }

        private sealed class ConformanceCase
        {
            public ConformanceCase(
                string name,
                RuntimeScanResult result,
                IEnumerable<KeyValuePair<string, bool>> stateChecks)
            {
                Name = name;
                Result = result;
                StateChecks = stateChecks.ToArray();
            }

            public string Name { get; }
            public RuntimeScanResult Result { get; }
            public IReadOnlyList<KeyValuePair<string, bool>> StateChecks { get; }
        }

        private sealed class DspStateSnapshot
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
            private readonly KeyValuePair<FieldInfo, object?>[] values;

            private DspStateSnapshot(
                GameData? gameData,
                GameDesc? gameDesc,
                KeyValuePair<FieldInfo, object?>[] values)
            {
                this.gameData = gameData;
                this.gameDesc = gameDesc;
                this.values = values;
            }

            public static DspStateSnapshot Capture()
            {
                GameData? data = GameMain.data;
                var values = new List<KeyValuePair<FieldInfo, object?>>();
                if (data != null)
                {
                    foreach (string name in TrackedGameDataFields)
                    {
                        FieldInfo? field = typeof(GameData).GetField(
                            name,
                            BindingFlags.Instance | BindingFlags.Public |
                            BindingFlags.NonPublic);
                        if (field != null)
                            values.Add(new KeyValuePair<FieldInfo, object?>(field, field.GetValue(data)));
                    }
                }
                return new DspStateSnapshot(data, DSPGame.GameDesc, values.ToArray());
            }

            public IReadOnlyList<KeyValuePair<string, bool>> CompareCurrent()
            {
                var checks = new List<KeyValuePair<string, bool>>
                {
                    Check("GameMain.data", ReferenceEquals(GameMain.data, gameData)),
                    Check("DSPGame.GameDesc", ReferenceEquals(DSPGame.GameDesc, gameDesc))
                };
                foreach (KeyValuePair<FieldInfo, object?> pair in values)
                {
                    object? current = gameData == null ? null : pair.Key.GetValue(gameData);
                    checks.Add(Check("GameData." + pair.Key.Name, ValuesEqual(current, pair.Value)));
                }
                return checks;
            }

            private static KeyValuePair<string, bool> Check(string name, bool value) =>
                new KeyValuePair<string, bool>(name, value);

            private static bool ValuesEqual(object? current, object? expected)
            {
                if (current == null || expected == null)
                    return current == expected;
                return current.GetType().IsValueType
                    ? current.Equals(expected)
                    : ReferenceEquals(current, expected);
            }
        }

        private static string F(decimal value) =>
            value.ToString("0.############################", CultureInfo.InvariantCulture);
    }
}
