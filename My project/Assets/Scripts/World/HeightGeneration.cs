using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeightGeneration : MonoBehaviour
{
    [SerializeField] private Shader vertShader;
    [SerializeField] private int size;
    [SerializeField] private int biomeCount;
    [SerializeField] private float scale;
    [SerializeField] private BiomeSettings biomeSettings;
    [SerializeField] private int blendRadius;
    [SerializeField] private bool useTexture;
    [SerializeField] private float waterHeight;
    [SerializeField] private float waterScale;
    [SerializeField] private float waterAmplitude;

    [Header("Chunks")]
    [SerializeField] private int chunkSize;
    private int chunkCount;

    private List<Biome> biomes;
    private Biome[,] cellBiomes;

    private float[,] finalNoise;

    [Button("Generate biomes")]
    private void CreateBiomes()
    {
        biomes = new List<Biome>();
        cellBiomes = new Biome[size, size];
        finalNoise = new float[size, size];

        chunkCount = size / chunkSize;

        for (int i = 0; i < biomeCount; i++)
        {
            int tries = 0;

            BiomeData data = biomeSettings.GetWeightedRandom();

            while (tries < 200)
            {
                tries += 1;

                int x = Random.Range(0, size);
                int y = Random.Range(0, size);

                bool toClose = false;
                foreach (Biome b in biomes)
                {
                    float dist = b.DistanceToPoint(new Vector2Int(x, y));
                    if (dist < b.data.minimumDistanceToOtherBiome + data.minimumDistanceToOtherBiome)
                    {
                        toClose = true;
                        break;
                    }
                }

                if (toClose)
                {
                    continue;
                }

                biomes.Add(new Biome(new Vector2Int(x, y), data));
                break;
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float minDist = float.MaxValue;
                int minIndex = -1;
                int index = 0;
                foreach (Biome b in biomes)
                {
                    float dist = b.DistanceToPoint(new Vector2Int(x, y));
                    if (dist < minDist)
                    {
                        minDist = dist;
                        minIndex = index;
                    }

                    index += 1;
                }

                finalNoise[x, y] = biomes[minIndex].GetHeight(new Vector2Int(x, y));
                cellBiomes[x, y] = biomes[minIndex];
            }
        }

        List<Vector2Int> offsets = new List<Vector2Int>();
        for (int x = -blendRadius; x < blendRadius + 1; x++)
        {
            for (int y = -blendRadius; y < blendRadius + 1; y++)
            {
                float dist = Vector2.Distance(new Vector2(x, y), Vector2.zero);
                if (dist < blendRadius)
                {
                    offsets.Add(new Vector2Int(x, y));
                }
            }
        }

        float[,] weighted = new float[size, size];
        float max = float.MinValue;

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                float total = 0;
                int samples = 0;
                foreach (Vector2Int offset in offsets)
                {
                    int newX = x + offset.x;
                    int newY = y + offset.y;
                    if (newX < 0 || newY < 0 || newX > size - 1 || newY > size - 1) continue;

                    float height = cellBiomes[x + offset.x, y + offset.y].GetHeight(new Vector2Int(x + offset.x, y + offset.y));
                    total += height;
                    samples += 1;
                }

                float mediumHeight = total / samples;
                weighted[x, y] = mediumHeight;

                max = Mathf.Max(mediumHeight, max);
            }
        }

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                finalNoise[x, y] = weighted[x, y];
            }
        }

        GameObject wrapper = new GameObject();
        wrapper.transform.SetParent(transform);
        for(int x = 0; x < chunkCount; x ++)
        {
            for(int y = 0; y < chunkCount; y ++)
            {
                CreateMeshObject(wrapper.transform, x, y);
            }
        }
    }

    private void CreateMeshObject(Transform t, int chunkX,int chunkY)
    {
        GameObject temp = GameObject.CreatePrimitive(PrimitiveType.Quad);
        temp.GetComponentInChildren<MeshRenderer>().sharedMaterial.shader = vertShader;
        temp.transform.SetParent(t);

        List<Vector3> verts = new List<Vector3>();
        List<int> tris = new List<int>();
        List<Color> vertColors = new List<Color>();
        int triangleIndex = 0;
        for (int x = chunkX * chunkSize; x < chunkX * chunkSize + chunkSize; x ++)
        {
            for (int y = chunkY * chunkSize; y < chunkY * chunkSize + chunkSize; y++)
            {
                float noiseHeight = finalNoise[x, y];
                if (noiseHeight < waterHeight)
                {
                    noiseHeight = waterHeight + Mathf.PerlinNoise(x * waterScale, y * waterScale) * waterAmplitude;
                }

                verts.Add(new Vector3(x, noiseHeight, y));
                verts.Add(new Vector3(x + 1, noiseHeight, y));
                verts.Add(new Vector3(x, noiseHeight, y + 1));
                verts.Add(new Vector3(x + 1, noiseHeight, y + 1));

                vertColors.Add(cellBiomes[x, y].GetColor());
                vertColors.Add(cellBiomes[x, y].GetColor());
                vertColors.Add(cellBiomes[x, y].GetColor());
                vertColors.Add(cellBiomes[x, y].GetColor());

                tris.Add(triangleIndex);
                tris.Add(triangleIndex + 3);
                tris.Add(triangleIndex + 1);

                tris.Add(triangleIndex);
                tris.Add(triangleIndex + 2);
                tris.Add(triangleIndex + 3);

                triangleIndex += 4;
            }
        }

        Mesh mesh = new Mesh();
        mesh.vertices = verts.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.colors = vertColors.ToArray();

        mesh.RecalculateNormals();

        temp.GetComponent<MeshFilter>().mesh = mesh;
    }
}
