using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Utility;
using Assets.Scripts.Visuals.UiHandler;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
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
        private ScrollRect scrollRect;

#pragma warning restore CS0649

        #region Mark for update

        private bool shouldUpdate = false;

        void LateUpdate()
        {
            if (shouldUpdate)
            {
                ExpandSelectedItem();
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
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        private void Construct(
            EditorEvents events,
            Factory uiBuilderFactory,
            HierarchyChangeSystem hierarchyChangeSystem,
            ContextMenuSystem contextMenuSystem,
            SceneManagerSystem sceneManagerSystem)
        {
            this.events = events;
            this.uiBuilder = uiBuilderFactory.Create(content);
            this.hierarchyChangeSystem = hierarchyChangeSystem;
            this.contextMenuSystem = contextMenuSystem;
            this.sceneManagerSystem = sceneManagerSystem;

            SetupEventListeners();
        }

        public void SetupEventListeners()
        {
            events.onHierarchyChangedEvent += MarkForUpdate;
            events.onSelectionChangedEvent += MarkForUpdate;
            MarkForUpdate();
        }

        private void UpdateVisuals()
        {
            var mainPanelData = NewPanelData();

            var scene = sceneManagerSystem.GetCurrentScene();

            if (scene == null)
            {
                mainPanelData.AddTitle("No scene loaded");
            }
            else
            {
                MakeHierarchyItemsRecursive(scene, 0, scene.EntitiesInSceneRoot, mainPanelData);

                mainPanelData.AddSpacer(300);
            }

            uiBuilder.Update(mainPanelData);
        }

        private void MakeHierarchyItemsRecursive([NotNull] DclScene scene, int level, IEnumerable<DclEntity> entities, PanelAtom.Data mainPanelData)
        {
            foreach (var entity in entities)
            {
                var isPrimarySelection = scene.SelectionState.PrimarySelectedEntity == entity;

                var isSecondarySelection = scene.SelectionState.SecondarySelectedEntities.Contains(entity);

                var style =
                    isPrimarySelection ?
                        TextHandler.TextStyle.PrimarySelection :
                        isSecondarySelection ?
                            TextHandler.TextStyle.SecondarySelection :
                            TextHandler.TextStyle.Normal;

                var isExpanded = hierarchyChangeSystem.IsExpanded(entity);

                mainPanelData.AddHierarchyItem(entity.ShownName, level, entity.Children.Any(), isExpanded, style,
                    isPrimarySelection,
                    new HierarchyItemHandler.UiHierarchyItemActions
                    {
                        onArrowClick = () => { hierarchyChangeSystem.ClickedOnEntityExpandArrow(entity); },
                        onNameClick = () => { hierarchyChangeSystem.ClickedOnEntityInHierarchy(entity); }
                    },
                    clickPosition =>
                    {
                        contextMenuSystem.OpenMenu(clickPosition, new List<ContextMenuItem>
                        {
                            new ContextSubmenuItem("Add entity...", new List<ContextMenuItem>
                            {
                                new ContextMenuTextItem("Empty Entity", () => Debug.Log("Add empty Entity")),
                                new ContextMenuSpacerItem(),
                                new ContextMenuTextItem("Gltf Entity", () => Debug.Log("Add Gltf Entity")),
                                new ContextMenuSpacerItem(),
                                new ContextMenuTextItem("Cube", () => Debug.Log("Add empty ")),
                                new ContextMenuTextItem("Sphere", () => Debug.Log("Add empty ")),
                                new ContextMenuTextItem("Cylinder", () => Debug.Log("Add empty ")),
                                new ContextMenuTextItem("Cone", () => Debug.Log("Add empty ")),
                            }),
                            new ContextMenuTextItem("Duplicate", () => Debug.Log("Duplicate entity")),
                            new ContextMenuTextItem("Delete", () => Debug.Log("Delete entity"))
                        });
                    });

                if (isExpanded)
                {
                    MakeHierarchyItemsRecursive(scene, level + 1, entity.Children, mainPanelData);
                }
            }
        }

        private void ExpandSelectedItem()
        {
            var selectedEntity = sceneManagerSystem.GetCurrentScene()?.SelectionState.PrimarySelectedEntity;
            hierarchyChangeSystem.ExpandParents(selectedEntity);
        }

        private HierarchyItemHandler HierarchyPanelSelectedItem()
        {
            HierarchyItemHandler uiItemHandler = null;
            foreach (var uiItem in scrollRect.content.GetChildren())
            {
                if (!uiItem.TryGetComponent<HierarchyItemHandler>(out var hierarchyItemHandler)) continue;

                if (hierarchyItemHandler.primarySelection)
                    uiItemHandler = hierarchyItemHandler;
            }

            return uiItemHandler;
        }

        private void ScrollPanelToSelectedItem()
        {
            var selectedUiItem = HierarchyPanelSelectedItem();
            if (selectedUiItem == null) return;

            if (!selectedUiItem.TryGetComponent<RectTransform>(out var uiItemTransform)) return;
            var uiItemPosition = uiItemTransform.localPosition;
            var offset = scrollRect.viewport.rect.height * 0.5f;

            scrollRect.content.localPosition = new Vector2(
                0,
                0 - (uiItemPosition.y + offset)
            );
        }
    }
}