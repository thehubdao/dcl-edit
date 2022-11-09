using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiHandler;
using UnityEngine;
using Zenject;
using static Assets.Scripts.Events.EventDependentTypes;

namespace Assets.Scripts.Visuals
{
    public class UiHierarchyVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
#pragma warning disable CS0649 // Warning: Uninitialized filed. Serialized fields will be initialized by Unity

        [SerializeField]
        private GameObject _content;

#pragma warning restore CS0649

        // Dependencies
        private EditorEvents _events;
        private UiBuilderSystem.UiBuilder.Factory _uiBuilderFactory;
        private UiBuilderVisuals.Factory _uiBuilderVisualsFactory;
        private SceneDirectoryState _sceneDirectoryState;
        private CommandSystem _commandSystem;
        private HierarchyChangeSystem _hierarchyChangeSystem;

        private UiBuilderVisuals _uiBuilderVisuals;

        [Inject]
        private void Construct(EditorEvents events, UiBuilderSystem.UiBuilder.Factory uiBuilderFactory, UiBuilderVisuals.Factory uiBuilderVisualsFactory, SceneDirectoryState scene, CommandSystem commandSystem, HierarchyChangeSystem hierarchyChangeSystem)
        {
            _events = events;
            _uiBuilderFactory = uiBuilderFactory;
            _uiBuilderVisualsFactory = uiBuilderVisualsFactory;
            _sceneDirectoryState = scene;
            _commandSystem = commandSystem;
            _hierarchyChangeSystem = hierarchyChangeSystem;

            _uiBuilderVisuals = _uiBuilderVisualsFactory.Create(new UiBuilderSetupKey{Id = UiBuilderSetupKey.UiBuilderId.Hierarchy}, _content);
            _uiBuilderVisuals.UpdateVisuals();
        }

        public void SetupSceneEventListeners()
        {
            _events.onHierarchyChangedEvent += UpdateVisuals;
            _events.onSelectionChangedEvent += UpdateVisuals;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            var uiBuilder = _uiBuilderFactory.Create(new UiBuilderSetupKey { Id = UiBuilderSetupKey.UiBuilderId.Hierarchy });

            MakeHierarchyItemsRecursive(uiBuilder, 0, _sceneDirectoryState.CurrentScene!.EntitiesInSceneRoot);

            uiBuilder.Spacer(300);

            uiBuilder.Done();
        }

        private void MakeHierarchyItemsRecursive(UiBuilderSystem.UiBuilder uiBuilder, int level, IEnumerable<DclEntity> entities)
        {
            foreach (var entity in entities)
            {
                var isPrimarySelection = _sceneDirectoryState.CurrentScene!.SelectionState.PrimarySelectedEntity == entity;

                var isSecondarySelection = _sceneDirectoryState.CurrentScene!.SelectionState.SecondarySelectedEntities.Contains(entity);

                var style =
                    isPrimarySelection ?
                        UiBuilderAtom.TextStyle.PrimarySelection :
                        isSecondarySelection ?
                            UiBuilderAtom.TextStyle.SecondarySelection :
                            UiBuilderAtom.TextStyle.Normal;

                var isExpanded = _hierarchyChangeSystem.IsExpanded(entity);

                uiBuilder.HierarchyItem(
                    entity.ShownName,
                    level,
                    entity.Children.Any(),
                    isExpanded,
                    style,
                    () => { _hierarchyChangeSystem.ClickedOnEntityExpandArrow(entity); },
                    () => { _hierarchyChangeSystem.ClickedOnEntityInHierarchy(entity); }
                    );

                if (isExpanded)
                {
                    MakeHierarchyItemsRecursive(uiBuilder, level + 1, entity.Children);
                }
            }
        }
    }
}