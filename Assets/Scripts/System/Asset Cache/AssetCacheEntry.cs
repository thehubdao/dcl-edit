using System.Collections.Generic;
using System.Threading;

[System.Serializable]
public class AssetCacheEntry
{
    /// <summary>
    /// Hash of the source format.
    /// </summary>
    public string sourceHash;

    /// <summary>
    /// The first element of the list is the original format of the asset. For example, a .blend file on the file system.
    /// 
    /// The other elements are formats that this asset has been converted to.
    /// </summary>
    public List<AssetFormat> formats = new List<AssetFormat>();

    /// <summary>
    /// This semaphore is responsible for blocking the entry into the conversion block. Only one conversion should be active
    /// at a time to avoid generating the same format multiple times when requested multiple times.
    /// </summary>
    public SemaphoreSlim conversionSemaphore = new SemaphoreSlim(1);
}
