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

    public DragEntityStrategy(DclEntity entity)
    {
        this.entity = entity;
    }
}

public class DragModelAssetStrategy : DragStrategy
{
    public override DragAndDropState.DropZoneCategory category => DragAndDropState.DropZoneCategory.ModelAsset;

    public Guid asset;

    public DragModelAssetStrategy(Guid asset)
    {
        this.asset = asset;
    }
}

public class DragSceneAssetStrategy : DragStrategy
{
    public override DragAndDropState.DropZoneCategory category => DragAndDropState.DropZoneCategory.SceneAsset;

    public Guid asset;

    public DragSceneAssetStrategy(Guid asset)
    {
        this.asset = asset;
    }
}

public class DragImageAssetStrategy : DragStrategy
{
    public override DragAndDropState.DropZoneCategory category => DragAndDropState.DropZoneCategory.ImageAsset;

    public Guid asset;

    public DragImageAssetStrategy(Guid asset)
    {
        this.asset = asset;
    }
}
