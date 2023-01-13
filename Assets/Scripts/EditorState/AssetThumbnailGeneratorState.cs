using System;
using System.Collections.Generic;
using UnityEngine;

public class AssetThumbnailGeneratorState
{
    public struct QueuedAsset
    {
        public QueuedAsset(Guid id, Action<Texture2D> then)
        {
            this.id = id;
            this.then = then;
        }

        public Guid id;
        public Action<Texture2D> then;
    }

    public Queue<QueuedAsset> queuedAssets = new Queue<QueuedAsset>();

    // Assets that were taken from the queue but their asset data wasn't loaded yet
    public List<QueuedAsset> waitingForAssetData = new List<QueuedAsset>();
}
