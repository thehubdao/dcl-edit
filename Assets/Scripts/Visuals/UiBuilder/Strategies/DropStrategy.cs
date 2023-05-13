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
}

public class DropModelAssetStrategy
{
    public Action<Guid> onModelDropped;

    public static implicit operator DropStrategy(DropModelAssetStrategy dropModelAssetStrategy)
    {
        return new DropStrategy {dropModelAssetStrategy = dropModelAssetStrategy};
    }
}