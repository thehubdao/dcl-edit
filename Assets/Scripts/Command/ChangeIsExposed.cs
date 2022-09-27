using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using System;

public class ChangeIsExposed : Command
{
    public override string Name => (newExposedState ? "Expose" : "Unexpose") + " entity";

    public override string Description => (newExposedState ? "Expose" : "Unexpose") + " entity from " +
                                          (oldExposedState ? "exposed" : "unexposed");

    private Guid entityId;
    private bool newExposedState, oldExposedState;

    // Dependencies
    private EditorEvents _editorEvents;

    public ChangeIsExposed(Guid entityId, bool newExposedState, bool oldExposedState, EditorEvents editorEvents)
    {
        this.entityId = entityId;
        this.newExposedState = newExposedState;
        this.oldExposedState = oldExposedState;
        _editorEvents = editorEvents;
    }

    public override void Do(DclScene sceneState)
    {
        sceneState.GetEntityFormId(entityId).IsExposed = newExposedState;
        _editorEvents.SelectionChangedEvent();
    }

    public override void Undo(DclScene sceneState)
    {
        sceneState.GetEntityFormId(entityId).IsExposed = oldExposedState;
        _editorEvents.SelectionChangedEvent();
    }
}
