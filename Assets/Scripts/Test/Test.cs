using System.IO;
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
    }
}