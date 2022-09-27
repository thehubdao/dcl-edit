using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class UiInspectorVisuals : MonoBehaviour, ISetupSceneEventListeners
{
    [SerializeField]
    private GameObject _content;

    // Dependencies
    private InputState _inputState;
    private UpdatePropertiesFromUiSystem _updatePropertiesSystem;
    private UiBuilder.Factory _uiBuilderFactory;
    private SceneState _sceneState;
    private EditorEvents _editorEvents;

    [Inject]
    private void Construct(
        InputState inputState,
        UpdatePropertiesFromUiSystem updatePropertiesSystem,
        UiBuilder.Factory uiBuilderFactory,
        SceneState sceneState,
        EditorEvents editorEvents)
    {
        _inputState = inputState;
        _updatePropertiesSystem = updatePropertiesSystem;
        _uiBuilderFactory = uiBuilderFactory;
        _sceneState = sceneState;
        _editorEvents = editorEvents;
    }

    public void SetupSceneEventListeners()
    {
        _editorEvents.onSelectionChangedEvent += UpdateVisuals;
        UpdateVisuals();
    }

    private void UpdateVisuals()
    {
        if (_inputState.InState == InputState.InStateType.UiInput)
        {
            return;
        }

        var inspectorBuilder = _uiBuilderFactory.Create();

        var selectedEntity = _sceneState.CurrentScene?.SelectionState.PrimarySelectedEntity;

        if (selectedEntity == null)
        {
            inspectorBuilder
                .Title("No Entity selected")
                .ClearAndMake(_content);
            return;
        }

        var nameInputActions = new UiBuilder.UiPropertyActions<string>
        {
            OnChange = _ => { },
            OnSubmit = value => _updatePropertiesSystem.SetNewName(selectedEntity, value),
            OnAbort = _ => { }
        };

        var exposedInputActions = new UiBuilder.UiPropertyActions<bool>
        {
            OnChange = _ => { },
            OnSubmit = value => _updatePropertiesSystem.SetIsExposed(selectedEntity, value),
            OnAbort = _ => { }
        };


        var entityHeadBuilder = _uiBuilderFactory.Create()
            .StringPropertyInput("Name", "Name", selectedEntity.CustomName ?? "", nameInputActions)
            .BooleanPropertyInput("Is Exposed", selectedEntity.IsExposed, exposedInputActions);

        inspectorBuilder.Panel(entityHeadBuilder);

        foreach (var component in selectedEntity.Components ?? new List<DclComponent>())
        {
            var componentBuilder = _uiBuilderFactory.Create()
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
                                OnChange = (value) => _updatePropertiesSystem.UpdateFloatingProperty(propertyIdentifier, value),
                                OnSubmit = (value) => _updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, value),
                                OnAbort = (_) => _updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
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
                                OnChange = (value) => _updatePropertiesSystem.UpdateFloatingProperty(propertyIdentifier, (int) value),
                                OnSubmit = (value) => _updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, (int) value),
                                OnAbort = (value) => _updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
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
                                OnChange = (value) => _updatePropertiesSystem.UpdateFloatingProperty(propertyIdentifier, value),
                                OnSubmit = (value) => _updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, value),
                                OnAbort = (value) => _updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
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
                                OnChange = (value) => _updatePropertiesSystem.UpdateFloatingProperty(propertyIdentifier, value),
                                OnSubmit = (value) => _updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, value),
                                OnAbort = (value) => _updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
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
