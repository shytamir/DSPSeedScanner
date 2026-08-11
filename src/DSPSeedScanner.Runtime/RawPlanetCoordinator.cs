using System;
using System.Collections.Generic;
using System.Threading;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public sealed class RawPlanetResult
    {
        public RawPlanetResult(
            RuntimeScanStatus status,
            RawPlanetRequest request,
            string stage,
            string code,
            string message,
            RuntimeFingerprint? fingerprint,
            NormalizedRawPlanetEvidence? evidence,
            RawPlanetCoverage coverage,
            IEnumerable<string> trace,
            bool stateRestored,
            string? rawDiagnostic = null)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            if (String.IsNullOrWhiteSpace(stage))
                throw new ArgumentException("Stage is required.", nameof(stage));
            if (String.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Result code is required.", nameof(code));
            if (String.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Result message is required.", nameof(message));
            if (trace == null)
                throw new ArgumentNullException(nameof(trace));
            if (coverage == null)
                throw new ArgumentNullException(nameof(coverage));
            if (status == RuntimeScanStatus.Success &&
                (evidence == null || !coverage.IsComplete || !stateRestored))
            {
                throw new ArgumentException("A successful raw result requires complete evidence and restored state.");
            }
            if (status != RuntimeScanStatus.Success && evidence != null)
                throw new ArgumentException("A non-success raw result cannot expose complete evidence.");
            if ((evidence != null) != coverage.IsComplete)
                throw new ArgumentException("Raw evidence and coverage completion must agree.");

            Status = status;
            GalaxySeed = request.Identity.GalaxySeed;
            PlanetId = request.PlanetId;
            Stage = stage;
            Code = code;
            Message = message;
            Fingerprint = fingerprint;
            Evidence = evidence;
            Coverage = coverage;
            Trace = Array.AsReadOnly(new List<string>(trace).ToArray());
            StateRestored = stateRestored;
            RawDiagnostic = rawDiagnostic;
        }

        public RuntimeScanStatus Status { get; }
        public RawPlanetRequest Request { get; }
        public int GalaxySeed { get; }
        public int PlanetId { get; }
        public string Stage { get; }
        public string Code { get; }
        public string Message { get; }
        public RuntimeFingerprint? Fingerprint { get; }
        public NormalizedRawPlanetEvidence? Evidence { get; }
        public RawPlanetCoverage Coverage { get; }
        public IReadOnlyList<string> Trace { get; }
        public bool StateRestored { get; }
        public string? RawDiagnostic { get; }
    }

    public sealed class RawPlanetCoordinator
    {
        public const string Stage = "raw-planet-generation";

        private readonly IRuntimeRawPlanetGateway gateway;
        private readonly RuntimeOperationGate operationGate;

        public RawPlanetCoordinator(
            IRuntimeRawPlanetGateway gateway,
            RuntimeOperationGate? operationGate = null)
        {
            this.gateway = gateway ?? throw new ArgumentNullException(nameof(gateway));
            this.operationGate = operationGate ?? new RuntimeOperationGate();
        }

        public RawPlanetResult TryGenerate(
            RawPlanetRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var trace = new List<string>();
            if (!operationGate.TryEnter())
                return Result(RuntimeScanStatus.Busy, request, "busy", "Another runtime request is active.", null, null, trace, true);

            RuntimeFingerprint? fingerprint = null;
            RuntimeStateLease? lease = null;
            RuntimeScanStatus status = RuntimeScanStatus.Failed;
            string code = "raw-runtime-failure";
            string message = "The raw planet request failed.";
            string? rawDiagnostic = null;
            NormalizedRawPlanetEvidence? evidence = null;
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
                    fingerprint = gateway.CaptureFingerprint(request.Identity);
                    trace.Add("fingerprint:capture");
                    CompatibilityDecision compatibility = CompatibilityPolicy.Evaluate(fingerprint);
                    CompatibilityDecision requestCompatibility =
                        CompatibilityPolicy.EvaluateRequest(request.Identity);
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
                        trace.Add("compatibility:supported");
                        lease = gateway.CaptureState();
                        trace.Add("state:capture");
                        cancellationToken.ThrowIfCancellationRequested();
                        evidence = gateway.GenerateRawPlanet(request, cancellationToken, trace.Add);
                        cancellationToken.ThrowIfCancellationRequested();
                        if (evidence.GalaxySeed != request.Identity.GalaxySeed ||
                            evidence.PlanetId != request.PlanetId ||
                            evidence.AlgorithmId != request.ExpectedAlgorithmId)
                        {
                            evidence = null;
                            status = RuntimeScanStatus.Failed;
                            code = "raw-target-mismatch";
                            message = "The generated raw planet did not match the requested target.";
                        }
                        else if (!evidence.Coverage.IsComplete)
                        {
                            evidence = null;
                            status = RuntimeScanStatus.Failed;
                            code = "raw-coverage-incomplete";
                            message = "The raw planet operation did not complete its declared coverage.";
                        }
                        else
                        {
                            status = RuntimeScanStatus.Success;
                            code = "success";
                            message = "The isolated raw planet was normalized completely.";
                            trace.Add("normalization:complete");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                evidence = null;
                status = RuntimeScanStatus.Cancelled;
                code = "cancelled";
                message = "The raw planet request was cancelled at an atomic boundary.";
                trace.Add("request:cancelled");
            }
            catch (RawCompatibilityException exception)
            {
                evidence = null;
                status = RuntimeScanStatus.Incompatible;
                code = exception.Code;
                message = exception.Message;
                rawDiagnostic = exception.RawDiagnostic;
                trace.Add("request:incompatible");
            }
            catch (Exception exception)
            {
                evidence = null;
                status = RuntimeScanStatus.Failed;
                code = "raw-runtime-exception";
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
                        evidence = null;
                        status = RuntimeScanStatus.Failed;
                        code = "state-restoration-failed";
                    }
                }
                operationGate.Exit();
            }

            return Result(status, request, code, message, fingerprint, evidence, trace, restored, rawDiagnostic);
        }

        private static RawPlanetResult Result(
            RuntimeScanStatus status,
            RawPlanetRequest request,
            string code,
            string message,
            RuntimeFingerprint? fingerprint,
            NormalizedRawPlanetEvidence? evidence,
            IEnumerable<string> trace,
            bool restored,
            string? rawDiagnostic = null)
        {
            return new RawPlanetResult(
                status,
                request,
                Stage,
                code,
                message,
                fingerprint,
                evidence,
                evidence?.Coverage ?? RawPlanetCoverage.Unavailable(),
                trace,
                restored,
                rawDiagnostic);
        }
    }
}
