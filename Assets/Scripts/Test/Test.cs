using System.Collections.Generic;
using System.IO;
using System.Text;
using RinWorld.Util.IO;
using RinWorld.World;
using RinWorld.World.Generator;
using UnityEngine;

namespace Test
{
    public class Test : MonoBehaviour
    {
        public void Start()
        {
            var waves = new Wave[2];

            waves[0] = new Wave(12.3f, 0.01f, 0.9f);
            waves[1] = new Wave(123.4f, 0.01f, 0.1f);
            NoiseSave(waves, "low_frequency");

            waves[0] = new Wave(12.3f, 0.9f, 0.9f);
            waves[1] = new Wave(123.4f, 0.9f, 0.1f);
            NoiseSave(waves, "high_frequency");
        }

        private static void NoiseSave(Wave[] waves, string name)
        {
            var noise = Noise.Generate(250, 250, waves);

            var texture = new Texture2D(250, 250, TextureFormat.ARGB32, false);
            for (var i = 0; i < 250; i++)
            for (var j = 0; j < 250; j++)
            {
                var color = Mathf.Clamp(noise[i, j], 0, 1);
                texture.SetPixel(i, j, new Color(color, color, color, 1));
            }

            texture.Apply();

            File.WriteAllBytes(Application.dataPath + $"/{name}.png", texture.EncodeToPNG());
        }
        
        private const float BiomeStep = 0.01f;
        internal static void BiomeCoverage()
        {
            var ranges = new List<Range>();
            for (float height = 0; height < 1f; height += BiomeStep)
                for (float moisture = 0; moisture < 1f; moisture += BiomeStep)
                    for (float heat = 0; heat < 1f; heat += BiomeStep)
                        CheckBiomeFor(height, moisture, heat, ranges);

            PrintMissingBiomeRangesIfMissing(ranges);
        }

        private static void CheckBiomeFor(float height, float moisture, float heat, List<Range> ranges)
        {
            var biome = BiomePreset.For(height, moisture, heat);
            if (biome == null)
            {
                foreach (var range in ranges)
                    if (range.CheckAndAdd(height, moisture, heat))
                        return; 
                ranges.Add(new Range(height, moisture, heat, BiomeStep, BiomeStep / 10f));
            }
        }

        private static void PrintMissingBiomeRangesIfMissing(List<Range> ranges)
        {
            if (ranges.Count == 0)
                return;

            var sb = new StringBuilder("(height, moisture, heat)\n");
            foreach (var range in ranges)
                sb.Append(range).Append('\n');

            Files.WriteContentsGameFolder("/MissingBiomes.txt", sb.ToString());
            Debug.LogError(
                "Missing biomes for some ranges. This indicates that configs in /mods/.../biomes/biomes.json are corrupted." +
                "\nRanges are available here: " + Application.dataPath + "/MissingBiomes.txt");
        }

        private class Range
        {
            private float from1;
            private float to1;

            private float from2;
            private float to2;

            private float from3;
            private float to3;

            private readonly float diff;

            public Range(float initial1, float initial2, float initial3, float step, float epsilon)
            {
                from1 = to1 = initial1;
                from2 = to2 = initial2;
                from3 = to3 = initial3;
                diff = step + epsilon;
            }

            public bool CheckAndAdd(float value1, float value2, float value3)
            {
                if (from1 <= value1 && value1 <= to1 &&
                    from2 <= value2 && value2 <= to2 &&
                    from3 <= value3 && value3 <= to3)
                    return true;
                if (from2 <= value2 && value2 <= to2 &&
                    from3 <= value3 && value3 <= to3)
                {
                    if (0 < from1 - value1 && from1 - value1 < diff)
                    {
                        from1 = value1;
                        return true;
                    }

                    if (0 < value1 - to1 && value1 - to1 < diff)
                    {
                        to1 = value1;
                        return true;
                    }
                }

                if (from1 <= value1 && value1 <= to1 &&
                    from3 <= value3 && value3 <= to3)
                {
                    if (0 < from2 - value2 && from2 - value2 < diff)
                    {
                        from2 = value2;
                        return true;
                    }

                    if (0 < value2 - to2 && value2 - to2 < diff)
                    {
                        to2 = value2;
                        return true;
                    }
                }

                if (from1 <= value1 && value1 <= to1 &&
                    from2 <= value2 && value2 <= to2)
                {
                    if (0 < from3 - value3 && from3 - value3 < diff)
                    {
                        from3 = value3;
                        return true;
                    }

                    if (0 < value3 - to3 && value3 - to3 < diff)
                    {
                        to3 = value3;
                        return true;
                    }
                }

                return false;
            }

            public override string ToString()
            {
                return $"({from1}-{to1}, {from2}-{to2}, {from3}-{to3})";
            }
        }
    }
}