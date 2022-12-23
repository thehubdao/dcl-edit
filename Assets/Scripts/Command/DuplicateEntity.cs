using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
public class DuplicateEntity : Command
{
    public override string Name => "Duplicate entity";

    public override string Description => "This command duplicates a selected entity and its children";

    private Guid entityId;

    public DuplicateEntity(Guid entityId)
    {
        this.entityId = entityId;
    }

    public override void Do(DclScene sceneState, EditorEvents editorEvents)
    {
        //DclEntity entity = sceneState.SelectionState.PrimarySelectedEntity;

        DclEntity entity = sceneState.GetEntityById(entityId);

        DclEntity newEntity = entity.DeepCopy(sceneState);

        sceneState.AddEntity(newEntity);

        this.entityId = newEntity.Id;

        editorEvents.InvokeHierarchyChangedEvent();

        sceneState.SelectionState.SecondarySelectedEntities.Clear();
        sceneState.SelectionState.PrimarySelectedEntity = newEntity;

        editorEvents.InvokeSelectionChangedEvent();
    }

    public override void Undo(DclScene sceneState, EditorEvents editorEvents)
    {
        //DclEntity entity = sceneState.SelectionState.PrimarySelectedEntity;
        sceneState.RemoveEntity(entityId);
        editorEvents.InvokeHierarchyChangedEvent();
        sceneState.SelectionState.SecondarySelectedEntities.Clear();
        sceneState.SelectionState.PrimarySelectedEntity = null;
        editorEvents.InvokeSelectionChangedEvent();
    }
}