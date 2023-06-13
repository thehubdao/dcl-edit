using Assets.Scripts.SceneState;
using Assets.Scripts.System;

public class MockSceneChangeDetectSystem : ISceneChangeDetectSystem
{
    public bool HasSceneChanged()
    {
        return false;
    }

    public void Reevaluate(CommandHistoryState historyState, SceneChangeDetectSystem.CommandEvent commandEvent)
    {
    }

    public void RememberCurrentState()
    {
    }
}
