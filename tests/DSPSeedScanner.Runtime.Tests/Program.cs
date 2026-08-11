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
                ("patcher and in-memory generation uncertainty reject safely", PatcherAndMethodUncertaintyRejectSafely),
                ("unsupported request identity rejects safely", UnsupportedRequestIdentityRejectsSafely),
                ("other star count preserves fixed and declines quantitative", OtherStarCountIsBounded),
                ("peace preview makes Dark Fog not applicable", PeacePreviewIsNotApplicable),
                ("altered combat preview retains facts as unknown", AlteredCombatPreviewIsUnknown),
                ("incomplete normalized preview fails closed", IncompleteNormalizedPreviewFailsClosed),
                ("unknown enum preserves raw diagnostic", UnknownEnumPreservesRawDiagnostic),
                ("thread affinity rejects before runtime access", ThreadAffinityRejectsBeforeRuntimeAccess),
                ("success failure and cancellation restore state", ExitPathsRestoreState),
                ("concurrent request receives busy rejection", ConcurrentRequestReceivesBusy),
                ("raw planet evidence is complete deterministic and immutable", RawPlanetEvidenceIsComplete),
                ("raw target mismatch fails closed", RawTargetMismatchFailsClosed),
                ("raw failure and boundary cancellation restore state", RawExitPathsRestoreState),
                ("raw compatibility diagnostics remain explicit", RawCompatibilityDiagnosticsRemainExplicit),
                ("preview and raw operations share serialization", PreviewAndRawShareSerialization),
                ("birth resources aggregate only complete declared coverage", BirthResourcesRequireCompleteCoverage),
                ("birth resource settings preserve facts but decline ranges", BirthResourceSettingsAreBounded),
                ("birth resource cancellation and failure retain diagnostics", BirthResourceExitPathsRetainDiagnostics),
                ("birth request shares serialization and preserves preview report", BirthRequestPreservesPreviewReport),
                ("complete cluster aggregates rare access and deferred strength", CompleteClusterAggregatesRareAccess),
                ("complete cluster partial exits expose no evidence", CompleteClusterPartialExitsExposeNoEvidence),
                ("complete cluster incompatibility remains explicit", CompleteClusterIncompatibilityIsExplicit),
                ("complete cluster bound rejects before raw generation", CompleteClusterBoundRejectsBeforeGeneration),
                ("complete cluster shares runtime serialization", CompleteClusterSharesSerialization),
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

        private static void PatcherAndMethodUncertaintyRejectSafely()
        {
            AssertRejected(
                Fingerprint(patchers: new[] { "example-preloader.dll:ABC" }),
                "generation-patcher-uncertain");
            AssertRejected(
                Fingerprint(methodIl: "BAD"),
                "generation-method-il-mismatch");
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

        private static void RawPlanetEvidenceIsComplete()
        {
            var gateway = new FakeRawGateway();
            RawPlanetResult result = new RawPlanetCoordinator(gateway)
                .TryGenerate(RawRequest(), CancellationToken.None);
            Equal(RuntimeScanStatus.Success, result.Status);
            Equal("success", result.Code);
            Equal(RawPlanetCoordinator.Stage, result.Stage);
            Equal(Request().ResourceMultiplier, result.Request.Identity.ResourceMultiplier);
            Equal(ConclusionDefinition.ReferenceGameVersion, result.Fingerprint?.GameVersion);
            True(result.Coverage.IsComplete);
            Equal(1, result.Coverage.ExpectedSubjects);
            Equal(1, result.Coverage.CompletedSubjects);
            True(result.StateRestored);
            Equal("original", gateway.StateMarker);
            Equal(2, result.Evidence?.Nodes.Count);
            Equal(1, result.Evidence?.Nodes[0].SourceIndex);
            Equal("oil", result.Evidence?.Nodes[1].ResourceId);
            Equal(RawResourceSemantics.OilFlow, result.Evidence?.Nodes[1].Semantics);
            Equal(1.25m, result.Evidence?.Nodes[1].OilSpeedMultiplier);
            Equal("runtime-oil-amount-units", result.Evidence?.Nodes[1].AmountUnit);
            Equal("dsp-planet-local-units", result.Evidence?.Nodes[1].PositionUnit);
            Equal(2, result.Evidence?.Groups.Count);
            True(result.Trace.Contains("raw:atomic:start"));
            True(result.Trace.Contains("raw:atomic:complete"));

            NormalizedRawVeinNode[] copy = result.Evidence?.Nodes.ToArray() ??
                Array.Empty<NormalizedRawVeinNode>();
            copy[0] = copy[1];
            Equal(1, result.Evidence?.Nodes[0].SourceIndex);
        }

        private static void RawTargetMismatchFailsClosed()
        {
            var gateway = new FakeRawGateway { Snapshot = RawSnapshot(planetId: 999) };
            RawPlanetResult result = new RawPlanetCoordinator(gateway)
                .TryGenerate(RawRequest(), CancellationToken.None);
            Equal(RuntimeScanStatus.Failed, result.Status);
            Equal("raw-target-mismatch", result.Code);
            Equal(null, result.Evidence);
            Equal(CoverageState.Unavailable, result.Coverage.State);
            True(result.StateRestored);
        }

        private static void RawExitPathsRestoreState()
        {
            var failure = new FakeRawGateway
            {
                GenerationFailure = new InvalidOperationException("injected raw failure")
            };
            RawPlanetResult failed = new RawPlanetCoordinator(failure)
                .TryGenerate(RawRequest(), CancellationToken.None);
            Equal(RuntimeScanStatus.Failed, failed.Status);
            Equal("raw-runtime-exception", failed.Code);
            Equal(null, failed.Evidence);
            Equal(CoverageState.Unavailable, failed.Coverage.State);
            Equal("original", failure.StateMarker);
            Equal(1, failure.RestoreCalls);

            var before = new FakeRawGateway();
            using (var source = new CancellationTokenSource())
            {
                source.Cancel();
                RawPlanetResult cancelled = new RawPlanetCoordinator(before)
                    .TryGenerate(RawRequest(), source.Token);
                Equal(RuntimeScanStatus.Cancelled, cancelled.Status);
                Equal(0, before.GenerateCalls);
                Equal(null, cancelled.Evidence);
                Equal(CoverageState.Unavailable, cancelled.Coverage.State);
                Equal(1, before.RestoreCalls);
            }

            var after = new FakeRawGateway();
            using (var source = new CancellationTokenSource())
            {
                after.OnAtomic = source.Cancel;
                RawPlanetResult cancelled = new RawPlanetCoordinator(after)
                    .TryGenerate(RawRequest(), source.Token);
                Equal(RuntimeScanStatus.Cancelled, cancelled.Status);
                Equal(1, after.GenerateCalls);
                Equal(null, cancelled.Evidence);
                Equal(CoverageState.Unavailable, cancelled.Coverage.State);
                Equal("original", after.StateMarker);
                Equal(1, after.RestoreCalls);
            }
        }

        private static void RawCompatibilityDiagnosticsRemainExplicit()
        {
            var gateway = new FakeRawGateway
            {
                GenerationFailure = new RawCompatibilityException(
                    "unknown-raw-resource-type",
                    "unsupported resource",
                    "EVeinType=99")
            };
            RawPlanetResult result = new RawPlanetCoordinator(gateway)
                .TryGenerate(RawRequest(), CancellationToken.None);
            Equal(RuntimeScanStatus.Incompatible, result.Status);
            Equal("unknown-raw-resource-type", result.Code);
            Equal("EVeinType=99", result.RawDiagnostic);
            Equal(null, result.Evidence);
            True(result.StateRestored);
        }

        private static void PreviewAndRawShareSerialization()
        {
            var gate = new RuntimeOperationGate();
            var previewGateway = new FakeGateway();
            var preview = new PreviewScanCoordinator(previewGateway, gate);
            var rawGateway = new FakeRawGateway();
            RuntimeScanResult? nested = null;
            rawGateway.OnAtomic = () => nested = preview.TryScan(Request(), CancellationToken.None);

            RawPlanetResult raw = new RawPlanetCoordinator(rawGateway, gate)
                .TryGenerate(RawRequest(), CancellationToken.None);
            Equal(RuntimeScanStatus.Success, raw.Status);
            Equal(RuntimeScanStatus.Busy, nested?.Status);
            Equal(0, previewGateway.GenerateCalls);
        }

        private static void BirthResourcesRequireCompleteCoverage()
        {
            var gateway = new FakeBirthGateway();
            var observed = new List<BirthSystemRawProgress>();
            BirthSystemRawResult result = new BirthSystemRawCoordinator(gateway)
                .TryGenerate(Request(), CancellationToken.None, observed.Add);
            Equal(RuntimeScanStatus.Success, result.Status);
            True(result.Coverage.IsComplete);
            Equal(2, result.Coverage.ExpectedPlanets);
            Equal(2, result.Coverage.CompletedPlanets);
            Equal(5, result.Progress.Count);
            Equal(result.Progress.Count, observed.Count);
            Equal(BirthSystemProgressState.Planned, result.Progress[0].State);
            Equal(BirthSystemProgressState.PlanetCompleted, result.Progress[4].State);
            Equal(42_000L, gateway.GeneratedAmount);
            ConclusionReport total = result.Reports.Single(report =>
                report.ConclusionId == "FS-RESOURCES.common-total");
            Equal(ComponentOutcome.DoesNotSupport, total.Outcome);
            Equal("40000", total.DecisiveFact?.Value);
            Equal(1, gateway.RestoreCalls);
            Equal("original", gateway.StateMarker);
        }

        private static void BirthResourceSettingsAreBounded()
        {
            var request = new PreviewScanRequest(
                16_315_224,
                ConclusionDefinition.ReferenceStarCount,
                ConclusionDefinition.ReferenceGameVersion,
                0.5m,
                CombatMode.Combat,
                ConclusionDefinition.ReferenceCombatSettingsKey);
            BirthSystemRawResult result = new BirthSystemRawCoordinator(new FakeBirthGateway())
                .TryGenerate(request, CancellationToken.None);
            Equal(RuntimeScanStatus.Success, result.Status);
            ConclusionReport total = result.Reports.Single(report =>
                report.ConclusionId == "FS-RESOURCES.common-total");
            Equal(ComponentOutcome.Unknown, total.Outcome);
            Equal("40000", total.DecisiveFact?.Value);
            Equal("unsupported-definition-scope", total.DiagnosticCause?.Code);
        }

        private static void BirthResourceExitPathsRetainDiagnostics()
        {
            var cancelledGateway = new FakeBirthGateway();
            using (var source = new CancellationTokenSource())
            {
                BirthSystemRawResult cancelled = new BirthSystemRawCoordinator(cancelledGateway)
                    .TryGenerate(Request(), source.Token, progress =>
                    {
                        if (progress.State == BirthSystemProgressState.PlanetCompleted)
                            source.Cancel();
                    });
                Equal(RuntimeScanStatus.Cancelled, cancelled.Status);
                Equal(CoverageState.Partial, cancelled.Coverage.State);
                Equal(2, cancelled.Coverage.ExpectedPlanets);
                Equal(1, cancelled.Coverage.CompletedPlanets);
                Equal(0, cancelled.Reports.Count);
                Equal(104, cancelled.AffectedPlanetId);
                Equal(1, cancelledGateway.RestoreCalls);
            }

            var failedGateway = new FakeBirthGateway { FailingPlanetId = 104 };
            BirthSystemRawResult failed = new BirthSystemRawCoordinator(failedGateway)
                .TryGenerate(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Failed, failed.Status);
            Equal(CoverageState.Partial, failed.Coverage.State);
            Equal(1, failed.Coverage.CompletedPlanets);
            Equal(104, failed.AffectedPlanetId);
            Equal(0, failed.Reports.Count);
            Equal(1, failedGateway.RestoreCalls);
        }

        private static void BirthRequestPreservesPreviewReport()
        {
            var gate = new RuntimeOperationGate();
            var previewGateway = new FakeGateway();
            var previewCoordinator = new PreviewScanCoordinator(previewGateway, gate);
            RuntimeScanResult preview = previewCoordinator.TryScan(Request(), CancellationToken.None);
            var birthGateway = new FakeBirthGateway();
            RuntimeScanResult? nested = null;
            birthGateway.OnGenerate = () => nested ??=
                previewCoordinator.TryScan(Request(), CancellationToken.None);

            BirthSystemRawResult birth = new BirthSystemRawCoordinator(birthGateway, gate)
                .TryGenerate(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Success, birth.Status);
            Equal(RuntimeScanStatus.Busy, nested?.Status);
            Equal(ComponentOutcome.Unknown, preview.Reports.Single(report =>
                report.ConclusionId == "FS-RESOURCES.common-total").Outcome);
            Equal(ComponentOutcome.DoesNotSupport, birth.Reports.Single(report =>
                report.ConclusionId == "FS-RESOURCES.common-total").Outcome);
            Equal(1, previewGateway.GenerateCalls);
        }

        private static void CompleteClusterAggregatesRareAccess()
        {
            var gateway = new FakeCompleteClusterGateway();
            CompleteClusterRawResult result = new CompleteClusterRawCoordinator(gateway)
                .TryGenerate(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Success, result.Status);
            True(result.Coverage.IsComplete);
            Equal(3, result.Coverage.ExpectedPlanets);
            Equal(3, result.Coverage.CompletedPlanets);
            Equal(7, result.RareResources.Count);
            NormalizedRareResourceEvidence kimberlite = result.RareResources.Single(
                resource => resource.ResourceId == "kimberlite");
            Equal(true, kimberlite.IsPresent);
            Equal(2m, kimberlite.DistanceFromBirthLy);
            Equal(200L, kimberlite.Amount);
            Equal(1, kimberlite.VeinGroups);
            NormalizedRareResourceEvidence absent = result.RareResources.Single(
                resource => resource.ResourceId == "fractal-silicon");
            Equal(false, absent.IsPresent);

            AssertReport(result, "RR-ACCESS.distance:kimberlite", ComponentOutcome.Supports);
            AssertReport(result, "RR-ACCESS.distance:unipolar-magnet", ComponentOutcome.DoesNotSupport);
            AssertReport(result, "RR-ACCESS.distance:fractal-silicon", ComponentOutcome.DoesNotSupport);
            ConclusionReport amount = result.Reports.Single(report =>
                report.ConclusionId == "RR-ACCESS.amount:kimberlite");
            Equal(ComponentOutcome.Unknown, amount.Outcome);
            Equal("200", amount.DecisiveFact?.Value);
            ConclusionReport cluster = result.Reports.Single(report =>
                report.ConclusionId == "MF-RESOURCE-SCOPE.strength");
            Equal(ComponentOutcome.Unknown, cluster.Outcome);
            Equal("30000", cluster.DecisiveFact?.Value);
            True(result.Reports.Any(report => report.ConclusionId ==
                "MF-SYSTEM-ROLE.role:rare-access"));
            True(result.Reports.Any(report => report.ConclusionId ==
                "TRAIT-SUMMARY.registry:close-rare-access:kimberlite"));
            Equal(1, gateway.RestoreCalls);
            Equal("original", gateway.StateMarker);
        }

        private static void CompleteClusterPartialExitsExposeNoEvidence()
        {
            var cancellationGateway = new FakeCompleteClusterGateway();
            using (var source = new CancellationTokenSource())
            {
                CompleteClusterRawResult cancelled =
                    new CompleteClusterRawCoordinator(cancellationGateway).TryGenerate(
                        Request(),
                        source.Token,
                        progress =>
                        {
                            if (progress.State == CompleteClusterProgressState.PlanetCompleted)
                                source.Cancel();
                        });
                Equal(RuntimeScanStatus.Cancelled, cancelled.Status);
                Equal(CoverageState.Partial, cancelled.Coverage.State);
                Equal(1, cancelled.Coverage.CompletedPlanets);
                Equal(0, cancelled.RareResources.Count);
                Equal(0, cancelled.Reports.Count);
                Equal(101, cancelled.AffectedPlanetId);
                Equal(1, cancellationGateway.RestoreCalls);
            }

            var failureGateway = new FakeCompleteClusterGateway { FailingPlanetId = 102 };
            CompleteClusterRawResult failed = new CompleteClusterRawCoordinator(failureGateway)
                .TryGenerate(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Failed, failed.Status);
            Equal(CoverageState.Partial, failed.Coverage.State);
            Equal(1, failed.Coverage.CompletedPlanets);
            Equal(0, failed.RareResources.Count);
            Equal(0, failed.Reports.Count);
            Equal(102, failed.AffectedPlanetId);
            Equal(1, failureGateway.RestoreCalls);
        }

        private static void CompleteClusterIncompatibilityIsExplicit()
        {
            var gateway = new FakeCompleteClusterGateway
            {
                GenerationFailure = new RawCompatibilityException(
                    "unknown-raw-resource-type",
                    "unsupported resource",
                    "EVeinType=99")
            };
            CompleteClusterRawResult result = new CompleteClusterRawCoordinator(gateway)
                .TryGenerate(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Incompatible, result.Status);
            Equal("unknown-raw-resource-type", result.Code);
            Equal("EVeinType=99", result.RawDiagnostic);
            Equal(CoverageState.Unavailable, result.Coverage.State);
            Equal(0, result.Reports.Count);
            Equal(1, gateway.RestoreCalls);
        }

        private static void CompleteClusterBoundRejectsBeforeGeneration()
        {
            var gateway = new FakeCompleteClusterGateway
            {
                TargetCount = CompleteClusterRawCoordinator.MaximumSolidPlanets + 1
            };
            CompleteClusterRawResult result = new CompleteClusterRawCoordinator(gateway)
                .TryGenerate(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Incompatible, result.Status);
            Equal("complete-cluster-planet-bound-exceeded", result.Code);
            Equal(257, result.Coverage.ExpectedPlanets);
            Equal(0, result.Coverage.CompletedPlanets);
            Equal(CoverageState.Unavailable, result.Coverage.State);
            Equal(0, result.RareResources.Count);
            Equal(0, result.Reports.Count);
            Equal(0, gateway.GenerateCalls);
            Equal(1, gateway.RestoreCalls);
            Equal("original", gateway.StateMarker);
        }

        private static void CompleteClusterSharesSerialization()
        {
            var gate = new RuntimeOperationGate();
            var previewGateway = new FakeGateway();
            var preview = new PreviewScanCoordinator(previewGateway, gate);
            var gateway = new FakeCompleteClusterGateway();
            RuntimeScanResult? nested = null;
            gateway.OnPlanet = () => nested ??= preview.TryScan(Request(), CancellationToken.None);
            CompleteClusterRawResult result = new CompleteClusterRawCoordinator(gateway, gate)
                .TryGenerate(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Success, result.Status);
            Equal(RuntimeScanStatus.Busy, nested?.Status);
            Equal(0, previewGateway.GenerateCalls);
        }

        private static void RuntimeBoundaryExposesNoGameObjects()
        {
            Assembly assembly = typeof(PreviewScanCoordinator).Assembly;
            string[] forbidden = { "Assembly-CSharp", "UnityEngine", "BepInEx" };
            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
                False(forbidden.Any(value => reference.Name?.StartsWith(value, StringComparison.Ordinal) == true));

            foreach (Type type in new[]
            {
                typeof(RuntimeScanResult),
                typeof(RuntimeFingerprint),
                typeof(RuntimePreviewSnapshot),
                typeof(RawPlanetResult),
                typeof(NormalizedRawPlanetEvidence),
                typeof(NormalizedRawVeinNode),
                typeof(NormalizedRawVeinGroup),
                typeof(BirthSystemRawResult),
                typeof(BirthSystemRawProgress),
                typeof(CompleteClusterRawResult),
                typeof(CompleteClusterRawProgress)
            })
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

        private static RawPlanetRequest RawRequest()
        {
            return new RawPlanetRequest(Request(), 103, 1);
        }

        private static NormalizedRawPlanetEvidence RawSnapshot(
            int planetId = 103,
            int algorithmId = 1)
        {
            var nodes = new[]
            {
                new NormalizedRawVeinNode(
                    2, 2, 7, "oil", 1007, RawResourceSemantics.OilFlow,
                    1_000, 2, 0.4m, 0.5m, 0.6m, 1.25m),
                new NormalizedRawVeinNode(
                    1, 1, 1, "iron", 1001, RawResourceSemantics.FiniteDeposit,
                    20_000, 1, 0.1m, 0.2m, 0.3m, null)
            };
            var groups = new[]
            {
                new NormalizedRawVeinGroup(
                    2, 7, "oil", RawResourceSemantics.OilFlow,
                    1, 1_000, 0.4m, 0.5m, 0.6m),
                new NormalizedRawVeinGroup(
                    1, 1, "iron", RawResourceSemantics.FiniteDeposit,
                    1, 20_000, 0.1m, 0.2m, 0.3m)
            };
            return new NormalizedRawPlanetEvidence(
                16_315_224,
                planetId,
                1,
                algorithmId,
                RawPlanetCoverage.Complete(),
                nodes,
                groups);
        }

        private static RuntimeFingerprint Fingerprint(
            string? gameVersion = null,
            int? algorithm = null,
            string? assembly = null,
            IEnumerable<string>? themes = null,
            bool members = true,
            string? missing = null,
            IEnumerable<string>? mods = null,
            string? methodIl = null,
            IEnumerable<string>? patchers = null)
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
                mods,
                methodIl ?? ConclusionDefinition.ReferenceGenerationMethodIlSha256,
                patchers);
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

        private static void AssertReport(
            CompleteClusterRawResult result,
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

        private sealed class FakeRawGateway : IRuntimeRawPlanetGateway
        {
            public RuntimeFingerprint Fingerprint { get; set; } = Program.Fingerprint();
            public NormalizedRawPlanetEvidence Snapshot { get; set; } = Program.RawSnapshot();
            public Exception? GenerationFailure { get; set; }
            public Action? OnAtomic { get; set; }
            public int? MainThreadIdOverride { get; set; }
            public int GenerateCalls { get; private set; }
            public int RestoreCalls { get; private set; }
            public string StateMarker { get; set; } = "original";
            public int MainThreadId => MainThreadIdOverride ?? Thread.CurrentThread.ManagedThreadId;

            public RuntimeFingerprint CaptureFingerprint(PreviewScanRequest request) => Fingerprint;

            public RuntimeStateLease CaptureState()
            {
                string original = StateMarker;
                StateMarker = "leased";
                return new FakeRawLease(this, original);
            }

            public NormalizedRawPlanetEvidence GenerateRawPlanet(
                RawPlanetRequest request,
                CancellationToken cancellationToken,
                Action<string> recordTrace)
            {
                GenerateCalls++;
                StateMarker = "raw-generated";
                recordTrace("raw:atomic:start");
                OnAtomic?.Invoke();
                if (GenerationFailure != null)
                    throw GenerationFailure;
                recordTrace("raw:atomic:complete");
                cancellationToken.ThrowIfCancellationRequested();
                return Snapshot;
            }

            private sealed class FakeRawLease : RuntimeStateLease
            {
                private readonly FakeRawGateway owner;
                private readonly string original;
                private bool restored;

                public FakeRawLease(FakeRawGateway owner, string original)
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

        private sealed class FakeBirthGateway : IRuntimeBirthSystemRawGateway
        {
            public int? FailingPlanetId { get; set; }
            public Action? OnGenerate { get; set; }
            public int GenerateCalls { get; private set; }
            public int RestoreCalls { get; private set; }
            public long GeneratedAmount { get; private set; }
            public string StateMarker { get; set; } = "original";
            public int MainThreadId => Thread.CurrentThread.ManagedThreadId;

            public RuntimeFingerprint CaptureFingerprint(PreviewScanRequest request) => Fingerprint();

            public RuntimeStateLease CaptureState()
            {
                string original = StateMarker;
                StateMarker = "leased";
                return new Lease(this, original);
            }

            public BirthSystemRawPlan DiscoverBirthSystem(
                PreviewScanRequest request,
                CancellationToken cancellationToken,
                Action<string> recordTrace)
            {
                recordTrace("birth-plan:declared=2");
                return new BirthSystemRawPlan(
                    Snapshot(),
                    new[]
                    {
                        new BirthSystemPlanetTarget(103, 1),
                        new BirthSystemPlanetTarget(104, 2)
                    });
            }

            public NormalizedRawPlanetEvidence GenerateRawPlanet(
                RawPlanetRequest request,
                CancellationToken cancellationToken,
                Action<string> recordTrace)
            {
                GenerateCalls++;
                OnGenerate?.Invoke();
                if (FailingPlanetId == request.PlanetId)
                    throw new InvalidOperationException("injected planet failure");
                NormalizedRawPlanetEvidence evidence = RawSnapshot(
                    request.PlanetId,
                    request.ExpectedAlgorithmId);
                GeneratedAmount += evidence.Nodes.Sum(node => node.Amount);
                return evidence;
            }

            private sealed class Lease : RuntimeStateLease
            {
                private readonly FakeBirthGateway owner;
                private readonly string original;
                private bool restored;

                public Lease(FakeBirthGateway owner, string original)
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

        private sealed class FakeCompleteClusterGateway : IRuntimeCompleteClusterRawGateway
        {
            public int? FailingPlanetId { get; set; }
            public Exception? GenerationFailure { get; set; }
            public Action? OnPlanet { get; set; }
            public int TargetCount { get; set; } = 3;
            public int GenerateCalls { get; private set; }
            public int RestoreCalls { get; private set; }
            public string StateMarker { get; set; } = "original";
            public int MainThreadId => Thread.CurrentThread.ManagedThreadId;

            public RuntimeFingerprint CaptureFingerprint(PreviewScanRequest request) => Fingerprint();

            public RuntimeStateLease CaptureState()
            {
                string original = StateMarker;
                StateMarker = "leased";
                return new Lease(this, original);
            }

            public BirthSystemRawPlan DiscoverBirthSystem(
                PreviewScanRequest request,
                CancellationToken cancellationToken,
                Action<string> recordTrace) =>
                throw new NotSupportedException();

            public NormalizedRawPlanetEvidence GenerateRawPlanet(
                RawPlanetRequest request,
                CancellationToken cancellationToken,
                Action<string> recordTrace) =>
                throw new NotSupportedException();

            public CompleteClusterRawPlan DiscoverCompleteCluster(
                PreviewScanRequest request,
                CancellationToken cancellationToken,
                Action<string> recordTrace)
            {
                recordTrace("cluster-plan:declared=" + TargetCount);
                var targets = new List<CompleteClusterPlanetTarget>(TargetCount);
                for (int index = 0; index < TargetCount; index++)
                {
                    int planetId = 101 + index;
                    targets.Add(new CompleteClusterPlanetTarget(
                        planetId,
                        index + 1,
                        new ConclusionSubject(
                            index == 0 ? SubjectKind.BirthSystem : SubjectKind.StarSystem,
                            (index + 1).ToString()),
                        index == 0 ? 0m : index == 1 ? 2m : 20m));
                }
                return new CompleteClusterRawPlan(
                    Snapshot(),
                    targets);
            }

            public void GenerateCompleteCluster(
                PreviewScanRequest request,
                CompleteClusterRawPlan plan,
                CancellationToken cancellationToken,
                Action<CompleteClusterPlanetTarget> planetStarted,
                Action<CompleteClusterPlanetTarget, NormalizedRawPlanetEvidence> planetCompleted,
                Action<string> recordTrace)
            {
                GenerateCalls++;
                if (GenerationFailure != null)
                    throw GenerationFailure;
                foreach (CompleteClusterPlanetTarget target in plan.Targets)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    planetStarted(target);
                    OnPlanet?.Invoke();
                    if (FailingPlanetId == target.PlanetId)
                        throw new InvalidOperationException("injected cluster planet failure");
                    NormalizedRawPlanetEvidence evidence = target.PlanetId switch
                    {
                        102 => ClusterSnapshot(target, "kimberlite", 9, 1128, 200),
                        103 => ClusterSnapshot(target, "unipolar-magnet", 14, 1016, 300),
                        _ => ClusterSnapshot(target, null, 0, 0, 0)
                    };
                    planetCompleted(target, evidence);
                }
                recordTrace("cluster-raw:candidate:released");
            }

            private static NormalizedRawPlanetEvidence ClusterSnapshot(
                CompleteClusterPlanetTarget target,
                string? rareId,
                int rareType,
                int rareProduct,
                long rareAmount)
            {
                var nodes = new List<NormalizedRawVeinNode>
                {
                    new NormalizedRawVeinNode(
                        1, 1, 1, "iron", 1001, RawResourceSemantics.FiniteDeposit,
                        10_000, 1, 0.1m, 0.2m, 0.3m, null)
                };
                var groups = new List<NormalizedRawVeinGroup>
                {
                    new NormalizedRawVeinGroup(
                        1, 1, "iron", RawResourceSemantics.FiniteDeposit,
                        1, 10_000, 0.1m, 0.2m, 0.3m)
                };
                if (rareId != null)
                {
                    nodes.Add(new NormalizedRawVeinNode(
                        2, 2, rareType, rareId, rareProduct,
                        RawResourceSemantics.FiniteDeposit,
                        rareAmount, 2, 0.4m, 0.5m, 0.6m, null));
                    groups.Add(new NormalizedRawVeinGroup(
                        2, rareType, rareId, RawResourceSemantics.FiniteDeposit,
                        1, rareAmount, 0.4m, 0.5m, 0.6m));
                }
                return new NormalizedRawPlanetEvidence(
                    16_315_224,
                    target.PlanetId,
                    1,
                    target.AlgorithmId,
                    RawPlanetCoverage.Complete(),
                    nodes,
                    groups);
            }

            private sealed class Lease : RuntimeStateLease
            {
                private readonly FakeCompleteClusterGateway owner;
                private readonly string original;
                private bool restored;

                public Lease(FakeCompleteClusterGateway owner, string original)
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
