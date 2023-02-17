using System;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;

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
            var affectedEntity  = sceneState.GetEntityById(affectedEntityId);
            var newParent = newParentId == default ? null : sceneState.GetEntityById(newParentId);

            affectedEntity.hierarchyOrder = newHierarchyOrder;
            affectedEntity.Parent = newParent;

            editorEvents.InvokeHierarchyChangedEvent();
            editorEvents.InvokeSelectionChangedEvent();
        }


        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            var affectedEntity  = sceneState.GetEntityById(affectedEntityId);
            var startParent = startParentId == default ? null : sceneState.GetEntityById(startParentId);

            affectedEntity.Parent = startParent;
            affectedEntity.hierarchyOrder = startHierarchyOrder;

            editorEvents.InvokeHierarchyChangedEvent();
            editorEvents.InvokeSelectionChangedEvent();
        }

    }
}
