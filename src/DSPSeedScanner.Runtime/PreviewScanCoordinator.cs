using System;
using System.Collections.Generic;
using System.Threading;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public enum RuntimeScanStatus
    {
        Success,
        Busy,
        Cancelled,
        Incompatible,
        Failed
    }

    public sealed class RuntimeScanResult
    {
        public RuntimeScanResult(
            RuntimeScanStatus status,
            int galaxySeed,
            string stage,
            string code,
            string message,
            RuntimeFingerprint? fingerprint,
            IEnumerable<ConclusionReport>? reports,
            IEnumerable<string> trace,
            bool stateRestored,
            int? generatedStarCount = null,
            string? rawDiagnostic = null,
            IEnumerable<RuntimeSystemDisplay>? systemDisplays = null,
            IEnumerable<NormalizedBirthPlanetEvidence>? birthPlanetAttributions = null,
            NormalizedHomePlanetTopology? homePlanetTopology = null,
            RuntimeSystemCandidates? systemCandidates = null,
            RuntimeDarkFogOccupation? darkFogOccupation = null)
        {
            Status = status;
            GalaxySeed = galaxySeed;
            Stage = stage;
            Code = code;
            Message = message;
            Fingerprint = fingerprint;
            Reports = Array.AsReadOnly(
                reports == null
                    ? Array.Empty<ConclusionReport>()
                    : new List<ConclusionReport>(reports).ToArray());
            Trace = Array.AsReadOnly(new List<string>(trace).ToArray());
            StateRestored = stateRestored;
            GeneratedStarCount = generatedStarCount;
            RawDiagnostic = rawDiagnostic;
            SystemDisplays = Array.AsReadOnly(
                systemDisplays == null
                    ? Array.Empty<RuntimeSystemDisplay>()
                    : new List<RuntimeSystemDisplay>(systemDisplays).ToArray());
            BirthPlanetAttributions = birthPlanetAttributions == null
                ? null
                : Array.AsReadOnly(
                    new List<NormalizedBirthPlanetEvidence>(birthPlanetAttributions).ToArray());
            HomePlanetTopology = homePlanetTopology;
            SystemCandidates = systemCandidates;
            DarkFogOccupation = darkFogOccupation;
        }

        public RuntimeScanStatus Status { get; }
        public int GalaxySeed { get; }
        public string Stage { get; }
        public string Code { get; }
        public string Message { get; }
        public RuntimeFingerprint? Fingerprint { get; }
        public IReadOnlyList<ConclusionReport> Reports { get; }
        public ConclusionReport? Conclusion
        {
            get
            {
                foreach (ConclusionReport report in Reports)
                {
                    if (String.Equals(
                        report.ConclusionId,
                        SharedSatelliteEvaluator.ConclusionId,
                        StringComparison.Ordinal))
                    {
                        return report;
                    }
                }
                return null;
            }
        }
        public IReadOnlyList<string> Trace { get; }
        public bool StateRestored { get; }
        public int? GeneratedStarCount { get; }
        public string? RawDiagnostic { get; }
        public IReadOnlyList<RuntimeSystemDisplay> SystemDisplays { get; }
        public IReadOnlyList<NormalizedBirthPlanetEvidence>? BirthPlanetAttributions { get; }
        public NormalizedHomePlanetTopology? HomePlanetTopology { get; }
        public RuntimeSystemCandidates? SystemCandidates { get; }
        public RuntimeDarkFogOccupation? DarkFogOccupation { get; }
    }

    public sealed class PreviewScanCoordinator
    {
        private readonly IRuntimePreviewGateway gateway;
        private readonly RuntimeOperationGate operationGate;

        public PreviewScanCoordinator(
            IRuntimePreviewGateway gateway,
            RuntimeOperationGate? operationGate = null)
        {
            this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            this.operationGate = operationGate ?? new RuntimeOperationGate();
        }

        public RuntimeScanResult TryScan(PreviewScanRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var trace = new List<string>();
            if (!operationGate.TryEnter())
                return Result(RuntimeScanStatus.Busy, request, "busy", "Another runtime request is active.", null, null, trace, true);

            RuntimeFingerprint? fingerprint = null;
            RuntimeStateLease? lease = null;
            RuntimeScanStatus status = RuntimeScanStatus.Failed;
            string code = "runtime-failure";
            string message = "The runtime request failed.";
            string? rawDiagnostic = null;
            IReadOnlyList<ConclusionReport>? reports = null;
            int? generatedStarCount = null;
            IReadOnlyList<RuntimeSystemDisplay>? systemDisplays = null;
            IReadOnlyList<NormalizedBirthPlanetEvidence>? birthPlanetAttributions = null;
            NormalizedHomePlanetTopology? homePlanetTopology = null;
            RuntimeSystemCandidates? systemCandidates = null;
            RuntimeDarkFogOccupation? darkFogOccupation = null;
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
                            trace.Add("compatibility:supported");
                            lease = gateway.CaptureState();
                            trace.Add("state:capture");
                            cancellationToken.ThrowIfCancellationRequested();
                            RuntimePreviewSnapshot snapshot = gateway.GeneratePreview(request, cancellationToken, trace.Add);
                            generatedStarCount = snapshot.GeneratedStarCount;
                            systemDisplays = snapshot.SystemDisplays;
                            cancellationToken.ThrowIfCancellationRequested();

                            if (snapshot.UnknownEnumValue.HasValue)
                            {
                                status = RuntimeScanStatus.Incompatible;
                                code = "unknown-runtime-enum";
                                rawDiagnostic = snapshot.UnknownEnumType + "=" + snapshot.UnknownEnumValue.Value;
                                message = "The runtime returned an enum value outside the supported contract.";
                            }
                            else if (snapshot.GeneratedStarCount != request.RequestedStarCount)
                            {
                                status = RuntimeScanStatus.Failed;
                                code = "generated-star-count-mismatch";
                                message = "The generated cluster did not contain the requested star count.";
                            }
                            else if (snapshot.Systems.Count != snapshot.GeneratedStarCount)
                            {
                                status = RuntimeScanStatus.Failed;
                                code = "normalized-system-count-mismatch";
                                message = "Normalized preview coverage omitted one or more generated systems.";
                            }
                            else if (snapshot.SystemDistances.Count !=
                                snapshot.GeneratedStarCount * (snapshot.GeneratedStarCount - 1) / 2)
                            {
                                status = RuntimeScanStatus.Failed;
                                code = "normalized-distance-coverage-mismatch";
                                message = "Normalized preview coverage omitted one or more system distances.";
                            }
                            else
                            {
                                reports = RuntimeConclusionEvaluator.Evaluate(request, fingerprint, snapshot);
                                birthPlanetAttributions = snapshot.BirthPlanetAttributions;
                                homePlanetTopology = snapshot.HomePlanetTopology;
                                systemCandidates = snapshot.SystemCandidates;
                                darkFogOccupation = RuntimeDarkFogOccupation.Project(
                                    request.CombatMode,
                                    snapshot.Systems);
                                status = RuntimeScanStatus.Success;
                                code = "success";
                                message = "The complete compatible preview was evaluated.";
                                trace.Add("evaluation:complete");
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                status = RuntimeScanStatus.Cancelled;
                code = "cancelled";
                message = "The preview request was cancelled at a safe boundary.";
                trace.Add("request:cancelled");
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
                        reports = null;
                    }
                }
                operationGate.Exit();
            }

            return Result(
                status,
                request,
                code,
                message,
                fingerprint,
                reports,
                trace,
                restored,
                generatedStarCount,
                rawDiagnostic,
                systemDisplays,
                birthPlanetAttributions,
                homePlanetTopology,
                systemCandidates,
                status == RuntimeScanStatus.Success ? darkFogOccupation : null);
        }

        private static RuntimeScanResult Result(
            RuntimeScanStatus status,
            PreviewScanRequest request,
            string code,
            string message,
            RuntimeFingerprint? fingerprint,
            IEnumerable<ConclusionReport>? reports,
            IEnumerable<string> trace,
            bool restored,
            int? generatedStarCount = null,
            string? rawDiagnostic = null,
            IEnumerable<RuntimeSystemDisplay>? systemDisplays = null,
            IEnumerable<NormalizedBirthPlanetEvidence>? birthPlanetAttributions = null,
            NormalizedHomePlanetTopology? homePlanetTopology = null,
            RuntimeSystemCandidates? systemCandidates = null,
            RuntimeDarkFogOccupation? darkFogOccupation = null)
        {
            return new RuntimeScanResult(
                status,
                request.GalaxySeed,
                "galaxy-preview",
                code,
                message,
                fingerprint,
                reports,
                trace,
                restored,
                generatedStarCount,
                rawDiagnostic,
                systemDisplays,
                birthPlanetAttributions,
                homePlanetTopology,
                systemCandidates,
                darkFogOccupation);
        }
    }
}
