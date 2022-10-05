using Assets.Scripts.Events;

namespace Assets.Scripts.SceneState
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract void Do(DclScene sceneState, EditorEvents editorEvents);
        public abstract void Undo(DclScene sceneState, EditorEvents editorEvents);
    }
}