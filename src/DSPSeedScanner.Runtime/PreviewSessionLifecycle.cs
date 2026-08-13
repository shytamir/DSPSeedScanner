using System;
using System.Threading;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public sealed class PreviewGenerationIdentity : IEquatable<PreviewGenerationIdentity>
    {
        public PreviewGenerationIdentity(
            GenerationIdentity galaxy,
            decimal resourceMultiplier,
            CombatMode combatMode,
            string combatSettingsKey,
            decimal initialColonize = 1m,
            decimal maxDensity = 1m)
        {
            GalaxyIdentity = galaxy ?? throw new ArgumentNullException(nameof(galaxy));
            if (resourceMultiplier <= 0)
                throw new ArgumentOutOfRangeException(nameof(resourceMultiplier));
            if (String.IsNullOrWhiteSpace(combatSettingsKey))
                throw new ArgumentException("Combat settings key is required.", nameof(combatSettingsKey));
            if (initialColonize < 0)
                throw new ArgumentOutOfRangeException(nameof(initialColonize));
            if (maxDensity < 0)
                throw new ArgumentOutOfRangeException(nameof(maxDensity));
            string expectedCombatKey = PreviewScanRequest.CombatSettingsKeyFor(
                initialColonize,
                maxDensity);
            if (!String.Equals(combatSettingsKey, expectedCombatKey, StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "Combat settings key does not match the supplied preview settings.",
                    nameof(combatSettingsKey));
            }

            ResourceMultiplier = resourceMultiplier;
            CombatMode = combatMode;
            CombatSettingsKey = combatSettingsKey;
            InitialColonize = initialColonize;
            MaxDensity = maxDensity;
        }

        public GenerationIdentity GalaxyIdentity { get; }
        public decimal ResourceMultiplier { get; }
        public CombatMode CombatMode { get; }
        public string CombatSettingsKey { get; }
        public decimal InitialColonize { get; }
        public decimal MaxDensity { get; }

        public bool Equals(PreviewGenerationIdentity? other)
        {
            return other != null &&
                GalaxyIdentity == other.GalaxyIdentity &&
                ResourceMultiplier == other.ResourceMultiplier &&
                CombatMode == other.CombatMode &&
                String.Equals(CombatSettingsKey, other.CombatSettingsKey, StringComparison.Ordinal) &&
                InitialColonize == other.InitialColonize &&
                MaxDensity == other.MaxDensity;
        }

        public override bool Equals(object? obj) => Equals(obj as PreviewGenerationIdentity);

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = GalaxyIdentity.GetHashCode();
                hash = (hash * 397) ^ ResourceMultiplier.GetHashCode();
                hash = (hash * 397) ^ CombatMode.GetHashCode();
                hash = (hash * 397) ^ StringComparer.Ordinal.GetHashCode(CombatSettingsKey);
                hash = (hash * 397) ^ InitialColonize.GetHashCode();
                hash = (hash * 397) ^ MaxDensity.GetHashCode();
                return hash;
            }
        }
    }

    public enum PreviewSessionRetirementReason
    {
        None,
        Replaced,
        PreviewExited
    }

    public sealed class PreviewSession
    {
        private readonly CancellationTokenSource lifetime = new CancellationTokenSource();
        private int retired;
        private PreviewSessionRetirementReason retirementReason;
        private string? homePlanetDisplayDesignation;

        internal PreviewSession(
            long sessionId,
            long loadSequence,
            PreviewGenerationIdentity identity)
        {
            SessionId = sessionId;
            LoadSequence = loadSequence;
            Identity = identity;
        }

        public long SessionId { get; }
        public long LoadSequence { get; }
        public PreviewGenerationIdentity Identity { get; }
        public CancellationToken Lifetime => lifetime.Token;
        public bool IsRetired => Volatile.Read(ref retired) != 0;
        public PreviewSessionRetirementReason RetirementReason => retirementReason;
        public string? HomePlanetDisplayDesignation => homePlanetDisplayDesignation;

        internal void SetHomePlanetDisplayDesignation(string designation)
        {
            if (String.IsNullOrWhiteSpace(designation))
            {
                throw new ArgumentException(
                    "Home planet display designation is required.",
                    nameof(designation));
            }
            if (IsRetired)
            {
                throw new InvalidOperationException(
                    "A retired preview session cannot accept presentation identity.");
            }
            if (homePlanetDisplayDesignation != null)
            {
                throw new InvalidOperationException(
                    "The home planet display designation is immutable once attached.");
            }
            homePlanetDisplayDesignation = designation;
        }

        internal void Retire(PreviewSessionRetirementReason reason)
        {
            if (reason == PreviewSessionRetirementReason.None)
                throw new ArgumentOutOfRangeException(nameof(reason));

            retirementReason = reason;
            if (Interlocked.Exchange(ref retired, 1) == 0)
            {
                homePlanetDisplayDesignation = null;
                lifetime.Cancel();
            }
        }
    }

    public enum PreviewLoadDisposition
    {
        SessionCreated,
        DuplicateCoalesced,
        StaleLoadIgnored,
        RetiredLoadIgnored
    }

    public sealed class PreviewLoadTransition
    {
        internal PreviewLoadTransition(
            PreviewLoadDisposition disposition,
            PreviewSession? currentSession,
            PreviewSession? retiredSession)
        {
            Disposition = disposition;
            CurrentSession = currentSession;
            RetiredSession = retiredSession;
        }

        public PreviewLoadDisposition Disposition { get; }
        public PreviewSession? CurrentSession { get; }
        public PreviewSession? RetiredSession { get; }
    }

    public sealed class PreviewSessionLifecycle
    {
        private readonly object sync = new object();
        private long nextSessionId;
        private long highestLoadSequence;
        private PreviewGenerationIdentity? highestLoadIdentity;
        private PreviewSession? activeSession;

        public PreviewSession? CurrentSession
        {
            get
            {
                lock (sync)
                    return activeSession;
            }
        }

        /// <summary>
        /// Observes a DSP preview only after its load has completed. The caller
        /// allocates the sequence when that load begins and reuses it for every
        /// callback belonging to the same load.
        /// </summary>
        public PreviewLoadTransition ObserveCompletedLoad(
            long loadSequence,
            PreviewGenerationIdentity identity)
        {
            if (loadSequence <= 0)
                throw new ArgumentOutOfRangeException(nameof(loadSequence));
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            PreviewSession? retired = null;
            PreviewLoadTransition transition;
            lock (sync)
            {
                if (loadSequence < highestLoadSequence)
                {
                    return new PreviewLoadTransition(
                        PreviewLoadDisposition.StaleLoadIgnored,
                        activeSession,
                        null);
                }

                if (loadSequence == highestLoadSequence)
                {
                    if (!identity.Equals(highestLoadIdentity))
                    {
                        throw new InvalidOperationException(
                            "One preview load sequence cannot identify different generation inputs.");
                    }

                    return new PreviewLoadTransition(
                        activeSession != null && activeSession.LoadSequence == loadSequence
                            ? PreviewLoadDisposition.DuplicateCoalesced
                            : PreviewLoadDisposition.RetiredLoadIgnored,
                        activeSession,
                        null);
                }

                retired = activeSession;
                highestLoadSequence = loadSequence;
                highestLoadIdentity = identity;
                activeSession = new PreviewSession(
                    checked(++nextSessionId),
                    loadSequence,
                    identity);
                transition = new PreviewLoadTransition(
                    PreviewLoadDisposition.SessionCreated,
                    activeSession,
                    retired);
            }

            retired?.Retire(PreviewSessionRetirementReason.Replaced);
            return transition;
        }

        /// <summary>
        /// Retires the current session when the New Game preview is left.
        /// Repeated exits are harmless.
        /// </summary>
        public PreviewSession? ExitPreview()
        {
            PreviewSession? retired;
            lock (sync)
            {
                retired = activeSession;
                activeSession = null;
            }

            retired?.Retire(PreviewSessionRetirementReason.PreviewExited);
            return retired;
        }

        /// <summary>
        /// Returns whether work attributed to a session may still update the
        /// current presentation state.
        /// </summary>
        public bool CanPublish(PreviewSession session)
        {
            if (session == null)
                throw new ArgumentNullException(nameof(session));

            lock (sync)
                return ReferenceEquals(activeSession, session) && !session.IsRetired;
        }
    }
}
