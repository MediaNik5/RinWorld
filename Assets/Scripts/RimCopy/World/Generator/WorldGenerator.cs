using UnityEngine;
using UnityEngine.Tilemaps;

namespace RimCopy.World.Generator
{
    public class WorldGenerator : MonoBehaviour
    {
        public BiomePreset[] biomes;
        public Tilemap tilemap;

        [Header("Dimensions")] public int width = 50;
        public int height = 50;
        public float scale = 1.0f;
        public Vector2 offset;

        [Header("Height Map")] public Wave[] heightWaves;
        private float[,] heightMap;

        [Header("Moisture Map")] public Wave[] moistureWaves;
        private float[,] moistureMap;

        [Header("Heat Map")] public Wave[] heatWaves;
        private float[,] heatMap;

        // private void GenerateMap()
        // {
        //     // heightMap = Noise.Generate(width, height, scale, heightWaves, offset);
        //     // moistureMap = Noise.Generate(width, height, scale, moistureWaves, offset);
        //     // heatMap = Noise.Generate(width, height, scale, heatWaves, offset);
        //
        //     for (int x = 0; x < width; x++)
        //         for (int y = 0; y < height; y++)
        //             tilemap.SetTile(new Vector3Int(x, y, 0), GetBiome(heightMap[x, y], moistureMap[x, y], heatMap[x, y]).GetTile());
        // }

        // private BiomePreset GetBiome(float height, float moisture, float heat)
        // {
        //     HashSet<BiomePreset> biomeTemp = new HashSet<BiomePreset>();
        //     foreach(BiomePreset biome in biomes)
        //     {
        //         if(biome.MatchCondition(height, moisture, heat))
        //         {
        //             biomeTemp.Add(biome);
        //         }
        //     }
        //
        //     BiomePreset biomeToReturn = null;
        //     float curVal = 0.0f;
        //     foreach(BiomePreset biome in biomeTemp)
        //     {
        //         if(biomeToReturn == null)
        //         {
        //             biomeToReturn = biome;
        //             curVal = biome.DiffValue(height, moisture, heat);
        //         }
        //         else if(biome.DiffValue(height, moisture, heat) < curVal)
        //         {
        //             biomeToReturn = biome;
        //             curVal = biome.DiffValue(height, moisture, heat);
        //         }
        //         
        //     }
        //     if(biomeToReturn == null)
        //         biomeToReturn = biomes[0];
        //     
        //     return biomeToReturn;
        // }

        // void Start()
        // {
        //     GenerateMap();
        // }
    }
}