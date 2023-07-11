using UnityEngine;

public class Biome
{
    public Vector2Int center { get; private set; }
    public BiomeData data { get; private set; }

    private Color color;

    public Biome(Vector2Int c, BiomeData d)
    {
        center = c;
        data = d;
        color = new Color(Random.value, Random.value, Random.value);
    }

    public float DistanceToPoint(Vector2Int pos)
    {
        return Vector2.Distance(GetSwiwele(center), GetSwiwele(pos));
    }

    public Vector2 GetSwiweleCenter()
    {
        return GetSwiwele(center);
    }

    public Vector2 GetSwiwele(Vector2 p)
    {
        float x = p.x + Mathf.PerlinNoise(p.x * data.swiwelScale, p.y * data.swiwelScale) * data.swiwelAmplitude;
        float y = p.y + Mathf.PerlinNoise((p.x + 1000) * data.swiwelScale, (p.y + 1000) * data.swiwelScale) * data.swiwelAmplitude;

        return new Vector2(x, y);
    }

    public Color GetColor()
    {
        return color;
    }

    public float GetHeight(Vector2Int pos)
    {
        return Mathf.PerlinNoise(pos.x * data.noiseScale, pos.y * data.noiseScale) * data.noiseAmplitude;
    }

    public Color GetHeightColor(Vector2Int pos)
    {
        float height = GetHeight(pos);
        return new Color(height / data.noiseAmplitude, height / data.noiseAmplitude, height / data.noiseAmplitude);
    }
}