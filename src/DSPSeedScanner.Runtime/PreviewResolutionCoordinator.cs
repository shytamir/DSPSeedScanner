using System;
using System.Collections.Generic;
using System.Linq;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public enum PreviewResolutionState
    {
        Scanning,
        Cached,
        Complete,
        Busy,
        Incompatible,
        Cancelled,
        Failed
    }

    public sealed class PreviewResolutionAttempt
    {
        private readonly List<ConclusionReport> previewReports = new List<ConclusionReport>();
        private readonly List<ConclusionReport> completeReports = new List<ConclusionReport>();
        private readonly List<RuntimeSystemDisplay> systemDisplays =
            new List<RuntimeSystemDisplay>();
        private readonly List<NormalizedBirthPlanetEvidence> birthPlanetAttributions =
            new List<NormalizedBirthPlanetEvidence>();

        internal PreviewResolutionAttempt(PreviewSession session)
        {
            Session = session;
            State = PreviewResolutionState.Scanning;
            Code = "preview-evaluation";
            Message = "Immediate preview evidence is being evaluated.";
        }

        public PreviewSession Session { get; }
        public PreviewResolutionState State { get; internal set; }
        public string Code { get; internal set; }
        public string Message { get; internal set; }
        public IReadOnlyList<ConclusionReport> PreviewReports => previewReports.AsReadOnly();
        public IReadOnlyList<ConclusionReport> CompleteReports => completeReports.AsReadOnly();
        public IReadOnlyList<RuntimeSystemDisplay> SystemDisplays =>
            systemDisplays.AsReadOnly();
        public IReadOnlyList<NormalizedBirthPlanetEvidence> BirthPlanetAttributions =>
            birthPlanetAttributions.AsReadOnly();
        public bool HasCompleteBirthPlanetAttribution { get; private set; }
        public NormalizedHomePlanetTopology? HomePlanetTopology { get; private set; }
        public RuntimeSystemCandidates? SystemCandidates { get; private set; }
        public RuntimeDarkFogOccupation? DarkFogOccupation { get; private set; }
        public HomeSystemBodyInventory? HomeSystemBodyInventory { get; private set; }
        public HomeSystemResourceStatistics? HomeSystemResources { get; private set; }
        public ClusterResourceStatistics? ClusterResources { get; private set; }
        public NearbyDeuteriumGasGiantSelection? NearbyDeuteriumGasGiant {
            get;
            private set;
        }
        public NotableStarStatistics? NotableStars { get; private set; }
        public PreviewGenerationIdentity? CachedPayloadSourceIdentity { get; private set; }
        public int ExpectedPlanets { get; internal set; }
        public int CompletedPlanets { get; internal set; }
        public bool CacheStored { get; internal set; }
        public int TerminalTransitionCount { get; internal set; }
        public bool IsTerminal => State != PreviewResolutionState.Scanning;

        internal void SetPreviewReports(IEnumerable<ConclusionReport> reports)
        {
            previewReports.Clear();
            previewReports.AddRange(reports);
        }

        internal void SetCompleteReports(IEnumerable<ConclusionReport> reports)
        {
            completeReports.Clear();
            completeReports.AddRange(reports);
        }

        internal void SetSystemDisplays(IEnumerable<RuntimeSystemDisplay> displays)
        {
            systemDisplays.Clear();
            systemDisplays.AddRange(displays);
        }

        internal void SetBirthPlanetAttributions(
            IReadOnlyList<NormalizedBirthPlanetEvidence>? attributions)
        {
            birthPlanetAttributions.Clear();
            HasCompleteBirthPlanetAttribution = attributions != null;
            if (attributions != null)
                birthPlanetAttributions.AddRange(attributions);
        }

        internal void SetSystemCandidates(RuntimeSystemCandidates? candidates)
        {
            SystemCandidates = candidates;
        }

        internal void SetHomePlanetTopology(NormalizedHomePlanetTopology? topology)
        {
            HomePlanetTopology = topology;
        }

        internal void SetDarkFogOccupation(RuntimeDarkFogOccupation? occupation)
        {
            DarkFogOccupation = occupation;
        }

        internal void SetHomeSystemBodyInventory(HomeSystemBodyInventory? inventory)
        {
            HomeSystemBodyInventory = inventory;
        }

        internal void SetHomeSystemResources(HomeSystemResourceStatistics? resources)
        {
            HomeSystemResources = resources;
        }

        internal void SetClusterResources(ClusterResourceStatistics? resources)
        {
            ClusterResources = resources;
        }

        internal void SetNearbyDeuteriumGasGiant(
            NearbyDeuteriumGasGiantSelection? selection)
        {
            NearbyDeuteriumGasGiant = selection;
        }

        internal void SetNotableStars(NotableStarStatistics? statistics)
        {
            NotableStars = statistics;
        }

        internal void SetCachedPayloadSourceIdentity(PreviewGenerationIdentity? identity)
        {
            CachedPayloadSourceIdentity = identity;
        }
    }

    /// <summary>
    /// Owns one cache-or-scan resolution attempt for the current completed
    /// preview load. The caller advances it once per Unity frame. Planet work
    /// alternates with a recovery-only frame while a complete scan is active.
    /// </summary>
    public sealed class PreviewResolutionCoordinator : IDisposable
    {
        private readonly PreviewSessionLifecycle lifecycle;
        private readonly PreviewScanCoordinator previewCoordinator;
        private readonly CompleteClusterRawCoordinator completeCoordinator;
        private readonly CompleteClusterConclusionCache cache;
        private PreviewResolutionAttempt? currentAttempt;
        private CompleteClusterRawOperation? currentOperation;

        public PreviewResolutionCoordinator(
            PreviewSessionLifecycle lifecycle,
            PreviewScanCoordinator previewCoordinator,
            CompleteClusterRawCoordinator completeCoordinator,
            CompleteClusterConclusionCache cache)
        {
            this.lifecycle = lifecycle ?? throw new ArgumentNullException(nameof(lifecycle));
            this.previewCoordinator = previewCoordinator ??
                throw new ArgumentNullException(nameof(previewCoordinator));
            this.completeCoordinator = completeCoordinator ??
                throw new ArgumentNullException(nameof(completeCoordinator));
            this.cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public PreviewResolutionAttempt? CurrentPublishedAttempt =>
            currentAttempt != null && lifecycle.CanPublish(currentAttempt.Session)
                ? currentAttempt
                : null;

        public PreviewLoadTransition ObserveCompletedLoad(
            long loadSequence,
            PreviewGenerationIdentity identity,
            PreviewScanRequest request)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            ValidateRequestIdentity(identity, request);

            PreviewLoadTransition transition = lifecycle.ObserveCompletedLoad(
                loadSequence,
                identity);
            if (transition.Disposition != PreviewLoadDisposition.SessionCreated)
                return transition;

            CancelCurrent();
            PreviewSession session = transition.CurrentSession ??
                throw new InvalidOperationException("A created preview load requires a session.");
            currentAttempt = new PreviewResolutionAttempt(session);

            RuntimeScanResult preview = previewCoordinator.TryScan(request, session.Lifetime);
            currentAttempt.SetPreviewReports(preview.Reports);
            currentAttempt.SetSystemDisplays(preview.SystemDisplays);
            currentAttempt.SetBirthPlanetAttributions(preview.BirthPlanetAttributions);
            currentAttempt.SetHomePlanetTopology(preview.HomePlanetTopology);
            currentAttempt.SetSystemCandidates(preview.SystemCandidates);
            currentAttempt.SetDarkFogOccupation(preview.DarkFogOccupation);
            currentAttempt.SetHomeSystemBodyInventory(preview.HomeSystemBodyInventory);
            currentAttempt.SetNearbyDeuteriumGasGiant(
                preview.NearbyDeuteriumGasGiant);
            currentAttempt.SetNotableStars(preview.NotableStars);
            if (preview.HomePlanetDisplayDesignation != null)
            {
                currentAttempt.Session.SetHomePlanetDisplayDesignation(
                    preview.HomePlanetDisplayDesignation);
            }
            if (preview.Status != RuntimeScanStatus.Success || preview.Fingerprint == null)
            {
                Finish(currentAttempt, preview.Status, preview.Code, preview.Message);
                return transition;
            }
            if (!FingerprintMatches(identity, preview.Fingerprint))
            {
                Finish(
                    currentAttempt,
                    PreviewResolutionState.Incompatible,
                    "preview-identity-changed",
                    "The runtime fingerprint changed while the preview load was being resolved.");
                return transition;
            }

            if (cache.TryRead(identity, preview.Fingerprint, out CachedCompleteClusterConclusions? hit) &&
                hit != null)
            {
                currentAttempt.SetCompleteReports(hit.Reports);
                currentAttempt.SetHomeSystemResources(hit.HomeSystemResources);
                currentAttempt.SetClusterResources(hit.ClusterResources);
                currentAttempt.SetCachedPayloadSourceIdentity(hit.Identity);
                currentAttempt.ExpectedPlanets = hit.Coverage.ExpectedPlanets;
                currentAttempt.CompletedPlanets = hit.Coverage.CompletedPlanets;
                Finish(
                    currentAttempt,
                    PreviewResolutionState.Cached,
                    "cache-hit",
                    "Complete conclusions were loaded from the validated local cache.");
                return transition;
            }

            currentOperation = completeCoordinator.TryStart(request, session.Lifetime);
            CopyProgress(currentAttempt, currentOperation);
            if (currentOperation.State == CompleteClusterRawOperationState.Completed)
                CompleteOperation(currentAttempt, identity);
            else
            {
                currentAttempt.Code = "complete-scan";
                currentAttempt.Message = "Complete cluster evidence is being scanned.";
            }
            return transition;
        }

        public void AdvanceCurrent()
        {
            PreviewResolutionAttempt? attempt = currentAttempt;
            CompleteClusterRawOperation? operation = currentOperation;
            if (attempt == null || operation == null || attempt.IsTerminal)
                return;

            if (!lifecycle.CanPublish(attempt.Session))
            {
                CancelCurrent();
                return;
            }

            operation.Advance();
            CopyProgress(attempt, operation);
            if (operation.State == CompleteClusterRawOperationState.Completed)
                CompleteOperation(attempt, attempt.Session.Identity);
        }

        public PreviewResolutionAttempt? ExitPreview()
        {
            PreviewResolutionAttempt? retired = currentAttempt;
            lifecycle.ExitPreview();
            CancelCurrent();
            currentAttempt = null;
            return retired;
        }

        public void Dispose()
        {
            ExitPreview();
        }

        private void CompleteOperation(
            PreviewResolutionAttempt attempt,
            PreviewGenerationIdentity identity)
        {
            CompleteClusterRawOperation operation = currentOperation ??
                throw new InvalidOperationException("The complete scan operation is unavailable.");
            CompleteClusterRawResult result = operation.Result ??
                throw new InvalidOperationException("A completed scan requires a result.");
            currentOperation = null;
            operation.Dispose();

            if (result.Status == RuntimeScanStatus.Success)
            {
                attempt.SetCompleteReports(result.Reports.Where(report =>
                    (report.Stage == EvidenceStage.BirthSystemRaw &&
                        report.Context == ConclusionContext.FreshStart) ||
                    report.Stage == EvidenceStage.CompleteClusterRaw));
                attempt.SetHomeSystemResources(result.HomeSystemResources);
                attempt.SetClusterResources(result.ClusterResources);
                attempt.CacheStored = cache.TryStore(identity, result);
                Finish(
                    attempt,
                    PreviewResolutionState.Complete,
                    attempt.CacheStored ? "complete" : "complete-cache-write-failed",
                    attempt.CacheStored
                        ? "Complete conclusions were scanned and cached."
                        : "Complete conclusions were scanned, but the cache write failed.");
                return;
            }
            Finish(attempt, result.Status, result.Code, result.Message);
        }

        private void CancelCurrent()
        {
            if (currentAttempt == null || currentAttempt.IsTerminal)
            {
                currentOperation?.Dispose();
                currentOperation = null;
                return;
            }

            if (currentOperation != null)
            {
                currentOperation.Dispose();
                CopyProgress(currentAttempt, currentOperation);
                CompleteClusterRawResult? result = currentOperation.Result;
                currentOperation = null;
                Finish(
                    currentAttempt,
                    PreviewResolutionState.Cancelled,
                    result?.Code ?? "cancelled",
                    result?.Message ?? "The obsolete preview resolution was cancelled.");
            }
            else
            {
                Finish(
                    currentAttempt,
                    PreviewResolutionState.Cancelled,
                    "cancelled",
                    "The obsolete preview resolution was cancelled.");
            }
        }

        private static void CopyProgress(
            PreviewResolutionAttempt attempt,
            CompleteClusterRawOperation operation)
        {
            attempt.ExpectedPlanets = operation.ExpectedPlanets;
            attempt.CompletedPlanets = operation.CompletedPlanets;
        }

        private static void Finish(
            PreviewResolutionAttempt attempt,
            RuntimeScanStatus status,
            string code,
            string message)
        {
            Finish(attempt, status switch
            {
                RuntimeScanStatus.Busy => PreviewResolutionState.Busy,
                RuntimeScanStatus.Incompatible => PreviewResolutionState.Incompatible,
                RuntimeScanStatus.Cancelled => PreviewResolutionState.Cancelled,
                RuntimeScanStatus.Failed => PreviewResolutionState.Failed,
                _ => throw new InvalidOperationException("Success requires an explicit terminal outcome.")
            }, code, message);
        }

        private static void Finish(
            PreviewResolutionAttempt attempt,
            PreviewResolutionState state,
            string code,
            string message)
        {
            if (attempt.IsTerminal)
                return;
            attempt.State = state;
            attempt.Code = code;
            attempt.Message = message;
            attempt.TerminalTransitionCount++;
        }

        private static void ValidateRequestIdentity(
            PreviewGenerationIdentity identity,
            PreviewScanRequest request)
        {
            GenerationIdentity galaxy = identity.GalaxyIdentity;
            if (galaxy.GalaxySeed != request.GalaxySeed ||
                galaxy.RequestedStarCount != request.RequestedStarCount ||
                !String.Equals(galaxy.CreationVersion, request.CreationVersion, StringComparison.Ordinal) ||
                identity.ResourceMultiplier != request.ResourceMultiplier ||
                identity.CombatMode != request.CombatMode ||
                !String.Equals(identity.CombatSettingsKey, request.CombatSettingsKey, StringComparison.Ordinal) ||
                identity.InitialColonize != request.InitialColonize ||
                identity.MaxDensity != request.MaxDensity)
            {
                throw new ArgumentException(
                    "The preview request does not match the completed-load identity.",
                    nameof(request));
            }
        }

        private static bool FingerprintMatches(
            PreviewGenerationIdentity identity,
            RuntimeFingerprint fingerprint)
        {
            GenerationIdentity galaxy = identity.GalaxyIdentity;
            return String.Equals(galaxy.GameVersion, fingerprint.GameVersion, StringComparison.Ordinal) &&
                galaxy.GalaxyAlgorithm == fingerprint.GalaxyAlgorithm &&
                String.Equals(galaxy.AssemblySha256, fingerprint.AssemblySha256, StringComparison.OrdinalIgnoreCase) &&
                String.Equals(galaxy.OrderedThemeIds, fingerprint.OrderedThemeIdsKey, StringComparison.Ordinal) &&
                String.Equals(galaxy.ScannerCompatibilityVersion, fingerprint.ScannerCompatibilityVersion, StringComparison.Ordinal);
        }
    }
}
