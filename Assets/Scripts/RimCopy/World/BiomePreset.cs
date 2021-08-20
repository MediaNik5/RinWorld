using System;
using RimCopy.Data;
using UnityEngine.Tilemaps;

namespace RimCopy.World
{
    public class BiomePreset : IComparable<BiomePreset>
    {
        public readonly Tile Tile;
        public readonly string name;
        private readonly float _minHeight;
        private readonly float _minMoisture;
        private readonly float _minHeat;

        // Reflection
        private BiomePreset()
        {
        }


        internal BiomePreset(Tile tile, string name, float minHeight, float minMoisture, float minHeat)
        {
            Tile = tile;
            this.name = name;
            _minHeight = minHeight;
            _minMoisture = minMoisture;
            _minHeat = minHeat;
        }

        public bool MatchCondition(float height, float moisture, float heat)
        {
            return height >= _minHeight && moisture >= _minMoisture && heat >= _minHeat;
        }

        public int CompareTo(BiomePreset other)
        {
            if (this == other) return 0;
            if (other == null) return 1;

            return _minHeight - other._minHeight +
                (_minMoisture - other._minMoisture) +
                (_minHeat - other._minHeat) > 0
                    ? 1
                    : -1;
        }

        public static BiomePreset For(float height, float moisture, float heat)
        {
            BiomePreset biomeToReturn = null;
            foreach (var biome in DataHolder.GetAllBiomes())
                if (biome.MatchCondition(height, moisture, heat))
                    if (biome.CompareTo(biomeToReturn) > 0)
                        biomeToReturn = biome;

            return biomeToReturn;
        }
    }
}