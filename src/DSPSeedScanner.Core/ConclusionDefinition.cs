using System;
using System.Collections.Generic;

namespace DSPSeedScanner.Core
{
    public enum ThresholdDirection
    {
        Increasing,
        Decreasing
    }

    public sealed record AcceptedRange
    {
        public AcceptedRange(
            decimal lower,
            decimal upper,
            ThresholdDirection direction,
            string unit)
        {
            if (lower > upper)
                throw new ArgumentException("The lower endpoint cannot exceed the upper endpoint.");
            if (String.IsNullOrWhiteSpace(unit))
                throw new ArgumentException("A unit is required.", nameof(unit));

            Lower = lower;
            Upper = upper;
            Direction = direction;
            Unit = unit;
        }

        public decimal Lower { get; }

        public decimal Upper { get; }

        public ThresholdDirection Direction { get; }

        public string Unit { get; }
    }

    public static class ConclusionDefinition
    {
        public const string ContractVersion = "0.1.0";
        public const string DefinitionVersion = "0.1.0";
        public const string ReferenceGameVersion = "0.10.34.28529";
        public const int ReferenceGalaxyAlgorithm = 20_200_403;
        public const int ReferenceStarCount = 64;
        public const string ReferenceAssemblySha256 =
            "AE0BA95F75BD879A62AA4CE253B2AB78EAA4FB3C7C595F5E1FEE75EBE0E0EF85";
        public const string ReferenceOrderedThemeIds =
            "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20,21,22,23,24,25";
        public const string ReferenceCombatSettingsKey =
            "GameDesc.SetForNewGame:0.10.34.28529";

        public static readonly IReadOnlyList<string> GasProductIds = Array.AsReadOnly(
            new[] { "deuterium", "fire-ice", "hydrogen" });

        public static readonly IReadOnlyList<string> CommonResourceIds = Array.AsReadOnly(
            new[] { "coal", "copper", "iron", "oil", "silicon", "stone", "titanium" });

        public static readonly IReadOnlyList<string> StarterTotalResourceIds = Array.AsReadOnly(
            new[] { "coal", "copper", "iron", "silicon", "stone", "titanium" });

        public static readonly IReadOnlyList<string> RareResourceIds = Array.AsReadOnly(
            new[]
            {
                "fire-ice",
                "fractal-silicon",
                "kimberlite",
                "optical-grating-crystal",
                "organic-crystal",
                "spiniform-stalagmite-crystal",
                "unipolar-magnet"
            });

        public static readonly AcceptedRange Solar =
            new AcceptedRange(1.16m, 1.35m, ThresholdDirection.Increasing, "ratio");
        public static readonly AcceptedRange Wind =
            new AcceptedRange(1.0m, 1.5m, ThresholdDirection.Increasing, "ratio");
        public static readonly AcceptedRange StarterCommonTotal =
            new AcceptedRange(74_788_292m, 105_667_431m, ThresholdDirection.Increasing,
                "runtime-amount-units");
        public static readonly AcceptedRange EnergyOutput =
            new AcceptedRange(2.4489998817m, 2.4900000095m,
                ThresholdDirection.Increasing, "dyson-luminosity");
        public static readonly AcceptedRange EnergySeparation =
            new AcceptedRange(1.1104599329m, 1.2183275480m,
                ThresholdDirection.Increasing, "ratio");
        public static readonly AcceptedRange SphereRadius =
            new AcceptedRange(76_200m, 191_400m, ThresholdDirection.Increasing,
                "radius-units");
        public static readonly AcceptedRange OrbitContainment =
            new AcceptedRange(1m, 2m, ThresholdDirection.Increasing, "orbits");
        public static readonly AcceptedRange FogOpportunity =
            new AcceptedRange(34m, 39m, ThresholdDirection.Increasing, "hives");
        public static readonly AcceptedRange CompactDistance =
            new AcceptedRange(2.5m, 10m, ThresholdDirection.Decreasing, "light-years");
        public static readonly AcceptedRange RareAccessDistance =
            new AcceptedRange(2.5m, 10m, ThresholdDirection.Decreasing, "light-years");

        public static bool IsReferencePreviewIdentity(GenerationIdentity identity)
        {
            if (identity == null)
                throw new ArgumentNullException(nameof(identity));

            return String.Equals(identity.GameVersion, ReferenceGameVersion,
                       StringComparison.Ordinal) &&
                   identity.GalaxyAlgorithm == ReferenceGalaxyAlgorithm &&
                   String.Equals(identity.AssemblySha256, ReferenceAssemblySha256,
                       StringComparison.Ordinal) &&
                   String.Equals(identity.OrderedThemeIds, ReferenceOrderedThemeIds,
                       StringComparison.Ordinal) &&
                   String.Equals(identity.ScannerCompatibilityVersion, DefinitionVersion,
                       StringComparison.Ordinal) &&
                   identity.RequestedStarCount == ReferenceStarCount &&
                   String.Equals(identity.CreationVersion, ReferenceGameVersion,
                       StringComparison.Ordinal);
        }

        public static AcceptedRange StarterAmount(string resourceId)
        {
            return resourceId switch
            {
                "iron" => Increasing(9_151_265m, 26_773_650m, "runtime-amount-units"),
                "copper" => Increasing(12_078_923m, 29_497_621m, "runtime-amount-units"),
                "silicon" => Increasing(3_355_497m, 12_453_357m, "runtime-amount-units"),
                "titanium" => Increasing(11_403_989m, 21_808_706m, "runtime-amount-units"),
                "stone" => Increasing(8_939_801m, 20_925_618m, "runtime-amount-units"),
                "coal" => Increasing(9_495_641m, 10_938_129m, "runtime-amount-units"),
                "oil" => Increasing(1_196_959m, 1_304_446m, "runtime-amount-units"),
                _ => throw new ArgumentOutOfRangeException(nameof(resourceId))
            };
        }

        public static AcceptedRange StarterGroups(string resourceId)
        {
            return resourceId switch
            {
                "iron" => Increasing(16m, 27m, "vein-groups"),
                "copper" => Increasing(18m, 28m, "vein-groups"),
                "silicon" => Increasing(4m, 11m, "vein-groups"),
                "titanium" => Increasing(8m, 15m, "vein-groups"),
                "stone" => Increasing(14m, 21m, "vein-groups"),
                "coal" => Increasing(13m, 15m, "vein-groups"),
                "oil" => Increasing(17m, 19m, "vein-groups"),
                _ => throw new ArgumentOutOfRangeException(nameof(resourceId))
            };
        }

        public static ComponentOutcome Evaluate(decimal value, AcceptedRange range)
        {
            if (range == null)
                throw new ArgumentNullException(nameof(range));

            if (range.Direction == ThresholdDirection.Increasing)
            {
                if (value >= range.Upper)
                    return ComponentOutcome.Supports;
                if (value < range.Lower)
                    return ComponentOutcome.DoesNotSupport;
                return ComponentOutcome.PreferenceSensitive;
            }

            if (value <= range.Lower)
                return ComponentOutcome.Supports;
            if (value > range.Upper)
                return ComponentOutcome.DoesNotSupport;
            return ComponentOutcome.PreferenceSensitive;
        }

        private static AcceptedRange Increasing(decimal lower, decimal upper, string unit)
        {
            return new AcceptedRange(lower, upper, ThresholdDirection.Increasing, unit);
        }
    }
}
