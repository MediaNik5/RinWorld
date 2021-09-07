using System;
using RinWorld.Util.Data;
using RinWorld.Util.Unity;
using UnityEngine.Tilemaps;

namespace RinWorld.World
{
    public class ReliefPreset : IComparable<ReliefPreset>
    {
        private readonly float _minHeight;
        public readonly string name;
        public readonly ImmutableTile tile;

        public ReliefPreset(ImmutableTile tile, string name, float minHeight)
        {
            this.tile = tile;
            this.name = name;
            _minHeight = minHeight;
        }

        public int CompareTo(ReliefPreset other)
        {
            if (this == other) return 0;
            if (other == null) return 1;

            return _minHeight.CompareTo(other._minHeight);
        }

        private bool Match(float height)
        {
            return height >= _minHeight;
        }

        public static ReliefPreset For(float height)
        {
            ReliefPreset reliefToReturn = null;
            foreach (var relief in DataHolder.GetAllReliefs())
                if (relief.Match(height))
                    if (relief.CompareTo(reliefToReturn) > 0)
                        reliefToReturn = relief;
            return reliefToReturn;
        }
    }
}