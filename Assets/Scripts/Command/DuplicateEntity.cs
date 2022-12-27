using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
public class DuplicateEntity : Command
{
    public override string Name => "Duplicate entity";

    public override string Description => "This command duplicates a selected entity and its children";

    private Guid entityId;

    private Guid prevEntityId;
    public DuplicateEntity(Guid entityId)
    {
        this.entityId = entityId;
    }

    public override void Do(DclScene sceneState, EditorEvents editorEvents)
    {
        DclEntity entity = sceneState.GetEntityById(entityId);

        DclEntity newEntity = entity.DeepCopy(sceneState);

        this.prevEntityId = entityId;

        this.entityId = newEntity.Id;
        
        editorEvents.InvokeHierarchyChangedEvent();

        sceneState.SelectionState.SecondarySelectedEntities.Clear();
        sceneState.SelectionState.PrimarySelectedEntity = newEntity;

        editorEvents.InvokeSelectionChangedEvent();
    }

    public override void Undo(DclScene sceneState, EditorEvents editorEvents)
    {
        sceneState.RemoveEntity(entityId);
        editorEvents.InvokeHierarchyChangedEvent();
        sceneState.SelectionState.SecondarySelectedEntities.Clear();
        sceneState.SelectionState.PrimarySelectedEntity = sceneState.GetEntityById(prevEntityId);
        editorEvents.InvokeSelectionChangedEvent();
    }
}