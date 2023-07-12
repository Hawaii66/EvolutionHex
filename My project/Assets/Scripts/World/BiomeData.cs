using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Biome")]
public class BiomeData : ScriptableObject,SelectWeighted
{
    public float selectionRate;

    public float noiseScale;
    public float noiseAmplitude;

    public float minimumDistanceToOtherBiome;

    public float swiwelScale;
    public float swiwelAmplitude;

    public float GetSelectionRate()
    {
        return selectionRate;
    }
}

public interface SelectWeighted
{
    public float GetSelectionRate();
}