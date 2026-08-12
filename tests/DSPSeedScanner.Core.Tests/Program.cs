using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Core.Tests
{
    internal static class Program
    {
        private static int Main()
        {
            var tests = new (string Name, Action Body)[]
            {
                ("IMPL-01 boundary remains valid", Impl01BoundaryRemainsValid),
                ("fixed predicate fixtures", FixedPredicateFixtures),
                ("preview quantitative fixtures", PreviewQuantitativeFixtures),
                ("range endpoint directions", RangeEndpointDirections),
                ("starter resource fixtures", StarterResourceFixtures),
                ("sphere geometry fixtures", SphereGeometryFixtures),
                ("Dark Fog evidence remains nonjudgmental", DarkFogEvidenceRemainsNonjudgmental),
                ("grouping and rare-access fixtures", GroupingAndRareAccessFixtures),
                ("roles and trait registry", RolesAndTraitRegistry),
                ("settings boundaries", SettingsBoundaries),
                ("coverage and compatibility isolation", CoverageAndCompatibilityIsolation),
                ("deferred components stay unknown", DeferredComponentsStayUnknown),
                ("deterministic complete reports", DeterministicCompleteReports),
                ("no scoring or runtime dependencies", NoScoringOrRuntimeDependencies)
            };

            var failures = new List<string>();
            foreach ((string name, Action body) in tests)
            {
                try
                {
                    body();
                    Console.WriteLine("PASS {0}", name);
                }
                catch (Exception exception)
                {
                    failures.Add(name + ": " + exception.Message);
                    Console.Error.WriteLine("FAIL {0}: {1}", name, exception);
                }
            }

            Console.WriteLine(
                "RESULT {0}/{1} passed",
                tests.Length - failures.Count,
                tests.Length);
            return failures.Count == 0 ? 0 : 1;
        }

        private static void Impl01BoundaryRemainsValid()
        {
            FixtureOptions supporting = Options(16_315_224);
            supporting.SharedBirthGiantBodies = 3;
            FixtureOptions negative = Options(73_339_583);
            negative.SharedBirthGiantBodies = 1;

            Equal(
                ComponentOutcome.Supports,
                Find(Evaluate(supporting), "FS-TOPOLOGY.shared-satellites").Outcome);
            Equal(
                ComponentOutcome.DoesNotSupport,
                Find(Evaluate(negative), "FS-TOPOLOGY.shared-satellites").Outcome);

            FixtureOptions missing = Options(16_315_224);
            missing.UnavailableScope = EvidenceScope.BirthSystemTopology;
            ConclusionReport unknown = Find(
                Evaluate(missing),
                "FS-TOPOLOGY.shared-satellites");
            Equal(ComponentOutcome.Unknown, unknown.Outcome);
            Equal("incomplete-coverage", unknown.DiagnosticCause?.Code);

            GenerationIdentity first = Identity(16_315_224);
            GenerationIdentity equal = Identity(16_315_224);
            Equal(first, equal);
            Equal(
                new EvidenceKey(first, EvidenceStage.GalaxyPreview),
                new EvidenceKey(equal, EvidenceStage.GalaxyPreview));
            NotEqual(
                new EvidenceKey(first, EvidenceStage.GalaxyPreview),
                new EvidenceKey(equal, EvidenceStage.BirthSystemRaw));
        }

        private static void FixedPredicateFixtures()
        {
            AssertOutcome(
                Options(45_772, options => options.HasTidalLockedSolidPlanet = true),
                "FS-POWER.birth-tidal",
                ComponentOutcome.Supports);
            AssertOutcome(
                Options(73_339_583, options => options.HasTidalLockedSolidPlanet = false),
                "FS-POWER.birth-tidal",
                ComponentOutcome.DoesNotSupport);

            FixtureOptions fireIce = Options(45_772);
            fireIce.GasProducts.Add(new NormalizedGasProduct("fire-ice", 0.62m));
            fireIce.GasProducts.Add(new NormalizedGasProduct("hydrogen", 0.84m));
            IReadOnlyList<ConclusionReport> fireIceReports = Evaluate(fireIce);
            Equal(ComponentOutcome.Supports,
                Find(fireIceReports, "FS-GAS-ROUTE.product:fire-ice").Outcome);
            Equal(ComponentOutcome.Supports,
                Find(fireIceReports, "FS-GAS-ROUTE.product:hydrogen").Outcome);
            Equal(ComponentOutcome.DoesNotSupport,
                Find(fireIceReports, "FS-GAS-ROUTE.product:deuterium").Outcome);

            FixtureOptions deuterium = Options(73_339_583);
            deuterium.GasProducts.Add(new NormalizedGasProduct("deuterium", 0.04m));
            deuterium.GasProducts.Add(new NormalizedGasProduct("hydrogen", 0.91m));
            IReadOnlyList<ConclusionReport> deuteriumReports = Evaluate(deuterium);
            Equal(ComponentOutcome.Supports,
                Find(deuteriumReports, "FS-GAS-ROUTE.product:deuterium").Outcome);
            Equal(ComponentOutcome.DoesNotSupport,
                Find(deuteriumReports, "FS-GAS-ROUTE.product:fire-ice").Outcome);

            FixtureOptions rawPresent = Options(45_772);
            rawPresent.ContainsFireIce = true;
            FixtureOptions rawAbsent = Options(73_339_583);
            rawAbsent.ContainsFireIce = false;
            AssertOutcome(rawPresent, "FS-RESOURCES.fire-ice", ComponentOutcome.Supports);
            AssertOutcome(rawAbsent, "FS-RESOURCES.fire-ice", ComponentOutcome.DoesNotSupport);
        }

        private static void PreviewQuantitativeFixtures()
        {
            AssertSolar(57_213_558, 1.16m, ComponentOutcome.PreferenceSensitive);
            AssertSolar(89_864_814, 1.35m, ComponentOutcome.Supports);
            AssertSolar(16_315_224, 0.92m, ComponentOutcome.DoesNotSupport);

            AssertRange(
                "FS-POWER.wind",
                options => options.MaximumWindRatio = 0.99m,
                ComponentOutcome.DoesNotSupport);
            AssertRange(
                "FS-POWER.wind",
                options => options.MaximumWindRatio = 1.0m,
                ComponentOutcome.PreferenceSensitive);
            AssertRange(
                "FS-POWER.wind",
                options => options.MaximumWindRatio = 1.5m,
                ComponentOutcome.Supports);

            AssertEnergy(63_925_962, 2.404m, 1m, ComponentOutcome.DoesNotSupport);
            AssertEnergy(50_245_375, 2.4489998817m, 1m,
                ComponentOutcome.PreferenceSensitive);
            AssertEnergy(8_692_056, 2.4900000095m, 1m, ComponentOutcome.Supports);
            AssertEnergy(64_181_741, 2.698m, 1m, ComponentOutcome.Supports);

            FixtureOptions tiedLeader = Options(61_571_387);
            tiedLeader.OtherLuminosity = 2.489m;
            tiedLeader.BirthLuminosity = 2.486m;
            IReadOnlyList<ConclusionReport> tiedReports = Evaluate(tiedLeader);
            Equal(ComponentOutcome.PreferenceSensitive,
                Find(tiedReports, "MF-ENERGY-SYSTEM.output").Outcome);
            Equal(ComponentOutcome.DoesNotSupport,
                Find(tiedReports, "MF-ENERGY-SYSTEM.separation").Outcome);
            False(HasRole(tiedReports, "strong-energy"),
                "Preference-sensitive output must not create a strong-energy role.");
        }

        private static void RangeEndpointDirections()
        {
            AssertEnergySeparation(
                ConclusionDefinition.EnergySeparation.Lower,
                ComponentOutcome.PreferenceSensitive);
            AssertEnergySeparation(
                ConclusionDefinition.EnergySeparation.Upper,
                ComponentOutcome.Supports);
            AssertContainment(12_345_678, 2, ComponentOutcome.Supports);
            AssertGrouping(12_345_678, 2.698m, 2.5m, ComponentOutcome.Supports);
            AssertGrouping(
                12_345_679,
                2.698m,
                10m,
                ComponentOutcome.PreferenceSensitive);
            AssertRare(12_345_680, "unipolar-magnet", 2.5m,
                ComponentOutcome.Supports);
            AssertRare(12_345_681, "unipolar-magnet", 10m,
                ComponentOutcome.PreferenceSensitive);
        }

        private static void StarterResourceFixtures()
        {
            AssertStarterTotal(73_339_583, 60_569_720, ComponentOutcome.DoesNotSupport);
            AssertStarterTotal(63_015_198, 74_788_292,
                ComponentOutcome.PreferenceSensitive);
            AssertStarterTotal(48_823_053, 105_667_431, ComponentOutcome.Supports);
            AssertStarterTotal(96_178_012, 124_175_637, ComponentOutcome.Supports);

            FixtureOptions oilIsIndependent = Options(12_345_678);
            oilIsIndependent.ResourceAmounts["iron"] = 74_788_292;
            oilIsIndependent.ResourceAmounts["oil"] = 9_999_999;
            IReadOnlyList<ConclusionReport> oilReports = Evaluate(oilIsIndependent);
            Equal("74788292", Find(oilReports, "FS-RESOURCES.common-total").DecisiveFact?.Value);
            Equal("9999999", Find(oilReports, "FS-RESOURCES.amount:oil").DecisiveFact?.Value);

            foreach (string resourceId in ConclusionDefinition.CommonResourceIds)
            {
                AcceptedRange amount = ConclusionDefinition.StarterAmount(resourceId);
                AcceptedRange groups = ConclusionDefinition.StarterGroups(resourceId);
                AssertStarterMetric(resourceId, (long)amount.Lower - 1, (int)groups.Lower,
                    "amount", ComponentOutcome.DoesNotSupport);
                AssertStarterMetric(resourceId, (long)amount.Lower, (int)groups.Lower,
                    "amount", ComponentOutcome.PreferenceSensitive);
                AssertStarterMetric(resourceId, (long)amount.Upper, (int)groups.Lower,
                    "amount", ComponentOutcome.Supports);
                AssertStarterMetric(resourceId, (long)amount.Lower, (int)groups.Lower - 1,
                    "groups", ComponentOutcome.DoesNotSupport);
                AssertStarterMetric(resourceId, (long)amount.Lower, (int)groups.Lower,
                    "groups", ComponentOutcome.PreferenceSensitive);
                AssertStarterMetric(resourceId, (long)amount.Lower, (int)groups.Upper,
                    "groups", ComponentOutcome.Supports);
            }

            FixtureOptions independent = Options(12_345_678);
            independent.ResourceAmounts["iron"] =
                (long)ConclusionDefinition.StarterAmount("iron").Upper;
            independent.ResourceGroups["iron"] = 0;
            IReadOnlyList<ConclusionReport> reports = Evaluate(independent);
            Equal(ComponentOutcome.Supports,
                Find(reports, "FS-RESOURCES.amount:iron").Outcome);
            Equal(ComponentOutcome.DoesNotSupport,
                Find(reports, "FS-RESOURCES.groups:iron").Outcome);
        }

        private static void SphereGeometryFixtures()
        {
            AssertContainment(86_764_391, 0, ComponentOutcome.DoesNotSupport);
            AssertContainment(45_772, 1, ComponentOutcome.PreferenceSensitive);
            AssertContainment(48_823_053, 4, ComponentOutcome.Supports);

            AssertRadius(52_322_682, 76_200, ComponentOutcome.PreferenceSensitive);
            AssertRadius(74_250_347, 191_400, ComponentOutcome.Supports);
            AssertRadius(64_181_741, 234_200, ComponentOutcome.Supports);
        }

        private static void DarkFogEvidenceRemainsNonjudgmental()
        {
            FixtureOptions options = FogOptions(67_937_149, 39, true);
            NormalizedClusterEvidence evidence = BuildEvidence(options);
            Equal(1, evidence.Systems.Single(system => system.IsBirthSystem)
                .InitialHiveCount);
            Equal(39, evidence.Systems.Sum(system => system.InitialHiveCount ?? 0));

            IReadOnlyList<ConclusionReport> reports = ConclusionEngine.Evaluate(evidence);
            False(reports.Any(report =>
                report.ConclusionId.StartsWith("DF-", StringComparison.Ordinal) ||
                report.Context == ConclusionContext.DarkFogFarming ||
                report.ConclusionId.Contains("fog-opportunity", StringComparison.Ordinal)),
                "Dark Fog evidence must not produce judgments or roles.");
        }

        private static void GroupingAndRareAccessFixtures()
        {
            AssertGrouping(1_369, 2.504m, 2.274181m, ComponentOutcome.Supports);
            AssertGrouping(61_224_745, 2.698m, 4.621132m,
                ComponentOutcome.PreferenceSensitive);
            AssertGrouping(64_181_741, 2.698m, 19.521508m,
                ComponentOutcome.DoesNotSupport);

            AssertRare(73_339_583, "kimberlite", 2.028m, ComponentOutcome.Supports);
            AssertRare(96_178_012, "unipolar-magnet", 7.353m,
                ComponentOutcome.PreferenceSensitive);
            AssertRare(45_772, "unipolar-magnet", 38.495m,
                ComponentOutcome.DoesNotSupport);

            FixtureOptions absent = Options(12_345_678);
            absent.RareResources.Add(new NormalizedRareResourceEvidence(
                "unipolar-magnet",
                false,
                null,
                null));
            Equal(ComponentOutcome.DoesNotSupport,
                Find(Evaluate(absent), "RR-ACCESS.distance:unipolar-magnet").Outcome);
        }

        private static void RolesAndTraitRegistry()
        {
            FixtureOptions strong = Options(64_181_741);
            strong.OtherLuminosity = 2.698m;
            IReadOnlyList<ConclusionReport> strongReports = Evaluate(strong);
            True(HasRole(strongReports, "strong-energy"),
                "Supporting output should create the strong-energy role.");
            True(HasRole(strongReports, "starter-anchor"),
                "The birth system should retain its fixed starter-anchor role.");

            FixtureOptions sensitive = Options(61_571_387);
            sensitive.OtherLuminosity = 2.489m;
            sensitive.BirthLuminosity = 2.486m;
            False(HasRole(Evaluate(sensitive), "strong-energy"),
                "Sensitive output cannot create a role.");

            FixtureOptions derivedRoles = Options(48_823_053);
            derivedRoles.OtherMaximumShellRadius = 234_200;
            derivedRoles.OtherContainedOrbitCount = 4;
            derivedRoles.OtherInitialHiveCount = 39;
            derivedRoles.RareResources.Add(new NormalizedRareResourceEvidence(
                "unipolar-magnet",
                true,
                OtherSubject(derivedRoles.Seed),
                2m,
                1_000,
                2));
            IReadOnlyList<ConclusionReport> roleReports = Evaluate(derivedRoles);
            True(HasRole(roleReports, "large-shell"),
                "Supporting radius should create a large-shell role.");
            True(HasRole(roleReports, "orbit-containment"),
                "Supporting containment should create an orbit-containment role.");
            False(HasRole(roleReports, "fog-opportunity"),
                "Occupation evidence must not create a Megafactory role.");
            True(HasRole(roleReports, "rare-access"),
                "Supporting rare access should create a rare-access role.");
            True(roleReports.Any(report => report.ConclusionId ==
                "TRAIT-SUMMARY.registry:multiple-contained-orbits"),
                "Supporting containment should create its registered trait.");
            True(roleReports.Any(report => report.ConclusionId ==
                "TRAIT-SUMMARY.registry:close-rare-access:unipolar-magnet"),
                "Supporting rare access should create its registered trait.");

            FixtureOptions traitFixture = Options(45_772);
            traitFixture.SharedBirthGiantBodies = 3;
            traitFixture.HasTidalLockedSolidPlanet = true;
            traitFixture.GasProducts.Add(new NormalizedGasProduct("fire-ice", 0.5m));
            string[] traits = Evaluate(traitFixture)
                .Where(report => report.ConclusionId.StartsWith(
                    "TRAIT-SUMMARY.registry:",
                    StringComparison.Ordinal))
                .Select(report => report.ConclusionId)
                .OrderBy(id => id, StringComparer.Ordinal)
                .ToArray();
            SequenceEqual(
                new[]
                {
                    "TRAIT-SUMMARY.registry:birth-system-gas-product:fire-ice",
                    "TRAIT-SUMMARY.registry:birth-system-tidal-lock",
                    "TRAIT-SUMMARY.registry:shared-birth-satellites"
                },
                traits);
            True(traits.All(trait => !trait.Contains("O", StringComparison.Ordinal)),
                "O-star count must not create a trait.");

            foreach (ConclusionReport trait in Evaluate(traitFixture).Where(report =>
                report.ConclusionId.StartsWith("TRAIT-SUMMARY", StringComparison.Ordinal)))
            {
                True(trait.SourceConclusionId != null,
                    "Every trait must name its source conclusion.");
                Equal(ComponentOutcome.Supports, trait.Outcome);
            }
        }

        private static void SettingsBoundaries()
        {
            FixtureOptions alteredResources = Options(12_345_678);
            alteredResources.ResourceMultiplier = 0.5m;
            alteredResources.ContainsFireIce = true;
            alteredResources.ResourceAmounts["iron"] = 30_000_000;
            alteredResources.ResourceGroups["iron"] = 30;
            IReadOnlyList<ConclusionReport> resourceReports = Evaluate(alteredResources);
            Equal(ComponentOutcome.Unknown,
                Find(resourceReports, "FS-RESOURCES.amount:iron").Outcome);
            Equal(ComponentOutcome.Unknown,
                Find(resourceReports, "FS-RESOURCES.groups:iron").Outcome);
            Equal("30000000",
                Find(resourceReports, "FS-RESOURCES.amount:iron").DecisiveFact?.Value);
            Equal(ComponentOutcome.Supports,
                Find(resourceReports, "FS-RESOURCES.fire-ice").Outcome);

            FixtureOptions otherStarCount = Options(12_345_678);
            otherStarCount.RequestedStarCount = 32;
            otherStarCount.HasTidalLockedSolidPlanet = true;
            otherStarCount.MaximumSolarRatio = 1.5m;
            IReadOnlyList<ConclusionReport> starCountReports = Evaluate(otherStarCount);
            Equal(ComponentOutcome.Unknown,
                Find(starCountReports, "FS-POWER.solar").Outcome);
            Equal(ComponentOutcome.Supports,
                Find(starCountReports, "FS-POWER.birth-tidal").Outcome);
        }

        private static void CoverageAndCompatibilityIsolation()
        {
            FixtureOptions partialRaw = Options(12_345_678);
            partialRaw.PartialScope = EvidenceScope.BirthSystemResources;
            partialRaw.MaximumSolarRatio = 1.35m;
            IReadOnlyList<ConclusionReport> partialReports = Evaluate(partialRaw);
            Equal(ComponentOutcome.Unknown,
                Find(partialReports, "FS-RESOURCES.common-total").Outcome);
            Equal(ComponentOutcome.Unknown,
                Find(partialReports, "FS-RESOURCES.fire-ice").Outcome);
            Equal(ComponentOutcome.Supports,
                Find(partialReports, "FS-POWER.solar").Outcome);

            FixtureOptions incompatible = Options(12_345_678);
            incompatible.Compatibility = CompatibilityState.Unsupported;
            IReadOnlyList<ConclusionReport> incompatibleReports = Evaluate(incompatible);
            foreach (ConclusionReport report in incompatibleReports.Where(report =>
                report.Outcome != ComponentOutcome.NotApplicable))
            {
                Equal(ComponentOutcome.Unknown, report.Outcome);
                Equal("unsupported-runtime", report.DiagnosticCause?.Code);
            }
        }

        private static void DeferredComponentsStayUnknown()
        {
            FixtureOptions first = Options(61_571_387);
            first.ClusterCommonResourceTotal = 32_048_044_700;
            FixtureOptions second = Options(96_178_012);
            second.ClusterCommonResourceTotal = 19_481_451_769;
            foreach (FixtureOptions options in new[] { first, second })
            {
                ConclusionReport report = Find(
                    Evaluate(options),
                    "MF-RESOURCE-SCOPE.strength");
                Equal(ComponentOutcome.Unknown, report.Outcome);
                Equal("no-accepted-range", report.DiagnosticCause?.Code);
                True(report.DecisiveFact != null, "Exact cluster total should remain attributed.");
            }

            FixtureOptions gas = Options(45_772);
            gas.GasProducts.Add(new NormalizedGasProduct("hydrogen", 1.25m));
            ConclusionReport gasRate = Find(
                Evaluate(gas),
                "FS-GAS-ROUTE.rate:hydrogen");
            Equal(ComponentOutcome.Unknown, gasRate.Outcome);
            Equal("1.25", gasRate.DecisiveFact?.Value);

            FixtureOptions rare = Options(96_178_012);
            rare.RareResources.Add(new NormalizedRareResourceEvidence(
                "unipolar-magnet",
                true,
                OtherSubject(rare.Seed),
                7.353m,
                amount: 99_999,
                veinGroups: 4));
            ConclusionReport rareAmount = Find(
                Evaluate(rare),
                "RR-ACCESS.amount:unipolar-magnet");
            Equal(ComponentOutcome.Unknown, rareAmount.Outcome);
            Equal("99999", rareAmount.DecisiveFact?.Value);
        }

        private static void DeterministicCompleteReports()
        {
            FixtureOptions first = Options(67_937_149);
            first.SharedBirthGiantBodies = 3;
            first.HasTidalLockedSolidPlanet = true;
            first.MaximumSolarRatio = 1.2m;
            first.MaximumWindRatio = 1.5m;
            first.OtherLuminosity = 2.698m;
            first.OtherMaximumShellRadius = 234_200;
            first.OtherContainedOrbitCount = 4;
            first.BirthInitialHiveCount = 1;
            first.OtherInitialHiveCount = 38;
            first.GasProducts.Add(new NormalizedGasProduct("hydrogen", 1.2m));
            first.SystemDistanceLy = 2m;
            first.RareResources.Add(new NormalizedRareResourceEvidence(
                "unipolar-magnet",
                true,
                OtherSubject(first.Seed),
                2m,
                1_000,
                2));

            FixtureOptions second = first.Clone();
            IReadOnlyList<ConclusionReport> firstReports = Evaluate(first);
            IReadOnlyList<ConclusionReport> secondReports = Evaluate(second);
            SequenceEqual(firstReports, secondReports);

            foreach (ConclusionReport report in firstReports)
            {
                Equal(ConclusionDefinition.ContractVersion, report.ContractVersion);
                Equal(ConclusionDefinition.DefinitionVersion, report.DefinitionVersion);
                Equal(first.ResourceMultiplier, report.Settings.ResourceMultiplier);
                True(!String.IsNullOrWhiteSpace(report.Subject.Identifier),
                    "Every report requires a subject.");
                if (report.DecisiveFact != null)
                    True(!String.IsNullOrWhiteSpace(report.DecisiveFact.Unit),
                        "Every decisive fact requires a unit.");
            }
        }

        private static void NoScoringOrRuntimeDependencies()
        {
            string[] forbiddenMemberTerms = { "Score", "Weight", "Rank" };
            MemberInfo[] publicMembers = typeof(ConclusionReport).Assembly
                .GetExportedTypes()
                .SelectMany(type => type.GetMembers(BindingFlags.Public |
                    BindingFlags.Instance | BindingFlags.Static))
                .ToArray();
            foreach (string term in forbiddenMemberTerms)
            {
                True(publicMembers.All(member =>
                    member.Name.IndexOf(term, StringComparison.OrdinalIgnoreCase) < 0),
                    "The public core contract must not expose " + term + ".");
            }

            string[] forbiddenPrefixes = { "Assembly-CSharp", "BepInEx", "UnityEngine" };
            string[] references = typeof(GenerationIdentity).Assembly
                .GetReferencedAssemblies()
                .Select(reference => reference.Name ?? String.Empty)
                .ToArray();
            foreach (string prefix in forbiddenPrefixes)
            {
                True(references.All(reference =>
                    !reference.StartsWith(prefix, StringComparison.Ordinal)),
                    "Core assembly references forbidden runtime dependency " + prefix + ".");
            }

            MethodInfo[] evaluateMethods = typeof(ConclusionEngine).GetMethods(
                BindingFlags.Public | BindingFlags.Static);
            Equal(1, evaluateMethods.Length);
            Equal(1, evaluateMethods[0].GetParameters().Length);
            True(evaluateMethods[0].GetParameters()[0].ParameterType ==
                typeof(NormalizedClusterEvidence),
                "The neutral engine must not require player preferences.");
        }

        private static void AssertSolar(int seed, decimal value, ComponentOutcome outcome)
        {
            FixtureOptions options = Options(seed);
            options.MaximumSolarRatio = value;
            AssertOutcome(options, "FS-POWER.solar", outcome);
        }

        private static void AssertEnergy(
            int seed,
            decimal maximum,
            decimal second,
            ComponentOutcome outcome)
        {
            FixtureOptions options = Options(seed);
            options.OtherLuminosity = maximum;
            options.BirthLuminosity = second;
            AssertOutcome(options, "MF-ENERGY-SYSTEM.output", outcome);
        }

        private static void AssertEnergySeparation(
            decimal ratio,
            ComponentOutcome outcome)
        {
            FixtureOptions options = Options(12_345_678);
            options.BirthLuminosity = 1m;
            options.OtherLuminosity = ratio;
            AssertOutcome(options, "MF-ENERGY-SYSTEM.separation", outcome);
        }

        private static void AssertStarterTotal(
            int seed,
            long total,
            ComponentOutcome outcome)
        {
            FixtureOptions options = Options(seed);
            foreach (string resourceId in ConclusionDefinition.CommonResourceIds)
                options.ResourceAmounts[resourceId] = 0;
            options.ResourceAmounts["iron"] = total;
            AssertOutcome(options, "FS-RESOURCES.common-total", outcome);
        }

        private static void AssertStarterMetric(
            string resourceId,
            long amount,
            int groups,
            string component,
            ComponentOutcome outcome)
        {
            FixtureOptions options = Options(12_345_678);
            options.ResourceAmounts[resourceId] = amount;
            options.ResourceGroups[resourceId] = groups;
            AssertOutcome(
                options,
                "FS-RESOURCES." + component + ":" + resourceId,
                outcome);
        }

        private static void AssertContainment(
            int seed,
            int count,
            ComponentOutcome outcome)
        {
            FixtureOptions options = Options(seed);
            options.OtherContainedOrbitCount = count;
            ConclusionReport report = Find(
                Evaluate(options),
                "MF-SPHERE-GEOMETRY.containment",
                OtherSubject(seed).Identifier);
            Equal(outcome, report.Outcome);
        }

        private static void AssertRadius(int seed, long radius, ComponentOutcome outcome)
        {
            FixtureOptions options = Options(seed);
            options.OtherMaximumShellRadius = radius;
            options.BirthMaximumShellRadius = 50_000;
            AssertOutcome(options, "MF-SPHERE-GEOMETRY.radius", outcome);
        }

        private static FixtureOptions FogOptions(int seed, int total, bool birthExposure)
        {
            FixtureOptions options = Options(seed);
            options.BirthInitialHiveCount = birthExposure ? 1 : 0;
            options.OtherInitialHiveCount = total - options.BirthInitialHiveCount;
            return options;
        }

        private static void AssertGrouping(
            int seed,
            decimal luminosity,
            decimal distance,
            ComponentOutcome outcome)
        {
            FixtureOptions options = Options(seed);
            options.OtherLuminosity = luminosity;
            options.BirthLuminosity = 1m;
            options.SystemDistanceLy = distance;
            ConclusionReport report = Evaluate(options).Single(item =>
                item.ConclusionId == "CX-GROUPING.distance" &&
                item.SourceConclusionId?.Contains("strong-energy") == true &&
                item.SourceConclusionId.Contains("starter-anchor"));
            Equal(outcome, report.Outcome);
        }

        private static void AssertRare(
            int seed,
            string resourceId,
            decimal distance,
            ComponentOutcome outcome)
        {
            FixtureOptions options = Options(seed);
            options.RareResources.Add(new NormalizedRareResourceEvidence(
                resourceId,
                true,
                OtherSubject(seed),
                distance,
                amount: 1_000,
                veinGroups: 2));
            IReadOnlyList<ConclusionReport> reports = Evaluate(options);
            Equal(outcome, Find(reports, "RR-ACCESS.distance:" + resourceId).Outcome);
            Equal(ComponentOutcome.Unknown,
                Find(reports, "RR-ACCESS.amount:" + resourceId).Outcome);
        }

        private static void AssertRange(
            string conclusionId,
            Action<FixtureOptions> configure,
            ComponentOutcome outcome)
        {
            FixtureOptions options = Options(12_345_678, configure);
            AssertOutcome(options, conclusionId, outcome);
        }

        private static void AssertOutcome(
            FixtureOptions options,
            string conclusionId,
            ComponentOutcome outcome)
        {
            Equal(outcome, Find(Evaluate(options), conclusionId).Outcome);
        }

        private static IReadOnlyList<ConclusionReport> Evaluate(FixtureOptions options)
        {
            return ConclusionEngine.Evaluate(BuildEvidence(options));
        }

        private static NormalizedClusterEvidence BuildEvidence(FixtureOptions options)
        {
            var systems = new[]
            {
                new NormalizedSystemEvidence(
                    BirthSubject(options.Seed),
                    isBirthSystem: true,
                    sharedBirthGiantBodies: options.SharedBirthGiantBodies,
                    hasTidalLockedSolidPlanet: options.HasTidalLockedSolidPlanet,
                    maximumSolarRatio: options.MaximumSolarRatio,
                    maximumWindRatio: options.MaximumWindRatio,
                    giantProducts: options.GasProducts,
                    dysonLuminosity: options.BirthLuminosity,
                    maximumShellRadius: options.BirthMaximumShellRadius,
                    containedOrbitCount: options.BirthContainedOrbitCount,
                    initialHiveCount: options.BirthInitialHiveCount),
                new NormalizedSystemEvidence(
                    OtherSubject(options.Seed),
                    isBirthSystem: false,
                    dysonLuminosity: options.OtherLuminosity,
                    maximumShellRadius: options.OtherMaximumShellRadius,
                    containedOrbitCount: options.OtherContainedOrbitCount,
                    initialHiveCount: options.OtherInitialHiveCount)
            };
            StarterResourceMetric[] resources = ConclusionDefinition.CommonResourceIds
                .Select(resourceId => new StarterResourceMetric(
                    resourceId,
                    options.ResourceAmounts[resourceId],
                    options.ResourceGroups[resourceId]))
                .ToArray();

            return new NormalizedClusterEvidence(
                Identity(options.Seed, options.RequestedStarCount),
                new EvaluationSettings(
                    options.ResourceMultiplier,
                    options.CombatMode,
                    options.CombatSettingsKey),
                new ConclusionSubject(
                    SubjectKind.Cluster,
                    options.Seed.ToString(CultureInfo.InvariantCulture) + ":cluster"),
                BirthSubject(options.Seed).Identifier,
                Coverages(options.PartialScope, options.UnavailableScope),
                systems,
                new NormalizedStarterResourceEvidence(
                    BirthSubject(options.Seed),
                    resources,
                    options.ContainsFireIce),
                options.RareResources,
                new[]
                {
                    new NormalizedSystemDistance(
                        BirthSubject(options.Seed).Identifier,
                        OtherSubject(options.Seed).Identifier,
                        options.SystemDistanceLy)
                },
                options.ClusterCommonResourceTotal,
                options.Compatibility,
                options.Compatibility == CompatibilityState.Unsupported
                    ? new DiagnosticCause(
                        "unsupported-runtime",
                        "The fixture runtime is deliberately unsupported.")
                    : null);
        }

        private static IEnumerable<EvidenceCoverage> Coverages(
            EvidenceScope? partialScope,
            EvidenceScope? unavailableScope)
        {
            foreach (EvidenceScope scope in Enum.GetValues(typeof(EvidenceScope)))
            {
                EvidenceStage stage = Stage(scope);
                if (scope == partialScope)
                {
                    yield return new EvidenceCoverage(
                        stage,
                        scope,
                        CoverageState.Partial,
                        expectedSubjects: 2,
                        completedSubjects: 1);
                }
                else if (scope == unavailableScope)
                {
                    yield return new EvidenceCoverage(
                        stage,
                        scope,
                        CoverageState.Unavailable,
                        expectedSubjects: 1,
                        completedSubjects: 0);
                }
                else
                {
                    int subjects = scope == EvidenceScope.BirthSystemTopology ||
                        scope == EvidenceScope.BirthSystemRotation ||
                        scope == EvidenceScope.BirthSystemPower ||
                        scope == EvidenceScope.BirthSystemGasProducts ||
                        scope == EvidenceScope.BirthSystemResources ||
                        scope == EvidenceScope.SystemDistances
                        ? 1
                        : 2;
                    yield return new EvidenceCoverage(
                        stage,
                        scope,
                        CoverageState.Complete,
                        subjects,
                        subjects);
                }
            }
        }

        private static EvidenceStage Stage(EvidenceScope scope)
        {
            if (scope == EvidenceScope.BirthSystemResources)
                return EvidenceStage.BirthSystemRaw;
            if (scope == EvidenceScope.CompleteClusterRareResources ||
                scope == EvidenceScope.CompleteClusterResources)
                return EvidenceStage.CompleteClusterRaw;
            return EvidenceStage.GalaxyPreview;
        }

        private static FixtureOptions Options(
            int seed,
            Action<FixtureOptions>? configure = null)
        {
            var options = new FixtureOptions(seed);
            configure?.Invoke(options);
            return options;
        }

        private static GenerationIdentity Identity(int seed, int starCount = 64)
        {
            return new GenerationIdentity(
                ConclusionDefinition.ReferenceGameVersion,
                ConclusionDefinition.ReferenceGalaxyAlgorithm,
                ConclusionDefinition.ReferenceAssemblySha256,
                ConclusionDefinition.ReferenceOrderedThemeIds,
                ConclusionDefinition.DefinitionVersion,
                seed,
                starCount,
                ConclusionDefinition.ReferenceGameVersion);
        }

        private static ConclusionSubject BirthSubject(int seed)
        {
            return new ConclusionSubject(
                SubjectKind.BirthSystem,
                seed.ToString(CultureInfo.InvariantCulture) + ":birth-system");
        }

        private static ConclusionSubject OtherSubject(int seed)
        {
            return new ConclusionSubject(
                SubjectKind.StarSystem,
                seed.ToString(CultureInfo.InvariantCulture) + ":system-2");
        }

        private static ConclusionReport Find(
            IEnumerable<ConclusionReport> reports,
            string conclusionId,
            string? subjectIdentifier = null)
        {
            ConclusionReport[] matches = reports.Where(report =>
                report.ConclusionId == conclusionId &&
                (subjectIdentifier == null ||
                 report.Subject.Identifier == subjectIdentifier)).ToArray();
            if (matches.Length != 1)
            {
                throw new InvalidOperationException(
                    "Expected one report for " + conclusionId + " but found " +
                    matches.Length + ".");
            }
            return matches[0];
        }

        private static bool HasRole(
            IEnumerable<ConclusionReport> reports,
            string roleId)
        {
            return reports.Any(report =>
                report.ConclusionId == "MF-SYSTEM-ROLE.role:" + roleId &&
                report.Outcome == ComponentOutcome.Supports);
        }

        private static void Equal<T>(T expected, T actual)
        {
            if (!EqualityComparer<T>.Default.Equals(expected, actual))
            {
                throw new InvalidOperationException(
                    "Expected '" + expected + "' but found '" + actual + "'.");
            }
        }

        private static void NotEqual<T>(T first, T second)
        {
            if (EqualityComparer<T>.Default.Equals(first, second))
                throw new InvalidOperationException("Values should differ.");
        }

        private static void SequenceEqual<T>(IEnumerable<T> expected, IEnumerable<T> actual)
        {
            if (!expected.SequenceEqual(actual))
                throw new InvalidOperationException("Sequences should be equal.");
        }

        private static void True(bool condition, string message)
        {
            if (!condition)
                throw new InvalidOperationException(message);
        }

        private static void False(bool condition, string message)
        {
            True(!condition, message);
        }

        private sealed class FixtureOptions
        {
            public FixtureOptions(int seed)
            {
                Seed = seed;
                foreach (string resourceId in ConclusionDefinition.CommonResourceIds)
                {
                    ResourceAmounts.Add(resourceId, 0);
                    ResourceGroups.Add(resourceId, 0);
                }
            }

            public int Seed { get; }
            public int RequestedStarCount { get; set; } = 64;
            public decimal ResourceMultiplier { get; set; } = 1m;
            public CombatMode CombatMode { get; set; } = CombatMode.Combat;
            public string CombatSettingsKey { get; set; } =
                ConclusionDefinition.ReferenceCombatSettingsKey;
            public int SharedBirthGiantBodies { get; set; } = 1;
            public bool HasTidalLockedSolidPlanet { get; set; }
            public decimal MaximumSolarRatio { get; set; } = 0.9m;
            public decimal MaximumWindRatio { get; set; } = 0.9m;
            public List<NormalizedGasProduct> GasProducts { get; } =
                new List<NormalizedGasProduct>();
            public decimal BirthLuminosity { get; set; } = 1m;
            public decimal OtherLuminosity { get; set; } = 2.4m;
            public long BirthMaximumShellRadius { get; set; } = 50_000;
            public long OtherMaximumShellRadius { get; set; } = 60_000;
            public int BirthContainedOrbitCount { get; set; }
            public int OtherContainedOrbitCount { get; set; }
            public int BirthInitialHiveCount { get; set; }
            public int OtherInitialHiveCount { get; set; }
            public bool ContainsFireIce { get; set; }
            public Dictionary<string, long> ResourceAmounts { get; } =
                new Dictionary<string, long>(StringComparer.Ordinal);
            public Dictionary<string, int> ResourceGroups { get; } =
                new Dictionary<string, int>(StringComparer.Ordinal);
            public List<NormalizedRareResourceEvidence> RareResources { get; } =
                new List<NormalizedRareResourceEvidence>();
            public decimal SystemDistanceLy { get; set; } = 20m;
            public long? ClusterCommonResourceTotal { get; set; }
            public EvidenceScope? PartialScope { get; set; }
            public EvidenceScope? UnavailableScope { get; set; }
            public CompatibilityState Compatibility { get; set; } =
                CompatibilityState.Supported;

            public FixtureOptions Clone()
            {
                var clone = new FixtureOptions(Seed)
                {
                    RequestedStarCount = RequestedStarCount,
                    ResourceMultiplier = ResourceMultiplier,
                    CombatMode = CombatMode,
                    CombatSettingsKey = CombatSettingsKey,
                    SharedBirthGiantBodies = SharedBirthGiantBodies,
                    HasTidalLockedSolidPlanet = HasTidalLockedSolidPlanet,
                    MaximumSolarRatio = MaximumSolarRatio,
                    MaximumWindRatio = MaximumWindRatio,
                    BirthLuminosity = BirthLuminosity,
                    OtherLuminosity = OtherLuminosity,
                    BirthMaximumShellRadius = BirthMaximumShellRadius,
                    OtherMaximumShellRadius = OtherMaximumShellRadius,
                    BirthContainedOrbitCount = BirthContainedOrbitCount,
                    OtherContainedOrbitCount = OtherContainedOrbitCount,
                    BirthInitialHiveCount = BirthInitialHiveCount,
                    OtherInitialHiveCount = OtherInitialHiveCount,
                    ContainsFireIce = ContainsFireIce,
                    SystemDistanceLy = SystemDistanceLy,
                    ClusterCommonResourceTotal = ClusterCommonResourceTotal,
                    PartialScope = PartialScope,
                    UnavailableScope = UnavailableScope,
                    Compatibility = Compatibility
                };
                clone.GasProducts.AddRange(GasProducts);
                clone.RareResources.AddRange(RareResources);
                foreach (string resourceId in ConclusionDefinition.CommonResourceIds)
                {
                    clone.ResourceAmounts[resourceId] = ResourceAmounts[resourceId];
                    clone.ResourceGroups[resourceId] = ResourceGroups[resourceId];
                }
                return clone;
            }
        }
    }
}
