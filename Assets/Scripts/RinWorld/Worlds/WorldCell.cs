using System;
using System.Collections.Generic;
using RinWorld.Util;
using RinWorld.Util.Attribute;
using RinWorld.Util.Data;
using RinWorld.Util.Unity;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RinWorld.Worlds
{
    public class WorldCell : IRenderable, IEquatable<WorldCell>
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

        [DontSave] private readonly float _maxTemperature;
        [DontSave] private readonly float _meanTemperature;
        [DontSave] private readonly float _minTemperature;

        [DontSave] public readonly int x;
        [DontSave] public readonly int y;
        [DontSave] private Colony _colony;

        static WorldCell()
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

        private WorldCell(int x, int y, float height, float moisture, float heat, Colony colony)
        // :base($"cell_{x}_{y}")
        {
            this.x = x;
            this.y = y;
            _height = Mathf.Clamp(height, 0f, 1f);
            _moisture = Mathf.Clamp(moisture, 0f, 1f);
            _heat = Mathf.Clamp(heat, 0f, 1f);
            _meanTemperature = MathHelper.MeanTemperature(heat, moisture, height);
            var deltaTemperature = MathHelper.DeltaTemperature(moisture);
            _maxTemperature = _meanTemperature + deltaTemperature;
            _minTemperature = _meanTemperature - deltaTemperature;

            _biome = BiomePreset.For(_height, _moisture, _heat);
            _relief = ReliefPreset.For(_height);
            _colony = colony;
        }

        public bool Changed { get; } = false;


        public void StartRender(Tilemap[] tilemaps)
        {
            tilemaps[0].SetTile(x, y, _biome.tile);
            tilemaps[1].SetTile(x, y, _relief.tile);
        }

        public void Render(Tilemap[] tilemaps)
        {
        }

        public static WorldCell Of(int x, int y, float height, float moisture, float heat, Colony colony)
        {
            return new WorldCell(x, y, height, moisture, heat, colony);
        }

        public Colony GenerateColony()
        {
            _colony = new Colony(x, y, _biome, _heat, _moisture, _height);
            return _colony;
        }

        public IEnumerable<(string, string)> GetInfo()
        {
            yield return ("Biome", _biome.name);
            yield return ("Relief", _relief.name);
            yield return ("Max temperature", _maxTemperature.ToString("F1"));
            yield return ("Mean temperature", _meanTemperature.ToString("F1"));
            yield return ("Min temperature", _minTemperature.ToString("F1"));
            yield return ("Average moisture", (int) (50 * (1 + MathHelper.Distribution(_moisture))) + "%");
#if UNITY_EDITOR
            yield return ("Heat value", _heat.ToString("F1"));
            yield return ("Moisture value", _moisture.ToString("F1"));
            yield return ("Height value", _height.ToString("F1"));
#endif
        }
        

        public bool Equals(WorldCell other)
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
            return Equals((WorldCell) obj);
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