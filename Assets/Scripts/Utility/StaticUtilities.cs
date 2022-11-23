using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
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

        public static bool PathEqual(this string self, string other)
        {
            var selfPath = Path.GetDirectoryName(self) + Path.DirectorySeparatorChar + Path.GetFileName(self);
            var otherPath = Path.GetDirectoryName(other) + Path.DirectorySeparatorChar + Path.GetFileName(other);

            return selfPath.Equals(otherPath);
        }

#if UNITY_EDITOR
        [MenuItem("Edit/Unlock Reload Assemblies")]
        public static void UnlockAssemblies()
        {
            EditorApplication.UnlockReloadAssemblies();
        }
#endif // UNITY_EDITOR

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
