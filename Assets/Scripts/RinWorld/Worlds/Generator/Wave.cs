using System;

namespace RinWorld.Worlds.Generator
{
    [Serializable]
    public class Wave
    {
        public readonly float Amplitude;
        public readonly float Frequency;
        public readonly float Seed;

        public Wave(float seed, float frequency, float amplitude)
        {
            Seed = seed;
            Frequency = frequency;
            Amplitude = amplitude;
        }
    }
}