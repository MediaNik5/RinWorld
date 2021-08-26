using System;
using System.Collections.Generic;
using System.Text;
using RinWorld.Util.Data;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace RinWorld.World
{
    public class BiomePreset : IComparable<BiomePreset>
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

        static BiomePreset()
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
        public static float Distribution(float x)
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
        public static float MeanTemperature(float heat, float moisture, float height)
        {
            return Linear * Distribution(heat) + Shift + MoistureShift * moisture + HeightShift * height;
        }

        /**
         * Delta temperature has its calculation process. It depends only on moisture and constants.
         * 
         * Max temperature is mean + delta. Min is mean - delta.
         */
        public static float DeltaTemperature(float moisture)
        {
            return MaxMoistureAddition / (1 + MoistureMultiplier * Mathf.Pow(moisture, MoistureExponent));
        }

        private readonly float _minHeat;
        private readonly float _minHeight;
        private readonly float _minMoisture;
        private readonly float _maxHeat;
        private readonly float _maxHeight;
        private readonly float _maxMoisture;
        public readonly string name;
        public readonly Tile tile;
        private readonly PointPreset[] _pointPresets;


        internal BiomePreset(Tile tile, string name, float minHeight, float minMoisture, float minHeat, float maxHeight,
            float maxMoisture, float maxHeat, PointPreset[] pointPresets)
        {
            this.tile = tile;
            this.name = name;
            _minHeight = minHeight;
            _minMoisture = minMoisture;
            _minHeat = minHeat;

            _pointPresets = pointPresets;
            _maxHeat = maxHeat;
            _maxHeight = maxHeight;
            _maxMoisture = maxMoisture;
        }

        public int CompareTo(BiomePreset other)
        {
            if (this == other) return 0;
            if (other == null) return 1;

            return _minHeight - other._minHeight +
                (_minMoisture - other._minMoisture) +
                (_minHeat - other._minHeat) > 0
                    ? 1
                    : -1;
        }

        public bool MatchCondition(float height, float moisture, float heat)
        {
            return _minHeight <= height && height <= _maxHeight &&
                   _minMoisture <= moisture && moisture <= _maxMoisture &&
                   _minHeat <= heat && heat <= _maxHeat;
        }

        public static BiomePreset For(float height, float moisture, float heat)
        {
            BiomePreset biomeToReturn = null;
            foreach (var biome in DataHolder.GetAllBiomes())
                if (biome.MatchCondition(height, moisture, heat))
                    if (biome.CompareTo(biomeToReturn) > 0)
                        biomeToReturn = biome;

            return biomeToReturn;
        }

        public PointPreset PointFor(float height, float worth, float presence)
        {
            PointPreset pointToReturn = null;
            foreach (var point in _pointPresets)
                if (point.MatchCondition(height, worth, presence))
                    if (point.CompareTo(pointToReturn) > 0)
                        pointToReturn = point;
            return pointToReturn;
        }

        public override string ToString()
        {
            return base.ToString() + $", name: {name}";
        }

        public static string CheckBiomeCoverage()
        {
            var sb = new StringBuilder("(height, moisture, heat)\n");
            var ranges = new List<Range>();
            const float step = 0.01f;
            for (float height = 0; height < 1f; height += step)
            for (float moisture = 0; moisture < 1f; moisture += step)
            for (float heat = 0; heat < 1f; heat += step)
            {
                var biome = For(height, moisture, heat);
                if (biome == null)
                {
                    foreach (var range in ranges)
                        if (range.CheckAndAdd(height, moisture, heat))
                            goto outer; // Java one love, java can solve this without doing 'goto'
                    ranges.Add(new Range(height, moisture, heat, step, step / 10f));
                }

                outer: ;
            }

            foreach (var range in ranges)
                sb.Append(range).Append('\n');
            return sb.ToString();
        }

        private class Range
        {
            private float from1;
            private float to1;

            private float from2;
            private float to2;

            private float from3;
            private float to3;

            private readonly float diff;

            public Range(float initial1, float initial2, float initial3, float step, float epsilon)
            {
                from1 = to1 = initial1;
                from2 = to2 = initial2;
                from3 = to3 = initial3;
                diff = step + epsilon;
            }

            public bool CheckAndAdd(float value1, float value2, float value3)
            {
                if (from1 <= value1 && value1 <= to1 &&
                    from2 <= value2 && value2 <= to2 &&
                    from3 <= value3 && value3 <= to3)
                    return true;
                if (from2 <= value2 && value2 <= to2 &&
                    from3 <= value3 && value3 <= to3)
                {
                    if (0 < from1 - value1 && from1 - value1 < diff)
                    {
                        from1 = value1;
                        return true;
                    }

                    if (0 < value1 - to1 && value1 - to1 < diff)
                    {
                        to1 = value1;
                        return true;
                    }
                }

                if (from1 <= value1 && value1 <= to1 &&
                    from3 <= value3 && value3 <= to3)
                {
                    if (0 < from2 - value2 && from2 - value2 < diff)
                    {
                        from2 = value2;
                        return true;
                    }

                    if (0 < value2 - to2 && value2 - to2 < diff)
                    {
                        to2 = value2;
                        return true;
                    }
                }

                if (from1 <= value1 && value1 <= to1 &&
                    from2 <= value2 && value2 <= to2)
                {
                    if (0 < from3 - value3 && from3 - value3 < diff)
                    {
                        from3 = value3;
                        return true;
                    }

                    if (0 < value3 - to3 && value3 - to3 < diff)
                    {
                        to3 = value3;
                        return true;
                    }
                }

                return false;
            }

            public override string ToString()
            {
                return $"({from1}-{to1}, {from2}-{to2}, {from3}-{to3})";
            }
        }
    }
}