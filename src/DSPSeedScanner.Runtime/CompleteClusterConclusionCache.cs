using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security;
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
            IEnumerable<ConclusionReport> reports,
            HomeSystemResourceStatistics homeSystemResources)
        {
            CacheKeyHash = cacheKeyHash;
            Identity = identity;
            Coverage = coverage;
            Reports = Array.AsReadOnly(reports.ToArray());
            HomeSystemResources = homeSystemResources ??
                throw new ArgumentNullException(nameof(homeSystemResources));
        }

        public string CacheKeyHash { get; }
        public PreviewGenerationIdentity Identity { get; }
        public CompleteClusterRawCoverage Coverage { get; }
        public IReadOnlyList<ConclusionReport> Reports { get; }
        public HomeSystemResourceStatistics HomeSystemResources { get; }
    }

    public sealed class CompleteClusterConclusionCache
    {
        internal const int SchemaVersion = 10;
        internal const string EntryExtension = ".dspseedscan";
        private const string Magic = "DSPSeedScanner.CompleteClusterCache";
        private const int MaximumEntryBytes = 256 * 1024;
        private const int MaximumReports = 1024;

        private readonly object sync = new object();
        private readonly int maximumEntries;
        private readonly Action<string>? reportDiagnostic;
        private readonly Action<string>? beforeFileOperation;
        private readonly bool available;
        private long lastTouchTicks;

        public CompleteClusterConclusionCache(
            string directoryPath,
            int maximumEntries = 256,
            Action<string>? reportDiagnostic = null)
            : this(directoryPath, maximumEntries, reportDiagnostic, null)
        {
        }

        internal CompleteClusterConclusionCache(
            string directoryPath,
            int maximumEntries,
            Action<string>? reportDiagnostic,
            Action<string>? beforeFileOperation)
        {
            if (String.IsNullOrWhiteSpace(directoryPath))
                throw new ArgumentException("A cache directory is required.", nameof(directoryPath));
            if (maximumEntries <= 0 || maximumEntries > 4096)
                throw new ArgumentOutOfRangeException(nameof(maximumEntries));
            DirectoryPath = Path.GetFullPath(directoryPath);
            this.maximumEntries = maximumEntries;
            this.reportDiagnostic = reportDiagnostic;
            this.beforeFileOperation = beforeFileOperation;
            available = true;
        }

        private CompleteClusterConclusionCache(
            int maximumEntries,
            Action<string>? reportDiagnostic,
            string diagnostic)
        {
            DirectoryPath = String.Empty;
            this.maximumEntries = maximumEntries;
            this.reportDiagnostic = reportDiagnostic;
            beforeFileOperation = null;
            available = false;
            Report(diagnostic);
        }

        public static CompleteClusterConclusionCache CreateOrDisabled(
            string? directoryPath,
            Action<string>? reportDiagnostic = null,
            int maximumEntries = 256)
        {
            if (maximumEntries <= 0 || maximumEntries > 4096)
                throw new ArgumentOutOfRangeException(nameof(maximumEntries));
            if (String.IsNullOrWhiteSpace(directoryPath))
            {
                return new CompleteClusterConclusionCache(
                    maximumEntries,
                    reportDiagnostic,
                    RuntimeFilesystemDiagnostics.Format(
                        "initialize-cache",
                        "active-config",
                        "Unavailable: No safe cache directory was selected."));
            }
            try
            {
                return new CompleteClusterConclusionCache(
                    directoryPath,
                    maximumEntries,
                    reportDiagnostic);
            }
            catch (Exception exception) when (RuntimeFilesystemDiagnostics.IsExpectedFailure(exception))
            {
                return new CompleteClusterConclusionCache(
                    maximumEntries,
                    reportDiagnostic,
                    RuntimeFilesystemDiagnostics.Format(
                        "initialize-cache",
                        "active-config",
                        exception));
            }
        }

        public string DirectoryPath { get; }
        public int MaximumEntries => maximumEntries;
        public bool Available => available;

        public bool TryRead(
            PreviewGenerationIdentity identity,
            RuntimeFingerprint fingerprint,
            out CachedCompleteClusterConclusions? result)
        {
            result = null;
            if (!available)
                return false;
            if (!CompleteClusterCacheKey.TryCreate(identity, fingerprint, out CompleteClusterCacheKey? key) ||
                key == null)
            {
                return false;
            }

            lock (sync)
            {
                string? path = null;
                try
                {
                    path = Path.Combine(DirectoryPath, key.FileName);
                    if (!File.Exists(path))
                        return false;
                    BeforeFileOperation("read");
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
                    if (path != null && IsInvalidEntry(exception))
                        TryDelete(path);
                    Report("read-cache", exception);
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
            if (!available)
                return false;
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
                string? destination = null;
                bool installed = false;
                try
                {
                    ConclusionReport[] cachedReports = result.Reports
                        .Where(IsAuditedReusableReport)
                        .ToArray();
                    BeforeFileOperation("write");
                    Directory.CreateDirectory(DirectoryPath);
                    destination = Path.Combine(DirectoryPath, key.FileName);
                    temporary = Path.Combine(
                        DirectoryPath,
                        "." + key.Hash + "." + Guid.NewGuid().ToString("N") + ".tmp");
                    byte[] payload;
                    using (var buffer = new MemoryStream())
                    using (var writer = new BinaryWriter(buffer, Encoding.UTF8, leaveOpen: true))
                    {
                        Write(
                            writer,
                            key,
                            result.Coverage,
                            cachedReports,
                            result.HomeSystemResources!);
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

                    BeforeFileOperation("replace");
                    if (File.Exists(destination))
                        File.Replace(temporary, destination, null, ignoreMetadataErrors: true);
                    else
                        File.Move(temporary, destination);
                    temporary = null;
                    installed = true;
                    Touch(destination);
                    Trim();
                    return true;
                }
                catch (Exception exception) when (IsRecoverable(exception))
                {
                    if (installed && destination != null)
                        TryDelete(destination);
                    Report("write-cache", exception);
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
            if (!available)
                return false;
            lock (sync)
            {
                try
                {
                    BeforeFileOperation("clear");
                    if (!Directory.Exists(DirectoryPath))
                        return true;
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
                    Report("clear-cache", exception);
                    return false;
                }
            }
        }

        private static bool IsCacheable(
            CompleteClusterCacheKey key,
            CompleteClusterRawResult result)
        {
            ConclusionReport[] cachedReports = result.Reports
                .Where(IsAuditedReusableReport)
                .ToArray();
            if (result.Status != RuntimeScanStatus.Success ||
                result.Fingerprint == null ||
                !result.StateRestored ||
                !result.Coverage.IsComplete ||
                result.Coverage.ExpectedPlanets <= 0 ||
                result.GalaxySeed != key.Identity.GalaxyIdentity.GalaxySeed ||
                result.AffectedPlanetId.HasValue ||
                result.RawDiagnostic != null ||
                result.HomeSystemResources == null ||
                cachedReports.Length == 0 ||
                cachedReports.Length > MaximumReports)
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

            EvaluationSettings settings = Settings(key.Identity);
            bool hasCompleteClusterReport = false;
            foreach (ConclusionReport report in cachedReports)
            {
                if (!IsCurrentReport(key.Identity, settings, report))
                    return false;
                hasCompleteClusterReport |= report.Coverage.IsComplete;
            }
            return hasCompleteClusterReport;
        }

        private static bool IsCacheable(
            CompleteClusterCacheKey key,
            CachedCompleteClusterConclusions result)
        {
            if (!String.Equals(result.CacheKeyHash, key.Hash, StringComparison.Ordinal) ||
                !CanReuseAcrossMode(key.Identity, result.Identity) ||
                !result.Coverage.IsComplete ||
                result.Coverage.ExpectedPlanets <= 0 ||
                result.Reports.Count == 0 ||
                result.Reports.Count > MaximumReports ||
                result.HomeSystemResources.Bodies.Count >
                    HomeSystemResourceStatistics.MaximumBodies)
            {
                return false;
            }

            EvaluationSettings settings = Settings(result.Identity);
            return result.Reports.All(report =>
                IsAuditedReusableReport(report) &&
                report.Coverage.IsComplete &&
                IsCurrentReport(result.Identity, settings, report));
        }

        private static bool IsAuditedReusableReport(ConclusionReport report)
        {
            string id = report.ConclusionId;
            if (report.Stage == EvidenceStage.BirthSystemRaw &&
                report.Context == ConclusionContext.FreshStart)
            {
                return String.Equals(id, "FS-RESOURCES.common-total", StringComparison.Ordinal) ||
                    String.Equals(id, "FS-RESOURCES.fire-ice", StringComparison.Ordinal) ||
                    id.StartsWith("FS-RESOURCES.amount:", StringComparison.Ordinal) ||
                    id.StartsWith("FS-RESOURCES.groups:", StringComparison.Ordinal);
            }
            if (report.Stage != EvidenceStage.CompleteClusterRaw)
                return false;
            return String.Equals(id, "MF-RESOURCE-SCOPE.strength", StringComparison.Ordinal) ||
                String.Equals(id, "CX-GROUPING.distance", StringComparison.Ordinal) ||
                id.StartsWith("RR-ACCESS.distance:", StringComparison.Ordinal) ||
                id.StartsWith("RR-ACCESS.amount:", StringComparison.Ordinal) ||
                id.StartsWith("MF-SYSTEM-ROLE.role:", StringComparison.Ordinal);
        }

        private static EvaluationSettings Settings(PreviewGenerationIdentity identity) =>
            new EvaluationSettings(
                identity.ResourceMultiplier,
                identity.CombatMode,
                identity.CombatSettingsKey);

        private static bool CanReuseAcrossMode(
            PreviewGenerationIdentity active,
            PreviewGenerationIdentity source) =>
            active.GalaxyIdentity == source.GalaxyIdentity &&
            active.ResourceMultiplier == source.ResourceMultiplier &&
            String.Equals(
                active.CombatSettingsKey,
                source.CombatSettingsKey,
                StringComparison.Ordinal) &&
            active.InitialColonize == source.InitialColonize &&
            active.MaxDensity == source.MaxDensity;

        private static bool IsCurrentReport(
            PreviewGenerationIdentity identity,
            EvaluationSettings settings,
            ConclusionReport report) =>
            report.Identity == identity.GalaxyIdentity &&
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
            IReadOnlyList<ConclusionReport> reports,
            HomeSystemResourceStatistics homeSystemResources)
        {
            writer.Write(Magic);
            writer.Write(SchemaVersion);
            writer.Write(key.CanonicalValue);
            writer.Write((int)key.Identity.CombatMode);
            writer.Write(coverage.ExpectedPlanets);
            writer.Write(reports.Count);
            foreach (ConclusionReport report in reports)
                WriteReport(writer, report);
            writer.Write(homeSystemResources.Bodies.Count);
            foreach (HomeSystemBodyResources body in homeSystemResources.Bodies)
            {
                writer.Write(body.BodyId);
                writer.Write(body.Resources.Count);
                foreach (HomeSystemResource resource in body.Resources)
                {
                    writer.Write(resource.ResourceId);
                    writer.Write((int)resource.Semantics);
                    writer.Write(resource.Amount);
                    writer.Write(resource.VeinGroups);
                }
            }
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

            CombatMode sourceMode = EnumValue<CombatMode>(reader.ReadInt32());
            var sourceIdentity = new PreviewGenerationIdentity(
                key.Identity.GalaxyIdentity,
                key.Identity.ResourceMultiplier,
                sourceMode,
                key.Identity.CombatSettingsKey,
                key.Identity.InitialColonize,
                key.Identity.MaxDensity);
            int expectedPlanets = Positive(reader.ReadInt32(), 4096, "planet count");
            int reportCount = Positive(reader.ReadInt32(), MaximumReports, "report count");
            var reports = new List<ConclusionReport>(reportCount);
            for (int index = 0; index < reportCount; index++)
                reports.Add(ReadReport(reader, sourceIdentity));
            int bodyCount = Bounded(
                reader.ReadInt32(),
                HomeSystemResourceStatistics.MaximumBodies,
                "home-system resource body count");
            var bodies = new List<HomeSystemBodyResources>(bodyCount);
            for (int index = 0; index < bodyCount; index++)
            {
                int bodyId = Positive(reader.ReadInt32(), Int32.MaxValue, "body ID");
                int oreCount = Bounded(reader.ReadInt32(), 32, "body ore count");
                var resources = new List<HomeSystemResource>(oreCount);
                for (int oreIndex = 0; oreIndex < oreCount; oreIndex++)
                {
                    string resourceId = Required(reader.ReadString(), "body ore");
                    int semanticsValue = reader.ReadInt32();
                    if (!Enum.IsDefined(typeof(RawResourceSemantics), semanticsValue))
                        throw new InvalidDataException("The body resource semantics are invalid.");
                    long amount = reader.ReadInt64();
                    if (amount < 0)
                        throw new InvalidDataException("The body resource amount is invalid.");
                    int veinGroups = Positive(
                        reader.ReadInt32(),
                        Int32.MaxValue,
                        "body resource vein-group count");
                    resources.Add(new HomeSystemResource(
                        resourceId,
                        (RawResourceSemantics)semanticsValue,
                        amount,
                        veinGroups));
                }
                bodies.Add(new HomeSystemBodyResources(bodyId, resources));
            }

            return new CachedCompleteClusterConclusions(
                key.Hash,
                sourceIdentity,
                new CompleteClusterRawCoverage(
                    CoverageState.Complete,
                    expectedPlanets,
                    expectedPlanets),
                reports,
                new HomeSystemResourceStatistics(bodies));
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
            BeforeFileOperation("touch");
            File.SetLastWriteTimeUtc(path, new DateTime(ticks, DateTimeKind.Utc));
        }

        private void Trim()
        {
            BeforeFileOperation("trim");
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
            exception is CryptographicException ||
            exception is SecurityException;

        private static bool IsInvalidEntry(Exception exception) =>
            exception is InvalidDataException ||
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

        private void TryDelete(string path)
        {
            try
            {
                BeforeFileOperation("delete");
                if (File.Exists(path))
                    File.Delete(path);
            }
            catch (Exception exception) when (RuntimeFilesystemDiagnostics.IsExpectedFailure(exception))
            {
                Report("delete-cache", exception);
            }
        }

        private void Report(string operation, Exception exception) =>
            Report(RuntimeFilesystemDiagnostics.Format(
                operation,
                "active-config",
                exception));

        private void Report(string diagnostic) => reportDiagnostic?.Invoke(diagnostic);

        private void BeforeFileOperation(string operation) =>
            beforeFileOperation?.Invoke(operation);
    }
}
