using System;
using System.Linq;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;

namespace Assets.Scripts.Command
{
    public class ChangeHierarchyOrder : SceneState.Command
    {
        private readonly Guid affectedEntityId;
        private readonly Guid startParentId;
        private readonly Guid newParentId;
        private readonly float startHierarchyOrder;
        private readonly float newHierarchyOrder;

        public ChangeHierarchyOrder(Guid affectedEntityId, Guid startParentId, float startHierarchyOrder, float newHierarchyOrder, Guid newParentId)
        {
            this.startParentId = startParentId;
            this.startHierarchyOrder = startHierarchyOrder;
            this.affectedEntityId = affectedEntityId;
            this.newParentId = newParentId;
            this.newHierarchyOrder = newHierarchyOrder;
        }

        public override string Name => "Change Entity Hierarchy Order";

        public override string Description =>
            $"Changing hierarchy order of Entity with id: \"{affectedEntityId}\"";

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            var draggedEntity  = sceneState.GetEntityById(affectedEntityId);
            var newParent = newParentId == default ? null : sceneState.GetEntityById(newParentId);

            draggedEntity.hierarchyOrder = newHierarchyOrder;
            draggedEntity.Parent = newParent;

            editorEvents.InvokeHierarchyChangedEvent();
            editorEvents.InvokeSelectionChangedEvent();
        }


        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            var draggedEntity  = affectedEntityId == default ? null : sceneState.GetEntityById(affectedEntityId);
            var startParent = startParentId == default ? null : sceneState.GetEntityById(startParentId);

            if (draggedEntity == null)
            {
                return;
            }

            draggedEntity.Parent = startParent;
            draggedEntity.hierarchyOrder = startHierarchyOrder;

            editorEvents.InvokeHierarchyChangedEvent();
            editorEvents.InvokeSelectionChangedEvent();
        }

    }
}
