using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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

        [MenuItem("Edit/Change Dev Project Path")]
        public static void ChangeDevProjectPath()
        {
            var devProjectPathFilePath = Application.dataPath + "/dev_project_path.txt";
            string oldPath = null;

            if (File.Exists(devProjectPathFilePath))
            {
                oldPath = File.ReadAllText(devProjectPathFilePath);
            }

            var newPath = EditorUtility.OpenFolderPanel("Select Dev Project Path", oldPath ?? Application.dataPath, "Project Folder");

            if (newPath == "")
            {
                Debug.Log("Path selection aborted by user");
                return;
            }

            if (!File.Exists(newPath + "/scene.json"))
            {
                // display a warning, but still allow it, so the developer is able to test invalid project paths
                Debug.LogWarning($"The path \"{newPath}\" does not seem to be a dcl project");
            }

            File.WriteAllText(devProjectPathFilePath, newPath);
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

        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static string MakeRelativePath(string fromPath, string toPath)
        {
            var fromPathFull = Path.GetFullPath(fromPath);
            var toPathFull = Path.GetFullPath(toPath);

            var fromPathSplit = fromPathFull.Split('/', '\\');
            var toPathSplit = toPathFull.Split('/', '\\');

            var samePartCount = 0;
            for (var i = 0; i < Math.Min(fromPathSplit.Length, toPathSplit.Length); i++)
            {
                if (fromPathSplit[i] == toPathSplit[i])
                {
                    samePartCount++;
                }
                else
                {
                    break;
                }
            }

            var relativePath = new StringBuilder();

            for (var i = samePartCount; i < fromPathSplit.Length; i++)
            {
                relativePath.Append("..");
                relativePath.Append("/");
            }

            for (var i = samePartCount; i < toPathSplit.Length; i++)
            {
                relativePath.Append(toPathSplit[i]);
                relativePath.Append("/");
            }

            return relativePath.ToString().TrimEnd('/');
        }

        /// <summary>Applies an accumulator function over a sequence. When the sequence is empty, returns the default value instead</summary>
        /// <param name="source">An <see cref="T:System.Collections.Generic.IEnumerable`1" /> to aggregate over.</param>
        /// <param name="func">An accumulator function to be invoked on each element.</param>
        /// <param name="defaultValue">A default value to be returned, when source is an empty sequence</param>
        /// <typeparam name="TSource">The type of the elements of <paramref name="source" />.</typeparam>
        /// <returns>The final accumulator value.</returns>
        /// <exception cref="T:System.ArgumentNullException">
        /// <paramref name="source" /> or <paramref name="func" /> is <see langword="null" />.</exception>
        public static TSource AggregateOrDefault<TSource>(
            this IEnumerable<TSource> source,
            Func<TSource, TSource, TSource> func,
            TSource defaultValue = default)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (func == null)
                throw new ArgumentNullException(nameof(func));

            using var enumerator = source.GetEnumerator();

            var source1 = enumerator.MoveNext() ? enumerator.Current : defaultValue;
            while (enumerator.MoveNext())
                source1 = func(source1, enumerator.Current);
            return source1;
        }


        public static void Deconstruct<TKey, TValue>(
            this KeyValuePair<TKey, TValue> kvp,
            out TKey key,
            out TValue value)
        {
            key = kvp.Key;
            value = kvp.Value;
        }

        public static Vector3 VectorFromTo(Vector3 from, Vector3 to)
        {
            return to - from;
        }
    }
}
