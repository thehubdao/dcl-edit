using Assets.Scripts.EditorState;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assets.Scripts.System
{
    public interface IAssetLoaderSystem
    {
        void ClearAllData();
        void CacheAllAssetMetadata();
        IEnumerable<Guid> GetAllAssetIds();
        AssetHierarchyItem GetHierarchy();
        Task<string> CopyAssetTo(Guid id);
    }
}