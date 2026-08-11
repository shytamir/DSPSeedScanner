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
        private bool probeAttempted;

        private void Awake()
        {
            coordinator = new PreviewScanCoordinator(
                new DspPreviewGateway(Thread.CurrentThread.ManagedThreadId, PluginGuid));
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
                var results = new List<RuntimeScanResult>();
                foreach (int seed in ParseSeeds())
                {
                    results.Add(ScanPreview(
                        new PreviewScanRequest(
                            seed,
                            ConclusionDefinition.ReferenceStarCount,
                            ConclusionDefinition.ReferenceGameVersion,
                            1m,
                            CombatMode.Combat,
                            ConclusionDefinition.ReferenceCombatSettingsKey),
                        CancellationToken.None));
                }
                WriteProbe(output, results);
                Logger.LogInfo("IMPL-04 preview probe completed: " + output);
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
    }
}
