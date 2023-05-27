using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Utility;
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
        private AvailableComponentsState availableComponentsState;
        private AssetManagerSystem assetManagerSystem;
        private DialogSystem dialogSystem;
        private EntityNameChangeManager entityNameChangeManager;
        private PropertyBindingManager propertyBindingManager;

        [Inject]
        private void Construct(
            InputState inputState,
            UpdatePropertiesFromUiSystem updatePropertiesSystem,
            UiBuilder.UiBuilder.Factory uiBuilderFactory,
            EditorEvents editorEvents,
            CommandSystem commandSystem,
            SceneManagerSystem sceneManagerSystem,
            ContextMenuSystem contextMenuSystem,
            AddComponentSystem addComponentSystem,
            AvailableComponentsState availableComponentsState,
            AssetManagerSystem assetManagerSystem,
            DialogSystem dialogSystem,
            EntityNameChangeManager entityNameChangeManager,
            PropertyBindingManager propertyBindingManager)
        {
            this.inputState = inputState;
            this.updatePropertiesSystem = updatePropertiesSystem;
            this.uiBuilder = uiBuilderFactory.Create(content);
            this.editorEvents = editorEvents;
            this.commandSystem = commandSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.contextMenuSystem = contextMenuSystem;
            this.addComponentSystem = addComponentSystem;
            this.availableComponentsState = availableComponentsState;
            this.assetManagerSystem = assetManagerSystem;
            this.dialogSystem = dialogSystem;
            this.entityNameChangeManager = entityNameChangeManager;
            this.propertyBindingManager = propertyBindingManager;

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

            var currentScene = sceneManagerSystem.GetCurrentSceneOrNull();

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
            entityHeadPanel.AddStringProperty("Name", "Name", entityNameChangeManager.GetNameFieldBinding(selectedEntity.Id));
            entityHeadPanel.AddBooleanProperty("Is Exposed", selectedEntity.IsExposed, exposedInputActions);
            
            if (selectedEntity.IsExposed)
            {
                entityHeadPanel.AddText(entityNameChangeManager.GetExposedNameForInspector(selectedEntity.Id));
            }

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
                            componentPanel.AddStringProperty(
                                property.PropertyName,
                                property.PropertyName,
                                propertyBindingManager.GetPropertyBinding<string>(propertyIdentifier));

                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Int:
                        {
                            var intActions = new StringPropertyAtom.UiPropertyActions<float> // number property requires float actions
                            {
                                OnChange = (value) => updatePropertiesSystem.UpdateFloatingProperty(propertyIdentifier, (int) value),
                                OnInvalid = (_) => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier),
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
                                OnInvalid = (_) => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier),
                                OnSubmit = (value) => updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, value),
                                OnAbort = (_) => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
                            };

                            componentPanel.AddNumberProperty(
                                property.PropertyName,
                                property.PropertyName,
                                property.GetConcrete<float>().Value,
                                floatActions);

                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Boolean:
                        {
                            var boolActions = new StringPropertyAtom.UiPropertyActions<bool>
                            {
                                OnChange = (value) => updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, value),
                                OnSubmit = (value) => updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, value)
                                // OnInvalid, OnAbort not required for a checkbox
                            };

                            componentPanel.AddBooleanProperty(
                                property.PropertyName,
                                property.GetConcrete<bool>().Value,
                                boolActions);
                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Vector3:
                        {
                            var vec3Actions = new StringPropertyAtom.UiPropertyActions<Vector3>
                            {
                                OnChange = (value) => updatePropertiesSystem.UpdateFloatingProperty(propertyIdentifier, value),
                                OnInvalid = _ => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier),
                                OnSubmit = (value) => updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, value),
                                OnAbort = (_) => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
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
                                OnInvalid = _ => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier),
                                OnSubmit = (value) => updatePropertiesSystem.UpdateFixedProperty(propertyIdentifier, Quaternion.Euler(value)),
                                OnAbort = (_) => updatePropertiesSystem.RevertFloatingProperty(propertyIdentifier)
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
                            var assetMetadata = assetManagerSystem.GetMetadataById(property.GetConcrete<Guid>().Value);
                            componentPanel.AddAssetProperty(
                                property.PropertyName,
                                assetMetadata,
                                (_) => dialogSystem.OpenAssetDialog(component));

                            break;
                        }
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }

            inspectorPanel.AddSpacer(20);

            inspectorPanel.AddButton("Add Component", new LeftClickStrategy
                (eventData =>
                {
                    var rect = eventData.gameObject.GetComponent<RectTransform>();

                    var menuItemCategories = new Dictionary<string, List<ContextMenuItem>>
                    {
                        {"", new List<ContextMenuItem>()}
                    };

                    List<ContextMenuItem> GetCategoryList(string categoryPath)
                    {
                        // Try to get the category list
                        if (menuItemCategories.TryGetValue(categoryPath, out var categoryList)) return categoryList;

                        // if category list does NOT exist yet
                        // split category by '/'
                        var categoryParts = categoryPath.Split('/');

                        // Generate parent category name. Either by concatenating the previews path parts or "" for the root category
                        var parentCategoryPath =
                            categoryParts.Length > 1 ?
                                string.Join("/", categoryParts.Take(categoryParts.Length - 1)) :
                                "";

                        // recursively get the parent category list
                        var parentCategoryList = GetCategoryList(parentCategoryPath);

                        // Create new list
                        categoryList = new List<ContextMenuItem>();

                        // add the list to the parent category
                        parentCategoryList.Add(new ContextSubmenuItem(categoryParts[categoryParts.Length - 1], categoryList));

                        // add the list to the categories dictionary
                        menuItemCategories.Add(categoryPath, categoryList);

                        // return the list
                        return categoryList;
                    }

                    foreach (var component in availableComponentsState.allAvailableComponents.Where(c => c.availableInAddComponentMenu))
                    {
                        var categoryMenu = GetCategoryList(component.category);

                        categoryMenu.Add(
                            new ContextMenuTextItem(
                                component.name,
                                () => addComponentSystem.AddComponent(selectedEntity.Id, component.componentDefinition),
                                !addComponentSystem.CanComponentBeAdded(selectedEntity, component.componentDefinition)));
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
                    }, menuItemCategories[""]);
                })
            );

            inspectorPanel.AddSpacer(20);

            uiBuilder.Update(inspectorPanel);
        }
    }
}
