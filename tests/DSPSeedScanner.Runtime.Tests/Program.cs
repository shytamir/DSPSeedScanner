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
                ("home topology verifies only the home planet parent", HomeTopologyVerifiesOnlyHomePlanetParent),
                ("system candidates are bounded deterministic and owned", SystemCandidatesAreBoundedDeterministicAndOwned),
                ("incomplete system candidate evidence stays unknown", IncompleteSystemCandidateEvidenceStaysUnknown),
                ("unsupported game identity rejects safely", UnsupportedGameIdentityRejectsSafely),
                ("missing members reject while plugins coexist", MissingMembersRejectWhilePluginsCoexist),
                ("generation changes coexist and remain identified", GenerationChangesCoexistAndRemainIdentified),
                ("runtime filesystem context follows the active process", RuntimeFilesystemContextFollowsActiveProcess),
                ("runtime filesystem context fails closed on identity conflicts", RuntimeFilesystemContextFailsClosedOnConflicts),
                ("runtime filesystem optional paths degrade independently", RuntimeFilesystemOptionalPathsDegradeIndependently),
                ("runtime file fingerprints fall back without throwing", RuntimeFileFingerprintsFallBackWithoutThrowing),
                ("runtime filesystem guards contain expected failures", RuntimeFilesystemGuardsContainExpectedFailures),
                ("unsupported request identity rejects safely", UnsupportedRequestIdentityRejectsSafely),
                ("other star count preserves fixed and declines quantitative", OtherStarCountIsBounded),
                ("peace preview omits Dark Fog status", PeacePreviewOmitsDarkFogStatus),
                ("combat preview exposes neutral Dark Fog facts", CombatPreviewExposesNeutralDarkFogFacts),
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
                ("incremental cluster polls terrain workers without changing results", IncrementalClusterMatchesSynchronousExecution),
                ("incremental cluster cancellation and failure restore state", IncrementalClusterExitPathsRestoreState),
                ("incremental cluster keeps serialization between yields", IncrementalClusterKeepsSerializationBetweenYields),
                ("complete cache keys cover the audited reusable identity", CompleteCacheKeysCoverReusableIdentity),
                ("complete cache reuses only audited payload across mode", CompleteCacheReusesOnlyAuditedPayloadAcrossMode),
                ("complete cache round trips and replaces atomically", CompleteCacheRoundTripsAndReplacesAtomically),
                ("complete cache bounds retention and clears manually", CompleteCacheBoundsRetentionAndClears),
                ("complete cache rejects unsafe and obsolete entries", CompleteCacheRejectsUnsafeEntries),
                ("complete cache disables and contains filesystem failures", CompleteCacheContainsFilesystemFailures),
                ("completed keyboard paste and random loads create one session each", CompletedInputLoadsCreateOneSessionEach),
                ("duplicate callbacks coalesce while same identity reloads", DuplicateCallbacksCoalesceAndReloadsReplace),
                ("replacement rejects stale publication and late loads", ReplacementRejectsStalePublication),
                ("preview exit retires once and blocks resurrection", PreviewExitRetiresAndBlocksResurrection),
                ("automatic resolution uses cache once per completed load", AutomaticResolutionUsesCacheOncePerLoad),
                ("automatic resolution reuses completed payload across mode", AutomaticResolutionReusesCompletedPayloadAcrossMode),
                ("automatic resolution cancels replacement and exit", AutomaticResolutionCancelsReplacementAndExit),
                ("automatic resolution terminal failures never retry", AutomaticResolutionFailuresNeverRetry),
                ("panel maps every operational state within text bounds", PanelMapsEveryOperationalState),
                ("panel corners map clockwise and avoid border centers", PanelCornersMapClockwise),
                ("statistics panel mirrors the authoritative conclusion layout", StatisticsPanelMirrorsConclusionLayout),
                ("home system body inventory is immutable complete and ordered", HomeSystemBodyInventoryIsImmutableCompleteAndOrdered),
                ("home system statistics show layout and exact energy facts", HomeSystemStatisticsShowLayoutAndExactEnergyFacts),
                ("home system resources join rows only when complete", HomeSystemResourcesJoinRowsOnlyWhenComplete),
                ("cluster statistics are keyed ordered and sectioned", ClusterStatisticsAreKeyedOrderedAndSectioned),
                ("cluster locations format AU and preserve stable ties", ClusterLocationsFormatAuAndPreserveStableTies),
                ("statistics panel follows preview lifecycle independently", StatisticsPanelFollowsPreviewLifecycleIndependently),
                ("home planet designation is shared immutable and session owned", HomePlanetDesignationIsSharedImmutableAndSessionOwned),
                ("panel rejects obsolete sessions and hides exactly", PanelRejectsObsoleteSessions),
                ("conclusion cards map every outcome and subject kind", ConclusionCardsMapEveryOutcomeAndSubject),
                ("fresh start copy is natural bounded and attributed", FreshStartCopyIsNaturalBoundedAndAttributed),
                ("tidal lock copy is bounded and literal", TidalLockCopyIsBoundedAndLiteral),
                ("fresh start omits unavailable attribution", FreshStartOmitsUnavailableAttribution),
                ("fresh start resources group by metric and outcome", FreshStartResourcesGroupByMetricAndOutcome),
                ("Megafactory copy is natural bounded and grouped", MegafactoryCopyIsNaturalBoundedAndGrouped),
                ("Compact routes are natural deduplicated and bounded", CompactRoutesAreNaturalDeduplicatedAndBounded),
                ("Sphere candidates are natural deterministic and bounded", SphereCandidatesAreNaturalDeterministicAndBounded),
                ("conclusion panel separates contexts stages and conflicts", ConclusionPanelSeparatesContextsAndConflicts),
                ("conclusion panel snapshot stays bounded and neutral", ConclusionPanelSnapshotIsBoundedAndNeutral),
                ("refined release candidate is coherent across scan and cache", RefinedReleaseCandidateIsCoherentAcrossScanAndCache),
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

        private static void HomeTopologyVerifiesOnlyHomePlanetParent()
        {
            const string home = "home";
            const string other = "other";
            RuntimePlanetOrbitEvidence Direct(int id = 101) =>
                Orbit(id, home, 1, true, false, 0, null);
            RuntimePlanetOrbitEvidence Giant(
                int id = 200,
                string system = home,
                int number = 2) => Orbit(id, system, number, false, true, 0, null);
            RuntimePlanetOrbitEvidence Moon(
                int id,
                int parentId = 200,
                int orbitAround = 2,
                string system = home) =>
                Orbit(id, system, id, true, false, orbitAround, parentId);

            Equal(
                HomePlanetOrbitKind.DirectStar,
                PreviewHomeTopologyNormalizer.Normalize(home, 101, new[]
                {
                    Direct(),
                    Giant(),
                    Giant(300, home, 3),
                    Moon(301, 300, 3),
                    Orbit(102, home, 4, true, false, 0, null)
                })?.OrbitKind);

            for (int count = 1; count <= 3; count++)
            {
                var planets = new List<RuntimePlanetOrbitEvidence> { Giant(), Moon(101) };
                for (int index = 1; index < count; index++)
                    planets.Add(Moon(101 + index));
                planets.Add(Giant(300, home, 3));
                planets.Add(Moon(301, 300, 3));
                planets.Add(Orbit(400, home, 4, true, false, 0, null));
                NormalizedHomePlanetTopology? topology =
                    PreviewHomeTopologyNormalizer.Normalize(home, 101, planets);
                Equal(HomePlanetOrbitKind.GiantMoon, topology?.OrbitKind);
                Equal(count, topology?.HomeGiantMoonCount);
            }

            Equal(HomePlanetOrbitKind.DirectStar,
                PreviewHomeTopologyNormalizer.Normalize(home, 101,
                    new[] { Direct(), Giant() })?.OrbitKind);
            True(PreviewHomeTopologyNormalizer.Normalize(home, 101,
                new[] { Orbit(101, other, 1, true, false, 0, null) }) == null);
            True(PreviewHomeTopologyNormalizer.Normalize(home, 101,
                new[] { Moon(101, 999) }) == null);
            True(PreviewHomeTopologyNormalizer.Normalize(home, 101,
                new[] { Moon(101, 202), Orbit(202, home, 2, true, false, 0, null) }) == null);
            True(PreviewHomeTopologyNormalizer.Normalize(home, 101,
                new[] { Moon(101), Giant(200, other) }) == null);
            True(PreviewHomeTopologyNormalizer.Normalize(home, 101,
                new[] { Moon(101, 200, 3), Giant() }) == null);
            True(PreviewHomeTopologyNormalizer.Normalize(home, 101,
                new[] { Orbit(101, home, 1, true, false, -1, null) }) == null);
            True(PreviewHomeTopologyNormalizer.Normalize(home, 101,
                new[] { Moon(101), Giant(200, home, 0) }) == null);

            RuntimeScanResult isolated = new PreviewScanCoordinator(new FakeGateway
            {
                Snapshot = Snapshot(includeHomePlanetTopology: false)
            }).TryScan(Request(), CancellationToken.None);
            Equal(ComponentOutcome.Unknown, FindReport(
                isolated,
                SharedSatelliteEvaluator.ConclusionId).Outcome);
            Equal(ComponentOutcome.Supports, FindReport(
                isolated,
                "FS-POWER.solar").Outcome);
        }

        private static void SystemCandidatesAreBoundedDeterministicAndOwned()
        {
            var facts = new Dictionary<int, (decimal Energy, long Radius, int Orbits)>
            {
                [2] = (10m, 1_000_000, 5),
                [3] = (9m, 3_000_000, 4),
                [4] = (8m, 2_000_000, 7),
                [5] = (7m, 4_000_000, 6),
                [6] = (7m, 4_000_000, 6)
            };
            var gateway = new FakeGateway
            {
                Snapshot = Snapshot(systemCandidateFacts: facts)
            };

            RuntimeScanResult result = new PreviewScanCoordinator(gateway)
                .TryScan(Request(), CancellationToken.None);
            RuntimeSystemCandidates candidates = result.SystemCandidates!;

            Equal(RuntimeScanStatus.Success, result.Status);
            Equal("2,3,4", CandidateIds(candidates.Energy));
            Equal("5,6,3", CandidateIds(candidates.ShellRadius));
            Equal("4,5,6", CandidateIds(candidates.ContainedOrbits));
            Equal(3, candidates.Energy!.Count);
            Equal(5, candidates.EnergySupportingCount);
            Equal(5, candidates.ShellRadiusSupportingCount);
            Equal(5, candidates.ContainedOrbitsSupportingCount);
            Equal("Star 2", candidates.Energy[0].DisplayName);
            Equal(10m, candidates.Energy[0].DecisiveValue);
            Equal(3_000_000m, candidates.ShellRadius!
                .Single(value => value.Identifier == "3").DecisiveValue);
            Equal(6m, candidates.ContainedOrbits!
                .Single(value => value.Identifier == "5").DecisiveValue);
        }

        private static void IncompleteSystemCandidateEvidenceStaysUnknown()
        {
            var gateway = new FakeGateway
            {
                Snapshot = Snapshot(missingEnergySystem: 7)
            };
            RuntimeScanResult result = new PreviewScanCoordinator(gateway)
                .TryScan(Request(), CancellationToken.None);

            Equal(RuntimeScanStatus.Success, result.Status);
            True(result.SystemCandidates != null);
            True(result.SystemCandidates!.Energy == null);
            Equal(0, result.SystemCandidates.EnergySupportingCount);
            Equal(3, result.SystemCandidates.ShellRadius!.Count);
            Equal(3, result.SystemCandidates.ContainedOrbits!.Count);

            WithTemporaryDirectory(path =>
            {
                var lifecycle = new PreviewSessionLifecycle();
                using var resolver = new PreviewResolutionCoordinator(
                    lifecycle,
                    new PreviewScanCoordinator(gateway),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt attempt = resolver.CurrentPublishedAttempt!;
                True(attempt.SystemCandidates != null);
                True(attempt.SystemCandidates!.Energy == null);
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
            Equal(40, result.DarkFogOccupation?.ClusterInitialHiveCount);
            Equal(1, result.DarkFogOccupation?.BirthSystemInitialHiveCount);
            False(result.Reports.Any(report =>
                report.ConclusionId.StartsWith("DF-", StringComparison.Ordinal)));
            AssertReport(result, "CX-GROUPING.distance", ComponentOutcome.Supports);
            True(result.Reports.Any(report => report.ConclusionId ==
                "MF-SYSTEM-ROLE.role:strong-energy"));
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

        private static void UnsupportedGameIdentityRejectsSafely()
        {
            AssertRejected(Fingerprint(gameVersion: "0.10.34.0"), "game-version-mismatch");
            True(CompatibilityPolicy.Evaluate(Fingerprint(assembly: "MODIFIED")).Supported);
        }

        private static void MissingMembersRejectWhilePluginsCoexist()
        {
            AssertRejected(Fingerprint(members: false, missing: "UniverseGen.CreateGalaxy"), "missing-runtime-member");
            True(CompatibilityPolicy.Evaluate(
                Fingerprint(mods: new[] { "example.unrelated.plugin" })).Supported);
        }

        private static void GenerationChangesCoexistAndRemainIdentified()
        {
            True(CompatibilityPolicy.Evaluate(
                Fingerprint(patchers: new[] { "example-preloader.dll:ABC" })).Supported);
            True(CompatibilityPolicy.Evaluate(Fingerprint(methodIl: "MODIFIED")).Supported);
            True(CompatibilityPolicy.Evaluate(Fingerprint(algorithm: 1)).Supported);
            True(CompatibilityPolicy.Evaluate(Fingerprint(
                themes: ConclusionDefinition.ReferenceOrderedThemeIds.Split(',').Reverse())).Supported);
        }

        private static void RuntimeFileFingerprintsFallBackWithoutThrowing()
        {
            WithTemporaryDirectory(path =>
            {
                string primary = Path.Combine(path, "missing.dll");
                string fallback = Path.Combine(path, "Assembly-CSharp.dll");
                byte[] content = { 1, 2, 3, 4, 5 };
                File.WriteAllBytes(fallback, content);
                string expected;
                using (SHA256 hash = SHA256.Create())
                {
                    expected = BitConverter.ToString(hash.ComputeHash(content))
                        .Replace("-", String.Empty);
                }

                Equal(expected, RuntimeFileFingerprint.FirstReadableSha256(
                    new string?[] { String.Empty, primary, fallback }));

                byte[] primaryContent = { 9, 8, 7 };
                File.WriteAllBytes(primary, primaryContent);
                string primaryExpected;
                using (SHA256 hash = SHA256.Create())
                {
                    primaryExpected = BitConverter.ToString(
                        hash.ComputeHash(primaryContent)).Replace("-", String.Empty);
                }
                Equal(primaryExpected, RuntimeFileFingerprint.FirstReadableSha256(
                    new string?[] { primary, fallback }));

                File.Delete(primary);
                Equal(RuntimeFileFingerprint.Unavailable,
                    RuntimeFileFingerprint.FirstReadableSha256(
                        new string?[] { null, String.Empty, primary }));

                False(RuntimeFileFingerprint.TrySha256(
                    "disappeared.dll",
                    _ => throw new FileNotFoundException("removed during capture"),
                    out string disappeared));
                Equal(RuntimeFileFingerprint.Unavailable, disappeared);
                False(RuntimeFileFingerprint.TrySha256(
                    "denied.dll",
                    _ => throw new UnauthorizedAccessException("access denied"),
                    out string denied));
                Equal(RuntimeFileFingerprint.Unavailable, denied);

                bool requiredRejected = false;
                try
                {
                    RuntimeFileFingerprint.RequiredSha256(primary, "active-managed-assembly");
                }
                catch (RuntimeFilesystemException exception)
                {
                    requiredRejected = true;
                    Equal("required-file-unavailable", exception.Code);
                    Equal("active-managed-assembly", exception.FilesystemSource);
                    True(exception.Diagnostic.Contains("hash-file", StringComparison.Ordinal));
                    False(exception.Diagnostic.Contains(" at ", StringComparison.Ordinal));
                    True(exception.Diagnostic.Length < 256);
                }
                True(requiredRejected);

                using (new FileStream(
                    fallback,
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.None))
                {
                    bool lockedRejected = false;
                    try
                    {
                        RuntimeFileFingerprint.RequiredSha256(
                            fallback,
                            "active-managed-assembly");
                    }
                    catch (RuntimeFilesystemException)
                    {
                        lockedRejected = true;
                    }
                    True(lockedRejected);
                }

                string patcher = Path.Combine(path, "locked-patcher.dll");
                File.WriteAllBytes(patcher, content);
                using (new FileStream(
                    patcher,
                    FileMode.Open,
                    FileAccess.ReadWrite,
                    FileShare.None))
                {
                    IReadOnlyList<string> inventory =
                        RuntimeFileFingerprint.Inventory(path, "*.dll");
                    True(inventory.Contains("locked-patcher.dll:unavailable"));
                }

                var inventoryDiagnostics = new List<string>();
                Equal(
                    "inventory:unavailable",
                    String.Join(",", RuntimeFileFingerprint.Inventory(
                        Path.Combine(path, "missing-patchers"),
                        "*.dll",
                        inventoryDiagnostics.Add,
                        "active-patchers")));
                Equal(1, inventoryDiagnostics.Count);
                True(inventoryDiagnostics[0].StartsWith(
                    "inventory-directory:active-patchers:",
                    StringComparison.Ordinal));
                False(inventoryDiagnostics[0].Contains(" at ", StringComparison.Ordinal));
            });
        }

        private static void RuntimeFilesystemContextFollowsActiveProcess()
        {
            WithTemporaryDirectory(path =>
            {
                RuntimeFilesystemFixture fixture = CreateRuntimeFilesystem(path, "active");
                RuntimeFilesystemResolution resolution = RuntimeFilesystemContextResolver.Resolve(
                    new RuntimeFilesystemInputs(
                        fixture.ExecutablePath,
                        fixture.GameRootPath,
                        fixture.PluginAssemblyPath,
                        fixture.ManagedAssemblyPath,
                        fixture.PatcherDirectoryPath,
                        fixture.ConfigurationDirectoryPath));

                True(resolution.Succeeded);
                RuntimeFilesystemContext context = resolution.Context!;
                Equal(Path.GetFullPath(fixture.GameRootPath), context.GameRootPath);
                Equal(Path.GetFullPath(fixture.ManagedAssemblyPath), context.ManagedAssemblyPath);
                Equal(Path.GetFullPath(fixture.PatcherDirectoryPath), context.PatcherDirectoryPath);
                Equal(Path.GetFullPath(fixture.ConfigurationDirectoryPath), context.ConfigurationDirectoryPath);
                Equal(
                    Path.Combine(fixture.ConfigurationDirectoryPath, "DSPSeedScanner", "cache"),
                    context.CacheDirectoryPath);
                Equal("process-executable", context.Provenance);

                RuntimeFilesystemResolution fallback = RuntimeFilesystemContextResolver.Resolve(
                    new RuntimeFilesystemInputs(
                        null,
                        fixture.GameRootPath,
                        fixture.PluginAssemblyPath,
                        String.Empty,
                        fixture.PatcherDirectoryPath,
                        fixture.ConfigurationDirectoryPath));
                True(fallback.Succeeded);
                Equal("bepinex-game-root", fallback.Context?.Provenance);
                Equal(context.GameRootPath, fallback.Context?.GameRootPath);

                RuntimeFilesystemResolution canonical = RuntimeFilesystemContextResolver.Resolve(
                    new RuntimeFilesystemInputs(
                        fixture.ExecutablePath,
                        fixture.GameRootPath,
                        fixture.PluginAssemblyPath,
                        String.Empty,
                        String.Empty,
                        String.Empty));
                True(canonical.Succeeded);
                Equal(fixture.PatcherDirectoryPath, canonical.Context?.PatcherDirectoryPath);
                Equal(
                    fixture.ConfigurationDirectoryPath,
                    canonical.Context?.ConfigurationDirectoryPath);
            });
        }

        private static void RuntimeFilesystemContextFailsClosedOnConflicts()
        {
            WithTemporaryDirectory(path =>
            {
                RuntimeFilesystemFixture active = CreateRuntimeFilesystem(path, "active");
                RuntimeFilesystemFixture other = CreateRuntimeFilesystem(path, "other");

                RuntimeFilesystemResolution rootConflict = RuntimeFilesystemContextResolver.Resolve(
                    new RuntimeFilesystemInputs(
                        active.ExecutablePath,
                        other.GameRootPath,
                        active.PluginAssemblyPath,
                        active.ManagedAssemblyPath,
                        active.PatcherDirectoryPath,
                        active.ConfigurationDirectoryPath));
                False(rootConflict.Succeeded);
                Equal("active-game-root-conflict", rootConflict.Code);

                RuntimeFilesystemResolution assemblyConflict = RuntimeFilesystemContextResolver.Resolve(
                    new RuntimeFilesystemInputs(
                        active.ExecutablePath,
                        active.GameRootPath,
                        active.PluginAssemblyPath,
                        other.ManagedAssemblyPath,
                        active.PatcherDirectoryPath,
                        active.ConfigurationDirectoryPath));
                False(assemblyConflict.Succeeded);
                Equal("target-assembly-path-conflict", assemblyConflict.Code);

                File.Delete(active.ManagedAssemblyPath);
                RuntimeFilesystemResolution missing = RuntimeFilesystemContextResolver.Resolve(
                    new RuntimeFilesystemInputs(
                        active.ExecutablePath,
                        active.GameRootPath,
                        active.PluginAssemblyPath,
                        null,
                        active.PatcherDirectoryPath,
                        active.ConfigurationDirectoryPath));
                False(missing.Succeeded);
                Equal("managed-assembly-missing", missing.Code);

                RuntimeFilesystemResolution malformed = RuntimeFilesystemContextResolver.Resolve(
                    new RuntimeFilesystemInputs(
                        "\0invalid",
                        null,
                        null,
                        null,
                        null,
                        null));
                False(malformed.Succeeded);
                True(malformed.Diagnostic.Length < 256);
                False(malformed.Diagnostic.Contains(" at ", StringComparison.Ordinal));
            });
        }

        private static void RuntimeFilesystemOptionalPathsDegradeIndependently()
        {
            WithTemporaryDirectory(path =>
            {
                RuntimeFilesystemFixture active = CreateRuntimeFilesystem(path, "active");
                RuntimeFilesystemFixture other = CreateRuntimeFilesystem(path, "other");
                string missingPatcher = Path.Combine(active.GameRootPath, "BepInEx", "missing-patchers");
                RuntimeFilesystemResolution resolution = RuntimeFilesystemContextResolver.Resolve(
                    new RuntimeFilesystemInputs(
                        active.ExecutablePath,
                        active.GameRootPath,
                        other.PluginAssemblyPath,
                        String.Empty,
                        missingPatcher,
                        other.ConfigurationDirectoryPath));

                True(resolution.Succeeded);
                True(resolution.Context?.PatcherDirectoryPath == null);
                True(resolution.Context?.ConfigurationDirectoryPath == null);
                True(resolution.Context?.CacheDirectoryPath == null);
                True(resolution.Context?.PatcherDiagnostic != null);
                True(resolution.Context?.ConfigurationDiagnostic != null);
                True(resolution.Context?.PluginDiagnostic != null);
                Equal(active.GameRootPath, resolution.Context?.GameRootPath);
            });
        }

        private static void RuntimeFilesystemGuardsContainExpectedFailures()
        {
            string? diagnostic = null;
            int result = RuntimeFilesystemGuard.ExecuteOrFallback(
                () => throw new IOException("setting file is locked\nstack-shaped detail"),
                1,
                "bind-setting",
                "active-config",
                value => diagnostic = value);
            Equal(1, result);
            True(diagnostic != null);
            True(diagnostic!.StartsWith("bind-setting:active-config:", StringComparison.Ordinal));
            False(diagnostic.Contains('\n'));
            False(diagnostic.Contains(" at ", StringComparison.Ordinal));

            bool escaped = false;
            try
            {
                RuntimeFilesystemGuard.ExecuteOrFallback<int>(
                    () => throw new InvalidOperationException("programming failure"),
                    1,
                    "bind-setting",
                    "active-config",
                    null);
            }
            catch (InvalidOperationException)
            {
                escaped = true;
            }
            True(escaped);
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

        private static void PeacePreviewOmitsDarkFogStatus()
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
            True(result.DarkFogOccupation == null);
            False(result.Reports.Any(report =>
                report.ConclusionId.StartsWith("DF-", StringComparison.Ordinal)));
        }

        private static void CombatPreviewExposesNeutralDarkFogFacts()
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
            Equal(40, result.DarkFogOccupation?.ClusterInitialHiveCount);
            Equal(1, result.DarkFogOccupation?.BirthSystemInitialHiveCount);
            False(result.Reports.Any(report =>
                report.ConclusionId.StartsWith("DF-", StringComparison.Ordinal) ||
                report.ConclusionId.Contains("fog-opportunity", StringComparison.Ordinal)));

            RuntimeScanResult incomplete = new PreviewScanCoordinator(new FakeGateway
            {
                Snapshot = Snapshot(missingHiveSystem: 7)
            }).TryScan(request, CancellationToken.None);
            Equal(RuntimeScanStatus.Success, incomplete.Status);
            True(incomplete.DarkFogOccupation == null);

            WithTemporaryDirectory(path =>
            {
                var gateway = new FakeGateway
                {
                    Snapshot = Snapshot(birthInitialHiveCount: 1, otherInitialHiveCount: 0)
                };
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(gateway),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(
                    1,
                    PreviewIdentity(16_315_224, initialColonize: 2m),
                    request);
                Equal(
                    "Dark Fog: 1 initial hive; 1 in starter system",
                    PreviewConclusionPresenter.Project(
                        resolver.CurrentPublishedAttempt!).DarkFogStatusLine);
            });
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
            True(result.DarkFogOccupation == null);
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
            HomeSystemResource homeIron = result.HomeSystemResources!.ForBody(101)!
                .Resources.Single(resource => resource.ResourceId == "iron");
            Equal(10_000L, homeIron.Amount);
            Equal(1, homeIron.VeinGroups);
            Equal(RawResourceSemantics.FiniteDeposit, homeIron.Semantics);

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
                True(cancelled.HomeSystemResources == null);
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
            True(failed.HomeSystemResources == null);
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
                Equal(
                    advances % 3 == 0 ? before + 1 : before,
                    operation.CompletedPlanets);
                Equal(
                    operation.State == CompleteClusterRawOperationState.Ready
                        ? "leased"
                        : "original",
                    gateway.StateMarker);
            }

            CompleteClusterRawResult incremental = operation.Result!;
            Equal(
                9,
                advances);
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
            Equal(0, incremental.Trace.Count(value =>
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
                Equal(CoverageState.Unavailable, cancelled.Coverage.State);
                Equal(0, cancelled.Coverage.CompletedPlanets);
                Equal(0, cancelled.Reports.Count);
                True(cancelled.StateRestored);
                Equal(0, cancellationGateway.YieldRestoreChecks);
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
                Equal(CompleteClusterRawOperationState.Ready, operation.State);
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
            while (operation.State == CompleteClusterRawOperationState.Ready)
            {
                operation.Advance();
                if (operation.State == CompleteClusterRawOperationState.Ready)
                {
                    Equal(RuntimeScanStatus.Busy,
                        preview.TryScan(Request(), CancellationToken.None).Status);
                }
            }
            Equal(RuntimeScanStatus.Success, operation.Result?.Status);
            Equal(RuntimeScanStatus.Success,
                preview.TryScan(Request(), CancellationToken.None).Status);
            Equal(1, previewGateway.GenerateCalls);
        }

        private static void CompleteCacheKeysCoverReusableIdentity()
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
                PreviewIdentity(16_315_224, combatMode: CombatMode.Peace),
                Fingerprint(),
                out CompleteClusterCacheKey? peace));
            False(PreviewIdentity(16_315_224).Equals(
                PreviewIdentity(16_315_224, combatMode: CombatMode.Peace)));
            Equal(first, peace);
            Equal(first?.Hash, peace?.Hash);
            False(first!.CanonicalValue.Contains("combat-mode", StringComparison.Ordinal));
            True(first.CanonicalValue.Contains("conclusion-contract", StringComparison.Ordinal));

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

            True(CompleteClusterCacheKey.TryCreate(
                PreviewIdentity(16_315_224),
                Fingerprint(methodIl: "obsolete"),
                out CompleteClusterCacheKey? changedMethod));
            False(first!.Equals(changedMethod));
            False(String.Equals(first.Hash, changedMethod?.Hash, StringComparison.Ordinal));
            True(CompleteClusterCacheKey.TryCreate(
                PreviewIdentity(16_315_224),
                Fingerprint(mods: new[] { "generation-mod" }),
                out CompleteClusterCacheKey? modded));
            False(first!.Equals(modded));
            False(String.Equals(first.Hash, modded?.Hash, StringComparison.Ordinal));

            True(CompleteClusterCacheKey.TryCreate(
                PreviewIdentity(16_315_224),
                Fingerprint(patchers: new[] { "example-preloader.dll:ABC" }),
                out CompleteClusterCacheKey? patched));
            False(first.Equals(patched));
            False(String.Equals(first.Hash, patched?.Hash, StringComparison.Ordinal));

            True(CompleteClusterCacheKey.TryCreate(
                PreviewIdentity(16_315_224, initialColonize: 0.5m),
                Fingerprint(),
                out CompleteClusterCacheKey? changedCombatValues));
            False(first.Equals(changedCombatValues));
            True(CompleteClusterCacheKey.TryCreate(
                PreviewIdentity(16_315_224, maxDensity: 0.5m),
                Fingerprint(),
                out CompleteClusterCacheKey? changedDensity));
            False(first.Equals(changedDensity));
        }

        private static void CompleteCacheReusesOnlyAuditedPayloadAcrossMode()
        {
            WithTemporaryDirectory(path =>
            {
                var cache = new CompleteClusterConclusionCache(path);
                PreviewGenerationIdentity combat = PreviewIdentity(16_315_224);
                PreviewGenerationIdentity peace = PreviewIdentity(
                    16_315_224,
                    combatMode: CombatMode.Peace);
                CompleteClusterRawResult source = CompleteResult();
                ConclusionReport template = source.Reports.First(report =>
                    report.Stage == EvidenceStage.CompleteClusterRaw);
                var unaudited = new ConclusionReport(
                    template.Identity,
                    template.Settings,
                    template.Coverage,
                    "FUTURE-UNAUDITED.fact",
                    template.Context,
                    template.ContractVersion,
                    template.DefinitionVersion,
                    template.Subject,
                    template.Outcome,
                    template.DecisiveFact,
                    template.DiagnosticCause,
                    template.SourceConclusionId);
                var expanded = new CompleteClusterRawResult(
                    source.Status,
                    source.GalaxySeed,
                    source.Code,
                    source.Message,
                    source.Fingerprint,
                    source.Coverage,
                    source.Progress,
                    source.RareResources,
                    source.Reports.Concat(new[] { unaudited }).ToArray(),
                    source.Trace,
                    source.StateRestored,
                    source.ElapsedMilliseconds,
                    source.ManagedMemoryDeltaBytes,
                    homeSystemResources: source.HomeSystemResources);

                True(cache.TryStore(combat, expanded));
                Equal(1, Directory.GetFiles(path, "*.dspseedscan").Length);
                True(cache.TryRead(peace, Fingerprint(), out CachedCompleteClusterConclusions? hit));
                Equal(combat, hit?.Identity);
                False(hit!.Identity.Equals(peace));
                True(hit.Reports.All(report => report.Settings.CombatMode == CombatMode.Combat));
                False(hit.Reports.Any(report => report.ConclusionId == "FUTURE-UNAUDITED.fact"));

                False(cache.TryRead(
                    PreviewIdentity(16_315_224, 0.5m, CombatMode.Peace),
                    Fingerprint(),
                    out _));
                False(cache.TryRead(
                    PreviewIdentity(16_315_224, combatMode: CombatMode.Peace,
                        initialColonize: 0.5m),
                    Fingerprint(),
                    out _));
                False(cache.TryRead(
                    PreviewIdentity(73_339_583, combatMode: CombatMode.Peace),
                    Fingerprint(),
                    out _));
                var differentStarCount = new PreviewGenerationIdentity(
                    new GenerationIdentity(
                        peace.GalaxyIdentity.GameVersion,
                        peace.GalaxyIdentity.GalaxyAlgorithm,
                        peace.GalaxyIdentity.AssemblySha256,
                        peace.GalaxyIdentity.OrderedThemeIds,
                        peace.GalaxyIdentity.ScannerCompatibilityVersion,
                        peace.GalaxyIdentity.GalaxySeed,
                        32,
                        peace.GalaxyIdentity.CreationVersion),
                    peace.ResourceMultiplier,
                    peace.CombatMode,
                    peace.CombatSettingsKey,
                    peace.InitialColonize,
                    peace.MaxDensity);
                False(cache.TryRead(differentStarCount, Fingerprint(), out _));
                False(cache.TryRead(peace, Fingerprint(methodIl: "changed"), out _));
                False(cache.TryRead(peace, Fingerprint(mods: new[] { "generation-mod" }), out _));
                False(cache.TryRead(peace, Fingerprint(
                    scannerCompatibility: "changed"), out _));
                False(cache.TryRead(peace, Fingerprint(
                    scannerContract: "changed"), out _));
            });
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
                    .Where(report => (report.Stage == EvidenceStage.BirthSystemRaw &&
                            report.Context == ConclusionContext.FreshStart) ||
                        report.Stage == EvidenceStage.CompleteClusterRaw)
                    .ToArray();
                Equal(identity, restored?.Identity);
                Equal(key.Hash, restored?.CacheKeyHash);
                Equal(source.Coverage, restored?.Coverage);
                True(expected.SequenceEqual(restored!.Reports));
                Equal(
                    String.Join(",", source.HomeSystemResources!.Bodies.Select(body =>
                        body.BodyId + ":" + String.Join("+", body.Resources.Select(resource =>
                            resource.ResourceId + ":" + resource.Semantics + ":" +
                            resource.Amount + ":" + resource.VeinGroups)))),
                    String.Join(",", restored.HomeSystemResources.Bodies.Select(body =>
                        body.BodyId + ":" + String.Join("+", body.Resources.Select(resource =>
                            resource.ResourceId + ":" + resource.Semantics + ":" +
                            resource.Amount + ":" + resource.VeinGroups)))));
                True(restored.Reports.All(report =>
                    (report.Stage == EvidenceStage.BirthSystemRaw &&
                        report.Context == ConclusionContext.FreshStart) ||
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
                    source.ManagedMemoryDeltaBytes,
                    homeSystemResources: source.HomeSystemResources);
                True(expanded.Reports.Count > 1_024);
                True(cache.TryStore(identity, expanded));
                True(cache.TryRead(identity, fingerprint, out restored));
                Equal(expected.Length, restored?.Reports.Count);
                False(typeof(CachedCompleteClusterConclusions).GetProperties()
                    .Any(property => property.Name == "RareResources" ||
                        property.Name == "Progress" || property.Name == "Trace" ||
                        property.Name == "ElapsedMilliseconds" ||
                        property.Name == "ManagedMemoryDeltaBytes" ||
                        property.Name == "BirthPlanetAttributions" ||
                        property.Name == "SystemCandidates"));
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
                    "unsupported fixture",
                    Fingerprint(gameVersion: "unsupported"),
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
                    writer.Write(7);
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

        private static void CompleteCacheContainsFilesystemFailures()
        {
            var diagnostics = new List<string>();
            CompleteClusterConclusionCache disabled =
                CompleteClusterConclusionCache.CreateOrDisabled(null, diagnostics.Add);
            False(disabled.Available);
            False(disabled.TryRead(PreviewIdentity(16_315_224), Fingerprint(), out _));
            False(disabled.TryStore(PreviewIdentity(16_315_224), CompleteResult()));
            False(disabled.Clear());
            Equal(1, diagnostics.Count);
            True(diagnostics[0].StartsWith(
                "initialize-cache:active-config:",
                StringComparison.Ordinal));

            WithTemporaryDirectory(path =>
            {
                string occupied = Path.Combine(path, "occupied");
                File.WriteAllText(occupied, "not a directory");
                diagnostics.Clear();
                CompleteClusterConclusionCache cache =
                    CompleteClusterConclusionCache.CreateOrDisabled(
                        occupied,
                        diagnostics.Add);
                True(cache.Available);
                False(cache.TryRead(PreviewIdentity(16_315_224), Fingerprint(), out _));
                False(cache.TryStore(PreviewIdentity(16_315_224), CompleteResult()));
                True(cache.Clear());
                True(diagnostics.Any(value => value.StartsWith(
                    "write-cache:active-config:",
                    StringComparison.Ordinal)));

                string cacheDirectory = Path.Combine(path, "cache");
                diagnostics.Clear();
                var clearCache = new CompleteClusterConclusionCache(
                    cacheDirectory,
                    reportDiagnostic: diagnostics.Add);
                True(clearCache.TryStore(
                    PreviewIdentity(16_315_224),
                    CompleteResult()));
                string entry = Directory.GetFiles(cacheDirectory, "*.dspseedscan").Single();
                using (new FileStream(
                    entry,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.None))
                {
                    False(clearCache.Clear());
                }
                True(diagnostics.Any(value => value.StartsWith(
                    "clear-cache:active-config:",
                    StringComparison.Ordinal)));
                True(diagnostics.All(value => !value.Contains(" at ", StringComparison.Ordinal)));

                foreach (string operation in new[]
                {
                    "write", "replace", "touch", "trim"
                })
                {
                    diagnostics.Clear();
                    var injected = new CompleteClusterConclusionCache(
                        Path.Combine(path, "inject-" + operation),
                        maximumEntries: 1,
                        reportDiagnostic: diagnostics.Add,
                        beforeFileOperation: current =>
                        {
                            if (current == operation)
                                throw new IOException("Injected " + operation + " failure.");
                        });
                    False(injected.TryStore(
                        PreviewIdentity(16_315_224),
                        CompleteResult()));
                    Equal(
                        0,
                        Directory.Exists(injected.DirectoryPath)
                            ? Directory.GetFiles(
                                injected.DirectoryPath,
                                "*.dspseedscan").Length
                            : 0);
                    True(diagnostics.Any(value => value.StartsWith(
                        "write-cache:active-config:",
                        StringComparison.Ordinal)));
                }

                string readPath = Path.Combine(path, "inject-read");
                var prepared = new CompleteClusterConclusionCache(readPath);
                PreviewGenerationIdentity identity = PreviewIdentity(16_315_224);
                True(prepared.TryStore(identity, CompleteResult()));
                diagnostics.Clear();
                var readFailure = new CompleteClusterConclusionCache(
                    readPath,
                    maximumEntries: 1,
                    reportDiagnostic: diagnostics.Add,
                    beforeFileOperation: operation =>
                    {
                        if (operation == "read")
                            throw new IOException("Injected read failure.");
                    });
                False(readFailure.TryRead(identity, Fingerprint(), out _));
                True(diagnostics.Any(value => value.StartsWith(
                    "read-cache:active-config:",
                    StringComparison.Ordinal)));

                string deletePath = Path.Combine(path, "inject-delete");
                prepared = new CompleteClusterConclusionCache(deletePath);
                True(prepared.TryStore(identity, CompleteResult()));
                string deleteEntry = Directory.GetFiles(deletePath, "*.dspseedscan").Single();
                byte[] corrupt = File.ReadAllBytes(deleteEntry);
                corrupt[10] ^= 0x01;
                File.WriteAllBytes(deleteEntry, corrupt);
                diagnostics.Clear();
                var deleteFailure = new CompleteClusterConclusionCache(
                    deletePath,
                    maximumEntries: 1,
                    reportDiagnostic: diagnostics.Add,
                    beforeFileOperation: operation =>
                    {
                        if (operation == "delete")
                            throw new IOException("Injected delete failure.");
                    });
                False(deleteFailure.TryRead(identity, Fingerprint(), out _));
                True(File.Exists(deleteEntry));
                True(diagnostics.Any(value => value.StartsWith(
                    "delete-cache:active-config:",
                    StringComparison.Ordinal)));

                diagnostics.Clear();
                var clearFailure = new CompleteClusterConclusionCache(
                    deletePath,
                    maximumEntries: 1,
                    reportDiagnostic: diagnostics.Add,
                    beforeFileOperation: operation =>
                    {
                        if (operation == "clear")
                            throw new IOException("Injected clear failure.");
                    });
                False(clearFailure.Clear());
                True(diagnostics.Any(value => value.StartsWith(
                    "clear-cache:active-config:",
                    StringComparison.Ordinal)));
                True(diagnostics.All(value => !value.Contains(" at ", StringComparison.Ordinal)));
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
                True(cached.SystemCandidates?.Energy?.Count == 3);
                Equal(scanned.CompleteReports.Count, cached.CompleteReports.Count);
                True(cached.CompleteReports.Select(report =>
                    report.ConclusionId + "\t" + report.Outcome).SequenceEqual(
                        scanned.CompleteReports.Select(report =>
                            report.ConclusionId + "\t" + report.Outcome)));
            });
        }

        private static void AutomaticResolutionReusesCompletedPayloadAcrossMode()
        {
            AssertCrossModeReuse(CombatMode.Combat, CombatMode.Peace);
            AssertCrossModeReuse(CombatMode.Peace, CombatMode.Combat);
            AssertIncompleteCrossModeReplacementRestarts();
        }

        private static void AssertIncompleteCrossModeReplacementRestarts()
        {
            WithTemporaryDirectory(path =>
            {
                var gate = new RuntimeOperationGate();
                var completeGateway = new FakeCompleteClusterGateway();
                var lifecycle = new PreviewSessionLifecycle();
                using var resolver = new PreviewResolutionCoordinator(
                    lifecycle,
                    new PreviewScanCoordinator(new FakeGateway(), gate),
                    new CompleteClusterRawCoordinator(completeGateway, gate),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(
                    1,
                    PreviewIdentity(16_315_224),
                    Request());
                PreviewResolutionAttempt incomplete = resolver.CurrentPublishedAttempt!;
                resolver.AdvanceCurrent();
                Equal(PreviewResolutionState.Scanning, incomplete.State);

                resolver.ObserveCompletedLoad(
                    2,
                    PreviewIdentity(16_315_224, combatMode: CombatMode.Peace),
                    Request(combatMode: CombatMode.Peace));
                PreviewResolutionAttempt replacement = resolver.CurrentPublishedAttempt!;
                Equal(PreviewResolutionState.Cancelled, incomplete.State);
                Equal(PreviewResolutionState.Scanning, replacement.State);
                Equal(2, completeGateway.GenerateCalls);
                True(replacement.CachedPayloadSourceIdentity == null);
                False(lifecycle.CanPublish(incomplete.Session));
                True(lifecycle.CanPublish(replacement.Session));
            });
        }

        private static void AssertCrossModeReuse(
            CombatMode sourceMode,
            CombatMode activeMode)
        {
            WithTemporaryDirectory(path =>
            {
                var gate = new RuntimeOperationGate();
                var previewGateway = new FakeGateway();
                var completeGateway = new FakeCompleteClusterGateway();
                var lifecycle = new PreviewSessionLifecycle();
                using var resolver = new PreviewResolutionCoordinator(
                    lifecycle,
                    new PreviewScanCoordinator(previewGateway, gate),
                    new CompleteClusterRawCoordinator(completeGateway, gate),
                    new CompleteClusterConclusionCache(path));
                PreviewGenerationIdentity sourceIdentity = PreviewIdentity(
                    16_315_224,
                    combatMode: sourceMode);
                PreviewScanRequest sourceRequest = Request(combatMode: sourceMode);
                resolver.ObserveCompletedLoad(1, sourceIdentity, sourceRequest);
                PreviewResolutionAttempt source = resolver.CurrentPublishedAttempt!;
                while (!source.IsTerminal)
                    resolver.AdvanceCurrent();
                Equal(PreviewResolutionState.Complete, source.State);
                Equal(1, completeGateway.GenerateCalls);

                PreviewGenerationIdentity activeIdentity = PreviewIdentity(
                    16_315_224,
                    combatMode: activeMode);
                resolver.ObserveCompletedLoad(
                    2,
                    activeIdentity,
                    Request(combatMode: activeMode));
                PreviewResolutionAttempt active = resolver.CurrentPublishedAttempt!;
                Equal(PreviewResolutionState.Cached, active.State);
                Equal(1, active.TerminalTransitionCount);
                Equal(1, completeGateway.GenerateCalls);
                Equal(activeIdentity, active.Session.Identity);
                Equal(sourceIdentity, active.CachedPayloadSourceIdentity);
                True(active.CompleteReports.All(report =>
                    report.Settings.CombatMode == sourceMode));
                if (activeMode == CombatMode.Peace)
                    True(active.DarkFogOccupation == null);
                else
                    True(active.DarkFogOccupation != null);
                PreviewConclusionPresentation presentation =
                    PreviewConclusionPresenter.Project(active);
                Equal(activeMode == CombatMode.Peace ? "Peace" : "Combat",
                    presentation.IdentityLine.Split(' ').Last());
                Equal(activeMode == CombatMode.Peace,
                    presentation.DarkFogStatusLine == null);

                PreviewResolutionAttempt retired = active;
                resolver.ObserveCompletedLoad(
                    3,
                    PreviewIdentity(73_339_583, combatMode: activeMode),
                    RequestForSeed(73_339_583, activeMode));
                False(lifecycle.CanPublish(retired.Session));
                True(lifecycle.CanPublish(resolver.CurrentPublishedAttempt!.Session));
                resolver.ExitPreview();
                True(resolver.CurrentPublishedAttempt == null);
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
                completeFailure.AdvanceCurrent();
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

            var panel = new PreviewPanelController();
            panel.ShowUnavailable(
                4,
                PreviewPanelCorner.BottomRight,
                "Runtime identity could not be read");
            True(panel.Current.Visible);
            Equal(PreviewPanelOperationalState.Failed, panel.Current.State);
            Equal("Scanner unavailable", panel.Current.Title);
            Equal("Runtime identity could not be read", panel.Current.Detail);
            True(panel.Current.Detail.Length <= PreviewPanelLayout.MaximumDetailCharacters);
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

        private static void StatisticsPanelMirrorsConclusionLayout()
        {
            foreach ((int width, int height) in new[]
            {
                (1920, 1080),
                (2560, 1440),
                (3840, 2160)
            })
            {
                foreach (PreviewPanelCorner corner in Enum.GetValues(
                    typeof(PreviewPanelCorner)))
                {
                    PreviewPanelPlacement placement = PreviewPanelLayout.PlacePanelPair(
                        corner,
                        width,
                        height);
                    Equal(
                        PreviewPanelLayout.PlaceConclusion(corner, width, height),
                        placement.ConclusionBounds);
                    Equal(
                        PreviewPanelLayout.HorizontalOpposite(corner),
                        placement.StatisticsCorner);
                    Equal(placement.ConclusionBounds.Y, placement.StatisticsBounds.Y);
                    Equal(placement.ConclusionBounds.Width, placement.StatisticsBounds.Width);
                    Equal(placement.ConclusionBounds.Height, placement.StatisticsBounds.Height);
                    Equal(
                        width - placement.ConclusionBounds.Right,
                        placement.StatisticsBounds.X);
                    False(placement.ConclusionAnchorsRight == placement.StatisticsAnchorsRight);
                    Equal(
                        corner == PreviewPanelCorner.TopLeft ||
                            corner == PreviewPanelCorner.TopRight,
                        placement.AnchorsTop);
                    False(Overlaps(
                        placement.ConclusionBounds,
                        placement.StatisticsBounds));

                    PreviewPanelPlacement compact = PreviewPanelLayout.PlacePanelPair(
                        corner,
                        width,
                        height,
                        false);
                    Equal(
                        PreviewPanelLayout.Place(corner, width, height),
                        compact.ConclusionBounds);
                    Equal(compact.ConclusionBounds.Y, compact.StatisticsBounds.Y);
                    Equal(compact.ConclusionBounds.Width, compact.StatisticsBounds.Width);
                    Equal(compact.ConclusionBounds.Height, compact.StatisticsBounds.Height);
                    Equal(width - compact.ConclusionBounds.Right, compact.StatisticsBounds.X);
                    False(Overlaps(compact.ConclusionBounds, compact.StatisticsBounds));
                }
            }
        }

        private static void HomeSystemBodyInventoryIsImmutableCompleteAndOrdered()
        {
            var source = new List<RuntimeHomeSystemBodyEvidence>
            {
                HomeBody(104, "Alpha IV", 4, 2, 102, 3),
                HomeBody(101, "Alpha I", 1, 0, null, 0),
                HomeBody(103, "Alpha III", 3, 2, 102, 2),
                HomeBody(102, "Alpha II", 2, 0, null, 1),
                HomeBody(105, "Alpha V", 5, 0, null, 4)
            };
            HomeSystemBodyInventory? projected = HomeSystemBodyInventory.Project(
                "home-system",
                source);
            True(projected != null);
            HomeSystemBodyInventory inventory = projected!;
            Equal("101,102,103,104,105", String.Join(",", inventory.Bodies.Select(
                value => value.BodyId)));
            Equal("Alpha I,Alpha II,Alpha III,Alpha IV,Alpha V", String.Join(",",
                inventory.Bodies.Select(value => value.DisplayDesignation)));
            Equal(HomeSystemBodyOrbitKind.Primary, inventory.Bodies[0].OrbitKind);
            Equal(HomeSystemBodyOrbitKind.Primary, inventory.Bodies[1].OrbitKind);
            Equal(HomeSystemBodyOrbitKind.Satellite, inventory.Bodies[2].OrbitKind);
            Equal(102, inventory.Bodies[2].ParentBodyId);
            Equal(HomeSystemBodyOrbitKind.Satellite, inventory.Bodies[3].OrbitKind);
            Equal(102, inventory.Bodies[3].ParentBodyId);
            Equal(HomeSystemBodyOrbitKind.Primary, inventory.Bodies[4].OrbitKind);
            source.Clear();
            Equal(5, inventory.Bodies.Count);
            HomeSystemBody[] copy = inventory.Bodies.ToArray();
            copy[0] = copy[4];
            Equal(101, inventory.Bodies[0].BodyId);

            True(HomeSystemBodyInventory.Project(
                "home-system",
                new[]
                {
                    HomeBody(101, "Alpha I", 1, 0, null, 0),
                    HomeBody(102, "Alpha II", 2, 0, null, 1)
                }) != null);
            True(HomeSystemBodyInventory.Project(
                "home-system",
                new[]
                {
                    HomeBody(101, "Alpha I", 1, 0, null, 0),
                    HomeBody(102, "Alpha II", 2, 0, null, 1),
                    HomeBody(103, "Alpha III", 3, 2, 102, 2)
                }) != null);
            True(HomeSystemBodyInventory.Project(
                "home-system",
                new[]
                {
                    HomeBody(101, "Alpha I", 1, 0, null, 0),
                    HomeBody(102, "Alpha II", 2, 0, null, 1),
                    HomeBody(103, "Alpha III", 3, 1, 101, 2),
                    HomeBody(104, "Alpha IV", 4, 2, 102, 3)
                }) != null);
            HomeSystemBodyInventory? unresolvedReference =
                HomeSystemBodyInventory.Project(
                    "home-system",
                    new[]
                    {
                        HomeBody(101, "Alpha I", 1, 0, null, 0),
                        HomeBody(102, "Alpha II", 2, 0, null, 1),
                        HomeBody(103, "Alpha III", 3, 2, null, 2)
                    });
            True(unresolvedReference != null);
            Equal(
                102,
                unresolvedReference!.Bodies.Single(body => body.BodyId == 103).ParentBodyId);
            HomeSystemBodyInventory? repeatedMoonNumbers =
                HomeSystemBodyInventory.Project(
                    "home-system",
                    new[]
                    {
                        HomeBody(101, "Menkent I", 1, 0, null, 0),
                        HomeBody(102, "Menkent II", 2, 0, null, 1),
                        HomeBody(103, "Menkent III", 1, 2, 102, 2),
                        HomeBody(104, "Menkent IV", 2, 2, 102, 3)
                    });
            True(repeatedMoonNumbers != null);
            Equal("101,102,103,104", String.Join(",", repeatedMoonNumbers!.Bodies.Select(
                body => body.BodyId)));
            True(HomeSystemBodyInventory.Project(
                "home-system",
                new[] { HomeBody(103, "Alpha III", 3, 2, 999, 0) }) == null);
        }

        private static void HomeSystemStatisticsShowLayoutAndExactEnergyFacts()
        {
            var source = new SingleEnumerationEnumerable<RuntimeHomeSystemBodyEvidence>(
                new[]
                {
                    HomeBody(
                        104, "Alpha IV", 4, 2, 102, 3,
                        HomeSystemBodyKind.Solid, null, null, 1.5m),
                    HomeBody(
                        101, "Alpha I", 1, 0, null, 0,
                        HomeSystemBodyKind.Solid, "Mediterranean", 1.23m, 0.8m),
                    HomeBody(
                        103, "Alpha III", 3, 2, 102, 2,
                        HomeSystemBodyKind.IceGiant),
                    HomeBody(
                        102, "Alpha II", 2, 0, null, 1,
                        HomeSystemBodyKind.GasGiant),
                    HomeBody(
                        105, "Alpha V", 5, 0, null, 4,
                        HomeSystemBodyKind.Solid, "Lava", 0m, null)
                });

            HomeSystemBodyInventory inventory = HomeSystemBodyInventory.Project(
                "home-system",
                source)!;
            Equal(1, source.EnumerationCount);
            string[] lines = inventory.Bodies
                .Select(body => HomeSystemBodyPresentation.Format(body))
                .ToArray();
            Equal(5, lines.Length);
            Equal("Alpha I | Mediterranean | Solar 123% | Wind 80%", lines[0]);
            Equal("Alpha II | Gas giant", lines[1]);
            Equal("Alpha III | Ice giant", lines[2]);
            Equal("Alpha IV | Wind 150%", lines[3]);
            Equal("Alpha V | Lava | Solar 0%", lines[4]);
            Equal("123.456789%", HomeSystemBodyPresentation.FormatPercentage(
                1.23456789m));
            False(lines.Any(line => line.Contains("ore", StringComparison.OrdinalIgnoreCase)));
            Equal(1, source.EnumerationCount);

            HomeSystemBody[] copy = inventory.Bodies.ToArray();
            copy[0] = copy[4];
            Equal("Alpha I", inventory.Bodies[0].DisplayDesignation);
        }

        private static void HomeSystemResourcesJoinRowsOnlyWhenComplete()
        {
            HomeSystemBodyInventory inventory = HomeSystemBodyInventory.Project(
                "home-system",
                new[]
                {
                    HomeBody(
                        101, "Alpha I", 1, 0, null, 0,
                        HomeSystemBodyKind.Solid, "Mediterranean", 1.2m, 0.8m),
                    HomeBody(
                        102, "Alpha II", 2, 0, null, 1,
                        HomeSystemBodyKind.GasGiant,
                        gasProducts: new[] { "fire-ice", "hydrogen" }),
                    HomeBody(
                        103, "Alpha III", 3, 2, 102, 2,
                        HomeSystemBodyKind.Solid, "Lava", 1.5m, 1m),
                    HomeBody(
                        104, "Alpha IV", 4, 0, null, 3,
                        HomeSystemBodyKind.IceGiant)
                })!;
            var resources = new HomeSystemResourceStatistics(new[]
            {
                new HomeSystemBodyResources(101, new[]
                {
                    new HomeSystemResource(
                        "iron", RawResourceSemantics.FiniteDeposit, 12_345_678, 12),
                    new HomeSystemResource(
                        "fire-ice", RawResourceSemantics.FiniteDeposit, 987_654, 1),
                    new HomeSystemResource(
                        "oil", RawResourceSemantics.OilFlow, 1_234_567, 4)
                }),
                new HomeSystemBodyResources(103, new[]
                {
                    new HomeSystemResource(
                        "copper", RawResourceSemantics.FiniteDeposit, 2_500_000, 3),
                    new HomeSystemResource(
                        "stone", RawResourceSemantics.FiniteDeposit, 750_000, 2)
                })
            });

            Equal(
                "Alpha I | Mediterranean | Solar 120% | Wind 80% | " +
                    "Ores (units / vein groups): Iron 12.3M / 12; " +
                    "Fire Ice veins 988K / 1 | " +
                    "Crude Oil (flow units / groups): 1.23M / 4",
                HomeSystemBodyPresentation.Format(inventory.Bodies[0], resources));
            Equal(
                "Alpha II | Gas giant | Gas products: Fire Ice, Hydrogen",
                HomeSystemBodyPresentation.Format(inventory.Bodies[1], resources));
            Equal(
                "Alpha III | Lava | Solar 150% | Wind 100% | " +
                    "Ores (units / vein groups): Copper 2.5M / 3; " +
                    "Stone 750K / 2",
                HomeSystemBodyPresentation.Format(inventory.Bodies[2], resources));
            Equal(
                "Alpha IV | Ice giant",
                HomeSystemBodyPresentation.Format(inventory.Bodies[3], resources));
            False(HomeSystemBodyPresentation.Format(inventory.Bodies[1], resources)
                .Contains("Ores:", StringComparison.Ordinal));
            False(HomeSystemBodyPresentation.Format(inventory.Bodies[2], resources)
                .Contains("None", StringComparison.Ordinal));
            False(HomeSystemBodyPresentation.Format(inventory.Bodies[0], resources)
                .Contains("Coal", StringComparison.Ordinal));
            Equal("999", HomeSystemBodyPresentation.FormatAmount(999));
            Equal("1K", HomeSystemBodyPresentation.FormatAmount(1_000));
            Equal("12.3K", HomeSystemBodyPresentation.FormatAmount(12_345));
            Equal("1M", HomeSystemBodyPresentation.FormatAmount(999_999));
            Equal("1.23M", HomeSystemBodyPresentation.FormatAmount(1_234_567));
            HomeSystemBodyTableRow firstRow =
                HomeSystemBodyPresentation.ProjectTableRow(
                    inventory.Bodies[0],
                    resources);
            Equal("Alpha I", firstRow.Body);
            Equal("Mediterranean", firstRow.World);
            Equal("120%", firstRow.Solar);
            Equal("80%", firstRow.Wind);
            Equal("Iron 12.3M / 12, Fire Ice veins 988K / 1", firstRow.Ores);
            Equal("1.23M / 4", firstRow.Oil);
            Equal(String.Empty, firstRow.GasProducts);
            False(firstRow.Cells.Any(cell => cell.Contains("|", StringComparison.Ordinal)));
            HomeSystemBodyTableRow giantRow =
                HomeSystemBodyPresentation.ProjectTableRow(
                    inventory.Bodies[1],
                    resources);
            Equal("Gas giant", giantRow.World);
            Equal("Fire Ice\nHydrogen", giantRow.GasProducts);
            Equal(String.Empty, giantRow.Ores);
            Equal(String.Empty, giantRow.Oil);

            WithTemporaryDirectory(path =>
            {
                var gateway = new FakeGateway { Snapshot = Snapshot(
                    homeSystemBodyInventory: inventory) };
                var completeGateway = new FakeCompleteClusterGateway();
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(gateway),
                    new CompleteClusterRawCoordinator(completeGateway),
                    new CompleteClusterConclusionCache(path));
                var statistics = new PreviewStatisticsPanelController();

                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt attempt = resolver.CurrentPublishedAttempt!;
                statistics.BeginSession(attempt.Session);
                True(statistics.Update(attempt));
                True(statistics.Current!.HomeSystemResources == null);
                Equal(
                    "Alpha I | Mediterranean | Solar 120% | Wind 80%",
                    HomeSystemBodyPresentation.Format(
                        statistics.Current.HomeSystem!.Bodies[0],
                        statistics.Current.HomeSystemResources));

                while (!attempt.IsTerminal)
                {
                    resolver.AdvanceCurrent();
                    statistics.Update(attempt);
                }
                Equal(PreviewResolutionState.Complete, attempt.State);
                True(statistics.Current!.HomeSystemResources != null);
                Equal(
                    "Alpha I | Mediterranean | Solar 120% | Wind 80% | " +
                        "Ores (units / vein groups): Iron 10K / 1",
                    HomeSystemBodyPresentation.Format(
                        statistics.Current.HomeSystem!.Bodies[0],
                        statistics.Current.HomeSystemResources));
                Equal(4, statistics.Current.HomeSystem.Bodies.Count);
                Equal(1, completeGateway.GenerateCalls);

                resolver.ObserveCompletedLoad(2, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt cached = resolver.CurrentPublishedAttempt!;
                Equal(PreviewResolutionState.Cached, cached.State);
                statistics.BeginSession(cached.Session);
                True(statistics.Update(cached));
                True(statistics.Current!.HomeSystemResources != null);
                Equal(
                    "Alpha I | Mediterranean | Solar 120% | Wind 80% | " +
                        "Ores (units / vein groups): Iron 10K / 1",
                    HomeSystemBodyPresentation.Format(
                        statistics.Current.HomeSystem!.Bodies[0],
                        statistics.Current.HomeSystemResources));
                Equal(1, completeGateway.GenerateCalls);

                HomeSystemBodyInventory replacementInventory =
                    HomeSystemBodyInventory.Project(
                        "replacement-home",
                        new[]
                        {
                            HomeBody(
                                201, "Gamma I", 1, 0, null, 0,
                                HomeSystemBodyKind.Solid,
                                "Arid desert", 1m, 0.6m)
                        })!;
                gateway.Snapshot = Snapshot(
                    homeSystemBodyInventory: replacementInventory);
                resolver.ObserveCompletedLoad(
                    3,
                    PreviewIdentity(73_339_583),
                    RequestForSeed(73_339_583));
                PreviewResolutionAttempt replacement = resolver.CurrentPublishedAttempt!;
                statistics.BeginSession(replacement.Session);
                True(statistics.Update(replacement));
                True(statistics.Current!.HomeSystemResources == null);
                Equal(1, statistics.Current.HomeSystem!.Bodies.Count);
                Equal(
                    "Gamma I | Arid desert | Solar 100% | Wind 60%",
                    HomeSystemBodyPresentation.Format(
                        statistics.Current.HomeSystem.Bodies[0],
                        statistics.Current.HomeSystemResources));
                False(HomeSystemBodyPresentation.Format(
                        statistics.Current.HomeSystem.Bodies[0],
                        statistics.Current.HomeSystemResources)
                    .Contains("Ores:", StringComparison.Ordinal));
                Equal(2, completeGateway.GenerateCalls);

                PreviewResolutionAttempt? retired = resolver.ExitPreview();
                True(retired != null);
                True(statistics.Hide(replacement.Session.SessionId));
                True(statistics.Current == null);
            });
        }

        private static void ClusterStatisticsAreKeyedOrderedAndSectioned()
        {
            PreviewClusterStatistics empty = new PreviewClusterStatistics();
            Equal(0, empty.Items.Count);
            Equal(0, empty.Sections().Count);

            PreviewClusterStatistics values = empty
                .With(new PreviewStatisticItem("plain", "Plain", 5))
                .With(new PreviewStatisticItem(
                    "rare-2", "Rare two", 2, "rare", "Rare resources", 1))
                .With(new PreviewStatisticItem(
                    "star-1", "Star one", 1, "stars", "Notable stars", 2))
                .With(new PreviewStatisticItem(
                    "rare-1", "Rare one", 1, "rare", "Rare resources", 1));
            Equal("plain,rare-1,rare-2,star-1", String.Join(",", values.Items.Select(
                value => value.Key)));
            Equal(3, values.Sections().Count);
            True(values.Sections()[0].Title == null);
            Equal("Rare resources", values.Sections()[1].Title);
            Equal("rare-1,rare-2", String.Join(",", values.Sections()[1].Items.Select(
                value => value.Key)));
            Equal("Notable stars", values.Sections()[2].Title);

            PreviewClusterStatistics replaced = values.With(
                new PreviewStatisticItem(
                    "rare-1", "Rare one replaced", 3, "rare", "Rare resources", 1));
            Equal(4, replaced.Items.Count);
            Equal("Rare one replaced", replaced.Items.Single(value =>
                value.Key == "rare-1").Text);
            Equal(4, values.Items.Count);
            Equal("Rare one", values.Items.Single(value => value.Key == "rare-1").Text);
            bool conflictingSectionRejected = false;
            try
            {
                values.With(new PreviewStatisticItem(
                    "bad", "Bad", 1, "rare", "Different title", 1));
            }
            catch (ArgumentException)
            {
                conflictingSectionRejected = true;
            }
            True(conflictingSectionRejected);
        }

        private static void ClusterLocationsFormatAuAndPreserveStableTies()
        {
            Equal("0 AU", DspAuFormatter.Format(0m));
            Equal("0.0123 AU", DspAuFormatter.Format(0.012345m));
            Equal("1.23 AU", DspAuFormatter.Format(1.2345m));
            Equal("12.3 AU", DspAuFormatter.Format(12.345m));
            Equal("123 AU", DspAuFormatter.Format(123.45m));
            Equal("1230 AU", DspAuFormatter.Format(1234.5m));

            ClusterBodyLocation first = Location("body-2", "Beta II", "beta", 12.345m, 2);
            ClusterBodyLocation second = Location("body-1", "Beta I", "beta", 12.345m, 1);
            ClusterBodyLocation home = Location("home-1", "Alpha I", "home", 0m, 3);
            IReadOnlyList<ClusterBodyLocation> ordered = DspAuFormatter.StableOrder(
                new[] { first, home, second });
            Equal("home-1,body-1,body-2", String.Join(",", ordered.Select(
                value => value.BodyIdentifier)));
            Equal("Beta I", ordered[1].DisplayDesignation);
            Equal("beta", ordered[1].HostSystemIdentifier);
            Equal("12.3 AU", ordered[1].FormattedDistance);
        }

        private static void StatisticsPanelFollowsPreviewLifecycleIndependently()
        {
            WithTemporaryDirectory(path =>
            {
                var gate = new RuntimeOperationGate();
                var gateway = new FakeGateway
                {
                    Snapshot = Snapshot(
                        homeSystemBodyInventory: HomeSystemBodyInventory.Project(
                            "1",
                            new[]
                            {
                                HomeBody(
                                    101, "Alpha I", 1, 0, null, 0,
                                    HomeSystemBodyKind.Solid,
                                    "Mediterranean", 1.2m, 0.8m),
                                HomeBody(
                                    102, "Alpha II", 2, 0, null, 1,
                                    HomeSystemBodyKind.GasGiant),
                                HomeBody(
                                    103, "Alpha III", 3, 2, 102, 2,
                                    HomeSystemBodyKind.Solid,
                                    "Oceanic jungle", null, 1.5m)
                            }))
                };
                var completeGateway = new FakeCompleteClusterGateway();
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(gateway, gate),
                    new CompleteClusterRawCoordinator(
                        completeGateway,
                        gate),
                    new CompleteClusterConclusionCache(path));
                var conclusions = new PreviewPanelController();
                var statistics = new PreviewStatisticsPanelController();

                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt first = resolver.CurrentPublishedAttempt!;
                conclusions.BeginSession(first.Session.SessionId, PreviewPanelCorner.BottomRight, 0);
                conclusions.Update(first, PreviewPanelCorner.BottomRight, 1);
                PreviewPanelView conclusionBefore = conclusions.Current;
                PreviewConclusionPresentation conclusionDocument = conclusions.Conclusions!;
                statistics.BeginSession(first.Session);
                True(statistics.Update(first));
                PreviewStatisticsDocument firstDocument = statistics.Current!;
                Equal(PreviewStatisticsDocument.HomeSystemTitle, "Home system");
                Equal(PreviewStatisticsDocument.ClusterTitle, "Cluster");
                Equal(conclusionDocument.IdentityLine, firstDocument.IdentityLine);
                Equal(3, firstDocument.HomeSystem!.Bodies.Count);
                Equal(
                    "Alpha I | Mediterranean | Solar 120% | Wind 80%",
                    HomeSystemBodyPresentation.Format(firstDocument.HomeSystem.Bodies[0]));
                Equal(
                    "Alpha II | Gas giant",
                    HomeSystemBodyPresentation.Format(firstDocument.HomeSystem.Bodies[1]));
                Equal(
                    "Alpha III | Oceanic jungle | Wind 150%",
                    HomeSystemBodyPresentation.Format(firstDocument.HomeSystem.Bodies[2]));
                Equal(0, firstDocument.Cluster.Items.Count);
                Equal(1, completeGateway.GenerateCalls);
                True(statistics.SetScrollPosition(first.Session.SessionId, 0, 74));
                Equal(74.0, statistics.ScrollY);
                Equal(1, completeGateway.GenerateCalls);

                gateway.Snapshot = Snapshot(
                    homeSystemBodyInventory: HomeSystemBodyInventory.Project(
                        "1",
                        new[] { HomeBody(201, "Gamma I", 1, 0, null, 0) }));
                resolver.ObserveCompletedLoad(
                    2,
                    PreviewIdentity(73_339_583),
                    RequestForSeed(73_339_583));
                PreviewResolutionAttempt replacement = resolver.CurrentPublishedAttempt!;
                False(statistics.Update(first));
                True(ReferenceEquals(firstDocument, statistics.Current));
                statistics.BeginSession(replacement.Session);
                Equal(0.0, statistics.ScrollY);
                True(statistics.Update(replacement));
                Equal(1, statistics.Current!.HomeSystem!.Bodies.Count);
                Equal(201, statistics.Current.HomeSystem.Bodies[0].BodyId);
                Equal(2, completeGateway.GenerateCalls);
                False(statistics.Hide(first.Session.SessionId));
                True(statistics.Current != null);

                PreviewResolutionAttempt? retired = resolver.ExitPreview();
                True(retired != null);
                True(statistics.Hide(replacement.Session.SessionId));
                True(statistics.Current == null);
                False(statistics.Update(replacement));
                Equal(2, completeGateway.GenerateCalls);
                True(ReferenceEquals(conclusionBefore, conclusions.Current));
                True(ReferenceEquals(conclusionDocument, conclusions.Conclusions));
            });
        }

        private static void HomePlanetDesignationIsSharedImmutableAndSessionOwned()
        {
            WithTemporaryDirectory(path =>
            {
                var gate = new RuntimeOperationGate();
                var gateway = new FakeGateway
                {
                    Snapshot = Snapshot(homePlanetDisplayDesignation: "Alpha III")
                };
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(gateway, gate),
                    new CompleteClusterRawCoordinator(
                        new FakeCompleteClusterGateway(),
                        gate),
                    new CompleteClusterConclusionCache(path));
                var conclusions = new PreviewPanelController();
                var statistics = new PreviewStatisticsPanelController();

                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt first = resolver.CurrentPublishedAttempt!;
                Equal("Alpha III", first.Session.HomePlanetDisplayDesignation);
                conclusions.BeginSession(
                    first.Session.SessionId,
                    PreviewPanelCorner.BottomRight,
                    0);
                statistics.BeginSession(first.Session);
                True(conclusions.Update(
                    first,
                    PreviewPanelCorner.BottomRight,
                    1));
                True(statistics.Update(first));
                const string firstTitle =
                    "Seed 16315224 | Home Alpha III | 64 stars | resources x1 | Combat";
                Equal(firstTitle, conclusions.Conclusions!.IdentityLine);
                Equal(firstTitle, statistics.Current!.IdentityLine);
                bool immutable = false;
                try
                {
                    first.Session.SetHomePlanetDisplayDesignation("Alpha IV");
                }
                catch (InvalidOperationException)
                {
                    immutable = true;
                }
                True(immutable);

                gateway.Snapshot = Snapshot(homePlanetDisplayDesignation: "Gamma II");
                resolver.ObserveCompletedLoad(
                    2,
                    PreviewIdentity(73_339_583),
                    RequestForSeed(73_339_583));
                PreviewResolutionAttempt replacement = resolver.CurrentPublishedAttempt!;
                True(first.Session.IsRetired);
                True(first.Session.HomePlanetDisplayDesignation == null);
                False(conclusions.Update(
                    first,
                    PreviewPanelCorner.BottomRight,
                    2));
                False(statistics.Update(first));
                conclusions.BeginSession(
                    replacement.Session.SessionId,
                    PreviewPanelCorner.BottomRight,
                    0);
                statistics.BeginSession(replacement.Session);
                conclusions.Update(
                    replacement,
                    PreviewPanelCorner.BottomRight,
                    1);
                statistics.Update(replacement);
                const string replacementTitle =
                    "Seed 73339583 | Home Gamma II | 64 stars | resources x1 | Combat";
                Equal(replacementTitle, conclusions.Conclusions!.IdentityLine);
                Equal(replacementTitle, statistics.Current!.IdentityLine);

                PreviewResolutionAttempt? exited = resolver.ExitPreview();
                True(exited != null);
                True(exited!.Session.HomePlanetDisplayDesignation == null);
                conclusions.HideCurrent();
                statistics.HideCurrent();
                False(conclusions.Update(
                    replacement,
                    PreviewPanelCorner.BottomRight,
                    2));
                False(statistics.Update(replacement));
                True(statistics.Current == null);

                gateway.Snapshot = Snapshot(homePlanetDisplayDesignation: "Alpha III");
                resolver.ObserveCompletedLoad(3, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt returned = resolver.CurrentPublishedAttempt!;
                False(ReferenceEquals(first.Session, returned.Session));
                Equal("Alpha III", returned.Session.HomePlanetDisplayDesignation);
                conclusions.BeginSession(
                    returned.Session.SessionId,
                    PreviewPanelCorner.TopLeft,
                    0);
                statistics.BeginSession(returned.Session);
                conclusions.Update(returned, PreviewPanelCorner.TopLeft, 1);
                statistics.Update(returned);
                Equal(firstTitle, conclusions.Conclusions!.IdentityLine);
                Equal(firstTitle, statistics.Current!.IdentityLine);
            });
        }

        private static bool Overlaps(PreviewPanelBounds first, PreviewPanelBounds second)
        {
            return first.X < second.Right && first.Right > second.X &&
                first.Y < second.Bottom && first.Bottom > second.Y;
        }

        private static RuntimeHomeSystemBodyEvidence HomeBody(
            int bodyId,
            string displayDesignation,
            int planetNumber,
            int orbitAround,
            int? parentBodyId,
            int stableGameOrder,
            HomeSystemBodyKind bodyKind = HomeSystemBodyKind.Solid,
            string? themeName = null,
            decimal? solarRatio = null,
            decimal? windRatio = null,
            IEnumerable<string>? gasProducts = null)
        {
            return new RuntimeHomeSystemBodyEvidence(
                bodyId,
                displayDesignation,
                planetNumber,
                orbitAround,
                parentBodyId,
                stableGameOrder,
                bodyKind,
                themeName,
                solarRatio,
                windRatio,
                gasProducts);
        }

        private static ClusterBodyLocation Location(
            string bodyIdentifier,
            string displayDesignation,
            string systemIdentifier,
            decimal distanceAu,
            int stableGameOrder)
        {
            return new ClusterBodyLocation(
                bodyIdentifier,
                displayDesignation,
                systemIdentifier,
                distanceAu,
                stableGameOrder);
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
                Equal(4, scanning.ImmediateGroups.Count);
                Equal(
                    "Fresh start,Megafactory,Compact expansion,Sphere / energy",
                    String.Join(",", scanning.ImmediateGroups.Select(group => group.Title)));
                Equal("Dark Fog: 40 initial hives; 1 in starter system",
                    scanning.DarkFogStatusLine);
                True(scanning.ImmediateGroups.SelectMany(group => group.Cards)
                    .All(card => card.Stage == EvidenceStage.GalaxyPreview));
                True(scanning.ImmediateGroups.SelectMany(group => group.Cards)
                    .All(card => card.Outcome != ComponentOutcome.Unknown &&
                        card.Outcome != ComponentOutcome.NotApplicable));
                True(scanning.ImmediateGroups.SelectMany(group => group.Cards)
                    .All(card => !card.Line.Contains(":star:", StringComparison.Ordinal)));
                resolver.AdvanceCurrent();
                resolver.AdvanceCurrent();
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
                PresentedContextGroup completedFresh = complete.DetailGroups.Single(group =>
                    group.Context == ConclusionContext.FreshStart);
                string completedFreshText = String.Join("\n",
                    completedFresh.Cards.Select(card => card.Line));
                True(completedFreshText.Contains("Iron", StringComparison.Ordinal));
                True(completedFreshText.Contains("scarce", StringComparison.Ordinal));
                True(completedFreshText.Contains("few vein groups", StringComparison.Ordinal));
                Equal(1, completedFresh.Cards.Count(card =>
                    card.Line.Contains("scarce", StringComparison.Ordinal)));
                Equal(1, completedFresh.Cards.Count(card =>
                    card.Line.Contains("few vein groups", StringComparison.Ordinal)));
                True(completedFreshText.Contains("No Fire Ice veins", StringComparison.Ordinal));
                False(completedFreshText.Contains(
                    "Combined starter deposits",
                    StringComparison.Ordinal));
                True(complete.DetailGroups.SelectMany(group => group.Cards)
                    .All(card => card.Stage == EvidenceStage.BirthSystemRaw ||
                        card.Stage == EvidenceStage.CompleteClusterRaw));
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
                string cachedFreshText = String.Join("\n", panel.Conclusions.DetailGroups
                    .Single(group => group.Context == ConclusionContext.FreshStart)
                    .Cards.Select(card => card.Line));
                Equal(completedFreshText, cachedFreshText);
            });
        }

        private static void FreshStartCopyIsNaturalBoundedAndAttributed()
        {
            WithTemporaryDirectory(path =>
            {
                var gateway = new FakeGateway
                {
                    Snapshot = Snapshot(birthPlanetAttributions: new[]
                    {
                        SolidAttribution(101, "Aspidiske I", 1.35m, 1.5m, true),
                        SolidAttribution(102, "Aspidiske II", 1.20m, 1.1m, false),
                        SolidAttribution(103, "Aspidiske III", 0.9m, 0.8m, false),
                        GasAttribution(104, "Aspidiske IV", "hydrogen"),
                        GasAttribution(105, "Aspidiske V", "hydrogen")
                    })
                };
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(gateway),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt attempt = resolver.CurrentPublishedAttempt!;
                PreviewConclusionPresentation presentation =
                    PreviewConclusionPresenter.Project(attempt);
                PresentedContextGroup fresh = presentation.ImmediateGroups.Single(group =>
                    group.Context == ConclusionContext.FreshStart);
                string rendered = String.Join("\n", fresh.Cards.Select(card => card.Line));

                True(rendered.Contains("Starter gas giants have Hydrogen", StringComparison.Ordinal));
                True(rendered.Contains("Starter gas giants lack Deuterium / Fire Ice", StringComparison.Ordinal));
                True(rendered.Contains("Aspidiske I has bright solar", StringComparison.Ordinal));
                True(rendered.Contains("Aspidiske I has strong wind", StringComparison.Ordinal));
                True(rendered.Contains("Aspidiske I is tidally locked", StringComparison.Ordinal));
                True(rendered.Contains("3 moons orbit the home giant", StringComparison.Ordinal));
                False(rendered.Contains("Combined starter deposits", StringComparison.Ordinal));
                foreach (string forbidden in new[]
                {
                    "@", "type star", "%", "ratio", "amount", "distribution",
                    "Birth-system", "collection rate", "+", "permanent solar source",
                    "gas giant neighbor"
                })
                {
                    False(rendered.Contains(forbidden, StringComparison.OrdinalIgnoreCase));
                }
                True(fresh.Cards.All(card => card.Line.Length <=
                    PreviewConclusionPresenter.MaximumLineCharacters));
            });
        }

        private static void FreshStartOmitsUnavailableAttribution()
        {
            WithTemporaryDirectory(path =>
            {
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(new FakeGateway()),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PresentedContextGroup fresh = PreviewConclusionPresenter
                    .Project(resolver.CurrentPublishedAttempt!)
                    .ImmediateGroups.Single(group =>
                        group.Context == ConclusionContext.FreshStart);
                string rendered = String.Join("\n", fresh.Cards.Select(card => card.Line));

                False(rendered.Contains("solar", StringComparison.OrdinalIgnoreCase));
                False(rendered.Contains("wind", StringComparison.OrdinalIgnoreCase));
                False(rendered.Contains("gas giant has", StringComparison.OrdinalIgnoreCase));
                False(rendered.Contains("gas giants have", StringComparison.OrdinalIgnoreCase));
                False(rendered.Contains("tidally locked", StringComparison.OrdinalIgnoreCase));
                True(rendered.Contains("moons orbit the home giant", StringComparison.OrdinalIgnoreCase));
            });
        }

        private static void TidalLockCopyIsBoundedAndLiteral()
        {
            ConclusionReport supports = PresentationReport(
                ConclusionContext.FreshStart,
                ComponentOutcome.Supports,
                EvidenceStage.GalaxyPreview,
                "FS-POWER.birth-tidal",
                new ConclusionSubject(SubjectKind.BirthSystem, "home"));
            ConclusionReport absent = PresentationReport(
                ConclusionContext.FreshStart,
                ComponentOutcome.DoesNotSupport,
                EvidenceStage.GalaxyPreview,
                "FS-POWER.birth-tidal",
                new ConclusionSubject(SubjectKind.BirthSystem, "home"));
            NormalizedBirthPlanetEvidence[] planets = Enumerable.Range(1, 4)
                .Select(index => SolidAttribution(
                    100 + index,
                    "Alpha " + index,
                    1m,
                    1m,
                    true))
                .ToArray();
            var topology = new NormalizedHomePlanetTopology(
                101,
                HomePlanetOrbitKind.DirectStar);

            Equal("Alpha 1 is tidally locked", FreshLines(
                new[] { supports }, planets.Take(1), topology).Single());
            Equal("Alpha 1 and Alpha 2 are tidally locked", FreshLines(
                new[] { supports }, planets.Take(2), topology).Single());
            Equal("Alpha 1, Alpha 2, and Alpha 3 are tidally locked", FreshLines(
                new[] { supports }, planets.Take(3), topology).Single());
            Equal("4 home planets are tidally locked", FreshLines(
                new[] { supports }, planets, topology).Single());
            Equal("No tidally locked home planets", FreshLines(
                new[] { absent }, Array.Empty<NormalizedBirthPlanetEvidence>(), topology).Single());
            Equal(0, FreshLines(new[] { supports }, null, topology).Count);
        }

        private static void FreshStartResourcesGroupByMetricAndOutcome()
        {
            var reports = new List<ConclusionReport>();
            foreach ((string resource, ComponentOutcome amount, ComponentOutcome groups) in
                new[]
                {
                    ("iron", ComponentOutcome.Supports, ComponentOutcome.Supports),
                    ("oil", ComponentOutcome.Supports, ComponentOutcome.Supports),
                    ("copper", ComponentOutcome.PreferenceSensitive,
                        ComponentOutcome.PreferenceSensitive),
                    ("silicon", ComponentOutcome.PreferenceSensitive,
                        ComponentOutcome.PreferenceSensitive),
                    ("stone", ComponentOutcome.PreferenceSensitive,
                        ComponentOutcome.PreferenceSensitive),
                    ("titanium", ComponentOutcome.PreferenceSensitive,
                        ComponentOutcome.PreferenceSensitive),
                    ("coal", ComponentOutcome.DoesNotSupport,
                        ComponentOutcome.DoesNotSupport)
                })
            {
                reports.Add(PresentationReport(
                    ConclusionContext.FreshStart,
                    amount,
                    EvidenceStage.BirthSystemRaw,
                    "FS-RESOURCES.amount:" + resource,
                    new ConclusionSubject(SubjectKind.Resource, resource)));
                reports.Add(PresentationReport(
                    ConclusionContext.FreshStart,
                    groups,
                    EvidenceStage.BirthSystemRaw,
                    "FS-RESOURCES.groups:" + resource,
                    new ConclusionSubject(SubjectKind.Resource, resource)));
            }
            IReadOnlyList<PresentedConclusionCard> cards = FreshCards(reports, null, null);
            string[] lines = cards.Select(card => card.Line).ToArray();

            True(lines.SequenceEqual(new[]
            {
                "Iron, Oil plentiful",
                "Copper, Silicon, Stone, Titanium normal",
                "Coal scarce",
                "Iron, Oil has many vein groups",
                "Copper, Silicon, Stone, Titanium has normal vein groups",
                "Coal has few vein groups"
            }));
            Equal(6, cards.Count);
            Equal(2, cards[0].SourceConclusionIds.Count);
            Equal(4, cards[1].SourceConclusionIds.Count);
            Equal(4, cards[4].SourceConclusionIds.Count);
        }

        private static void MegafactoryCopyIsNaturalBoundedAndGrouped()
        {
            WithTemporaryDirectory(path =>
            {
                var facts = new Dictionary<int, (decimal Energy, long Radius, int Orbits)>
                {
                    [2] = (2.70m, 250_000, 4),
                    [3] = (2.60m, 200_000, 3),
                    [4] = (2.55m, 195_000, 2),
                    [5] = (2.51m, 192_000, 2)
                };
                var previewGateway = new FakeGateway
                {
                    Snapshot = Snapshot(systemCandidateFacts: facts)
                };
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(previewGateway),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt attempt = resolver.CurrentPublishedAttempt!;
                PreviewConclusionPresentation immediate =
                    PreviewConclusionPresenter.Project(attempt);
                string immediateText = String.Join("\n", immediate.ImmediateGroups
                    .Single(group => group.Context == ConclusionContext.Megafactory)
                    .Cards.Select(card => card.Line));

                True(immediateText.Contains(
                    "Many bright stars: Star 2, Star 3, and Star 4",
                    StringComparison.Ordinal));
                True(immediateText.Contains(
                    "Many large spheres: Star 2, Star 3, and Star 4",
                    StringComparison.Ordinal));
                True(immediateText.Contains(
                    "Many contained-orbit systems: Star 2, Star 3, and Star 4",
                    StringComparison.Ordinal));

                while (!attempt.IsTerminal)
                    resolver.AdvanceCurrent();
                PreviewConclusionPresentation complete =
                    PreviewConclusionPresenter.Project(attempt);
                string detailText = String.Join("\n", complete.DetailGroups
                    .Single(group => group.Context == ConclusionContext.Megafactory)
                    .Cards.Select(card => card.Line));
                True(detailText.Contains("Nearby Kimberlite in Star 2", StringComparison.Ordinal));
                True(detailText.Contains(
                    "Distant Unipolar Magnet in Star 3",
                    StringComparison.Ordinal));
                True(detailText.Contains(
                    "Many rare resources absent: Fire Ice, Fractal Silicon, and Optical Grating Crystal",
                    StringComparison.Ordinal));

                string rendered = immediateText + "\n" + detailText;
                foreach (string forbidden in new[]
                {
                    "@", "type star", "MF-", "RR-", "strong-energy",
                    "large-shell", "orbit-containment", "rare-access",
                    "runtime-amount", "+"
                })
                {
                    False(rendered.Contains(forbidden, StringComparison.OrdinalIgnoreCase));
                }
                True(complete.ImmediateGroups.Concat(complete.DetailGroups)
                    .Where(group => group.Context == ConclusionContext.Megafactory)
                    .SelectMany(group => group.Cards)
                    .All(card => card.Line.Length <=
                        PreviewConclusionPresenter.MaximumLineCharacters));

                resolver.ObserveCompletedLoad(2, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt cached = resolver.CurrentPublishedAttempt!;
                Equal(PreviewResolutionState.Cached, cached.State);
                string cachedDetailText = String.Join("\n", PreviewConclusionPresenter
                    .Project(cached)
                    .DetailGroups.Single(group =>
                        group.Context == ConclusionContext.Megafactory)
                    .Cards.Select(card => card.Line));
                Equal(detailText, cachedDetailText);
            });

            WithTemporaryDirectory(path =>
            {
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(new FakeGateway()),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                string text = String.Join("\n", PreviewConclusionPresenter
                    .Project(resolver.CurrentPublishedAttempt!)
                    .ImmediateGroups.Single(group =>
                        group.Context == ConclusionContext.Megafactory)
                    .Cards.Select(card => card.Line));
                True(text.Contains(
                    "Star 2: outshines all, large sphere, 4 contained orbits",
                    StringComparison.Ordinal));
            });

            WithTemporaryDirectory(path =>
            {
                var facts = new Dictionary<int, (decimal Energy, long Radius, int Orbits)>
                {
                    [2] = (2.46m, 60_000, 0)
                };
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(new FakeGateway
                    {
                        Snapshot = Snapshot(systemCandidateFacts: facts)
                    }),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                string text = String.Join("\n", PreviewConclusionPresenter
                    .Project(resolver.CurrentPublishedAttempt!)
                    .ImmediateGroups.Single(group =>
                        group.Context == ConclusionContext.Megafactory)
                    .Cards.Select(card => card.Line));
                True(text.Contains("Star 2 brightest", StringComparison.Ordinal));
                True(text.Contains("No large spheres", StringComparison.Ordinal));
                True(text.Contains("No contained orbits", StringComparison.Ordinal));
            });

            WithTemporaryDirectory(path =>
            {
                var facts = new Dictionary<int, (decimal Energy, long Radius, int Orbits)>
                {
                    [2] = (2.60m, 60_000, 0),
                    [3] = (2.59m, 60_000, 0)
                };
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(new FakeGateway
                    {
                        Snapshot = Snapshot(systemCandidateFacts: facts)
                    }),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                string text = String.Join("\n", PreviewConclusionPresenter
                    .Project(resolver.CurrentPublishedAttempt!)
                    .ImmediateGroups.Single(group =>
                        group.Context == ConclusionContext.Megafactory)
                    .Cards.Select(card => card.Line));
                True(text.Contains("Star 2 unusually bright", StringComparison.Ordinal));
                True(text.Contains("Star 3 bright", StringComparison.Ordinal));
            });

            WithTemporaryDirectory(path =>
            {
                var facts = new Dictionary<int, (decimal Energy, long Radius, int Orbits)>
                {
                    [2] = (2.40m, 60_000, 0)
                };
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(new FakeGateway
                    {
                        Snapshot = Snapshot(systemCandidateFacts: facts)
                    }),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                string text = String.Join("\n", PreviewConclusionPresenter
                    .Project(resolver.CurrentPublishedAttempt!)
                    .ImmediateGroups.Single(group =>
                        group.Context == ConclusionContext.Megafactory)
                    .Cards.Select(card => card.Line));
                True(text.Contains("No bright stars", StringComparison.Ordinal));
            });
        }

        private static void CompactRoutesAreNaturalDeduplicatedAndBounded()
        {
            var energyOnly = new Dictionary<int, (decimal Energy, long Radius, int Orbits)>
            {
                [2] = (2.70m, 60_000, 0)
            };
            True(CompleteCompactText(Snapshot(
                systemCandidateFacts: energyOnly,
                primaryDistanceLy: 2m)).Contains(
                    "Short routes: starter, energy, rares"));
            True(CompleteCompactText(Snapshot(
                systemCandidateFacts: energyOnly,
                primaryDistanceLy: 4m)).SequenceEqual(new[]
                {
                    "Short routes: starter, rares",
                    "Normal routes: energy"
                }));
            True(CompleteCompactText(Snapshot(
                systemCandidateFacts: energyOnly,
                primaryDistanceLy: 20m)).SequenceEqual(new[]
                {
                    "Short routes: starter, rares",
                    "Long routes: energy"
                }));

            var repeatedOrbitRole = new Dictionary<int, (decimal Energy, long Radius, int Orbits)>
            {
                [2] = (1m, 60_000, 3),
                [3] = (1m, 60_000, 2)
            };
            string[] deduplicated = CompleteCompactText(Snapshot(
                systemCandidateFacts: repeatedOrbitRole,
                primaryDistanceLy: 2m));
            Equal("Short routes: starter, orbits, rares", deduplicated.Single());

            var noPreviewRoles = new Dictionary<int, (decimal Energy, long Radius, int Orbits)>
            {
                [2] = (1m, 60_000, 0)
            };
            string[] withRare = CompleteCompactText(Snapshot(
                systemCandidateFacts: noPreviewRoles,
                primaryDistanceLy: 2m));
            Equal("Short routes: starter, rares", withRare.Single());

            var allRoles = new Dictionary<int, (decimal Energy, long Radius, int Orbits)>
            {
                [2] = (2.70m, 250_000, 4)
            };
            string bounded = CompleteCompactText(Snapshot(
                systemCandidateFacts: allRoles,
                primaryDistanceLy: 2m)).Single();
            Equal("Short routes: starter, energy, sphere", bounded);
            foreach (string forbidden in new[]
            {
                "ly", "<->", "Star ", "type star", "starter-anchor",
                "strong-energy", "large-shell", "orbit-containment",
                "rare-access", "CX-", "@"
            })
            {
                False(bounded.Contains(forbidden, StringComparison.OrdinalIgnoreCase));
            }
        }

        private static string[] CompleteCompactText(RuntimePreviewSnapshot snapshot)
        {
            string[]? result = null;
            WithTemporaryDirectory(path =>
            {
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(new FakeGateway { Snapshot = snapshot }),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt attempt = resolver.CurrentPublishedAttempt!;
                while (!attempt.IsTerminal)
                    resolver.AdvanceCurrent();
                PresentedContextGroup compact = PreviewConclusionPresenter.Project(attempt)
                    .DetailGroups.Single(group =>
                        group.Context == ConclusionContext.CompactExpansion);
                result = compact.Cards.Select(card => card.Line).ToArray();

                resolver.ObserveCompletedLoad(2, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt cached = resolver.CurrentPublishedAttempt!;
                Equal(PreviewResolutionState.Cached, cached.State);
                True(result.SequenceEqual(
                    PreviewConclusionPresenter.Project(cached)
                        .DetailGroups.Single(group =>
                            group.Context == ConclusionContext.CompactExpansion)
                        .Cards.Select(card => card.Line)));
            });
            return result ?? throw new InvalidOperationException(
                "Compact route fixture did not publish a result.");
        }

        private static void SphereCandidatesAreNaturalDeterministicAndBounded()
        {
            WithTemporaryDirectory(path =>
            {
                var facts = new Dictionary<int, (decimal Energy, long Radius, int Orbits)>
                {
                    [1] = (1m, 60_000, 0),
                    [2] = (1m, 250_000, 4),
                    [3] = (1m, 100_000, 1)
                };
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(new FakeGateway
                    {
                        Snapshot = Snapshot(systemCandidateFacts: facts)
                    }),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt attempt = resolver.CurrentPublishedAttempt!;
                PresentedContextGroup sphere = PreviewConclusionPresenter.Project(attempt)
                    .ImmediateGroups.Single(group =>
                        group.Context == ConclusionContext.SphereShowcase);
                string[] lines = sphere.Cards.Select(card => card.Line).ToArray();

                True(lines.SequenceEqual(new[]
                {
                    "Grand shell at Star 2",
                    "Normal shell at Star 3",
                    "Tiny shell at Alpha",
                    "Many contained orbits at Star 2",
                    "1 contained orbit at Star 3",
                    "No contained orbits at Alpha"
                }));
                Equal(
                    String.Join("\n", lines),
                    String.Join("\n", PreviewConclusionPresenter.Project(attempt)
                        .ImmediateGroups.Single(group =>
                            group.Context == ConclusionContext.SphereShowcase)
                        .Cards.Select(card => card.Line)));
                True(sphere.Cards.All(card => card.Line.Length <=
                    PreviewConclusionPresenter.MaximumLineCharacters));
                foreach (string forbidden in new[]
                {
                    "radius", "orbit distance", "+", "@", "geometry",
                    "receiver", "output", "type star", "MF-"
                })
                {
                    False(String.Join("\n", lines).Contains(
                        forbidden,
                        StringComparison.OrdinalIgnoreCase));
                }
            });

            WithTemporaryDirectory(path =>
            {
                var facts = new Dictionary<int, (decimal Energy, long Radius, int Orbits)>
                {
                    [2] = (1m, 250_000, 4),
                    [3] = (1m, 240_000, 3),
                    [4] = (1m, 230_000, 2)
                };
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(new FakeGateway
                    {
                        Snapshot = Snapshot(systemCandidateFacts: facts)
                    }),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));
                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                string[] lines = PreviewConclusionPresenter
                    .Project(resolver.CurrentPublishedAttempt!)
                    .ImmediateGroups.Single(group =>
                        group.Context == ConclusionContext.SphereShowcase)
                    .Cards.Select(card => card.Line).ToArray();
                True(lines.Contains(
                    "Grand shells at Star 2, Star 3, and Star 4"));
                True(lines.Contains(
                    "Many contained orbits at Star 2, Star 3, and Star 4"));
                True(lines.All(line => line.Count(character => character == ',') <= 2));
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
                Equal("Dark Fog: 40 initial hives; 1 in starter system",
                    document.Lines[2].Text);
                Equal("Fresh start", document.Lines[3].Text);
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

        private static void RefinedReleaseCandidateIsCoherentAcrossScanAndCache()
        {
            WithTemporaryDirectory(path =>
            {
                var previewGateway = new FakeGateway
                {
                    Snapshot = Snapshot(birthPlanetAttributions: new[]
                    {
                        SolidAttribution(101, "Alpha I", 1.35m, 1.5m, true),
                        SolidAttribution(102, "Alpha II", 1.20m, 1.1m, false),
                        GasAttribution(103, "Alpha III", "hydrogen")
                    })
                };
                using var resolver = new PreviewResolutionCoordinator(
                    new PreviewSessionLifecycle(),
                    new PreviewScanCoordinator(previewGateway),
                    new CompleteClusterRawCoordinator(new FakeCompleteClusterGateway()),
                    new CompleteClusterConclusionCache(path));

                resolver.ObserveCompletedLoad(1, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt scanned = resolver.CurrentPublishedAttempt!;
                Equal(PreviewResolutionState.Scanning, scanned.State);
                while (!scanned.IsTerminal)
                    resolver.AdvanceCurrent();
                Equal(PreviewResolutionState.Complete, scanned.State);
                PreviewConclusionPresentation complete =
                    PreviewConclusionPresenter.Project(scanned);
                Equal(
                    "Fresh start,Megafactory,Compact expansion,Sphere / energy",
                    String.Join(",", complete.ImmediateGroups
                        .Concat(complete.DetailGroups)
                        .Select(group => group.Title)
                        .Distinct(StringComparer.Ordinal)
                        .OrderBy(title => title == "Fresh start" ? 0 :
                            title == "Megafactory" ? 1 :
                            title == "Compact expansion" ? 2 : 3)));
                Equal("Dark Fog: 40 initial hives; 1 in starter system",
                    complete.DarkFogStatusLine);
                True(CompleteContextText(complete, ConclusionContext.FreshStart)
                    .Contains("Starter gas giant has Hydrogen", StringComparison.Ordinal));
                True(CompleteContextText(complete, ConclusionContext.Megafactory)
                    .Contains("outshines all", StringComparison.Ordinal));
                True(CompleteContextText(complete, ConclusionContext.CompactExpansion)
                    .Contains("Short routes", StringComparison.Ordinal));
                True(CompleteContextText(complete, ConclusionContext.SphereShowcase)
                    .Contains("Grand shell", StringComparison.Ordinal));

                string rendered = CompletePresentationText(complete);
                foreach (string forbidden in new[]
                {
                    "TRAIT-", "DF-", "Decision-relevant traits",
                    "Dark Fog farming", "@", "+N", ":star:",
                    "runtime-amount", "orbit distance", "radius-units"
                })
                {
                    False(rendered.Contains(forbidden, StringComparison.OrdinalIgnoreCase));
                }

                resolver.ObserveCompletedLoad(2, PreviewIdentity(16_315_224), Request());
                PreviewResolutionAttempt cachedAttempt = resolver.CurrentPublishedAttempt!;
                Equal(PreviewResolutionState.Cached, cachedAttempt.State);
                PreviewConclusionPresentation cached =
                    PreviewConclusionPresenter.Project(cachedAttempt);
                True(cached.IsCached);
                Equal(rendered, CompletePresentationText(cached));
            });
        }

        private static string CompleteContextText(
            PreviewConclusionPresentation presentation,
            ConclusionContext context) => String.Join("\n", presentation.ImmediateGroups
                .Concat(presentation.DetailGroups)
                .Where(group => group.Context == context)
                .SelectMany(group => group.Cards)
                .Select(card => card.Line));

        private static string CompletePresentationText(
            PreviewConclusionPresentation presentation) => String.Join("\n",
                presentation.ImmediateGroups
                    .Concat(presentation.DetailGroups)
                    .SelectMany(group => group.Cards.Select(card =>
                        ((int)group.Context).ToString() + "|" +
                        ((int)card.Stage).ToString() + "|" +
                        ((int)card.Outcome).ToString() + "|" + card.Line)));

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
                typeof(RuntimeSystemCandidate),
                typeof(RuntimeSystemCandidates),
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
                typeof(PreviewPanelPlacement),
                typeof(PreviewPanelView),
                typeof(PreviewPanelController),
                typeof(RuntimeHomeSystemBodyEvidence),
                typeof(HomeSystemBody),
                typeof(HomeSystemBodyInventory),
                typeof(HomeSystemBodyResources),
                typeof(HomeSystemResourceStatistics),
                typeof(ClusterBodyLocation),
                typeof(PreviewStatisticItem),
                typeof(PreviewStatisticSubsection),
                typeof(PreviewClusterStatistics),
                typeof(PreviewStatisticsDocument),
                typeof(PreviewStatisticsPanelController),
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

        private static PreviewScanRequest RequestForSeed(
            int seed,
            CombatMode combatMode = CombatMode.Combat)
        {
            return new PreviewScanRequest(
                seed,
                ConclusionDefinition.ReferenceStarCount,
                ConclusionDefinition.ReferenceGameVersion,
                1m,
                combatMode,
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

        private static RuntimePlanetOrbitEvidence Orbit(
            int planetId,
            string systemIdentifier,
            int planetNumber,
            bool isSolid,
            bool isGiant,
            int orbitAround,
            int? parentPlanetId)
        {
            return new RuntimePlanetOrbitEvidence(
                planetId,
                systemIdentifier,
                planetNumber,
                isSolid,
                isGiant,
                orbitAround,
                parentPlanetId);
        }

        private static IReadOnlyList<PresentedConclusionCard> FreshCards(
            IReadOnlyList<ConclusionReport> reports,
            IEnumerable<NormalizedBirthPlanetEvidence>? planets,
            NormalizedHomePlanetTopology? topology)
        {
            MethodInfo build = typeof(PreviewConclusionPresenter).GetMethod(
                "BuildFreshStartCards",
                BindingFlags.Static | BindingFlags.NonPublic) ??
                throw new InvalidOperationException(
                    "Fresh start presentation composer was not found.");
            return (IReadOnlyList<PresentedConclusionCard>)(build.Invoke(
                null,
                new object?[] { reports, planets?.ToArray(), topology }) ??
                throw new InvalidOperationException(
                    "Fresh start presentation composer returned no cards."));
        }

        private static IReadOnlyList<string> FreshLines(
            IReadOnlyList<ConclusionReport> reports,
            IEnumerable<NormalizedBirthPlanetEvidence>? planets,
            NormalizedHomePlanetTopology? topology)
        {
            return FreshCards(reports, planets, topology)
                .Select(card => card.Line)
                .ToArray();
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

        private static RuntimeFilesystemFixture CreateRuntimeFilesystem(
            string parent,
            string name)
        {
            string gameRoot = Path.Combine(parent, name);
            string managed = Path.Combine(gameRoot, "DSPGAME_Data", "Managed");
            string bepInEx = Path.Combine(gameRoot, "BepInEx");
            string patchers = Path.Combine(bepInEx, "patchers");
            string plugins = Path.Combine(bepInEx, "plugins", "DSPSeedScanner");
            string config = Path.Combine(bepInEx, "config");
            Directory.CreateDirectory(managed);
            Directory.CreateDirectory(patchers);
            Directory.CreateDirectory(plugins);
            Directory.CreateDirectory(config);
            string executable = Path.Combine(gameRoot, "DSPGAME.exe");
            string assembly = Path.Combine(managed, "Assembly-CSharp.dll");
            string plugin = Path.Combine(plugins, "DSPSeedScanner.dll");
            File.WriteAllBytes(executable, new byte[] { 1 });
            File.WriteAllBytes(assembly, new byte[] { 2 });
            File.WriteAllBytes(plugin, new byte[] { 3 });
            return new RuntimeFilesystemFixture(
                gameRoot,
                executable,
                assembly,
                plugin,
                patchers,
                config);
        }

        private sealed class RuntimeFilesystemFixture
        {
            public RuntimeFilesystemFixture(
                string gameRootPath,
                string executablePath,
                string managedAssemblyPath,
                string pluginAssemblyPath,
                string patcherDirectoryPath,
                string configurationDirectoryPath)
            {
                GameRootPath = Path.GetFullPath(gameRootPath);
                ExecutablePath = Path.GetFullPath(executablePath);
                ManagedAssemblyPath = Path.GetFullPath(managedAssemblyPath);
                PluginAssemblyPath = Path.GetFullPath(pluginAssemblyPath);
                PatcherDirectoryPath = Path.GetFullPath(patcherDirectoryPath);
                ConfigurationDirectoryPath = Path.GetFullPath(configurationDirectoryPath);
            }

            public string GameRootPath { get; }
            public string ExecutablePath { get; }
            public string ManagedAssemblyPath { get; }
            public string PluginAssemblyPath { get; }
            public string PatcherDirectoryPath { get; }
            public string ConfigurationDirectoryPath { get; }
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
            IEnumerable<string>? patchers = null,
            string? scannerCompatibility = null,
            string? scannerContract = null)
        {
            return new RuntimeFingerprint(
                gameVersion ?? ConclusionDefinition.ReferenceGameVersion,
                algorithm ?? ConclusionDefinition.ReferenceGalaxyAlgorithm,
                assembly ?? ConclusionDefinition.ReferenceAssemblySha256,
                themes ?? ConclusionDefinition.ReferenceOrderedThemeIds.Split(','),
                scannerCompatibility ?? ConclusionDefinition.DefinitionVersion,
                scannerContract ?? ConclusionDefinition.ContractVersion,
                members,
                missing,
                mods,
                methodIl ?? ConclusionDefinition.ReferenceGenerationMethodIlSha256,
                patchers);
        }

        private static RuntimePreviewSnapshot Snapshot(
            int generatedStarCount = 64,
            IEnumerable<NormalizedBirthPlanetEvidence>? birthPlanetAttributions = null,
            IReadOnlyDictionary<int, (decimal Energy, long Radius, int Orbits)>?
                systemCandidateFacts = null,
            int? missingEnergySystem = null,
            int birthInitialHiveCount = 1,
            int otherInitialHiveCount = 39,
            int? missingHiveSystem = null,
            decimal primaryDistanceLy = 2m,
            bool includeHomePlanetTopology = true,
            HomeSystemBodyInventory? homeSystemBodyInventory = null,
            string? homePlanetDisplayDesignation = null)
        {
            var systems = new List<NormalizedSystemEvidence>();
            for (int index = 0; index < generatedStarCount; index++)
            {
                bool birth = index == 0;
                bool leader = index == 1;
                int systemId = index + 1;
                decimal energy = leader ? 2.698m : 1m;
                long radius = leader ? 234_200 : 50_000;
                int orbits = leader ? 4 : 0;
                if (systemCandidateFacts != null &&
                    systemCandidateFacts.TryGetValue(
                        systemId,
                        out (decimal Energy, long Radius, int Orbits) facts))
                {
                    energy = facts.Energy;
                    radius = facts.Radius;
                    orbits = facts.Orbits;
                }
                systems.Add(new NormalizedSystemEvidence(
                    new ConclusionSubject(
                        birth ? SubjectKind.BirthSystem : SubjectKind.StarSystem,
                        (index + 1).ToString()),
                    birth,
                    birth ? includeHomePlanetTopology ? 3 : null : null,
                    birth ? true : null,
                    birth ? 1.35m : null,
                    birth ? 1.5m : null,
                    birth ? new[] { new NormalizedGasProduct("hydrogen", 0.5m) } : null,
                    systemId == missingEnergySystem ? null : energy,
                    radius,
                    orbits,
                    systemId == missingHiveSystem
                        ? null
                        : birth ? birthInitialHiveCount : leader ? otherInitialHiveCount : 0,
                    birth ? birthPlanetAttributions : null,
                    birth && includeHomePlanetTopology
                        ? new NormalizedHomePlanetTopology(
                            birthPlanetAttributions?.FirstOrDefault(value => !value.IsGasGiant)
                                ?.PlanetId ?? 101,
                            HomePlanetOrbitKind.GiantMoon,
                            3)
                        : null));
            }

            var distances = new List<NormalizedSystemDistance>();
            for (int first = 0; first < generatedStarCount; first++)
            {
                for (int second = first + 1; second < generatedStarCount; second++)
                {
                    distances.Add(new NormalizedSystemDistance(
                        (first + 1).ToString(),
                        (second + 1).ToString(),
                        first == 0 && second == 1 ? primaryDistanceLy : 20m));
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
                        index == 2 ? "O type star" : "G type star")),
                homeSystemBodyInventory: homeSystemBodyInventory,
                homePlanetDisplayDesignation: homePlanetDisplayDesignation);
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

        private static string CandidateIds(
            IReadOnlyList<RuntimeSystemCandidate>? candidates)
        {
            True(candidates != null);
            return String.Join(",", candidates!.Select(value => value.Identifier));
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

        private sealed class SingleEnumerationEnumerable<T> : IEnumerable<T>
        {
            private readonly IEnumerable<T> source;

            public SingleEnumerationEnumerable(IEnumerable<T> source)
            {
                this.source = source;
            }

            public int EnumerationCount { get; private set; }

            public IEnumerator<T> GetEnumerator()
            {
                EnumerationCount++;
                if (EnumerationCount > 1)
                    throw new InvalidOperationException("Evidence was enumerated more than once.");
                return source.GetEnumerator();
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() =>
                GetEnumerator();
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
                    bool birth = index == 0;
                    targets.Add(new CompleteClusterPlanetTarget(
                        planetId,
                        index + 1,
                        new ConclusionSubject(
                            birth ? SubjectKind.BirthSystem : SubjectKind.StarSystem,
                            birth ? Snapshot().BirthSystemIdentifier : (index + 1).ToString()),
                        birth ? 0m : index == 1 ? 2m : 20m));
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
                private CompleteClusterPlanetTarget? pendingTarget;
                private int pendingPolls;
                private bool disposed;

                public Session(
                    FakeCompleteClusterGateway owner,
                    int galaxySeed,
                    Action<string> recordTrace)
                {
                    this.owner = owner;
                    this.galaxySeed = galaxySeed;
                    this.recordTrace = recordTrace;
                    StateRestored = false;
                }

                public bool StateRestored { get; private set; }

                public void StartPlanet(
                    CompleteClusterPlanetTarget target,
                    CancellationToken cancellationToken,
                    Action<string> stepTrace)
                {
                    if (disposed)
                        throw new ObjectDisposedException(nameof(Session));
                    cancellationToken.ThrowIfCancellationRequested();
                    if (pendingTarget != null)
                        throw new InvalidOperationException("fake scan already pending");
                    pendingTarget = target;
                    pendingPolls = 0;
                    stepTrace("raw:terrain-worker:start:planet=" + target.PlanetId);
                }

                public bool TryCompletePlanet(
                    CompleteClusterPlanetTarget target,
                    CancellationToken cancellationToken,
                    Action<string> stepTrace,
                    out NormalizedRawPlanetEvidence? evidence)
                {
                    evidence = null;
                    if (disposed)
                        throw new ObjectDisposedException(nameof(Session));
                    cancellationToken.ThrowIfCancellationRequested();
                    if (pendingTarget?.PlanetId != target.PlanetId)
                        throw new InvalidOperationException("fake scan target mismatch");
                    if (pendingPolls++ == 0)
                        return false;
                    owner.StateMarker = "candidate";
                    try
                    {
                        if (owner.GenerationFailure != null)
                            throw owner.GenerationFailure;
                        owner.OnPlanet?.Invoke();
                        if (owner.FailingPlanetId == target.PlanetId)
                            throw new InvalidOperationException("injected cluster planet failure");
                        evidence = target.PlanetId switch
                        {
                            102 => ClusterSnapshot(galaxySeed, target, "kimberlite", 9, 1128, 200),
                            103 => ClusterSnapshot(galaxySeed, target, "unipolar-magnet", 14, 1016, 300),
                            _ => ClusterSnapshot(galaxySeed, target, null, 0, 0, 0)
                        };
                        pendingTarget = null;
                        stepTrace("raw:terrain-worker:complete");
                        return true;
                    }
                    finally
                    {
                        owner.StateMarker = "leased";
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
