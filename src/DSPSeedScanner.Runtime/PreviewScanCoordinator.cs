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
            ConclusionReport? conclusion,
            IEnumerable<string> trace,
            bool stateRestored,
            string? rawDiagnostic = null)
        {
            Status = status;
            GalaxySeed = galaxySeed;
            Stage = stage;
            Code = code;
            Message = message;
            Fingerprint = fingerprint;
            Conclusion = conclusion;
            Trace = Array.AsReadOnly(new List<string>(trace).ToArray());
            StateRestored = stateRestored;
            RawDiagnostic = rawDiagnostic;
        }

        public RuntimeScanStatus Status { get; }
        public int GalaxySeed { get; }
        public string Stage { get; }
        public string Code { get; }
        public string Message { get; }
        public RuntimeFingerprint? Fingerprint { get; }
        public ConclusionReport? Conclusion { get; }
        public IReadOnlyList<string> Trace { get; }
        public bool StateRestored { get; }
        public string? RawDiagnostic { get; }
    }

    public sealed class PreviewScanCoordinator
    {
        private readonly IRuntimePreviewGateway gateway;
        private int active;

        public PreviewScanCoordinator(IRuntimePreviewGateway gateway)
        {
            this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
        }

        public RuntimeScanResult TryScan(PreviewScanRequest request, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var trace = new List<string>();
            if (Interlocked.CompareExchange(ref active, 1, 0) != 0)
                return Result(RuntimeScanStatus.Busy, request, "busy", "Another runtime request is active.", null, null, trace, true);

            RuntimeFingerprint? fingerprint = null;
            RuntimeStateLease? lease = null;
            RuntimeScanStatus status = RuntimeScanStatus.Failed;
            string code = "runtime-failure";
            string message = "The runtime request failed.";
            string? rawDiagnostic = null;
            ConclusionReport? conclusion = null;
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
                            RuntimeTopologySnapshot snapshot = gateway.GeneratePreview(request, cancellationToken, trace.Add);
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
                            else
                            {
                                conclusion = Evaluate(request, fingerprint, snapshot);
                                status = RuntimeScanStatus.Success;
                                code = "success";
                                message = "Compatible preview topology was evaluated.";
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
                        conclusion = null;
                    }
                }
                Volatile.Write(ref active, 0);
            }

            return Result(status, request, code, message, fingerprint, conclusion, trace, restored, rawDiagnostic);
        }

        private static ConclusionReport Evaluate(
            PreviewScanRequest request,
            RuntimeFingerprint fingerprint,
            RuntimeTopologySnapshot snapshot)
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
            var coverage = new EvidenceCoverage(
                EvidenceStage.GalaxyPreview,
                EvidenceScope.BirthSystemTopology,
                CoverageState.Complete,
                1,
                1);
            var evidence = new NormalizedBirthTopologyEvidence(
                identity,
                settings,
                coverage,
                new ConclusionSubject(SubjectKind.BirthSystem, snapshot.BirthSystemIdentifier),
                snapshot.SharedBirthGiantBodies);
            return SharedSatelliteEvaluator.Evaluate(evidence);
        }

        private static RuntimeScanResult Result(
            RuntimeScanStatus status,
            PreviewScanRequest request,
            string code,
            string message,
            RuntimeFingerprint? fingerprint,
            ConclusionReport? conclusion,
            IEnumerable<string> trace,
            bool restored,
            string? rawDiagnostic = null)
        {
            return new RuntimeScanResult(
                status,
                request.GalaxySeed,
                "galaxy-preview",
                code,
                message,
                fingerprint,
                conclusion,
                trace,
                restored,
                rawDiagnostic);
        }
    }
}
