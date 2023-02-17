using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using System.Collections.Generic;
using System.Linq;

public class DuplicateEntity : Command
{
    public override string Name => "Duplicate entity";

    public override string Description => "This command duplicates a selected entity and its children";

    private Guid entityId;

    private Guid doupEntityId;

    private int seed;

    private List<DclEntity> secondarySelectedEntities;
    private readonly float? hierarchyOrder;

    public DuplicateEntity(Guid entityId, float? hierarchyOrder)
    {
        this.entityId = entityId;
        this.hierarchyOrder = hierarchyOrder;
        System.Random rand = new System.Random();
        this.seed = rand.Next();
    }

    public override void Do(DclScene sceneState, EditorEvents editorEvents)
    {
        DclEntity entity = sceneState.GetEntityById(entityId);

        DclEntity newEntity = entity.DeepCopy(sceneState, new System.Random(this.seed), hierarchyOrder);

        this.doupEntityId = newEntity.Id;

        editorEvents.InvokeHierarchyChangedEvent();

        this.secondarySelectedEntities = sceneState.SelectionState.SecondarySelectedEntities.ToList();

        sceneState.SelectionState.SecondarySelectedEntities.Clear();
        sceneState.SelectionState.PrimarySelectedEntity = newEntity;

        editorEvents.InvokeSelectionChangedEvent();
    }

    public override void Undo(DclScene sceneState, EditorEvents editorEvents)
    {
        sceneState.RemoveEntity(doupEntityId);
        editorEvents.InvokeHierarchyChangedEvent();
        foreach (DclEntity entity in secondarySelectedEntities)
        {
            sceneState.SelectionState.SecondarySelectedEntities.Add(entity);
        }
        sceneState.SelectionState.PrimarySelectedEntity = sceneState.GetEntityById(entityId);
        editorEvents.InvokeSelectionChangedEvent();
    }
}