using System;
using System.Collections.Generic;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    internal static class RuntimeConclusionEvaluator
    {
        public static IReadOnlyList<ConclusionReport> Evaluate(
            PreviewScanRequest request,
            RuntimeFingerprint fingerprint,
            RuntimePreviewSnapshot snapshot,
            NormalizedStarterResourceEvidence? starterResources = null,
            EvidenceCoverage? starterCoverage = null)
        {
            var identity = new GenerationIdentity(
                fingerprint.GameVersion,
                fingerprint.GalaxyAlgorithm,
                fingerprint.AssemblySha256,
                fingerprint.OrderedThemeIdsKey,
                fingerprint.ScannerCompatibilityVersion,
                request.GalaxySeed,
                request.RequestedStarCount,
                request.CreationVersion);
            var settings = new EvaluationSettings(
                request.ResourceMultiplier,
                request.CombatMode,
                request.CombatSettingsKey);
            int systemCount = snapshot.Systems.Count;
            int distanceCount = snapshot.SystemDistances.Count;
            var coverages = new List<EvidenceCoverage>
            {
                Complete(EvidenceScope.BirthSystemTopology, 1),
                Complete(EvidenceScope.BirthSystemRotation, 1),
                Complete(EvidenceScope.BirthSystemPower, 1),
                Complete(EvidenceScope.BirthSystemGasProducts, 1),
                Complete(EvidenceScope.ClusterEnergy, systemCount),
                Complete(EvidenceScope.ClusterSphereGeometry, systemCount),
                Complete(EvidenceScope.ClusterOccupation, systemCount),
                Complete(EvidenceScope.SystemDistances, Math.Max(1, distanceCount))
            };
            if (starterCoverage != null)
                coverages.Add(starterCoverage);

            var evidence = new NormalizedClusterEvidence(
                identity,
                settings,
                new ConclusionSubject(SubjectKind.Cluster, request.GalaxySeed + ":cluster"),
                snapshot.BirthSystemIdentifier,
                coverages,
                snapshot.Systems,
                starterResources,
                systemDistances: snapshot.SystemDistances);
            return ConclusionEngine.Evaluate(evidence);
        }

        private static EvidenceCoverage Complete(EvidenceScope scope, int subjects) =>
            new EvidenceCoverage(
                EvidenceStage.GalaxyPreview,
                scope,
                CoverageState.Complete,
                subjects,
                subjects);
    }
}
