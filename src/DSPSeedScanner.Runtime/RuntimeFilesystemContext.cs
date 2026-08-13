using System;
using System.IO;
using System.Security;

namespace DSPSeedScanner.Runtime
{
    internal sealed class RuntimeFilesystemInputs
    {
        public RuntimeFilesystemInputs(
            string? executablePath,
            string? bepInExGameRootPath,
            string? pluginAssemblyPath,
            string? targetAssemblyPath,
            string? patcherDirectoryPath,
            string? configurationDirectoryPath)
        {
            ExecutablePath = executablePath;
            BepInExGameRootPath = bepInExGameRootPath;
            PluginAssemblyPath = pluginAssemblyPath;
            TargetAssemblyPath = targetAssemblyPath;
            PatcherDirectoryPath = patcherDirectoryPath;
            ConfigurationDirectoryPath = configurationDirectoryPath;
        }

        public string? ExecutablePath { get; }
        public string? BepInExGameRootPath { get; }
        public string? PluginAssemblyPath { get; }
        public string? TargetAssemblyPath { get; }
        public string? PatcherDirectoryPath { get; }
        public string? ConfigurationDirectoryPath { get; }
    }

    internal sealed class RuntimeFilesystemContext
    {
        public RuntimeFilesystemContext(
            string gameRootPath,
            string bepInExRootPath,
            string managedAssemblyPath,
            string? patcherDirectoryPath,
            string? configurationDirectoryPath,
            string provenance,
            string? pluginDiagnostic,
            string? patcherDiagnostic,
            string? configurationDiagnostic)
        {
            GameRootPath = gameRootPath;
            BepInExRootPath = bepInExRootPath;
            ManagedAssemblyPath = managedAssemblyPath;
            PatcherDirectoryPath = patcherDirectoryPath;
            ConfigurationDirectoryPath = configurationDirectoryPath;
            CacheDirectoryPath = configurationDirectoryPath == null
                ? null
                : Path.Combine(configurationDirectoryPath, "DSPSeedScanner", "cache");
            Provenance = provenance;
            PluginDiagnostic = pluginDiagnostic;
            PatcherDiagnostic = patcherDiagnostic;
            ConfigurationDiagnostic = configurationDiagnostic;
        }

        public string GameRootPath { get; }
        public string BepInExRootPath { get; }
        public string ManagedAssemblyPath { get; }
        public string? PatcherDirectoryPath { get; }
        public string? ConfigurationDirectoryPath { get; }
        public string? CacheDirectoryPath { get; }
        public string Provenance { get; }
        public string? PluginDiagnostic { get; }
        public string? PatcherDiagnostic { get; }
        public string? ConfigurationDiagnostic { get; }
    }

    internal sealed class RuntimeFilesystemResolution
    {
        private RuntimeFilesystemResolution(
            RuntimeFilesystemContext? context,
            string code,
            string message,
            string diagnostic)
        {
            Context = context;
            Code = code;
            Message = message;
            Diagnostic = diagnostic;
        }

        public bool Succeeded => Context != null;
        public RuntimeFilesystemContext? Context { get; }
        public string Code { get; }
        public string Message { get; }
        public string Diagnostic { get; }

        public static RuntimeFilesystemResolution Success(RuntimeFilesystemContext context) =>
            new RuntimeFilesystemResolution(
                context ?? throw new ArgumentNullException(nameof(context)),
                "resolved",
                "The active runtime filesystem context was resolved.",
                "runtime-context:resolved:" + context.Provenance);

        public static RuntimeFilesystemResolution Failure(
            string code,
            string message,
            string diagnostic) =>
            new RuntimeFilesystemResolution(
                null,
                Required(code, nameof(code)),
                Required(message, nameof(message)),
                Required(diagnostic, nameof(diagnostic)));

        public RuntimeFilesystemException ToException() =>
            new RuntimeFilesystemException(Code, "runtime-context", Message, Diagnostic);

        private static string Required(string value, string name)
        {
            if (String.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Value is required.", name);
            return value;
        }
    }

    internal static class RuntimeFilesystemContextResolver
    {
        public static RuntimeFilesystemResolution Resolve(RuntimeFilesystemInputs inputs)
        {
            if (inputs == null)
                throw new ArgumentNullException(nameof(inputs));

            try
            {
                return ResolveCore(inputs);
            }
            catch (Exception exception) when (RuntimeFilesystemDiagnostics.IsExpectedFailure(exception))
            {
                return RuntimeFilesystemResolution.Failure(
                    "runtime-filesystem-resolution-failed",
                    "The active DSP filesystem context could not be resolved.",
                    RuntimeFilesystemDiagnostics.Format(
                        "resolve-context",
                        "active-process",
                        exception));
            }
        }

        private static RuntimeFilesystemResolution ResolveCore(RuntimeFilesystemInputs inputs)
        {

            string? executable = Normalize(inputs.ExecutablePath);
            string? gameRoot;
            string provenance;
            if (executable != null)
            {
                gameRoot = Path.GetDirectoryName(executable);
                provenance = "process-executable";
            }
            else
            {
                gameRoot = Normalize(inputs.BepInExGameRootPath);
                provenance = "bepinex-game-root";
            }

            if (String.IsNullOrWhiteSpace(gameRoot))
            {
                return Fail(
                    "active-game-root-unavailable",
                    "The active DSP installation could not be identified.",
                    "select-root",
                    provenance,
                    "No usable active-process root was reported.");
            }
            gameRoot = Normalize(gameRoot);
            if (gameRoot == null)
            {
                return Fail(
                    "active-game-root-invalid",
                    "The active DSP installation path was invalid.",
                    "normalize-root",
                    provenance,
                    "The selected root could not be normalized.");
            }

            string? reportedGameRoot = Normalize(inputs.BepInExGameRootPath);
            if (!String.IsNullOrWhiteSpace(inputs.BepInExGameRootPath) &&
                reportedGameRoot == null)
            {
                return Fail(
                    "bepinex-game-root-invalid",
                    "BepInEx reported an invalid active game root.",
                    "validate-root",
                    "bepinex-game-root",
                    "The reported root could not be normalized.");
            }
            if (reportedGameRoot != null && !SamePath(gameRoot, reportedGameRoot))
            {
                return Fail(
                    "active-game-root-conflict",
                    "The active DSP installation paths disagreed.",
                    "validate-root",
                    "bepinex-game-root",
                    "The reported root did not match the process executable.");
            }

            string managedDirectory = Path.Combine(gameRoot, "DSPGAME_Data", "Managed");
            string bepInExRoot = Path.Combine(gameRoot, "BepInEx");
            if (!Directory.Exists(managedDirectory) || !Directory.Exists(bepInExRoot))
            {
                return Fail(
                    "active-game-structure-missing",
                    "The active DSP installation structure was incomplete.",
                    "validate-structure",
                    provenance,
                    "The managed or BepInEx directory was unavailable.");
            }

            string managedAssembly = Path.Combine(managedDirectory, "Assembly-CSharp.dll");
            string? targetAssembly = Normalize(inputs.TargetAssemblyPath);
            if (!String.IsNullOrWhiteSpace(inputs.TargetAssemblyPath) && targetAssembly == null)
            {
                return Fail(
                    "target-assembly-path-invalid",
                    "The loaded game assembly path was invalid.",
                    "validate-assembly",
                    "loaded-target",
                    "The reported path could not be normalized.");
            }
            if (targetAssembly != null && !SamePath(managedAssembly, targetAssembly))
            {
                return Fail(
                    "target-assembly-path-conflict",
                    "The loaded game assembly was outside the active installation.",
                    "validate-assembly",
                    "loaded-target",
                    "The reported path did not match the active managed assembly.");
            }
            if (!File.Exists(managedAssembly))
            {
                return Fail(
                    "managed-assembly-missing",
                    "The active game assembly was unavailable.",
                    "validate-assembly",
                    "active-managed",
                    "Assembly-CSharp.dll was not present.");
            }

            string? pluginAssembly = Normalize(inputs.PluginAssemblyPath);
            string? pluginDiagnostic = null;
            if (pluginAssembly != null)
            {
                string pluginRoot = Path.Combine(bepInExRoot, "plugins");
                if (!IsWithin(pluginRoot, pluginAssembly))
                {
                    pluginDiagnostic = RuntimeFilesystemDiagnostics.Format(
                        "validate-plugin",
                        "loaded-plugin",
                        "ExternalPath: The plugin was loaded outside the active BepInEx plugin tree.");
                }
            }
            else if (!String.IsNullOrWhiteSpace(inputs.PluginAssemblyPath))
            {
                pluginDiagnostic = RuntimeFilesystemDiagnostics.Format(
                    "validate-plugin",
                    "loaded-plugin",
                    "InvalidPath: The plugin location could not be normalized.");
            }

            string canonicalPatcher = Path.Combine(bepInExRoot, "patchers");
            string? patcher = SelectOptionalDirectory(
                inputs.PatcherDirectoryPath,
                canonicalPatcher,
                bepInExRoot,
                "patcher-path",
                requireExisting: true,
                out string? patcherDiagnostic);
            string canonicalConfig = Path.Combine(bepInExRoot, "config");
            string? config = SelectOptionalDirectory(
                inputs.ConfigurationDirectoryPath,
                canonicalConfig,
                bepInExRoot,
                "config-path",
                requireExisting: false,
                out string? configDiagnostic);

            return RuntimeFilesystemResolution.Success(
                new RuntimeFilesystemContext(
                    gameRoot,
                    bepInExRoot,
                    managedAssembly,
                    patcher,
                    config,
                    provenance,
                    pluginDiagnostic,
                    patcherDiagnostic,
                    configDiagnostic));
        }

        private static string? SelectOptionalDirectory(
            string? reportedPath,
            string canonicalPath,
            string requiredRoot,
            string source,
            bool requireExisting,
            out string? diagnostic)
        {
            diagnostic = null;
            string? selected = String.IsNullOrWhiteSpace(reportedPath)
                ? Normalize(canonicalPath)
                : Normalize(reportedPath);
            if (selected == null)
            {
                diagnostic = RuntimeFilesystemDiagnostics.Format(
                    "select-directory",
                    source,
                    "InvalidPath: The reported directory could not be normalized.");
                return null;
            }
            if (!IsWithin(requiredRoot, selected))
            {
                diagnostic = RuntimeFilesystemDiagnostics.Format(
                    "select-directory",
                    source,
                    "ExternalPath: The reported directory was outside the active BepInEx root.");
                return null;
            }
            if (requireExisting && !Directory.Exists(selected))
            {
                diagnostic = RuntimeFilesystemDiagnostics.Format(
                    "select-directory",
                    source,
                    "DirectoryNotFound: The directory was unavailable.");
                return null;
            }
            return selected;
        }

        private static RuntimeFilesystemResolution Fail(
            string code,
            string message,
            string operation,
            string source,
            string detail) =>
            RuntimeFilesystemResolution.Failure(
                code,
                message,
                RuntimeFilesystemDiagnostics.Format(operation, source, detail));

        private static string? Normalize(string? path)
        {
            if (String.IsNullOrWhiteSpace(path))
                return null;
            try
            {
                return Path.GetFullPath(path)
                    .TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
            catch (Exception exception) when (RuntimeFilesystemDiagnostics.IsExpectedFailure(exception))
            {
                return null;
            }
        }

        private static bool SamePath(string first, string second) =>
            String.Equals(Normalize(first), Normalize(second), StringComparison.OrdinalIgnoreCase);

        private static bool IsWithin(string root, string path)
        {
            string? normalizedRoot = Normalize(root);
            string? normalizedPath = Normalize(path);
            if (normalizedRoot == null || normalizedPath == null)
                return false;
            if (SamePath(normalizedRoot, normalizedPath))
                return true;
            string prefix = normalizedRoot + Path.DirectorySeparatorChar;
            return normalizedPath.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
        }
    }

    internal sealed class RuntimeFilesystemException : Exception
    {
        public RuntimeFilesystemException(
            string code,
            string source,
            string message,
            string diagnostic)
            : base(message)
        {
            Code = code;
            FilesystemSource = source;
            Diagnostic = diagnostic;
        }

        public string Code { get; }
        public string FilesystemSource { get; }
        public string Diagnostic { get; }
    }

    internal static class RuntimeFilesystemDiagnostics
    {
        private const int MaximumDetailLength = 180;

        public static bool IsExpectedFailure(Exception exception)
        {
            if (exception == null)
                return false;
            return exception is ArgumentException ||
                exception is IOException ||
                exception is UnauthorizedAccessException ||
                exception is NotSupportedException ||
                exception is SecurityException;
        }

        public static string Format(string operation, string source, Exception exception) =>
            Format(operation, source, exception.GetType().Name + ": " + exception.Message);

        public static string Format(string operation, string source, string detail)
        {
            string safeOperation = Token(operation, "filesystem-operation");
            string safeSource = Token(source, "filesystem-source");
            string safeDetail = String.IsNullOrWhiteSpace(detail)
                ? "Failure details were unavailable."
                : detail.Replace('\r', ' ').Replace('\n', ' ').Replace('\t', ' ').Trim();
            if (safeDetail.Length > MaximumDetailLength)
                safeDetail = safeDetail.Substring(0, MaximumDetailLength) + "...";
            return safeOperation + ":" + safeSource + ":" + safeDetail;
        }

        private static string Token(string value, string fallback)
        {
            if (String.IsNullOrWhiteSpace(value))
                return fallback;
            char[] result = value.Trim().ToCharArray();
            for (int index = 0; index < result.Length; index++)
            {
                if (!Char.IsLetterOrDigit(result[index]) && result[index] != '-' && result[index] != '_')
                    result[index] = '-';
            }
            return new string(result);
        }
    }

    internal static class RuntimeFilesystemGuard
    {
        public static T ExecuteOrFallback<T>(
            Func<T> operation,
            T fallback,
            string operationName,
            string source,
            Action<string>? reportDiagnostic)
        {
            if (operation == null)
                throw new ArgumentNullException(nameof(operation));
            try
            {
                return operation();
            }
            catch (Exception exception) when (RuntimeFilesystemDiagnostics.IsExpectedFailure(exception))
            {
                reportDiagnostic?.Invoke(RuntimeFilesystemDiagnostics.Format(
                    operationName,
                    source,
                    exception));
                return fallback;
            }
        }
    }
}
