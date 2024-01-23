using System;
using System.Collections.Generic;
using Assets.Scripts.Assets;
using UnityEngine;

public class AssetThumbnailGeneratorState
{
    public struct QueuedAsset
    {
        public QueuedAsset(CommonAssetTypes.IModelProvider model, Action<Sprite> then)
        {
            this.model = model;
            this.then = then;
        }

        public CommonAssetTypes.IModelProvider model;
        public Action<Sprite> then;
    }

    public Queue<QueuedAsset> queuedAssets = new Queue<QueuedAsset>();

    // Assets that were taken from the queue but their asset data wasn't loaded yet
    public List<QueuedAsset> waitingForAssetData = new List<QueuedAsset>();
}
