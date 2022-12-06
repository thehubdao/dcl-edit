using System;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class UiInspectorVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        [SerializeField]
        private GameObject content;

        // Dependencies
        private InputState inputState;
        private UpdatePropertiesFromUiSystem updatePropertiesSystem;
        private UiBuilder.UiBuilder uiBuilder;
        private SceneDirectoryState sceneDirectoryState;
        private EditorEvents editorEvents;

        [Inject]
        private void Construct(
            InputState inputState,
            UpdatePropertiesFromUiSystem updatePropertiesSystem,
            UiBuilder.UiBuilder.Factory uiBuilderFactory,
            SceneDirectoryState sceneDirectoryState,
            EditorEvents editorEvents)
        {
            this.inputState = inputState;
            this.updatePropertiesSystem = updatePropertiesSystem;
            uiBuilder = uiBuilderFactory.Create(content);
            this.sceneDirectoryState = sceneDirectoryState;
            this.editorEvents = editorEvents;
        }

        public void SetupSceneEventListeners()
        {
            editorEvents.onSelectionChangedEvent += UpdateVisuals;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            if (inputState.InState == InputState.InStateType.UiInput)
            {
                return;
            }

            var inspectorPanel = new PanelAtom.Data();

            var selectedEntity = sceneDirectoryState.CurrentScene?.SelectionState.PrimarySelectedEntity;

            if (selectedEntity == null)
            {
                inspectorPanel.AddTitle("No Entity selected");

                uiBuilder.Update(inspectorPanel);
                return;
            }

            var nameInputActions = new StringPropertyAtom.UiPropertyActions<string>
            {
                OnChange = _ => { },
                OnSubmit = value => updatePropertiesSystem.SetNewName(selectedEntity, value),
                OnAbort = _ => { }
            };

            var exposedInputActions = new StringPropertyAtom.UiPropertyActions<bool>
            {
                OnChange = _ => { },
                OnSubmit = value => updatePropertiesSystem.SetIsExposed(selectedEntity, value),
                OnAbort = _ => { }
            };

            var entityHeadPanel = inspectorPanel.AddPanelWithBorder();
            entityHeadPanel.AddStringProperty("Name", "Name", selectedEntity.CustomName ?? "", nameInputActions);
            entityHeadPanel.AddBooleanProperty("Is Exposed", selectedEntity.IsExposed, exposedInputActions);

            foreach (var component in selectedEntity.Components ?? new List<DclComponent>())
            {
                var componentPanel = inspectorPanel.AddPanelWithBorder();
                componentPanel.AddPanelHeader(component.NameInCode, null);

                foreach (var property in component.Properties)
                {
                    var propertyIdentifier = new DclPropertyIdentifier(selectedEntity.Id, component.NameInCode, property.PropertyName);
                    switch (property.Type)
                    {
                        case DclComponent.DclComponentProperty.PropertyType.None: // not supported
                            componentPanel.AddText("None property not supported");
                            break;
                        case DclComponent.DclComponentProperty.PropertyType.String:
                        {
                            var stringActions = new StringPropertyAtom.UiPropertyActions<string>
                            {
                                OnChange = (value) => updatePropertiesSystem.UpdateFloatingProperty(propertyIdentifier, value),
                                OnSubmit = (value) => updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, value),
                                OnAbort = (_) => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
                            };

                            componentPanel.AddStringProperty(
                                property.PropertyName,
                                property.PropertyName,
                                property.GetConcrete<string>().Value,
                                stringActions);

                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Int:
                        {
                            var intActions = new StringPropertyAtom.UiPropertyActions<float> // number property requires float actions
                            {
                                OnChange = (value) => updatePropertiesSystem.UpdateFloatingProperty(propertyIdentifier, (int) value),
                                OnSubmit = (value) => updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, (int) value),
                                OnAbort = (value) => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
                            };

                            componentPanel.AddNumberProperty(
                                property.PropertyName,
                                property.PropertyName,
                                property.GetConcrete<int>().Value,
                                intActions);

                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Float:
                        {
                            var floatActions = new StringPropertyAtom.UiPropertyActions<float>
                            {
                                OnChange = (value) => updatePropertiesSystem.UpdateFloatingProperty(propertyIdentifier, value),
                                OnSubmit = (value) => updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, value),
                                OnAbort = (value) => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
                            };

                            componentPanel.AddNumberProperty(
                                property.PropertyName,
                                property.PropertyName,
                                property.GetConcrete<float>().Value,
                                floatActions);

                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Boolean: // not supported yet
                        {
                            componentPanel.AddText("Boolean property not supported yet");
                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Vector3:
                        {
                            var vec3Actions = new StringPropertyAtom.UiPropertyActions<Vector3>
                            {
                                OnChange = (value) => updatePropertiesSystem.UpdateFloatingProperty(propertyIdentifier, value),
                                OnSubmit = (value) => updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, value),
                                OnAbort = (value) => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
                            };

                            componentPanel.AddVector3Property(
                                property.PropertyName,
                                new List<string> {"x", "y", "z"},
                                property.GetConcrete<Vector3>().Value,
                                vec3Actions);

                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Quaternion: // Shows quaternions in euler angles
                        {
                            var vec3Actions = new StringPropertyAtom.UiPropertyActions<Vector3>
                            {
                                OnChange = (value) => updatePropertiesSystem.UpdateFloatingProperty(propertyIdentifier, Quaternion.Euler(value)),
                                OnSubmit = (value) => updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, Quaternion.Euler(value)),
                                OnAbort = (value) => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
                            };

                            componentPanel.AddVector3Property(
                                property.PropertyName,
                                new List<string> {"pitch", "yaw", "roll"},
                                property.GetConcrete<Quaternion>().Value.eulerAngles,
                                vec3Actions);

                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Asset: // not supported yet
                        {
                            componentPanel.AddText("Asset property not supported yet");
                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            uiBuilder.Update(inspectorPanel);
        }
    }
}
