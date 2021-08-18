using UnityEngine;

namespace RimCopy.World.Generator
{
    public class Noise
    {
        public static float[,] Generate(int width, int height, Wave[] waves, float xOffset = 0f, float yOffset = 0f,
            float scale = 1f)
        {
            var noiseMap = new float[width, height];

            for (var x = 0; x < width; x++)
            for (var y = 0; y < height; y++)
                noiseMap[x, y] = GenerateSingle(x, y, waves, xOffset, yOffset, scale);

            return noiseMap;
        }

        public static float GenerateSingle(int x, int y, Wave[] waves, float xOffset = 0f, float yOffset = 0f,
            float scale = 1f)
        {
            var output = 0f;

            var finalX = x * scale + xOffset;
            var finalY = y * scale + yOffset;

            var normalization = 0f;

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
    }
}