using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public sealed class PreviewScanRequest
    {
        public PreviewScanRequest(
            int galaxySeed,
            int requestedStarCount,
            string creationVersion,
            decimal resourceMultiplier,
            CombatMode combatMode,
            string combatSettingsKey)
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

            GalaxySeed = galaxySeed;
            RequestedStarCount = requestedStarCount;
            CreationVersion = creationVersion;
            ResourceMultiplier = resourceMultiplier;
            CombatMode = combatMode;
            CombatSettingsKey = combatSettingsKey;
        }

        public int GalaxySeed { get; }
        public int RequestedStarCount { get; }
        public string CreationVersion { get; }
        public decimal ResourceMultiplier { get; }
        public CombatMode CombatMode { get; }
        public string CombatSettingsKey { get; }
    }

    public sealed class RuntimeFingerprint
    {
        private readonly string[] orderedThemeIds;
        private readonly string[] loadedGenerationModIds;

        public RuntimeFingerprint(
            string gameVersion,
            int galaxyAlgorithm,
            string assemblySha256,
            IEnumerable<string> orderedThemeIds,
            string scannerCompatibilityVersion,
            string scannerContractVersion,
            bool requiredMembersAvailable,
            string? missingMember,
            IEnumerable<string>? loadedGenerationModIds)
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

    public sealed class RuntimeTopologySnapshot
    {
        public RuntimeTopologySnapshot(
            string birthSystemIdentifier,
            int sharedBirthGiantBodies,
            int generatedStarCount,
            string? unknownEnumType = null,
            int? unknownEnumValue = null)
        {
            if (String.IsNullOrWhiteSpace(birthSystemIdentifier))
                throw new ArgumentException("Birth system identifier is required.", nameof(birthSystemIdentifier));
            if (sharedBirthGiantBodies < 1)
                throw new ArgumentOutOfRangeException(nameof(sharedBirthGiantBodies));
            if (generatedStarCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(generatedStarCount));
            if ((unknownEnumType == null) != (!unknownEnumValue.HasValue))
                throw new ArgumentException("Unknown enum type and raw value must be supplied together.");

            BirthSystemIdentifier = birthSystemIdentifier;
            SharedBirthGiantBodies = sharedBirthGiantBodies;
            GeneratedStarCount = generatedStarCount;
            UnknownEnumType = unknownEnumType;
            UnknownEnumValue = unknownEnumValue;
        }

        public string BirthSystemIdentifier { get; }
        public int SharedBirthGiantBodies { get; }
        public int GeneratedStarCount { get; }
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
        RuntimeTopologySnapshot GeneratePreview(
            PreviewScanRequest request,
            CancellationToken cancellationToken,
            Action<string> recordTrace);
    }
}
