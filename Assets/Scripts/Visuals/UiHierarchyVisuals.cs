using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using Assets.Scripts.Utility;
using Assets.Scripts.Visuals.UiHandler;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class UiHierarchyVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
#pragma warning disable CS0649 // Warning: Uninitialized filed. Serialized fields will be initialized by Unity

        [SerializeField]
        private GameObject _content;
        [SerializeField] private ScrollRect scrollRect;
        
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
            
            _events.onSelectionChangedEvent += ExpandSelectedItem;
            _events.onSelectionChangedEvent += UpdateVisuals;
            _events.onSelectionChangedEvent += ScrollPanelToSelectedItem;
            
            UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            var uiBuilder = _uiBuilderFactory.Create();

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

                uiBuilder.HierarchyItem(entity.ShownName, level, entity.Children.Any(), isExpanded, style,
                    new HierarchyItemHandler.UiHierarchyItemActions
                {
                    OnArrowClick = () => { _hierarchyChangeSystem.ClickedOnEntityExpandArrow(entity); },
                    OnNameClick = () => { _hierarchyChangeSystem.ClickedOnEntityInHierarchy(entity); }
                }, isPrimarySelection);

                if (isExpanded)
                {
                    MakeHierarchyItemsRecursive(uiBuilder, level + 1, entity.Children);
                }
            }
        }
        
        private void ExpandSelectedItem()
        {   
            var selectedEntity = _sceneDirectoryState.CurrentScene?.SelectionState.PrimarySelectedEntity;
            _hierarchyChangeSystem.ExpandParents(selectedEntity);
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
            
            if(!selectedUiItem.TryGetComponent<RectTransform>(out var uiItemTransform)) return;
            var uiItemPosition = uiItemTransform.localPosition;
            var offset = scrollRect.viewport.rect.height * 0.5f;

            scrollRect.content.localPosition = new Vector2(
                0,
                0 - (uiItemPosition.y + offset)
                );
        }
    }
}