using System;
using System.Linq;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;

namespace Assets.Scripts.Command
{
    public class ChangeHierarchyOrder : SceneState.Command
    {
        private readonly HierarchyExpansionState hierarchyExpansionState;
        private readonly Guid draggedEntityId;
        private readonly Guid hoveredEntityId;
        private readonly Guid startParentId;
        private readonly float startHierarchyOrder;
        private readonly bool? startExpansionStateParent;
        private readonly bool? startExpansionStateHoveredEntity;
        private DclEntity startPrimarySelection;
        private readonly float newHierarchyOrder;
        private readonly Guid newParentId;
        private readonly bool shouldExpand;

        public ChangeHierarchyOrder(DclEntity draggedEntity, [CanBeNull] DclEntity hoveredEntity, HierarchyExpansionState hierarchyExpansionState, float newHierarchyOrder, [CanBeNull] DclEntity newParent, bool shouldExpand)
        {
            this.hierarchyExpansionState = hierarchyExpansionState;
            
            startParentId = draggedEntity.ParentId;
            startHierarchyOrder = draggedEntity.hierarchyOrder;
            
            draggedEntityId = draggedEntity.Id;
            hoveredEntityId = hoveredEntity?.Id ?? default;
            newParentId = newParent?.Id ?? default;
            this.newHierarchyOrder = newHierarchyOrder;
            
            this.shouldExpand = shouldExpand;
            
            if (startParentId != default)
            {
                startExpansionStateParent = hierarchyExpansionState.IsExpanded(startParentId);
            }
            
            if (hoveredEntity != null)
            {
                startExpansionStateHoveredEntity = hierarchyExpansionState.IsExpanded(hoveredEntity.Id);
            }
        }

        public override string Name => "Change Entity Hierarchy Order";

        public override string Description =>
            $"Changing hierarchy order of Entity with id: \"{draggedEntityId}\"";

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            var hoveredEntity  = hoveredEntityId == default ? null : sceneState.GetEntityById(hoveredEntityId);
            var draggedEntity  = sceneState.GetEntityById(draggedEntityId);
            var newParent = newParentId == default ? null : sceneState.GetEntityById(newParentId);

            draggedEntity.hierarchyOrder = newHierarchyOrder;
            draggedEntity.Parent = newParent;

            startPrimarySelection = sceneState.SelectionState.PrimarySelectedEntity;
            sceneState.SelectionState.PrimarySelectedEntity = draggedEntity;

            if (shouldExpand && hoveredEntity != null)
            {
                hierarchyExpansionState.SetExpanded(hoveredEntity.Id, true);
            }

            editorEvents.InvokeHierarchyChangedEvent();
            editorEvents.InvokeSelectionChangedEvent();
        }


        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            var draggedEntity  = draggedEntityId == default ? null : sceneState.GetEntityById(draggedEntityId);
            var startParent = startParentId == default ? null : sceneState.GetEntityById(startParentId);

            if (draggedEntity == null)
            {
                return;
            }

            draggedEntity.Parent = startParent;
            draggedEntity.hierarchyOrder = startHierarchyOrder;

            if (startExpansionStateParent != null)
            {
                hierarchyExpansionState.SetExpanded(startParentId, (bool)startExpansionStateParent);
            }
            
            if (hoveredEntityId != default && startExpansionStateHoveredEntity != null)
            {
                hierarchyExpansionState.SetExpanded(hoveredEntityId, (bool)startExpansionStateHoveredEntity);
            }

            sceneState.SelectionState.PrimarySelectedEntity = startPrimarySelection;

            editorEvents.InvokeHierarchyChangedEvent();
            editorEvents.InvokeSelectionChangedEvent();
        }

    }
}
