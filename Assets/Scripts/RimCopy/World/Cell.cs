using System.Collections.Generic;
using RimCopy.Attribute;
using RimCopy.Colony;
using RimCopy.Data;
using RimCopy.IO;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RimCopy.World
{
    public class Cell : Unit, IRenderable
    {
        [DontSave] public readonly int x;
        [DontSave] public readonly int y;
        [DontSave] private readonly float _height;
        [DontSave] private readonly float _moisture;
        [DontSave] private readonly float _heat;
        [DontSave] private readonly BiomePreset _biome;
        [DontSave] private readonly ReliefPreset _relief;
        [DontSave] private ColonyInfo _colony;
        private readonly float meanTemperature;
        private readonly float maxTemperature;
        private readonly float minTemperature;

        public bool Changed { get; } = false;

        private Cell(int x, int y, float height, float moisture, float heat)
        {
            this.x = x;
            this.y = y;
            _height = Mathf.Clamp(height, 0f, 1f);
            _moisture = Mathf.Clamp(moisture, 0f, 1f);
            _heat = Mathf.Clamp(heat, 0f, 1f);
            meanTemperature = MeanTemperature();
            var deltaTemperature = DeltaTemperature();
            maxTemperature = meanTemperature + deltaTemperature;
            minTemperature = meanTemperature - deltaTemperature;
            
            _biome = BiomePreset.For(_height, _moisture, _heat);
            _relief = ReliefPreset.For(_height);
            _colony = DataHolder.GetColony(x, y);
        }

        public static Cell Of(int x, int y, float height, float moisture, float heat)
        {
            return new Cell(x, y, height, moisture, heat);
        }


        public void StartRender(Tilemap[] tilemaps)
        {
            tilemaps[0].SetTile(new Vector3Int(x, y, 0), _biome.Tile);
            tilemaps[1].SetTile(new Vector3Int(x, y, 0), _relief.Tile);
        }

        public void Render(Tilemap[] tilemaps)
        {
        }


        private static readonly float MaxMeanTemperature;
        private static readonly float MinMeanTemperature;
        private static readonly float Linear;
        private static readonly float Shift;
        private static readonly float HeightShift;
        private static readonly float MaxMoistureAddition;
        private static readonly float MoistureShift;
        private static readonly float MoistureMultiplier;
        private static readonly float MoistureExponent;

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


        /**
         * Range [-1, 1]
         */
        private static float Distribution(float x)
        {
            return 2 * x - 1f;
        }

        /**
         * Mean temperature has its calculation process:
         * The formula is Mean = Linear*Distribution(heat) + Shift + MoistureShift*moisture + HeightShift*height.
         * Where Linear, Shift, MoistureShift, HeightShift are constants, and
         * MoistureShift, HeightShift values loaded from /core/conditions/cell_conditions.json.
         * Distribution(x) is mean temperature scatter, [-1, 1].
         * Linear and Shift change f(x)'s range [-1, 1] to range [MinMeanTemperature, MaxMeanTemperature].
         * MoistureShift*moisture + HeightShift*height are affects of moisture and height on mean temperature.
         */
        private float MeanTemperature()
        {
            return Linear * Distribution(_heat) + Shift + MoistureShift * _moisture + HeightShift * _height;
        }

        /**
         * Delta temperature has its calculation process. It depends only on moisture and constants.
         * 
         * Max temperature is mean + delta. Min is mean - delta.
         */
        private float DeltaTemperature()
        {
            return MaxMoistureAddition / (1 + MoistureMultiplier * Mathf.Pow(_moisture, MoistureExponent));
        }

        public IEnumerable<(string, string)> GetInfo()
        {
            yield return ("Biome", _biome.name);
            yield return ("Relief", _relief.name);
            yield return ("Max temperature", maxTemperature.ToString("F1"));
            yield return ("Mean temperature", meanTemperature.ToString("F1"));
            yield return ("Min temperature", minTemperature.ToString("F1"));
            yield return ("Average moisture", (int) (50 * (1 + Distribution(_moisture))) + "%");
#if UNITY_EDITOR
            yield return ("Heat value", _heat.ToString("F1"));
            yield return ("Moisture value", _moisture.ToString("F1"));
            yield return ("Height value", _height.ToString("F1"));
#endif
        }
    }
}