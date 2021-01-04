using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xenogears.ResourceRipper
{
    public static class Extensions
    {
        public static int? ToInt32(this object o)
        {
            if (o == null)
                return null;
            int a;
            if (Int32.TryParse(o.ToString(), out a))
            {
                return a;
            }
            return null;
        }

        public static float? ToFloat(this object o)
        {
            if (o == null)
                return null;
            float a;
            if (float.TryParse(o.ToString(), out a))
            {
                return a;
            }
            return null;
        }

        public static IEnumerable<IEnumerable<TSource>> Batch<TSource>(
                  this IEnumerable<TSource> source, int size)
        {
            TSource[] bucket = null;
            var count = 0;

            foreach (var item in source)
            {
                if (bucket == null)
                    bucket = new TSource[size];

                bucket[count++] = item;
                if (count != size)
                    continue;

                yield return bucket;

                bucket = null;
                count = 0;
            }

            if (bucket != null && count > 0)
                yield return bucket.Take(count).ToArray();
        }
    }
}
