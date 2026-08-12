using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public sealed record RuntimeSystemDisplay
    {
        public RuntimeSystemDisplay(
            string identifier,
            string displayName,
            string starType)
        {
            Identifier = Required(identifier, nameof(identifier));
            DisplayName = Required(displayName, nameof(displayName));
            StarType = Required(starType, nameof(starType));
        }

        public string Identifier { get; }
        public string DisplayName { get; }
        public string StarType { get; }

        private static string Required(string value, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value is required.", parameterName);
            return value;
        }
    }

    public sealed record RuntimeSystemCandidate
    {
        public RuntimeSystemCandidate(
            string identifier,
            string displayName,
            decimal decisiveValue)
        {
            Identifier = Required(identifier, nameof(identifier));
            DisplayName = Required(displayName, nameof(displayName));
            if (decisiveValue < 0)
                throw new ArgumentOutOfRangeException(nameof(decisiveValue));
            DecisiveValue = decisiveValue;
        }

        public string Identifier { get; }
        public string DisplayName { get; }
        public decimal DecisiveValue { get; }

        private static string Required(string value, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value is required.", parameterName);
            return value;
        }
    }

    public sealed class RuntimeSystemCandidates
    {
        private const int MaximumCandidates = 3;
        private readonly RuntimeSystemCandidate[]? energy;
        private readonly RuntimeSystemCandidate[]? shellRadius;
        private readonly RuntimeSystemCandidate[]? containedOrbits;

        private RuntimeSystemCandidates(
            RuntimeSystemCandidate[]? energy,
            RuntimeSystemCandidate[]? shellRadius,
            RuntimeSystemCandidate[]? containedOrbits)
        {
            this.energy = energy;
            this.shellRadius = shellRadius;
            this.containedOrbits = containedOrbits;
        }

        public IReadOnlyList<RuntimeSystemCandidate>? Energy => ReadOnly(energy);
        public IReadOnlyList<RuntimeSystemCandidate>? ShellRadius => ReadOnly(shellRadius);
        public IReadOnlyList<RuntimeSystemCandidate>? ContainedOrbits =>
            ReadOnly(containedOrbits);

        internal static RuntimeSystemCandidates Project(
            IReadOnlyList<NormalizedSystemEvidence> systems,
            IReadOnlyList<RuntimeSystemDisplay> displays)
        {
            var displayByIdentifier = displays.ToDictionary(
                value => value.Identifier,
                StringComparer.Ordinal);
            return new RuntimeSystemCandidates(
                Rank(systems, displayByIdentifier, system => system.DysonLuminosity),
                Rank(systems, displayByIdentifier, system => system.MaximumShellRadius),
                Rank(systems, displayByIdentifier, system => system.ContainedOrbitCount));
        }

        private static RuntimeSystemCandidate[]? Rank(
            IReadOnlyList<NormalizedSystemEvidence> systems,
            IReadOnlyDictionary<string, RuntimeSystemDisplay> displays,
            Func<NormalizedSystemEvidence, decimal?> selectValue)
        {
            if (systems.Count == 0 || systems.Any(system =>
                !selectValue(system).HasValue ||
                !displays.ContainsKey(system.Subject.Identifier)))
            {
                return null;
            }

            return systems
                .OrderByDescending(system => selectValue(system)!.Value)
                .ThenBy(system => system.Subject.Identifier, StringComparer.Ordinal)
                .Take(MaximumCandidates)
                .Select(system => new RuntimeSystemCandidate(
                    system.Subject.Identifier,
                    displays[system.Subject.Identifier].DisplayName,
                    selectValue(system)!.Value))
                .ToArray();
        }

        private static RuntimeSystemCandidate[]? Rank(
            IReadOnlyList<NormalizedSystemEvidence> systems,
            IReadOnlyDictionary<string, RuntimeSystemDisplay> displays,
            Func<NormalizedSystemEvidence, long?> selectValue)
        {
            return Rank(
                systems,
                displays,
                system => selectValue(system).HasValue
                    ? Convert.ToDecimal(selectValue(system)!.Value)
                    : null);
        }

        private static RuntimeSystemCandidate[]? Rank(
            IReadOnlyList<NormalizedSystemEvidence> systems,
            IReadOnlyDictionary<string, RuntimeSystemDisplay> displays,
            Func<NormalizedSystemEvidence, int?> selectValue)
        {
            return Rank(
                systems,
                displays,
                system => selectValue(system).HasValue
                    ? Convert.ToDecimal(selectValue(system)!.Value)
                    : null);
        }

        private static IReadOnlyList<RuntimeSystemCandidate>? ReadOnly(
            RuntimeSystemCandidate[]? candidates)
        {
            return candidates == null
                ? null
                : Array.AsReadOnly((RuntimeSystemCandidate[])candidates.Clone());
        }
    }

    public sealed class PreviewScanRequest
    {
        public PreviewScanRequest(
            int galaxySeed,
            int requestedStarCount,
            string creationVersion,
            decimal resourceMultiplier,
            CombatMode combatMode,
            string combatSettingsKey,
            decimal initialColonize = 1m,
            decimal maxDensity = 1m)
        {
            if (galaxySeed < 0 || galaxySeed > 99_999_999)
                throw new ArgumentOutOfRangeException(nameof(galaxySeed));
            if (requestedStarCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(requestedStarCount));
            if (String.IsNullOrWhiteSpace(creationVersion))
                throw new ArgumentException("Creation version is required.", nameof(creationVersion));
            if (resourceMultiplier <= 0)
                throw new ArgumentOutOfRangeException(nameof(resourceMultiplier));
            if (String.IsNullOrWhiteSpace(combatSettingsKey))
                throw new ArgumentException("Combat settings key is required.", nameof(combatSettingsKey));
            if (initialColonize < 0)
                throw new ArgumentOutOfRangeException(nameof(initialColonize));
            if (maxDensity < 0)
                throw new ArgumentOutOfRangeException(nameof(maxDensity));
            string expectedCombatKey = CombatSettingsKeyFor(initialColonize, maxDensity);
            if (!String.Equals(combatSettingsKey, expectedCombatKey, StringComparison.Ordinal))
                throw new ArgumentException(
                    "Combat settings key does not match the supplied preview settings.",
                    nameof(combatSettingsKey));

            GalaxySeed = galaxySeed;
            RequestedStarCount = requestedStarCount;
            CreationVersion = creationVersion;
            ResourceMultiplier = resourceMultiplier;
            CombatMode = combatMode;
            CombatSettingsKey = combatSettingsKey;
            InitialColonize = initialColonize;
            MaxDensity = maxDensity;
        }

        public int GalaxySeed { get; }
        public int RequestedStarCount { get; }
        public string CreationVersion { get; }
        public decimal ResourceMultiplier { get; }
        public CombatMode CombatMode { get; }
        public string CombatSettingsKey { get; }
        public decimal InitialColonize { get; }
        public decimal MaxDensity { get; }

        public static string CombatSettingsKeyFor(
            decimal initialColonize,
            decimal maxDensity)
        {
            if (initialColonize == 1m && maxDensity == 1m)
                return ConclusionDefinition.ReferenceCombatSettingsKey;
            return "preview-combat:initialColonize=" +
                initialColonize.ToString(System.Globalization.CultureInfo.InvariantCulture) +
                ";maxDensity=" +
                maxDensity.ToString(System.Globalization.CultureInfo.InvariantCulture);
        }
    }

    public sealed class RuntimeFingerprint
    {
        private readonly string[] orderedThemeIds;
        private readonly string[] loadedGenerationModIds;
        private readonly string[] loadedPatcherIds;

        public RuntimeFingerprint(
            string gameVersion,
            int galaxyAlgorithm,
            string assemblySha256,
            IEnumerable<string> orderedThemeIds,
            string scannerCompatibilityVersion,
            string scannerContractVersion,
            bool requiredMembersAvailable,
            string? missingMember,
            IEnumerable<string>? loadedGenerationModIds,
            string generationMethodIlSha256 = "unavailable",
            IEnumerable<string>? loadedPatcherIds = null)
        {
            GameVersion = Required(gameVersion, nameof(gameVersion));
            GalaxyAlgorithm = galaxyAlgorithm;
            AssemblySha256 = Required(assemblySha256, nameof(assemblySha256));
            this.orderedThemeIds = CopyRequired(orderedThemeIds, nameof(orderedThemeIds));
            ScannerCompatibilityVersion = Required(
                scannerCompatibilityVersion,
                nameof(scannerCompatibilityVersion));
            ScannerContractVersion = Required(scannerContractVersion, nameof(scannerContractVersion));
            RequiredMembersAvailable = requiredMembersAvailable;
            MissingMember = missingMember;
            this.loadedGenerationModIds = loadedGenerationModIds == null
                ? Array.Empty<string>()
                : loadedGenerationModIds.Where(value => !String.IsNullOrWhiteSpace(value)).ToArray();
            GenerationMethodIlSha256 = Required(
                generationMethodIlSha256,
                nameof(generationMethodIlSha256));
            this.loadedPatcherIds = loadedPatcherIds == null
                ? Array.Empty<string>()
                : loadedPatcherIds.Where(value => !String.IsNullOrWhiteSpace(value))
                    .OrderBy(value => value, StringComparer.Ordinal)
                    .ToArray();
        }

        public string GameVersion { get; }
        public int GalaxyAlgorithm { get; }
        public string AssemblySha256 { get; }
        public IReadOnlyList<string> OrderedThemeIds => Array.AsReadOnly((string[])orderedThemeIds.Clone());
        public string OrderedThemeIdsKey => String.Join(",", orderedThemeIds);
        public string ScannerCompatibilityVersion { get; }
        public string ScannerContractVersion { get; }
        public bool RequiredMembersAvailable { get; }
        public string? MissingMember { get; }
        public IReadOnlyList<string> LoadedGenerationModIds =>
            Array.AsReadOnly((string[])loadedGenerationModIds.Clone());
        public string GenerationMethodIlSha256 { get; }
        public IReadOnlyList<string> LoadedPatcherIds =>
            Array.AsReadOnly((string[])loadedPatcherIds.Clone());

        private static string Required(string value, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value is required.", parameterName);
            return value;
        }

        private static string[] CopyRequired(IEnumerable<string> values, string parameterName)
        {
            if (values == null)
                throw new ArgumentNullException(parameterName);
            string[] result = values.ToArray();
            if (result.Length == 0 || result.Any(String.IsNullOrWhiteSpace))
                throw new ArgumentException("At least one nonblank value is required.", parameterName);
            return result;
        }
    }

    public sealed class RuntimePreviewSnapshot
    {
        private readonly NormalizedSystemEvidence[] systems;
        private readonly NormalizedSystemDistance[] systemDistances;
        private readonly RuntimeSystemDisplay[] systemDisplays;
        private readonly NormalizedBirthPlanetEvidence[]? birthPlanetAttributions;

        public RuntimePreviewSnapshot(
            string birthSystemIdentifier,
            int generatedStarCount,
            IEnumerable<NormalizedSystemEvidence> systems,
            IEnumerable<NormalizedSystemDistance> systemDistances,
            string? unknownEnumType = null,
            int? unknownEnumValue = null,
            IEnumerable<RuntimeSystemDisplay>? systemDisplays = null)
        {
            if (String.IsNullOrWhiteSpace(birthSystemIdentifier))
                throw new ArgumentException("Birth system identifier is required.", nameof(birthSystemIdentifier));
            if (generatedStarCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(generatedStarCount));
            if (systems == null)
                throw new ArgumentNullException(nameof(systems));
            if (systemDistances == null)
                throw new ArgumentNullException(nameof(systemDistances));
            if ((unknownEnumType == null) != (!unknownEnumValue.HasValue))
                throw new ArgumentException("Unknown enum type and raw value must be supplied together.");

            this.systems = systems.ToArray();
            this.systemDistances = systemDistances.ToArray();
            this.systemDisplays = (systemDisplays ?? Array.Empty<RuntimeSystemDisplay>())
                .ToArray();
            NormalizedSystemEvidence? birthSystem = this.systems
                .SingleOrDefault(system => system.IsBirthSystem);
            this.birthPlanetAttributions = birthSystem?.BirthPlanets?.ToArray();
            if (this.systemDisplays.Select(value => value.Identifier)
                .Distinct(StringComparer.Ordinal).Count() != this.systemDisplays.Length)
            {
                throw new ArgumentException(
                    "System display identifiers must be unique.",
                    nameof(systemDisplays));
            }
            if (this.systems.Length == 0 && unknownEnumType == null)
                throw new ArgumentException("At least one generated system is required.", nameof(systems));
            SystemCandidates = RuntimeSystemCandidates.Project(
                this.systems,
                this.systemDisplays);
            BirthSystemIdentifier = birthSystemIdentifier;
            GeneratedStarCount = generatedStarCount;
            UnknownEnumType = unknownEnumType;
            UnknownEnumValue = unknownEnumValue;
        }

        public string BirthSystemIdentifier { get; }
        public int GeneratedStarCount { get; }
        public IReadOnlyList<NormalizedSystemEvidence> Systems =>
            Array.AsReadOnly((NormalizedSystemEvidence[])systems.Clone());
        public IReadOnlyList<NormalizedSystemDistance> SystemDistances =>
            Array.AsReadOnly((NormalizedSystemDistance[])systemDistances.Clone());
        public IReadOnlyList<RuntimeSystemDisplay> SystemDisplays =>
            Array.AsReadOnly((RuntimeSystemDisplay[])systemDisplays.Clone());
        public RuntimeSystemCandidates SystemCandidates { get; }
        public IReadOnlyList<NormalizedBirthPlanetEvidence>? BirthPlanetAttributions =>
            birthPlanetAttributions == null
                ? null
                : Array.AsReadOnly(
                    (NormalizedBirthPlanetEvidence[])birthPlanetAttributions.Clone());
        public string? UnknownEnumType { get; }
        public int? UnknownEnumValue { get; }
    }

    public abstract class RuntimeStateLease : IDisposable
    {
        public abstract bool Restored { get; }
        public abstract void Dispose();
    }

    public interface IRuntimePreviewGateway
    {
        int MainThreadId { get; }
        RuntimeFingerprint CaptureFingerprint(PreviewScanRequest request);
        RuntimeStateLease CaptureState();
        RuntimePreviewSnapshot GeneratePreview(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<string> recordTrace);
    }
}
