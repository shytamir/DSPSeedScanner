using UnityEngine;

// Compile-only member shapes for a hosted runner that cannot possess DSP.
// This assembly contains no generation implementation and must never be
// packaged or used for runtime validation; local builds use the installed game.

public struct Version
{
    public string ToFullString() => string.Empty;
}

public static class GameConfig
{
    public static Version gameVersion => default;
}

public sealed class GameDesc
{
    public int[] savedThemeIds = System.Array.Empty<int>();
    public Version creationVersion;
    public int galaxySeed;
    public int starCount;
    public float resourceMultiplier;
    public bool isPeaceMode;
    public CombatSettings combatSettings;

    public void SetForNewGame(
        int algorithm,
        int seed,
        int starCount,
        int playerPrototype,
        float resourceMultiplier)
    {
    }
}

public class UIGalaxySelect
{
    public void SetStarmapGalaxy() { }
}

public struct CombatSettings
{
    public float initialColonize;
    public float maxDensity;
}

public static class UniverseGen
{
    public static int algoVersion;
    public static GalaxyData CreateGalaxy(GameDesc descriptor) => new GalaxyData();
}

public sealed class GalaxyData
{
    public const double LY = 2_400_000d;
    public int starCount;
    public StarData[] stars = System.Array.Empty<StarData>();
    public int birthPlanetId;
    public int birthStarId;

    public void Free() { }
    public StarData StarById(int id) => new StarData();
    public PlanetData PlanetById(int id) => new PlanetData();
}

public sealed class StarData
{
    public int id;
    public string displayName => string.Empty;
    public string typeString => string.Empty;
    public VectorLF3 uPosition;
    public float dysonRadius;
    public PlanetData[] planets = System.Array.Empty<PlanetData>();
    public int initialHiveCount;
    public float dysonLumino => 0f;
}

public sealed class PlanetData
{
    public int id;
    public int number;
    public string displayName => string.Empty;
    public int orbitAround;
    public PlanetData? orbitAroundPlanet;
    public float orbitRadius;
    public EPlanetSingularity singularity;
    public float luminosity;
    public float windStrength;
    public int[]? gasItems;
    public float[]? gasSpeeds;
    public EPlanetType type;
    public int theme;
    public int iceFlag;
    public int algoId;
    public int precision;
    public double mod_x;
    public double mod_y;
    public byte[]? modData;
    public PlanetRawData? data;
    public PlanetAuxData? aux;
    public VeinGroup[]? veinGroups;

    public void RegenerateRawDataImmediately() { }
    public void SummarizeVeinGroups() { }
}

public sealed class GameData
{
    public GameDesc? gameDesc;
    public GalaxyData? galaxy;
    public PlanetFactory[]? factories;
    public int factoryCount;
    public GameHistoryData? history;
    public GameStatData? statistics;
}

public sealed class PlanetFactory { }
public sealed class GameHistoryData { }
public sealed class GameStatData { }

public static class GameMain
{
    public static GameData? data;
}

public static class DSPGame
{
    public static GameDesc? GameDesc;
}

public static class LDB
{
    public static ThemeProtoSet themes => new ThemeProtoSet();
}

public sealed class ThemeProtoSet
{
    public int Length => dataArray.Length;
    public ThemeProto[] dataArray = System.Array.Empty<ThemeProto>();
}

public sealed class ThemeProto
{
    public int ID;
    public string displayName => string.Empty;
    public EPlanetType PlanetType;
    public int[]? Algos;
}

public enum EPlanetType
{
    None = 0,
    Vocano = 1,
    Ocean = 2,
    Desert = 3,
    Ice = 4,
    Gas = 5
}

[System.Flags]
public enum EPlanetSingularity
{
    None = 0,
    TidalLocked = 1,
    TidalLocked2 = 2,
    TidalLocked4 = 4,
    LaySide = 8,
    ClockwiseRotate = 16,
    MultipleSatellites = 32
}

public enum EVeinType : byte
{
    None = 0,
    Iron = 1,
    Copper = 2,
    Silicium = 3,
    Titanium = 4,
    Stone = 5,
    Coal = 6,
    Oil = 7,
    Fireice = 8,
    Diamond = 9,
    Fractal = 10,
    Crysrub = 11,
    Grat = 12,
    Bamboo = 13,
    Mag = 14,
    Max = 15
}

public sealed class PlanetRawData
{
    public PlanetRawData(int precision) { }

    public VeinData[] veinPool = System.Array.Empty<VeinData>();
    public int veinCursor;
    public byte[] InitModData(byte[]? modData) => System.Array.Empty<byte>();
    public void CalcVerts() { }
    public void Free() { }
}

public sealed class PlanetAuxData
{
    public PlanetAuxData(PlanetData planet) { }

    public void Free() { }
}

public class PlanetAlgorithm
{
    public void GenerateTerrain(double modX, double modY) { }
    public void GenerateVeins() { }
}

public struct VeinData
{
    public int id;
    public EVeinType type;
    public short groupIndex;
    public int amount;
    public int productId;
    public Vector3 pos;
    public static float oilSpeedMultiplier;
}

public struct VeinGroup
{
    public EVeinType type;
    public Vector3 pos;
    public int count;
    public long amount;
}

public static class RandomTable
{
    public static void Init() { }
}

public static class PlanetModelingManager
{
    public static PlanetAlgorithm Algorithm(PlanetData planet) => new PlanetAlgorithm();
}

public struct VectorLF3
{
    public double x;
    public double y;
    public double z;
}
