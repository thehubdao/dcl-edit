using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.Visuals;
using UnityEngine;

public class UiInspectorVisuals : MonoBehaviour, ISetupSceneEventListeners
{
    [SerializeField]
    private GameObject _content;

    public void SetupSceneEventListeners()
    {
        EditorStates.CurrentSceneState.CurrentScene?.SelectionState.SelectionChangedEvent.AddListener(UpdateVisuals);
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        var inspectorBuilder = new UiBuilder();

        if (EditorStates.CurrentSceneState.CurrentScene?.SelectionState.PrimarySelectedEntity == null)
        {
            inspectorBuilder
                .Title("No Entity selected")
                .ClearAndMake(_content);
            
            return;
        }

        var entityHeadBuilder = new UiBuilder()
            .StringPropertyInput("Name", "Name",
                EditorStates.CurrentSceneState.CurrentScene?.SelectionState.PrimarySelectedEntity?.CustomName ?? "");

        inspectorBuilder.Panel(entityHeadBuilder);

        foreach (var component in EditorStates.CurrentSceneState.CurrentScene?.SelectionState.PrimarySelectedEntity?.Components ?? new List<DclComponent>())
        {
            var componentBuilder = new UiBuilder()
                .PanelHeader(component.NameInCode, null);

            foreach (var property in component.Properties)
            {
                switch (property.Type)
                {
                    case DclComponent.DclComponentProperty.PropertyType.None: // not supported
                        componentBuilder.Text("None property not supported");
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.String:
                        componentBuilder.StringPropertyInput(property.PropertyName, property.PropertyName,
                            property.GetConcrete<string>().Value);
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.Int:
                        componentBuilder.NumberPropertyInput(property.PropertyName, property.PropertyName,
                            property.GetConcrete<int>().Value);
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.Float:
                        componentBuilder.NumberPropertyInput(property.PropertyName, property.PropertyName,
                            property.GetConcrete<float>().Value);
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.Boolean: // not supported yet
                        componentBuilder.Text("Boolean property not supported yet");
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.Vector3:
                        {
                            string[] xyzString = { "x", "y", "z" };
                            componentBuilder.Vector3PropertyInput(property.PropertyName, xyzString,
                                property.GetConcrete<Vector3>().Value);
                            break;
                        }
                    case DclComponent.DclComponentProperty.PropertyType.Quaternion: // not supported yet
                        componentBuilder.Text("Quaternion property not supported yet");
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.Asset: // not supported yet
                        componentBuilder.Text("Asset property not supported yet");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            inspectorBuilder.Panel(componentBuilder);
        }

        inspectorBuilder.ClearAndMake(_content);
    }
}
