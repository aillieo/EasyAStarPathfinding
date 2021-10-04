using System;
using System.Collections.Generic;

namespace AillieoUtils
{
    public static class IEnumerableExt
    {
        public static T MinFor<T>(this IEnumerable<T> enumerable, Func<T, float> func)
        {
            T min = default;
            float minValue = float.MaxValue;
            foreach (var item in enumerable)
            {
                float newValue = func(item);
                if (newValue < minValue)
                {
                    minValue = newValue;
                    min = item;
                }
            }

            return min;
        }
    }
}
