using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using DSPSeedScanner.Core;
using DSPSeedScanner.Runtime;

namespace DSPSeedScanner.Runtime.Tests
{
    internal static class Program
    {
        private static int Main()
        {
            var tests = new (string Name, Action Body)[]
            {
                ("supported topology reaches evaluator", SupportedTopologyReachesEvaluator),
                ("identity mismatches reject safely", IdentityMismatchesRejectSafely),
                ("member and mod uncertainty reject safely", MemberAndModUncertaintyRejectSafely),
                ("unsupported request identity rejects safely", UnsupportedRequestIdentityRejectsSafely),
                ("unknown enum preserves raw diagnostic", UnknownEnumPreservesRawDiagnostic),
                ("thread affinity rejects before runtime access", ThreadAffinityRejectsBeforeRuntimeAccess),
                ("success failure and cancellation restore state", ExitPathsRestoreState),
                ("concurrent request receives busy rejection", ConcurrentRequestReceivesBusy),
                ("runtime boundary exposes no game objects", RuntimeBoundaryExposesNoGameObjects)
            };

            int failures = 0;
            foreach ((string name, Action body) in tests)
            {
                try
                {
                    body();
                    Console.WriteLine("PASS {0}", name);
                }
                catch (Exception exception)
                {
                    failures++;
                    Console.Error.WriteLine("FAIL {0}: {1}", name, exception);
                }
            }
            Console.WriteLine("RESULT {0}/{1} passed", tests.Length - failures, tests.Length);
            return failures == 0 ? 0 : 1;
        }

        private static void SupportedTopologyReachesEvaluator()
        {
            var gateway = new FakeGateway();
            RuntimeScanResult result = new PreviewScanCoordinator(gateway).TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Success, result.Status);
            Equal(ComponentOutcome.Supports, result.Conclusion?.Outcome);
            Equal("3", result.Conclusion?.DecisiveFact?.Value);
            Equal(16_315_224, result.GalaxySeed);
            True(result.StateRestored);
            True(result.Trace.Any(value => value.StartsWith("generate:thread=", StringComparison.Ordinal)));
            Equal(1, gateway.GenerateCalls);
        }

        private static void IdentityMismatchesRejectSafely()
        {
            AssertRejected(Fingerprint(assembly: "BAD"), "assembly-mismatch");
            AssertRejected(Fingerprint(themes: ConclusionDefinition.ReferenceOrderedThemeIds.Split(',').Reverse()), "theme-catalogue-mismatch");
            AssertRejected(Fingerprint(gameVersion: "0.10.34.0"), "game-version-mismatch");
            AssertRejected(Fingerprint(algorithm: 1), "galaxy-algorithm-mismatch");
        }

        private static void MemberAndModUncertaintyRejectSafely()
        {
            AssertRejected(Fingerprint(members: false, missing: "UniverseGen.CreateGalaxy"), "missing-runtime-member");
            AssertRejected(Fingerprint(mods: new[] { "example.generation.patch" }), "generation-mod-uncertain");
        }

        private static void UnsupportedRequestIdentityRejectsSafely()
        {
            var gateway = new FakeGateway();
            var request = new PreviewScanRequest(
                16_315_224,
                32,
                ConclusionDefinition.ReferenceGameVersion,
                1m,
                CombatMode.Combat,
                ConclusionDefinition.ReferenceCombatSettingsKey);
            RuntimeScanResult result = new PreviewScanCoordinator(gateway).TryScan(request, CancellationToken.None);
            Equal(RuntimeScanStatus.Incompatible, result.Status);
            Equal("request-identity-unsupported", result.Code);
            Equal(0, gateway.GenerateCalls);
            Equal(0, gateway.RestoreCalls);
        }

        private static void UnknownEnumPreservesRawDiagnostic()
        {
            var gateway = new FakeGateway
            {
                Snapshot = new RuntimeTopologySnapshot("1", 3, 64, "EPlanetType", 99)
            };
            RuntimeScanResult result = new PreviewScanCoordinator(gateway).TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Incompatible, result.Status);
            Equal("unknown-runtime-enum", result.Code);
            Equal("EPlanetType=99", result.RawDiagnostic);
            Equal(null, result.Conclusion);
            True(result.StateRestored);
        }

        private static void ThreadAffinityRejectsBeforeRuntimeAccess()
        {
            var gateway = new FakeGateway { MainThreadIdOverride = -1 };
            RuntimeScanResult result = new PreviewScanCoordinator(gateway).TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Incompatible, result.Status);
            Equal("thread-affinity-mismatch", result.Code);
            Equal(0, gateway.FingerprintCalls);
            Equal(0, gateway.GenerateCalls);
        }

        private static void ExitPathsRestoreState()
        {
            var success = new FakeGateway();
            RuntimeScanResult successResult = new PreviewScanCoordinator(success).TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Success, successResult.Status);
            Equal(1, success.RestoreCalls);
            Equal("original", success.StateMarker);

            var failure = new FakeGateway { GenerationFailure = new InvalidOperationException("injected") };
            RuntimeScanResult failureResult = new PreviewScanCoordinator(failure).TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Failed, failureResult.Status);
            Equal("runtime-exception", failureResult.Code);
            True(failureResult.Message.Contains("injected", StringComparison.Ordinal));
            Equal(1, failure.RestoreCalls);
            Equal("original", failure.StateMarker);

            var cancelled = new FakeGateway();
            using var source = new CancellationTokenSource();
            source.Cancel();
            RuntimeScanResult cancelledResult = new PreviewScanCoordinator(cancelled).TryScan(Request(), source.Token);
            Equal(RuntimeScanStatus.Cancelled, cancelledResult.Status);
            Equal(0, cancelled.GenerateCalls);
            Equal(1, cancelled.RestoreCalls);
            Equal("original", cancelled.StateMarker);
        }

        private static void ConcurrentRequestReceivesBusy()
        {
            var gateway = new FakeGateway();
            var coordinator = new PreviewScanCoordinator(gateway);
            RuntimeScanResult? nested = null;
            gateway.OnGenerate = () => nested = coordinator.TryScan(Request(), CancellationToken.None);
            RuntimeScanResult outer = coordinator.TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Success, outer.Status);
            Equal(RuntimeScanStatus.Busy, nested?.Status);
            Equal("busy", nested?.Code);
            Equal(1, gateway.GenerateCalls);
            Equal(1, gateway.RestoreCalls);
        }

        private static void RuntimeBoundaryExposesNoGameObjects()
        {
            Assembly assembly = typeof(PreviewScanCoordinator).Assembly;
            string[] forbidden = { "Assembly-CSharp", "UnityEngine", "BepInEx" };
            foreach (AssemblyName reference in assembly.GetReferencedAssemblies())
                False(forbidden.Any(value => reference.Name?.StartsWith(value, StringComparison.Ordinal) == true));

            foreach (Type type in new[] { typeof(RuntimeScanResult), typeof(RuntimeFingerprint), typeof(RuntimeTopologySnapshot) })
            {
                foreach (PropertyInfo property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                    False(forbidden.Any(value => property.PropertyType.Assembly.GetName().Name?.StartsWith(value, StringComparison.Ordinal) == true));
            }
        }

        private static void AssertRejected(RuntimeFingerprint fingerprint, string code)
        {
            var gateway = new FakeGateway { Fingerprint = fingerprint };
            RuntimeScanResult result = new PreviewScanCoordinator(gateway).TryScan(Request(), CancellationToken.None);
            Equal(RuntimeScanStatus.Incompatible, result.Status);
            Equal(code, result.Code);
            Equal(0, gateway.GenerateCalls);
            Equal(0, gateway.RestoreCalls);
        }

        private static PreviewScanRequest Request()
        {
            return new PreviewScanRequest(
                16_315_224,
                ConclusionDefinition.ReferenceStarCount,
                ConclusionDefinition.ReferenceGameVersion,
                1m,
                CombatMode.Combat,
                ConclusionDefinition.ReferenceCombatSettingsKey);
        }

        private static RuntimeFingerprint Fingerprint(
            string? gameVersion = null,
            int? algorithm = null,
            string? assembly = null,
            IEnumerable<string>? themes = null,
            bool members = true,
            string? missing = null,
            IEnumerable<string>? mods = null)
        {
            return new RuntimeFingerprint(
                gameVersion ?? ConclusionDefinition.ReferenceGameVersion,
                algorithm ?? ConclusionDefinition.ReferenceGalaxyAlgorithm,
                assembly ?? ConclusionDefinition.ReferenceAssemblySha256,
                themes ?? ConclusionDefinition.ReferenceOrderedThemeIds.Split(','),
                ConclusionDefinition.DefinitionVersion,
                ConclusionDefinition.ContractVersion,
                members,
                missing,
                mods);
        }

        private sealed class FakeGateway : IRuntimePreviewGateway
        {
            public RuntimeFingerprint Fingerprint { get; set; } = Program.Fingerprint();
            public RuntimeTopologySnapshot Snapshot { get; set; } = new RuntimeTopologySnapshot("1", 3, 64);
            public Exception? GenerationFailure { get; set; }
            public Action? OnGenerate { get; set; }
            public int? MainThreadIdOverride { get; set; }
            public int FingerprintCalls { get; private set; }
            public int GenerateCalls { get; private set; }
            public int RestoreCalls { get; private set; }
            public string StateMarker { get; set; } = "original";
            public int MainThreadId => MainThreadIdOverride ?? Thread.CurrentThread.ManagedThreadId;

            public RuntimeFingerprint CaptureFingerprint(PreviewScanRequest request)
            {
                FingerprintCalls++;
                return Fingerprint;
            }

            public RuntimeStateLease CaptureState()
            {
                string original = StateMarker;
                StateMarker = "leased";
                return new FakeLease(this, original);
            }

            public RuntimeTopologySnapshot GeneratePreview(
                PreviewScanRequest request,
                CancellationToken cancellationToken,
                Action<string> recordTrace)
            {
                GenerateCalls++;
                recordTrace("generate:thread=" + Thread.CurrentThread.ManagedThreadId);
                StateMarker = "generated";
                OnGenerate?.Invoke();
                if (GenerationFailure != null)
                    throw GenerationFailure;
                return Snapshot;
            }

            private sealed class FakeLease : RuntimeStateLease
            {
                private readonly FakeGateway owner;
                private readonly string original;
                private bool restored;

                public FakeLease(FakeGateway owner, string original)
                {
                    this.owner = owner;
                    this.original = original;
                }

                public override bool Restored => restored;

                public override void Dispose()
                {
                    owner.StateMarker = original;
                    owner.RestoreCalls++;
                    restored = true;
                }
            }
        }

        private static void Equal<T>(T expected, T actual)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
                throw new InvalidOperationException("Expected " + expected + ", got " + actual + ".");
        }

        private static void True(bool value)
        {
            if (!value) throw new InvalidOperationException("Expected true.");
        }

        private static void False(bool value)
        {
            if (value) throw new InvalidOperationException("Expected false.");
        }
    }
}
