using System;
using System.Collections.Generic;
using System.Linq;

namespace DSPSeedScanner.Core
{
    public enum CombatMode
    {
        Peace,
        Combat
    }

    public enum CompatibilityState
    {
        Supported,
        Unsupported
    }

    public sealed record EvaluationSettings
    {
        public EvaluationSettings(
            decimal resourceMultiplier,
            CombatMode combatMode,
            string combatSettingsKey)
        {
            if (resourceMultiplier <= 0)
                throw new ArgumentOutOfRangeException(nameof(resourceMultiplier));
            if (String.IsNullOrWhiteSpace(combatSettingsKey))
                throw new ArgumentException(
                    "Combat settings key is required.",
                    nameof(combatSettingsKey));

            ResourceMultiplier = resourceMultiplier;
            CombatMode = combatMode;
            CombatSettingsKey = combatSettingsKey;
        }

        public decimal ResourceMultiplier { get; }

        public CombatMode CombatMode { get; }

        public string CombatSettingsKey { get; }
    }

    public sealed record NormalizedGasProduct
    {
        public NormalizedGasProduct(string productId, decimal? collectionRate)
        {
            if (String.IsNullOrWhiteSpace(productId))
                throw new ArgumentException("Product ID is required.", nameof(productId));
            if (collectionRate < 0)
                throw new ArgumentOutOfRangeException(nameof(collectionRate));

            ProductId = productId;
            CollectionRate = collectionRate;
        }

        public string ProductId { get; }

        public decimal? CollectionRate { get; }
    }

    public sealed record NormalizedSystemEvidence
    {
        public NormalizedSystemEvidence(
            ConclusionSubject subject,
            bool isBirthSystem,
            int? sharedBirthGiantBodies = null,
            bool? hasTidalLockedSolidPlanet = null,
            decimal? maximumSolarRatio = null,
            decimal? maximumWindRatio = null,
            IEnumerable<NormalizedGasProduct>? giantProducts = null,
            decimal? dysonLuminosity = null,
            long? maximumShellRadius = null,
            int? containedOrbitCount = null,
            int? initialHiveCount = null)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            if (subject.Kind != SubjectKind.BirthSystem &&
                subject.Kind != SubjectKind.StarSystem)
            {
                throw new ArgumentException(
                    "System evidence requires a system subject.",
                    nameof(subject));
            }
            if (isBirthSystem && subject.Kind != SubjectKind.BirthSystem)
                throw new ArgumentException(
                    "The birth system requires a birth-system subject.",
                    nameof(subject));
            if (!isBirthSystem && subject.Kind != SubjectKind.StarSystem)
                throw new ArgumentException(
                    "A non-birth system requires a star-system subject.",
                    nameof(subject));
            if (sharedBirthGiantBodies < 0)
                throw new ArgumentOutOfRangeException(nameof(sharedBirthGiantBodies));
            if (maximumSolarRatio < 0)
                throw new ArgumentOutOfRangeException(nameof(maximumSolarRatio));
            if (maximumWindRatio < 0)
                throw new ArgumentOutOfRangeException(nameof(maximumWindRatio));
            if (dysonLuminosity <= 0)
                throw new ArgumentOutOfRangeException(nameof(dysonLuminosity));
            if (maximumShellRadius < 0)
                throw new ArgumentOutOfRangeException(nameof(maximumShellRadius));
            if (containedOrbitCount < 0)
                throw new ArgumentOutOfRangeException(nameof(containedOrbitCount));
            if (initialHiveCount < 0)
                throw new ArgumentOutOfRangeException(nameof(initialHiveCount));

            NormalizedGasProduct[] products = (giantProducts ??
                Array.Empty<NormalizedGasProduct>())
                .OrderBy(product => product.ProductId, StringComparer.Ordinal)
                .ToArray();
            if (products.Select(product => product.ProductId)
                .Distinct(StringComparer.Ordinal).Count() != products.Length)
            {
                throw new ArgumentException(
                    "Gas product IDs must be unique.",
                    nameof(giantProducts));
            }

            IsBirthSystem = isBirthSystem;
            SharedBirthGiantBodies = sharedBirthGiantBodies;
            HasTidalLockedSolidPlanet = hasTidalLockedSolidPlanet;
            MaximumSolarRatio = maximumSolarRatio;
            MaximumWindRatio = maximumWindRatio;
            GiantProducts = Array.AsReadOnly(products);
            DysonLuminosity = dysonLuminosity;
            MaximumShellRadius = maximumShellRadius;
            ContainedOrbitCount = containedOrbitCount;
            InitialHiveCount = initialHiveCount;
        }

        public ConclusionSubject Subject { get; }

        public bool IsBirthSystem { get; }

        public int? SharedBirthGiantBodies { get; }

        public bool? HasTidalLockedSolidPlanet { get; }

        public decimal? MaximumSolarRatio { get; }

        public decimal? MaximumWindRatio { get; }

        public IReadOnlyList<NormalizedGasProduct> GiantProducts { get; }

        public decimal? DysonLuminosity { get; }

        public long? MaximumShellRadius { get; }

        public int? ContainedOrbitCount { get; }

        public int? InitialHiveCount { get; }
    }

    public sealed record StarterResourceMetric
    {
        public StarterResourceMetric(string resourceId, long amount, int veinGroups)
        {
            if (String.IsNullOrWhiteSpace(resourceId))
                throw new ArgumentException("Resource ID is required.", nameof(resourceId));
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            if (veinGroups < 0)
                throw new ArgumentOutOfRangeException(nameof(veinGroups));

            ResourceId = resourceId;
            Amount = amount;
            VeinGroups = veinGroups;
        }

        public string ResourceId { get; }

        public long Amount { get; }

        public int VeinGroups { get; }
    }

    public sealed record NormalizedStarterResourceEvidence
    {
        public NormalizedStarterResourceEvidence(
            ConclusionSubject subject,
            IEnumerable<StarterResourceMetric> resources,
            bool containsFireIce)
        {
            Subject = subject ?? throw new ArgumentNullException(nameof(subject));
            if (subject.Kind != SubjectKind.BirthSystem)
                throw new ArgumentException(
                    "Starter resources require a birth-system subject.",
                    nameof(subject));
            if (resources == null)
                throw new ArgumentNullException(nameof(resources));

            StarterResourceMetric[] metrics = resources
                .OrderBy(resource => resource.ResourceId, StringComparer.Ordinal)
                .ToArray();
            if (metrics.Select(resource => resource.ResourceId)
                .Distinct(StringComparer.Ordinal).Count() != metrics.Length)
            {
                throw new ArgumentException(
                    "Starter resource IDs must be unique.",
                    nameof(resources));
            }

            Resources = Array.AsReadOnly(metrics);
            ContainsFireIce = containsFireIce;
        }

        public ConclusionSubject Subject { get; }

        public IReadOnlyList<StarterResourceMetric> Resources { get; }

        public bool ContainsFireIce { get; }
    }

    public sealed record NormalizedRareResourceEvidence
    {
        public NormalizedRareResourceEvidence(
            string resourceId,
            bool isPresent,
            ConclusionSubject? nearestSystem,
            decimal? distanceFromBirthLy,
            long? amount = null,
            int? veinGroups = null)
        {
            if (String.IsNullOrWhiteSpace(resourceId))
                throw new ArgumentException("Resource ID is required.", nameof(resourceId));
            if (distanceFromBirthLy < 0)
                throw new ArgumentOutOfRangeException(nameof(distanceFromBirthLy));
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));
            if (veinGroups < 0)
                throw new ArgumentOutOfRangeException(nameof(veinGroups));
            if (isPresent && (nearestSystem == null || !distanceFromBirthLy.HasValue))
            {
                throw new ArgumentException(
                    "Present rare resources require their nearest system and distance.",
                    nameof(nearestSystem));
            }
            if (!isPresent &&
                (nearestSystem != null || distanceFromBirthLy.HasValue || amount.HasValue ||
                 veinGroups.HasValue))
            {
                throw new ArgumentException(
                    "Absent rare resources cannot carry deposit evidence.",
                    nameof(isPresent));
            }
            if (nearestSystem != null && nearestSystem.Kind != SubjectKind.StarSystem &&
                nearestSystem.Kind != SubjectKind.BirthSystem)
            {
                throw new ArgumentException(
                    "Rare-resource access requires a system subject.",
                    nameof(nearestSystem));
            }

            ResourceId = resourceId;
            IsPresent = isPresent;
            NearestSystem = nearestSystem;
            DistanceFromBirthLy = distanceFromBirthLy;
            Amount = amount;
            VeinGroups = veinGroups;
        }

        public string ResourceId { get; }

        public bool IsPresent { get; }

        public ConclusionSubject? NearestSystem { get; }

        public decimal? DistanceFromBirthLy { get; }

        public long? Amount { get; }

        public int? VeinGroups { get; }
    }

    public sealed record NormalizedSystemDistance
    {
        public NormalizedSystemDistance(
            string firstSystemIdentifier,
            string secondSystemIdentifier,
            decimal lightYears)
        {
            if (String.IsNullOrWhiteSpace(firstSystemIdentifier))
                throw new ArgumentException(
                    "First system is required.",
                    nameof(firstSystemIdentifier));
            if (String.IsNullOrWhiteSpace(secondSystemIdentifier))
                throw new ArgumentException(
                    "Second system is required.",
                    nameof(secondSystemIdentifier));
            if (String.Equals(
                firstSystemIdentifier,
                secondSystemIdentifier,
                StringComparison.Ordinal))
                throw new ArgumentException("A distance requires two different systems.");
            if (lightYears < 0)
                throw new ArgumentOutOfRangeException(nameof(lightYears));

            if (StringComparer.Ordinal.Compare(firstSystemIdentifier, secondSystemIdentifier) < 0)
            {
                FirstSystemIdentifier = firstSystemIdentifier;
                SecondSystemIdentifier = secondSystemIdentifier;
            }
            else
            {
                FirstSystemIdentifier = secondSystemIdentifier;
                SecondSystemIdentifier = firstSystemIdentifier;
            }
            LightYears = lightYears;
        }

        public string FirstSystemIdentifier { get; }

        public string SecondSystemIdentifier { get; }

        public decimal LightYears { get; }

        public bool Connects(string first, string second)
        {
            return (String.Equals(FirstSystemIdentifier, first, StringComparison.Ordinal) &&
                    String.Equals(SecondSystemIdentifier, second, StringComparison.Ordinal)) ||
                   (String.Equals(FirstSystemIdentifier, second, StringComparison.Ordinal) &&
                    String.Equals(SecondSystemIdentifier, first, StringComparison.Ordinal));
        }
    }

    public sealed class NormalizedClusterEvidence
    {
        public NormalizedClusterEvidence(
            GenerationIdentity identity,
            EvaluationSettings settings,
            ConclusionSubject clusterSubject,
            string birthSystemIdentifier,
            IEnumerable<EvidenceCoverage> coverages,
            IEnumerable<NormalizedSystemEvidence> systems,
            NormalizedStarterResourceEvidence? starterResources = null,
            IEnumerable<NormalizedRareResourceEvidence>? rareResources = null,
            IEnumerable<NormalizedSystemDistance>? systemDistances = null,
            long? clusterCommonResourceTotal = null,
            CompatibilityState compatibility = CompatibilityState.Supported,
            DiagnosticCause? compatibilityFailure = null)
        {
            Identity = identity ?? throw new ArgumentNullException(nameof(identity));
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            ClusterSubject = clusterSubject ??
                throw new ArgumentNullException(nameof(clusterSubject));
            if (clusterSubject.Kind != SubjectKind.Cluster)
                throw new ArgumentException(
                    "Cluster evidence requires a cluster subject.",
                    nameof(clusterSubject));
            if (String.IsNullOrWhiteSpace(birthSystemIdentifier))
                throw new ArgumentException(
                    "Birth-system identifier is required.",
                    nameof(birthSystemIdentifier));
            if (coverages == null)
                throw new ArgumentNullException(nameof(coverages));
            if (systems == null)
                throw new ArgumentNullException(nameof(systems));
            if (clusterCommonResourceTotal < 0)
                throw new ArgumentOutOfRangeException(nameof(clusterCommonResourceTotal));
            if (compatibility == CompatibilityState.Unsupported && compatibilityFailure == null)
                throw new ArgumentException(
                    "Unsupported evidence requires a compatibility diagnostic.",
                    nameof(compatibilityFailure));
            if (compatibility == CompatibilityState.Supported && compatibilityFailure != null)
                throw new ArgumentException(
                    "Supported evidence cannot carry a compatibility failure.",
                    nameof(compatibilityFailure));

            EvidenceCoverage[] coverageArray = coverages
                .OrderBy(coverage => coverage.Scope)
                .ToArray();
            if (coverageArray.Select(coverage => coverage.Scope).Distinct().Count() !=
                coverageArray.Length)
            {
                throw new ArgumentException(
                    "Evidence scopes must be unique.",
                    nameof(coverages));
            }

            NormalizedSystemEvidence[] systemArray = systems
                .OrderBy(system => system.Subject.Identifier, StringComparer.Ordinal)
                .ToArray();
            if (systemArray.Select(system => system.Subject.Identifier)
                .Distinct(StringComparer.Ordinal).Count() != systemArray.Length)
            {
                throw new ArgumentException(
                    "System identifiers must be unique.",
                    nameof(systems));
            }
            NormalizedSystemEvidence[] birthSystems = systemArray
                .Where(system => system.IsBirthSystem)
                .ToArray();
            if (birthSystems.Length > 1)
                throw new ArgumentException(
                    "Only one system can be the birth system.",
                    nameof(systems));
            if (birthSystems.Length == 1 &&
                !String.Equals(
                    birthSystems[0].Subject.Identifier,
                    birthSystemIdentifier,
                    StringComparison.Ordinal))
            {
                throw new ArgumentException(
                    "The birth-system identifier does not match the birth-system evidence.",
                    nameof(birthSystemIdentifier));
            }

            NormalizedRareResourceEvidence[] rareArray = (rareResources ??
                Array.Empty<NormalizedRareResourceEvidence>())
                .OrderBy(resource => resource.ResourceId, StringComparer.Ordinal)
                .ToArray();
            if (rareArray.Select(resource => resource.ResourceId)
                .Distinct(StringComparer.Ordinal).Count() != rareArray.Length)
            {
                throw new ArgumentException(
                    "Rare resource IDs must be unique.",
                    nameof(rareResources));
            }

            NormalizedSystemDistance[] distanceArray = (systemDistances ??
                Array.Empty<NormalizedSystemDistance>())
                .OrderBy(distance => distance.FirstSystemIdentifier, StringComparer.Ordinal)
                .ThenBy(distance => distance.SecondSystemIdentifier, StringComparer.Ordinal)
                .ToArray();
            if (distanceArray
                .Select(distance => distance.FirstSystemIdentifier + "\n" +
                    distance.SecondSystemIdentifier)
                .Distinct(StringComparer.Ordinal).Count() != distanceArray.Length)
            {
                throw new ArgumentException(
                    "System-distance pairs must be unique.",
                    nameof(systemDistances));
            }

            BirthSystemIdentifier = birthSystemIdentifier;
            Coverages = Array.AsReadOnly(coverageArray);
            Systems = Array.AsReadOnly(systemArray);
            StarterResources = starterResources;
            RareResources = Array.AsReadOnly(rareArray);
            SystemDistances = Array.AsReadOnly(distanceArray);
            ClusterCommonResourceTotal = clusterCommonResourceTotal;
            Compatibility = compatibility;
            CompatibilityFailure = compatibilityFailure;
        }

        public GenerationIdentity Identity { get; }

        public EvaluationSettings Settings { get; }

        public ConclusionSubject ClusterSubject { get; }

        public string BirthSystemIdentifier { get; }

        public IReadOnlyList<EvidenceCoverage> Coverages { get; }

        public IReadOnlyList<NormalizedSystemEvidence> Systems { get; }

        public NormalizedStarterResourceEvidence? StarterResources { get; }

        public IReadOnlyList<NormalizedRareResourceEvidence> RareResources { get; }

        public IReadOnlyList<NormalizedSystemDistance> SystemDistances { get; }

        public long? ClusterCommonResourceTotal { get; }

        public CompatibilityState Compatibility { get; }

        public DiagnosticCause? CompatibilityFailure { get; }

        public EvidenceCoverage Coverage(EvidenceScope scope, EvidenceStage stage)
        {
            EvidenceCoverage? coverage = Coverages.SingleOrDefault(item => item.Scope == scope);
            if (coverage != null)
            {
                if (coverage.Stage != stage)
                {
                    throw new InvalidOperationException(
                        "Coverage stage does not match the normalized evidence scope.");
                }
                return coverage;
            }

            return new EvidenceCoverage(
                stage,
                scope,
                CoverageState.Unavailable,
                expectedSubjects: 1,
                completedSubjects: 0);
        }
    }
}
