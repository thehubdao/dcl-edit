using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Utility
{
    public static class StaticUtilities
    {
        public static IEnumerable<Transform> GetChildren(this Transform parent)
        {
            return parent.Cast<Transform>();
        }

        public static void ForAll<T>(this IEnumerable<T> ts, Action<T> action)
        {
            foreach (var t in ts)
            {
                action(t);
            }
        }

        public static IEnumerable<T> InEnumerable<T>(this T t)
        {
            yield return t;
        }

        public static T FirstOrNull<T>(this IEnumerable<T> ts, Func<T, bool> predicate) where T : class
        {
            foreach (var t in ts)
            {
                if (predicate.Invoke(t))
                    return t;
            }

            return null;
        }

        public static string Shortened(this Guid guid)
        {
            var guidString = guid.ToString();
            return guidString.Substring(0, 4) + " ... " + guidString.Substring(guidString.Length - 4, 4);
        }

        public static string Indent(this string value, int level)
        {
            var builder = new StringBuilder(value.Length + (level * 4));

            for (int i = 0; i < level; i++)
            {
                builder.Append("    ");
            }

            builder.Append(value);

            return builder.ToString();
        }

        public static void SetLayerRecursive(GameObject gameObject, LayerMask layer)
        {
            gameObject.layer = layer;
            foreach (Transform child in gameObject.transform)
            {
                SetLayerRecursive(child.gameObject, layer);
            }
        }
    }
}
