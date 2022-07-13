#if UNITY_EDITOR

using System;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Visuals
{
    public static class CustomEditorUtils
    {

        public static void DrawEntityToGui(DclEntity entity, int indent = 0, bool shortened = false)
        {
            var indentString = "";

            for (int i = 0; i < indent; i++)
            {
                indentString += "    ";
            }

            // print name
            GUILayout.Label(indentString + $"Name: {entity.CustomName} ({entity.Id.Shortened()})");

            // print parent
            if(!shortened)
            {
                if (entity.Parent != null)
                {
                    GUILayout.Label(indentString + $"Parent: {entity.Parent.CustomName} ({entity.Parent.Id.Shortened()})");
                }
                else
                {
                    GUILayout.Label(indentString + "Parent: None");
                }
            }

            // Print Children amount
            if(!shortened)
            {
                GUILayout.Label(indentString + "Children: " + entity.Children.Count());
            }

            // Print components
            GUILayout.Label(indentString + "Components: " + entity.Components.Count);
            foreach (var component in entity.Components)
            {
                GUILayout.Label(indentString + $"    {component.NameInCode}({component.NameOfSlot})");
                if(!shortened)
                {
                    foreach (var property in component.Properties)
                    {
                        GUILayout.Label(indentString + $"        {property.PropertyName}({Enum.GetName(typeof(DclComponent.DclComponentProperty.PropertyType), property.Type)})");
                        switch (property.Type)
                        {
                            case DclComponent.DclComponentProperty.PropertyType.None:
                                GUILayout.Label(indentString + "            No Value");
                                break;
                            case DclComponent.DclComponentProperty.PropertyType.String:
                                GUILayout.Label(indentString + "            " + property.GetConcrete<string>().Value);
                                break;
                            case DclComponent.DclComponentProperty.PropertyType.Int:
                                GUILayout.Label(indentString + "            " + property.GetConcrete<int>().Value);
                                break;
                            case DclComponent.DclComponentProperty.PropertyType.Float:
                                GUILayout.Label(indentString + "            " + property.GetConcrete<float>().Value);
                                break;
                            case DclComponent.DclComponentProperty.PropertyType.Boolean:
                                GUILayout.Label(indentString + "            " + (property.GetConcrete<bool>().Value ? "true" : "false"));
                                break;
                            case DclComponent.DclComponentProperty.PropertyType.Vector3:
                                GUILayout.Label(indentString + "            " + property.GetConcrete<Vector3>().Value);
                                break;
                            case DclComponent.DclComponentProperty.PropertyType.Quaternion:
                                GUILayout.Label(indentString + "            " + property.GetConcrete<Quaternion>().Value);
                                break;
                            case DclComponent.DclComponentProperty.PropertyType.Asset:
                                var guid = property.GetConcrete<Guid>().Value;
                                var asset = EditorStates.CurrentProjectState.Assets.GetAssetById(guid);
                                if (asset != null)
                                    GUILayout.Label(indentString + $"            {asset.Name} ({guid.Shortened()})");
                                else
                                    GUILayout.Label(indentString + $"            Asset not found: {guid}");
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

    }
}

#endif