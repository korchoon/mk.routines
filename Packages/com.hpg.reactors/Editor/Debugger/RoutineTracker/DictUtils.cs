using System.Collections.Generic;

namespace Lib.DataFlow
{
    public static class DictUtils
    {
        public static bool TryAdd<TKey, TValue>(this Dictionary<TKey, TValue> d, TKey key, TValue value)
        {
            if (d.ContainsKey(key))
                return false;

            d.Add(key, value);
            return true;
        }
    }
}