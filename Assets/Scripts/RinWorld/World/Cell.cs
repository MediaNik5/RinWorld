using System;
using System.Collections.Generic;
using RinWorld.Util.Attribute;
using RinWorld.Util.Data;
using RinWorld.Util.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RinWorld.World
{
    public class Cell : Unit, IRenderable
    {
        private static readonly float MaxMeanTemperature;
        private static readonly float MinMeanTemperature;
        private static readonly float Linear;
        private static readonly float Shift;
        private static readonly float HeightShift;
        private static readonly float MaxMoistureAddition;
        private static readonly float MoistureShift;
        private static readonly float MoistureMultiplier;
        private static readonly float MoistureExponent;

        [DontSave] private readonly BiomePreset _biome;
        [DontSave] private readonly float _heat;
        [DontSave] private readonly float _height;
        [DontSave] private readonly float _moisture;
        [DontSave] private readonly ReliefPreset _relief;

        [DontSave] private readonly float maxTemperature;
        [DontSave] private readonly float meanTemperature;
        [DontSave] private readonly float minTemperature;

        [DontSave] public readonly int x;
        [DontSave] public readonly int y;
        [DontSave] private Colony _colony;

        static Cell()
        {
            MaxMeanTemperature = DataHolder.GetCondition(nameof(MaxMeanTemperature));
            MinMeanTemperature = DataHolder.GetCondition(nameof(MinMeanTemperature));
            Linear = (MaxMeanTemperature - MinMeanTemperature) / 2;
            Shift = (MaxMeanTemperature + MinMeanTemperature) / 2;
            HeightShift = DataHolder.GetCondition(nameof(HeightShift));
            MoistureShift = DataHolder.GetCondition(nameof(MoistureShift));
            MaxMoistureAddition = DataHolder.GetCondition(nameof(MaxMoistureAddition));
            MoistureMultiplier = DataHolder.GetCondition(nameof(MoistureMultiplier));
            MoistureExponent = DataHolder.GetCondition(nameof(MoistureExponent));
        }

        private Cell(int x, int y, float height, float moisture, float heat, Colony colony)
        {
            this.x = x;
            this.y = y;
            _height = Mathf.Clamp(height, 0f, 1f);
            _moisture = Mathf.Clamp(moisture, 0f, 1f);
            _heat = Mathf.Clamp(heat, 0f, 1f);
            meanTemperature = BiomePreset.MeanTemperature(heat, moisture, height);
            var deltaTemperature = BiomePreset.DeltaTemperature(moisture);
            maxTemperature = meanTemperature + deltaTemperature;
            minTemperature = meanTemperature - deltaTemperature;

            _biome = BiomePreset.For(_height, _moisture, _heat);
            _relief = ReliefPreset.For(_height);
            _colony = colony;
        }

        public bool Changed { get; } = false;


        public void StartRender(Tilemap[] tilemaps)
        {
            try
            {
                tilemaps[0].SetTile(new Vector3Int(x, y, 0), _biome.tile);
                tilemaps[1].SetTile(new Vector3Int(x, y, 0), _relief.Tile);
            }
            catch (NullReferenceException ex)
            {
                BiomePreset.For(_height, _moisture, _heat);
                Debug.LogError("Exception!!!");
            }
        }

        public void Render(Tilemap[] tilemaps)
        {
        }

        public static Cell Of(int x, int y, float height, float moisture, float heat, Colony colony)
        {
            return new Cell(x, y, height, moisture, heat, colony);
        }

        public Colony GenerateColony()
        {
            _colony = new Colony(x, y, _biome, _heat, _moisture, _height);
            _colony.Initialize();
            return _colony;
        }

        public IEnumerable<(string, string)> GetInfo()
        {
            yield return ("Biome", _biome.name);
            yield return ("Relief", _relief.name);
            yield return ("Max temperature", maxTemperature.ToString("F1"));
            yield return ("Mean temperature", meanTemperature.ToString("F1"));
            yield return ("Min temperature", minTemperature.ToString("F1"));
            yield return ("Average moisture", (int) (50 * (1 + BiomePreset.Distribution(_moisture))) + "%");
#if UNITY_EDITOR
            yield return ("Heat value", _heat.ToString("F1"));
            yield return ("Moisture value", _moisture.ToString("F1"));
            yield return ("Height value", _height.ToString("F1"));
#endif
        }
    }
}