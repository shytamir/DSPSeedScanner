using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public sealed class BirthSystemRawCoordinator
    {
        public const string Stage = "birth-system-raw";

        private readonly IRuntimeBirthSystemRawGateway gateway;
        private readonly RuntimeOperationGate operationGate;

        public BirthSystemRawCoordinator(
            IRuntimeBirthSystemRawGateway gateway,
            RuntimeOperationGate? operationGate = null)
        {
            this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            this.operationGate = operationGate ?? new RuntimeOperationGate();
        }

        public BirthSystemRawResult TryGenerate(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<BirthSystemRawProgress>? reportProgress = null)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var trace = new List<string>();
            var progress = new List<BirthSystemRawProgress>();
            if (!operationGate.TryEnter())
                return Result(RuntimeScanStatus.Busy, request, "busy", "Another runtime request is active.", null, 0, 0, progress, null, trace, true);

            RuntimeFingerprint? fingerprint = null;
            RuntimeStateLease? lease = null;
            RuntimeScanStatus status = RuntimeScanStatus.Failed;
            string code = "runtime-failure";
            string message = "The birth-system raw request failed.";
            string? rawDiagnostic = null;
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
                    if (!compatibility.Supported)
                    {
                        status = RuntimeScanStatus.Incompatible;
                        code = compatibility.Code;
                        message = compatibility.Message;
                    }
                    else
                    {
                        CompatibilityDecision requestCompatibility = CompatibilityPolicy.EvaluateRequest(request);
                        if (!requestCompatibility.Supported)
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
                            BirthSystemRawPlan plan = gateway.DiscoverBirthSystem(request, cancellationToken, trace.Add);
                            ValidatePreview(request, plan.Preview);
                            expected = plan.Targets.Count;
                            Publish(new BirthSystemRawProgress(BirthSystemProgressState.Planned, expected, 0, null));

                            var planets = new List<NormalizedRawPlanetEvidence>();
                            foreach (BirthSystemPlanetTarget target in plan.Targets)
                            {
                                affectedPlanet = target.PlanetId;
                                cancellationToken.ThrowIfCancellationRequested();
                                Publish(new BirthSystemRawProgress(BirthSystemProgressState.PlanetStarted, expected, completed, target.PlanetId));
                                NormalizedRawPlanetEvidence planet = gateway.GenerateRawPlanet(
                                    new RawPlanetRequest(request, target.PlanetId, target.AlgorithmId),
                                    cancellationToken,
                                    trace.Add);
                                if (planet.GalaxySeed != request.GalaxySeed ||
                                    planet.PlanetId != target.PlanetId ||
                                    planet.AlgorithmId != target.AlgorithmId ||
                                    !planet.Coverage.IsComplete)
                                {
                                    throw new InvalidOperationException("Raw planet evidence did not match its declared birth-system target.");
                                }
                                planets.Add(planet);
                                completed++;
                                Publish(new BirthSystemRawProgress(BirthSystemProgressState.PlanetCompleted, expected, completed, target.PlanetId));
                            }

                            NormalizedStarterResourceEvidence starter = Aggregate(plan.Preview, planets);
                            var coverage = new EvidenceCoverage(
                                EvidenceStage.BirthSystemRaw,
                                EvidenceScope.BirthSystemResources,
                                CoverageState.Complete,
                                expected,
                                completed);
                            reports = RuntimeConclusionEvaluator.Evaluate(
                                request,
                                fingerprint,
                                plan.Preview,
                                starter,
                                coverage);
                            status = RuntimeScanStatus.Success;
                            code = "success";
                            message = "Every solid birth-system planet was generated and evaluated.";
                            affectedPlanet = null;
                            trace.Add("evaluation:complete");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                status = RuntimeScanStatus.Cancelled;
                code = "cancelled";
                message = "The birth-system raw request was cancelled between planets.";
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
            }

            if (status != RuntimeScanStatus.Success)
                reports = null;
            return Result(status, request, code, message, fingerprint, expected, completed, progress, reports, trace, restored, affectedPlanet, rawDiagnostic);

            void Publish(BirthSystemRawProgress value)
            {
                progress.Add(value);
                reportProgress?.Invoke(value);
            }
        }

        private static void ValidatePreview(PreviewScanRequest request, RuntimePreviewSnapshot preview)
        {
            if (preview.UnknownEnumValue.HasValue)
                throw new RawCompatibilityException("unknown-runtime-enum", "The runtime returned an enum value outside the supported contract.", preview.UnknownEnumType + "=" + preview.UnknownEnumValue.Value);
            if (preview.GeneratedStarCount != request.RequestedStarCount ||
                preview.Systems.Count != preview.GeneratedStarCount ||
                preview.SystemDistances.Count != preview.GeneratedStarCount * (preview.GeneratedStarCount - 1) / 2)
            {
                throw new InvalidOperationException("The birth-system plan did not include a complete normalized preview.");
            }
        }

        private static NormalizedStarterResourceEvidence Aggregate(
            RuntimePreviewSnapshot preview,
            IEnumerable<NormalizedRawPlanetEvidence> planets)
        {
            NormalizedRawPlanetEvidence[] evidence = planets.ToArray();
            StarterResourceMetric[] metrics = ConclusionDefinition.CommonResourceIds
                .Select(resourceId => new StarterResourceMetric(
                    resourceId,
                    checked(evidence.SelectMany(planet => planet.Nodes)
                        .Where(node => String.Equals(node.ResourceId, resourceId, StringComparison.Ordinal))
                        .Sum(node => node.Amount)),
                    checked(evidence.SelectMany(planet => planet.Groups)
                        .Count(group => String.Equals(group.ResourceId, resourceId, StringComparison.Ordinal)))))
                .ToArray();
            bool fireIce = evidence.SelectMany(planet => planet.Nodes)
                .Any(node => String.Equals(node.ResourceId, "fire-ice", StringComparison.Ordinal));
            return new NormalizedStarterResourceEvidence(
                new ConclusionSubject(SubjectKind.BirthSystem, preview.BirthSystemIdentifier),
                metrics,
                fireIce);
        }

        private static BirthSystemRawResult Result(
            RuntimeScanStatus status,
            PreviewScanRequest request,
            string code,
            string message,
            RuntimeFingerprint? fingerprint,
            int expected,
            int completed,
            IEnumerable<BirthSystemRawProgress> progress,
            IEnumerable<ConclusionReport>? reports,
            IEnumerable<string> trace,
            bool restored,
            int? affectedPlanet = null,
            string? rawDiagnostic = null)
        {
            CoverageState state = expected > 0 && completed == expected
                ? CoverageState.Complete
                : completed > 0 ? CoverageState.Partial : CoverageState.Unavailable;
            return new BirthSystemRawResult(
                status, request.GalaxySeed, code, message, fingerprint,
                new BirthSystemRawCoverage(state, expected, completed), progress, reports, trace,
                restored, affectedPlanet, rawDiagnostic);
        }
    }
}
