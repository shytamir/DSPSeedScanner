using System;

namespace DSPSeedScanner.Core
{
    public sealed record GenerationIdentity
    {
        public GenerationIdentity(
            string gameVersion,
            int galaxyAlgorithm,
            string assemblySha256,
            string orderedThemeIds,
            string scannerCompatibilityVersion,
            int galaxySeed,
            int requestedStarCount,
            string creationVersion)
        {
            GameVersion = Required(gameVersion, nameof(gameVersion));
            GalaxyAlgorithm = galaxyAlgorithm;
            AssemblySha256 = Required(assemblySha256, nameof(assemblySha256));
            OrderedThemeIds = Required(orderedThemeIds, nameof(orderedThemeIds));
            ScannerCompatibilityVersion = Required(
                scannerCompatibilityVersion,
                nameof(scannerCompatibilityVersion));

            if (galaxySeed < 0 || galaxySeed > 99_999_999)
                throw new ArgumentOutOfRangeException(nameof(galaxySeed));
            if (requestedStarCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(requestedStarCount));

            GalaxySeed = galaxySeed;
            RequestedStarCount = requestedStarCount;
            CreationVersion = Required(creationVersion, nameof(creationVersion));
        }

        public string GameVersion { get; }

        public int GalaxyAlgorithm { get; }

        public string AssemblySha256 { get; }

        public string OrderedThemeIds { get; }

        public string ScannerCompatibilityVersion { get; }

        public int GalaxySeed { get; }

        public int RequestedStarCount { get; }

        public string CreationVersion { get; }

        private static string Required(string value, string parameterName)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value is required.", parameterName);

            return value;
        }
    }
}
