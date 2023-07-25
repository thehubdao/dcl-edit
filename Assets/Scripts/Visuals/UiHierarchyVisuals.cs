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
using UnityEngine.UI;
using UnityEngine.Assertions;
using Visuals.UiHandler;
using Zenject;
using static Assets.Scripts.Visuals.UiBuilder.UiBuilder;

namespace Assets.Scripts.Visuals
{
    public class UiHierarchyVisuals : MonoBehaviour
    {
#pragma warning disable CS0649 // Warning: Uninitialized filed. Serialized fields will be initialized by Unity

        [SerializeField]
        private GameObject content;

        [SerializeField]
        private HierarchyViewportHandler hierarchyViewportHandler;

        [SerializeField]
        private ScrollRect scrollRect;

#pragma warning restore CS0649

        private Vector3? prevScrollPosition;
        private const float uiItemHeight = 30f;

        #region Mark for update

        private bool shouldUpdate = false;

        void LateUpdate()
        {
            if (shouldUpdate)
            {
                UpdateVisuals();
                ScrollPanelToSelectedItem();
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
        private EntityChangeManager entityChangeManager;

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
            EntityChangeManager entityChangeManager)
        {
            this.events = events;
            this.uiBuilder = uiBuilderFactory.Create(content);
            this.hierarchyChangeSystem = hierarchyChangeSystem;
            this.contextMenuSystem = contextMenuSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.commandSystem = commandSystem;
            this.hierarchyContextMenuSystem = hierarchyContextMenuSystem;
            this.hierarchyOrderSystem = hierarchyOrderSystem;
            this.entityChangeManager = entityChangeManager;

            SetupRightClickHandler();
            SetupEventListeners();
        }

        private void SetupEventListeners()
        {
            events.onHierarchyChangedEvent += MarkForUpdate;
            events.onSelectionChangedEvent += MarkForUpdate;
            events.onValueChangedEvent += UpdateValues;
            MarkForUpdate();
        }

        private void SetupRightClickHandler()
        {
            hierarchyViewportHandler.clickHandler.clickStrategy = new RightClickStrategy
            (eventData =>
            {
                var addEntityMenuItems = new List<ContextMenuItem>();

                foreach (var preset in hierarchyContextMenuSystem.GetPresets())
                {
                    addEntityMenuItems.Add(new ContextMenuTextItem(preset.name,
                        () => hierarchyContextMenuSystem.AddEntityFromPreset(preset)));
                }

                contextMenuSystem.OpenMenu(eventData.position, new List<ContextMenuItem>
                {
                    new ContextSubmenuItem("Add entity...", addEntityMenuItems),
                });
            });
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
                if (scene.EntitiesInSceneRoot.Count() == 0)
                {
                    mainPanelData.AddTitle("No Entities.");
                    mainPanelData.AddText("Use right-click to add entities.");
                }
                else
                {
                    ExpandSelectedItem(scene);

                    var entitiesInRoot = scene.EntitiesInSceneRoot;
                    entitiesInRoot = entitiesInRoot.OrderBy(entity => entity.hierarchyOrder);

                    mainPanelData.AddSpacer(1);
                    MakeHierarchyItemsRecursive(scene, 0, entitiesInRoot, mainPanelData);
                }

                mainPanelData.AddSpacer(
                    300,
                    new RightClickStrategy(eventData =>
                    {
                        var addEntityMenuItems = new List<ContextMenuItem>();

                        foreach (var preset in hierarchyContextMenuSystem.GetPresets())
                        {
                            addEntityMenuItems.Add(new ContextMenuTextItem(preset.name,
                                () => hierarchyContextMenuSystem.AddEntityFromPreset(preset)));
                        }

                        contextMenuSystem.OpenMenu(eventData.position, new List<ContextMenuItem>
                        {
                            new ContextSubmenuItem("Add entity...", addEntityMenuItems),
                        });
                    }),
                    new DropEntityStrategy(entity => { hierarchyOrderSystem.DropSpacer(entity); }));
            }

            uiBuilder.Update(mainPanelData);
        }

        private void UpdateValues()
        {
            uiBuilder.UpdateValues();
        }

        private void MakeHierarchyItemsRecursive([NotNull] DclScene scene, int level, IEnumerable<DclEntity> entities,
            PanelAtom.Data mainPanelData)
        {
            var dclEntities = entities.ToList();

            foreach (var entity in dclEntities)
            {
                Assert.IsNotNull(entity);

                var selectionState = scene.SelectionState;
                var isPrimarySelection = selectionState.PrimarySelectedEntity == entity;

                var isSecondarySelection = selectionState.SecondarySelectedEntities.Contains(entity);

                var style =
                    isPrimarySelection ? TextHandler.TextStyle.PrimarySelection :
                    isSecondarySelection ? TextHandler.TextStyle.SecondarySelection :
                    TextHandler.TextStyle.Normal;

                if (!entity.Children.Any())
                {
                    hierarchyChangeSystem.SetExpanded(entity, false);
                }

                var isExpanded = hierarchyChangeSystem.IsExpanded(entity);
                var isFirstChild = entity.Parent != null && entity.Parent.Children.OrderBy(e => e.hierarchyOrder)
                    .First().Id.Equals(entity.Id);

                var (upperDropStrategy, middleDropStrategy, lowerDropStrategy) = CreateDropStrategies(entity);

                mainPanelData.AddHierarchyItem(
                    entityChangeManager.GetNameStrategy(entity.Id),
                    level,
                    entity.Children.Any(),
                    isExpanded,
                    isFirstChild,
                    style,
                    isPrimarySelection,
                    clickTextStrategy: new ClickStrategy(
                        leftClickStrategy: new LeftClickStrategy(_ =>
                        {
                            Debug.Log($"Clicked on: {entity.ShownName}");
                            hierarchyChangeSystem.ClickedOnEntityInHierarchy(entity);
                        }),
                        rightClickStrategy: CreateRightClickMenu(entity, scene)),
                    clickArrowStrategy: new LeftClickStrategy(_ => { hierarchyChangeSystem.ClickedOnEntityExpandArrow(entity); }),
                    dropStrategyUpper: upperDropStrategy,
                    dropStrategyMiddle: middleDropStrategy,
                    dropStrategyLower: lowerDropStrategy,
                    dragStrategy: new DragEntityStrategy(entity)
                );

                if (isExpanded)
                {
                    var sortedChildren = entity.Children.OrderBy(e => e.hierarchyOrder);
                    MakeHierarchyItemsRecursive(scene, level + 1, sortedChildren, mainPanelData);
                }
            }
        }

        private (DropStrategy, DropStrategy, DropStrategy) CreateDropStrategies(DclEntity entity)
        {
            var isExpanded = hierarchyChangeSystem.IsExpanded(entity);

            var upperDropStrategy = new DropEntityStrategy(draggedEntity =>
            {
                var aboveEntity = hierarchyOrderSystem.GetAboveSibling(entity);
                hierarchyOrderSystem.DropUpper(draggedEntity, entity, aboveEntity);
            });

            var middleDropStrategy = new DropEntityStrategy(draggedEntity => { hierarchyOrderSystem.DropMiddle(draggedEntity, entity); });

            var lowerDropStrategy = new DropEntityStrategy(draggedEntity =>
            {
                var belowEntity = hierarchyOrderSystem.GetBelowSibling(entity);
                var firstChildOfHoveredEntity = entity.Children.OrderBy(e => e.hierarchyOrder).FirstOrDefault();

                hierarchyOrderSystem.DropLower(draggedEntity, entity, belowEntity, firstChildOfHoveredEntity, isExpanded);
            });

            return (upperDropStrategy,middleDropStrategy, lowerDropStrategy);
        }

        private RightClickStrategy CreateRightClickMenu(DclEntity entity, DclScene scene)
        {
            var selectionState = scene.SelectionState;

            var isNothingOrHoveredEntitySelected = selectionState.PrimarySelectedEntity == null || selectionState.PrimarySelectedEntity.Id.Equals(entity.Id);

            var strategy = new RightClickStrategy
            (eventData =>
            {
                var addEntityMenuItems = new List<ContextMenuItem>();

                foreach (var preset in hierarchyContextMenuSystem.GetPresets())
                {
                    addEntityMenuItems.Add(new ContextMenuTextItem(preset.name,
                        () => hierarchyContextMenuSystem.AddEntityFromPreset(preset, entity.Id)));
                }

                var placeSelectedEntityMenuItems = new List<ContextMenuItem>()
                {
                    new ContextMenuTextItem("Place above",
                        () => hierarchyOrderSystem.PlaceAbove(entity), isNothingOrHoveredEntitySelected),
                    new ContextMenuTextItem("Place below",
                        () => hierarchyOrderSystem.PlaceBelow(entity), isNothingOrHoveredEntitySelected),
                    new ContextMenuTextItem("Place as Child",
                        () => hierarchyOrderSystem.PlaceAsChild(entity), isNothingOrHoveredEntitySelected)
                };

                var belowSibling = hierarchyOrderSystem.GetBelowSibling(entity);
                var newHierarchyOrderForDuplicatedEntity =
                    belowSibling != null ?
                        hierarchyOrderSystem.GetHierarchyOrderPlaceBetweenSiblings(entity, belowSibling) :
                        hierarchyOrderSystem.GetHierarchyOrderPlaceBeneathSibling(entity);


                contextMenuSystem.OpenMenu(eventData.position, new List<ContextMenuItem>
                {
                    new ContextSubmenuItem("Add entity...", addEntityMenuItems),
                    new ContextMenuTextItem("Duplicate",
                        () => commandSystem.ExecuteCommand(
                            commandSystem.CommandFactory.CreateDuplicateEntity(entity.Id, newHierarchyOrderForDuplicatedEntity))),
                    new ContextMenuTextItem("Delete",
                        () => commandSystem.ExecuteCommand(
                            commandSystem.CommandFactory.CreateRemoveEntity(entity))),
                    new ContextSubmenuItem("Selected entity...", placeSelectedEntityMenuItems)
                });
            });

            return strategy;
        }

        private void ExpandSelectedItem(DclScene scene)
        {
            var selectedEntity = scene.SelectionState.PrimarySelectedEntity;
            hierarchyChangeSystem.ExpandParents(selectedEntity);
        }

        private (HierarchyItemHandler selectedItem, int index) HierarchyPanelSelectedItem()
        {
            var list = content.GetComponentsInChildren<HierarchyItemHandler>();

            for (var i = 0; i < list.Length; i++)
            {
                if (list[i].primarySelection)
                    return (list[i], i);
            }

            return (null, 0);
        }

        private void ScrollPanelToSelectedItem()
        {
            var (selectedUiItem, index) = HierarchyPanelSelectedItem();
            if (selectedUiItem == null)
            {
                if (!prevScrollPosition.HasValue) return;

                scrollRect.content.localPosition = prevScrollPosition.Value;
                prevScrollPosition = null;
                return;
            }

            prevScrollPosition ??= scrollRect.content.localPosition;
            var newVerticalPos = uiItemHeight * index;

            if (scrollRect.viewport.rect.height + scrollRect.content.localPosition.y < newVerticalPos || newVerticalPos < scrollRect.content.localPosition.y)
            {
                scrollRect.content.localPosition = new Vector2(
                    0,
                    uiItemHeight * index
                );
            }
        }
    }
}