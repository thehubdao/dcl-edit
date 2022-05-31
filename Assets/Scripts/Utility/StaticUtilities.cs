using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public static class StaticUtilities
    {
        public static IEnumerable<Transform> GetChildren(this Transform parent)
        {
            return parent.Cast<Transform>();
        }

        public static void ForAll<T>(this IEnumerable<T> ts,Action<T> action)
        {
            foreach (var t in ts)
            {
                action(t);
            }
        }
    }
}
