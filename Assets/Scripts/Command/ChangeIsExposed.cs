using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;

public class ChangeIsExposed : Command
{
    public override string Name => (newExposedState ? "Expose" : "Unexpose") + " entity";

    public override string Description => (newExposedState ? "Expose" : "Unexpose") + " entity from " +
                                          (oldExposedState ? "exposed" : "unexposed");

    private Guid entityId;
    private bool newExposedState, oldExposedState;

    public ChangeIsExposed(Guid entityId, bool newExposedState, bool oldExposedState)
    {
        this.entityId = entityId;
        this.newExposedState = newExposedState;
        this.oldExposedState = oldExposedState;
    }

    public override void Do(DclScene sceneState, EditorEvents editorEvents)
    {
        sceneState.GetEntityFormId(entityId).IsExposed = newExposedState;
        editorEvents.InvokeSelectionChangedEvent();
    }

    public override void Undo(DclScene sceneState, EditorEvents editorEvents)
    {
        sceneState.GetEntityFormId(entityId).IsExposed = oldExposedState;
        editorEvents.InvokeSelectionChangedEvent();
    }
}
