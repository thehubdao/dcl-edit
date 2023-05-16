using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Windows.WebCam;

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
}

public class DropEntityStrategy
{
    public Action<DclEntity> onEntityDropped;

    public static implicit operator DropStrategy(DropEntityStrategy dropEntityStrategy)
    {
        return new DropStrategy {dropEntityStrategy = dropEntityStrategy};
    }

    public DropEntityStrategy(Action<DclEntity> onEntityDropped)
    {
        this.onEntityDropped = onEntityDropped;
    }
}

public class DropModelAssetStrategy
{
    public Action<Guid> onModelDropped;

    public static implicit operator DropStrategy(DropModelAssetStrategy dropModelAssetStrategy)
    {
        return new DropStrategy {dropModelAssetStrategy = dropModelAssetStrategy};
    }

    public DropModelAssetStrategy(Action<Guid> onModelDropped)
    {
        this.onModelDropped = onModelDropped;
    }
}

public class DropSceneAssetStrategy
{
    public Action<Guid> onSceneDropped;

    public static implicit operator DropStrategy(DropSceneAssetStrategy dropSceneAssetStrategy)
    {
        return new DropStrategy {dropSceneAssetStrategy = dropSceneAssetStrategy};
    }

    public DropSceneAssetStrategy(Action<Guid> onSceneDropped)
    {
        this.onSceneDropped = onSceneDropped;
    }
}

public class DropImageAssetStrategy
{
    public Action<Guid> onImageDropped;

    public static implicit operator DropStrategy(DropImageAssetStrategy dropImageAssetStrategy)
    {
        return new DropStrategy {dropImageAssetStrategy = dropImageAssetStrategy};
    }

    public DropImageAssetStrategy(Action<Guid> onImageDropped)
    {
        this.onImageDropped = onImageDropped;
    }
}