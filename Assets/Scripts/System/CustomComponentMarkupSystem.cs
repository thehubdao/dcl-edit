using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class CustomComponentMarkupSystem
    {
#pragma warning disable CS0649 // unassigned field

        public struct DceCompComponentData
        {
            public string @className;

            [CanBeNull]
            public string @componentName;

            [CanBeNull]
            public string @importFile;

            public List<DceCompPropertyData> @properties;
        }

        public struct DceCompPropertyData
        {
            public string @name;

            /// <summary>
            /// possible values:<br/>
            /// string<br/>
            /// number<br/>
            /// vector3
            /// </summary>
            public string @type;

            [CanBeNull]
            public JsonLineNumbers.JTokenWithLineInfo @default;
        }

#pragma warning restore CS0649

        // Dependencies
        private FileManagerSystem fileManagerSystem;
        private AvailableComponentsState availableComponentsState;
        private PathState pathState;

        [Inject]
        private void Construct(
            FileManagerSystem fileManagerSystem,
            AvailableComponentsState availableComponentsState,
            PathState pathState)
        {
            this.fileManagerSystem = fileManagerSystem;
            this.availableComponentsState = availableComponentsState;
            this.pathState = pathState;
        }


        public void SetupCustomComponents()
        {
            var markUpDates =
                fileManagerSystem
                    .GetAllFilesWithExtension(".js", ".ts", ".dcecomp")
                    .SelectMany(FindCustomComponentMarkups);

            foreach (var componentData in markUpDates)
            {
                availableComponentsState.UpdateCustomComponent(new AvailableComponentsState.AvailableComponent
                {
                    category = $"Custom",
                    availableInAddComponentMenu = true,
                    componentDefinition = new DclComponent.ComponentDefinition(
                        componentData.className,
                        componentData.componentName ?? componentData.className,
                        componentData.importFile,
                        componentData
                            .properties
                            .Select(p => new DclComponent.DclComponentProperty.PropertyDefinition(
                                p.name,
                                GetTypeFromString(p.type),
                                GetDefaultPropertyValue(GetTypeFromString(p.type), p.@default)))
                            .ToArray())
                });
            }
        }

        private DclComponent.DclComponentProperty.PropertyType GetTypeFromString(string typeString)
        {
            return typeString switch
            {
                "string" => DclComponent.DclComponentProperty.PropertyType.String,
                "number" => DclComponent.DclComponentProperty.PropertyType.Float,
                "vector3" => DclComponent.DclComponentProperty.PropertyType.Vector3,
                _ => throw new Exception($"Unknown type {typeString}")
            };
        }

        private dynamic GetDefaultPropertyValue(DclComponent.DclComponentProperty.PropertyType type, [CanBeNull] JsonLineNumbers.JTokenWithLineInfo value)
        {
            if (value == null)
            {
                return GetDefaultDefaultFromType(type);
            }

            Debug.Log($"value: line: {value.line} column: {value.column}");

            return type switch
            {
                DclComponent.DclComponentProperty.PropertyType.None => null,
                DclComponent.DclComponentProperty.PropertyType.String => GetStringFromJObject(value),
                DclComponent.DclComponentProperty.PropertyType.Int => GetIntFromJObject(value),
                DclComponent.DclComponentProperty.PropertyType.Float => GetFloatFromJObject(value),
                DclComponent.DclComponentProperty.PropertyType.Boolean => GetBoolFromJObject(value),
                DclComponent.DclComponentProperty.PropertyType.Vector3 => GetVector3FromJObject(value),
                DclComponent.DclComponentProperty.PropertyType.Quaternion => GetQuaternionFromJObject(value),
                DclComponent.DclComponentProperty.PropertyType.Asset => GetGuidFromJObject(value),
                _ => throw new ArgumentOutOfRangeException(nameof(type))
            };
        }

        private string GetStringFromJObject(JToken value)
        {
            return value.Value<string>();
        }

        private int GetIntFromJObject(JToken value)
        {
            return value.Value<int>();
        }

        private float GetFloatFromJObject(JToken value)
        {
            return value.Value<float>();
        }

        private bool GetBoolFromJObject(JToken value)
        {
            return value.Value<bool>();
        }

        private Vector3 GetVector3FromJObject(JToken value)
        {
            // value is a list with 3 numbers
            // extract that list
            var list = value.Value<JArray>();

            // TODO: catch errors

            // extract the numbers
            var x = list[0].Value<float>();
            var y = list[1].Value<float>();
            var z = list[2].Value<float>();

            // return the vector
            return new Vector3(x, y, z);
        }

        private Quaternion GetQuaternionFromJObject(JToken value)
        {
            return Quaternion.identity; // TODO: get actual default value
        }

        private Guid GetGuidFromJObject(JToken value)
        {
            return Guid.Empty;
        }


        private dynamic GetDefaultDefaultFromType(DclComponent.DclComponentProperty.PropertyType type)
        {
            return type switch
            {
                DclComponent.DclComponentProperty.PropertyType.None => null,
                DclComponent.DclComponentProperty.PropertyType.String => "",
                DclComponent.DclComponentProperty.PropertyType.Int => 0,
                DclComponent.DclComponentProperty.PropertyType.Float => 0f,
                DclComponent.DclComponentProperty.PropertyType.Boolean => false,
                DclComponent.DclComponentProperty.PropertyType.Vector3 => Vector3.zero,
                DclComponent.DclComponentProperty.PropertyType.Quaternion => Quaternion.identity,
                DclComponent.DclComponentProperty.PropertyType.Asset => Guid.Empty,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        public DceCompComponentData[] FindCustomComponentMarkups(string path)
        {
            try
            {
                var fileContents = File.ReadAllText(path);

                // if file is js or ts, filter out everything, that is not a comment
                var isJsOrTs = Path.GetExtension(path) == ".js" || Path.GetExtension(path) == ".ts";
                if (isJsOrTs)
                {
                    fileContents = FilterComments(fileContents);
                }

                // find raw component markup texts
                //var rawTexts = fileContents.Split(new[] {"#DCECOMP"}, StringSplitOptions.None);

                var indices = GetIndicesOf(fileContents, "#DCECOMP");

                // limit markup texts to the json only
                //var jsonTexts = rawTexts.Skip(1).Select(ExtractJsonFromRawString).Where(t => t != null);
                var jsonTexts = indices.Select(i => ExtractJsonFromRawString(fileContents, i + 8));

                var enumerable = jsonTexts as string[] ?? jsonTexts.ToArray();
                if (enumerable.Length == 0)
                    return Array.Empty<DceCompComponentData>();

                Debug.Log($"jsonTexts.First(): {enumerable.First()}");


                return enumerable.Select(t => JsonConvert.DeserializeObject<DceCompComponentData>(t, new JsonLineNumbers())).Select(d =>
                {
                    if (isJsOrTs && d.importFile is null)
                    {
                        d.importFile = GetImportPathFromFilePath(path);
                    }

                    return d;
                }).ToArray();
            }
            catch (Exception e)
            {
                Debug.LogException(e);

                return Array.Empty<DceCompComponentData>();
            }
        }

        private List<int> GetIndicesOf(string haystack, string needle)
        {
            var i = 0;
            var result = new List<int>();

            while (i >= 0)
            {
                var newFind = haystack.IndexOf(needle, i, StringComparison.InvariantCulture);

                if (newFind >= 0)
                {
                    result.Add(newFind);
                    i = newFind + 1;
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        private string GetImportPathFromFilePath(string path)
        {
            var relativePath = StaticUtilities.MakeRelativePath(pathState.ProjectPath, path);
            var folderPath = Path.GetDirectoryName(relativePath);
            var fileWithoutExtension = Path.GetFileNameWithoutExtension(relativePath);
            var folderWithFile = Path.Combine(folderPath, fileWithoutExtension);
            var pathWithForwardSlashes = folderWithFile.Replace('\\', '/');
            return pathWithForwardSlashes;
        }

        private string ExtractJsonFromRawString(string value, int startingFrom)
        {
            var reader = new StringReader(value);
            var result = new StringBuilder();
            var bracketLevel = 0;

            // go to start
            for (int i = 0; i < startingFrom; i++)
            {
                var c = (char) reader.Read();

                if (c == '\r')
                {
                    result.Append("\r");
                }
                else if (c == '\n')
                {
                    result.Append("\n");
                }
                else
                {
                    result.Append(" ");
                }
            }

            // find start
            while (reader.Peek() >= 0)
            {
                var c = (char) reader.Read();

                if (c == '{')
                {
                    result.Append('{');
                    bracketLevel++;
                    break;
                }

                switch (c)
                {
                    case '\r':
                        result.Append("\r");
                        break;
                    case '\n':
                        result.Append("\n");
                        break;
                    default:
                        result.Append(" ");
                        break;
                }
            }

            // extract json
            while (reader.Peek() >= 0)
            {
                var c = (char) reader.Read();

                switch (c)
                {
                    case '{':
                        bracketLevel++;
                        break;
                    case '}':
                        bracketLevel--;
                        break;
                }

                if (bracketLevel <= 0)
                {
                    result.Append(c);
                    break;
                }

                result.Append(c);
            }

            return result.ToString();
        }

        /// <summary>
        /// Changes text so that only comments remain
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public string FilterComments(string text)
        {
            // 0: no comment
            // 1: block comment
            // 2: line comment
            var commentMode = 0;
            var result = new StringBuilder();

            for (var i = 0; i < text.Length; i++)
            {
                if (commentMode == 0)
                {
                    // if currently not in a comment
                    // see if the next text is "/*"
                    if (text[i] == '/' && text[i + 1] == '*')
                    {
                        // start a block comment
                        commentMode = 1;
                        i++;
                        result.Append("  ");
                    }
                    else if (text[i] == '/' && text[i + 1] == '/')
                    {
                        // start a line comment
                        commentMode = 2;
                        i++;
                        result.Append("  ");
                    }
                    else
                    {
                        if (text[i] == '\n')
                        {
                            result.Append("\n");
                        }
                        else if (text[i] == '\r')
                        {
                            result.Append("\r");
                        }
                        else
                        {
                            result.Append(" ");
                        }
                    }
                }
                else if (commentMode == 1)
                {
                    // if currently in a block comment
                    // see if the next text is */
                    if (text[i] == '*' && text[i + 1] == '/')
                    {
                        // end the block comment
                        commentMode = 0;
                        i++;
                        result.Append("  ");
                    }
                    else
                    {
                        // add the current character to the result
                        result.Append(text[i]);
                    }
                }
                else if (commentMode == 2)
                {
                    // if currently in a line comment
                    // see if the next text is a new line
                    if (text[i] == '\n')
                    {
                        // end the line comment
                        result.Append("\n");
                        commentMode = 0;
                    }
                    else if (text[i] == '\r')
                    {
                        // end the line comment
                        result.Append("\r");
                        commentMode = 0;
                    }
                    else
                    {
                        // add the current character to the result
                        result.Append(text[i]);
                    }
                }
            }

            return result.ToString();
        }
    }
}
