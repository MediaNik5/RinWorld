using System;
using RinWorld.Util;
using RinWorld.Worlds.Generator;
using UnityEngine.Tilemaps;

namespace RinWorld.Worlds.Maps
{
    public class Map : IRenderable
    {
        private readonly int _height;
        private readonly int _width;

        private readonly float _heatSeed;
        private readonly float _moistureSeed;
        private readonly float _heightSeed;

        private MapCell[,] _points;
        private readonly BiomePreset _biome;

        public Map(int height, int width, BiomePreset biome, float heatSeed, float moistureSeed, float heightSeed)
        // :base($"Map_{heatSeed}_{moistureSeed}_{heightSeed}")
        {
            _height = height;
            _width = width;
            _biome = biome;
            _heatSeed = heatSeed;
            _moistureSeed = moistureSeed;
            _heightSeed = heightSeed;
        }
        
        public void GenerateMap()
        {
            var random = new Random((int) (((_heatSeed * 31 + _moistureSeed) * 31 + _heightSeed) * 31));
            var heightWaves = GenerateWaves(random);
            var worthWaves = GenerateWaves(random);
            var presenceWaves = GenerateWaves(random);

            var heightMap = Noise.Generate(_width, _height, heightWaves);
            var worthMap = Noise.Generate(_width, _height, worthWaves);
            var presenceMap = Noise.Generate(_width, _height, presenceWaves);

            _points = new MapCell[_width, _height];
            for (var x = 0; x < _width; x++)
                for (var y = 0; y < _height; y++)
                    _points[x, y] = MapCell.Of(x, y, _biome, CorrectHeight(heightMap[x, y]), worthMap[x, y],
                        presenceMap[x, y]);
        }

        private float CorrectHeight(float height)
        {
            if (height == _heightSeed)
                return height;

            return (height + 2 * _heightSeed) / 3;
        }

        private Wave[] GenerateWaves(Random random)
        {
            var waves = new Wave[World.WaveNumber];
            for (var i = 0; i < waves.Length; i++)
                waves[i] = new Wave(
                    random.Next(5000), //5000 is magic number
                    0.01f, //0.01 is magic number
                    (float) random.NextDouble()
                );

            return waves;
        }

        public void StartRender(Tilemap[] tilemaps)
        {
            foreach (var point in _points)
                point.StartRender(tilemaps);
        }

        public void Render(Tilemap[] tilemaps)
        {
            foreach (var point in _points)
                point.Render(tilemaps);
        }
    }
}