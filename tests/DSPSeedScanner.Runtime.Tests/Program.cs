using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
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
                ("birth planet attribution is deterministic and owned", BirthPlanetAttributionIsDeterministicAndOwned),
                ("birth planet attribution distinguishes gas cardinality and unknown", BirthPlanetAttributionDistinguishesCardinalityAndUnknown),
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
                ("incremental cluster doubles safe recovery frames without changing results", IncrementalClusterMatchesSynchronousExecution),
                ("incremental cluster cancellation and failure restore state", IncrementalClusterExitPathsRestoreState),
                ("incremental cluster keeps serialization between yields", IncrementalClusterKeepsSerializationBetweenYields),
                ("complete cache keys cover the full supported identity", CompleteCacheKeysCoverSupportedIdentity),
                ("complete cache round trips and replaces atomically", CompleteCacheRoundTripsAndReplacesAtomically),
                ("complete cache bounds retention and clears manually", CompleteCacheBoundsRetentionAndClears),
                ("complete cache rejects unsafe and obsolete entries", CompleteCacheRejectsUnsafeEntries),
                ("completed keyboard paste and random loads create one session each", CompletedInputLoadsCreateOneSessionEach),
                ("duplicate callbacks coalesce while same identity reloads", DuplicateCallbacksCoalesceAndReloadsReplace),
                ("replacement rejects stale publication and late loads", ReplacementRejectsStalePublication),
                ("preview exit retires once and blocks resurrection", PreviewExitRetiresAndBlocksResurrection),
                ("automatic resolution uses cache once per completed load", AutomaticResolutionUsesCacheOncePerLoad),
                ("automatic resolution cancels replacement and exit", AutomaticResolutionCancelsReplacementAndExit),
                ("automatic resolution terminal failures never retry", AutomaticResolutionFailuresNeverRetry),
                ("panel maps every operational state within text bounds", PanelMapsEveryOperationalState),
                ("panel corners map clockwise and avoid border centers", PanelCornersMapClockwise),
                ("panel rejects obsolete sessions and hides exactly", PanelRejectsObsoleteSessions),
                ("conclusion cards map every outcome and subject kind", ConclusionCardsMapEveryOutcomeAndSubject),
                ("conclusion panel separates contexts stages and conflicts", ConclusionPanelSeparatesContextsAndConflicts),
                ("conclusion panel snapshot stays bounded and neutral", ConclusionPanelSnapshotIsBoundedAndNeutral),
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

        private static void BirthPlanetAttributionIsDeterministicAndOwned()
        {
            NormalizedBirthPlanetEvidence[] input =
            {
                GasAttribution(202, "Alpha II", "hydrogen"),
                SolidAttribution(201, "Alpha I", 1.35m, 1.1m, true),
                GasAttribution(203, "Alpha III", "fire-ice", "hydrogen")
            };
            var gateway = new FakeGateway
            {
                Snapshot = Snapshot(birthPlanetAttributions: input)
            };

            RuntimeScanResult result = new PreviewScanCoordinator(gateway)
                .TryScan(Request(), CancellationToken.None);

            Equal(RuntimeScanStatus.Success, result.Status);
            True(result.BirthPlanetAttributions != null);
            Equal("201,202,203", String.Join(",", result.BirthPlanetAttributions!
                .Select(value => value.PlanetId)));
            Equal("Alpha I", result.BirthPlanetAttributions![0].DisplayName);
            Equal(1.35m, result.BirthPlanetAttributions[0].SolarRatio);
            Equal(1.1m, result.BirthPlanetAttributions[0].WindRatio);
            Equal(true, result.BirthPlanetAttributions[0].IsTidalLocked);
            Equal("fire-ice,hydrogen", String.Join(",",
                result.BirthPlanetAttributions[2].GasProductIds));
            Equal("hydrogen", String.Join(",",
                result.BirthPlanetAttributions[1].GasProductIds));

            input[0] = GasAttribution(999, "Mutated", "deuterium");
            Equal(202, result.BirthPlanetAttributions[1].PlanetId);
            False(result.BirthPlanetAttributions[1].GasProductIds.Contains("deuterium"));

            bool duplicateRejected = false;
            try
            {
                Snapshot(birthPlanetAttributions: new[]
                {
                    GasAttribution(202, "Alpha II", "hydrogen"),
                    GasAttribution(202, "Alpha II", "fire-ice")
                });
            }
            catch (ArgumentException)
            {
                duplicateRejected = true;
            }
            True(duplicateRejected);

            bool incompleteGasRejected = false;
            try
            {
                _ = new NormalizedBirthPlanetEvidence(
                    202,
                    "Alpha II",
                    true,
                    null,
                    null,
                    null,
                    null);
            }
            catch (ArgumentNullException)
            {
                incompleteGasRejected = true;
            }
            True(incompleteGasRejected);
        }

        private static void BirthPlanetAttributionDistinguishesCardinalityAndUnknown()
        {
            RuntimePreviewSnapshot[] snapshots =
            {
                Snapshot(birthPlanetAttributions: new[]
                {
                    SolidAttribution(201, "Alpha I", 1.35m, 1.1m, true)
                }),
                Snapshot(birthPlanetAttributions: new[]
                {
                    SolidAttribution(201, "Alpha I", 1.35m, 1.1m, true),
                    GasAttribution(202, "Alpha II", "hydrogen")
                }),
                Snapshot(birthPlanetAttributions: new[]
                {
                    SolidAttribution(201, "Alpha I", 1.35m, 1.1m, true),
                    GasAttribution(202, "Alpha II", "hydrogen"),
                    GasAttribution(203, "Alpha III", "fire-ice")
                })
            };
            int[] expectedGasCounts = { 0, 1, 2 };
            for (int index = 0; index < snapshots.Length; index++)
            {
                var gateway = new FakeGateway { Snapshot = snapshots[index] };
                RuntimeScanResult result = new PreviewScanCoordinator(gateway)
                    .TryScan(Request(), CancellationToken.None);
                True(result.BirthPlanetAttributions != null);
                Equal(expectedGasCounts[index], result.BirthPlanetAttributions!
                    .Count(value => value.IsGasGiant));
            }

            var unknownGateway = new FakeGateway { Snapshot = Snapshot() };
            RuntimeScanResult unknown = new PreviewScanCoordinator(unknownGateway)
                .TryScan(Request(), CancellationToken.None);
            True(unknown.BirthPlanetAttributions == null);

            var incompleteGateway = new FakeGateway
            {
                Snapshot = Snapshot(32, new[]
                {
                    SolidAttribution(201, "Alpha I", 1.35m, 1.1m, true)
                })
            };
            RuntimeScanResult incomplete = new PreviewScanCoordinator(incompleteGateway)
                .TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Failed, incomplete.Status);
            True(incomplete.BirthPlanetAttributions == null);

            WithTemporaryDirectory(path =>
            {
                var lifecycle = new PreviewSessionLifecycle();
                using var resolver = new PreviewResolutionCoordinator(
                    lifecycle,
                    new PreviewScanCoordinator(unknownGateway),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt attempt = resolver.CurrentPublishedAttempt!;
                False(attempt.HasCompleteBirthPlanetAttribution);
                Equal(0, attempt.BirthPlanetAttributions.Count);
            });
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

        private static void IncrementalClusterMatchesSynchronousExecution()
        {
            CompleteClusterRawResult synchronous =
                new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway())
                    .TryGenerate(Request(), CancellationToken.None);
            var gateway = new FakeCompleteClusterGateway();
            var observed = new List<CompleteClusterRawProgress>();
            using CompleteClusterRawOperation operation =
                new CompleteClusterRawCoordinator(gateway).TryStart(
                    Request(),
                    CancellationToken.None,
                    observed.Add);

            Equal(CompleteClusterRawOperationState.Ready, operation.State);
            Equal(3, operation.ExpectedPlanets);
            Equal(0, operation.CompletedPlanets);
            int advances = 0;
            while (operation.State == CompleteClusterRawOperationState.Ready)
            {
                int before = operation.CompletedPlanets;
                operation.Advance();
                advances++;
                True(operation.IsYieldStateRestored);
                Equal(
                    advances % 2 == 1 ? before + 1 : before,
                    operation.CompletedPlanets);
                Equal(
                    operation.State == CompleteClusterRawOperationState.Ready
                        ? "leased"
                        : "original",
                    gateway.StateMarker);
            }

            CompleteClusterRawResult incremental = operation.Result!;
            Equal(6, advances);
            Equal(RuntimeScanStatus.Success, incremental.Status);
            Equal(synchronous.Coverage, incremental.Coverage);
            True(synchronous.RareResources.SequenceEqual(incremental.RareResources));
            True(synchronous.Reports.SequenceEqual(incremental.Reports));
            Equal(7, observed.Count);
            Equal(CompleteClusterProgressState.Planned, observed[0].State);
            for (int planetIndex = 0; planetIndex < 3; planetIndex++)
            {
                CompleteClusterRawProgress started = observed[1 + (planetIndex * 2)];
                CompleteClusterRawProgress completed = observed[2 + (planetIndex * 2)];
                Equal(CompleteClusterProgressState.PlanetStarted, started.State);
                Equal(planetIndex, started.CompletedPlanets);
                Equal(CompleteClusterProgressState.PlanetCompleted, completed.State);
                Equal(planetIndex + 1, completed.CompletedPlanets);
                Equal(started.PlanetId, completed.PlanetId);
            }
            Equal(3, incremental.Trace.Count(value =>
                value.StartsWith("cluster-step:yield:", StringComparison.Ordinal)));
            Equal(3, incremental.Trace.Count(value =>
                value.StartsWith("cluster-step:recovery:", StringComparison.Ordinal)));
            Equal(3, gateway.YieldRestoreChecks);
            Equal(1, gateway.SessionDisposeCalls);
            Equal(1, gateway.RestoreCalls);
        }

        private static void IncrementalClusterExitPathsRestoreState()
        {
            var cancellationGateway = new FakeCompleteClusterGateway();
            using (var source = new CancellationTokenSource())
            using (CompleteClusterRawOperation operation =
                new CompleteClusterRawCoordinator(cancellationGateway).TryStart(
                    Request(), source.Token))
            {
                operation.Advance();
                Equal(CompleteClusterRawOperationState.Ready, operation.State);
                Equal("leased", cancellationGateway.StateMarker);
                source.Cancel();
                operation.Advance();
                CompleteClusterRawResult cancelled = operation.Result!;
                Equal(RuntimeScanStatus.Cancelled, cancelled.Status);
                Equal(CoverageState.Partial, cancelled.Coverage.State);
                Equal(1, cancelled.Coverage.CompletedPlanets);
                Equal(0, cancelled.Reports.Count);
                True(cancelled.StateRestored);
                Equal(1, cancellationGateway.YieldRestoreChecks);
                Equal("original", cancellationGateway.StateMarker);
                Equal(1, cancellationGateway.SessionDisposeCalls);
                Equal(1, cancellationGateway.RestoreCalls);
            }

            var failureGateway = new FakeCompleteClusterGateway { FailingPlanetId = 102 };
            using (CompleteClusterRawOperation operation =
                new CompleteClusterRawCoordinator(failureGateway).TryStart(
                    Request(), CancellationToken.None))
            {
                operation.Advance();
                Equal(CompleteClusterRawOperationState.Ready, operation.State);
                operation.Advance();
                Equal(CompleteClusterRawOperationState.Ready, operation.State);
                operation.Advance();
                CompleteClusterRawResult failed = operation.Result!;
                Equal(RuntimeScanStatus.Failed, failed.Status);
                Equal(CoverageState.Partial, failed.Coverage.State);
                Equal(1, failed.Coverage.CompletedPlanets);
                Equal(0, failed.RareResources.Count);
                True(failed.StateRestored);
                Equal("original", failureGateway.StateMarker);
                Equal(2, failureGateway.YieldRestoreChecks);
                Equal(1, failureGateway.SessionDisposeCalls);
                Equal(1, failureGateway.RestoreCalls);
            }

            var disposalGateway = new FakeCompleteClusterGateway();
            CompleteClusterRawOperation disposed =
                new CompleteClusterRawCoordinator(disposalGateway).TryStart(
                    Request(), CancellationToken.None);
            disposed.Advance();
            disposed.Dispose();
            Equal(RuntimeScanStatus.Cancelled, disposed.Result?.Status);
            Equal("original", disposalGateway.StateMarker);
            Equal(1, disposalGateway.SessionDisposeCalls);
            Equal(1, disposalGateway.RestoreCalls);
        }

        private static void IncrementalClusterKeepsSerializationBetweenYields()
        {
            var gate = new RuntimeOperationGate();
            var previewGateway = new FakeGateway();
            var preview = new PreviewScanCoordinator(previewGateway, gate);
            var clusterGateway = new FakeCompleteClusterGateway();
            using CompleteClusterRawOperation operation =
                new CompleteClusterRawCoordinator(clusterGateway, gate).TryStart(
                    Request(), CancellationToken.None);

            Equal(RuntimeScanStatus.Busy,
                preview.TryScan(Request(), CancellationToken.None).Status);
            operation.Advance();
            Equal(RuntimeScanStatus.Busy,
                preview.TryScan(Request(), CancellationToken.None).Status);
            operation.Advance();
            Equal(RuntimeScanStatus.Busy,
                preview.TryScan(Request(), CancellationToken.None).Status);
            operation.Advance();
            Equal(RuntimeScanStatus.Busy,
                preview.TryScan(Request(), CancellationToken.None).Status);
            operation.Advance();
            Equal(RuntimeScanStatus.Busy,
                preview.TryScan(Request(), CancellationToken.None).Status);
            operation.Advance();
            Equal(RuntimeScanStatus.Busy,
                preview.TryScan(Request(), CancellationToken.None).Status);
            operation.Advance();
            Equal(RuntimeScanStatus.Success, operation.Result?.Status);
            Equal(RuntimeScanStatus.Success,
                preview.TryScan(Request(), CancellationToken.None).Status);
            Equal(1, previewGateway.GenerateCalls);
        }

        private static void CompleteCacheKeysCoverSupportedIdentity()
        {
            True(CompleteClusterCacheKey.TryCreate(
                PreviewIdentity(16_315_224, 1.0m),
                Fingerprint(),
                out CompleteClusterCacheKey? first));
            True(CompleteClusterCacheKey.TryCreate(
                PreviewIdentity(16_315_224, 1.00m),
                Fingerprint(),
                out CompleteClusterCacheKey? equivalent));
            Equal(first, equivalent);
            Equal(first?.Hash, equivalent?.Hash);
            Equal(first?.CanonicalValue, equivalent?.CanonicalValue);

            True(CompleteClusterCacheKey.TryCreate(
                PreviewIdentity(16_315_224, 0.5m),
                Fingerprint(),
                out CompleteClusterCacheKey? differentResources));
            True(CompleteClusterCacheKey.TryCreate(
                PreviewIdentity(73_339_583),
                Fingerprint(),
                out CompleteClusterCacheKey? differentSeed));
            False(first!.Equals(differentResources));
            False(first.Equals(differentSeed));
            False(String.Equals(first?.Hash, differentResources?.Hash, StringComparison.Ordinal));
            False(String.Equals(first?.Hash, differentSeed?.Hash, StringComparison.Ordinal));

            False(CompleteClusterCacheKey.TryCreate(
                PreviewIdentity(16_315_224),
                Fingerprint(methodIl: "obsolete"),
                out _));
            False(CompleteClusterCacheKey.TryCreate(
                PreviewIdentity(16_315_224),
                Fingerprint(mods: new[] { "generation-mod" }),
                out _));
        }

        private static void CompleteCacheRoundTripsAndReplacesAtomically()
        {
            WithTemporaryDirectory(path =>
            {
                var cache = new CompleteClusterConclusionCache(path, maximumEntries: 3);
                PreviewGenerationIdentity identity = PreviewIdentity(16_315_224);
                RuntimeFingerprint fingerprint = Fingerprint();
                CompleteClusterRawResult source = CompleteResult();
                False(cache.TryRead(identity, fingerprint, out _));
                True(cache.TryStore(identity, source));

                True(CompleteClusterCacheKey.TryCreate(identity, fingerprint, out CompleteClusterCacheKey? key));
                string entry = Path.Combine(path, key!.FileName);
                True(File.Exists(entry));
                File.WriteAllText(entry, "incomplete replacement fixture");
                True(cache.TryStore(identity, source));
                Equal(1, Directory.GetFiles(path, "*.dspseedscan").Length);
                Equal(0, Directory.GetFiles(path, ".*.tmp").Length);

                var reopened = new CompleteClusterConclusionCache(path, maximumEntries: 3);
                True(reopened.TryRead(
                    identity,
                    fingerprint,
                    out CachedCompleteClusterConclusions? restored));
                ConclusionReport[] expected = source.Reports
                    .Where(report => report.Stage == EvidenceStage.CompleteClusterRaw)
                    .ToArray();
                Equal(identity, restored?.Identity);
                Equal(key.Hash, restored?.CacheKeyHash);
                Equal(source.Coverage, restored?.Coverage);
                True(expected.SequenceEqual(restored!.Reports));
                True(restored.Reports.All(report =>
                    report.Stage == EvidenceStage.CompleteClusterRaw));
                True(source.Reports.Any(report =>
                    report.Stage == EvidenceStage.GalaxyPreview));
                True(restored.Reports.Count < source.Reports.Count);

                ConclusionReport previewTemplate = source.Reports.First(report =>
                    report.Stage == EvidenceStage.GalaxyPreview);
                var expandedReports = source.Reports.Concat(
                    Enumerable.Repeat(previewTemplate, 1_025)).ToArray();
                var expanded = new CompleteClusterRawResult(
                    source.Status,
                    source.GalaxySeed,
                    source.Code,
                    source.Message,
                    source.Fingerprint,
                    source.Coverage,
                    source.Progress,
                    source.RareResources,
                    expandedReports,
                    source.Trace,
                    source.StateRestored,
                    source.ElapsedMilliseconds,
                    source.ManagedMemoryDeltaBytes);
                True(expanded.Reports.Count > 1_024);
                True(cache.TryStore(identity, expanded));
                True(cache.TryRead(identity, fingerprint, out restored));
                Equal(expected.Length, restored?.Reports.Count);
                False(typeof(CachedCompleteClusterConclusions).GetProperties()
                    .Any(property => property.Name == "RareResources" ||
                        property.Name == "Progress" || property.Name == "Trace" ||
                        property.Name == "ElapsedMilliseconds" ||
                        property.Name == "ManagedMemoryDeltaBytes" ||
                        property.Name == "BirthPlanetAttributions"));
                True(new FileInfo(entry).Length <= 256 * 1024);
            });
        }

        private static void CompleteCacheBoundsRetentionAndClears()
        {
            WithTemporaryDirectory(path =>
            {
                var cache = new CompleteClusterConclusionCache(path, maximumEntries: 2);
                PreviewGenerationIdentity firstIdentity = PreviewIdentity(16_315_224, 1m);
                PreviewGenerationIdentity secondIdentity = PreviewIdentity(16_315_224, 0.5m);
                PreviewGenerationIdentity thirdIdentity = PreviewIdentity(16_315_224, 2m);
                True(cache.TryStore(firstIdentity, CompleteResult(1m)));
                True(cache.TryStore(secondIdentity, CompleteResult(0.5m)));
                True(cache.TryStore(thirdIdentity, CompleteResult(2m)));

                Equal(2, Directory.GetFiles(path, "*.dspseedscan").Length);
                False(cache.TryRead(firstIdentity, Fingerprint(), out _));
                True(cache.TryRead(secondIdentity, Fingerprint(), out _));
                True(cache.TryRead(thirdIdentity, Fingerprint(), out _));
                True(cache.Clear());
                Equal(0, Directory.GetFiles(path).Length);
                False(cache.TryRead(secondIdentity, Fingerprint(), out _));
                True(cache.Clear());
            });
        }

        private static void CompleteCacheRejectsUnsafeEntries()
        {
            WithTemporaryDirectory(path =>
            {
                var cache = new CompleteClusterConclusionCache(path);
                Equal(256, cache.MaximumEntries);
                PreviewGenerationIdentity identity = PreviewIdentity(16_315_224);
                CompleteClusterRawResult source = CompleteResult();
                var partial = new CompleteClusterRawResult(
                    RuntimeScanStatus.Success,
                    source.GalaxySeed,
                    "partial",
                    "partial fixture",
                    source.Fingerprint,
                    new CompleteClusterRawCoverage(CoverageState.Partial, 3, 1),
                    source.Progress,
                    source.RareResources,
                    source.Reports,
                    source.Trace,
                    true,
                    0,
                    0);
                var failed = new CompleteClusterRawResult(
                    RuntimeScanStatus.Failed,
                    source.GalaxySeed,
                    "failed",
                    "failed fixture",
                    source.Fingerprint,
                    source.Coverage,
                    source.Progress,
                    source.RareResources,
                    source.Reports,
                    source.Trace,
                    true,
                    0,
                    0);
                var cancelled = new CompleteClusterRawResult(
                    RuntimeScanStatus.Cancelled,
                    source.GalaxySeed,
                    "cancelled",
                    "cancelled fixture",
                    source.Fingerprint,
                    source.Coverage,
                    source.Progress,
                    source.RareResources,
                    source.Reports,
                    source.Trace,
                    true,
                    0,
                    0);
                var incompatible = new CompleteClusterRawResult(
                    RuntimeScanStatus.Success,
                    source.GalaxySeed,
                    "success",
                    "incompatible fixture",
                    Fingerprint(methodIl: "obsolete"),
                    source.Coverage,
                    source.Progress,
                    source.RareResources,
                    source.Reports,
                    source.Trace,
                    true,
                    0,
                    0);
                ConclusionReport template = source.Reports.First(report =>
                    report.Stage == EvidenceStage.CompleteClusterRaw);
                var oversizedReport = new ConclusionReport(
                    template.Identity,
                    template.Settings,
                    template.Coverage,
                    new string('x', 300 * 1024),
                    template.Context,
                    template.ContractVersion,
                    template.DefinitionVersion,
                    template.Subject,
                    template.Outcome,
                    template.DecisiveFact,
                    template.DiagnosticCause,
                    template.SourceConclusionId);
                var oversized = new CompleteClusterRawResult(
                    RuntimeScanStatus.Success,
                    source.GalaxySeed,
                    "success",
                    "oversized fixture",
                    source.Fingerprint,
                    source.Coverage,
                    source.Progress,
                    source.RareResources,
                    new[] { oversizedReport },
                    source.Trace,
                    true,
                    0,
                    0);

                False(cache.TryStore(identity, partial));
                False(cache.TryStore(identity, failed));
                False(cache.TryStore(identity, cancelled));
                False(cache.TryStore(identity, incompatible));
                False(cache.TryStore(identity, oversized));
                False(cache.TryRead(identity, Fingerprint(), out _));
                Equal(0, Directory.Exists(path) ? Directory.GetFiles(path).Length : 0);

                True(cache.TryStore(identity, source));
                string entry = Directory.GetFiles(path, "*.dspseedscan").Single();
                using (var stream = new FileStream(entry, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
                {
                    using (var reader = new BinaryReader(stream, System.Text.Encoding.UTF8, leaveOpen: true))
                        reader.ReadString();
                    using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
                    writer.Write(999);
                }
                byte[] obsoleteEntry = File.ReadAllBytes(entry);
                using (SHA256 sha = SHA256.Create())
                {
                    byte[] digest = sha.ComputeHash(
                        obsoleteEntry,
                        0,
                        obsoleteEntry.Length - 32);
                    Buffer.BlockCopy(
                        digest,
                        0,
                        obsoleteEntry,
                        obsoleteEntry.Length - digest.Length,
                        digest.Length);
                }
                File.WriteAllBytes(entry, obsoleteEntry);
                False(cache.TryRead(identity, Fingerprint(), out _));
                False(File.Exists(entry));

                True(cache.TryStore(identity, source));
                entry = Directory.GetFiles(path, "*.dspseedscan").Single();
                byte[] corruptEntry = File.ReadAllBytes(entry);
                corruptEntry[10] ^= 0x01;
                File.WriteAllBytes(entry, corruptEntry);
                False(cache.TryRead(identity, Fingerprint(), out _));
                False(File.Exists(entry));
            });
        }

        private static void CompletedInputLoadsCreateOneSessionEach()
        {
            var lifecycle = new PreviewSessionLifecycle();
            PreviewGenerationIdentity keyboardIdentity = PreviewIdentity(11_111_111);
            PreviewGenerationIdentity pasteIdentity = PreviewIdentity(
                22_222_222,
                0.5m,
                CombatMode.Peace,
                0.75m,
                1.25m);
            PreviewGenerationIdentity randomIdentity = PreviewIdentity(33_333_333);

            True(lifecycle.CurrentSession == null);

            PreviewLoadTransition keyboard = lifecycle.ObserveCompletedLoad(1, keyboardIdentity);
            Equal(PreviewLoadDisposition.SessionCreated, keyboard.Disposition);
            Equal(1L, keyboard.CurrentSession?.SessionId);
            True(ReferenceEquals(keyboardIdentity, keyboard.CurrentSession?.Identity));

            PreviewLoadTransition paste = lifecycle.ObserveCompletedLoad(2, pasteIdentity);
            Equal(PreviewLoadDisposition.SessionCreated, paste.Disposition);
            Equal(2L, paste.CurrentSession?.SessionId);
            Equal(22_222_222, paste.CurrentSession?.Identity.GalaxyIdentity.GalaxySeed);
            Equal(0.5m, paste.CurrentSession?.Identity.ResourceMultiplier);
            Equal(CombatMode.Peace, paste.CurrentSession?.Identity.CombatMode);
            Equal(0.75m, paste.CurrentSession?.Identity.InitialColonize);
            Equal(1.25m, paste.CurrentSession?.Identity.MaxDensity);
            Equal(
                PreviewScanRequest.CombatSettingsKeyFor(0.75m, 1.25m),
                paste.CurrentSession?.Identity.CombatSettingsKey);

            PreviewLoadTransition random = lifecycle.ObserveCompletedLoad(3, randomIdentity);
            Equal(PreviewLoadDisposition.SessionCreated, random.Disposition);
            Equal(3L, random.CurrentSession?.SessionId);
            Equal(3L, lifecycle.CurrentSession?.SessionId);
        }

        private static void DuplicateCallbacksCoalesceAndReloadsReplace()
        {
            var lifecycle = new PreviewSessionLifecycle();
            PreviewGenerationIdentity identity = PreviewIdentity(16_315_224);
            PreviewSession first = lifecycle.ObserveCompletedLoad(10, identity).CurrentSession!;

            PreviewLoadTransition duplicate = lifecycle.ObserveCompletedLoad(
                10,
                PreviewIdentity(16_315_224));
            Equal(PreviewLoadDisposition.DuplicateCoalesced, duplicate.Disposition);
            True(ReferenceEquals(first, duplicate.CurrentSession));
            False(first.IsRetired);

            bool reusedSequenceRejected = false;
            try
            {
                lifecycle.ObserveCompletedLoad(10, PreviewIdentity(73_339_583));
            }
            catch (InvalidOperationException)
            {
                reusedSequenceRejected = true;
            }
            True(reusedSequenceRejected);
            True(ReferenceEquals(first, lifecycle.CurrentSession));

            PreviewLoadTransition reload = lifecycle.ObserveCompletedLoad(
                11,
                PreviewIdentity(16_315_224));
            Equal(PreviewLoadDisposition.SessionCreated, reload.Disposition);
            False(ReferenceEquals(first, reload.CurrentSession));
            True(ReferenceEquals(first, reload.RetiredSession));
            True(first.IsRetired);
            True(first.Lifetime.IsCancellationRequested);
            Equal(PreviewSessionRetirementReason.Replaced, first.RetirementReason);
            True(lifecycle.CanPublish(reload.CurrentSession!));
        }

        private static void ReplacementRejectsStalePublication()
        {
            var lifecycle = new PreviewSessionLifecycle();
            PreviewSession first = lifecycle.ObserveCompletedLoad(
                20,
                PreviewIdentity(16_315_224)).CurrentSession!;
            True(lifecycle.CanPublish(first));

            PreviewSession replacement = lifecycle.ObserveCompletedLoad(
                21,
                PreviewIdentity(73_339_583)).CurrentSession!;
            False(lifecycle.CanPublish(first));
            True(lifecycle.CanPublish(replacement));

            PreviewLoadTransition late = lifecycle.ObserveCompletedLoad(
                20,
                PreviewIdentity(16_315_224));
            Equal(PreviewLoadDisposition.StaleLoadIgnored, late.Disposition);
            True(ReferenceEquals(replacement, late.CurrentSession));
            True(ReferenceEquals(replacement, lifecycle.CurrentSession));
        }

        private static void PreviewExitRetiresAndBlocksResurrection()
        {
            var lifecycle = new PreviewSessionLifecycle();
            PreviewGenerationIdentity identity = PreviewIdentity(16_315_224);
            PreviewSession active = lifecycle.ObserveCompletedLoad(30, identity).CurrentSession!;

            PreviewSession? retired = lifecycle.ExitPreview();
            True(ReferenceEquals(active, retired));
            True(active.IsRetired);
            True(active.Lifetime.IsCancellationRequested);
            Equal(PreviewSessionRetirementReason.PreviewExited, active.RetirementReason);
            False(lifecycle.CanPublish(active));
            True(lifecycle.CurrentSession == null);
            True(lifecycle.ExitPreview() == null);

            PreviewLoadTransition duplicate = lifecycle.ObserveCompletedLoad(30, identity);
            Equal(PreviewLoadDisposition.RetiredLoadIgnored, duplicate.Disposition);
            True(duplicate.CurrentSession == null);
            True(lifecycle.CurrentSession == null);
        }

        private static void AutomaticResolutionUsesCacheOncePerLoad()
        {
            WithTemporaryDirectory(path =>
            {
                var gate = new RuntimeOperationGate();
                var previewGateway = new FakeGateway
                {
                    Snapshot = Snapshot(birthPlanetAttributions: new[]
                    {
                        SolidAttribution(201, "Alpha I", 1.35m, 1.1m, true),
                        GasAttribution(202, "Alpha II", "hydrogen")
                    })
                };
                var completeGateway = new FakeCompleteClusterGateway();
                var lifecycle = new PreviewSessionLifecycle();
                using var resolver = new PreviewResolutionCoordinator(
                    lifecycle,
                    new PreviewScanCoordinator(previewGateway, gate),
                    new CompleteClusterRawCoordinator(completeGateway, gate),
                    new CompleteClusterConclusionCache(path));

                PreviewLoadTransition first = resolver.ObserveCompletedLoad(
                    1,
                    PreviewIdentity(16_315_224),
                    Request());
                Equal(PreviewLoadDisposition.SessionCreated, first.Disposition);
                PreviewResolutionAttempt scanned = resolver.CurrentPublishedAttempt!;
                Equal(PreviewResolutionState.Scanning, scanned.State);
                Equal(1, previewGateway.GenerateCalls);
                Equal(1, completeGateway.GenerateCalls);

                PreviewLoadTransition duplicate = resolver.ObserveCompletedLoad(
                    1,
                    PreviewIdentity(16_315_224),
                    Request());
                Equal(PreviewLoadDisposition.DuplicateCoalesced, duplicate.Disposition);
                Equal(1, previewGateway.GenerateCalls);
                Equal(1, completeGateway.GenerateCalls);

                while (!scanned.IsTerminal)
                    resolver.AdvanceCurrent();
                Equal(PreviewResolutionState.Complete, scanned.State);
                Equal(1, scanned.TerminalTransitionCount);
                True(scanned.CacheStored);
                True(scanned.PreviewReports.Count > 0);
                True(scanned.CompleteReports.Count > 0);
                Equal(scanned.ExpectedPlanets, scanned.CompletedPlanets);

                PreviewLoadTransition reload = resolver.ObserveCompletedLoad(
                    2,
                    PreviewIdentity(16_315_224),
                    Request());
                Equal(PreviewLoadDisposition.SessionCreated, reload.Disposition);
                PreviewResolutionAttempt cached = resolver.CurrentPublishedAttempt!;
                Equal(PreviewResolutionState.Cached, cached.State);
                Equal(1, cached.TerminalTransitionCount);
                Equal(2, previewGateway.GenerateCalls);
                Equal(1, completeGateway.GenerateCalls);
                True(cached.HasCompleteBirthPlanetAttribution);
                Equal("Alpha I,Alpha II", String.Join(",",
                    cached.BirthPlanetAttributions.Select(value => value.DisplayName)));
                Equal(scanned.CompleteReports.Count, cached.CompleteReports.Count);
                True(cached.CompleteReports.Select(report =>
                    report.ConclusionId + "\t" + report.Outcome).SequenceEqual(
                        scanned.CompleteReports.Select(report =>
                            report.ConclusionId + "\t" + report.Outcome)));
            });
        }

        private static void AutomaticResolutionCancelsReplacementAndExit()
        {
            WithTemporaryDirectory(path =>
            {
                var gate = new RuntimeOperationGate();
                var lifecycle = new PreviewSessionLifecycle();
                using var resolver = new PreviewResolutionCoordinator(
                    lifecycle,
                    new PreviewScanCoordinator(new FakeGateway(), gate),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway(), gate),
                    new CompleteClusterConclusionCache(path));

                resolver.ObserveCompletedLoad(10, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt obsolete = resolver.CurrentPublishedAttempt!;
                resolver.AdvanceCurrent();

                PreviewScanRequest replacementRequest = RequestForSeed(73_339_583);
                resolver.ObserveCompletedLoad(
                    11,
                    PreviewIdentity(73_339_583),
                    replacementRequest);
                PreviewResolutionAttempt replacement = resolver.CurrentPublishedAttempt!;
                Equal(PreviewResolutionState.Cancelled, obsolete.State);
                Equal(1, obsolete.TerminalTransitionCount);
                False(lifecycle.CanPublish(obsolete.Session));
                True(lifecycle.CanPublish(replacement.Session));

                PreviewResolutionAttempt? exited = resolver.ExitPreview();
                True(ReferenceEquals(replacement, exited));
                Equal(PreviewResolutionState.Cancelled, replacement.State);
                Equal(1, replacement.TerminalTransitionCount);
                True(resolver.CurrentPublishedAttempt == null);
                resolver.AdvanceCurrent();
                Equal(1, replacement.TerminalTransitionCount);
            });
        }

        private static void AutomaticResolutionFailuresNeverRetry()
        {
            WithTemporaryDirectory(path =>
            {
                var busyGate = new RuntimeOperationGate();
                True(busyGate.TryEnter());
                using (var busy = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(new FakeGateway(), busyGate),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway(), busyGate),
                    new CompleteClusterConclusionCache(Path.Combine(path, "busy"))))
                {
                    busy.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                    PreviewResolutionAttempt attempt = busy.CurrentPublishedAttempt!;
                    Equal(PreviewResolutionState.Busy, attempt.State);
                    busy.AdvanceCurrent();
                    Equal(1, attempt.TerminalTransitionCount);
                }
                busyGate.Exit();

                var incompatibleGateway = new FakeGateway
                {
                    Fingerprint = Fingerprint(gameVersion: "unsupported")
                };
                var incompatibleGate = new RuntimeOperationGate();
                using (var incompatible = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(incompatibleGateway, incompatibleGate),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway(), incompatibleGate),
                    new CompleteClusterConclusionCache(Path.Combine(path, "incompatible"))))
                {
                    incompatible.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                    PreviewResolutionAttempt attempt = incompatible.CurrentPublishedAttempt!;
                    Equal(PreviewResolutionState.Incompatible, attempt.State);
                    incompatible.AdvanceCurrent();
                    Equal(1, incompatibleGateway.FingerprintCalls);
                    Equal(1, attempt.TerminalTransitionCount);
                }

                var failedGateway = new FakeGateway
                {
                    GenerationFailure = new InvalidOperationException("injected preview failure")
                };
                var failedGate = new RuntimeOperationGate();
                using (var failed = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(failedGateway, failedGate),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway(), failedGate),
                    new CompleteClusterConclusionCache(Path.Combine(path, "failed"))))
                {
                    failed.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                    PreviewResolutionAttempt attempt = failed.CurrentPublishedAttempt!;
                    Equal(PreviewResolutionState.Failed, attempt.State);
                    failed.AdvanceCurrent();
                    Equal(1, failedGateway.GenerateCalls);
                    Equal(1, attempt.TerminalTransitionCount);
                    Equal(0, attempt.CompleteReports.Count);
                }

                var rawFailure = new FakeCompleteClusterGateway
                {
                    GenerationFailure = new InvalidOperationException("injected complete failure")
                };
                var rawFailureGate = new RuntimeOperationGate();
                using var completeFailure = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(new FakeGateway(), rawFailureGate),
                    new CompleteClusterRawCoordinator(rawFailure, rawFailureGate),
                    new CompleteClusterConclusionCache(Path.Combine(path, "raw-failed")));
                completeFailure.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt rawAttempt = completeFailure.CurrentPublishedAttempt!;
                completeFailure.AdvanceCurrent();
                Equal(PreviewResolutionState.Failed, rawAttempt.State);
                completeFailure.AdvanceCurrent();
                Equal(1, rawAttempt.TerminalTransitionCount);
                Equal(0, rawAttempt.CompleteReports.Count);
            });
        }

        private static void PanelMapsEveryOperationalState()
        {
            PreviewPanelView waiting = PreviewPanelStateMapper.Waiting(
                1,
                PreviewPanelCorner.BottomRight,
                0);
            Equal(PreviewPanelOperationalState.Waiting, waiting.State);
            True(waiting.Spinner.HasValue);
            False(waiting.Spinner == PreviewPanelStateMapper.Waiting(
                1,
                PreviewPanelCorner.BottomRight,
                1).Spinner);

            var expected = new[]
            {
                (PreviewResolutionState.Scanning, 10, 3, PreviewPanelOperationalState.Scanning, true),
                (PreviewResolutionState.Cached, 10, 10, PreviewPanelOperationalState.Cached, false),
                (PreviewResolutionState.Complete, 10, 10, PreviewPanelOperationalState.Complete, false),
                (PreviewResolutionState.Cancelled, 10, 3, PreviewPanelOperationalState.Cancelled, false),
                (PreviewResolutionState.Incompatible, 0, 0, PreviewPanelOperationalState.Unsupported, false),
                (PreviewResolutionState.Busy, 0, 0, PreviewPanelOperationalState.Failed, false),
                (PreviewResolutionState.Failed, 10, 3, PreviewPanelOperationalState.Failed, false)
            };
            foreach ((PreviewResolutionState source, int total, int done,
                PreviewPanelOperationalState state, bool spins) in expected)
            {
                PreviewPanelView view = PreviewPanelStateMapper.Project(
                    2,
                    source,
                    total,
                    done,
                    PreviewPanelCorner.TopLeft,
                    2);
                True(view.Visible);
                Equal(state, view.State);
                Equal(spins, view.Spinner.HasValue);
                int renderedTitleLength = view.Title.Length +
                    (view.Spinner.HasValue ? 3 : 0);
                True(renderedTitleLength <= PreviewPanelLayout.MaximumTitleCharacters);
                True(view.Detail.Length <= PreviewPanelLayout.MaximumDetailCharacters);
                False(view.Title.Contains('\n'));
                False(view.Detail.Contains('\n'));
            }

            PreviewPanelView modUncertainty = PreviewPanelStateMapper.Project(
                4,
                PreviewResolutionState.Incompatible,
                0,
                0,
                PreviewPanelCorner.BottomRight,
                0,
                "generation-mod-uncertain");
            True(modUncertainty.Detail.Contains("loaded plugins"));
            True(modUncertainty.Detail.Length <= PreviewPanelLayout.MaximumDetailCharacters);

            PreviewPanelView planning = PreviewPanelStateMapper.Project(
                3,
                PreviewResolutionState.Scanning,
                0,
                0,
                PreviewPanelCorner.BottomLeft,
                3);
            Equal(PreviewPanelOperationalState.Waiting, planning.State);
            True(planning.Spinner.HasValue);
            False(planning.Spinner == PreviewPanelStateMapper.Project(
                3,
                PreviewResolutionState.Scanning,
                1,
                0,
                PreviewPanelCorner.BottomLeft,
                0).Spinner);
        }

        private static void PanelCornersMapClockwise()
        {
            Equal(1, PreviewPanelLayout.DefaultCornerCode);
            Equal(PreviewPanelCorner.BottomRight, PreviewPanelLayout.ParseCorner(1));
            Equal(PreviewPanelCorner.BottomLeft, PreviewPanelLayout.ParseCorner(2));
            Equal(PreviewPanelCorner.TopLeft, PreviewPanelLayout.ParseCorner(3));
            Equal(PreviewPanelCorner.TopRight, PreviewPanelLayout.ParseCorner(4));
            Equal(PreviewPanelCorner.BottomRight, PreviewPanelLayout.ParseCorner(0));
            Equal(PreviewPanelCorner.BottomRight, PreviewPanelLayout.ParseCorner(5));
            Equal(1.0, PreviewPanelLayout.ScaleForScreen(
                1920,
                1080,
                PreviewPanelLayout.Width,
                PreviewPanelLayout.Height));
            Equal(1.5, PreviewPanelLayout.ScaleForScreen(
                3840,
                2160,
                PreviewPanelLayout.Width,
                PreviewPanelLayout.Height));
            const int legacyDocumentWidth = 1040;
            double fittedScale = PreviewPanelLayout.ScaleForScreen(
                3840,
                2160,
                legacyDocumentWidth,
                1600);
            True(fittedScale > 1.0 && fittedScale < 1.5);
            True((legacyDocumentWidth + PreviewPanelLayout.Margin * 2) *
                fittedScale <= 3840);
            True((1600 + PreviewPanelLayout.Margin * 2) * fittedScale <= 2160);

            double presentationScale = PreviewPanelLayout.ScaleForScreen(
                3840,
                2160,
                legacyDocumentWidth,
                600);
            int logicalWidth = (int)Math.Floor(3840 / presentationScale);
            int logicalHeight = (int)Math.Floor(2160 / presentationScale);
            PreviewPanelBounds presentationBounds = PreviewPanelLayout.PlaceSized(
                PreviewPanelCorner.BottomRight,
                logicalWidth,
                logicalHeight,
                legacyDocumentWidth,
                600);
            True(presentationBounds.X >= logicalWidth / 2);
            True(presentationBounds.Y >= logicalHeight / 2);
            True(presentationBounds.Bottom <=
                logicalHeight - PreviewPanelLayout.BottomClearance);

            const int width = 3840;
            const int height = 2160;
            var expected = new[]
            {
                (PreviewPanelCorner.BottomRight, 3296, 1924),
                (PreviewPanelCorner.BottomLeft, 24, 1924),
                (PreviewPanelCorner.TopLeft, 24, 24),
                (PreviewPanelCorner.TopRight, 3296, 24)
            };
            foreach ((PreviewPanelCorner corner, int x, int y) in expected)
            {
                PreviewPanelBounds bounds = PreviewPanelLayout.Place(corner, width, height);
                Equal(x, bounds.X);
                Equal(y, bounds.Y);
                Equal(PreviewPanelLayout.Width, bounds.Width);
                Equal(PreviewPanelLayout.Height, bounds.Height);
                True(bounds.X > 0 && bounds.Y > 0);
                True(bounds.Right < width && bounds.Bottom < height);
                True(bounds.Right < width / 2 || bounds.X > width / 2);
                True(bounds.Bottom < height / 2 || bounds.Y > height / 2);
            }

            const int logical4KWidth = 2560;
            const int logical4KHeight = 1440;
            var conclusionExpected = new[]
            {
                (PreviewPanelCorner.BottomRight, 1589, 787),
                (PreviewPanelCorner.BottomLeft, 24, 755),
                (PreviewPanelCorner.TopLeft, 24, 120),
                (PreviewPanelCorner.TopRight, 1589, 384)
            };
            foreach ((PreviewPanelCorner corner, int x, int y) in conclusionExpected)
            {
                PreviewPanelBounds bounds = PreviewPanelLayout.PlaceConclusion(
                    corner,
                    logical4KWidth,
                    logical4KHeight);
                Equal(x, bounds.X);
                Equal(y, bounds.Y);
                Equal(947, bounds.Width);
                Equal(533, bounds.Height);
                Equal(
                    (int)Math.Round(
                        logical4KWidth * PreviewPanelLayout.ConclusionWidthRatio,
                        MidpointRounding.AwayFromZero),
                    bounds.Width);
                Equal(
                    (int)Math.Round(
                        logical4KHeight * PreviewPanelLayout.ConclusionHeightRatio,
                        MidpointRounding.AwayFromZero),
                    bounds.Height);
                True(bounds.Right <= logical4KWidth - PreviewPanelLayout.Margin);
                True(bounds.Bottom <= logical4KHeight - PreviewPanelLayout.Margin);
            }

            PreviewPanelBounds topRight = PreviewPanelLayout.PlaceConclusion(
                PreviewPanelCorner.TopRight,
                1920,
                1080);
            Equal(
                PreviewPanelLayout.Margin + PreviewPanelLayout.TopRightClearance,
                topRight.Y);
            PreviewPanelBounds bottomLeft = PreviewPanelLayout.PlaceConclusion(
                PreviewPanelCorner.BottomLeft,
                1920,
                1080);
            Equal(
                1080 - PreviewPanelLayout.Margin -
                    PreviewPanelLayout.BottomLeftClearance,
                bottomLeft.Bottom);
        }

        private static void PanelRejectsObsoleteSessions()
        {
            WithTemporaryDirectory(path =>
            {
                var gate = new RuntimeOperationGate();
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(new FakeGateway(), gate),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway(), gate),
                    new CompleteClusterConclusionCache(path));
                var panel = new PreviewPanelController();

                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt first = resolver.CurrentPublishedAttempt!;
                panel.BeginSession(first.Session.SessionId, PreviewPanelCorner.BottomRight, 0);
                Equal(PreviewPanelOperationalState.Waiting, panel.Current.State);
                True(panel.Update(first, PreviewPanelCorner.BottomRight, 1));
                Equal(PreviewPanelOperationalState.Scanning, panel.Current.State);
                PreviewPanelView firstVisible = panel.Current;
                PreviewConclusionPresentation firstConclusions = panel.Conclusions!;

                resolver.ObserveCompletedLoad(
                    2,
                    PreviewIdentity(73_339_583),
                    RequestForSeed(73_339_583));
                PreviewResolutionAttempt replacement = resolver.CurrentPublishedAttempt!;
                False(panel.Update(first, PreviewPanelCorner.TopLeft, 2));
                True(ReferenceEquals(firstVisible, panel.Current));
                True(ReferenceEquals(firstConclusions, panel.Conclusions));

                panel.BeginSession(
                    replacement.Session.SessionId,
                    PreviewPanelCorner.TopLeft,
                    2);
                PreviewPanelView replacementWaiting = panel.Current;
                True(panel.Conclusions == null);
                False(panel.Update(first, PreviewPanelCorner.BottomRight, 3));
                True(ReferenceEquals(replacementWaiting, panel.Current));
                True(panel.Conclusions == null);
                False(panel.Hide(first.Session.SessionId));
                True(panel.Current.Visible);
                True(panel.Update(replacement, PreviewPanelCorner.TopLeft, 3));
                Equal(replacement.Session.SessionId, panel.Current.SessionId);
                True(panel.Hide(replacement.Session.SessionId));
                False(panel.Current.Visible);
                True(panel.Conclusions == null);
                Equal(PreviewPanelOperationalState.Hidden, panel.Current.State);
            });
        }

        private static void ConclusionCardsMapEveryOutcomeAndSubject()
        {
            var outcomes = new[]
            {
                (ComponentOutcome.Supports, PreviewConclusionColumn.Strength),
                (ComponentOutcome.DoesNotSupport, PreviewConclusionColumn.Limitation),
                (ComponentOutcome.PreferenceSensitive,
                    PreviewConclusionColumn.PreferenceSensitive),
                (ComponentOutcome.Tradeoff, PreviewConclusionColumn.PreferenceSensitive),
                (ComponentOutcome.Caution, PreviewConclusionColumn.Limitation)
            };
            RuntimeSystemDisplay[] displays =
            {
                new RuntimeSystemDisplay("1", "Alpha", "G type star"),
                new RuntimeSystemDisplay("2", "Beta", "O type star"),
                new RuntimeSystemDisplay("4", "Delta", "B type star"),
                new RuntimeSystemDisplay("7", "Eta", "A type star"),
                new RuntimeSystemDisplay("9", "Iota", "K type star")
            };
            foreach ((ComponentOutcome outcome, PreviewConclusionColumn column) in outcomes)
            {
                ConclusionReport report = PresentationReport(
                    ConclusionContext.Megafactory,
                    outcome,
                    EvidenceStage.GalaxyPreview,
                    "MF-ENERGY-SYSTEM.output",
                    new ConclusionSubject(SubjectKind.StarSystem, "7"));
                PresentedConclusionCard card = PreviewConclusionPresenter.MapCard(
                    report,
                    displays);
                Equal(outcome, card.Outcome);
                Equal(column, card.Column);
                True(card.Line.Contains("Eta (A type star)", StringComparison.Ordinal));
                False(card.Line.Contains("System 7", StringComparison.Ordinal));
                True(card.Line.Length <= PreviewConclusionPresenter.MaximumLineCharacters);
                Equal(1, card.SourceConclusionIds.Count);
                False(card.Line.Contains("987654321", StringComparison.Ordinal));
            }

            var subjectCases = new[]
            {
                (PresentationReport(
                    ConclusionContext.FreshStart,
                    ComponentOutcome.Supports,
                    EvidenceStage.GalaxyPreview,
                    "FS-TOPOLOGY.shared-satellites",
                    new ConclusionSubject(SubjectKind.BirthSystem, "1")),
                    "Alpha (G type star)"),
                (PresentationReport(
                    ConclusionContext.DarkFogFarming,
                    ComponentOutcome.Tradeoff,
                    EvidenceStage.GalaxyPreview,
                    "DF-OCCUPATION.tradeoff",
                    new ConclusionSubject(SubjectKind.Cluster, "cluster")), "Cluster"),
                (PresentationReport(
                    ConclusionContext.Megafactory,
                    ComponentOutcome.Supports,
                    EvidenceStage.CompleteClusterRaw,
                    "RR-ACCESS.distance:unipolar-magnet",
                    new ConclusionSubject(
                        SubjectKind.Resource,
                        "seed:resource:unipolar-magnet"),
                    new DecisiveFact("distanceFromBirth", "12.34567", "light-years")),
                    "Unipolar Magnet - 12.3 ly from birth"),
                (PresentationReport(
                    ConclusionContext.CompactExpansion,
                    ComponentOutcome.PreferenceSensitive,
                    EvidenceStage.GalaxyPreview,
                    "CX-GROUPING.distance",
                    new ConclusionSubject(SubjectKind.SystemPair, "2<->9:role-a+role-b"),
                    new DecisiveFact("systemDistance", "2.34567", "light-years")),
                    "2.35 ly between Beta (O type star) / Iota (K type star)"),
                (PresentationReport(
                    ConclusionContext.DecisionRelevantTraits,
                    ComponentOutcome.Supports,
                    EvidenceStage.GalaxyPreview,
                    "TRAIT-SUMMARY.registry:shared-birth-satellites",
                    new ConclusionSubject(SubjectKind.Trait, "shared-birth-satellites@1")),
                    "Shared Birth Satellites"),
                (PresentationReport(
                    ConclusionContext.FreshStart,
                    ComponentOutcome.Supports,
                    EvidenceStage.GalaxyPreview,
                    "FS-GAS-ROUTE.product:hydrogen",
                    new ConclusionSubject(SubjectKind.BirthSystem, "1")),
                    "Hydrogen presence"),
                (PresentationReport(
                    ConclusionContext.Megafactory,
                    ComponentOutcome.Supports,
                    EvidenceStage.GalaxyPreview,
                    "MF-SYSTEM-ROLE.role:strong-energy",
                    new ConclusionSubject(SubjectKind.StarSystem, "2")),
                    "Strong Energy @ Beta (O type star)"),
                (PresentationReport(
                    ConclusionContext.SphereShowcase,
                    ComponentOutcome.Supports,
                    EvidenceStage.GalaxyPreview,
                    "MF-SPHERE-GEOMETRY.containment",
                    new ConclusionSubject(SubjectKind.StarSystem, "4")),
                    "Contained orbits @ Delta (B type star)")
            };
            foreach ((ConclusionReport report, string subject) in subjectCases)
            {
                PresentedConclusionCard card = PreviewConclusionPresenter.MapCard(
                    report,
                    displays);
                True(card.Subjects.Contains(subject));
                True(card.Line.Contains(subject, StringComparison.Ordinal));
            }

            bool unsupportedRejected = false;
            try
            {
                PreviewConclusionPresenter.MapCard(PresentationReport(
                    ConclusionContext.FreshStart,
                    ComponentOutcome.Supports,
                    EvidenceStage.GalaxyPreview,
                    "UNACCEPTED.claim",
                    new ConclusionSubject(SubjectKind.Cluster, "cluster")));
            }
            catch (InvalidOperationException)
            {
                unsupportedRejected = true;
            }
            True(unsupportedRejected);
        }

        private static void ConclusionPanelSeparatesContextsAndConflicts()
        {
            WithTemporaryDirectory(path =>
            {
                var gate = new RuntimeOperationGate();
                var lifecycle = new PreviewSessionLifecycle();
                using var resolver = new PreviewResolutionCoordinator(
                    lifecycle,
                    new PreviewScanCoordinator(new FakeGateway(), gate),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway(), gate),
                    new CompleteClusterConclusionCache(path));
                var panel = new PreviewPanelController();

                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt attempt = resolver.CurrentPublishedAttempt!;
                panel.BeginSession(attempt.Session.SessionId, PreviewPanelCorner.BottomRight, 0);
                True(panel.Update(attempt, PreviewPanelCorner.BottomRight, 0));
                PreviewConclusionPresentation scanning = panel.Conclusions!;
                Equal("Seed 16315224 | 64 stars | resources x1 | Combat", scanning.IdentityLine);
                Equal(0, scanning.DetailGroups.Count);
                Equal(6, scanning.ImmediateGroups.Count);
                Equal(
                    "Fresh start,Megafactory,Dark Fog farming,Compact expansion,Sphere / energy,Decision-relevant traits",
                    String.Join(",", scanning.ImmediateGroups.Select(group => group.Title)));
                True(scanning.ImmediateGroups.SelectMany(group => group.Cards)
                    .All(card => card.Stage == EvidenceStage.GalaxyPreview));
                True(scanning.ImmediateGroups.SelectMany(group => group.Cards)
                    .All(card => card.Outcome != ComponentOutcome.Unknown &&
                        card.Outcome != ComponentOutcome.NotApplicable));
                True(scanning.ImmediateGroups.SelectMany(group => group.Cards)
                    .All(card => !card.Line.Contains(":star:", StringComparison.Ordinal)));
                resolver.AdvanceCurrent();
                panel.Update(attempt, PreviewPanelCorner.BottomRight, 1);
                True(ReferenceEquals(scanning, panel.Conclusions));
                Equal("Planets 1 / 3", panel.Current.Detail);

                ConclusionReport[] decisiveReports = attempt.PreviewReports.Where(report =>
                    report.Stage == EvidenceStage.GalaxyPreview &&
                    (report.Outcome == ComponentOutcome.Tradeoff ||
                     report.Outcome == ComponentOutcome.Caution)).ToArray();
                PresentedConclusionCard[] decisiveCards = scanning.ImmediateGroups
                    .SelectMany(group => group.Cards)
                    .Where(card => card.Outcome == ComponentOutcome.Tradeoff ||
                        card.Outcome == ComponentOutcome.Caution)
                    .ToArray();
                Equal(decisiveReports.Length, decisiveCards.Length);
                foreach (ConclusionReport report in decisiveReports)
                {
                    True(decisiveCards.Any(card =>
                        card.SourceConclusionIds.Contains(report.ConclusionId)));
                }

                while (!attempt.IsTerminal)
                {
                    resolver.AdvanceCurrent();
                    panel.Update(attempt, PreviewPanelCorner.BottomRight, 1);
                }
                Equal(PreviewResolutionState.Complete, attempt.State);
                PreviewConclusionPresentation complete = panel.Conclusions!;
                True(complete.DetailGroups.Count > 0);
                True(complete.DetailGroups.SelectMany(group => group.Cards)
                    .All(card => card.Stage == EvidenceStage.CompleteClusterRaw));
                True(complete.ImmediateGroups.SelectMany(group => group.Cards)
                    .All(card => card.Stage == EvidenceStage.GalaxyPreview));
                PreviewPanelDocument completeDocument = PreviewConclusionPresenter.Compose(
                    panel.Current,
                    complete);
                True(completeDocument.Lines.Count <=
                    PreviewConclusionPresenter.MaximumDocumentLines);
                foreach (PreviewPanelCorner corner in Enum.GetValues<PreviewPanelCorner>())
                {
                    PreviewPanelBounds bounds = PreviewPanelLayout.PlaceConclusion(
                        corner,
                        3840,
                        2160);
                    True(bounds.X > 0 && bounds.Y > 0);
                    True(bounds.Right < 3840 && bounds.Bottom < 2160);
                }

                resolver.ObserveCompletedLoad(2, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt cachedAttempt = resolver.CurrentPublishedAttempt!;
                panel.BeginSession(
                    cachedAttempt.Session.SessionId,
                    PreviewPanelCorner.BottomRight,
                    0);
                True(panel.Update(cachedAttempt, PreviewPanelCorner.BottomRight, 0));
                Equal(PreviewResolutionState.Cached, cachedAttempt.State);
                True(panel.Conclusions!.IsCached);
                True(panel.Conclusions.DetailGroups.Count > 0);
            });
        }

        private static void ConclusionPanelSnapshotIsBoundedAndNeutral()
        {
            WithTemporaryDirectory(path =>
            {
                var gate = new RuntimeOperationGate();
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(new FakeGateway(), gate),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway(), gate),
                    new CompleteClusterConclusionCache(path));
                var panel = new PreviewPanelController();
                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt attempt = resolver.CurrentPublishedAttempt!;
                panel.BeginSession(attempt.Session.SessionId, PreviewPanelCorner.TopRight, 0);
                panel.Update(attempt, PreviewPanelCorner.TopRight, 0);

                PreviewPanelDocument document = PreviewConclusionPresenter.Compose(
                    panel.Current,
                    panel.Conclusions);
                Equal(PreviewPanelLineKind.Identity, document.Lines[0].Kind);
                Equal("Seed 16315224 | 64 stars | resources x1 | Combat", document.Lines[0].Text);
                Equal(
                    "|  Scanning complete cluster - Planets 0 / 3",
                    document.Lines[1].Text);
                Equal("Fresh start", document.Lines[2].Text);
                False(document.Lines.Any(line => line.Text.Contains(
                    "conclusions",
                    StringComparison.OrdinalIgnoreCase)));
                True(document.Lines.Count <= PreviewConclusionPresenter.MaximumDocumentLines);
                True(document.Lines.All(line =>
                    line.Text.Length <= PreviewConclusionPresenter.MaximumLineCharacters));

                string rendered = String.Join("\n", document.Lines.Select(line => line.Text));
                foreach (string forbidden in new[]
                {
                    "FS-", "MF-", "DF-", "CX-", "RR-", "TRAIT-",
                    "runtime-amount", "score", "ranking", "universal verdict",
                    "best seed", "Unknown:", "Not applicable:", "System 16315224"
                })
                {
                    False(rendered.IndexOf(forbidden, StringComparison.OrdinalIgnoreCase) >= 0);
                }

                PreviewPanelBounds bounds = PreviewPanelLayout.PlaceConclusion(
                    PreviewPanelCorner.TopRight,
                    3840,
                    2160);
                True(bounds.X > 1920);
                True(bounds.Y > 0);
                True(bounds.Right < 3840);
                True(bounds.Bottom < 2160);
            });
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
                typeof(RuntimeSystemDisplay),
                typeof(RawPlanetResult),
                typeof(NormalizedRawPlanetEvidence),
                typeof(NormalizedRawVeinNode),
                typeof(NormalizedRawVeinGroup),
                typeof(BirthSystemRawResult),
                typeof(BirthSystemRawProgress),
                typeof(CompleteClusterRawResult),
                typeof(CompleteClusterRawProgress),
                typeof(CompleteClusterRawOperation),
                typeof(CompleteClusterCacheKey),
                typeof(CompleteClusterConclusionCache),
                typeof(CachedCompleteClusterConclusions),
                typeof(PreviewGenerationIdentity),
                typeof(PreviewSession),
                typeof(PreviewLoadTransition),
                typeof(PreviewSessionLifecycle),
                typeof(PreviewResolutionAttempt),
                typeof(PreviewResolutionCoordinator),
                typeof(PreviewPanelBounds),
                typeof(PreviewPanelView),
                typeof(PreviewPanelController),
                typeof(PresentedConclusionCard),
                typeof(PresentedContextGroup),
                typeof(PreviewConclusionPresentation),
                typeof(PreviewPanelDocument)
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

        private static PreviewScanRequest Request(
            decimal resourceMultiplier = 1m,
            CombatMode combatMode = CombatMode.Combat,
            decimal initialColonize = 1m,
            decimal maxDensity = 1m)
        {
            return new PreviewScanRequest(
                16_315_224,
                ConclusionDefinition.ReferenceStarCount,
                ConclusionDefinition.ReferenceGameVersion,
                resourceMultiplier,
                combatMode,
                PreviewScanRequest.CombatSettingsKeyFor(initialColonize, maxDensity),
                initialColonize,
                maxDensity);
        }

        private static PreviewScanRequest RequestForSeed(int seed)
        {
            return new PreviewScanRequest(
                seed,
                ConclusionDefinition.ReferenceStarCount,
                ConclusionDefinition.ReferenceGameVersion,
                1m,
                CombatMode.Combat,
                ConclusionDefinition.ReferenceCombatSettingsKey);
        }

        private static ConclusionReport PresentationReport(
            ConclusionContext context,
            ComponentOutcome outcome,
            EvidenceStage stage,
            string conclusionId,
            ConclusionSubject subject,
            DecisiveFact? decisiveFact = null)
        {
            DiagnosticCause? cause = outcome == ComponentOutcome.Unknown ||
                outcome == ComponentOutcome.NotApplicable
                ? new DiagnosticCause("fixture", "Presentation fixture diagnostic.")
                : null;
            return new ConclusionReport(
                PreviewIdentity(16_315_224).GalaxyIdentity,
                new EvaluationSettings(
                    1m,
                    CombatMode.Combat,
                    ConclusionDefinition.ReferenceCombatSettingsKey),
                new EvidenceCoverage(
                    stage,
                    stage == EvidenceStage.CompleteClusterRaw
                        ? EvidenceScope.CompleteClusterResources
                        : EvidenceScope.ClusterEnergy,
                    CoverageState.Complete,
                    1,
                    1),
                conclusionId,
                context,
                ConclusionDefinition.ContractVersion,
                ConclusionDefinition.DefinitionVersion,
                subject,
                outcome,
                decisiveFact ?? new DecisiveFact(
                    "fixture",
                    "987654321",
                    "fixture-units"),
                cause);
        }

        private static CompleteClusterRawResult CompleteResult(decimal resourceMultiplier = 1m) =>
            new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway())
                .TryGenerate(Request(resourceMultiplier), CancellationToken.None);

        private static void WithTemporaryDirectory(Action<string> action)
        {
            string path = Path.Combine(
                Path.GetTempPath(),
                "DSPSeedScanner.Runtime.Tests",
                Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(path);
            try
            {
                action(path);
            }
            finally
            {
                if (Directory.Exists(path))
                    Directory.Delete(path, recursive: true);
            }
        }

        private static PreviewGenerationIdentity PreviewIdentity(
            int seed,
            decimal resourceMultiplier = 1m,
            CombatMode combatMode = CombatMode.Combat,
            decimal initialColonize = 1m,
            decimal maxDensity = 1m)
        {
            var galaxy = new GenerationIdentity(
                ConclusionDefinition.ReferenceGameVersion,
                ConclusionDefinition.ReferenceGalaxyAlgorithm,
                ConclusionDefinition.ReferenceAssemblySha256,
                ConclusionDefinition.ReferenceOrderedThemeIds,
                ConclusionDefinition.DefinitionVersion,
                seed,
                ConclusionDefinition.ReferenceStarCount,
                ConclusionDefinition.ReferenceGameVersion);
            return new PreviewGenerationIdentity(
                galaxy,
                resourceMultiplier,
                combatMode,
                PreviewScanRequest.CombatSettingsKeyFor(initialColonize, maxDensity),
                initialColonize,
                maxDensity);
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

        private static RuntimePreviewSnapshot Snapshot(
            int generatedStarCount = 64,
            IEnumerable<NormalizedBirthPlanetEvidence>? birthPlanetAttributions = null)
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
                    birth ? 1 : leader ? 39 : 0,
                    birth ? birthPlanetAttributions : null));
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
                distances,
                systemDisplays: Enumerable.Range(1, generatedStarCount)
                    .Select(index => new RuntimeSystemDisplay(
                        index.ToString(),
                        index == 1 ? "Alpha" : "Star " + index,
                        index == 2 ? "O type star" : "G type star")));
        }

        private static NormalizedBirthPlanetEvidence SolidAttribution(
            int planetId,
            string displayName,
            decimal solarRatio,
            decimal windRatio,
            bool isTidalLocked)
        {
            return new NormalizedBirthPlanetEvidence(
                planetId,
                displayName,
                false,
                solarRatio,
                windRatio,
                isTidalLocked,
                null);
        }

        private static NormalizedBirthPlanetEvidence GasAttribution(
            int planetId,
            string displayName,
            params string[] products)
        {
            return new NormalizedBirthPlanetEvidence(
                planetId,
                displayName,
                true,
                null,
                null,
                null,
                products);
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
            public int YieldRestoreChecks { get; private set; }
            public int SessionDisposeCalls { get; private set; }
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

            public IRuntimeCompleteClusterRawSession OpenCompleteCluster(
                PreviewScanRequest request,
                CompleteClusterRawPlan plan,
                CancellationToken cancellationToken,
                Action<string> recordTrace)
            {
                GenerateCalls++;
                cancellationToken.ThrowIfCancellationRequested();
                return new Session(this, request.GalaxySeed, recordTrace);
            }

            private sealed class Session : IRuntimeCompleteClusterRawSession
            {
                private readonly FakeCompleteClusterGateway owner;
                private readonly int galaxySeed;
                private readonly Action<string> recordTrace;
                private bool disposed;

                public Session(
                    FakeCompleteClusterGateway owner,
                    int galaxySeed,
                    Action<string> recordTrace)
                {
                    this.owner = owner;
                    this.galaxySeed = galaxySeed;
                    this.recordTrace = recordTrace;
                    StateRestored = true;
                }

                public bool StateRestored { get; private set; }

                public NormalizedRawPlanetEvidence GeneratePlanet(
                    CompleteClusterPlanetTarget target,
                    CancellationToken cancellationToken,
                    Action<string> stepTrace)
                {
                    if (disposed)
                        throw new ObjectDisposedException(nameof(Session));
                    cancellationToken.ThrowIfCancellationRequested();
                    owner.StateMarker = "candidate";
                    StateRestored = false;
                    try
                    {
                        if (owner.GenerationFailure != null)
                            throw owner.GenerationFailure;
                        owner.OnPlanet?.Invoke();
                        if (owner.FailingPlanetId == target.PlanetId)
                            throw new InvalidOperationException("injected cluster planet failure");
                        return target.PlanetId switch
                        {
                            102 => ClusterSnapshot(galaxySeed, target, "kimberlite", 9, 1128, 200),
                            103 => ClusterSnapshot(galaxySeed, target, "unipolar-magnet", 14, 1016, 300),
                            _ => ClusterSnapshot(galaxySeed, target, null, 0, 0, 0)
                        };
                    }
                    finally
                    {
                        owner.StateMarker = "leased";
                        StateRestored = true;
                        owner.YieldRestoreChecks++;
                    }
                }

                public void Dispose()
                {
                    if (disposed)
                        return;
                    disposed = true;
                    owner.StateMarker = "leased";
                    StateRestored = true;
                    owner.SessionDisposeCalls++;
                    recordTrace("cluster-raw:candidate:released");
                }
            }

            private static NormalizedRawPlanetEvidence ClusterSnapshot(
                int galaxySeed,
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
                    galaxySeed,
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
