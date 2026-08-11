using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public enum CompleteClusterRawOperationState
    {
        Ready,
        Completed
    }

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

        public CompleteClusterRawOperation TryStart(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<CompleteClusterRawProgress>? reportProgress = null)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            return CompleteClusterRawOperation.Start(
                gateway,
                operationGate,
                request,
                cancellationToken,
                reportProgress);
        }

        public CompleteClusterRawResult TryGenerate(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<CompleteClusterRawProgress>? reportProgress = null)
        {
            using CompleteClusterRawOperation operation = TryStart(
                request,
                cancellationToken,
                reportProgress);
            while (operation.State == CompleteClusterRawOperationState.Ready)
                operation.Advance();
            return operation.Result ?? throw new InvalidOperationException(
                "A completed raw operation must expose its result.");
        }
    }

    public sealed class CompleteClusterRawOperation : IDisposable
    {
        private readonly IRuntimeCompleteClusterRawGateway gateway;
        private readonly RuntimeOperationGate operationGate;
        private readonly PreviewScanRequest request;
        private readonly CancellationToken cancellationToken;
        private readonly Action<CompleteClusterRawProgress>? reportProgress;
        private readonly Stopwatch stopwatch = Stopwatch.StartNew();
        private readonly long initialMemory = GC.GetTotalMemory(false);
        private readonly List<string> trace = new List<string>();
        private readonly List<CompleteClusterRawProgress> progress =
            new List<CompleteClusterRawProgress>();
        private readonly ClusterAggregate aggregate = new ClusterAggregate();

        private bool ownsGate;
        private RuntimeFingerprint? fingerprint;
        private RuntimeStateLease? lease;
        private IRuntimeCompleteClusterRawSession? runtimeSession;
        private CompleteClusterRawPlan? plan;
        private int expected;
        private int completed;
        private int? affectedPlanet;
        private string? rawDiagnostic;
        private bool advancing;

        private CompleteClusterRawOperation(
            IRuntimeCompleteClusterRawGateway gateway,
            RuntimeOperationGate operationGate,
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<CompleteClusterRawProgress>? reportProgress)
        {
            this.gateway = gateway;
            this.operationGate = operationGate;
            this.request = request;
            this.cancellationToken = cancellationToken;
            this.reportProgress = reportProgress;
        }

        public CompleteClusterRawOperationState State { get; private set; }
        public CompleteClusterRawResult? Result { get; private set; }
        public int ExpectedPlanets => expected;
        public int CompletedPlanets => completed;
        public bool IsYieldStateRestored { get; private set; } = true;

        internal static CompleteClusterRawOperation Start(
            IRuntimeCompleteClusterRawGateway gateway,
            RuntimeOperationGate operationGate,
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<CompleteClusterRawProgress>? reportProgress)
        {
            var operation = new CompleteClusterRawOperation(
                gateway,
                operationGate,
                request,
                cancellationToken,
                reportProgress);
            operation.Initialize();
            return operation;
        }

        public void Advance()
        {
            if (State != CompleteClusterRawOperationState.Ready)
                throw new InvalidOperationException("The complete-cluster operation is already complete.");
            if (plan == null || runtimeSession == null)
                throw new InvalidOperationException("The complete-cluster operation was not initialized.");
            if (advancing)
                throw new InvalidOperationException("A complete-cluster step cannot be re-entered.");

            advancing = true;
            try
            {
                if (Thread.CurrentThread.ManagedThreadId != gateway.MainThreadId)
                {
                    Finish(
                        RuntimeScanStatus.Incompatible,
                        "thread-affinity-mismatch",
                        "Runtime generation must execute on the captured Unity main thread.");
                    return;
                }

                cancellationToken.ThrowIfCancellationRequested();
                CompleteClusterPlanetTarget target = plan.Targets[completed];
                affectedPlanet = target.PlanetId;
                Publish(new CompleteClusterRawProgress(
                    CompleteClusterProgressState.PlanetStarted,
                    expected,
                    completed,
                    target.PlanetId));

                NormalizedRawPlanetEvidence evidence = runtimeSession.GeneratePlanet(
                    target,
                    cancellationToken,
                    trace.Add);
                IsYieldStateRestored = runtimeSession.StateRestored;
                if (!IsYieldStateRestored)
                    throw new InvalidOperationException("Runtime state was not restored after a planet step.");

                ValidatePlanet(request, target, evidence);
                aggregate.Add(target, evidence);
                completed++;
                Publish(new CompleteClusterRawProgress(
                    CompleteClusterProgressState.PlanetCompleted,
                    expected,
                    completed,
                    target.PlanetId));
                trace.Add("cluster-step:yield:completed=" + completed);

                if (completed == expected)
                    CompleteSuccessfully(plan);
            }
            catch (OperationCanceledException)
            {
                trace.Add("request:cancelled");
                Finish(
                    RuntimeScanStatus.Cancelled,
                    "cancelled",
                    "The complete-cluster raw request was cancelled at a planet boundary.");
            }
            catch (RawCompatibilityException exception)
            {
                rawDiagnostic = exception.RawDiagnostic;
                trace.Add("request:incompatible");
                Finish(RuntimeScanStatus.Incompatible, exception.Code, exception.Message);
            }
            catch (Exception exception)
            {
                trace.Add("request:failed");
                Finish(
                    RuntimeScanStatus.Failed,
                    "runtime-exception",
                    exception.GetType().Name + ": " + exception.Message);
            }
            finally
            {
                advancing = false;
            }
        }

        public void Dispose()
        {
            if (State == CompleteClusterRawOperationState.Ready)
            {
                trace.Add("request:disposed");
                Finish(
                    RuntimeScanStatus.Cancelled,
                    "cancelled",
                    "The complete-cluster raw request was cancelled before completion.");
            }
        }

        private void Initialize()
        {
            if (!operationGate.TryEnter())
            {
                Finish(
                    RuntimeScanStatus.Busy,
                    "busy",
                    "Another runtime request is active.");
                return;
            }
            ownsGate = true;

            try
            {
                int threadId = Thread.CurrentThread.ManagedThreadId;
                trace.Add("request:thread=" + threadId);
                if (threadId != gateway.MainThreadId)
                {
                    Finish(
                        RuntimeScanStatus.Incompatible,
                        "thread-affinity-mismatch",
                        "Runtime generation must execute on the captured Unity main thread.");
                    return;
                }

                fingerprint = gateway.CaptureFingerprint(request);
                trace.Add("fingerprint:capture");
                CompatibilityDecision compatibility = CompatibilityPolicy.Evaluate(fingerprint);
                CompatibilityDecision requestCompatibility =
                    CompatibilityPolicy.EvaluateRequest(request);
                if (!compatibility.Supported)
                {
                    Finish(RuntimeScanStatus.Incompatible, compatibility.Code, compatibility.Message);
                    return;
                }
                if (!requestCompatibility.Supported)
                {
                    Finish(
                        RuntimeScanStatus.Incompatible,
                        requestCompatibility.Code,
                        requestCompatibility.Message);
                    return;
                }

                lease = gateway.CaptureState();
                trace.Add("state:capture");
                cancellationToken.ThrowIfCancellationRequested();
                plan = gateway.DiscoverCompleteCluster(
                    request,
                    cancellationToken,
                    trace.Add);
                ValidatePreview(request, plan.Preview);
                expected = plan.Targets.Count;
                if (expected > CompleteClusterRawCoordinator.MaximumSolidPlanets)
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
                runtimeSession = gateway.OpenCompleteCluster(
                    request,
                    plan,
                    cancellationToken,
                    trace.Add);
                IsYieldStateRestored = runtimeSession.StateRestored;
                if (!IsYieldStateRestored)
                    throw new InvalidOperationException("Runtime state was not restored after raw-session setup.");
                State = CompleteClusterRawOperationState.Ready;
            }
            catch (OperationCanceledException)
            {
                trace.Add("request:cancelled");
                Finish(
                    RuntimeScanStatus.Cancelled,
                    "cancelled",
                    "The complete-cluster raw request was cancelled at a planet boundary.");
            }
            catch (RawCompatibilityException exception)
            {
                rawDiagnostic = exception.RawDiagnostic;
                trace.Add("request:incompatible");
                Finish(RuntimeScanStatus.Incompatible, exception.Code, exception.Message);
            }
            catch (Exception exception)
            {
                trace.Add("request:failed");
                Finish(
                    RuntimeScanStatus.Failed,
                    "runtime-exception",
                    exception.GetType().Name + ": " + exception.Message);
            }
        }

        private void CompleteSuccessfully(CompleteClusterRawPlan completedPlan)
        {
            if (completed != expected)
                throw new InvalidOperationException("Complete-cluster generation omitted declared planets.");

            IReadOnlyList<NormalizedRareResourceEvidence> rareResources =
                aggregate.RareResources();
            EvidenceCoverage rareCoverage = Complete(
                EvidenceScope.CompleteClusterRareResources,
                expected);
            EvidenceCoverage resourceCoverage = Complete(
                EvidenceScope.CompleteClusterResources,
                expected);
            IReadOnlyList<ConclusionReport> reports = RuntimeConclusionEvaluator.Evaluate(
                request,
                fingerprint ?? throw new InvalidOperationException("A successful scan requires a fingerprint."),
                completedPlan.Preview,
                rareResources: rareResources,
                rareCoverage: rareCoverage,
                clusterCommonResourceTotal: aggregate.CommonTotal,
                clusterResourceCoverage: resourceCoverage);
            affectedPlanet = null;
            trace.Add("evaluation:complete");
            Finish(
                RuntimeScanStatus.Success,
                "success",
                "Every solid planet was generated and exact rare access was evaluated.",
                rareResources,
                reports);
        }

        private void Finish(
            RuntimeScanStatus status,
            string code,
            string message,
            IReadOnlyList<NormalizedRareResourceEvidence>? rareResources = null,
            IReadOnlyList<ConclusionReport>? reports = null)
        {
            if (State == CompleteClusterRawOperationState.Completed)
                return;

            bool restored = true;
            Exception? restorationFailure = null;
            if (runtimeSession != null)
            {
                try
                {
                    runtimeSession.Dispose();
                    restored &= runtimeSession.StateRestored;
                }
                catch (Exception exception)
                {
                    restored = false;
                    restorationFailure = exception;
                }
                runtimeSession = null;
            }

            if (lease != null)
            {
                try
                {
                    lease.Dispose();
                    restored &= lease.Restored;
                }
                catch (Exception exception)
                {
                    restored = false;
                    if (restorationFailure == null)
                        restorationFailure = exception;
                }
                trace.Add("state:restore=" + restored);
                lease = null;
            }

            if (ownsGate)
            {
                operationGate.Exit();
                ownsGate = false;
            }
            stopwatch.Stop();

            if (!restored)
            {
                status = RuntimeScanStatus.Failed;
                code = "state-restoration-failed";
                message = restorationFailure == null
                    ? "Runtime state restoration failed."
                    : restorationFailure.GetType().Name + ": " + restorationFailure.Message;
            }
            if (status != RuntimeScanStatus.Success)
            {
                rareResources = null;
                reports = null;
            }

            Result = BuildResult(
                status,
                code,
                message,
                restored,
                rareResources,
                reports);
            State = CompleteClusterRawOperationState.Completed;
        }

        private CompleteClusterRawResult BuildResult(
            RuntimeScanStatus status,
            string code,
            string message,
            bool restored,
            IEnumerable<NormalizedRareResourceEvidence>? rareResources,
            IEnumerable<ConclusionReport>? reports)
        {
            CoverageState coverageState = expected > 0 && completed == expected
                ? CoverageState.Complete
                : completed > 0 ? CoverageState.Partial : CoverageState.Unavailable;
            return new CompleteClusterRawResult(
                status,
                request.GalaxySeed,
                code,
                message,
                fingerprint,
                new CompleteClusterRawCoverage(coverageState, expected, completed),
                progress,
                rareResources,
                reports,
                trace,
                restored,
                stopwatch.ElapsedMilliseconds,
                GC.GetTotalMemory(false) - initialMemory,
                affectedPlanet,
                rawDiagnostic);
        }

        private void Publish(CompleteClusterRawProgress value)
        {
            progress.Add(value);
            reportProgress?.Invoke(value);
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
