using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public sealed record BirthSystemPlanetTarget
    {
        public BirthSystemPlanetTarget(int planetId, int algorithmId)
        {
            if (planetId <= 0)
                throw new ArgumentOutOfRangeException(nameof(planetId));
            if (algorithmId <= 0)
                throw new ArgumentOutOfRangeException(nameof(algorithmId));
            PlanetId = planetId;
            AlgorithmId = algorithmId;
        }

        public int PlanetId { get; }
        public int AlgorithmId { get; }
    }

    public sealed class BirthSystemRawPlan
    {
        private readonly BirthSystemPlanetTarget[] targets;

        public BirthSystemRawPlan(
            RuntimePreviewSnapshot preview,
            IEnumerable<BirthSystemPlanetTarget> targets)
        {
            Preview = preview ?? throw new ArgumentNullException(nameof(preview));
            if (targets == null)
                throw new ArgumentNullException(nameof(targets));
            this.targets = targets.OrderBy(target => target.PlanetId).ToArray();
            if (this.targets.Length == 0)
                throw new ArgumentException("The birth system must declare at least one solid planet.", nameof(targets));
            if (this.targets.Select(target => target.PlanetId).Distinct().Count() != this.targets.Length)
                throw new ArgumentException("Birth-system planet IDs must be unique.", nameof(targets));
        }

        public RuntimePreviewSnapshot Preview { get; }
        public IReadOnlyList<BirthSystemPlanetTarget> Targets =>
            Array.AsReadOnly((BirthSystemPlanetTarget[])targets.Clone());
    }

    public enum BirthSystemProgressState
    {
        Planned,
        PlanetStarted,
        PlanetCompleted
    }

    public sealed record BirthSystemRawCoverage
    {
        public BirthSystemRawCoverage(CoverageState state, int expectedPlanets, int completedPlanets)
        {
            if (expectedPlanets < 0)
                throw new ArgumentOutOfRangeException(nameof(expectedPlanets));
            if (completedPlanets < 0 || completedPlanets > expectedPlanets)
                throw new ArgumentOutOfRangeException(nameof(completedPlanets));
            if (state == CoverageState.Complete && (expectedPlanets == 0 || completedPlanets != expectedPlanets))
                throw new ArgumentException("Complete coverage requires every declared planet.");
            if (state == CoverageState.Partial && (completedPlanets == 0 || completedPlanets == expectedPlanets))
                throw new ArgumentException("Partial coverage requires some but not all declared planets.");
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

    public sealed record BirthSystemRawProgress
    {
        public BirthSystemRawProgress(
            BirthSystemProgressState state,
            int expectedPlanets,
            int completedPlanets,
            int? planetId)
        {
            if (expectedPlanets <= 0)
                throw new ArgumentOutOfRangeException(nameof(expectedPlanets));
            if (completedPlanets < 0 || completedPlanets > expectedPlanets)
                throw new ArgumentOutOfRangeException(nameof(completedPlanets));
            if (state == BirthSystemProgressState.Planned && planetId.HasValue)
                throw new ArgumentException("Planned progress has no active planet.", nameof(planetId));
            if (state != BirthSystemProgressState.Planned && !planetId.HasValue)
                throw new ArgumentException("Planet progress requires a planet ID.", nameof(planetId));
            State = state;
            ExpectedPlanets = expectedPlanets;
            CompletedPlanets = completedPlanets;
            PlanetId = planetId;
        }

        public BirthSystemProgressState State { get; }
        public int ExpectedPlanets { get; }
        public int CompletedPlanets { get; }
        public int? PlanetId { get; }
    }

    public sealed class BirthSystemRawResult
    {
        public BirthSystemRawResult(
            RuntimeScanStatus status,
            int galaxySeed,
            string code,
            string message,
            RuntimeFingerprint? fingerprint,
            BirthSystemRawCoverage coverage,
            IEnumerable<BirthSystemRawProgress> progress,
            IEnumerable<ConclusionReport>? reports,
            IEnumerable<string> trace,
            bool stateRestored,
            int? affectedPlanetId = null,
            string? rawDiagnostic = null)
        {
            Status = status;
            GalaxySeed = galaxySeed;
            Code = code;
            Message = message;
            Fingerprint = fingerprint;
            Coverage = coverage ?? throw new ArgumentNullException(nameof(coverage));
            Progress = Array.AsReadOnly(new List<BirthSystemRawProgress>(progress).ToArray());
            Reports = Array.AsReadOnly(reports == null
                ? Array.Empty<ConclusionReport>()
                : new List<ConclusionReport>(reports).ToArray());
            Trace = Array.AsReadOnly(new List<string>(trace).ToArray());
            StateRestored = stateRestored;
            AffectedPlanetId = affectedPlanetId;
            RawDiagnostic = rawDiagnostic;
        }

        public RuntimeScanStatus Status { get; }
        public int GalaxySeed { get; }
        public string Stage => BirthSystemRawCoordinator.Stage;
        public string Code { get; }
        public string Message { get; }
        public RuntimeFingerprint? Fingerprint { get; }
        public BirthSystemRawCoverage Coverage { get; }
        public IReadOnlyList<BirthSystemRawProgress> Progress { get; }
        public IReadOnlyList<ConclusionReport> Reports { get; }
        public IReadOnlyList<string> Trace { get; }
        public bool StateRestored { get; }
        public int? AffectedPlanetId { get; }
        public string? RawDiagnostic { get; }
    }

    public interface IRuntimeBirthSystemRawGateway : IRuntimeRawPlanetGateway
    {
        BirthSystemRawPlan DiscoverBirthSystem(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<string> recordTrace);
    }
}
