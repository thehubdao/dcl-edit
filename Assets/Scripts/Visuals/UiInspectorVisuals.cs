using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        private UiBuilder.UiBuilder uiBuilder;
        private EditorEvents editorEvents;
        private CommandSystem commandSystem;
        private SceneManagerSystem sceneManagerSystem;
        private ContextMenuSystem contextMenuSystem;
        private AddComponentSystem addComponentSystem;
        private AvailableComponentsState availableComponentsState;
        private AssetManagerSystem assetManagerSystem;
        private DialogSystem dialogSystem;
        private EntityChangeManager entityChangeManager;
        private ExposeEntitySystem exposeEntitySystem;
        private PromptSystem promptSystem;

        [Inject]
        private void Construct(
            InputState inputState,
            UiBuilder.UiBuilder.Factory uiBuilderFactory,
            EditorEvents editorEvents,
            CommandSystem commandSystem,
            SceneManagerSystem sceneManagerSystem,
            ContextMenuSystem contextMenuSystem,
            AddComponentSystem addComponentSystem,
            AvailableComponentsState availableComponentsState,
            AssetManagerSystem assetManagerSystem,
            DialogSystem dialogSystem,
            EntityChangeManager entityChangeManager,
            ExposeEntitySystem exposeEntitySystem,
            PromptSystem promptSystem)
        {
            this.inputState = inputState;
            this.uiBuilder = uiBuilderFactory.Create(content);
            this.editorEvents = editorEvents;
            this.commandSystem = commandSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.contextMenuSystem = contextMenuSystem;
            this.addComponentSystem = addComponentSystem;
            this.availableComponentsState = availableComponentsState;
            this.assetManagerSystem = assetManagerSystem;
            this.dialogSystem = dialogSystem;
            this.entityChangeManager = entityChangeManager;
            this.exposeEntitySystem = exposeEntitySystem;
            this.promptSystem = promptSystem;

            SetupEventListeners();
        }

        public void SetupEventListeners()
        {
            editorEvents.onSelectionChangedEvent += SetDirty;
            editorEvents.onValueChangedEvent += UpdateValues;
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

        private void UpdateValues()
        {
            uiBuilder.UpdateValues();
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

            var entityHeadPanel = inspectorPanel.AddPanelWithBorder();
            entityHeadPanel.AddStringProperty("Name", "Name", entityChangeManager.GetNameFieldBinding(selectedEntity.Id));
            entityHeadPanel.AddBooleanProperty("Is Exposed", entityChangeManager.GetIsExposedBinding(selectedEntity.Id));

            if (selectedEntity.IsExposed)
            {
                entityHeadPanel.AddText(entityChangeManager.GetExposedNameStrategy(selectedEntity.Id));
            }

            foreach (var component in selectedEntity.Components ?? new List<DclComponent>())
            {
                var componentPanel = inspectorPanel.AddPanelWithBorder();

                if (component.NameInCode == "Transform")
                    componentPanel.AddPanelHeader(component.NameInCode, null);
                else
                    componentPanel.AddPanelHeader(component.NameInCode, clickCloseStrategy:
                        new LeftClickStrategy(_ => commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateRemoveComponent(selectedEntity.Id, component))));

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
                                entityChangeManager.GetPropertyBinding<string>(propertyIdentifier));
                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Int:
                        {
                            componentPanel.AddNumberProperty(
                                property.PropertyName,
                                property.PropertyName,
                                entityChangeManager.GetPropertyBinding<int>(propertyIdentifier));
                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Float:
                        {
                            componentPanel.AddNumberProperty(
                                property.PropertyName,
                                property.PropertyName,
                                entityChangeManager.GetPropertyBinding<float>(propertyIdentifier));
                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Boolean:
                        {
                            componentPanel.AddBooleanProperty(
                                property.PropertyName,
                                entityChangeManager.GetPropertyBinding<bool>(propertyIdentifier));
                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Vector3:
                        {
                            componentPanel.AddVector3Property(
                                property.PropertyName,
                                ("x", "y", "z"),
                                entityChangeManager.GetPropertyBinding<Vector3>(propertyIdentifier));
                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Quaternion: // Shows quaternions in euler angles
                        {
                            // TODO Make a rotation property based on a Quaternion
                            var concreteProperty = sceneManagerSystem.GetCurrentScene().GetPropertyFromIdentifier(propertyIdentifier).GetConcrete<Quaternion>();
                            var valueBindStrategy = new ValueBindStrategy<Vector3>(
                                value: () => concreteProperty.Value.eulerAngles,
                                onValueSubmitted: value =>
                                {
                                    concreteProperty.ResetFloating();
                                    var oldValue = concreteProperty.Value;
                                    var command = commandSystem.CommandFactory.CreateChangePropertyCommand(propertyIdentifier, oldValue, Quaternion.Euler(value));

                                    commandSystem.ExecuteCommand(command);
                                },
                                onErrorSubmitted: _ => concreteProperty.ResetFloating(),
                                onValueChanged: value => concreteProperty.SetFloatingValue(Quaternion.Euler(value)),
                                onErrorChanged: _ => concreteProperty.ResetFloating());

                            componentPanel.AddVector3Property(
                                property.PropertyName,
                                ("pitch", "yaw", "roll"),
                                valueBindStrategy);

                            break;
                        }
                        case DclComponent.DclComponentProperty.PropertyType.Asset:
                        {
                            componentPanel.AddAssetProperty(
                                property.PropertyName,
                                new SetValueStrategy<Guid>(() => property.GetConcrete<Guid>().Value),
                                entityChangeManager.GetClickAssetStrategy(propertyIdentifier));
                            // TODO: Add drop strategy
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
