using System;
using System.Globalization;

namespace DSPSeedScanner.Core
{
    public static class SharedSatelliteEvaluator
    {
        public const string ConclusionId = "FS-TOPOLOGY.shared-satellites";
        public const string ContractVersion = "0.1.0";
        public const string DefinitionVersion = "0.1.0";

        public static ConclusionReport Evaluate(
            NormalizedBirthTopologyEvidence evidence)
        {
            if (evidence == null)
                throw new ArgumentNullException(nameof(evidence));

            if (!evidence.Coverage.IsComplete ||
                !evidence.SharedBirthGiantBodies.HasValue)
            {
                return new ConclusionReport(
                    evidence.Identity,
                    evidence.Settings,
                    evidence.Coverage,
                    ConclusionId,
                    ConclusionContext.FreshStart,
                    ContractVersion,
                    DefinitionVersion,
                    evidence.Subject,
                    ComponentOutcome.Unknown,
                    null,
                    new DiagnosticCause(
                        "incomplete-coverage",
                        "Complete birth-system topology evidence is required."));
            }

            int count = evidence.SharedBirthGiantBodies.Value;
            return new ConclusionReport(
                evidence.Identity,
                evidence.Settings,
                evidence.Coverage,
                ConclusionId,
                ConclusionContext.FreshStart,
                ContractVersion,
                DefinitionVersion,
                evidence.Subject,
                count >= 2
                    ? ComponentOutcome.Supports
                    : ComponentOutcome.DoesNotSupport,
                new DecisiveFact(
                    "sharedBirthGiantBodies",
                    count.ToString(CultureInfo.InvariantCulture),
                    "bodies"),
                null);
        }
    }
}
