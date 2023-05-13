using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Windows.WebCam;

public abstract class DropStrategy
{
    public abstract DragAndDropState.DropZoneCategory dropZoneCategory { get; }

    public void OnDropped(DragStrategy dragStrategy)
    {
        if (dragStrategy == null) return;

        Assert.AreEqual(dropZoneCategory, dragStrategy.category);

        switch (dragStrategy)
        {
            case DragEntityStrategy dragEntityStrategy:
                Assert.IsTrue(this is DropEntityStrategy);
                (this as DropEntityStrategy)!.onEntityDropped(dragEntityStrategy.entity);
                break;

            case DragModeAssetStrategy dragModeAssetStrategy:
                Assert.IsTrue(this is DropModelAssetStrategy);
                (this as DropModelAssetStrategy)!.onModelDropped(dragModeAssetStrategy.asset);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(dragStrategy));
        }
    }
}

public class DropEntityStrategy : DropStrategy
{
    public override DragAndDropState.DropZoneCategory dropZoneCategory => DragAndDropState.DropZoneCategory.Entity;
    public Action<DclEntity> onEntityDropped;
}

public class DropModelAssetStrategy : DropStrategy
{
    public override DragAndDropState.DropZoneCategory dropZoneCategory => DragAndDropState.DropZoneCategory.ModelAsset;
    public Action<Guid> onModelDropped;
}