using System;
using System.Collections.Generic;
using System.Linq;

namespace DSPSeedScanner.Core
{
    public sealed class NormalizedSystemResources
    {
        private readonly Dictionary<string, long> amounts;

        public NormalizedSystemResources(ConclusionSubject system, decimal distanceFromBirthLy,
            IEnumerable<KeyValuePair<string, long>> finiteAmounts)
        {
            System = system ?? throw new ArgumentNullException(nameof(system));
            if (system.Kind != SubjectKind.BirthSystem && system.Kind != SubjectKind.StarSystem)
                throw new ArgumentException("A resource system requires a system identity.", nameof(system));
            if (distanceFromBirthLy < 0) throw new ArgumentOutOfRangeException(nameof(distanceFromBirthLy));
            amounts = finiteAmounts.ToDictionary(pair => pair.Key, pair => pair.Value, StringComparer.Ordinal);
            if (amounts.Any(pair => String.IsNullOrWhiteSpace(pair.Key) || pair.Value < 0 || pair.Key == "oil"))
                throw new ArgumentException("Finite resource totals must be named, nonnegative deposits.", nameof(finiteAmounts));
            DistanceFromBirthLy = distanceFromBirthLy;
        }

        public ConclusionSubject System { get; }
        public decimal DistanceFromBirthLy { get; }
        public IReadOnlyDictionary<string, long> FiniteAmounts =>
            new global::System.Collections.ObjectModel.ReadOnlyDictionary<string, long>(amounts);
        public long Amount(string resourceId) => amounts.TryGetValue(resourceId, out long value) ? value : 0;
    }
}
