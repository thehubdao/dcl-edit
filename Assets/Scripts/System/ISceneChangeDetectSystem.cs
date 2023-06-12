using Assets.Scripts.SceneState;

namespace Assets.Scripts.System
{
    public interface ISceneChangeDetectSystem
    {
        bool HasSceneChanged();
        void Reevaluate(CommandHistoryState historyState, SceneChangeDetectSystem.CommandEvent commandEvent);
        void RememberCurrentState();
    }
}