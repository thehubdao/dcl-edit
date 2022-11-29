using System;
using System.Collections.Generic;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Zenject;

namespace Assets.Scripts.System
{
    public class HierarchyChangeSystem
    {
        // Dependencies
        private EntitySelectSystem _entitySelectSystem;
        private HierarchyExpansionState _hierarchyExpansionState;
        private EditorEvents _editorEvents;

        [Inject]
        private void Construct(EntitySelectSystem entitySelectSystem, HierarchyExpansionState hierarchyExpansionState, EditorEvents editorEvents)
        {
            _entitySelectSystem = entitySelectSystem;
            _hierarchyExpansionState = hierarchyExpansionState;
            _editorEvents = editorEvents;
        }

        public void ClickedOnEntityInHierarchy(DclEntity entity)
        {
            _entitySelectSystem.ClickedOnEntity(entity.Id);
        }

        public void ClickedOnEntityExpandArrow(DclEntity entity)
        {
            _hierarchyExpansionState.ToggleExpanded(entity.Id);
            _editorEvents.InvokeHierarchyChangedEvent();
        }

        public bool IsExpanded(DclEntity entity)
        {
            return _hierarchyExpansionState.IsExpanded(entity.Id);
        }

        public void ToggleExpand(DclEntity entity)
        {
            _hierarchyExpansionState.ToggleExpanded(entity.Id);
        }

        public void ExpandParents(DclEntity entity)
        {
            if (entity == null)
            {
                _hierarchyExpansionState.ExpandParents(Guid.Empty, null);
                return;
            }
            
            var parentList = new List<Guid>();
            var currentEntity = entity.Parent;

            while (currentEntity != null)
            {
                parentList.Add(currentEntity.Id);
                currentEntity = currentEntity.Parent;
            }

            _hierarchyExpansionState.ExpandParents(entity.Id, parentList);
        }
    }
}
