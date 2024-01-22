using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.EditorState
{
    //public class BuilderAssetLoaderState
    //{
    //    public class DataStorage
    //    {
    //        public enum CacheState
    //        {
    //            Loaded, // Loaded to ram
    //            Cached, // available as file
    //            Loading, // while downloading and opening file
    //            NotCached // not downloaded
    //        }
    //
    //        public Guid Id;
    //        public string Name;
    //
    //        public string modelPath;
    //        public Dictionary<string, string> contentsPathToHash;
    //
    //        public string ThumbnailHash;
    //
    //        // Data
    //        public CacheState DataCacheState = CacheState.NotCached;
    //        public CacheState ThumbnailCacheState = CacheState.NotCached;
    //        public GameObject Model;
    //    }
    //
    //
    //    public Dictionary<Guid, DataStorage> Data = new Dictionary<Guid, DataStorage>();
    //
    //    // loaded models with the id as key
    //    public Dictionary<Guid, GameObject> loadedModels = new Dictionary<Guid, GameObject>();
    //
    //    // loaded thumbnails with the Hash as key
    //    public Dictionary<string, Texture2D> LoadedThumbnails = new Dictionary<string, Texture2D>();
    //
    //    public Queue<Guid> thumbnailRequestQueue = new Queue<Guid>();
    //
    //    public AssetHierarchyItem assetHierarchy = new AssetHierarchyItem
    //    {
    //        name = "Builder Assets",
    //        path = "/Builder Assets"
    //    };
    //}
}