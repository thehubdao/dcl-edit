using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
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
        if(EditorStates.CurrentInputState.InState == InputState.InStateType.UiInput)
        {
            return;
        }

        var inspectorBuilder = new UiBuilder();

        var selectedEntity = EditorStates.CurrentSceneState.CurrentScene?.SelectionState.PrimarySelectedEntity;

        if (selectedEntity == null)
        {
            inspectorBuilder
                .Title("No Entity selected")
                .ClearAndMake(_content);
            return;
        }

        // TODO: change this to actually change the entity
        var actions = new UiBuilder.UiPropertyActions<string>
        {
            OnChange = Debug.Log,
            OnSubmit = Debug.Log,
            OnAbort = Debug.Log
        };


        var entityHeadBuilder = new UiBuilder()
            .StringPropertyInput("Name", "Name",
                selectedEntity.CustomName ?? "", actions)
            .BooleanPropertyInput("Is Exposed", true);

        inspectorBuilder.Panel(entityHeadBuilder);

        foreach (var component in selectedEntity.Components ?? new List<DclComponent>())
        {
            var componentBuilder = new UiBuilder()
                .PanelHeader(component.NameInCode, null);

            foreach (var property in component.Properties)
            {
                var propertyIdentifier = new DclPropertyIdentifier(selectedEntity.Id, component.NameInCode, property.PropertyName);
                switch (property.Type)
                {
                    case DclComponent.DclComponentProperty.PropertyType.None: // not supported
                        componentBuilder.Text("None property not supported");
                        break;
                    case DclComponent.DclComponentProperty.PropertyType.String:
                        {
                            var stringActions = new UiBuilder.UiPropertyActions<string>
                            {
                                OnChange = (value) => UpdatePropertiesFromUiSystem.UpdateFloatingProperty(propertyIdentifier, value),
                                OnSubmit = (value) => UpdatePropertiesFromUiSystem.UpdateFixedProperty(propertyIdentifier, value),
                                OnAbort = (_) => UpdatePropertiesFromUiSystem.RevertFloatingProperty(propertyIdentifier)
                            };

                            componentBuilder.StringPropertyInput(
                                property.PropertyName, 
                                property.PropertyName,
                                property.GetConcrete<string>().Value,
                                stringActions);

                            break;
                        }
                    case DclComponent.DclComponentProperty.PropertyType.Int:
                        {
                            var intActions = new UiBuilder.UiPropertyActions<float> // number property requires float actions
                            {
                                OnChange = (value) => UpdatePropertiesFromUiSystem.UpdateFloatingProperty(propertyIdentifier, (int)value),
                                OnSubmit = (value) => UpdatePropertiesFromUiSystem.UpdateFixedProperty(propertyIdentifier, (int)value),
                                OnAbort = (value) => UpdatePropertiesFromUiSystem.RevertFloatingProperty(propertyIdentifier)
                            };

                            componentBuilder.NumberPropertyInput(
                                property.PropertyName,
                                property.PropertyName,
                                property.GetConcrete<int>().Value,
                                intActions);

                            break;
                        }
                    case DclComponent.DclComponentProperty.PropertyType.Float:
                        {
                            var floatActions = new UiBuilder.UiPropertyActions<float>
                            {
                                OnChange = (value)=> UpdatePropertiesFromUiSystem.UpdateFloatingProperty(propertyIdentifier, value),
                                OnSubmit = (value)=> UpdatePropertiesFromUiSystem.UpdateFixedProperty(propertyIdentifier, value),
                                OnAbort = (value) => UpdatePropertiesFromUiSystem.RevertFloatingProperty(propertyIdentifier)
                            };
                        
                            componentBuilder.NumberPropertyInput(
                                property.PropertyName, 
                                property.PropertyName,
                                property.GetConcrete<float>().Value,
                                floatActions);

                            break;
                        }
                    case DclComponent.DclComponentProperty.PropertyType.Boolean: // not supported yet
                        {
                            componentBuilder.Text("Boolean property not supported yet");
                            break;
                        }
                    case DclComponent.DclComponentProperty.PropertyType.Vector3:
                        {
                            var vec3Actions = new UiBuilder.UiPropertyActions<Vector3>
                            {
                                OnChange = (value) => UpdatePropertiesFromUiSystem.UpdateFloatingProperty(propertyIdentifier, value),
                                OnSubmit = (value) => UpdatePropertiesFromUiSystem.UpdateFixedProperty(propertyIdentifier, value),
                                OnAbort = (value) => UpdatePropertiesFromUiSystem.RevertFloatingProperty(propertyIdentifier)
                            };

                            string[] xyzString = { "x", "y", "z" };
                            componentBuilder.Vector3PropertyInput(
                                property.PropertyName, 
                                xyzString,
                                property.GetConcrete<Vector3>().Value,
                                vec3Actions);

                            break;
                        }
                    case DclComponent.DclComponentProperty.PropertyType.Quaternion: // not supported yet
                        {
                            componentBuilder.Text("Quaternion property not supported yet");
                            break;
                        }
                    case DclComponent.DclComponentProperty.PropertyType.Asset: // not supported yet
                        {
                            componentBuilder.Text("Asset property not supported yet");
                            break;
                        }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            inspectorBuilder.Panel(componentBuilder);
        }

        inspectorBuilder.ClearAndMake(_content);
    }
}
