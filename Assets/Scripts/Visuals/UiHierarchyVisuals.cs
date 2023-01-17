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
using Zenject;
using static Assets.Scripts.Visuals.UiBuilder.UiBuilder;

namespace Assets.Scripts.Visuals
{
    public class UiHierarchyVisuals : MonoBehaviour
    {
#pragma warning disable CS0649 // Warning: Uninitialized filed. Serialized fields will be initialized by Unity

        [SerializeField]
        private GameObject content;

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

        [Inject]
        private void Construct(
            EditorEvents events,
            Factory uiBuilderFactory,
            HierarchyChangeSystem hierarchyChangeSystem,
            ContextMenuSystem contextMenuSystem,
            SceneManagerSystem sceneManagerSystem,
            HierarchyContextMenuSystem hierarchyContextMenuSystem)
        {
            this.events = events;
            this.uiBuilder = uiBuilderFactory.Create(content);
            this.hierarchyChangeSystem = hierarchyChangeSystem;
            this.contextMenuSystem = contextMenuSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.hierarchyContextMenuSystem = hierarchyContextMenuSystem;

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

                mainPanelData.AddHierarchyItem(entity.ShownName, level, entity.Children.Any(), isExpanded, style, new HierarchyItemHandler.UiHierarchyItemActions
                    {
                        onArrowClick = () => { hierarchyChangeSystem.ClickedOnEntityExpandArrow(entity); },
                        onNameClick = () => { hierarchyChangeSystem.ClickedOnEntityInHierarchy(entity); }
                    },
                    clickPosition =>
                    {
                        var addEntityMenuItems = new List<ContextMenuItem>();

                        foreach (var preset in hierarchyContextMenuSystem.GetPresets())
                        {
                            addEntityMenuItems.Add(new ContextMenuTextItem(preset.name, () => hierarchyContextMenuSystem.AddEntityFromPreset(preset, entity.Id)));
                        }

                        contextMenuSystem.OpenMenu(clickPosition, new List<ContextMenuItem>
                        {
                            new ContextSubmenuItem("Add entity...", addEntityMenuItems),
                            new ContextMenuTextItem("Duplicate", () => Debug.Log("Duplicate entity"), true),
                            new ContextMenuTextItem("Delete", () => Debug.Log("Delete entity"), true)
                        });
                    });

                if (isExpanded)
                {
                    MakeHierarchyItemsRecursive(scene, level + 1, entity.Children, mainPanelData);
                }
            }
        }
    }
}