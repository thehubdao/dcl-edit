using Assets.Scripts.EditorState;
using System;
using System.Collections.Generic;

namespace Assets.Scripts.System
{
    public interface IAssetLoaderSystem
    {
        void ClearAllData();
        void CacheAllAssetMetadata();
        IEnumerable<Guid> GetAllAssetIds();
        AssetHierarchyItem GetHierarchy();
        AssetMetadata GetMetadataById(Guid id);
        AssetThumbnail GetThumbnailById(Guid id);
        AssetData GetDataById(Guid id);
    }
}