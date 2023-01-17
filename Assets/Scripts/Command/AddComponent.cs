using System;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Command
{
    public class AddComponent : SceneState.Command
    {
        // Information
        private Guid entityId;

        private DclComponent.ComponentDefinition component;


        public AddComponent(Guid entityId, DclComponent.ComponentDefinition component)
        {
            this.entityId = entityId;
            this.component = component;
        }

        public override string Name => "Add Component";
        public override string Description => $"Add a {component.NameInCode} component to the entity {entityId.Shortened()}";

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            var entity = sceneState.GetEntityById(entityId);

            if (entity.IsComponentSlotOccupied(component.NameOfSlot))
            {
                throw new Exception($"The slot {component.NameOfSlot} is already occupied");
            }

            entity.AddComponent(new DclComponent(component));
            editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            sceneState.GetEntityById(entityId).RemoveComponent(component);
            editorEvents.InvokeSelectionChangedEvent();
        }
    }
}