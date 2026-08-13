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

        public static string RequiredSha256(string? path, string source)
        {
            if (String.IsNullOrWhiteSpace(source))
                throw new ArgumentException("A filesystem source is required.", nameof(source));
            if (String.IsNullOrWhiteSpace(path))
            {
                throw new RuntimeFilesystemException(
                    "required-file-unavailable",
                    source,
                    "The required runtime file could not be read.",
                    RuntimeFilesystemDiagnostics.Format(
                        "hash-file",
                        source,
                        "InvalidPath: The file path was blank."));
            }
            try
            {
                using Stream stream = File.OpenRead(path);
                using SHA256 hash = SHA256.Create();
                return BitConverter.ToString(hash.ComputeHash(stream))
                    .Replace("-", String.Empty);
            }
            catch (Exception exception) when (IsExpectedFileFailure(exception))
            {
                throw new RuntimeFilesystemException(
                    "required-file-unavailable",
                    source,
                    "The required runtime file could not be read.",
                    RuntimeFilesystemDiagnostics.Format(
                        "hash-file",
                        source,
                        exception));
            }
        }

        public static IReadOnlyList<string> Inventory(
            string? directoryPath,
            string searchPattern,
            Action<string>? reportDiagnostic = null,
            string source = "runtime-inventory")
        {
            if (String.IsNullOrWhiteSpace(directoryPath) ||
                String.IsNullOrWhiteSpace(searchPattern))
            {
                reportDiagnostic?.Invoke(RuntimeFilesystemDiagnostics.Format(
                    "inventory-directory",
                    source,
                    "InvalidPath: The inventory path or pattern was blank."));
                return Array.Empty<string>();
            }

            string[] paths;
            try
            {
                if (!Directory.Exists(directoryPath))
                {
                    reportDiagnostic?.Invoke(RuntimeFilesystemDiagnostics.Format(
                        "inventory-directory",
                        source,
                        "DirectoryNotFound: The inventory directory was unavailable."));
                    return new[] { "inventory:" + Unavailable };
                }
                paths = Directory.GetFiles(
                    directoryPath,
                    searchPattern,
                    SearchOption.TopDirectoryOnly);
            }
            catch (Exception exception) when (IsExpectedFileFailure(exception))
            {
                reportDiagnostic?.Invoke(RuntimeFilesystemDiagnostics.Format(
                    "inventory-directory",
                    source,
                    exception));
                return new[] { "inventory:" + Unavailable };
            }

            return paths
                .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
                .Select(path =>
                {
                    bool readable = TrySha256(
                        path,
                        File.OpenRead,
                        out string digest,
                        out string? diagnostic);
                    if (!readable && diagnostic != null)
                    {
                        reportDiagnostic?.Invoke(RuntimeFilesystemDiagnostics.Format(
                            "inventory-file",
                            source,
                            diagnostic));
                    }
                    return Path.GetFileName(path) + ":" +
                        (readable ? digest : Unavailable);
                })
                .ToArray();
        }

        internal static bool TrySha256(
            string? path,
            Func<string, Stream> openRead,
            out string digest)
        {
            return TrySha256(path, openRead, out digest, out _);
        }

        internal static bool TrySha256(
            string? path,
            Func<string, Stream> openRead,
            out string digest,
            out string? diagnostic)
        {
            if (openRead == null)
                throw new ArgumentNullException(nameof(openRead));

            digest = Unavailable;
            diagnostic = null;
            if (String.IsNullOrWhiteSpace(path))
            {
                diagnostic = RuntimeFilesystemDiagnostics.Format(
                    "hash-file",
                    "runtime-file",
                    "InvalidPath: The file path was blank.");
                return false;
            }

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
                diagnostic = RuntimeFilesystemDiagnostics.Format(
                    "hash-file",
                    "runtime-file",
                    exception);
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
