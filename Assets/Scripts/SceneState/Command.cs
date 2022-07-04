namespace Assets.Scripts.SceneState
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract void Do(DclScene sceneState);
        public abstract void Undo(DclScene sceneState);
        
    }
}