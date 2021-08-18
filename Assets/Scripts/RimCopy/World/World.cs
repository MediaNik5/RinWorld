using System;
using RimCopy.Attribute;
using RimCopy.World.Generator;
using UnityEngine.Tilemaps;

namespace RimCopy.World
{
    public class World : Unit, Renderable
    {
        [DontSave] private const int WaveNumber = 2;
        private readonly int _seed;
        private readonly int _width;
        private readonly int _height;

        [DontSave] private Cell[,] _cells;

        // Reflection
        private World()
        {
        }

        public World(int width, int height, int seed)
        {
            _width = width;
            _height = height;
            _seed = seed;
        }

        public override void Initialize()
        {
            GenerateWorld();
        }

        private void GenerateWorld()
        {
            var random = new Random(_seed);
            var heightWaves = GenerateWaves(random);
            var moistureWaves = GenerateWaves(random);
            var heatWaves = GenerateWaves(random);

            var heightMap = Noise.Generate(_width, _height, heightWaves);
            var moistureMap = Noise.Generate(_width, _height, moistureWaves);
            var heatMap = Noise.Generate(_width, _height, heatWaves);

            _cells = new Cell[_width, _height];
            for (var x = 0; x < _width; x++)
            for (var y = 0; y < _height; y++)
                _cells[x, y] = Cell.Of(x, y, heightMap[x, y], moistureMap[x, y], heatMap[x, y]);
        }

        private Wave[] GenerateWaves(Random random)
        {
            var waves = new Wave[WaveNumber];
            for (var i = 0; i < waves.Length; i++)
                waves[i] = new Wave(
                    random.Next(5000), //5000 is magic number
                    (float) (random.NextDouble() * 0.08), //0.08 is magic number
                    (float) random.NextDouble()
                );

            return waves;
        }

        public void StartRender(Tilemap[] tilemaps)
        {
            foreach (var cell in _cells) cell.StartRender(tilemaps);
        }

        public void Render(Tilemap[] tilemaps)
        {
        }
    }
}