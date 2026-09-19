using System;

namespace DSPSeedScanner.Runtime
{
    public static class PreviewSphereGeometry
    {
        public static long MaximumRadiusMeters(float dysonRadius)
        {
            float defaultRadius = (float)((double)dysonRadius * 40_000.0);
            float maximum = defaultRadius * 2f;
            return checked((long)((float)Math.Round(maximum / 100f) * 100f));
        }

        public static bool ContainsOrbit(float orbitRadius, float? parentOrbitRadius,
            long maximumRadiusMeters) =>
            ((double)orbitRadius + (parentOrbitRadius ?? 0f)) * 40_000.0 <= maximumRadiusMeters;
    }
}
