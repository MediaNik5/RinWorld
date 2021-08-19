using System;

namespace RimCopy.World.Generator
{
    [Serializable]
    public class Wave
    {
        public readonly float Seed;
        public readonly float Frequency;
        public readonly float Amplitude;

        public Wave(float seed, float frequency, float amplitude)
        {
            Seed = seed;
            Frequency = frequency;
            Amplitude = amplitude;
        }
    }
}