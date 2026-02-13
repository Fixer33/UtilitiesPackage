using UnityEngine;

namespace Utilities.Extensions
{
    public static class FloatExtensions
    {
        public static float Clamp(this float value, float min, float max)
        {
            return Mathf.Clamp(value, min, max);
        }
        
        public static float Clamp01(this float value)
        {
            return Mathf.Clamp01(value);
        }
        
        public static float Map(this float value, float from1, float to1, float from2, float to2)
        {
            return Mathf.Lerp(from2, to2, (value - from1) / (to1 - from1));
        }
        
        public static float Round(this float value, int digits)
        {
            return Mathf.Round(value * Mathf.Pow(10, digits)) / Mathf.Pow(10, digits);
        }
        
        public static float RoundToNearest(this float value, float nearest)
        {
            return Mathf.Round(value / nearest) * nearest;
        }
        
        public static float RoundToNearest(this float value, int nearest)
        {
            return Mathf.Round(value / nearest) * nearest;
        }
        
        public static float Sign(this float value)
        {
            return Mathf.Sign(value);
        }
        
        public static float Abs(this float value)
        {
            return Mathf.Abs(value);
        }
        
        public static float Sqrt(this float value)
        {
            return Mathf.Sqrt(value);
        }
        
        public static float Pow(this float value, float power)
        {
            return Mathf.Pow(value, power);
        }
        
        public static float Log(this float value, float baseValue)
        {
            return Mathf.Log(value, baseValue);
        }
        
        public static float Log(this float value)
        {
            return Mathf.Log(value);
        }
        
        public static float Ceil(this float value)
        {
            return Mathf.Ceil(value);
        }
        
        public static float Floor(this float value)
        {
            return Mathf.Floor(value);
        }
        
        public static int RoundToInt(this float value)
        {
            return Mathf.RoundToInt(value);
        }

        /// <summary>
        /// Returns the smallest integer number (cast as int) greater than or equal to f.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int CeilToInt(this float value)
        {
            return Mathf.CeilToInt(value);
        }
        
        /// <summary>
        /// Returns the greatest integer number (cast as int) smaller than or equal to f.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int FloorToInt(this float value)
        {
            return Mathf.FloorToInt(value);
        }
        
        public static float Lerp(this float value1, float value2, float amount)
        {
            return Mathf.Lerp(value1, value2, amount);
        }
        
        public static float Min(this float value1, float value2)
        {
            return Mathf.Min(value1, value2);
        }
        
        public static float Max(this float value1, float value2)
        {
            return Mathf.Max(value1, value2);
        }
        
        public static float ToRadians(this float degrees)
        {
            return degrees * Mathf.Deg2Rad;
        }
        
        public static float ToDegrees(this float radians)
        {
            return radians * Mathf.Rad2Deg;
        }
    }
}