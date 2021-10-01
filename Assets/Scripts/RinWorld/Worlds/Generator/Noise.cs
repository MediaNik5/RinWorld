using UnityEngine;
using Random = System.Random;

namespace RinWorld.Worlds.Generator
{
    /**
     * Uses algorithm from this article
     * https://coding.degree/procedural-2d-maps-unity-tutorial/
     */
    public class Noise
    {
        public static float[,] Generate(
            int width, 
            int height, 
            Wave[] waves, 
            float xOffset = 0f, 
            float yOffset = 0f,
            float scale = 1f
        ) {
            float[,] noiseMap = new float[width, height];

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    noiseMap[x, y] = GenerateSingle(x, y, waves, xOffset, yOffset, scale);

            return noiseMap;
        }

        public static float GenerateSingle(
            int x, 
            int y, 
            Wave[] waves, 
            float xOffset = 0f, 
            float yOffset = 0f,
            float scale = 1f
        ) {
            float output = 0f;

            float finalX = x * scale + xOffset;
            float finalY = y * scale + yOffset;

            float normalization = 0f;

            foreach (var wave in waves)
            {
                // sample the perlin noise taking into consideration amplitude and frequency
                output += wave.Amplitude * Mathf.PerlinNoise(
                    finalX * wave.Frequency + wave.Seed,
                    finalY * wave.Frequency + wave.Seed
                );
                normalization += wave.Amplitude;
            }

            return output / normalization;
        }
        

        public static Wave[] GenerateWaves(Random random, int waveNumber, float frequencyMultiplier = 0.08f) //0.08 is magic number
        {
            var waves = new Wave[waveNumber];
            for (int i = 0; i < waves.Length; i++)
                waves[i] = new Wave(
                    random.Next(5000), //5000 is magic number
                    (float) (random.NextDouble() * frequencyMultiplier), 
                    (float) random.NextDouble()
                );

            return waves;
        }

        public static float[,] GenerateNormalized(
            int width,
            int height, 
            Wave[] waves, 
            float xOffset = 0f, 
            float yOffset = 0f, 
            float scale = 1f, 
            float normalizationValue = 1f
        )
        {
            float[,] noise = Generate(width, height, waves, xOffset, yOffset, scale);
            
            
            float value = 0f;
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    value += noise[x, y];
            if (value == 0 || Mathf.Abs(normalizationValue - value) < 0.01f)
                return noise;

            float multiplierToNormalize = normalizationValue / value;
            
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    noise[x, y] *= multiplierToNormalize;
            
            return noise;
        }
    }
}