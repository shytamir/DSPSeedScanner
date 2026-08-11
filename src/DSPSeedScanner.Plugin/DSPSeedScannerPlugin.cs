using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using BepInEx;
using DSPSeedScanner.Core;
using DSPSeedScanner.Runtime;
using UnityEngine;

namespace DSPSeedScanner.Plugin
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    public sealed class DSPSeedScannerPlugin : BaseUnityPlugin
    {
        public const string PluginGuid = "io.github.shytamir.dspseedscanner";
        public const string PluginName = "DSP Seed Scanner";
        public const string PluginVersion = "0.1.0";

        private PreviewScanCoordinator? coordinator;
        private RawPlanetCoordinator? rawCoordinator;
        private BirthSystemRawCoordinator? birthSystemCoordinator;
        private DspRawPlanetGateway? rawGateway;
        private bool probeAttempted;

        private void Awake()
        {
            var operationGate = new RuntimeOperationGate();
            var previewGateway = new DspPreviewGateway(
                Thread.CurrentThread.ManagedThreadId,
                PluginGuid);
            rawGateway = new DspRawPlanetGateway(previewGateway);
            coordinator = new PreviewScanCoordinator(previewGateway, operationGate);
            rawCoordinator = new RawPlanetCoordinator(rawGateway, operationGate);
            birthSystemCoordinator = new BirthSystemRawCoordinator(rawGateway, operationGate);
            Logger.LogInfo("Runtime boundary initialized on managed thread " +
                Thread.CurrentThread.ManagedThreadId.ToString(CultureInfo.InvariantCulture) + ".");
        }

        private void Update()
        {
            string? output = Environment.GetEnvironmentVariable("DSP_SEED_SCANNER_PROBE_OUTPUT");
            if (probeAttempted || String.IsNullOrWhiteSpace(output) ||
                LDB.themes == null || LDB.themes.Length == 0)
            {
                return;
            }

            probeAttempted = true;
            try
            {
                string? mode = Environment.GetEnvironmentVariable("DSP_SEED_SCANNER_PROBE_MODE");
                if (String.Equals(mode, "birth-resources", StringComparison.Ordinal))
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
                        .Append(String.Join(",", result.Fingerprint.LoadedGenerationModIds))
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

        private static string F(decimal value) =>
            value.ToString("0.############################", CultureInfo.InvariantCulture);
    }
}
