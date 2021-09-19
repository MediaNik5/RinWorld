using System;
using System.Collections.Generic;
using RinWorld.Util.Attribute;
using RinWorld.Util.IO;
using RinWorld.World.Generator;
using UnityEngine.Tilemaps;

namespace RinWorld.World
{
    public class World : Unit, IRenderable
    {
        [DontSave] public const int WaveNumber = 2;
        private readonly int _height;
        private readonly int _width;
        private readonly int _seed;

        private static readonly Dictionary<(int, int), Colony> colonies = new Dictionary<(int, int), Colony>();

        [DontSave] private WorldCell[,] _cells;

        // Reflection
        private World()
        {
        }

        public World(int width, int height, int seed)
        {
            _width = width;
            _height = height;
            _seed = seed;
            _cells = new WorldCell[_width, _height];
        }

        public void StartRender(Tilemap[] tilemaps)
        {
            foreach (var cell in _cells)
                cell.StartRender(tilemaps);
        }

        public void Render(Tilemap[] tilemaps)
        {
        }

        public IEnumerable<(string, string)> GetCellInfo(int x, int y)
        {
            return _cells[x, y].GetInfo();
        }

        public Colony GenerateColony(int x, int y)
        {
            return _cells[x, y].GenerateColony();
        }


        public override void Initialize()
        {
            GenerateWorld();
        }

        private void GenerateWorld()
        {
            var random = new Random(_seed);
            var heightWaves = Noise.GenerateWaves(random, WaveNumber);
            var moistureWaves = Noise.GenerateWaves(random, WaveNumber);
            var heatWaves = Noise.GenerateWaves(random, WaveNumber);

            var heightMap = Noise.Generate(_width, _height, heightWaves);
            var moistureMap = Noise.Generate(_width, _height, moistureWaves);
            var heatMap = Noise.Generate(_width, _height, heatWaves);

            for (var x = 0; x < _width; x++)
                for (var y = 0; y < _height; y++)
                    _cells[x, y] = WorldCell.Of(x, y, heightMap[x, y], moistureMap[x, y], heatMap[x, y],
                        colonies.ContainsKey((x, y)) ? colonies[(x, y)] : null);
        }
    }
}