using System;

namespace DSPSeedScanner.Core
{
    public enum EvidenceStage
    {
        GalaxyPreview,
        BirthSystemRaw,
        CompleteClusterRaw
    }

    public enum EvidenceScope
    {
        BirthSystemTopology
    }

    public enum CoverageState
    {
        Complete,
        Partial,
        Unavailable
    }

    public sealed record EvidenceKey
    {
        public EvidenceKey(GenerationIdentity identity, EvidenceStage stage)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Stage = stage;
        }

        public GenerationIdentity Identity { get; }

        public EvidenceStage Stage { get; }
    }

    public sealed record EvidenceCoverage
    {
        public EvidenceCoverage(
            EvidenceStage stage,
            EvidenceScope scope,
            CoverageState state,
            int expectedSubjects,
            int completedSubjects)
        {
            if (expectedSubjects <= 0)
                throw new ArgumentOutOfRangeException(nameof(expectedSubjects));
            if (completedSubjects < 0 || completedSubjects > expectedSubjects)
                throw new ArgumentOutOfRangeException(nameof(completedSubjects));
            if (state == CoverageState.Complete &&
                completedSubjects != expectedSubjects)
            {
                throw new ArgumentException(
                    "Complete coverage requires every expected subject.",
                    nameof(completedSubjects));
            }
            if (state == CoverageState.Partial &&
                (completedSubjects == 0 || completedSubjects == expectedSubjects))
            {
                throw new ArgumentException(
                    "Partial coverage requires some but not all subjects.",
                    nameof(completedSubjects));
            }
            if (state == CoverageState.Unavailable && completedSubjects != 0)
            {
                throw new ArgumentException(
                    "Unavailable coverage cannot contain completed subjects.",
                    nameof(completedSubjects));
            }

            Stage = stage;
            Scope = scope;
            State = state;
            ExpectedSubjects = expectedSubjects;
            CompletedSubjects = completedSubjects;
        }

        public EvidenceStage Stage { get; }

        public EvidenceScope Scope { get; }

        public CoverageState State { get; }

        public int ExpectedSubjects { get; }

        public int CompletedSubjects { get; }

        public bool IsComplete => State == CoverageState.Complete;
    }

    public sealed record NormalizedBirthTopologyEvidence
    {
        public NormalizedBirthTopologyEvidence(
            GenerationIdentity identity,
            EvidenceCoverage coverage,
            ConclusionSubject subject,
            int? sharedBirthGiantBodies)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Coverage = coverage ?? throw new ArgumentNullException(nameof(coverage));
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));

            if (coverage.Stage != EvidenceStage.GalaxyPreview)
                throw new ArgumentException(
                    "Birth topology is preview evidence.",
                    nameof(coverage));
            if (coverage.Scope != EvidenceScope.BirthSystemTopology)
                throw new ArgumentException(
                    "Coverage scope does not describe birth topology.",
                    nameof(coverage));
            if (coverage.IsComplete &&
                (!sharedBirthGiantBodies.HasValue ||
                 sharedBirthGiantBodies.Value < 1))
            {
                throw new ArgumentOutOfRangeException(
                    nameof(sharedBirthGiantBodies),
                    "Complete topology requires the birth body count.");
            }
            if (sharedBirthGiantBodies < 0)
                throw new ArgumentOutOfRangeException(nameof(sharedBirthGiantBodies));

            SharedBirthGiantBodies = sharedBirthGiantBodies;
        }

        public GenerationIdentity Identity { get; }

        public EvidenceCoverage Coverage { get; }

        public ConclusionSubject Subject { get; }

        public int? SharedBirthGiantBodies { get; }
    }
}
