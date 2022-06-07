namespace Assets.Scripts.SceneState
{
    public abstract class Command
    {
        public abstract void Do(DclScene sceneState);
        public abstract void Undo(DclScene sceneState);
        
    }
}