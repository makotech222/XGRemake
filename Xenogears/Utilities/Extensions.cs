using System;
using System.Collections.Generic;
using System.Linq;

namespace Xenogears.Utilities
{
    public static class Extensions
    {

        public static int? ToInt32(this object o)
        {
            if (o == null)
                return null;
            int a;
            if (Int32.TryParse(o.ToString(),out a))
            {
                return a;
            }
            return null;
        }
        /// <summary>
        /// Adds to an integer value and loops around if it hits the maximum.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="amount"></param>
        /// <param name="max"></param>
        /// <param name="min"></param>
        /// <returns></returns>
        public static int LoopAdd(this int value, int amount, int max, int min = 0)
        {
            if (amount == 0)
                return value;
            bool amountPositive = amount > 0;
            for (int i = 0; i < Math.Abs(amount); i++)
            {
                value += (amountPositive) ? 1 : -1;
                if (amountPositive && value > max)
                    value = min;
                else if (!amountPositive && value < min)
                    value = max;
            }
            return value;
        }
        public static bool RoughlyEquals(this float f1, float f2, float diff = 0.01f)
        {
            return Math.Abs(f1 - f2) < diff;
        }

        public static T Next<T>(this T v) where T : Enum
        {
            return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] { default(T) }).SkipWhile(e => !v.Equals(e)).Skip(1).First();
        }

        public static T Previous<T>(this T v) where T : Enum
        {
            return Enum.GetValues(v.GetType()).Cast<T>().Concat(new[] { default(T) }).Reverse().SkipWhile(e => !v.Equals(e)).Skip(1).First();
        }
        public static string NullIfEmpty(this string s)
        {
            if (s == null)
                return null;
            if (String.IsNullOrWhiteSpace(s))
                return null;
            return s;
        }
        public static void AddRange<T, S>(this Dictionary<T, S> source, Dictionary<T, S> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Collection is null");
            }

            foreach (var item in collection)
            {
                if (!source.ContainsKey(item.Key))
                {
                    source.Add(item.Key, item.Value);
                }
                else
                {
                    // handle duplicate key issue here
                }
            }
        }
    }
}