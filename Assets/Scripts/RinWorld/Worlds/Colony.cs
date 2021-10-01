using RinWorld.Util;
using RinWorld.Worlds.Maps;
using UnityEngine.Tilemaps;

namespace RinWorld.Worlds
{
    public class Colony : IRenderable
    {
        public readonly int x;
        public readonly int y;

        private readonly float _heat;
        private readonly float _moisture;
        private readonly float _height;

        private readonly Map _map;

        public Colony(int x, int y, BiomePreset biome, float heat, float moisture, float height)
        // :base($"Colony ({x}, {y})")
        {
            this.x = x;
            this.y = y;
            _heat = heat;
            _moisture = moisture;
            _height = height;

            _map = new Map(150, 150, biome, heat, moisture, height);
            _map.GenerateMap();
        }

        public void StartRender(Tilemap[] tilemaps)
        {
            _map.StartRender(tilemaps);
        }

        public void Render(Tilemap[] tilemaps)
        {
            _map.Render(tilemaps);
        }
    }
}