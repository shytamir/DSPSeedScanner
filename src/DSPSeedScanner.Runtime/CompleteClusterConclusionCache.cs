using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public sealed class CompleteClusterCacheKey : IEquatable<CompleteClusterCacheKey>
    {
        private CompleteClusterCacheKey(
            PreviewGenerationIdentity identity,
            RuntimeFingerprint fingerprint,
            string canonicalValue,
            string hash)
        {
            Identity = identity;
            Fingerprint = fingerprint;
            CanonicalValue = canonicalValue;
            Hash = hash;
        }

        public PreviewGenerationIdentity Identity { get; }
        public RuntimeFingerprint Fingerprint { get; }
        public string CanonicalValue { get; }
        public string Hash { get; }
        public string FileName => Hash + CompleteClusterConclusionCache.EntryExtension;

        public static bool TryCreate(
            PreviewGenerationIdentity identity,
            RuntimeFingerprint fingerprint,
            out CompleteClusterCacheKey? key)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));
            if (fingerprint == null)
                throw new ArgumentNullException(nameof(fingerprint));

            GenerationIdentity galaxy = identity.GalaxyIdentity;
            CompatibilityDecision compatibility = CompatibilityPolicy.Evaluate(fingerprint);
            if (!compatibility.Supported ||
                !Enum.IsDefined(typeof(CombatMode), identity.CombatMode) ||
                !String.Equals(galaxy.GameVersion, fingerprint.GameVersion, StringComparison.Ordinal) ||
                galaxy.GalaxyAlgorithm != fingerprint.GalaxyAlgorithm ||
                !String.Equals(galaxy.AssemblySha256, fingerprint.AssemblySha256, StringComparison.Ordinal) ||
                !String.Equals(galaxy.OrderedThemeIds, fingerprint.OrderedThemeIdsKey, StringComparison.Ordinal) ||
                !String.Equals(
                    galaxy.ScannerCompatibilityVersion,
                    fingerprint.ScannerCompatibilityVersion,
                    StringComparison.Ordinal) ||
                !String.Equals(
                    fingerprint.ScannerContractVersion,
                    ConclusionDefinition.ContractVersion,
                    StringComparison.Ordinal))
            {
                key = null;
                return false;
            }

            string canonical = Canonical(identity, fingerprint);
            key = new CompleteClusterCacheKey(
                identity,
                fingerprint,
                canonical,
                Sha256(canonical));
            return true;
        }

        public bool Equals(CompleteClusterCacheKey? other) =>
            other != null && String.Equals(Hash, other.Hash, StringComparison.Ordinal) &&
            String.Equals(CanonicalValue, other.CanonicalValue, StringComparison.Ordinal);

        public override bool Equals(object? obj) => Equals(obj as CompleteClusterCacheKey);

        public override int GetHashCode() => StringComparer.Ordinal.GetHashCode(Hash);

        private static string Canonical(
            PreviewGenerationIdentity identity,
            RuntimeFingerprint fingerprint)
        {
            GenerationIdentity galaxy = identity.GalaxyIdentity;
            var value = new StringBuilder();
            Add(value, "cache-schema", CompleteClusterConclusionCache.SchemaVersion.ToString(CultureInfo.InvariantCulture));
            Add(value, "stage", EvidenceStage.CompleteClusterRaw.ToString());
            Add(value, "game-version", galaxy.GameVersion);
            Add(value, "galaxy-algorithm", galaxy.GalaxyAlgorithm.ToString(CultureInfo.InvariantCulture));
            Add(value, "assembly-sha256", galaxy.AssemblySha256);
            Add(value, "ordered-theme-ids", galaxy.OrderedThemeIds);
            Add(value, "creation-version", galaxy.CreationVersion);
            Add(value, "galaxy-seed", galaxy.GalaxySeed.ToString(CultureInfo.InvariantCulture));
            Add(value, "star-count", galaxy.RequestedStarCount.ToString(CultureInfo.InvariantCulture));
            Add(value, "resource-multiplier", DecimalValue(identity.ResourceMultiplier));
            Add(value, "combat-mode", ((int)identity.CombatMode).ToString(CultureInfo.InvariantCulture));
            Add(value, "combat-settings", identity.CombatSettingsKey);
            Add(value, "initial-colonize", DecimalValue(identity.InitialColonize));
            Add(value, "max-density", DecimalValue(identity.MaxDensity));
            Add(value, "generation-method-il", fingerprint.GenerationMethodIlSha256);
            Add(value, "generation-mods", String.Join(",", fingerprint.LoadedGenerationModIds.OrderBy(item => item, StringComparer.Ordinal)));
            Add(value, "patchers", String.Join(",", fingerprint.LoadedPatcherIds.OrderBy(item => item, StringComparer.Ordinal)));
            Add(value, "scanner-compatibility", fingerprint.ScannerCompatibilityVersion);
            Add(value, "scanner-contract", fingerprint.ScannerContractVersion);
            Add(value, "conclusion-definition", ConclusionDefinition.DefinitionVersion);
            Add(value, "conclusion-contract", ConclusionDefinition.ContractVersion);
            return value.ToString();
        }

        private static void Add(StringBuilder target, string name, string value)
        {
            target.Append(name).Append('=').Append(value.Length).Append(':').Append(value).Append(';');
        }

        private static string DecimalValue(decimal value) =>
            value.ToString("G29", CultureInfo.InvariantCulture);

        private static string Sha256(string value)
        {
            using SHA256 sha = SHA256.Create();
            byte[] digest = sha.ComputeHash(Encoding.UTF8.GetBytes(value));
            var result = new StringBuilder(digest.Length * 2);
            foreach (byte item in digest)
                result.Append(item.ToString("x2", CultureInfo.InvariantCulture));
            return result.ToString();
        }
    }

    public sealed class CachedCompleteClusterConclusions
    {
        internal CachedCompleteClusterConclusions(
            string cacheKeyHash,
            PreviewGenerationIdentity identity,
            CompleteClusterRawCoverage coverage,
            IEnumerable<ConclusionReport> reports)
        {
            CacheKeyHash = cacheKeyHash;
            Identity = identity;
            Coverage = coverage;
            Reports = Array.AsReadOnly(reports.ToArray());
        }

        public string CacheKeyHash { get; }
        public PreviewGenerationIdentity Identity { get; }
        public CompleteClusterRawCoverage Coverage { get; }
        public IReadOnlyList<ConclusionReport> Reports { get; }
    }

    public sealed class CompleteClusterConclusionCache
    {
        internal const int SchemaVersion = 2;
        internal const string EntryExtension = ".dspseedscan";
        private const string Magic = "DSPSeedScanner.CompleteClusterCache";
        private const int MaximumEntryBytes = 512 * 1024;
        private const int MaximumReports = 1024;

        private readonly object sync = new object();
        private readonly int maximumEntries;
        private long lastTouchTicks;

        public CompleteClusterConclusionCache(string directoryPath, int maximumEntries = 128)
        {
            if (String.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("A cache directory is required.", nameof(directoryPath));
            if (maximumEntries <= 0 || maximumEntries > 4096)
                throw new ArgumentOutOfRangeException(nameof(maximumEntries));
            DirectoryPath = Path.GetFullPath(directoryPath);
            this.maximumEntries = maximumEntries;
        }

        public string DirectoryPath { get; }
        public int MaximumEntries => maximumEntries;

        public bool TryRead(
            PreviewGenerationIdentity identity,
            RuntimeFingerprint fingerprint,
            out CachedCompleteClusterConclusions? result)
        {
            result = null;
            if (!CompleteClusterCacheKey.TryCreate(identity, fingerprint, out CompleteClusterCacheKey? key) ||
                key == null)
            {
                return false;
            }

            lock (sync)
            {
                string path = Path.Combine(DirectoryPath, key.FileName);
                if (!File.Exists(path))
                    return false;
                try
                {
                    var info = new FileInfo(path);
                    if (info.Length <= 32 || info.Length > MaximumEntryBytes)
                        throw new InvalidDataException("The cache entry size is invalid.");
                    byte[] entry = File.ReadAllBytes(path);
                    if (entry.Length <= 32 || entry.Length > MaximumEntryBytes)
                        throw new InvalidDataException("The cache entry size changed during read.");
                    int payloadLength = entry.Length - 32;
                    byte[] actualDigest = Digest(entry, 0, payloadLength);
                    if (!BytesEqual(entry, payloadLength, actualDigest))
                        throw new InvalidDataException("The cache entry checksum is invalid.");
                    using var stream = new MemoryStream(entry, 0, payloadLength, writable: false);
                    using var reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
                    CachedCompleteClusterConclusions candidate = Read(reader, key);
                    if (stream.Position != stream.Length || !IsCacheable(key, candidate))
                        throw new InvalidDataException("The cache entry contract is invalid.");
                    Touch(path);
                    result = candidate;
                    return true;
                }
                catch (Exception exception) when (IsRecoverable(exception))
                {
                    TryDelete(path);
                    return false;
                }
            }
        }

        public bool TryStore(
            PreviewGenerationIdentity identity,
            CompleteClusterRawResult result)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));
            if (result == null)
                throw new ArgumentNullException(nameof(result));
            if (result.Fingerprint == null ||
                !CompleteClusterCacheKey.TryCreate(
                    identity,
                    result.Fingerprint,
                    out CompleteClusterCacheKey? key) ||
                key == null ||
                !IsCacheable(key, result))
            {
                return false;
            }

            lock (sync)
            {
                string? temporary = null;
                try
                {
                    ConclusionReport[] cachedReports = result.Reports
                        .Where(report => report.Stage == EvidenceStage.CompleteClusterRaw)
                        .ToArray();
                    Directory.CreateDirectory(DirectoryPath);
                    string destination = Path.Combine(DirectoryPath, key.FileName);
                    temporary = Path.Combine(
                        DirectoryPath,
                        "." + key.Hash + "." + Guid.NewGuid().ToString("N") + ".tmp");
                    byte[] payload;
                    using (var buffer = new MemoryStream())
                    using (var writer = new BinaryWriter(buffer, Encoding.UTF8, leaveOpen: true))
                    {
                        Write(writer, key, result.Coverage, cachedReports);
                        writer.Flush();
                        payload = buffer.ToArray();
                    }
                    byte[] digest = Digest(payload, 0, payload.Length);
                    if (payload.Length + digest.Length > MaximumEntryBytes)
                        throw new InvalidDataException("The cache entry exceeds its size bound.");
                    using (var stream = new FileStream(
                        temporary,
                        FileMode.CreateNew,
                        FileAccess.Write,
                        FileShare.None))
                    {
                        stream.Write(payload, 0, payload.Length);
                        stream.Write(digest, 0, digest.Length);
                        stream.Flush(true);
                    }

                    if (File.Exists(destination))
                        File.Replace(temporary, destination, null, ignoreMetadataErrors: true);
                    else
                        File.Move(temporary, destination);
                    temporary = null;
                    Touch(destination);
                    Trim();
                    return true;
                }
                catch (Exception exception) when (IsRecoverable(exception))
                {
                    return false;
                }
                finally
                {
                    if (temporary != null)
                        TryDelete(temporary);
                }
            }
        }

        public bool Clear()
        {
            lock (sync)
            {
                if (!Directory.Exists(DirectoryPath))
                    return true;
                try
                {
                    foreach (string path in Directory.GetFiles(
                        DirectoryPath,
                        "*" + EntryExtension,
                        SearchOption.TopDirectoryOnly))
                    {
                        File.Delete(path);
                    }
                    foreach (string path in Directory.GetFiles(
                        DirectoryPath,
                        ".*.tmp",
                        SearchOption.TopDirectoryOnly))
                    {
                        File.Delete(path);
                    }
                    return true;
                }
                catch (Exception exception) when (IsRecoverable(exception))
                {
                    return false;
                }
            }
        }

        private static bool IsCacheable(
            CompleteClusterCacheKey key,
            CompleteClusterRawResult result)
        {
            if (result.Status != RuntimeScanStatus.Success ||
                result.Fingerprint == null ||
                !result.StateRestored ||
                !result.Coverage.IsComplete ||
                result.Coverage.ExpectedPlanets <= 0 ||
                result.GalaxySeed != key.Identity.GalaxyIdentity.GalaxySeed ||
                result.AffectedPlanetId.HasValue ||
                result.RawDiagnostic != null ||
                result.Reports.Count == 0 ||
                result.Reports.Count > MaximumReports)
            {
                return false;
            }

            if (!CompleteClusterCacheKey.TryCreate(
                    key.Identity,
                    result.Fingerprint,
                    out CompleteClusterCacheKey? resultKey) ||
                resultKey == null || !key.Equals(resultKey))
            {
                return false;
            }

            EvaluationSettings settings = new EvaluationSettings(
                key.Identity.ResourceMultiplier,
                key.Identity.CombatMode,
                key.Identity.CombatSettingsKey);
            bool hasCompleteClusterReport = false;
            foreach (ConclusionReport report in result.Reports)
            {
                if (!IsCurrentReport(key, settings, report))
                    return false;
                hasCompleteClusterReport |= report.Stage == EvidenceStage.CompleteClusterRaw &&
                    report.Coverage.IsComplete;
            }
            return hasCompleteClusterReport;
        }

        private static bool IsCacheable(
            CompleteClusterCacheKey key,
            CachedCompleteClusterConclusions result)
        {
            if (!String.Equals(result.CacheKeyHash, key.Hash, StringComparison.Ordinal) ||
                !result.Identity.Equals(key.Identity) ||
                !result.Coverage.IsComplete ||
                result.Coverage.ExpectedPlanets <= 0 ||
                result.Reports.Count == 0 ||
                result.Reports.Count > MaximumReports)
            {
                return false;
            }

            var settings = new EvaluationSettings(
                key.Identity.ResourceMultiplier,
                key.Identity.CombatMode,
                key.Identity.CombatSettingsKey);
            return result.Reports.All(report =>
                report.Stage == EvidenceStage.CompleteClusterRaw &&
                report.Coverage.IsComplete &&
                IsCurrentReport(key, settings, report));
        }

        private static bool IsCurrentReport(
            CompleteClusterCacheKey key,
            EvaluationSettings settings,
            ConclusionReport report) =>
            report.Identity == key.Identity.GalaxyIdentity &&
            report.Settings == settings &&
            String.Equals(
                report.ContractVersion,
                ConclusionDefinition.ContractVersion,
                StringComparison.Ordinal) &&
            String.Equals(
                report.DefinitionVersion,
                ConclusionDefinition.DefinitionVersion,
                StringComparison.Ordinal);

        private static void Write(
            BinaryWriter writer,
            CompleteClusterCacheKey key,
            CompleteClusterRawCoverage coverage,
            IReadOnlyList<ConclusionReport> reports)
        {
            writer.Write(Magic);
            writer.Write(SchemaVersion);
            writer.Write(key.CanonicalValue);
            writer.Write(coverage.ExpectedPlanets);
            writer.Write(reports.Count);
            foreach (ConclusionReport report in reports)
                WriteReport(writer, report);
        }

        private static CachedCompleteClusterConclusions Read(
            BinaryReader reader,
            CompleteClusterCacheKey key)
        {
            if (!String.Equals(reader.ReadString(), Magic, StringComparison.Ordinal))
                throw new InvalidDataException("The cache magic is invalid.");
            if (reader.ReadInt32() != SchemaVersion)
                throw new InvalidDataException("The cache schema is obsolete.");
            if (!String.Equals(reader.ReadString(), key.CanonicalValue, StringComparison.Ordinal))
                throw new InvalidDataException("The cache identity is not current.");

            int expectedPlanets = Positive(reader.ReadInt32(), 4096, "planet count");
            int reportCount = Positive(reader.ReadInt32(), MaximumReports, "report count");
            var reports = new List<ConclusionReport>(reportCount);
            for (int index = 0; index < reportCount; index++)
                reports.Add(ReadReport(reader, key.Identity));

            return new CachedCompleteClusterConclusions(
                key.Hash,
                key.Identity,
                new CompleteClusterRawCoverage(
                    CoverageState.Complete,
                    expectedPlanets,
                    expectedPlanets),
                reports);
        }

        private static void WriteReport(BinaryWriter writer, ConclusionReport report)
        {
            WriteCoverage(writer, report.Coverage);
            writer.Write(report.ConclusionId);
            writer.Write((int)report.Context);
            WriteSubject(writer, report.Subject);
            writer.Write((int)report.Outcome);
            writer.Write(report.DecisiveFact != null);
            if (report.DecisiveFact != null)
            {
                writer.Write(report.DecisiveFact.FactId);
                writer.Write(report.DecisiveFact.Value);
                writer.Write(report.DecisiveFact.Unit);
            }
            writer.Write(report.DiagnosticCause != null);
            if (report.DiagnosticCause != null)
            {
                writer.Write(report.DiagnosticCause.Code);
                writer.Write(report.DiagnosticCause.Message);
            }
            WriteOptionalString(writer, report.SourceConclusionId);
        }

        private static ConclusionReport ReadReport(
            BinaryReader reader,
            PreviewGenerationIdentity identity)
        {
            EvidenceCoverage coverage = ReadCoverage(reader);
            string conclusionId = Required(reader.ReadString(), "conclusion ID");
            ConclusionContext context = EnumValue<ConclusionContext>(reader.ReadInt32());
            ConclusionSubject subject = ReadSubject(reader);
            ComponentOutcome outcome = EnumValue<ComponentOutcome>(reader.ReadInt32());
            DecisiveFact? fact = reader.ReadBoolean()
                ? new DecisiveFact(
                    Required(reader.ReadString(), "fact ID"),
                    Required(reader.ReadString(), "fact value"),
                    Required(reader.ReadString(), "fact unit"))
                : null;
            DiagnosticCause? diagnostic = reader.ReadBoolean()
                ? new DiagnosticCause(
                    Required(reader.ReadString(), "diagnostic code"),
                    Required(reader.ReadString(), "diagnostic message"))
                : null;
            return new ConclusionReport(
                identity.GalaxyIdentity,
                new EvaluationSettings(
                    identity.ResourceMultiplier,
                    identity.CombatMode,
                    identity.CombatSettingsKey),
                coverage,
                conclusionId,
                context,
                ConclusionDefinition.ContractVersion,
                ConclusionDefinition.DefinitionVersion,
                subject,
                outcome,
                fact,
                diagnostic,
                ReadOptionalString(reader));
        }

        private static void WriteCoverage(BinaryWriter writer, EvidenceCoverage coverage)
        {
            writer.Write((int)coverage.Stage);
            writer.Write((int)coverage.Scope);
            writer.Write((int)coverage.State);
            writer.Write(coverage.ExpectedSubjects);
            writer.Write(coverage.CompletedSubjects);
        }

        private static EvidenceCoverage ReadCoverage(BinaryReader reader) =>
            new EvidenceCoverage(
                EnumValue<EvidenceStage>(reader.ReadInt32()),
                EnumValue<EvidenceScope>(reader.ReadInt32()),
                EnumValue<CoverageState>(reader.ReadInt32()),
                Positive(reader.ReadInt32(), 4096, "coverage expected subjects"),
                Bounded(reader.ReadInt32(), 4096, "coverage completed subjects"));

        private static void WriteSubject(BinaryWriter writer, ConclusionSubject subject)
        {
            writer.Write((int)subject.Kind);
            writer.Write(subject.Identifier);
        }

        private static ConclusionSubject ReadSubject(BinaryReader reader) =>
            new ConclusionSubject(
                EnumValue<SubjectKind>(reader.ReadInt32()),
                Required(reader.ReadString(), "subject identifier"));

        private static void WriteOptionalString(BinaryWriter writer, string? value)
        {
            writer.Write(value != null);
            if (value != null)
                writer.Write(value);
        }

        private static string? ReadOptionalString(BinaryReader reader) =>
            reader.ReadBoolean() ? Required(reader.ReadString(), "optional string") : null;

        private void Touch(string path)
        {
            long ticks = Math.Max(DateTime.UtcNow.Ticks, checked(lastTouchTicks + 1));
            lastTouchTicks = ticks;
            File.SetLastWriteTimeUtc(path, new DateTime(ticks, DateTimeKind.Utc));
        }

        private void Trim()
        {
            FileInfo[] entries = new DirectoryInfo(DirectoryPath)
                .GetFiles("*" + EntryExtension, SearchOption.TopDirectoryOnly)
                .OrderByDescending(item => item.LastWriteTimeUtc)
                .ThenBy(item => item.Name, StringComparer.Ordinal)
                .ToArray();
            foreach (FileInfo entry in entries.Skip(maximumEntries))
                entry.Delete();
        }

        private static int Positive(int value, int maximum, string name)
        {
            if (value <= 0 || value > maximum)
                throw new InvalidDataException("Invalid " + name + ".");
            return value;
        }

        private static int Bounded(int value, int maximum, string name)
        {
            if (value < 0 || value > maximum)
                throw new InvalidDataException("Invalid " + name + ".");
            return value;
        }

        private static T EnumValue<T>(int value) where T : struct
        {
            if (!Enum.IsDefined(typeof(T), value))
                throw new InvalidDataException("Invalid " + typeof(T).Name + " value.");
            return (T)Enum.ToObject(typeof(T), value);
        }

        private static string Required(string value, string name)
        {
            if (String.IsNullOrWhiteSpace(value) || value.Length > 1_000_000)
                throw new InvalidDataException("Invalid " + name + ".");
            return value;
        }

        private static bool IsRecoverable(Exception exception) =>
            exception is IOException ||
            exception is UnauthorizedAccessException ||
            exception is InvalidDataException ||
            exception is ArgumentException ||
            exception is InvalidOperationException ||
            exception is NotSupportedException ||
            exception is EndOfStreamException ||
            exception is CryptographicException;

        private static byte[] Digest(byte[] value, int offset, int count)
        {
            using SHA256 sha = SHA256.Create();
            return sha.ComputeHash(value, offset, count);
        }

        private static bool BytesEqual(byte[] entry, int offset, byte[] expected)
        {
            if (entry.Length - offset != expected.Length)
                return false;
            int difference = 0;
            for (int index = 0; index < expected.Length; index++)
                difference |= entry[offset + index] ^ expected[index];
            return difference == 0;
        }

        private static void TryDelete(string path)
        {
            try
            {
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (IOException)
            {
            }
            catch (UnauthorizedAccessException)
            {
            }
        }
    }
}
