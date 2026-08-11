using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using DSPSeedScanner.Core;
using DSPSeedScanner.Runtime;

namespace DSPSeedScanner.Runtime.Tests
{
    internal static class Program
    {
        private static int Main()
        {
            var tests = new (string Name, Action Body)[]
            {
                ("supported topology reaches evaluator", SupportedTopologyReachesEvaluator),
                ("complete preview reaches all immediate families", CompletePreviewReachesAllImmediateFamilies),
                ("identity mismatches reject safely", IdentityMismatchesRejectSafely),
                ("member and mod uncertainty reject safely", MemberAndModUncertaintyRejectSafely),
                ("unsupported request identity rejects safely", UnsupportedRequestIdentityRejectsSafely),
                ("other star count preserves fixed and declines quantitative", OtherStarCountIsBounded),
                ("peace preview makes Dark Fog not applicable", PeacePreviewIsNotApplicable),
                ("altered combat preview retains facts as unknown", AlteredCombatPreviewIsUnknown),
                ("incomplete normalized preview fails closed", IncompleteNormalizedPreviewFailsClosed),
                ("unknown enum preserves raw diagnostic", UnknownEnumPreservesRawDiagnostic),
                ("thread affinity rejects before runtime access", ThreadAffinityRejectsBeforeRuntimeAccess),
                ("success failure and cancellation restore state", ExitPathsRestoreState),
                ("concurrent request receives busy rejection", ConcurrentRequestReceivesBusy),
                ("runtime boundary exposes no game objects", RuntimeBoundaryExposesNoGameObjects)
            };

            int failures = 0;
            foreach ((string name, Action body) in tests)
            {
                try
                {
                    body();
                    Console.WriteLine("PASS {0}", name);
                }
                catch (Exception exception)
                {
                    failures++;
                    Console.Error.WriteLine("FAIL {0}: {1}", name, exception);
                }
            }
            Console.WriteLine("RESULT {0}/{1} passed", tests.Length - failures, tests.Length);
            return failures == 0 ? 0 : 1;
        }

        private static void SupportedTopologyReachesEvaluator()
        {
            var gateway = new FakeGateway();
            RuntimeScanResult result = new PreviewScanCoordinator(gateway).TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Success, result.Status);
            Equal(ComponentOutcome.Supports, result.Conclusion?.Outcome);
            Equal("3", result.Conclusion?.DecisiveFact?.Value);
            Equal(16_315_224, result.GalaxySeed);
            True(result.StateRestored);
            True(result.Trace.Any(value => value.StartsWith("generate:thread=", StringComparison.Ordinal)));
            Equal(1, gateway.GenerateCalls);
        }

        private static void CompletePreviewReachesAllImmediateFamilies()
        {
            RuntimeScanResult result = new PreviewScanCoordinator(new FakeGateway())
                .TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Success, result.Status);
            Equal(64, result.GeneratedStarCount);
            AssertReport(result, "FS-TOPOLOGY.shared-satellites", ComponentOutcome.Supports);
            AssertReport(result, "FS-POWER.birth-tidal", ComponentOutcome.Supports);
            AssertReport(result, "FS-POWER.solar", ComponentOutcome.Supports);
            AssertReport(result, "FS-GAS-ROUTE.product:hydrogen", ComponentOutcome.Supports);
            AssertReport(result, "MF-ENERGY-SYSTEM.output", ComponentOutcome.Supports);
            AssertReport(result, "MF-SPHERE-GEOMETRY.radius", ComponentOutcome.Supports);
            AssertReport(result, "DF-OCCUPATION.opportunity", ComponentOutcome.Supports);
            AssertReport(result, "DF-OCCUPATION.tradeoff", ComponentOutcome.Tradeoff);
            AssertReport(result, "CX-GROUPING.distance", ComponentOutcome.Supports);
            True(result.Reports.Any(report => report.ConclusionId ==
                "MF-SYSTEM-ROLE.role:strong-energy"));
            True(result.Reports.Any(report => report.ConclusionId ==
                "TRAIT-SUMMARY.registry:close-strong-energy-system"));
            AssertReport(result, "FS-RESOURCES.common-total", ComponentOutcome.Unknown);
            False(result.Trace.Any(value =>
                value.IndexOf("raw", StringComparison.OrdinalIgnoreCase) >= 0));

            ConclusionReport solar = FindReport(result, "FS-POWER.solar");
            Equal(EvidenceStage.GalaxyPreview, solar.Stage);
            Equal(EvidenceScope.BirthSystemPower, solar.Coverage.Scope);
            Equal(1m, solar.Settings.ResourceMultiplier);
            foreach (ConclusionReport report in result.Reports)
            {
                Equal(ConclusionDefinition.ContractVersion, report.ContractVersion);
                Equal(ConclusionDefinition.DefinitionVersion, report.DefinitionVersion);
                True(!String.IsNullOrWhiteSpace(report.Subject.Identifier));
            }
        }

        private static void IdentityMismatchesRejectSafely()
        {
            AssertRejected(Fingerprint(assembly: "BAD"), "assembly-mismatch");
            AssertRejected(Fingerprint(themes: ConclusionDefinition.ReferenceOrderedThemeIds.Split(',').Reverse()), "theme-catalogue-mismatch");
            AssertRejected(Fingerprint(gameVersion: "0.10.34.0"), "game-version-mismatch");
            AssertRejected(Fingerprint(algorithm: 1), "galaxy-algorithm-mismatch");
        }

        private static void MemberAndModUncertaintyRejectSafely()
        {
            AssertRejected(Fingerprint(members: false, missing: "UniverseGen.CreateGalaxy"), "missing-runtime-member");
            AssertRejected(Fingerprint(mods: new[] { "example.generation.patch" }), "generation-mod-uncertain");
        }

        private static void UnsupportedRequestIdentityRejectsSafely()
        {
            var gateway = new FakeGateway();
            var request = new PreviewScanRequest(
                16_315_224,
                64,
                "0.10.33.0",
                1m,
                CombatMode.Combat,
                ConclusionDefinition.ReferenceCombatSettingsKey);
            RuntimeScanResult result = new PreviewScanCoordinator(gateway).TryScan(request, CancellationToken.None);
            Equal(RuntimeScanStatus.Incompatible, result.Status);
            Equal("request-identity-unsupported", result.Code);
            Equal(0, gateway.GenerateCalls);
            Equal(0, gateway.RestoreCalls);
        }

        private static void OtherStarCountIsBounded()
        {
            var gateway = new FakeGateway { Snapshot = Snapshot(32) };
            var request = new PreviewScanRequest(
                16_315_224,
                32,
                ConclusionDefinition.ReferenceGameVersion,
                1m,
                CombatMode.Combat,
                ConclusionDefinition.ReferenceCombatSettingsKey);
            RuntimeScanResult result = new PreviewScanCoordinator(gateway)
                .TryScan(request, CancellationToken.None);
            Equal(RuntimeScanStatus.Success, result.Status);
            AssertReport(result, "FS-TOPOLOGY.shared-satellites", ComponentOutcome.Supports);
            AssertReport(result, "FS-POWER.solar", ComponentOutcome.Unknown);
            Equal("unsupported-definition-scope",
                FindReport(result, "FS-POWER.solar").DiagnosticCause?.Code);
        }

        private static void PeacePreviewIsNotApplicable()
        {
            var request = new PreviewScanRequest(
                16_315_224,
                64,
                ConclusionDefinition.ReferenceGameVersion,
                1m,
                CombatMode.Peace,
                ConclusionDefinition.ReferenceCombatSettingsKey);
            RuntimeScanResult result = new PreviewScanCoordinator(new FakeGateway())
                .TryScan(request, CancellationToken.None);
            Equal(RuntimeScanStatus.Success, result.Status);
            AssertReport(result, "DF-OCCUPATION.opportunity", ComponentOutcome.NotApplicable);
            AssertReport(result, "DF-OCCUPATION.birth-exposure", ComponentOutcome.NotApplicable);
            False(result.Reports.Any(report =>
                report.ConclusionId == "DF-OCCUPATION.tradeoff"));
        }

        private static void AlteredCombatPreviewIsUnknown()
        {
            string key = PreviewScanRequest.CombatSettingsKeyFor(2m, 1m);
            var request = new PreviewScanRequest(
                16_315_224,
                64,
                ConclusionDefinition.ReferenceGameVersion,
                1m,
                CombatMode.Combat,
                key,
                initialColonize: 2m,
                maxDensity: 1m);
            RuntimeScanResult result = new PreviewScanCoordinator(new FakeGateway())
                .TryScan(request, CancellationToken.None);
            Equal(RuntimeScanStatus.Success, result.Status);
            ConclusionReport opportunity = FindReport(result, "DF-OCCUPATION.opportunity");
            Equal(ComponentOutcome.Unknown, opportunity.Outcome);
            Equal("unsupported-definition-scope", opportunity.DiagnosticCause?.Code);
            True(opportunity.DecisiveFact != null);
            Equal(key, opportunity.Settings.CombatSettingsKey);
            AssertReport(result, "DF-OCCUPATION.birth-exposure", ComponentOutcome.Caution);
        }

        private static void IncompleteNormalizedPreviewFailsClosed()
        {
            RuntimePreviewSnapshot complete = Snapshot();
            var gateway = new FakeGateway
            {
                Snapshot = new RuntimePreviewSnapshot(
                    complete.BirthSystemIdentifier,
                    complete.GeneratedStarCount,
                    complete.Systems,
                    complete.SystemDistances.Take(1))
            };
            RuntimeScanResult result = new PreviewScanCoordinator(gateway)
                .TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Failed, result.Status);
            Equal("normalized-distance-coverage-mismatch", result.Code);
            Equal(0, result.Reports.Count);
            True(result.StateRestored);
        }

        private static void UnknownEnumPreservesRawDiagnostic()
        {
            var gateway = new FakeGateway
            {
                Snapshot = new RuntimePreviewSnapshot(
                    "1",
                    64,
                    Array.Empty<NormalizedSystemEvidence>(),
                    Array.Empty<NormalizedSystemDistance>(),
                    "EPlanetType",
                    99)
            };
            RuntimeScanResult result = new PreviewScanCoordinator(gateway).TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Incompatible, result.Status);
            Equal("unknown-runtime-enum", result.Code);
            Equal("EPlanetType=99", result.RawDiagnostic);
            Equal(null, result.Conclusion);
            True(result.StateRestored);
        }

        private static void ThreadAffinityRejectsBeforeRuntimeAccess()
        {
            var gateway = new FakeGateway { MainThreadIdOverride = -1 };
            RuntimeScanResult result = new PreviewScanCoordinator(gateway).TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Incompatible, result.Status);
            Equal("thread-affinity-mismatch", result.Code);
            Equal(0, gateway.FingerprintCalls);
            Equal(0, gateway.GenerateCalls);
        }

        private static void ExitPathsRestoreState()
        {
            var success = new FakeGateway();
            RuntimeScanResult successResult = new PreviewScanCoordinator(success).TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Success, successResult.Status);
            Equal(1, success.RestoreCalls);
            Equal("original", success.StateMarker);

            var failure = new FakeGateway { GenerationFailure = new InvalidOperationException("injected") };
            RuntimeScanResult failureResult = new PreviewScanCoordinator(failure).TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Failed, failureResult.Status);
            Equal("runtime-exception", failureResult.Code);
            True(failureResult.Message.Contains("injected", StringComparison.Ordinal));
            Equal(1, failure.RestoreCalls);
            Equal("original", failure.StateMarker);

            var cancelled = new FakeGateway();
            using var source = new CancellationTokenSource();
            source.Cancel();
            RuntimeScanResult cancelledResult = new PreviewScanCoordinator(cancelled).TryScan(Request(), source.Token);
            Equal(RuntimeScanStatus.Cancelled, cancelledResult.Status);
            Equal(0, cancelled.GenerateCalls);
            Equal(1, cancelled.RestoreCalls);
            Equal("original", cancelled.StateMarker);
        }

        private static void ConcurrentRequestReceivesBusy()
        {
            var gateway = new FakeGateway();
            var coordinator = new PreviewScanCoordinator(gateway);
            RuntimeScanResult? nested = null;
            gateway.OnGenerate = () => nested = coordinator.TryScan(Request(), CancellationToken.None);
            RuntimeScanResult outer = coordinator.TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Success, outer.Status);
            Equal(RuntimeScanStatus.Busy, nested?.Status);
            Equal("busy", nested?.Code);
            Equal(1, gateway.GenerateCalls);
            Equal(1, gateway.RestoreCalls);
        }

        private static void RuntimeBoundaryExposesNoGameObjects()
        {
            Assembly assembly = typeof(PreviewScanCoordinator).Assembly;
            string[] forbidden = { "Assembly-CSharp", "UnityEngine", "BepInEx" };
            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
                False(forbidden.Any(value => reference.Name?.StartsWith(value, StringComparison.Ordinal) == true));

            foreach (Type type in new[] { typeof(RuntimeScanResult), typeof(RuntimeFingerprint), typeof(RuntimePreviewSnapshot) })
            {
                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    False(forbidden.Any(value => property.PropertyType.Assembly.GetName().Name?.StartsWith(value, StringComparison.Ordinal) == true));
            }
        }

        private static void AssertRejected(RuntimeFingerprint fingerprint, string code)
        {
            var gateway = new FakeGateway { Fingerprint = fingerprint };
            RuntimeScanResult result = new PreviewScanCoordinator(gateway).TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Incompatible, result.Status);
            Equal(code, result.Code);
            Equal(0, gateway.GenerateCalls);
            Equal(0, gateway.RestoreCalls);
        }

        private static PreviewScanRequest Request()
        {
            return new PreviewScanRequest(
                16_315_224,
                ConclusionDefinition.ReferenceStarCount,
                ConclusionDefinition.ReferenceGameVersion,
                1m,
                CombatMode.Combat,
                ConclusionDefinition.ReferenceCombatSettingsKey);
        }

        private static RuntimeFingerprint Fingerprint(
            string? gameVersion = null,
            int? algorithm = null,
            string? assembly = null,
            IEnumerable<string>? themes = null,
            bool members = true,
            string? missing = null,
            IEnumerable<string>? mods = null)
        {
            return new RuntimeFingerprint(
                gameVersion ?? ConclusionDefinition.ReferenceGameVersion,
                algorithm ?? ConclusionDefinition.ReferenceGalaxyAlgorithm,
                assembly ?? ConclusionDefinition.ReferenceAssemblySha256,
                themes ?? ConclusionDefinition.ReferenceOrderedThemeIds.Split(','),
                ConclusionDefinition.DefinitionVersion,
                ConclusionDefinition.ContractVersion,
                members,
                missing,
                mods);
        }

        private static RuntimePreviewSnapshot Snapshot(int generatedStarCount = 64)
        {
            var systems = new List<NormalizedSystemEvidence>();
            for (int index = 0; index < generatedStarCount; index++)
            {
                bool birth = index == 0;
                bool leader = index == 1;
                systems.Add(new NormalizedSystemEvidence(
                    new ConclusionSubject(
                        birth ? SubjectKind.BirthSystem : SubjectKind.StarSystem,
                        (index + 1).ToString()),
                    birth,
                    birth ? 3 : null,
                    birth ? true : null,
                    birth ? 1.35m : null,
                    birth ? 1.5m : null,
                    birth ? new[] { new NormalizedGasProduct("hydrogen", 0.5m) } : null,
                    leader ? 2.698m : 1m,
                    leader ? 234_200 : 50_000,
                    leader ? 4 : 0,
                    birth ? 1 : leader ? 39 : 0));
            }

            var distances = new List<NormalizedSystemDistance>();
            for (int first = 0; first < generatedStarCount; first++)
            {
                for (int second = first + 1; second < generatedStarCount; second++)
                {
                    distances.Add(new NormalizedSystemDistance(
                        (first + 1).ToString(),
                        (second + 1).ToString(),
                        first == 0 && second == 1 ? 2m : 20m));
                }
            }
            return new RuntimePreviewSnapshot(
                "1",
                generatedStarCount,
                systems,
                distances);
        }

        private static ConclusionReport FindReport(
            RuntimeScanResult result,
            string conclusionId)
        {
            ConclusionReport[] reports = result.Reports
                .Where(report => report.ConclusionId == conclusionId)
                .ToArray();
            if (reports.Length != 1)
                throw new InvalidOperationException(
                    "Expected one report for " + conclusionId + ", found " + reports.Length + ".");
            return reports[0];
        }

        private static void AssertReport(
            RuntimeScanResult result,
            string conclusionId,
            ComponentOutcome outcome)
        {
            if (!result.Reports.Any(report =>
                report.ConclusionId == conclusionId && report.Outcome == outcome))
            {
                throw new InvalidOperationException(
                    "Expected " + conclusionId + " to include " + outcome + ".");
            }
        }

        private sealed class FakeGateway : IRuntimePreviewGateway
        {
            public RuntimeFingerprint Fingerprint { get; set; } = Program.Fingerprint();
            public RuntimePreviewSnapshot Snapshot { get; set; } = Program.Snapshot();
            public Exception? GenerationFailure { get; set; }
            public Action? OnGenerate { get; set; }
            public int? MainThreadIdOverride { get; set; }
            public int FingerprintCalls { get; private set; }
            public int GenerateCalls { get; private set; }
            public int RestoreCalls { get; private set; }
            public string StateMarker { get; set; } = "original";
            public int MainThreadId => MainThreadIdOverride ?? Thread.CurrentThread.ManagedThreadId;

            public RuntimeFingerprint CaptureFingerprint(PreviewScanRequest request)
            {
                FingerprintCalls++;
                return Fingerprint;
            }

            public RuntimeStateLease CaptureState()
            {
                string original = StateMarker;
                StateMarker = "leased";
                return new FakeLease(this, original);
            }

            public RuntimePreviewSnapshot GeneratePreview(
                PreviewScanRequest request,
                CancellationToken cancellationToken,
                Action<string> recordTrace)
            {
                GenerateCalls++;
                recordTrace("generate:thread=" + Thread.CurrentThread.ManagedThreadId);
                StateMarker = "generated";
                OnGenerate?.Invoke();
                if (GenerationFailure != null)
                    throw GenerationFailure;
                return Snapshot;
            }

            private sealed class FakeLease : RuntimeStateLease
            {
                private readonly FakeGateway owner;
                private readonly string original;
                private bool restored;

                public FakeLease(FakeGateway owner, string original)
                {
                    this.owner = owner;
                    this.original = original;
                }

                public override bool Restored => restored;

                public override void Dispose()
                {
                    owner.StateMarker = original;
                    owner.RestoreCalls++;
                    restored = true;
                }
            }
        }

        private static void Equal<T>(T expected, T actual)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new InvalidOperationException("Expected " + expected + ", got " + actual + ".");
        }

        private static void True(bool value)
        {
            if (!value) throw new InvalidOperationException("Expected true.");
        }

        private static void False(bool value)
        {
            if (value) throw new InvalidOperationException("Expected false.");
        }
    }
}
