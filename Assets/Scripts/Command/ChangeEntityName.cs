using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;

public class ChangeEntityName : Command
{
    public override string Name => $"Change Name to {newName}";
    public override string Description => $"Change the name of the entity from {oldName} to {newName}";

    private Guid entityId;
    private string newName, oldName;

    public ChangeEntityName(Guid entityId, string newName, string oldName)
    {
        this.entityId = entityId;
        this.newName = newName;
        this.oldName = oldName;
    }

    public override void Do(DclScene sceneState)
    {
        var entity = sceneState.GetEntityFormId(entityId);
        if (entity == null)
        {
            Debug.LogError($"Entity {entityId} not found");
            return;
        }

        entity.CustomName = newName;

        sceneState.SelectionState.SelectionChangedEvent.Invoke(); 
    }

    public override void Undo(DclScene sceneState)
    {
        var entity = sceneState.GetEntityFormId(entityId);
        if (entity == null)
        {
            Debug.LogError($"Entity {entityId} not found");
            return;
        }

        entity.CustomName = oldName;

        sceneState.SelectionState.SelectionChangedEvent.Invoke();
    }
}
