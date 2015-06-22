using System;
using System.Collections.Generic;
using System.Linq;

namespace Surrogates.Applications.Infrastructure
{
    public static class CollectionMixins
    {
        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
            where T : IDictionary<K, V>, new()
        {
            T newMap = new T();
            foreach (IDictionary<K, V> src in
                (new List<IDictionary<K, V>> { me }).Concat(others))
            {
                // ^-- echk. Not quite there type-system.
                foreach (KeyValuePair<K, V> p in src)
                {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        }

        public static T[] Prepend<T>(this T[] self, T value)
        {
            var newValues = (T[]) Array.CreateInstance(
                typeof(Type), self.Length + 1);
            newValues[0] = value;
            Array.Copy(self, 0, newValues, 1, self.Length);

            return newValues;
        }
    }
}
