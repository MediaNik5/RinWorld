using System;
using UnityEngine;

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
            Debug.Log($"Wave with seed {seed}, frequency {frequency}, amplitude {amplitude}");
            Seed = seed;
            Frequency = frequency;
            Amplitude = amplitude;
        }
    }
}