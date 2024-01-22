#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Utility;
using UnityEngine;
using Zenject;
using static Assets.Scripts.SceneState.DclComponent.DclComponentProperty;
using Debug = UnityEngine.Debug;

public class CustomComponentDefinitionSystem
{
    private const string fileExtensionTs = ".ts";

    // Dependencies
#pragma warning disable CS8618
    private FileManagerSystem fileManagerSystem;
    private AvailableComponentsState availableComponentsState;
    private IPathState pathState;
#pragma warning restore CS8618

    [Inject]
    public void Construct(
        FileManagerSystem fileManagerSystem,
        AvailableComponentsState availableComponentsState,
        IPathState pathState)
    {
        this.fileManagerSystem = fileManagerSystem;
        this.availableComponentsState = availableComponentsState;
        this.pathState = pathState;
    }

    public void DiscoverComponentDefinitions()
    {
        fileManagerSystem.ForAllFilesByFilterNowAndOnChange(
            Path.Combine(pathState.ProjectPath, "src"),
            "*" + fileExtensionTs,
            FileScannerCallback);
    }

    private void FileScannerCallback(FileManagerSystem.FileWatcherEvent e, string path, string oldPath)
    {
        switch (e)
        {
            case FileManagerSystem.FileWatcherEvent.Initial:
            case FileManagerSystem.FileWatcherEvent.Created:
            case FileManagerSystem.FileWatcherEvent.Changed:
            case FileManagerSystem.FileWatcherEvent.Renamed:
                Task.Run(() => ScanForDefineComponent(path, oldPath));
                break;
            case FileManagerSystem.FileWatcherEvent.Deleted:
                availableComponentsState.RemoveAllCustomComponentsWithPath(path);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(e), e, null);
        }
    }

    private PropertyType? GetPropertyTypeFromSchema(string schemaType)
    {
        return schemaType switch
        {
            "Boolean" => PropertyType.Boolean,
            "String" => PropertyType.String,
            "Float" => PropertyType.Float,
            "Double" => PropertyType.Float,
            "Byte" => PropertyType.Int,
            "Short" => PropertyType.Int,
            "Int" => PropertyType.Int,
            "Int64" => PropertyType.Int,
            "Number" => PropertyType.Float,
            "Vector3" => PropertyType.Vector3,
            "Quaternion" => PropertyType.Quaternion,
            "Color3" => null,
            "Color4" => null,
            "Entity" => null,
            "EnumNumber" => null,
            "EnumString" => null,
            "Array" => null,
            "Map" => null,
            "Optional" => null,
            "OneOf" => null,
            _ => throw new ArgumentOutOfRangeException(nameof(schemaType), schemaType, null)
        };
    }

    private dynamic? GetDefaultDefaultFromType(DclComponent.DclComponentProperty.PropertyType type)
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

    private string GetImportPathFromFilePath(string path)
    {
        var relativePath = StaticUtilities.MakeRelativePath(pathState.ProjectPath, path);
        var folderPath = Path.GetDirectoryName(relativePath);
        var fileWithoutExtension = Path.GetFileNameWithoutExtension(relativePath);
        var folderWithFile = Path.Combine(folderPath, fileWithoutExtension);
        var pathWithForwardSlashes = folderWithFile.Replace('\\', '/');
        var pathRelativeToScenesTs = "../../../" + pathWithForwardSlashes;
        return pathRelativeToScenesTs;
    }

    private void ScanForDefineComponent(string path, string oldPath)
    {
        var file = File.ReadAllText(path);

        // summoning Cthulhu
        var validTsSymbol = "[$_\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}][$_\\p{Lu}\\p{Ll}\\p{Lt}\\p{Lm}\\p{Lo}\\p{Nl}\\u200C\\u200D\\p{Mn}\\p{Mc}\\p{Nd}\\p{Pc}]*";

        // What the fuck is this?
        var regexPattern = $"export\\s*(const|var|let)\\s*(?<class>{validTsSymbol})\\s*=\\s*engine\\s*\\.\\s*defineComponent\\s*\\(\\s*(\"(?<componentName>[^\"]*)\"|\'(?<componentName>[^\']*)\')\\s*,\\s*(?<schema>{{[\\s\\S]*?}})\\s*(\\,\\s*(?<defaults>{{[\\s\\S]*?}}))?\\s*\\)";

        var result = Regex.Matches(file, regexPattern);

        string NoWhiteSpaces(string s)
        {
            return s.Where(c => !char.IsWhiteSpace(c)).Aggregate("", (s1, c) => s1 + c);
        }

        var newlyAvailableComponents = new List<AvailableComponentsState.AvailableComponent>();

        foreach (Match match in result)
        {
            var className = match.Groups["class"].Value;
            var componentName = match.Groups["componentName"].Value;
            var schema = match.Groups["schema"].Value;
            var defaults = match.Groups["defaults"].Value;

            //Debug.Log($"Found component: {NoWhiteSpaces(className)} {NoWhiteSpaces(componentName)} {NoWhiteSpaces(schema)} {NoWhiteSpaces(defaults)}");

            // I hate my self
            var schemaPattern = $"(?<propertyName>{validTsSymbol})\\s*:\\s*Schemas\\s*.\\s*(?<schemaType>Boolean|String|Float|Double|Byte|Short|Int|Int64|Number|Vector3|Quaternion|Color3|Color4|Entity|EnumNumber|EnumString|Array|Map|Optional|OneOf)";

            var schemaResult = Regex.Matches(schema, schemaPattern);

            var propertyList = new List<PropertyDefinition>();

            foreach (Match schemaMatch in schemaResult)
            {
                var propertyName = schemaMatch.Groups["propertyName"].Value;
                var schemaType = schemaMatch.Groups["schemaType"].Value;

                var propertyType = GetPropertyTypeFromSchema(schemaType);
                if (!propertyType.HasValue)
                    continue;

                propertyList.Add(new PropertyDefinition(propertyName, propertyType.Value, GetDefaultDefaultFromType(propertyType.Value)));
            }

            newlyAvailableComponents.Add(new AvailableComponentsState.AvailableComponent
            {
                name = componentName,
                category = "Custom",
                availableInAddComponentMenu = true,
                componentDefinition = new DclComponent.ComponentDefinition(
                    className,
                    componentName,
                    true,
                    GetImportPathFromFilePath(path),
                    propertyList.ToArray())
            });
        }

        lock (availableComponentsState)
        {
            availableComponentsState.RemoveAllCustomComponentsWithPath(oldPath);
            foreach (var newlyAvailableComponent in newlyAvailableComponents)
            {
                availableComponentsState.UpdateCustomComponent(newlyAvailableComponent);
            }
        }
    }
}
