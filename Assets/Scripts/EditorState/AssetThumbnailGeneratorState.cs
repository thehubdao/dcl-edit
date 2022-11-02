using System;
using System.Collections.Generic;

public class AssetThumbnailGeneratorState
{
    public Queue<Guid> queuedAssets = new Queue<Guid>();

    // Assets that were taken from the queue but their asset data wasn't loaded yet
    public List<Guid> waitingForAssetData = new List<Guid>();
}
