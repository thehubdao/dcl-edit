using System;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;
using UnityEngine.Assertions;

public class DropStrategy
{
    [CanBeNull]
    public DropEntityStrategy dropEntityStrategy;

    [CanBeNull]
    public DropModelAssetStrategy dropModelAssetStrategy;

    [CanBeNull]
    public DropSceneAssetStrategy dropSceneAssetStrategy;

    [CanBeNull]
    public DropImageAssetStrategy dropImageAssetStrategy;

    public void OnDropped(DragStrategy dragStrategy)
    {
        if (dragStrategy == null) return;


        switch (dragStrategy.category)
        {
            case DragAndDropState.DropZoneCategory.Entity:
                Assert.IsNotNull(dropEntityStrategy);
                Assert.IsNotNull(dragStrategy as DragEntityStrategy);

                dropEntityStrategy!.onEntityDropped((dragStrategy as DragEntityStrategy)!.entity);
                break;

            case DragAndDropState.DropZoneCategory.ModelAsset:
                Assert.IsNotNull(dropModelAssetStrategy);
                Assert.IsNotNull(dragStrategy as DragModelAssetStrategy);

                dropModelAssetStrategy!.onModelDropped((dragStrategy as DragModelAssetStrategy)!.asset);
                break;

            case DragAndDropState.DropZoneCategory.SceneAsset:
                Assert.IsNotNull(dropSceneAssetStrategy);
                Assert.IsNotNull(dragStrategy as DragSceneAssetStrategy);

                dropSceneAssetStrategy!.onSceneDropped((dragStrategy as DragSceneAssetStrategy)!.asset);
                break;

            case DragAndDropState.DropZoneCategory.ImageAsset:
                Assert.IsNotNull(dropImageAssetStrategy);
                Assert.IsNotNull(dragStrategy as DragImageAssetStrategy);

                dropImageAssetStrategy!.onImageDropped((dragStrategy as DragImageAssetStrategy)!.asset);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(dragStrategy));
        }
    }

    public void OnHover(DragStrategy dragHandlerDragStrategy)
    {
        if (dragHandlerDragStrategy == null) return;

        switch (dragHandlerDragStrategy.category)
        {
            case DragAndDropState.DropZoneCategory.Entity:
                Assert.IsNotNull(dropEntityStrategy);
                Assert.IsNotNull(dragHandlerDragStrategy as DragEntityStrategy);

                dropEntityStrategy!.onEntityHover?.Invoke((dragHandlerDragStrategy as DragEntityStrategy)!.entity);
                break;

            case DragAndDropState.DropZoneCategory.ModelAsset:
                Assert.IsNotNull(dropModelAssetStrategy);
                Assert.IsNotNull(dragHandlerDragStrategy as DragModelAssetStrategy);

                dropModelAssetStrategy!.onModelHover?.Invoke((dragHandlerDragStrategy as DragModelAssetStrategy)!.asset);
                break;

            case DragAndDropState.DropZoneCategory.SceneAsset:
                Assert.IsNotNull(dropSceneAssetStrategy);
                Assert.IsNotNull(dragHandlerDragStrategy as DragSceneAssetStrategy);

                dropSceneAssetStrategy!.onSceneHover?.Invoke((dragHandlerDragStrategy as DragSceneAssetStrategy)!.asset);
                break;

            case DragAndDropState.DropZoneCategory.ImageAsset:
                Assert.IsNotNull(dropImageAssetStrategy);
                Assert.IsNotNull(dragHandlerDragStrategy as DragImageAssetStrategy);

                dropImageAssetStrategy!.onImageHover?.Invoke((dragHandlerDragStrategy as DragImageAssetStrategy)!.asset);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(dragHandlerDragStrategy));
        }
    }
}

public class DropEntityStrategy
{
    public Action<DclEntity> onEntityDropped;

    [CanBeNull]
    public Action<DclEntity> onEntityHover;

    public static implicit operator DropStrategy(DropEntityStrategy dropEntityStrategy)
    {
        return new DropStrategy {dropEntityStrategy = dropEntityStrategy};
    }

    public DropEntityStrategy(Action<DclEntity> onEntityDropped, Action<DclEntity> onEntityHover = null)
    {
        this.onEntityDropped = onEntityDropped;
        this.onEntityHover = onEntityHover;
    }
}

public class DropModelAssetStrategy
{
    public Action<Guid> onModelDropped;

    [CanBeNull]
    public Action<Guid> onModelHover;

    public static implicit operator DropStrategy(DropModelAssetStrategy dropModelAssetStrategy)
    {
        return new DropStrategy {dropModelAssetStrategy = dropModelAssetStrategy};
    }

    public DropModelAssetStrategy(Action<Guid> onModelDropped, Action<Guid> onModelHover = null)
    {
        this.onModelDropped = onModelDropped;
        this.onModelHover = onModelHover;
    }
}

public class DropSceneAssetStrategy
{
    public Action<Guid> onSceneDropped;

    [CanBeNull]
    public Action<Guid> onSceneHover;

    public static implicit operator DropStrategy(DropSceneAssetStrategy dropSceneAssetStrategy)
    {
        return new DropStrategy {dropSceneAssetStrategy = dropSceneAssetStrategy};
    }

    public DropSceneAssetStrategy(Action<Guid> onSceneDropped, Action<Guid> onSceneHover = null)
    {
        this.onSceneDropped = onSceneDropped;
        this.onSceneHover = onSceneHover;
    }
}

public class DropImageAssetStrategy
{
    public Action<Guid> onImageDropped;

    [CanBeNull]
    public Action<Guid> onImageHover;

    public static implicit operator DropStrategy(DropImageAssetStrategy dropImageAssetStrategy)
    {
        return new DropStrategy {dropImageAssetStrategy = dropImageAssetStrategy};
    }

    public DropImageAssetStrategy(Action<Guid> onImageDropped, Action<Guid> onImageHover = null)
    {
        this.onImageDropped = onImageDropped;
        this.onImageHover = onImageHover;
    }
}