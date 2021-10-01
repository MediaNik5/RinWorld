using System;
using RinWorld.Buildings;

namespace RinWorld.Worlds.Maps
{
    public class MapCellPreset : IComparable<MapCellPreset>
    {
        private readonly float _minHeight;
        private readonly float _minWorth;
        private readonly float _minPresence;
        public readonly string name;

        public readonly BuildingPreset BuildingPreset;
        public readonly FloorPreset FloorPreset;

        internal MapCellPreset(BuildingPreset buildingPreset, FloorPreset floorPreset, string name, float minHeight, float minWorth,
            float minPresence)
        {
            BuildingPreset = buildingPreset;
            FloorPreset = floorPreset;
            this.name = name;
            _minPresence = minPresence;
            _minWorth = minWorth;
            _minHeight = minHeight;
        }

        public int CompareTo(MapCellPreset other)
        {
            if (this == other) return 0;
            if (other == null) return 1;

            return _minHeight - other._minHeight +
                (_minWorth - other._minWorth) +
                (_minPresence - other._minPresence) > 0 ? 1 : -1;
        }

        public bool MatchCondition(float height, float worth, float presence)
        {
            return height >= _minHeight && worth >= _minWorth && presence >= _minPresence;
        }
    }
}