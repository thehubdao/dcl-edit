using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
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

    public override void Do(DclScene sceneState, EditorEvents editorEvents)
    {
        var entity = sceneState.GetEntityById(entityId);
        if (entity == null)
        {
            Debug.LogError($"Entity {entityId} not found");
            return;
        }

        entity.CustomName = newName;
    }

    public override void Undo(DclScene sceneState, EditorEvents editorEvents)
    {
        var entity = sceneState.GetEntityById(entityId);
        if (entity == null)
        {
            Debug.LogError($"Entity {entityId} not found");
            return;
        }

        entity.CustomName = oldName;
    }
}
