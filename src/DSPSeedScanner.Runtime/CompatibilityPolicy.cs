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
            if (!String.Equals(fingerprint.ScannerCompatibilityVersion, ConclusionDefinition.DefinitionVersion, StringComparison.Ordinal))
                return Reject("scanner-compatibility-mismatch", fingerprint.ScannerCompatibilityVersion);
            if (!String.Equals(fingerprint.ScannerContractVersion, ConclusionDefinition.ContractVersion, StringComparison.Ordinal))
                return Reject("scanner-contract-mismatch", fingerprint.ScannerContractVersion);

            return new CompatibilityDecision(true, "supported", "The runtime fingerprint is supported.");
        }

        public static CompatibilityDecision EvaluateRequest(
            PreviewScanRequest request,
            RuntimeFingerprint fingerprint)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (fingerprint == null)
                throw new ArgumentNullException(nameof(fingerprint));
            if (!String.Equals(
                request.CreationVersion,
                fingerprint.GameVersion,
                StringComparison.Ordinal))
            {
                return Reject(
                    "request-identity-unsupported",
                    "The request creation version does not match the running game.");
            }
            return new CompatibilityDecision(true, "supported", "The requested identity is supported.");
        }

        public static string? UnverifiedNotice(RuntimeFingerprint fingerprint)
        {
            if (fingerprint == null)
                throw new ArgumentNullException(nameof(fingerprint));
            bool referenceVersion = String.Equals(fingerprint.GameVersion,
                ConclusionDefinition.ReferenceGameVersion, StringComparison.Ordinal);
            if (referenceVersion &&
                fingerprint.GalaxyAlgorithm == ConclusionDefinition.ReferenceGalaxyAlgorithm &&
                String.Equals(fingerprint.AssemblySha256,
                    ConclusionDefinition.ReferenceAssemblySha256, StringComparison.OrdinalIgnoreCase) &&
                String.Equals(fingerprint.GenerationMethodIlSha256,
                    ConclusionDefinition.ReferenceGenerationMethodIlSha256, StringComparison.OrdinalIgnoreCase) &&
                String.Equals(fingerprint.OrderedThemeIdsKey,
                    ConclusionDefinition.ReferenceOrderedThemeIds, StringComparison.Ordinal))
                return null;

            return (referenceVersion ? "Unverified game identity" : "Unverified game version") +
                " - results may be inaccurate.\nCurrent DSP " + fingerprint.GameVersion +
                " | Reference DSP " + ConclusionDefinition.ReferenceGameVersion;
        }

        private static CompatibilityDecision Reject(string code, string detail)
        {
            return new CompatibilityDecision(false, code, detail);
        }
    }
}
