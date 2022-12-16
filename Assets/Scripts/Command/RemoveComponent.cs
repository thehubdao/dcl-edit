using System;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;

namespace Assets.Scripts.Command
{
    public class RemoveComponent : SceneState.Command
    {
        private readonly Guid entityId;
        private readonly DclComponent component;
        public override string Name { get; }
        public override string Description { get; }

        public RemoveComponent(Guid entityId, DclComponent component)
        {
            this.entityId = entityId;
            this.component = component;
        }
        
        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            var entity = sceneState.GetEntityById(entityId);
            entity.RemoveComponent(entity.GetComponentByName(component.NameInCode));
            editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            var entity = sceneState.GetEntityById(entityId);
            entity.AddComponent(component);
            editorEvents.InvokeSelectionChangedEvent();
        }
    }
}