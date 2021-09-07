using System;
using System.Collections.Generic;
using System.Text;
using RinWorld.Util;
using RinWorld.Util.Data;
using RinWorld.Util.Unity;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RinWorld.World
{
    public class BiomePreset : IComparable<BiomePreset>
    {

        private readonly float _minHeat;
        private readonly float _minHeight;
        private readonly float _minMoisture;
        private readonly float _maxHeat;
        private readonly float _maxHeight;
        private readonly float _maxMoisture;
        public readonly string name;
        public readonly ImmutableTile tile;
        private readonly PointPreset[] _pointPresets;


        internal BiomePreset(Tile tile, string name, float minHeight, float minMoisture, float minHeat, float maxHeight,
            float maxMoisture, float maxHeat, PointPreset[] pointPresets)
        {
            this.tile = new ImmutableTile(tile);
            this.name = name;
            _minHeight = minHeight;
            _minMoisture = minMoisture;
            _minHeat = minHeat;

            _pointPresets = pointPresets;
            _maxHeat = maxHeat;
            _maxHeight = maxHeight;
            _maxMoisture = maxMoisture;
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

        public bool MatchCondition(float height, float moisture, float heat)
        {
            return _minHeight <= height && height <= _maxHeight &&
                   _minMoisture <= moisture && moisture <= _maxMoisture &&
                   _minHeat <= heat && heat <= _maxHeat;
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

        public PointPreset PointFor(float height, float worth, float presence)
        {
            PointPreset pointToReturn = null;
            foreach (var point in _pointPresets)
                if (point.MatchCondition(height, worth, presence))
                    if (point.CompareTo(pointToReturn) > 0)
                        pointToReturn = point;
            return pointToReturn;
        }

        public override string ToString()
        {
            return base.ToString() + $", name: {name}";
        }
    }
}