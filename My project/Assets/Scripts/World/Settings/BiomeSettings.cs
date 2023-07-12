using UnityEngine;

[CreateAssetMenu(menuName ="Custom/Map/BiomeSettings")]
public class BiomeSettings : ScriptableObject
{
    [SerializeField] private BiomeData[] biomes;

    public BiomeData GetWeightedRandom()
    {
        return biomes.GetWeighted();
    }
}