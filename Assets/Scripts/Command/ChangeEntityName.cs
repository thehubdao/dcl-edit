using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using System;
using UnityEngine;

public class ChangeEntityName : Command
{
    public override string Name => $"Change Name to {newName}";
    public override string Description => $"Change the name of the entity from {oldName} to {newName}";

    private Guid entityId;
    private string newName, oldName;

    // Dependencies
    private EditorEvents _editorEvents;

    public ChangeEntityName(Guid entityId, string newName, string oldName, EditorEvents editorEvents)
    {
        this.entityId = entityId;
        this.newName = newName;
        this.oldName = oldName;
        _editorEvents = editorEvents;
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

        _editorEvents.SelectionChangedEvent();
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

        _editorEvents.SelectionChangedEvent();
    }
}
