using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using JetBrains.Annotations;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Visuals.UiHandler;
using Zenject;
using static Assets.Scripts.Visuals.UiBuilder.UiBuilder;

namespace Assets.Scripts.Visuals
{
    public class UiHierarchyVisuals : MonoBehaviour
    {
#pragma warning disable CS0649 // Warning: Uninitialized filed. Serialized fields will be initialized by Unity

        [SerializeField] private GameObject content;

        [SerializeField] private HierarchyViewportHandler hierarchyViewportHandler;

#pragma warning restore CS0649

        #region Mark for update

        private bool shouldUpdate = false;

        void LateUpdate()
        {
            if (shouldUpdate)
            {
                UpdateVisuals();
                shouldUpdate = false;
            }
        }

        private void MarkForUpdate()
        {
            shouldUpdate = true;
        }

        #endregion

        // Dependencies
        private EditorEvents events;
        private UiBuilder.UiBuilder uiBuilder;
        private HierarchyChangeSystem hierarchyChangeSystem;
        private ContextMenuSystem contextMenuSystem;
        private HierarchyContextMenuSystem hierarchyContextMenuSystem;
        private SceneManagerSystem sceneManagerSystem;
        private CommandSystem commandSystem;
        private HierarchyOrderSystem hierarchyOrderSystem;
        private AddEntitySystem addEntitySystem;

        [Inject]
        private void Construct(
            EditorEvents events,
            Factory uiBuilderFactory,
            HierarchyChangeSystem hierarchyChangeSystem,
            ContextMenuSystem contextMenuSystem,
            SceneManagerSystem sceneManagerSystem,
            CommandSystem commandSystem,
            HierarchyContextMenuSystem hierarchyContextMenuSystem,
            HierarchyOrderSystem hierarchyOrderSystem,
            AddEntitySystem addEntitySystem)
        {
            this.events = events;
            this.uiBuilder = uiBuilderFactory.Create(content);
            this.hierarchyChangeSystem = hierarchyChangeSystem;
            this.contextMenuSystem = contextMenuSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.commandSystem = commandSystem;
            this.hierarchyContextMenuSystem = hierarchyContextMenuSystem;
            this.hierarchyOrderSystem = hierarchyOrderSystem;
            this.addEntitySystem = addEntitySystem;


            SetupRightClickHandler();
            SetupEventListeners();
        }

        private void SetupEventListeners()
        {
            events.onHierarchyChangedEvent += MarkForUpdate;
            events.onSelectionChangedEvent += MarkForUpdate;
            MarkForUpdate();
        }

        private void SetupRightClickHandler()
        {
            hierarchyViewportHandler.rightClickHandler.onRightClick = clickPosition =>
            {
                var addEntityMenuItems = new List<ContextMenuItem>();

                foreach (var preset in hierarchyContextMenuSystem.GetPresets())
                {
                    addEntityMenuItems.Add(new ContextMenuTextItem(preset.name,
                        () => hierarchyContextMenuSystem.AddEntityFromPreset(preset)));
                }

                contextMenuSystem.OpenMenu(clickPosition, new List<ContextMenuItem>
                {
                    new ContextSubmenuItem("Add entity...", addEntityMenuItems),
                });
            };
        }

        private void UpdateVisuals()
        {
            var mainPanelData = NewPanelData();

            var scene = sceneManagerSystem.GetCurrentSceneOrNull();

            if (scene == null)
            {
                mainPanelData.AddTitle("No scene loaded");
            }
            else
            {
                var entitiesInRoot = scene.EntitiesInSceneRoot;
                entitiesInRoot = entitiesInRoot.OrderBy(entity => entity.hierarchyOrder);

                mainPanelData.AddSpacer(1);
                
                MakeHierarchyItemsRecursive(scene, 0, entitiesInRoot, mainPanelData);

                mainPanelData.AddSpacer(300, clickPosition =>
                {
                    var addEntityMenuItems = new List<ContextMenuItem>();

                    foreach (var preset in hierarchyContextMenuSystem.GetPresets())
                    {
                        addEntityMenuItems.Add(new ContextMenuTextItem(preset.name,
                            () => hierarchyContextMenuSystem.AddEntityFromPreset(preset)));
                    }

                    contextMenuSystem.OpenMenu(clickPosition, new List<ContextMenuItem>
                    {
                        new ContextSubmenuItem("Add entity...", addEntityMenuItems),
                    });
                }, draggedGameObject =>
                {
                    var draggedEntity = draggedGameObject.GetComponent<DragAndDropHandler>().draggedEntity;
                    hierarchyOrderSystem.DropSpacer(draggedEntity);
                });
            }

            uiBuilder.Update(mainPanelData);
        }

        private void MakeHierarchyItemsRecursive([NotNull] DclScene scene, int level, IEnumerable<DclEntity> entities,
            PanelAtom.Data mainPanelData)
        {
            var dclEntities = entities.ToList();

            foreach (var entity in dclEntities)
            {
                Assert.IsNotNull(entity);
                
                var isPrimarySelection = scene.SelectionState.PrimarySelectedEntity == entity;

                var isSecondarySelection = scene.SelectionState.SecondarySelectedEntities.Contains(entity);

                var style =
                    isPrimarySelection ? TextHandler.TextStyle.PrimarySelection :
                    isSecondarySelection ? TextHandler.TextStyle.SecondarySelection :
                    TextHandler.TextStyle.Normal;
                
                if (!entity.Children.Any())
                {
                    hierarchyChangeSystem.SetExpanded(entity, false);
                }
                
                var isExpanded = hierarchyChangeSystem.IsExpanded(entity);
                var isParentExpanded = entity.Parent != null && hierarchyChangeSystem.IsExpanded(entity.Parent);

                mainPanelData.AddHierarchyItem(entity.ShownName, level, entity.Children.Any(), isExpanded, isParentExpanded, style,
                    new HierarchyItemHandler.UiHierarchyItemActions
                    {
                        onArrowClick = () => { hierarchyChangeSystem.ClickedOnEntityExpandArrow(entity); },
                        onNameClick = () => { hierarchyChangeSystem.ClickedOnEntityInHierarchy(entity); }
                    },
                    clickPosition =>
                    {
                        var addEntityMenuItems = new List<ContextMenuItem>();

                        foreach (var preset in hierarchyContextMenuSystem.GetPresets())
                        {
                            addEntityMenuItems.Add(new ContextMenuTextItem(preset.name,
                                () => hierarchyContextMenuSystem.AddEntityFromPreset(preset, entity.Id)));
                        }

                        contextMenuSystem.OpenMenu(clickPosition, new List<ContextMenuItem>
                        {
                            new ContextSubmenuItem("Add entity...", addEntityMenuItems),
                            new ContextMenuTextItem("Duplicate",
                                () => addEntitySystem.DuplicateEntityAsCommand(entity)),
                            new ContextMenuTextItem("Delete",
                                () => commandSystem.ExecuteCommand(
                                    commandSystem.CommandFactory.CreateRemoveEntity(entity)))
                        });
                    },
                    draggedGameObject =>
                    {
                        var draggedEntity = draggedGameObject.GetComponent<DragAndDropHandler>().draggedEntity;
                        var aboveEntity = hierarchyOrderSystem.GetAboveSibling(entity);
                        
                        hierarchyOrderSystem.DropUpper(draggedEntity, entity, aboveEntity);
                    },
                    draggedGameObject =>
                    {
                        var draggedEntity = draggedGameObject.GetComponent<DragAndDropHandler>().draggedEntity;
                        
                        hierarchyOrderSystem.DropMiddle(draggedEntity, entity);
                    },
                    draggedGameObject =>
                    {
                        var draggedEntity = draggedGameObject.GetComponent<DragAndDropHandler>().draggedEntity;
                        var belowEntity = hierarchyOrderSystem.GetBelowSibling(entity);
                        var firstChildOfHoveredEntity = entity.Children.OrderBy(e => e.hierarchyOrder).FirstOrDefault();
                        
                        hierarchyOrderSystem.DropLower(draggedEntity, entity, belowEntity, firstChildOfHoveredEntity, isExpanded);
                    },
                    entity);

                if (isExpanded)
                {
                    var sortedChildren = entity.Children.OrderBy(entity => entity.hierarchyOrder);
                    MakeHierarchyItemsRecursive(scene, level + 1, sortedChildren, mainPanelData);
                }
            }
        }
    }
}