using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.NewUiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class UiHierarchyVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
#pragma warning disable CS0649 // Warning: Uninitialized filed. Serialized fields will be initialized by Unity

        [SerializeField]
        private GameObject content;

#pragma warning restore CS0649

        void Update()
        {
            // Print UiBuilder Stats
            if (Input.GetKeyDown(KeyCode.L))
            {
                NewUiBuilder.NewUiBuilder.Stats.Dump();
            }
        }

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
        private UiBuilder.Factory uiBuilderFactory;
        private NewUiBuilder.NewUiBuilder newUiBuilder;
        private SceneDirectoryState sceneDirectoryState;
        private CommandSystem commandSystem;
        private HierarchyChangeSystem hierarchyChangeSystem;
        private ContextMenuSystem contextMenuSystem;

        [Inject]
        private void Construct(
            EditorEvents events,
            UiBuilder.Factory uiBuilderFactory,
            NewUiBuilder.NewUiBuilder.Factory newUiBuilderFactory,
            SceneDirectoryState scene, CommandSystem commandSystem,
            HierarchyChangeSystem hierarchyChangeSystem,
            ContextMenuSystem contextMenuSystem)
        {
            this.events = events;
            this.uiBuilderFactory = uiBuilderFactory;
            newUiBuilder = newUiBuilderFactory.Create(content);
            sceneDirectoryState = scene;
            this.commandSystem = commandSystem;
            this.hierarchyChangeSystem = hierarchyChangeSystem;
            this.contextMenuSystem = contextMenuSystem;
        }

        public void SetupSceneEventListeners()
        {
            events.onHierarchyChangedEvent += MarkForUpdate;
            events.onSelectionChangedEvent += MarkForUpdate;
            MarkForUpdate();
        }

        private void UpdateVisuals()
        {
            var mainPanelData = new PanelAtom.Data();


            MakeHierarchyItemsRecursive(0, sceneDirectoryState.CurrentScene!.EntitiesInSceneRoot, mainPanelData);

            mainPanelData.AddSpacer(300);

            newUiBuilder.Update(mainPanelData);
        }

        private void MakeHierarchyItemsRecursive(int level, IEnumerable<DclEntity> entities, PanelAtom.Data mainPanelData)
        {
            foreach (var entity in entities)
            {
                var isPrimarySelection = sceneDirectoryState.CurrentScene!.SelectionState.PrimarySelectedEntity == entity;

                var isSecondarySelection = sceneDirectoryState.CurrentScene!.SelectionState.SecondarySelectedEntities.Contains(entity);

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
                    MakeHierarchyItemsRecursive(level + 1, entity.Children, mainPanelData);
                }
            }
        }
    }
}