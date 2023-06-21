using System;
using System.Collections.Generic;

namespace Assets.Scripts.Utility
{
    public static class ExtensionMethods
    {
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> source, Dictionary<TKey, TValue> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException("Collection is null");
            }

            foreach (var item in collection)
            {
                source[item.Key] = item.Value;
            } 
        }
    }
}