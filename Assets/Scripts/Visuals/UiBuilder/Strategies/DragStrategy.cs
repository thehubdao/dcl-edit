using System;
using Assets.Scripts.SceneState;

public abstract class DragStrategy
{
    public abstract DragAndDropState.DropZoneCategory category { get; }
}


public class DragEntityStrategy : DragStrategy
{
    public override DragAndDropState.DropZoneCategory category => DragAndDropState.DropZoneCategory.Entity;

    public DclEntity entity;
}

public class DragModeAssetStrategy : DragStrategy
{
    public override DragAndDropState.DropZoneCategory category { get; }

    public Guid asset;
}
