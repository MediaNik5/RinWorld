using RinWorld.Util.IO;
using UnityEngine.Tilemaps;

namespace RinWorld.World
{
    public class Colony : Unit, IRenderable
    {
        public readonly int x;
        public readonly int y;

        private readonly float _heat;
        private readonly float _moisture;
        private readonly float _height;

        private readonly Map _map;

        public Colony(int x, int y, BiomePreset biome, float heat, float moisture, float height)
        {
            this.x = x;
            this.y = y;
            _heat = heat;
            _moisture = moisture;
            _height = height;

            _map = new Map(150, 150, biome, heat, moisture, height);
        }

        public override void Initialize()
        {
            _map.Initialize();
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