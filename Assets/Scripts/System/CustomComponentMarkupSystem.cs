using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class CustomComponentMarkupSystem
    {
#pragma warning disable CS0649 // unassigned field

        public struct DceCompComponentData
        {
            public string @name;
            public List<DceCompPropertyData> @properties;
        }

        public struct DceCompPropertyData
        {
            public string @name;
            public string @type;
        }

#pragma warning restore CS0649

        // Dependencies
        private FileManagerSystem fileManagerSystem;
        private AvailableComponentsState availableComponentsState;

        [Inject]
        private void Construct(
            FileManagerSystem fileManagerSystem,
            AvailableComponentsState availableComponentsState)
        {
            this.fileManagerSystem = fileManagerSystem;
            this.availableComponentsState = availableComponentsState;
        }


        public void SetupCustomComponents()
        {
            var markUpDates =
                fileManagerSystem
                    .GetAllFilesWithExtension(".js", ".ts", ".dceComp")
                    .SelectMany(FindCustomComponentMarkups);

            foreach (var componentData in markUpDates)
            {
                availableComponentsState.UpdateCustomComponent(new AvailableComponentsState.AvailableComponent
                {
                    category = $"Custom/{componentData.name}",
                    availableInAddComponentMenu = true,
                    componentDefinition = new DclComponent.ComponentDefinition(
                        componentData.name,
                        componentData.name,
                        componentData
                            .properties
                            .Select(p => new DclComponent.DclComponentProperty.PropertyDefinition(
                                p.name,
                                DclComponent.DclComponentProperty.PropertyType.String,
                                ""))
                            .ToArray())
                });
            }
        }

        public IEnumerable<DceCompComponentData> FindCustomComponentMarkups(string path)
        {
            var fileContents = File.ReadAllText(path);

            // if file is js or ts, filter out everything, that is not a comment
            if (Path.GetExtension(path) == ".js" || Path.GetExtension(path) == ".ts")
            {
                fileContents = FilterComments(fileContents);
            }

            // find raw component markup texts
            var rawTexts = fileContents.Split(new[] {"#DCECOMP"}, StringSplitOptions.None);

            // limit markup texts to the json only
            var jsonTexts = rawTexts.Skip(1).Select(ExtractJsonFromRawString);

            return jsonTexts.Select(JsonConvert.DeserializeObject<DceCompComponentData>);
        }

        private string ExtractJsonFromRawString(string value)
        {
            // has to start with '{'
            value = value.Trim();

            var result = new StringBuilder();
            var bracketLevel = 0;

            foreach (var c in value)
            {
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
                    }
                    else if (text[i] == '/' && text[i + 1] == '/')
                    {
                        // start a line comment
                        commentMode = 2;
                        i++;
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
                    if (text[i] == '\n' || text[i] == '\r')
                    {
                        // end the line comment
                        result.Append("\r\n");
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
