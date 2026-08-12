using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DSPSeedScanner.Core
{
    public static class ConclusionEngine
    {
        public static IReadOnlyList<ConclusionReport> Evaluate(
            NormalizedClusterEvidence evidence)
        {
            if (evidence == null)
                throw new ArgumentNullException(nameof(evidence));

            var reports = new List<ConclusionReport>();
            EvaluateTopology(evidence, reports);
            EvaluatePower(evidence, reports);
            EvaluateGasProducts(evidence, reports);
            EvaluateStarterResources(evidence, reports);
            EvaluateEnergy(evidence, reports);
            EvaluateSphereGeometry(evidence, reports);
            EvaluateDeferredResourceScope(evidence, reports);
            EvaluateRareAccess(evidence, reports);

            IReadOnlyList<RoleAssignment> roles = EvaluateRoles(evidence, reports);
            EvaluateGrouping(evidence, roles, reports);
            EvaluateTraits(reports);

            ConclusionReport[] ordered = reports
                .OrderBy(report => report.ConclusionId, StringComparer.Ordinal)
                .ThenBy(report => report.Subject.Identifier, StringComparer.Ordinal)
                .ThenBy(report => report.Outcome)
                .ToArray();
            return Array.AsReadOnly(ordered);
        }

        private static void EvaluateTopology(
            NormalizedClusterEvidence evidence,
            ICollection<ConclusionReport> reports)
        {
            EvidenceCoverage coverage = evidence.Coverage(
                EvidenceScope.BirthSystemTopology,
                EvidenceStage.GalaxyPreview);
            NormalizedSystemEvidence? birth = BirthSystem(evidence);
            DiagnosticCause? unavailable = UnavailableCause(evidence, coverage);
            if (unavailable != null || birth?.SharedBirthGiantBodies == null)
            {
                reports.Add(Report(
                    evidence,
                    coverage,
                    SharedSatelliteEvaluator.ConclusionId,
                    ConclusionContext.FreshStart,
                    birth?.Subject ?? BirthSubject(evidence),
                    ComponentOutcome.Unknown,
                    null,
                    unavailable ?? MissingFact("sharedBirthGiantBodies")));
                return;
            }

            reports.Add(SharedSatelliteEvaluator.Evaluate(
                new NormalizedBirthTopologyEvidence(
                    evidence.Identity,
                    evidence.Settings,
                    coverage,
                    birth.Subject,
                    birth.SharedBirthGiantBodies)));
        }

        private static void EvaluatePower(
            NormalizedClusterEvidence evidence,
            ICollection<ConclusionReport> reports)
        {
            NormalizedSystemEvidence? birth = BirthSystem(evidence);
            ConclusionSubject subject = birth?.Subject ?? BirthSubject(evidence);
            EvidenceCoverage rotation = evidence.Coverage(
                EvidenceScope.BirthSystemRotation,
                EvidenceStage.GalaxyPreview);
            DiagnosticCause? unavailable = UnavailableCause(evidence, rotation);
            if (unavailable != null || birth?.HasTidalLockedSolidPlanet == null)
            {
                reports.Add(Report(
                    evidence,
                    rotation,
                    "FS-POWER.birth-tidal",
                    ConclusionContext.FreshStart,
                    subject,
                    ComponentOutcome.Unknown,
                    null,
                    unavailable ?? MissingFact("hasTidalLockedSolidPlanet")));
            }
            else
            {
                bool present = birth.HasTidalLockedSolidPlanet.Value;
                reports.Add(Report(
                    evidence,
                    rotation,
                    "FS-POWER.birth-tidal",
                    ConclusionContext.FreshStart,
                    subject,
                    present ? ComponentOutcome.Supports : ComponentOutcome.DoesNotSupport,
                    Fact("hasTidalLockedSolidPlanet", present ? "present" : "absent", "state"),
                    null));
            }

            EvidenceCoverage power = evidence.Coverage(
                EvidenceScope.BirthSystemPower,
                EvidenceStage.GalaxyPreview);
            AddRangeReport(
                evidence,
                reports,
                power,
                "FS-POWER.solar",
                ConclusionContext.FreshStart,
                subject,
                "maximumSolarRatio",
                birth?.MaximumSolarRatio,
                ConclusionDefinition.Solar,
                ConclusionDefinition.IsReferencePreviewIdentity(evidence.Identity));
            AddRangeReport(
                evidence,
                reports,
                power,
                "FS-POWER.wind",
                ConclusionContext.FreshStart,
                subject,
                "maximumWindRatio",
                birth?.MaximumWindRatio,
                ConclusionDefinition.Wind,
                ConclusionDefinition.IsReferencePreviewIdentity(evidence.Identity));
        }

        private static void EvaluateGasProducts(
            NormalizedClusterEvidence evidence,
            ICollection<ConclusionReport> reports)
        {
            EvidenceCoverage coverage = evidence.Coverage(
                EvidenceScope.BirthSystemGasProducts,
                EvidenceStage.GalaxyPreview);
            NormalizedSystemEvidence? birth = BirthSystem(evidence);
            ConclusionSubject subject = birth?.Subject ?? BirthSubject(evidence);
            DiagnosticCause? unavailable = UnavailableCause(evidence, coverage);

            foreach (string productId in ConclusionDefinition.GasProductIds)
            {
                NormalizedGasProduct? product = birth?.GiantProducts.SingleOrDefault(
                    item => String.Equals(item.ProductId, productId, StringComparison.Ordinal));
                string productConclusionId = "FS-GAS-ROUTE.product:" + productId;
                if (unavailable != null || birth == null)
                {
                    reports.Add(Report(
                        evidence,
                        coverage,
                        productConclusionId,
                        ConclusionContext.FreshStart,
                        subject,
                        ComponentOutcome.Unknown,
                        null,
                        unavailable ?? MissingFact("giantProducts")));
                }
                else
                {
                    bool present = product != null;
                    reports.Add(Report(
                        evidence,
                        coverage,
                        productConclusionId,
                        ConclusionContext.FreshStart,
                        subject,
                        present ? ComponentOutcome.Supports : ComponentOutcome.DoesNotSupport,
                        Fact("giantProduct", present ? productId : "absent:" + productId,
                            "product-id"),
                        null));
                }

                reports.Add(Report(
                    evidence,
                    coverage,
                    "FS-GAS-ROUTE.rate:" + productId,
                    ConclusionContext.FreshStart,
                    subject,
                    ComponentOutcome.Unknown,
                    product?.CollectionRate.HasValue == true
                        ? DecimalFact("collectionRate", product.CollectionRate.Value,
                            "runtime-rate")
                        : null,
                    unavailable ?? NoAcceptedRange("FS-GAS-ROUTE.rate")));
            }
        }

        private static void EvaluateStarterResources(
            NormalizedClusterEvidence evidence,
            ICollection<ConclusionReport> reports)
        {
            EvidenceCoverage coverage = evidence.Coverage(
                EvidenceScope.BirthSystemResources,
                EvidenceStage.BirthSystemRaw);
            NormalizedStarterResourceEvidence? starter = evidence.StarterResources;
            ConclusionSubject subject = starter?.Subject ?? BirthSubject(evidence);
            DiagnosticCause? unavailable = UnavailableCause(evidence, coverage);
            bool acceptedSettings =
                ConclusionDefinition.IsReferencePreviewIdentity(evidence.Identity) &&
                evidence.Settings.ResourceMultiplier == 1m;

            StarterResourceMetric[] metrics = ConclusionDefinition.StarterTotalResourceIds
                .Select(resourceId => starter?.Resources.SingleOrDefault(resource =>
                    String.Equals(resource.ResourceId, resourceId, StringComparison.Ordinal)))
                .Where(resource => resource != null)
                .Cast<StarterResourceMetric>()
                .ToArray();
            decimal? total = metrics.Length == ConclusionDefinition.StarterTotalResourceIds.Count
                ? metrics.Sum(resource => (decimal)resource.Amount)
                : null;

            AddRangeReport(
                evidence,
                reports,
                coverage,
                "FS-RESOURCES.common-total",
                ConclusionContext.FreshStart,
                subject,
                "commonResourceTotal",
                total,
                ConclusionDefinition.StarterCommonTotal,
                acceptedSettings);

            foreach (string resourceId in ConclusionDefinition.CommonResourceIds)
            {
                StarterResourceMetric? metric = starter?.Resources.SingleOrDefault(resource =>
                    String.Equals(resource.ResourceId, resourceId, StringComparison.Ordinal));
                ConclusionSubject resourceSubject = ResourceSubject(evidence, resourceId);
                AddRangeReport(
                    evidence,
                    reports,
                    coverage,
                    "FS-RESOURCES.amount:" + resourceId,
                    ConclusionContext.FreshStart,
                    resourceSubject,
                    resourceId + "Amount",
                    metric?.Amount,
                    ConclusionDefinition.StarterAmount(resourceId),
                    acceptedSettings);
                AddRangeReport(
                    evidence,
                    reports,
                    coverage,
                    "FS-RESOURCES.groups:" + resourceId,
                    ConclusionContext.FreshStart,
                    resourceSubject,
                    resourceId + "VeinGroups",
                    metric?.VeinGroups,
                    ConclusionDefinition.StarterGroups(resourceId),
                    acceptedSettings);
            }

            if (unavailable != null || starter == null)
            {
                reports.Add(Report(
                    evidence,
                    coverage,
                    "FS-RESOURCES.fire-ice",
                    ConclusionContext.FreshStart,
                    subject,
                    ComponentOutcome.Unknown,
                    null,
                    unavailable ?? MissingFact("containsFireIce")));
            }
            else
            {
                reports.Add(Report(
                    evidence,
                    coverage,
                    "FS-RESOURCES.fire-ice",
                    ConclusionContext.FreshStart,
                    subject,
                    starter.ContainsFireIce
                        ? ComponentOutcome.Supports
                        : ComponentOutcome.DoesNotSupport,
                    Fact("containsFireIce", starter.ContainsFireIce ? "present" : "absent",
                        "state"),
                    null));
            }
        }

        private static void EvaluateEnergy(
            NormalizedClusterEvidence evidence,
            ICollection<ConclusionReport> reports)
        {
            EvidenceCoverage coverage = evidence.Coverage(
                EvidenceScope.ClusterEnergy,
                EvidenceStage.GalaxyPreview);
            DiagnosticCause? unavailable = UnavailableCause(evidence, coverage);
            NormalizedSystemEvidence[] ranked = evidence.Systems
                .Where(system => system.DysonLuminosity.HasValue)
                .OrderByDescending(system => system.DysonLuminosity)
                .ThenBy(system => system.Subject.Identifier, StringComparer.Ordinal)
                .ToArray();
            bool allValuesPresent = evidence.Systems.Count > 0 &&
                ranked.Length == evidence.Systems.Count;
            ConclusionSubject subject = ranked.FirstOrDefault()?.Subject ?? evidence.ClusterSubject;
            decimal? maximum = allValuesPresent ? ranked[0].DysonLuminosity : null;
            bool acceptedIdentity =
                ConclusionDefinition.IsReferencePreviewIdentity(evidence.Identity);

            AddRangeReport(
                evidence,
                reports,
                coverage,
                "MF-ENERGY-SYSTEM.output",
                ConclusionContext.Megafactory,
                subject,
                "maximumDysonLuminosity",
                maximum,
                ConclusionDefinition.EnergyOutput,
                acceptedIdentity);

            decimal? separation = null;
            if (allValuesPresent && ranked.Length >= 2)
            {
                separation = ranked[0].DysonLuminosity!.Value /
                    ranked[1].DysonLuminosity!.Value;
            }
            AddRangeReport(
                evidence,
                reports,
                coverage,
                "MF-ENERGY-SYSTEM.separation",
                ConclusionContext.Megafactory,
                subject,
                "leaderLuminosityRatio",
                separation,
                ConclusionDefinition.EnergySeparation,
                acceptedIdentity);
        }

        private static void EvaluateSphereGeometry(
            NormalizedClusterEvidence evidence,
            ICollection<ConclusionReport> reports)
        {
            EvidenceCoverage coverage = evidence.Coverage(
                EvidenceScope.ClusterSphereGeometry,
                EvidenceStage.GalaxyPreview);
            bool acceptedIdentity =
                ConclusionDefinition.IsReferencePreviewIdentity(evidence.Identity);
            NormalizedSystemEvidence[] radiusSystems = evidence.Systems
                .Where(system => system.MaximumShellRadius.HasValue)
                .OrderByDescending(system => system.MaximumShellRadius)
                .ThenBy(system => system.Subject.Identifier, StringComparer.Ordinal)
                .ToArray();
            decimal? radius = evidence.Systems.Count > 0 &&
                radiusSystems.Length == evidence.Systems.Count
                ? radiusSystems[0].MaximumShellRadius
                : null;
            AddRangeReport(
                evidence,
                reports,
                coverage,
                "MF-SPHERE-GEOMETRY.radius",
                ConclusionContext.SphereShowcase,
                radiusSystems.FirstOrDefault()?.Subject ?? evidence.ClusterSubject,
                "maximumShellRadius",
                radius,
                ConclusionDefinition.SphereRadius,
                acceptedIdentity);

            if (evidence.Systems.Count == 0)
            {
                AddRangeReport(
                    evidence,
                    reports,
                    coverage,
                    "MF-SPHERE-GEOMETRY.containment",
                    ConclusionContext.SphereShowcase,
                    evidence.ClusterSubject,
                    "containedOrbitCount",
                    null,
                    ConclusionDefinition.OrbitContainment,
                    acceptedIdentity);
                return;
            }

            foreach (NormalizedSystemEvidence system in evidence.Systems)
            {
                AddRangeReport(
                    evidence,
                    reports,
                    coverage,
                    "MF-SPHERE-GEOMETRY.containment",
                    ConclusionContext.SphereShowcase,
                    system.Subject,
                    "containedOrbitCount",
                    system.ContainedOrbitCount,
                    ConclusionDefinition.OrbitContainment,
                    acceptedIdentity);
            }
        }

        private static void EvaluateDeferredResourceScope(
            NormalizedClusterEvidence evidence,
            ICollection<ConclusionReport> reports)
        {
            EvidenceCoverage coverage = evidence.Coverage(
                EvidenceScope.CompleteClusterResources,
                EvidenceStage.CompleteClusterRaw);
            DiagnosticCause? unavailable = UnavailableCause(evidence, coverage);
            reports.Add(Report(
                evidence,
                coverage,
                "MF-RESOURCE-SCOPE.strength",
                ConclusionContext.Megafactory,
                evidence.ClusterSubject,
                ComponentOutcome.Unknown,
                evidence.ClusterCommonResourceTotal.HasValue
                    ? DecimalFact(
                        "clusterCommonResourceTotal",
                        evidence.ClusterCommonResourceTotal.Value,
                        "runtime-amount-units")
                    : null,
                unavailable ?? NoAcceptedRange("MF-RESOURCE-SCOPE")));
        }

        private static void EvaluateRareAccess(
            NormalizedClusterEvidence evidence,
            ICollection<ConclusionReport> reports)
        {
            EvidenceCoverage coverage = evidence.Coverage(
                EvidenceScope.CompleteClusterRareResources,
                EvidenceStage.CompleteClusterRaw);
            DiagnosticCause? unavailable = UnavailableCause(evidence, coverage);
            bool acceptedSettings =
                ConclusionDefinition.IsReferencePreviewIdentity(evidence.Identity) &&
                evidence.Settings.ResourceMultiplier == 1m;

            if (evidence.RareResources.Count == 0)
            {
                reports.Add(Report(
                    evidence,
                    coverage,
                    "RR-ACCESS.distance",
                    ConclusionContext.Megafactory,
                    evidence.ClusterSubject,
                    ComponentOutcome.Unknown,
                    null,
                    unavailable ?? MissingFact("rareResources")));
                reports.Add(Report(
                    evidence,
                    coverage,
                    "RR-ACCESS.amount",
                    ConclusionContext.Megafactory,
                    evidence.ClusterSubject,
                    ComponentOutcome.Unknown,
                    null,
                    unavailable ?? NoAcceptedRange("RR-ACCESS.amount")));
                return;
            }

            foreach (NormalizedRareResourceEvidence resource in evidence.RareResources)
            {
                ConclusionSubject subject = resource.IsPresent &&
                    resource.NearestSystem != null
                    ? resource.NearestSystem
                    : ResourceSubject(evidence, resource.ResourceId);
                string distanceId = "RR-ACCESS.distance:" + resource.ResourceId;
                if (unavailable != null)
                {
                    reports.Add(Report(
                        evidence,
                        coverage,
                        distanceId,
                        ConclusionContext.Megafactory,
                        subject,
                        ComponentOutcome.Unknown,
                        null,
                        unavailable));
                }
                else if (!resource.IsPresent)
                {
                    reports.Add(Report(
                        evidence,
                        coverage,
                        distanceId,
                        ConclusionContext.Megafactory,
                        subject,
                        ComponentOutcome.DoesNotSupport,
                        Fact("rareResourcePresence", "absent", "state"),
                        null));
                }
                else
                {
                    AddRangeReport(
                        evidence,
                        reports,
                        coverage,
                        distanceId,
                        ConclusionContext.Megafactory,
                        subject,
                        "distanceFromBirth",
                        resource.DistanceFromBirthLy,
                        ConclusionDefinition.RareAccessDistance,
                        acceptedSettings);
                }

                reports.Add(Report(
                    evidence,
                    coverage,
                    "RR-ACCESS.amount:" + resource.ResourceId,
                    ConclusionContext.Megafactory,
                    subject,
                    ComponentOutcome.Unknown,
                    resource.Amount.HasValue
                        ? IntegerFact("rareResourceAmount", resource.Amount.Value,
                            "runtime-amount-units")
                        : null,
                    unavailable ?? NoAcceptedRange("RR-ACCESS.amount")));
            }
        }

        private static IReadOnlyList<RoleAssignment> EvaluateRoles(
            NormalizedClusterEvidence evidence,
            ICollection<ConclusionReport> reports)
        {
            var assignments = new List<RoleAssignment>();
            EvidenceCoverage topology = evidence.Coverage(
                EvidenceScope.BirthSystemTopology,
                EvidenceStage.GalaxyPreview);
            NormalizedSystemEvidence? birth = BirthSystem(evidence);
            DiagnosticCause? unavailable = UnavailableCause(evidence, topology);
            if (unavailable == null && birth != null)
            {
                AddRole(
                    evidence,
                    reports,
                    assignments,
                    "starter-anchor",
                    birth.Subject,
                    topology,
                    "FS-TOPOLOGY.shared-satellites");
            }

            foreach (ConclusionReport source in reports
                .Where(report => report.Outcome == ComponentOutcome.Supports)
                .ToArray())
            {
                if (source.ConclusionId == "MF-ENERGY-SYSTEM.output")
                {
                    AddRole(evidence, reports, assignments, "strong-energy", source.Subject,
                        source.Coverage, source.ConclusionId);
                }
                else if (source.ConclusionId == "MF-SPHERE-GEOMETRY.radius")
                {
                    AddRole(evidence, reports, assignments, "large-shell", source.Subject,
                        source.Coverage, source.ConclusionId);
                }
                else if (source.ConclusionId == "MF-SPHERE-GEOMETRY.containment")
                {
                    AddRole(evidence, reports, assignments, "orbit-containment", source.Subject,
                        source.Coverage, source.ConclusionId);
                }
                else if (source.ConclusionId.StartsWith(
                    "RR-ACCESS.distance:",
                    StringComparison.Ordinal))
                {
                    NormalizedRareResourceEvidence? rare = evidence.RareResources.SingleOrDefault(
                        item => source.ConclusionId.EndsWith(
                            ":" + item.ResourceId,
                            StringComparison.Ordinal));
                    if (rare?.NearestSystem != null)
                    {
                        AddRole(evidence, reports, assignments, "rare-access",
                            rare.NearestSystem, source.Coverage, source.ConclusionId);
                    }
                }
            }

            return assignments;
        }

        private static void EvaluateGrouping(
            NormalizedClusterEvidence evidence,
            IReadOnlyList<RoleAssignment> roles,
            ICollection<ConclusionReport> reports)
        {
            EvidenceCoverage coverage = evidence.Coverage(
                EvidenceScope.SystemDistances,
                EvidenceStage.GalaxyPreview);
            if (evidence.Compatibility == CompatibilityState.Unsupported)
            {
                reports.Add(Report(
                    evidence,
                    coverage,
                    "CX-GROUPING.distance",
                    ConclusionContext.CompactExpansion,
                    evidence.ClusterSubject,
                    ComponentOutcome.Unknown,
                    null,
                    evidence.CompatibilityFailure));
                return;
            }
            RoleAssignment[] nonStarter = roles
                .Where(role => role.RoleId != "starter-anchor")
                .ToArray();
            if (nonStarter.Length == 0)
            {
                reports.Add(Report(
                    evidence,
                    coverage,
                    "CX-GROUPING.distance",
                    ConclusionContext.CompactExpansion,
                    evidence.ClusterSubject,
                    ComponentOutcome.Unknown,
                    null,
                    new DiagnosticCause(
                        "unsupported-role",
                        "Grouping requires at least two independently supported system roles.")));
                return;
            }

            var seen = new HashSet<string>(StringComparer.Ordinal);
            for (int firstIndex = 0; firstIndex < roles.Count; firstIndex++)
            {
                for (int secondIndex = firstIndex + 1; secondIndex < roles.Count; secondIndex++)
                {
                    RoleAssignment first = roles[firstIndex];
                    RoleAssignment second = roles[secondIndex];
                    if (String.Equals(
                        first.Subject.Identifier,
                        second.Subject.Identifier,
                        StringComparison.Ordinal))
                    {
                        continue;
                    }
                    string key = StringComparer.Ordinal.Compare(
                        first.RoleId + first.Subject.Identifier,
                        second.RoleId + second.Subject.Identifier) < 0
                        ? first.RoleId + "|" + first.Subject.Identifier + "|" +
                          second.RoleId + "|" + second.Subject.Identifier
                        : second.RoleId + "|" + second.Subject.Identifier + "|" +
                          first.RoleId + "|" + first.Subject.Identifier;
                    if (!seen.Add(key))
                        continue;

                    NormalizedSystemDistance? distance = evidence.SystemDistances.SingleOrDefault(
                        item => item.Connects(
                            first.Subject.Identifier,
                            second.Subject.Identifier));
                    string pairIdentifier = StringComparer.Ordinal.Compare(
                        first.Subject.Identifier,
                        second.Subject.Identifier) < 0
                        ? first.Subject.Identifier + "<->" + second.Subject.Identifier
                        : second.Subject.Identifier + "<->" + first.Subject.Identifier;
                    var pairSubject = new ConclusionSubject(
                        SubjectKind.SystemPair,
                        pairIdentifier + ":" + first.RoleId + "+" + second.RoleId);
                    EvidenceStage pairStage = first.Report.Stage > second.Report.Stage
                        ? first.Report.Stage
                        : second.Report.Stage;
                    var pairCoverage = new EvidenceCoverage(
                        pairStage,
                        coverage.Scope,
                        coverage.State,
                        coverage.ExpectedSubjects,
                        coverage.CompletedSubjects);
                    AddRangeReport(
                        evidence,
                        reports,
                        pairCoverage,
                        "CX-GROUPING.distance",
                        ConclusionContext.CompactExpansion,
                        pairSubject,
                        "systemDistance",
                        distance?.LightYears,
                        ConclusionDefinition.CompactDistance,
                        ConclusionDefinition.IsReferencePreviewIdentity(evidence.Identity),
                        first.Report.ConclusionId + "," + second.Report.ConclusionId);
                }
            }
        }

        private static void EvaluateTraits(ICollection<ConclusionReport> reports)
        {
            foreach (ConclusionReport source in reports
                .Where(report => report.Outcome == ComponentOutcome.Supports)
                .ToArray())
            {
                string? traitId = null;
                if (source.ConclusionId == "FS-TOPOLOGY.shared-satellites")
                    traitId = "shared-birth-satellites";
                else if (source.ConclusionId == "FS-POWER.birth-tidal")
                    traitId = "birth-system-tidal-lock";
                else if (source.ConclusionId.StartsWith(
                    "FS-GAS-ROUTE.product:",
                    StringComparison.Ordinal))
                    traitId = "birth-system-gas-product:" +
                        source.ConclusionId.Substring("FS-GAS-ROUTE.product:".Length);
                else if (source.ConclusionId == "MF-SPHERE-GEOMETRY.containment")
                    traitId = "multiple-contained-orbits";
                else if (source.ConclusionId == "CX-GROUPING.distance" &&
                    source.SourceConclusionId?.Contains("strong-energy") == true)
                    traitId = "close-strong-energy-system";
                else if (source.ConclusionId.StartsWith(
                    "RR-ACCESS.distance:",
                    StringComparison.Ordinal))
                    traitId = "close-rare-access:" +
                        source.ConclusionId.Substring("RR-ACCESS.distance:".Length);

                if (traitId == null)
                    continue;

                reports.Add(new ConclusionReport(
                    source.Identity,
                    source.Settings,
                    source.Coverage,
                    "TRAIT-SUMMARY.registry:" + traitId,
                    ConclusionContext.DecisionRelevantTraits,
                    ConclusionDefinition.ContractVersion,
                    ConclusionDefinition.DefinitionVersion,
                    new ConclusionSubject(
                        SubjectKind.Trait,
                        traitId + "@" + source.Subject.Identifier),
                    ComponentOutcome.Supports,
                    Fact("trait", traitId, "trait-id"),
                    null,
                    source.ConclusionId));
            }
        }

        private static void AddRangeReport(
            NormalizedClusterEvidence evidence,
            ICollection<ConclusionReport> reports,
            EvidenceCoverage coverage,
            string conclusionId,
            ConclusionContext context,
            ConclusionSubject subject,
            string factId,
            decimal? value,
            AcceptedRange range,
            bool acceptedSettings,
            string? sourceConclusionId = null)
        {
            DiagnosticCause? unavailable = UnavailableCause(evidence, coverage);
            DecisiveFact? fact = value.HasValue
                ? DecimalFact(factId, value.Value, range.Unit)
                : null;
            if (unavailable != null)
            {
                reports.Add(Report(
                    evidence,
                    coverage,
                    conclusionId,
                    context,
                    subject,
                    ComponentOutcome.Unknown,
                    fact,
                    unavailable,
                    sourceConclusionId));
                return;
            }
            if (!value.HasValue)
            {
                reports.Add(Report(
                    evidence,
                    coverage,
                    conclusionId,
                    context,
                    subject,
                    ComponentOutcome.Unknown,
                    null,
                    MissingFact(factId),
                    sourceConclusionId));
                return;
            }
            if (!acceptedSettings)
            {
                reports.Add(Report(
                    evidence,
                    coverage,
                    conclusionId,
                    context,
                    subject,
                    ComponentOutcome.Unknown,
                    fact,
                    new DiagnosticCause(
                        "unsupported-definition-scope",
                        "The value is retained, but definition 0.1.0 has no " +
                        "range for these settings."),
                    sourceConclusionId));
                return;
            }

            reports.Add(Report(
                evidence,
                coverage,
                conclusionId,
                context,
                subject,
                ConclusionDefinition.Evaluate(value.Value, range),
                fact,
                null,
                sourceConclusionId));
        }

        private static void AddRole(
            NormalizedClusterEvidence evidence,
            ICollection<ConclusionReport> reports,
            ICollection<RoleAssignment> assignments,
            string roleId,
            ConclusionSubject subject,
            EvidenceCoverage coverage,
            string sourceConclusionId)
        {
            if (subject.Kind != SubjectKind.BirthSystem &&
                subject.Kind != SubjectKind.StarSystem)
            {
                return;
            }
            if (assignments.Any(role =>
                role.RoleId == roleId && role.Subject == subject))
            {
                return;
            }

            ConclusionReport report = Report(
                evidence,
                coverage,
                "MF-SYSTEM-ROLE.role:" + roleId,
                ConclusionContext.Megafactory,
                subject,
                ComponentOutcome.Supports,
                Fact("systemRole", roleId, "role-id"),
                null,
                sourceConclusionId);
            reports.Add(report);
            assignments.Add(new RoleAssignment(roleId, subject, report));
        }

        private static DiagnosticCause? UnavailableCause(
            NormalizedClusterEvidence evidence,
            EvidenceCoverage coverage)
        {
            if (evidence.Compatibility == CompatibilityState.Unsupported)
                return evidence.CompatibilityFailure;
            if (!coverage.IsComplete)
            {
                return new DiagnosticCause(
                    "incomplete-coverage",
                    "Complete " + coverage.Scope + " evidence is required.");
            }
            return null;
        }

        private static ConclusionReport Report(
            NormalizedClusterEvidence evidence,
            EvidenceCoverage coverage,
            string conclusionId,
            ConclusionContext context,
            ConclusionSubject subject,
            ComponentOutcome outcome,
            DecisiveFact? fact,
            DiagnosticCause? cause,
            string? sourceConclusionId = null)
        {
            return new ConclusionReport(
                evidence.Identity,
                evidence.Settings,
                coverage,
                conclusionId,
                context,
                ConclusionDefinition.ContractVersion,
                ConclusionDefinition.DefinitionVersion,
                subject,
                outcome,
                fact,
                cause,
                sourceConclusionId);
        }

        private static NormalizedSystemEvidence? BirthSystem(
            NormalizedClusterEvidence evidence)
        {
            return evidence.Systems.SingleOrDefault(system => system.IsBirthSystem);
        }

        private static ConclusionSubject BirthSubject(NormalizedClusterEvidence evidence)
        {
            return new ConclusionSubject(
                SubjectKind.BirthSystem,
                evidence.BirthSystemIdentifier);
        }

        private static ConclusionSubject ResourceSubject(
            NormalizedClusterEvidence evidence,
            string resourceId)
        {
            return new ConclusionSubject(
                SubjectKind.Resource,
                evidence.Identity.GalaxySeed.ToString(CultureInfo.InvariantCulture) +
                ":resource:" + resourceId);
        }

        private static DecisiveFact Fact(string factId, string value, string unit)
        {
            return new DecisiveFact(factId, value, unit);
        }

        private static DecisiveFact DecimalFact(string factId, decimal value, string unit)
        {
            return Fact(factId, value.ToString(CultureInfo.InvariantCulture), unit);
        }

        private static DecisiveFact IntegerFact(string factId, long value, string unit)
        {
            return Fact(factId, value.ToString(CultureInfo.InvariantCulture), unit);
        }

        private static DiagnosticCause MissingFact(string factId)
        {
            return new DiagnosticCause(
                "missing-evidence",
                "Normalized evidence is missing required fact " + factId + ".");
        }

        private static DiagnosticCause NoAcceptedRange(string componentId)
        {
            return new DiagnosticCause(
                "no-accepted-range",
                "Definition 0.1.0 has no accepted range for " + componentId + ".");
        }

        private sealed record RoleAssignment(
            string RoleId,
            ConclusionSubject Subject,
            ConclusionReport Report);
    }
}
