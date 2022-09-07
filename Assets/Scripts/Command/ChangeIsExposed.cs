using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.SceneState;
using UnityEngine;

public class ChangeIsExposed : Command
{
    public override string Name => (newExposedState?"Expose":"Unexpose")+" entity";

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

    public override void Do(DclScene sceneState)
    {
        sceneState.GetEntityFormId(entityId).IsExposed = newExposedState;
        sceneState.SelectionState.SelectionChangedEvent.Invoke();
    }

    public override void Undo(DclScene sceneState)
    {
        sceneState.GetEntityFormId(entityId).IsExposed = oldExposedState;
        sceneState.SelectionState.SelectionChangedEvent.Invoke();
    }
}
