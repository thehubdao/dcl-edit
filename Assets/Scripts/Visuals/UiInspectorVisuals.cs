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
    public class UiInspectorVisuals : MonoBehaviour
    {
        [SerializeField]
        private GameObject content;

        // Dependencies
        private InputState inputState;
        private UpdatePropertiesFromUiSystem updatePropertiesSystem;
        private UiBuilder.UiBuilder uiBuilder;
        private EditorEvents editorEvents;
        private CommandSystem commandSystem;
        private SceneManagerSystem sceneManagerSystem;
        private ContextMenuSystem contextMenuSystem;
        private AddComponentSystem addComponentSystem;

        [Inject]
        private void Construct(
            InputState inputState,
            UpdatePropertiesFromUiSystem updatePropertiesSystem,
            UiBuilder.UiBuilder.Factory uiBuilderFactory,
            EditorEvents editorEvents,
            CommandSystem commandSystem,
            SceneManagerSystem sceneManagerSystem,
            ContextMenuSystem contextMenuSystem,
            AddComponentSystem addComponentSystem)
        {
            this.inputState = inputState;
            this.updatePropertiesSystem = updatePropertiesSystem;
            this.uiBuilder = uiBuilderFactory.Create(content);
            this.editorEvents = editorEvents;
            this.commandSystem = commandSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.contextMenuSystem = contextMenuSystem;
            this.addComponentSystem = addComponentSystem;

            SetupEventListeners();
        }

        public void SetupEventListeners()
        {
            editorEvents.onSelectionChangedEvent += SetDirty;
            SetDirty();
        }


        private bool _dirty;

        void SetDirty()
        {
            _dirty = true;
        }

        void LateUpdate()
        {
            if (_dirty)
            {
                _dirty = false;
                UpdateVisuals();
            }
        }

        private void UpdateVisuals()
        {
            if (inputState.InState == InputState.InStateType.UiInput)
            {
                return;
            }

            var inspectorPanel = new PanelAtom.Data();

            var currentScene = sceneManagerSystem.GetCurrentScene();

            if (currentScene == null)
            {
                inspectorPanel.AddTitle("No Scene loaded");

                uiBuilder.Update(inspectorPanel);
                return;
            }

            var selectedEntity = currentScene.SelectionState.PrimarySelectedEntity;

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
                
                if (component.NameInCode == "Transform")
                    componentPanel.AddPanelHeader(component.NameInCode, null);
                else
                    componentPanel.AddPanelHeader(component.NameInCode, () => commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateRemoveComponent(selectedEntity.Id, component)));

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
                                OnInvalid = () => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier),
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
                                OnInvalid = () => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier),
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
                                OnInvalid = () => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier),
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
                                OnInvalid = () => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier),
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

            inspectorPanel.AddSpacer(50);

            inspectorPanel.AddButton("Add Component", go =>
            {
                var rect = go.GetComponent<RectTransform>();

                var menuItems = new List<ContextMenuItem>();

                foreach (var component in addComponentSystem.GetAvailableComponents())
                {
                    menuItems.Add(new ContextMenuTextItem(component.NameInCode, () => addComponentSystem.AddComponent(selectedEntity.Id, component)));
                }

                contextMenuSystem.OpenMenu(new List<ContextMenuState.Placement>
                {
                    new ContextMenuState.Placement
                    {
                        position = rect.position + new Vector3(0, -rect.sizeDelta.y, 0),
                        expandDirection = ContextMenuState.Placement.Direction.Right,
                    },
                    new ContextMenuState.Placement
                    {
                        position = rect.position + new Vector3(rect.sizeDelta.x, -rect.sizeDelta.y, 0),
                        expandDirection = ContextMenuState.Placement.Direction.Left,
                    }
                }, menuItems);
            });

            inspectorPanel.AddSpacer(50);

            uiBuilder.Update(inspectorPanel);
        }
    }
}
