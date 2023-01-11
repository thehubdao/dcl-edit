using Assets.Scripts.Command.Utility;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Command
{
    public class RemoveEntity : SceneState.Command
    {
        private readonly DclEntity entity;

        public RemoveEntity(DclEntity entity)
        {
            this.entity = entity;
        }

        public override string Name => "Remove Entity";
        public override string Description => $"Removing Entity \"{entity.ShownName}\" with id \"{entity.Id.Shortened()}\"" + (entity.Parent != null ? $" from Parent {entity.Parent.CustomName}" : "");

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            EntityUtility.DeleteEntity(sceneState, entity.Id);
            editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            EntityUtility.AddEntity(sceneState, entity);
            editorEvents.InvokeSelectionChangedEvent();
        }
    }
}
