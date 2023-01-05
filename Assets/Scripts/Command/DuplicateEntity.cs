using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using System.Collections.Generic;

public class DuplicateEntity : Command
{
    public override string Name => "Duplicate entity";

    public override string Description => "This command duplicates a selected entity and its children";

    private Guid entityId;

    private Guid prevEntityId;

    private int seed;

    private List<DclEntity> secondarySelectedEntities;
    
    public DuplicateEntity(Guid entityId)
    {
        this.entityId = entityId;
    }

    public override void Do(DclScene sceneState, EditorEvents editorEvents)
    {
        DclEntity entity = sceneState.GetEntityById(entityId);

        System.Random rand = new System.Random();
        this.seed = rand.Next();

        DclEntity newEntity = entity.DeepCopy(sceneState, new System.Random(this.seed));

        this.prevEntityId = entityId;

        this.entityId = newEntity.Id;
        
        editorEvents.InvokeHierarchyChangedEvent();

        this.secondarySelectedEntities = sceneState.SelectionState.SecondarySelectedEntities;

        sceneState.SelectionState.SecondarySelectedEntities.Clear();
        sceneState.SelectionState.PrimarySelectedEntity = newEntity;

        editorEvents.InvokeSelectionChangedEvent();
    }

    public override void Undo(DclScene sceneState, EditorEvents editorEvents)
    {
        sceneState.RemoveEntity(entityId);
        editorEvents.InvokeHierarchyChangedEvent();
        foreach (DclEntity entity in secondarySelectedEntities)
        {
            sceneState.SelectionState.SecondarySelectedEntities.Add(entity);
        }
        sceneState.SelectionState.PrimarySelectedEntity = sceneState.GetEntityById(prevEntityId);
        editorEvents.InvokeSelectionChangedEvent();
    }
}