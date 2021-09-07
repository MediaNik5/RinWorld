using RinWorld.Util.Data;
using UnityEngine;

namespace RinWorld.Util
{
    public static class MathHelper
    {
        
        /// <summary>
        /// Distributes x through another range.
        /// </summary>
        /// <param name="x">Range [0, 1]</param>
        /// <returns>Range [-1, 1]</returns>
        public static float Distribution(float x)
        {
            return 2 * x - 1f;
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

        static MathHelper()
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
        /// <summary>
        /// Mean temperature has its calculation process:
        /// The formula is Mean = Linear*Distribution(heat) + Shift + MoistureShift*moisture + HeightShift*height.
        /// Where Linear, Shift, MoistureShift, HeightShift are constants, and
        /// MoistureShift, HeightShift values loaded from /core/conditions/cell_conditions.json.
        /// Distribution(x) is mean temperature scatter, [-1, 1].
        /// Linear and Shift change f(x)'s range [-1, 1] to range [MinMeanTemperature, MaxMeanTemperature].
        /// MoistureShift*moisture + HeightShift*height are affects of moisture and height on mean temperature.
        /// </summary>
        /// <param name="heat">Heat value in range [0, 1]</param>
        /// <param name="moisture">Moisture value in range [0, 1]</param>
        /// <param name="height">Height value in range [0, 1]</param>
        /// <returns>Mean temperature for those heat, moisture, height in range [MinTemperature - MoistureShift - HeightShift, MaxTemperature + MoistureShift + HeightShift]</returns>
        
        public static float MeanTemperature(float heat, float moisture, float height)
        {
            return Linear * Distribution(heat) + Shift + MoistureShift * moisture + HeightShift * height;
        }
        /// <summary>
        /// Delta temperature has its calculation process. It depends only on moisture and constants.
        ///
        /// Max temperature is mean + delta. Min is mean - delta.
        /// </summary>
        /// <param name="moisture">Moisture value in range [0, 1]</param>
        /// <returns>Delta temperature for this moisture</returns>
        public static float DeltaTemperature(float moisture)
        {
            return MaxMoistureAddition / (1 + MoistureMultiplier * Mathf.Pow(moisture, MoistureExponent));
        }
    }
}