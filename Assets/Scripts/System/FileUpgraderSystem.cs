using Assets.Scripts.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

namespace Assets.Scripts.System
{
    public class FileUpgraderSystem
    {
        [Inject]
        private void Construct()
        {
            SetupUpgrades();
        }
        
        public struct Version
        {
            public int major;
            public int minor;
            public int patch;

            public Version(string versionString)
            {
                var parts = versionString.Split('.');
                Assert.AreEqual(3, parts.Length);
                major = int.Parse(parts[0]);
                minor = int.Parse(parts[1]);
                patch = int.Parse(parts[2]);
            }

            public Version(int major, int minor, int patch)
            {
                this.major = major;
                this.minor = minor;
                this.patch = patch;
            }

            public void Deconstruct(out int major, out int minor, out int patch)
            {
                major = this.major;
                minor = this.minor;
                patch = this.patch;
            }

            public static implicit operator Version((int, int, int) value)
            {
                return new Version(value.Item1, value.Item2, value.Item3);
            }

            public override string ToString()
            {
                return $"{major}.{minor}.{patch}";
            }

            public bool Equals(Version other)
            {
                return major == other.major && minor == other.minor && patch == other.patch;
            }

            public override bool Equals(object obj)
            {
                return obj is Version other && Equals(other);
            }

            public override int GetHashCode()
            {
                return (major, minor, patch).GetHashCode();
            }


            public static bool operator <(Version lhs, Version rhs)
            {
                if (lhs.major < rhs.major)
                {
                    return true;
                }

                if (lhs.major == rhs.major && lhs.minor < rhs.minor)
                {
                    return true;
                }

                if (lhs.major == rhs.major && lhs.minor == rhs.minor && lhs.patch < rhs.patch)
                {
                    return true;
                }

                return false;
            }

            public static bool operator >(Version lhs, Version rhs)
            {
                if (lhs.major > rhs.major)
                {
                    return true;
                }

                if (lhs.major == rhs.major && lhs.minor > rhs.minor)
                {
                    return true;
                }

                if (lhs.major == rhs.major && lhs.minor == rhs.minor && lhs.patch > rhs.patch)
                {
                    return true;
                }

                return false;
            }

            public static bool operator ==(Version lhs, Version rhs)
            {
                return lhs.major == rhs.major && lhs.minor == rhs.minor && lhs.patch == rhs.patch;
            }

            public static bool operator !=(Version lhs, Version rhs)
            {
                return !(lhs == rhs);
            }

            public static bool operator <=(Version lhs, Version rhs)
            {
                return lhs < rhs || lhs == rhs;
            }

            public static bool operator >=(Version lhs, Version rhs)
            {
                return lhs > rhs || lhs == rhs;
            }
        }

        [Serializable]
        private struct FileInfo
        {
            // ReSharper disable once UnassignedField.Local
            public string @dclEditVersionNumber;
        }

        public Version GetFileVersion(string path)
        {
            var fileContents = File.ReadAllText(path);
            var fileInfo = JsonConvert.DeserializeObject<FileInfo>(fileContents);
            var versionString = fileInfo.dclEditVersionNumber;
            return new Version(versionString);
        }

        private static void SetFileVersion(string path, Version version)
        {
            if (!File.Exists(path))
            {
                return;
            }
            
            var fileContents = File.ReadAllText(path);
            var json = JObject.Parse(fileContents);
            json["dclEditVersionNumber"] = (JToken)version.ToString();
            var newFileContents = json.ToString(Formatting.Indented);
            File.WriteAllText(path, newFileContents);
        }

        public void CheckUpgrades(string path)
        {
            var currentVersion = new Version(Application.version);
            var fileVersion = GetFileVersion(path);

            // file version should not be above current version
            if (fileVersion > currentVersion)
            {
                throw new Exception($"The file {path} was saved with a newer version of the editor ({fileVersion}). Please update the editor to the latest version.");
            }

            try
            {
                foreach (var (version, actions) in upgradeActions)
                {
                    if (version <= fileVersion)
                    {
                        continue;
                    }

                    if (version > currentVersion)
                    {
                        break;
                    }

                    foreach (var action in actions)
                    {
                        action(path);
                    }
                }

                SetFileVersion(path, currentVersion);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        // Utility
        private bool IsDclAssetFile(string path)
        {
            return path.EndsWith(".dclasset");
        }

        private bool IsSceneJsonFile(string path)
        {
            return path.EndsWith("scene.json");
        }

        private bool IsEntityFile(string path)
        {
            /*
             * This pattern matches filenames with the following format:
             * 
             * A human-readable name that can contain any character, followed by a dash (-).
             * A sequence of 8 hexadecimal digits (0-9 and a-f or A-F) followed by a dash (-).
             * A sequence of 3 groups of dash-separated sequences of 4 hexadecimal digits.
             * A final sequence of 12 hexadecimal digits.
             * The extension .json.
             */
            var regex = new Regex("^.+-[0-9a-fA-F]{8}(-[0-9a-fA-F]{4}){3}-[0-9a-fA-F]{12}.json$");
            return regex.IsMatch(Path.GetFileName(path));
        }

        // Upgrades
        private readonly SortedDictionary<Version, List<Action<string>>> upgradeActions = new SortedDictionary<Version, List<Action<string>>>( /*Comparer<Version>.Create((l, r) => l > r ? -1 : l < r ? 1 : 0)*/);

        private void SetupUpgrades()
        {
            upgradeActions.Add((1, 0, 2), new List<Action<string>>
            {
                UpgradeDclAssetFileNamesToIncludeOriginalFileEnding,
                UpgradePropertySceneIdToSceneInSceneComponent
            } );
        }

        private void UpgradeDclAssetFileNamesToIncludeOriginalFileEnding(string path)
        {
            if (!IsDclAssetFile(path))
            {
                return;
            }
            
            try
            {
                var fileContents = File.ReadAllText(path);
                var json = JObject.Parse(fileContents);

                var assetMetaData = json["metadata"];
                
                if (assetMetaData == null)
                {
                    throw new Exception("No metadata in file!");
                }
                
                var newMetaFileName = assetMetaData["assetFilename"] + ".dclasset";
                var newPath = Path.Combine(Path.GetDirectoryName(path), newMetaFileName);
                
                var currentVersion = new Version(Application.version);
                json["dclEditVersionNumber"] = currentVersion.ToString();
                
                var newFileContents = json.ToString(Formatting.Indented);
                File.WriteAllText(newPath, newFileContents);
                
                File.Delete(path);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        private void UpgradePropertySceneIdToSceneInSceneComponent(string path)
        {
            if (IsEntityFile(path))
            {
                var fileContents = File.ReadAllText(path);
                var json = JObject.Parse(fileContents);

                foreach (JToken property in json["components"]
                    .Where(c => c["nameInCode"].Value<String>() == "Scene")
                    .SelectMany(c => c["properties"])
                    .Where(p => p["name"].Value<String>() == "sceneId"))
                {
                    property["name"] = "scene";
                    property["type"] = "Asset";
                }

                var newFileContents = json.ToString(Formatting.Indented);
                File.WriteAllText(path, newFileContents);
            }
        }
    }
}
