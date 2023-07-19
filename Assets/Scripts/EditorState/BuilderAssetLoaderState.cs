using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.EditorState
{
    public class BuilderAssetLoaderState
    {
        public class DataStorage
        {
            public Guid Id;
            public string Name;

            public string modelPath;
            public Dictionary<string, string> contentsPathToHash;

            public string ThumbnailHash;

            public GameObject Model;
            public Texture2D Thumbnail;

            /// <summary>
            /// If the asset data is currently being loaded, this is the task that runs the loading operation. 
            /// This task will be completed once the asset data is loaded.
            /// </summary>
            public Task<int> loadingTask;
            public bool IsLoading
            {
                get
                {
                    if (loadingTask == null) return false;
                    return !loadingTask.IsCompleted;
                }
            }
        }

        public Dictionary<Guid, DataStorage> Data = new Dictionary<Guid, DataStorage>();

        public AssetHierarchyItem assetHierarchy = new AssetHierarchyItem
        {
            name = "Builder Assets",
            path = "/Builder Assets"
        };
    }
}