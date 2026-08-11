using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public sealed record CompleteClusterPlanetTarget
    {
        public CompleteClusterPlanetTarget(
            int planetId,
            int algorithmId,
            ConclusionSubject system,
            decimal distanceFromBirthLy)
        {
            if (planetId <= 0)
                throw new ArgumentOutOfRangeException(nameof(planetId));
            if (algorithmId <= 0)
                throw new ArgumentOutOfRangeException(nameof(algorithmId));
            System = system ?? throw new ArgumentNullException(nameof(system));
            if (system.Kind != SubjectKind.BirthSystem &&
                system.Kind != SubjectKind.StarSystem)
            {
                throw new ArgumentException("A planet target requires a system subject.", nameof(system));
            }
            if (distanceFromBirthLy < 0)
                throw new ArgumentOutOfRangeException(nameof(distanceFromBirthLy));
            PlanetId = planetId;
            AlgorithmId = algorithmId;
            DistanceFromBirthLy = distanceFromBirthLy;
        }

        public int PlanetId { get; }
        public int AlgorithmId { get; }
        public ConclusionSubject System { get; }
        public decimal DistanceFromBirthLy { get; }
    }

    public sealed class CompleteClusterRawPlan
    {
        private readonly CompleteClusterPlanetTarget[] targets;

        public CompleteClusterRawPlan(
            RuntimePreviewSnapshot preview,
            IEnumerable<CompleteClusterPlanetTarget> targets)
        {
            Preview = preview ?? throw new ArgumentNullException(nameof(preview));
            if (targets == null)
                throw new ArgumentNullException(nameof(targets));
            this.targets = targets.OrderBy(target => target.PlanetId).ToArray();
            if (this.targets.Length == 0)
                throw new ArgumentException("A complete cluster must declare solid planets.", nameof(targets));
            if (this.targets.Select(target => target.PlanetId).Distinct().Count() != this.targets.Length)
                throw new ArgumentException("Complete-cluster planet IDs must be unique.", nameof(targets));
        }

        public RuntimePreviewSnapshot Preview { get; }
        public IReadOnlyList<CompleteClusterPlanetTarget> Targets =>
            Array.AsReadOnly((CompleteClusterPlanetTarget[])targets.Clone());
    }

    public enum CompleteClusterProgressState
    {
        Planned,
        PlanetStarted,
        PlanetCompleted
    }

    public sealed record CompleteClusterRawProgress
    {
        public CompleteClusterRawProgress(
            CompleteClusterProgressState state,
            int expectedPlanets,
            int completedPlanets,
            int? planetId)
        {
            if (expectedPlanets <= 0)
                throw new ArgumentOutOfRangeException(nameof(expectedPlanets));
            if (completedPlanets < 0 || completedPlanets > expectedPlanets)
                throw new ArgumentOutOfRangeException(nameof(completedPlanets));
            if ((state == CompleteClusterProgressState.Planned) == planetId.HasValue)
                throw new ArgumentException("Only planet progress may identify a planet.", nameof(planetId));
            State = state;
            ExpectedPlanets = expectedPlanets;
            CompletedPlanets = completedPlanets;
            PlanetId = planetId;
        }

        public CompleteClusterProgressState State { get; }
        public int ExpectedPlanets { get; }
        public int CompletedPlanets { get; }
        public int? PlanetId { get; }
    }

    public sealed record CompleteClusterRawCoverage
    {
        public CompleteClusterRawCoverage(
            CoverageState state,
            int expectedPlanets,
            int completedPlanets)
        {
            if (expectedPlanets < 0)
                throw new ArgumentOutOfRangeException(nameof(expectedPlanets));
            if (completedPlanets < 0 || completedPlanets > expectedPlanets)
                throw new ArgumentOutOfRangeException(nameof(completedPlanets));
            if (state == CoverageState.Complete &&
                (expectedPlanets == 0 || completedPlanets != expectedPlanets))
                throw new ArgumentException("Complete coverage requires every declared planet.");
            if (state == CoverageState.Partial &&
                (completedPlanets == 0 || completedPlanets == expectedPlanets))
                throw new ArgumentException("Partial coverage requires some declared planets.");
            if (state == CoverageState.Unavailable && completedPlanets != 0)
                throw new ArgumentException("Unavailable coverage cannot contain completed planets.");
            State = state;
            ExpectedPlanets = expectedPlanets;
            CompletedPlanets = completedPlanets;
        }

        public CoverageState State { get; }
        public int ExpectedPlanets { get; }
        public int CompletedPlanets { get; }
        public bool IsComplete => State == CoverageState.Complete;
    }

    public sealed class CompleteClusterRawResult
    {
        public CompleteClusterRawResult(
            RuntimeScanStatus status,
            int galaxySeed,
            string code,
            string message,
            RuntimeFingerprint? fingerprint,
            CompleteClusterRawCoverage coverage,
            IEnumerable<CompleteClusterRawProgress> progress,
            IEnumerable<NormalizedRareResourceEvidence>? rareResources,
            IEnumerable<ConclusionReport>? reports,
            IEnumerable<string> trace,
            bool stateRestored,
            long elapsedMilliseconds,
            long managedMemoryDeltaBytes,
            int? affectedPlanetId = null,
            string? rawDiagnostic = null)
        {
            Status = status;
            GalaxySeed = galaxySeed;
            Code = code;
            Message = message;
            Fingerprint = fingerprint;
            Coverage = coverage ?? throw new ArgumentNullException(nameof(coverage));
            Progress = Array.AsReadOnly(new List<CompleteClusterRawProgress>(progress).ToArray());
            RareResources = Array.AsReadOnly(rareResources == null
                ? Array.Empty<NormalizedRareResourceEvidence>()
                : new List<NormalizedRareResourceEvidence>(rareResources).ToArray());
            Reports = Array.AsReadOnly(reports == null
                ? Array.Empty<ConclusionReport>()
                : new List<ConclusionReport>(reports).ToArray());
            Trace = Array.AsReadOnly(new List<string>(trace).ToArray());
            StateRestored = stateRestored;
            ElapsedMilliseconds = elapsedMilliseconds;
            ManagedMemoryDeltaBytes = managedMemoryDeltaBytes;
            AffectedPlanetId = affectedPlanetId;
            RawDiagnostic = rawDiagnostic;
        }

        public RuntimeScanStatus Status { get; }
        public int GalaxySeed { get; }
        public string Stage => CompleteClusterRawCoordinator.Stage;
        public string Code { get; }
        public string Message { get; }
        public RuntimeFingerprint? Fingerprint { get; }
        public CompleteClusterRawCoverage Coverage { get; }
        public IReadOnlyList<CompleteClusterRawProgress> Progress { get; }
        public IReadOnlyList<NormalizedRareResourceEvidence> RareResources { get; }
        public IReadOnlyList<ConclusionReport> Reports { get; }
        public IReadOnlyList<string> Trace { get; }
        public bool StateRestored { get; }
        public long ElapsedMilliseconds { get; }
        public long ManagedMemoryDeltaBytes { get; }
        public int? AffectedPlanetId { get; }
        public string? RawDiagnostic { get; }
    }

    public interface IRuntimeCompleteClusterRawGateway : IRuntimeBirthSystemRawGateway
    {
        CompleteClusterRawPlan DiscoverCompleteCluster(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<string> recordTrace);

        void GenerateCompleteCluster(
            PreviewScanRequest request,
            CompleteClusterRawPlan plan,
            CancellationToken cancellationToken,
            Action<CompleteClusterPlanetTarget> planetStarted,
            Action<CompleteClusterPlanetTarget, NormalizedRawPlanetEvidence> planetCompleted,
            Action<string> recordTrace);
    }
}
