using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Biome")]
public class BiomeData : ScriptableObject
{
    public float noiseScale;
    public float noiseAmplitude;

    public float minDist;

    public float swiwelScale;
    public float swiwelAmplitude;
}