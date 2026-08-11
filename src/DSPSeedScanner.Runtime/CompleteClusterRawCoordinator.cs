using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public sealed class CompleteClusterRawCoordinator
    {
        public const string Stage = "complete-cluster-raw";
        public const int MaximumSolidPlanets = 256;

        private readonly IRuntimeCompleteClusterRawGateway gateway;
        private readonly RuntimeOperationGate operationGate;

        public CompleteClusterRawCoordinator(
            IRuntimeCompleteClusterRawGateway gateway,
            RuntimeOperationGate? operationGate = null)
        {
            this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            this.operationGate = operationGate ?? new RuntimeOperationGate();
        }

        public CompleteClusterRawResult TryGenerate(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<CompleteClusterRawProgress>? reportProgress = null)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var stopwatch = Stopwatch.StartNew();
            long initialMemory = GC.GetTotalMemory(false);
            var trace = new List<string>();
            var progress = new List<CompleteClusterRawProgress>();
            if (!operationGate.TryEnter())
            {
                return Result(
                    RuntimeScanStatus.Busy,
                    request,
                    "busy",
                    "Another runtime request is active.",
                    null,
                    0,
                    0,
                    progress,
                    null,
                    null,
                    trace,
                    true,
                    stopwatch.ElapsedMilliseconds,
                    GC.GetTotalMemory(false) - initialMemory);
            }

            RuntimeFingerprint? fingerprint = null;
            RuntimeStateLease? lease = null;
            RuntimeScanStatus status = RuntimeScanStatus.Failed;
            string code = "runtime-failure";
            string message = "The complete-cluster raw request failed.";
            string? rawDiagnostic = null;
            IReadOnlyList<NormalizedRareResourceEvidence>? rareResources = null;
            IReadOnlyList<ConclusionReport>? reports = null;
            int expected = 0;
            int completed = 0;
            int? affectedPlanet = null;
            bool restored = true;

            try
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                trace.Add("request:thread=" + threadId);
                if (threadId != gateway.MainThreadId)
                {
                    status = RuntimeScanStatus.Incompatible;
                    code = "thread-affinity-mismatch";
                    message = "Runtime generation must execute on the captured Unity main thread.";
                }
                else
                {
                    fingerprint = gateway.CaptureFingerprint(request);
                    trace.Add("fingerprint:capture");
                    CompatibilityDecision compatibility = CompatibilityPolicy.Evaluate(fingerprint);
                    CompatibilityDecision requestCompatibility =
                        CompatibilityPolicy.EvaluateRequest(request);
                    if (!compatibility.Supported)
                    {
                        status = RuntimeScanStatus.Incompatible;
                        code = compatibility.Code;
                        message = compatibility.Message;
                    }
                    else if (!requestCompatibility.Supported)
                    {
                        status = RuntimeScanStatus.Incompatible;
                        code = requestCompatibility.Code;
                        message = requestCompatibility.Message;
                    }
                    else
                    {
                        lease = gateway.CaptureState();
                        trace.Add("state:capture");
                        cancellationToken.ThrowIfCancellationRequested();
                        CompleteClusterRawPlan plan = gateway.DiscoverCompleteCluster(
                            request,
                            cancellationToken,
                            trace.Add);
                        ValidatePreview(request, plan.Preview);
                        expected = plan.Targets.Count;
                        if (expected > MaximumSolidPlanets)
                        {
                            throw new RawCompatibilityException(
                                "complete-cluster-planet-bound-exceeded",
                                "The generated cluster exceeds the certified single-operation planet bound.",
                                "expected=" + expected);
                        }

                        Publish(new CompleteClusterRawProgress(
                            CompleteClusterProgressState.Planned,
                            expected,
                            0,
                            null));
                        var aggregate = new ClusterAggregate();
                        gateway.GenerateCompleteCluster(
                            request,
                            plan,
                            cancellationToken,
                            target =>
                            {
                                affectedPlanet = target.PlanetId;
                                Publish(new CompleteClusterRawProgress(
                                    CompleteClusterProgressState.PlanetStarted,
                                    expected,
                                    completed,
                                    target.PlanetId));
                            },
                            (target, evidence) =>
                            {
                                ValidatePlanet(request, target, evidence);
                                aggregate.Add(target, evidence);
                                completed++;
                                Publish(new CompleteClusterRawProgress(
                                    CompleteClusterProgressState.PlanetCompleted,
                                    expected,
                                    completed,
                                    target.PlanetId));
                            },
                            trace.Add);

                        if (completed != expected)
                            throw new InvalidOperationException("Complete-cluster generation omitted declared planets.");

                        rareResources = aggregate.RareResources();
                        var rareCoverage = Complete(
                            EvidenceScope.CompleteClusterRareResources,
                            expected);
                        var resourceCoverage = Complete(
                            EvidenceScope.CompleteClusterResources,
                            expected);
                        reports = RuntimeConclusionEvaluator.Evaluate(
                            request,
                            fingerprint,
                            plan.Preview,
                            rareResources: rareResources,
                            rareCoverage: rareCoverage,
                            clusterCommonResourceTotal: aggregate.CommonTotal,
                            clusterResourceCoverage: resourceCoverage);
                        status = RuntimeScanStatus.Success;
                        code = "success";
                        message = "Every solid planet was generated and exact rare access was evaluated.";
                        affectedPlanet = null;
                        trace.Add("evaluation:complete");
                    }
                }
            }
            catch (OperationCanceledException)
            {
                status = RuntimeScanStatus.Cancelled;
                code = "cancelled";
                message = "The complete-cluster raw request was cancelled at a planet boundary.";
                trace.Add("request:cancelled");
            }
            catch (RawCompatibilityException exception)
            {
                status = RuntimeScanStatus.Incompatible;
                code = exception.Code;
                message = exception.Message;
                rawDiagnostic = exception.RawDiagnostic;
                trace.Add("request:incompatible");
            }
            catch (Exception exception)
            {
                status = RuntimeScanStatus.Failed;
                code = "runtime-exception";
                message = exception.GetType().Name + ": " + exception.Message;
                trace.Add("request:failed");
            }
            finally
            {
                if (lease != null)
                {
                    try
                    {
                        lease.Dispose();
                        restored = lease.Restored;
                    }
                    catch (Exception exception)
                    {
                        restored = false;
                        message = exception.GetType().Name + ": " + exception.Message;
                    }
                    trace.Add("state:restore=" + restored);
                    if (!restored)
                    {
                        status = RuntimeScanStatus.Failed;
                        code = "state-restoration-failed";
                    }
                }
                operationGate.Exit();
                stopwatch.Stop();
            }

            if (status != RuntimeScanStatus.Success)
            {
                rareResources = null;
                reports = null;
            }
            return Result(
                status,
                request,
                code,
                message,
                fingerprint,
                expected,
                completed,
                progress,
                rareResources,
                reports,
                trace,
                restored,
                stopwatch.ElapsedMilliseconds,
                GC.GetTotalMemory(false) - initialMemory,
                affectedPlanet,
                rawDiagnostic);

            void Publish(CompleteClusterRawProgress value)
            {
                progress.Add(value);
                reportProgress?.Invoke(value);
            }
        }

        private static void ValidatePreview(
            PreviewScanRequest request,
            RuntimePreviewSnapshot preview)
        {
            if (preview.UnknownEnumValue.HasValue)
            {
                throw new RawCompatibilityException(
                    "unknown-runtime-enum",
                    "The runtime returned an enum value outside the supported contract.",
                    preview.UnknownEnumType + "=" + preview.UnknownEnumValue.Value);
            }
            if (preview.GeneratedStarCount != request.RequestedStarCount ||
                preview.Systems.Count != preview.GeneratedStarCount ||
                preview.SystemDistances.Count !=
                    preview.GeneratedStarCount * (preview.GeneratedStarCount - 1) / 2)
            {
                throw new InvalidOperationException(
                    "The complete-cluster plan did not include a complete normalized preview.");
            }
        }

        private static void ValidatePlanet(
            PreviewScanRequest request,
            CompleteClusterPlanetTarget target,
            NormalizedRawPlanetEvidence evidence)
        {
            if (evidence.GalaxySeed != request.GalaxySeed ||
                evidence.PlanetId != target.PlanetId ||
                evidence.AlgorithmId != target.AlgorithmId ||
                !evidence.Coverage.IsComplete)
            {
                throw new InvalidOperationException(
                    "Raw evidence did not match its declared complete-cluster target.");
            }
        }

        private static EvidenceCoverage Complete(EvidenceScope scope, int subjects) =>
            new EvidenceCoverage(
                EvidenceStage.CompleteClusterRaw,
                scope,
                CoverageState.Complete,
                subjects,
                subjects);

        private static CompleteClusterRawResult Result(
            RuntimeScanStatus status,
            PreviewScanRequest request,
            string code,
            string message,
            RuntimeFingerprint? fingerprint,
            int expected,
            int completed,
            IEnumerable<CompleteClusterRawProgress> progress,
            IEnumerable<NormalizedRareResourceEvidence>? rareResources,
            IEnumerable<ConclusionReport>? reports,
            IEnumerable<string> trace,
            bool restored,
            long elapsedMilliseconds,
            long memoryDelta,
            int? affectedPlanet = null,
            string? rawDiagnostic = null)
        {
            CoverageState state = expected > 0 && completed == expected
                ? CoverageState.Complete
                : completed > 0 ? CoverageState.Partial : CoverageState.Unavailable;
            return new CompleteClusterRawResult(
                status,
                request.GalaxySeed,
                code,
                message,
                fingerprint,
                new CompleteClusterRawCoverage(state, expected, completed),
                progress,
                rareResources,
                reports,
                trace,
                restored,
                elapsedMilliseconds,
                memoryDelta,
                affectedPlanet,
                rawDiagnostic);
        }

        private sealed class ClusterAggregate
        {
            private readonly Dictionary<string, RareAggregate> rare =
                ConclusionDefinition.RareResourceIds.ToDictionary(
                    resourceId => resourceId,
                    _ => new RareAggregate(),
                    StringComparer.Ordinal);

            public long CommonTotal { get; private set; }

            public void Add(
                CompleteClusterPlanetTarget target,
                NormalizedRawPlanetEvidence planet)
            {
                foreach (NormalizedRawVeinNode node in planet.Nodes)
                {
                    if (ConclusionDefinition.StarterTotalResourceIds.Contains(node.ResourceId))
                        CommonTotal = checked(CommonTotal + node.Amount);
                    if (rare.TryGetValue(node.ResourceId, out RareAggregate? resource))
                    {
                        resource.Amount = checked(resource.Amount + node.Amount);
                        resource.Present = true;
                        if (resource.NearestSystem == null ||
                            target.DistanceFromBirthLy < resource.DistanceFromBirthLy)
                        {
                            resource.NearestSystem = target.System;
                            resource.DistanceFromBirthLy = target.DistanceFromBirthLy;
                        }
                    }
                }
                foreach (NormalizedRawVeinGroup group in planet.Groups)
                {
                    if (rare.TryGetValue(group.ResourceId, out RareAggregate? resource))
                        resource.Groups = checked(resource.Groups + 1);
                }
            }

            public IReadOnlyList<NormalizedRareResourceEvidence> RareResources() =>
                rare.OrderBy(pair => pair.Key, StringComparer.Ordinal)
                    .Select(pair => pair.Value.Present
                        ? new NormalizedRareResourceEvidence(
                            pair.Key,
                            true,
                            pair.Value.NearestSystem,
                            pair.Value.DistanceFromBirthLy,
                            pair.Value.Amount,
                            pair.Value.Groups)
                        : new NormalizedRareResourceEvidence(
                            pair.Key,
                            false,
                            null,
                            null))
                    .ToArray();

            private sealed class RareAggregate
            {
                public bool Present { get; set; }
                public long Amount { get; set; }
                public int Groups { get; set; }
                public ConclusionSubject? NearestSystem { get; set; }
                public decimal DistanceFromBirthLy { get; set; }
            }
        }
    }
}
