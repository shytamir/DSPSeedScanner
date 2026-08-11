using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Core.Tests
{
    internal static class Program
    {
        private const string AssemblyHash =
            "AE0BA95F75BD879A62AA4CE253B2AB78EAA4FB3C7C595F5E1FEE75EBE0E0EF85";

        private static int Main()
        {
            var tests = new (string Name, Action Body)[]
            {
                ("supporting fixture", SupportingFixture),
                ("non-supporting fixture", NonSupportingFixture),
                ("missing coverage", MissingCoverage),
                ("partial coverage", PartialCoverage),
                ("identity and stage keys", IdentityAndStageKeys),
                ("deterministic reports", DeterministicReports),
                ("runtime-independent references", RuntimeIndependentReferences)
            };

            var failures = new List<string>();
            foreach ((string name, Action body) in tests)
            {
                try
                {
                    body();
                    Console.WriteLine("PASS {0}", name);
                }
                catch (Exception exception)
                {
                    failures.Add(name + ": " + exception.Message);
                    Console.Error.WriteLine("FAIL {0}: {1}", name, exception);
                }
            }

            Console.WriteLine(
                "RESULT {0}/{1} passed",
                tests.Length - failures.Count,
                tests.Length);
            return failures.Count == 0 ? 0 : 1;
        }

        private static void SupportingFixture()
        {
            NormalizedBirthTopologyEvidence evidence = CompleteEvidence(
                seed: 16_315_224,
                sharedBodies: 3);

            ConclusionReport report = SharedSatelliteEvaluator.Evaluate(evidence);

            Equal(ComponentOutcome.Supports, report.Outcome);
            Equal("16315224:birth-system", report.Subject.Identifier);
            Equal("3", report.DecisiveFact?.Value);
            Equal("bodies", report.DecisiveFact?.Unit);
            Equal(SharedSatelliteEvaluator.ConclusionId, report.ConclusionId);
            Equal("0.1.0", report.ContractVersion);
            Equal("0.1.0", report.DefinitionVersion);
            Equal(EvidenceStage.GalaxyPreview, report.Stage);
            True(report.Coverage.IsComplete, "Coverage should be complete.");
            Null(report.DiagnosticCause);
        }

        private static void NonSupportingFixture()
        {
            ConclusionReport report = SharedSatelliteEvaluator.Evaluate(
                CompleteEvidence(seed: 73_339_583, sharedBodies: 1));

            Equal(ComponentOutcome.DoesNotSupport, report.Outcome);
            Equal("1", report.DecisiveFact?.Value);
            Null(report.DiagnosticCause);
        }

        private static void MissingCoverage()
        {
            GenerationIdentity identity = Identity(seed: 16_315_224);
            var coverage = new EvidenceCoverage(
                EvidenceStage.GalaxyPreview,
                EvidenceScope.BirthSystemTopology,
                CoverageState.Unavailable,
                expectedSubjects: 1,
                completedSubjects: 0);
            var evidence = new NormalizedBirthTopologyEvidence(
                identity,
                coverage,
                Subject(identity),
                sharedBirthGiantBodies: null);

            ConclusionReport report = SharedSatelliteEvaluator.Evaluate(evidence);

            Equal(ComponentOutcome.Unknown, report.Outcome);
            Equal("incomplete-coverage", report.DiagnosticCause?.Code);
            Null(report.DecisiveFact);
            Equal(identity, report.Identity);
            Equal(coverage, report.Coverage);
        }

        private static void IdentityAndStageKeys()
        {
            GenerationIdentity first = Identity(seed: 16_315_224);
            GenerationIdentity equal = Identity(seed: 16_315_224);
            GenerationIdentity differentThemes = Identity(
                seed: 16_315_224,
                orderedThemeIds: "25,24,23,22,21,20,19,18,17,16,15,14,13,12,11,10,9,8,7,6,5,4,3,2,1");

            Equal(first, equal);
            NotEqual(first, differentThemes);
            Equal(
                new EvidenceKey(first, EvidenceStage.GalaxyPreview),
                new EvidenceKey(equal, EvidenceStage.GalaxyPreview));
            NotEqual(
                new EvidenceKey(first, EvidenceStage.GalaxyPreview),
                new EvidenceKey(equal, EvidenceStage.BirthSystemRaw));
        }

        private static void PartialCoverage()
        {
            GenerationIdentity identity = Identity(seed: 16_315_224);
            var evidence = new NormalizedBirthTopologyEvidence(
                identity,
                new EvidenceCoverage(
                    EvidenceStage.GalaxyPreview,
                    EvidenceScope.BirthSystemTopology,
                    CoverageState.Partial,
                    expectedSubjects: 2,
                    completedSubjects: 1),
                Subject(identity),
                sharedBirthGiantBodies: 1);

            ConclusionReport report = SharedSatelliteEvaluator.Evaluate(evidence);

            Equal(ComponentOutcome.Unknown, report.Outcome);
            Equal("incomplete-coverage", report.DiagnosticCause?.Code);
            Null(report.DecisiveFact);
        }

        private static void DeterministicReports()
        {
            NormalizedBirthTopologyEvidence first = CompleteEvidence(
                seed: 16_315_224,
                sharedBodies: 3);
            NormalizedBirthTopologyEvidence equal = CompleteEvidence(
                seed: 16_315_224,
                sharedBodies: 3);

            ConclusionReport firstReport = SharedSatelliteEvaluator.Evaluate(first);
            ConclusionReport equalReport = SharedSatelliteEvaluator.Evaluate(equal);

            Equal(firstReport, equalReport);
            Equal(firstReport.GetHashCode(), equalReport.GetHashCode());
        }

        private static void RuntimeIndependentReferences()
        {
            string[] forbiddenPrefixes = { "Assembly-CSharp", "BepInEx", "UnityEngine" };
            string[] references = typeof(GenerationIdentity).Assembly
                .GetReferencedAssemblies()
                .Select(reference => reference.Name ?? String.Empty)
                .ToArray();

            foreach (string prefix in forbiddenPrefixes)
            {
                True(
                    references.All(reference =>
                        !reference.StartsWith(prefix, StringComparison.Ordinal)),
                    "Core assembly references forbidden runtime dependency " + prefix + ".");
            }
        }

        private static NormalizedBirthTopologyEvidence CompleteEvidence(
            int seed,
            int sharedBodies)
        {
            GenerationIdentity identity = Identity(seed);
            return new NormalizedBirthTopologyEvidence(
                identity,
                new EvidenceCoverage(
                    EvidenceStage.GalaxyPreview,
                    EvidenceScope.BirthSystemTopology,
                    CoverageState.Complete,
                    expectedSubjects: 1,
                    completedSubjects: 1),
                Subject(identity),
                sharedBodies);
        }

        private static GenerationIdentity Identity(
            int seed,
            string orderedThemeIds =
                "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25")
        {
            return new GenerationIdentity(
                gameVersion: "0.10.34.28529",
                galaxyAlgorithm: 20_200_403,
                assemblySha256: AssemblyHash,
                orderedThemeIds: orderedThemeIds,
                scannerCompatibilityVersion: "0.1.0",
                galaxySeed: seed,
                requestedStarCount: 64,
                creationVersion: "0.10.34.28529");
        }

        private static ConclusionSubject Subject(GenerationIdentity identity)
        {
            return new ConclusionSubject(
                SubjectKind.BirthSystem,
                identity.GalaxySeed.ToString(CultureInfo.InvariantCulture) +
                ":birth-system");
        }

        private static void Equal<T>(T expected, T actual)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new InvalidOperationException(
                    "Expected '" + expected + "' but found '" + actual + "'.");
        }

        private static void NotEqual<T>(T first, T second)
        {
            if (EqualityComparer<T>.Default.Equals(first, second))
                throw new InvalidOperationException("Values should differ.");
        }

        private static void True(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }

        private static void Null(object? value)
        {
            if (value != null)
                throw new InvalidOperationException("Expected null.");
        }
    }
}
