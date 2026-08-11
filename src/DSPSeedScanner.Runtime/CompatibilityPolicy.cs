using System;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public sealed class CompatibilityDecision
    {
        public CompatibilityDecision(bool supported, string code, string message)
        {
            Supported = supported;
            Code = code;
            Message = message;
        }

        public bool Supported { get; }
        public string Code { get; }
        public string Message { get; }
    }

    public static class CompatibilityPolicy
    {
        public static CompatibilityDecision Evaluate(RuntimeFingerprint fingerprint)
        {
            if (fingerprint == null)
                throw new ArgumentNullException(nameof(fingerprint));

            if (!fingerprint.RequiredMembersAvailable)
                return Reject("missing-runtime-member", fingerprint.MissingMember ?? "Required member unavailable.");
            if (fingerprint.LoadedGenerationModIds.Count != 0)
                return Reject("generation-mod-uncertain", String.Join(",", fingerprint.LoadedGenerationModIds));
            if (!String.Equals(fingerprint.GameVersion, ConclusionDefinition.ReferenceGameVersion, StringComparison.Ordinal))
                return Reject("game-version-mismatch", fingerprint.GameVersion);
            if (fingerprint.GalaxyAlgorithm != ConclusionDefinition.ReferenceGalaxyAlgorithm)
                return Reject("galaxy-algorithm-mismatch", fingerprint.GalaxyAlgorithm.ToString());
            if (!String.Equals(fingerprint.AssemblySha256, ConclusionDefinition.ReferenceAssemblySha256, StringComparison.OrdinalIgnoreCase))
                return Reject("assembly-mismatch", fingerprint.AssemblySha256);
            if (!String.Equals(fingerprint.OrderedThemeIdsKey, ConclusionDefinition.ReferenceOrderedThemeIds, StringComparison.Ordinal))
                return Reject("theme-catalogue-mismatch", fingerprint.OrderedThemeIdsKey);
            if (!String.Equals(fingerprint.ScannerCompatibilityVersion, ConclusionDefinition.DefinitionVersion, StringComparison.Ordinal))
                return Reject("scanner-compatibility-mismatch", fingerprint.ScannerCompatibilityVersion);
            if (!String.Equals(fingerprint.ScannerContractVersion, ConclusionDefinition.ContractVersion, StringComparison.Ordinal))
                return Reject("scanner-contract-mismatch", fingerprint.ScannerContractVersion);

            return new CompatibilityDecision(true, "supported", "The runtime fingerprint is supported.");
        }

        public static CompatibilityDecision EvaluateRequest(PreviewScanRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (!String.Equals(
                request.CreationVersion,
                ConclusionDefinition.ReferenceGameVersion,
                StringComparison.Ordinal))
            {
                return Reject(
                    "request-identity-unsupported",
                    "Only the accepted creation version is supported.");
            }
            return new CompatibilityDecision(true, "supported", "The requested identity is supported.");
        }

        private static CompatibilityDecision Reject(string code, string detail)
        {
            return new CompatibilityDecision(false, code, detail);
        }
    }
}
