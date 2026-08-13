using System;
using System.Collections.Generic;
using System.Linq;
using DSPSeedScanner.Core;

namespace DSPSeedScanner.Runtime
{
    public sealed record RuntimePlanetOrbitEvidence
    {
        public RuntimePlanetOrbitEvidence(
            int planetId,
            string systemIdentifier,
            int planetNumber,
            bool isSolid,
            bool isGiant,
            int orbitAround,
            int? resolvedParentPlanetId)
        {
            if (planetId <= 0)
                throw new ArgumentOutOfRangeException(nameof(planetId));
            if (String.IsNullOrWhiteSpace(systemIdentifier))
                throw new ArgumentException("System identifier is required.", nameof(systemIdentifier));
            if (isSolid == isGiant)
                throw new ArgumentException("A planet must be either solid or giant.");

            PlanetId = planetId;
            SystemIdentifier = systemIdentifier;
            PlanetNumber = planetNumber;
            IsSolid = isSolid;
            IsGiant = isGiant;
            OrbitAround = orbitAround;
            ResolvedParentPlanetId = resolvedParentPlanetId;
        }

        public int PlanetId { get; }
        public string SystemIdentifier { get; }
        public int PlanetNumber { get; }
        public bool IsSolid { get; }
        public bool IsGiant { get; }
        public int OrbitAround { get; }
        public int? ResolvedParentPlanetId { get; }
    }

    public static class PreviewHomeTopologyNormalizer
    {
        public static NormalizedHomePlanetTopology? Normalize(
            string homeSystemIdentifier,
            int homePlanetId,
            IEnumerable<RuntimePlanetOrbitEvidence> planets)
        {
            if (String.IsNullOrWhiteSpace(homeSystemIdentifier))
                throw new ArgumentException("Home system identifier is required.", nameof(homeSystemIdentifier));
            if (homePlanetId <= 0)
                throw new ArgumentOutOfRangeException(nameof(homePlanetId));
            if (planets == null)
                throw new ArgumentNullException(nameof(planets));

            RuntimePlanetOrbitEvidence[] values = planets.ToArray();
            if (values.Select(value => value.PlanetId).Distinct().Count() != values.Length)
                return null;
            RuntimePlanetOrbitEvidence? home = values.SingleOrDefault(value =>
                value.PlanetId == homePlanetId);
            if (home == null || !home.IsSolid ||
                !String.Equals(home.SystemIdentifier, homeSystemIdentifier, StringComparison.Ordinal))
            {
                return null;
            }

            if (home.OrbitAround < 0)
                return null;
            if (home.OrbitAround == 0)
            {
                return home.ResolvedParentPlanetId.HasValue
                    ? null
                    : new NormalizedHomePlanetTopology(
                        home.PlanetId,
                        HomePlanetOrbitKind.DirectStar);
            }
            if (!home.ResolvedParentPlanetId.HasValue ||
                home.ResolvedParentPlanetId.Value <= 0)
                return null;

            RuntimePlanetOrbitEvidence? parent = values.SingleOrDefault(value =>
                value.PlanetId == home.ResolvedParentPlanetId.Value);
            if (parent == null || !parent.IsGiant || parent.PlanetNumber <= 0 ||
                !String.Equals(parent.SystemIdentifier, homeSystemIdentifier, StringComparison.Ordinal) ||
                home.OrbitAround != parent.PlanetNumber)
            {
                return null;
            }

            int moons = values.Count(value =>
                value.IsSolid &&
                String.Equals(value.SystemIdentifier, homeSystemIdentifier, StringComparison.Ordinal) &&
                value.OrbitAround == parent.PlanetNumber &&
                value.ResolvedParentPlanetId == parent.PlanetId);
            return moons < 1
                ? null
                : new NormalizedHomePlanetTopology(
                    home.PlanetId,
                    HomePlanetOrbitKind.GiantMoon,
                    moons);
        }
    }
}
