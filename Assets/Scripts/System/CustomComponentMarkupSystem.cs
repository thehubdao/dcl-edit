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

            [CanBeNull]
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
            public JsonLineNumbers.JTokenWithLineInfo @type;

            [CanBeNull]
            public JsonLineNumbers.JTokenWithLineInfo @default;
        }

#pragma warning restore CS0649

        public class CustomComponentProblem : IFileReadingProblem
        {
            public CustomComponentProblem(string description, string location)
            {
                this.description = description;
                this.location = location;
            }

            public string description { get; }
            public string location { get; }
        }

        public class CustomComponentException : Exception, IFileReadingProblem
        {
            public CustomComponentException(string description, int line, int column, string path, string lineText)
            {
                this.description = description;
                this.line = line;
                this.column = column;
                this.path = path;
                this.lineText = lineText;
            }

            public CustomComponentException(string description, JsonLineNumbers.JTokenWithLineInfo token)
            {
                this.description = description;
                this.line = token.line;
                this.column = token.column;
                this.path = token.path;
                this.lineText = token.lineText;
            }

            public string description { get; }
            public string path { get; }

            private readonly int line;
            private readonly int column;
            private readonly string lineText;

            public override string Message =>
                $"{description}\n  at {location}";


            public string location => $"{path}:{line}:{column}";

            public string faultyCode => $"{lineText}\n{"^".Indent(column, " ")}";
        }

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
            try
            {
                var customComponentProblems = new List<IFileReadingProblem>();

                var markUpDates =
                    fileManagerSystem
                        .GetAllFilesWithExtension(".js", ".ts", ".dcecomp")
                        .SelectMany(path => FindCustomComponentMarkups(path, customComponentProblems));

                foreach (var componentData in markUpDates)
                {
                    try
                    {
                        var properties = componentData.properties ?? new List<DceCompPropertyData>();

                        var propertyDefinitions = properties
                            .Select(p =>
                            {
                                var typeFromString = GetTypeFromString(p.type);
                                return new DclComponent.DclComponentProperty.PropertyDefinition(
                                    p.name,
                                    typeFromString,
                                    GetDefaultPropertyValue(typeFromString, p.@default));
                            })
                            .ToArray();

                        var componentDefinition = new DclComponent.ComponentDefinition(
                            componentData.className,
                            componentData.componentName ?? componentData.className,
                            componentData.importFile,
                            propertyDefinitions);

                        var availableComponent = new AvailableComponentsState.AvailableComponent
                        {
                            category = "Custom",
                            availableInAddComponentMenu = true,
                            componentDefinition = componentDefinition
                        };

                        availableComponentsState.UpdateCustomComponent(availableComponent);
                    }
                    catch (CustomComponentException cce)
                    {
                        customComponentProblems.Add(cce);
                    }
                }

                foreach (var problem in customComponentProblems)
                {
                    Debug.LogWarning($"{problem.description} at {problem.location}");
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }

        private DclComponent.DclComponentProperty.PropertyType GetTypeFromString(JsonLineNumbers.JTokenWithLineInfo typeToken)
        {
            string typeString;
            try
            {
                typeString = typeToken.token.Value<string>();
            }
            catch (Exception)
            {
                throw new CustomComponentException("The type has to have a string value", typeToken);
            }

            return typeString switch
            {
                "string" => DclComponent.DclComponentProperty.PropertyType.String,
                "number" => DclComponent.DclComponentProperty.PropertyType.Float,
                "vector3" => DclComponent.DclComponentProperty.PropertyType.Vector3,
                _ => throw new CustomComponentException("Type has to be one of the following values: \"string\", \"number\", \"vector3\" ", typeToken)
            };
        }

        private dynamic GetDefaultPropertyValue(DclComponent.DclComponentProperty.PropertyType type, [CanBeNull] JsonLineNumbers.JTokenWithLineInfo value)
        {
            if (value == null)
            {
                return GetDefaultDefaultFromType(type);
            }

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

        private string GetStringFromJObject(JsonLineNumbers.JTokenWithLineInfo token)
        {
            try
            {
                return token.token.Value<string>();
            }
            catch (Exception)
            {
                throw new CustomComponentException("The default of a property of type string has to be a string", token);
            }
        }

        private int GetIntFromJObject(JsonLineNumbers.JTokenWithLineInfo token)
        {
            try
            {
                return token.token.Value<int>();
            }
            catch (Exception)
            {
                throw new CustomComponentException("The default of a property of type int has to be an number", token);
            }
        }

        private float GetFloatFromJObject(JsonLineNumbers.JTokenWithLineInfo token)
        {
            try
            {
                return token.token.Value<float>();
            }
            catch (Exception)
            {
                throw new CustomComponentException("The default of a property of type number has to be a number", token);
            }
        }

        private bool GetBoolFromJObject(JsonLineNumbers.JTokenWithLineInfo token)
        {
            try
            {
                return token.token.Value<bool>();
            }
            catch (Exception)
            {
                throw new CustomComponentException("The default of a property of type bool has to be a bool", token);
            }
        }

        private Vector3 GetVector3FromJObject(JsonLineNumbers.JTokenWithLineInfo token)
        {
            // value is a list with 3 numbers
            // extract that list
            var list = token.token.Value<JArray>();

            // TODO: catch errors

            // extract the numbers
            var x = list[0].Value<float>();
            var y = list[1].Value<float>();
            var z = list[2].Value<float>();

            // return the vector
            return new Vector3(x, y, z);
        }

        private Quaternion GetQuaternionFromJObject(JsonLineNumbers.JTokenWithLineInfo token)
        {
            return Quaternion.identity; // TODO: get actual default value
        }

        private Guid GetGuidFromJObject(JsonLineNumbers.JTokenWithLineInfo token)
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

        public DceCompComponentData[] FindCustomComponentMarkups(string path, List<IFileReadingProblem> problems)
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
                var indices = GetIndicesOf(fileContents, "#DCECOMP");

                // limit markup texts to the json only
                var jsonTexts = indices.Select(i => ExtractJsonFromRawString(fileContents, i + 8));

                var enumerable = jsonTexts as string[] ?? jsonTexts.ToArray();
                if (enumerable.Length == 0)
                    return Array.Empty<DceCompComponentData>();

                //Debug.Log($"jsonTexts.First(): {enumerable.First()}");


                return enumerable
                    .SelectMany(t =>
                    {
                        try
                        {
                            return JsonConvert.DeserializeObject<DceCompComponentData>(t, new JsonLineNumbers(t, path)).InEnumerable();
                        }
                        catch (Exception)
                        {
                            problems.Add(new CustomComponentProblem("A #DCECOMP tag was not followed by valid Json", path));
                            return Enumerable.Empty<DceCompComponentData>();
                        }
                    })
                    .Select(d =>
                    {
                        if (isJsOrTs && d.importFile is null)
                        {
                            d.importFile = GetImportPathFromFilePath(path);
                        }

                        return d;
                    })
                    .ToArray();
            }
            catch (Exception e)
            {
                problems.Add(new CustomComponentProblem(e.Message, path));

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
