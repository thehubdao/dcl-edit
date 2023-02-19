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
        private EntitySelectSystem entitySelectSystem;
        private HierarchyExpansionState hierarchyExpansionState;
        private EditorEvents editorEvents;

        [Inject]
        private void Construct(EntitySelectSystem entitySelectSystem, HierarchyExpansionState hierarchyExpansionState, EditorEvents editorEvents)
        {
            this.entitySelectSystem = entitySelectSystem;
            this.hierarchyExpansionState = hierarchyExpansionState;
            this.editorEvents = editorEvents;
        }

        public void ClickedOnEntityInHierarchy(DclEntity entity)
        {
            entitySelectSystem.ClickedOnEntity(entity.Id);
        }

        public void ClickedOnEntityExpandArrow(DclEntity entity)
        {
            hierarchyExpansionState.ToggleExpanded(entity.Id);
            editorEvents.InvokeHierarchyChangedEvent();
        }

        public void SetExpanded(DclEntity entity, bool isExpanded)
        {
            hierarchyExpansionState.SetExpanded(entity.Id, isExpanded);
        }

        public bool IsExpanded(DclEntity entity)
        {
            return hierarchyExpansionState.IsExpanded(entity.Id);
        }

        public void ToggleExpand(DclEntity entity)
        {
            hierarchyExpansionState.ToggleExpanded(entity.Id);
        }

        public void ExpandParents(DclEntity entity)
        {
            if (entity == null)
            {
                hierarchyExpansionState.ExpandParents(Guid.Empty, null);
                return;
            }
            
            var parentList = new List<Guid>();
            var currentEntity = entity.Parent;

            while (currentEntity != null)
            {
                parentList.Add(currentEntity.Id);
                currentEntity = currentEntity.Parent;
            }

            hierarchyExpansionState.ExpandParents(entity.Id, parentList);
        }
    }
}
