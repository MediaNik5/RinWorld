using System;
using RinWorld.Buildings;
using RinWorld.Things;
using RinWorld.Util.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RinWorld.World
{
    public class MapCell : Unit, IRenderable, IEquatable<MapCell>
    {
        private readonly int x;
        private readonly int y;
        private readonly Vector3Int _position;
        private readonly BiomePreset _biome;
        private readonly float _height;
        private readonly float _worth;
        private readonly float _presence;

        private readonly Building _building;
        private readonly Floor _floor;
        private Thing _thing;

        private readonly bool changed = false;

        public MapCell(int x, int y, BiomePreset biome, float height, float worth, float presence)
        {
            this.x = x;
            this.y = y;
            _position = new Vector3Int(x, y, 0);
            _biome = biome;
            _height = Mathf.Clamp01(height);
            _worth = Mathf.Clamp01(worth);
            _presence = Mathf.Clamp01(presence);

            var pointPreset = biome.MapCellPresetFor(_height, _worth, _presence);
            _building = pointPreset.Building;
            _floor = pointPreset.Floor;
        }

        public static MapCell Of(int x, int y, BiomePreset biome, float height, float worth, float presence)
        {
            return new MapCell(x, y, biome, height, worth, presence);
        }

        public void StartRender(Tilemap[] tilemaps)
        {
            _floor?.Tile.ApplyFor(tilemaps[Floor.Layer], _position);
            _building?.Tile.ApplyFor(tilemaps[Building.Layer], _position);
        }

        public void Render(Tilemap[] tilemaps)
        {
            if (!changed)
                return;

            _building?.Tile.ApplyFor(tilemaps[Building.Layer], _position);
            _floor?.Tile.ApplyFor(tilemaps[Floor.Layer], _position);
            _thing?.tile.ApplyFor(tilemaps[Thing.Layer], _position);
        }

        public bool Equals(MapCell other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return x == other.x && y == other.y;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((MapCell) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (x * 397) ^ y;
            }
        }
    }
}