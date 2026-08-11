using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public enum RawResourceSemantics
    {
        FiniteDeposit,
        OilFlow
    }

    public sealed record RawPlanetCoverage
    {
        public RawPlanetCoverage(CoverageState state, int completedSubjects)
        {
            if (completedSubjects < 0 || completedSubjects > 1)
                throw new ArgumentOutOfRangeException(nameof(completedSubjects));
            if (state == CoverageState.Partial)
                throw new ArgumentException("Single-planet raw coverage cannot be partial.", nameof(state));
            if (state == CoverageState.Complete && completedSubjects != 1)
                throw new ArgumentException("Complete raw coverage requires the planet.");
            if (state != CoverageState.Complete && completedSubjects != 0)
                throw new ArgumentException("An incomplete single-planet operation has no completed planet.");

            State = state;
            CompletedSubjects = completedSubjects;
        }

        public CoverageState State { get; }
        public int ExpectedSubjects => 1;
        public int CompletedSubjects { get; }
        public bool IsComplete => State == CoverageState.Complete;

        public static RawPlanetCoverage Complete() =>
            new RawPlanetCoverage(CoverageState.Complete, 1);

        public static RawPlanetCoverage Unavailable() =>
            new RawPlanetCoverage(CoverageState.Unavailable, 0);
    }

    public sealed record NormalizedRawVeinNode
    {
        public NormalizedRawVeinNode(
            int sourceIndex,
            int nodeId,
            int resourceType,
            string resourceId,
            int productItemId,
            RawResourceSemantics semantics,
            long amount,
            int groupIndex,
            decimal positionX,
            decimal positionY,
            decimal positionZ,
            decimal? oilSpeedMultiplier)
        {
            if (sourceIndex <= 0)
                throw new ArgumentOutOfRangeException(nameof(sourceIndex));
            if (nodeId <= 0)
                throw new ArgumentOutOfRangeException(nameof(nodeId));
            if (resourceType <= 0)
                throw new ArgumentOutOfRangeException(nameof(resourceType));
            if (String.IsNullOrWhiteSpace(resourceId))
                throw new ArgumentException("Resource ID is required.", nameof(resourceId));
            if (productItemId <= 0)
                throw new ArgumentOutOfRangeException(nameof(productItemId));
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            if (groupIndex <= 0)
                throw new ArgumentOutOfRangeException(nameof(groupIndex));
            if (semantics == RawResourceSemantics.OilFlow && !oilSpeedMultiplier.HasValue)
                throw new ArgumentException("Oil nodes require their flow multiplier.");
            if (semantics != RawResourceSemantics.OilFlow && oilSpeedMultiplier.HasValue)
                throw new ArgumentException("Finite deposits cannot carry an oil multiplier.");

            SourceIndex = sourceIndex;
            NodeId = nodeId;
            ResourceType = resourceType;
            ResourceId = resourceId;
            ProductItemId = productItemId;
            Semantics = semantics;
            Amount = amount;
            GroupIndex = groupIndex;
            PositionX = positionX;
            PositionY = positionY;
            PositionZ = positionZ;
            OilSpeedMultiplier = oilSpeedMultiplier;
        }

        public int SourceIndex { get; }
        public int NodeId { get; }
        public int ResourceType { get; }
        public string ResourceId { get; }
        public int ProductItemId { get; }
        public RawResourceSemantics Semantics { get; }
        public long Amount { get; }
        public int GroupIndex { get; }
        public decimal PositionX { get; }
        public decimal PositionY { get; }
        public decimal PositionZ { get; }
        public decimal? OilSpeedMultiplier { get; }
        public string AmountUnit => Semantics == RawResourceSemantics.OilFlow
            ? "runtime-oil-amount-units"
            : "runtime-amount-units";
        public string PositionUnit => "dsp-planet-local-units";
    }

    public sealed record NormalizedRawVeinGroup
    {
        public NormalizedRawVeinGroup(
            int groupIndex,
            int resourceType,
            string resourceId,
            RawResourceSemantics semantics,
            int nodeCount,
            long amount,
            decimal positionX,
            decimal positionY,
            decimal positionZ)
        {
            if (groupIndex <= 0)
                throw new ArgumentOutOfRangeException(nameof(groupIndex));
            if (resourceType <= 0)
                throw new ArgumentOutOfRangeException(nameof(resourceType));
            if (String.IsNullOrWhiteSpace(resourceId))
                throw new ArgumentException("Resource ID is required.", nameof(resourceId));
            if (nodeCount <= 0)
                throw new ArgumentOutOfRangeException(nameof(nodeCount));
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            GroupIndex = groupIndex;
            ResourceType = resourceType;
            ResourceId = resourceId;
            Semantics = semantics;
            NodeCount = nodeCount;
            Amount = amount;
            PositionX = positionX;
            PositionY = positionY;
            PositionZ = positionZ;
        }

        public int GroupIndex { get; }
        public int ResourceType { get; }
        public string ResourceId { get; }
        public RawResourceSemantics Semantics { get; }
        public int NodeCount { get; }
        public long Amount { get; }
        public decimal PositionX { get; }
        public decimal PositionY { get; }
        public decimal PositionZ { get; }
        public string AmountUnit => Semantics == RawResourceSemantics.OilFlow
            ? "runtime-oil-amount-units"
            : "runtime-amount-units";
        public string PositionUnit => "dsp-planet-local-units";
    }

    public sealed class NormalizedRawPlanetEvidence
    {
        private readonly NormalizedRawVeinNode[] nodes;
        private readonly NormalizedRawVeinGroup[] groups;

        public NormalizedRawPlanetEvidence(
            int galaxySeed,
            int planetId,
            int themeId,
            int algorithmId,
            RawPlanetCoverage coverage,
            IEnumerable<NormalizedRawVeinNode> nodes,
            IEnumerable<NormalizedRawVeinGroup> groups)
        {
            if (galaxySeed < 0 || galaxySeed > 99_999_999)
                throw new ArgumentOutOfRangeException(nameof(galaxySeed));
            if (planetId <= 0)
                throw new ArgumentOutOfRangeException(nameof(planetId));
            if (themeId <= 0)
                throw new ArgumentOutOfRangeException(nameof(themeId));
            if (algorithmId <= 0)
                throw new ArgumentOutOfRangeException(nameof(algorithmId));
            Coverage = coverage ?? throw new ArgumentNullException(nameof(coverage));
            if (!coverage.IsComplete)
                throw new ArgumentException("Raw evidence requires complete planet coverage.", nameof(coverage));
            if (nodes == null)
                throw new ArgumentNullException(nameof(nodes));
            if (groups == null)
                throw new ArgumentNullException(nameof(groups));

            this.nodes = nodes.OrderBy(node => node.SourceIndex).ToArray();
            this.groups = groups.OrderBy(group => group.GroupIndex).ToArray();
            if (this.nodes.Select(node => node.SourceIndex).Distinct().Count() != this.nodes.Length)
                throw new ArgumentException("Raw node source indexes must be unique.", nameof(nodes));
            if (this.groups.Select(group => group.GroupIndex).Distinct().Count() != this.groups.Length)
                throw new ArgumentException("Raw group indexes must be unique.", nameof(groups));

            GalaxySeed = galaxySeed;
            PlanetId = planetId;
            ThemeId = themeId;
            AlgorithmId = algorithmId;
        }

        public int GalaxySeed { get; }
        public int PlanetId { get; }
        public int ThemeId { get; }
        public int AlgorithmId { get; }
        public RawPlanetCoverage Coverage { get; }
        public IReadOnlyList<NormalizedRawVeinNode> Nodes =>
            Array.AsReadOnly((NormalizedRawVeinNode[])nodes.Clone());
        public IReadOnlyList<NormalizedRawVeinGroup> Groups =>
            Array.AsReadOnly((NormalizedRawVeinGroup[])groups.Clone());
    }

    public sealed class RawPlanetRequest
    {
        public RawPlanetRequest(PreviewScanRequest identity, int planetId, int expectedAlgorithmId)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            if (planetId <= 0)
                throw new ArgumentOutOfRangeException(nameof(planetId));
            if (expectedAlgorithmId <= 0)
                throw new ArgumentOutOfRangeException(nameof(expectedAlgorithmId));
            PlanetId = planetId;
            ExpectedAlgorithmId = expectedAlgorithmId;
        }

        public PreviewScanRequest Identity { get; }
        public int PlanetId { get; }
        public int ExpectedAlgorithmId { get; }
    }

    public sealed class RawCompatibilityException : Exception
    {
        public RawCompatibilityException(string code, string message, string? rawDiagnostic = null)
            : base(message)
        {
            if (String.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Compatibility code is required.", nameof(code));
            Code = code;
            RawDiagnostic = rawDiagnostic;
        }

        public string Code { get; }
        public string? RawDiagnostic { get; }
    }

    public interface IRuntimeRawPlanetGateway
    {
        int MainThreadId { get; }
        RuntimeFingerprint CaptureFingerprint(PreviewScanRequest request);
        RuntimeStateLease CaptureState();
        NormalizedRawPlanetEvidence GenerateRawPlanet(
            RawPlanetRequest request,
            CancellationToken cancellationToken,
            Action<string> recordTrace);
    }
}
