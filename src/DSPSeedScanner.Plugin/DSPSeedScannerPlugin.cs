using System;
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
                int seed = ParseSeed(Environment.GetEnvironmentVariable("DSP_SEED_SCANNER_PROBE_SEED"));
                RuntimeScanResult result = ScanPreview(
                    new PreviewScanRequest(
                        seed,
                        ConclusionDefinition.ReferenceStarCount,
                        ConclusionDefinition.ReferenceGameVersion,
                        1m,
                        CombatMode.Combat,
                        ConclusionDefinition.ReferenceCombatSettingsKey),
                    CancellationToken.None);
                WriteProbe(output, result);
                Logger.LogInfo("IMPL-03 probe completed: " + output);
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

        private static int ParseSeed(string? value)
        {
            return Int32.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int seed)
                ? seed
                : 16_315_224;
        }

        private static void WriteProbe(string path, RuntimeScanResult result)
        {
            var lines = new StringBuilder();
            lines.Append("status\t").Append(result.Status).AppendLine();
            lines.Append("seed\t").Append(result.GalaxySeed.ToString(CultureInfo.InvariantCulture)).AppendLine();
            lines.Append("stage\t").Append(result.Stage).AppendLine();
            lines.Append("code\t").Append(result.Code).AppendLine();
            lines.Append("stateRestored\t").Append(result.StateRestored).AppendLine();
            if (result.Fingerprint != null)
            {
                lines.Append("gameVersion\t").Append(result.Fingerprint.GameVersion).AppendLine();
                lines.Append("galaxyAlgorithm\t").Append(result.Fingerprint.GalaxyAlgorithm.ToString(CultureInfo.InvariantCulture)).AppendLine();
                lines.Append("assemblySha256\t").Append(result.Fingerprint.AssemblySha256).AppendLine();
                lines.Append("orderedThemeIds\t").Append(result.Fingerprint.OrderedThemeIdsKey).AppendLine();
                lines.Append("loadedGenerationMods\t").Append(String.Join(",", result.Fingerprint.LoadedGenerationModIds)).AppendLine();
            }
            if (result.Conclusion != null)
            {
                lines.Append("conclusion\t").Append(result.Conclusion.ConclusionId).AppendLine();
                lines.Append("outcome\t").Append(result.Conclusion.Outcome).AppendLine();
                lines.Append("fact\t").Append(result.Conclusion.DecisiveFact?.Value).AppendLine();
            }
            for (int index = 0; index < result.Trace.Count; index++)
                lines.Append("trace\t").Append(index).Append('\t').Append(result.Trace[index]).AppendLine();
            File.WriteAllText(path, lines.ToString(), new UTF8Encoding(false));
        }
    }
}
