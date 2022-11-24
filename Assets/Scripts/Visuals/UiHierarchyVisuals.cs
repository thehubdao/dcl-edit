using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiHandler;
using UnityEngine;
using Zenject;

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
        private UiBuilder.Factory _uiBuilderFactory;
        private SceneDirectoryState _sceneDirectoryState;
        private CommandSystem _commandSystem;
        private HierarchyChangeSystem _hierarchyChangeSystem;

        [Inject]
        private void Construct(EditorEvents events, UiBuilder.Factory uiBuilderFactory, SceneDirectoryState scene, CommandSystem commandSystem, HierarchyChangeSystem hierarchyChangeSystem)
        {
            _events = events;
            _uiBuilderFactory = uiBuilderFactory;
            _sceneDirectoryState = scene;
            _commandSystem = commandSystem;
            _hierarchyChangeSystem = hierarchyChangeSystem;
        }

        public void SetupSceneEventListeners()
        {
            _events.onHierarchyChangedEvent += UpdateVisuals;
            _events.onSelectionChangedEvent += UpdateVisuals;
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            var uiBuilder = _uiBuilderFactory.Create();

            ExpandParents(_sceneDirectoryState.CurrentScene!.EntitiesInSceneRoot);
            MakeHierarchyItemsRecursive(uiBuilder, 0, _sceneDirectoryState.CurrentScene!.EntitiesInSceneRoot);

            uiBuilder.Spacer(300);

            uiBuilder.ClearAndMake(_content);
        }

        private void MakeHierarchyItemsRecursive(UiBuilder uiBuilder, int level, IEnumerable<DclEntity> entities)
        {
            foreach (var entity in entities)
            {
                var isPrimarySelection = _sceneDirectoryState.CurrentScene!.SelectionState.PrimarySelectedEntity == entity;

                var isSecondarySelection = _sceneDirectoryState.CurrentScene!.SelectionState.SecondarySelectedEntities.Contains(entity);

                var style =
                    isPrimarySelection ?
                        TextHandler.TextStyle.PrimarySelection :
                        isSecondarySelection ?
                            TextHandler.TextStyle.SecondarySelection :
                            TextHandler.TextStyle.Normal;

                var isExpanded = _hierarchyChangeSystem.IsExpanded(entity);

                uiBuilder.HierarchyItem(entity.ShownName, level, entity.Children.Any(), isExpanded, style, new HierarchyItemHandler.UiHierarchyItemActions
                {
                    OnArrowClick = () => { _hierarchyChangeSystem.ClickedOnEntityExpandArrow(entity); },
                    OnNameClick = () => { _hierarchyChangeSystem.ClickedOnEntityInHierarchy(entity); }
                });

                if (isExpanded)
                {
                    MakeHierarchyItemsRecursive(uiBuilder, level + 1, entity.Children);
                }
            }
        }
        
        private void ExpandParents(IEnumerable<DclEntity> entities)
        {   
            var selectedEntity = _sceneDirectoryState.CurrentScene?.SelectionState.PrimarySelectedEntity;
            if (selectedEntity == null) return;

            var foundChild = false;
            
            foreach (var entity in entities)
            {
                if(foundChild) continue;

                var selectedChild = FindIfSelectedChildren(selectedEntity.Parent, entity);
                if (!selectedChild) continue;
                
                // Debug.Log("Doing stuffz!");
                if (!_hierarchyChangeSystem.IsExpanded(entity))
                    _hierarchyChangeSystem.ClickedOnEntityExpandArrow(entity);
                
                ExpandParents(entity.Children);
                foundChild = true;
            }
        }

        private bool FindIfSelectedChildren(DclEntity parent, DclEntity wannaBe)
        {
            if (parent == null) return false;
            
            var isSelectedChildren = parent.Id == wannaBe.Id;
            return isSelectedChildren || FindIfSelectedChildren(parent.Parent, wannaBe);
        }
    }
}