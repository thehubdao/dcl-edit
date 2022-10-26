using Assets.Scripts.EditorState;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.System
{
    public interface IAssetLoaderSystem
    {
        public void CacheAllAssetMetadata();
        public IEnumerable<Guid> GetAllAssetIds();
        public AssetMetadata GetMetadataById(Guid id);
        public Texture2D GetThumbnailById(Guid id);
        public AssetData GetDataById(Guid id);
    }
}