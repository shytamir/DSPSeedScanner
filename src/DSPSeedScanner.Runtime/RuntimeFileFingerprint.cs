using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.Cryptography;

namespace DSPSeedScanner.Runtime
{
    public static class RuntimeFileFingerprint
    {
        public const string Unavailable = "unavailable";

        public static string FirstReadableSha256(IEnumerable<string?> candidatePaths)
        {
            if (candidatePaths == null)
                throw new ArgumentNullException(nameof(candidatePaths));

            foreach (string? path in candidatePaths)
            {
                if (TrySha256(path, File.OpenRead, out string digest))
                    return digest;
            }
            return Unavailable;
        }

        public static IReadOnlyList<string> Inventory(
            string? directoryPath,
            string searchPattern)
        {
            if (String.IsNullOrWhiteSpace(directoryPath) ||
                String.IsNullOrWhiteSpace(searchPattern))
            {
                return Array.Empty<string>();
            }

            string[] paths;
            try
            {
                if (!Directory.Exists(directoryPath))
                    return Array.Empty<string>();
                paths = Directory.GetFiles(
                    directoryPath,
                    searchPattern,
                    SearchOption.TopDirectoryOnly);
            }
            catch (Exception exception) when (IsExpectedFileFailure(exception))
            {
                return new[] { "inventory:" + Unavailable };
            }

            return paths
                .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
                .Select(path => Path.GetFileName(path) + ":" +
                    (TrySha256(path, File.OpenRead, out string digest)
                        ? digest
                        : Unavailable))
                .ToArray();
        }

        internal static bool TrySha256(
            string? path,
            Func<string, Stream> openRead,
            out string digest)
        {
            if (openRead == null)
                throw new ArgumentNullException(nameof(openRead));

            digest = Unavailable;
            if (String.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                using Stream stream = openRead(path);
                using SHA256 hash = SHA256.Create();
                digest = BitConverter.ToString(hash.ComputeHash(stream))
                    .Replace("-", String.Empty);
                return true;
            }
            catch (Exception exception) when (IsExpectedFileFailure(exception))
            {
                return false;
            }
        }

        private static bool IsExpectedFileFailure(Exception exception) =>
            exception is ArgumentException ||
            exception is IOException ||
            exception is UnauthorizedAccessException ||
            exception is NotSupportedException ||
            exception is SecurityException;
    }
}
