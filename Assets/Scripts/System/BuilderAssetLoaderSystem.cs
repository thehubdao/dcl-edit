using System;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Assets.Scripts.System
{
    /*
     * Load assets from the Official builder using the public API
     *
     * Get Meta data: https://builder-api.decentraland.org/v1/assetPacks?owner=default
     *
     * Get single file from hash: https://builder-api.decentraland.org/v1/storage/contents/<hash>
     */


    public class BuilderAssetLoaderSystem : IAssetLoaderSystem
    {
        #region Response Json

        struct AssetPacks
        {
            public bool ok;
            public List<AssetPacksData> data;
        }

        struct AssetPacksData
        {
            public string id;
            public string title;
            public string thumbnail;
            public string created_at;
            public string updated_at;
            public string eth_address;
            public List<AssetPacksAsset> assets;
        }

        struct AssetPacksAsset
        {
            public string id;
            public string asset_pack_id;
            public string name;
            public string model;
            public string thumbnail;
            public List<string> tags;
            public string category;
            public JObject contents;
            public string created_at;
            public string updated_at;
            public AssetPacksMetrics metrics;
            public string script;
            public JArray parameters;
            public JArray actions;
            public string legacy_id;
        }

        struct AssetPacksMetrics
        {
            public int triangles;
            public int materials;
            public int textures;
            public int meshes;
            public int bodies;
            public int entities;
        }

        #endregion


        // Dependencies
        private BuilderAssetLoaderState _loaderState;
        private EditorEvents _editorEvents;
        private LoadGltfFromFileSystem _loadGltfFromFileSystem;
        private WebRequestSystem _webRequestSystem;

        [Inject]
        public void Construct(BuilderAssetLoaderState loaderState, EditorEvents editorEvents, LoadGltfFromFileSystem loadGltfFromFileSystem, WebRequestSystem webRequestSystem)
        {
            _loaderState = loaderState;
            _editorEvents = editorEvents;
            _loadGltfFromFileSystem = loadGltfFromFileSystem;
            _webRequestSystem = webRequestSystem;
        }

        public void CacheAllAssetMetadata()
        {
            // load all asset metadata from the official builder
            _webRequestSystem.Get("https://builder-api.decentraland.org/v1/assetPacks?owner=default", request =>
            {
                var assetData = JsonConvert.DeserializeObject<AssetPacks>(request.webRequest.downloadHandler.text);

                foreach (var assetPack in assetData.data)
                {
                    foreach (var asset in assetPack.assets)
                    {
                        var id = Guid.Parse(asset.id);
                        _loaderState.Data.Add(id, new BuilderAssetLoaderState.DataStorage
                        {
                            Id = id,
                            Name = asset.name,
                            ModelHash = asset.contents[asset.model]?.Value<string>(),
                            ThumbnailHash = asset.thumbnail
                        });
                    }
                }

                _editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
            });
        }

        public IEnumerable<Guid> GetAllAssetIds()
        {
            return _loaderState.Data.Keys;
        }

        public AssetMetadata GetMetadataById(Guid id)
        {
            if (_loaderState.Data.TryGetValue(id, out var data))
            {
                return new AssetMetadata(data.Name, id, AssetMetadata.AssetType.Model);
            }

            return null;
        }

        public Texture2D GetThumbnailById(Guid id)
        {
            throw new NotImplementedException();
        }

        private readonly string _modelCachePath = Application.persistentDataPath + "/cache/models";

        public AssetData GetDataById(Guid id)
        {
            // check if id is a builder asset else return null
            if (!_loaderState.Data.TryGetValue(id, out var data))
            {
                return null;
            }

            // get hash from asset id
            var hash = data.ModelHash;

            // check if hash is loaded
            if (data.DataCacheState == BuilderAssetLoaderState.DataStorage.CacheState.Loaded)
            {
                // if hash is loaded, return instance of loaded model
                var copy = Object.Instantiate(_loaderState.LoadedModels[data.ModelHash]);
                copy.SetActive(true);
                copy.transform.SetParent(null);
                return new ModelAssetData(id, copy);
            }

            // check if data is already loading
            if (data.DataCacheState == BuilderAssetLoaderState.DataStorage.CacheState.Loading)
            {
                return new AssetData(id, AssetData.State.IsLoading);
            }

            // check if hash is cached
            if (IsModelCached(hash))
            {
                // if hash is cached, load model and return isLoading
                data.DataCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loading;

                _loadGltfFromFileSystem.LoadGltfFromPath(MakeCachedModelPath(hash), go =>
                {
                    _loaderState.LoadedModels.Add(hash, go);

                    data.DataCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loaded;

                    var updatedIds = new List<Guid> {id};
                    _editorEvents.InvokeAssetDataUpdatedEvent(updatedIds);
                });

                return new AssetData(id, AssetData.State.IsLoading);
            }

            // Download model
            data.DataCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loading;

            _webRequestSystem.Get($"https://builder-api.decentraland.org/v1/storage/contents/{data.ModelHash}",
                request =>
                {
                    SaveBytes(data.ModelHash, request.webRequest.downloadHandler.data);

                    _loadGltfFromFileSystem.LoadGltfFromPath(MakeCachedModelPath(hash), go =>
                    {
                        _loaderState.LoadedModels.Add(hash, go);

                        data.DataCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loaded;

                        var updatedIds = new List<Guid> {id};
                        _editorEvents.InvokeAssetDataUpdatedEvent(updatedIds);
                    });
                });

            return new AssetData(id, AssetData.State.IsLoading);
        }

        private string MakeCachedModelPath(string hash)
        {
            return $"{_modelCachePath}/{hash}.glb";
        }

        private bool IsModelCached(string hash)
        {
            return File.Exists(MakeCachedModelPath(hash));
        }

        private void SaveBytes(string hash, byte[] bytes)
        {
            Directory.CreateDirectory(_modelCachePath);
            File.WriteAllBytes(MakeCachedModelPath(hash), bytes);
        }
    }
}