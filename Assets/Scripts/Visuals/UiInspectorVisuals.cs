using System;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;
using static Assets.Scripts.Events.EventDependentTypes;

namespace Assets.Scripts.Visuals
{
    public class UiInspectorVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        [SerializeField]
        private GameObject _content;

        // Dependencies
        private InputState _inputState;
        private UpdatePropertiesFromUiSystem _updatePropertiesSystem;
        private UiBuilderSystem.UiBuilder.Factory _uiBuilderFactory;
        private UiBuilderVisuals.Factory _uiBuilderVisualsFactory;
        private SceneDirectoryState _sceneDirectoryState;
        private EditorEvents _editorEvents;

        [Inject]
        private void Construct(
            InputState inputState,
            UpdatePropertiesFromUiSystem updatePropertiesSystem,
            UiBuilderSystem.UiBuilder.Factory uiBuilderFactory,
            UiBuilderVisuals.Factory uiBuilderVisualsFactory,
            SceneDirectoryState sceneDirectoryState,
            EditorEvents editorEvents)
        {
            _inputState = inputState;
            _updatePropertiesSystem = updatePropertiesSystem;
            _uiBuilderFactory = uiBuilderFactory;
            _uiBuilderVisualsFactory = uiBuilderVisualsFactory;
            _sceneDirectoryState = sceneDirectoryState;
            _editorEvents = editorEvents;


            _uiBuilderVisualsFactory.Create(new UiBuilderSetupKey{Id = UiBuilderSetupKey.UiBuilderId.Inspector}, _content);
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

            var inspectorBuilder = _uiBuilderFactory.Create(new UiBuilderSetupKey { Id = UiBuilderSetupKey.UiBuilderId.Inspector });

            var selectedEntity = _sceneDirectoryState.CurrentScene?.SelectionState.PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                inspectorBuilder
                    .Title("No Entity selected")
                    .Done();
                

                return;
            }

            var nameInputActions = new UiBuilderAtom.UiPropertyActions<string>
            {
                OnChange = _ => { },
                OnSubmit = value => _updatePropertiesSystem.SetNewName(selectedEntity, value),
                OnAbort = _ => { }
            };

            var exposedInputActions = new UiBuilderAtom.UiPropertyActions<bool>
            {
                OnChange = _ => { },
                OnSubmit = value => _updatePropertiesSystem.SetIsExposed(selectedEntity, value),
                OnAbort = _ => { }
            };


            var entityHeadBuilder = _uiBuilderFactory.Create(new UiBuilderSetupKey { Id = UiBuilderSetupKey.UiBuilderId.Inspector, SubId = "0-Entity-Head"})
                .StringPropertyInput("Name", "Name", selectedEntity.CustomName ?? "", nameInputActions)
                .BooleanPropertyInput("Is Exposed", selectedEntity.IsExposed, exposedInputActions);

            entityHeadBuilder.Done();

            inspectorBuilder.Panel(entityHeadBuilder.key);

            foreach (var component in selectedEntity.Components ?? new List<DclComponent>())
            {
                var componentBuilder = _uiBuilderFactory.Create(new UiBuilderSetupKey { Id = UiBuilderSetupKey.UiBuilderId.Inspector, SubId = $"Component-{component.NameInCode}" })
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
                            var stringActions = new UiBuilderAtom.UiPropertyActions<string>
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
                            var intActions = new UiBuilderAtom.UiPropertyActions<float> // number property requires float actions
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
                            var floatActions = new UiBuilderAtom.UiPropertyActions<float>
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
                            var vec3Actions = new UiBuilderAtom.UiPropertyActions<Vector3>
                            {
                                OnChange = (value) => _updatePropertiesSystem.UpdateFloatingProperty(propertyIdentifier, value),
                                OnSubmit = (value) => _updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, value),
                                OnAbort = (value) => _updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
                            };

                            string[] xyzString = {"x", "y", "z"};
                            componentBuilder.Vector3PropertyInput(
                                property.PropertyName,
                                xyzString,
                                property.GetConcrete<Vector3>().Value,
                                vec3Actions);

                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Quaternion: // Shows quaternions in euler angles
                        {
                            var vec3Actions = new UiBuilderAtom.UiPropertyActions<Vector3>
                            {
                                OnChange = (value) => _updatePropertiesSystem.UpdateFloatingProperty(propertyIdentifier, Quaternion.Euler(value)),
                                OnSubmit = (value) => _updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, Quaternion.Euler(value)),
                                OnAbort = (value) => _updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
                            };

                            string[] pyrString = {"pitch", "yaw", "roll"};
                            componentBuilder.Vector3PropertyInput(
                                property.PropertyName,
                                pyrString,
                                property.GetConcrete<Quaternion>().Value.eulerAngles,
                                vec3Actions);

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
                componentBuilder.Done();

                inspectorBuilder.Panel(componentBuilder.key);
            }

            inspectorBuilder.Done();
        }
    }
}
