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
        private IWebRequestSystem _webRequestSystem;

        [Inject]
        public void Construct(BuilderAssetLoaderState loaderState, EditorEvents editorEvents, LoadGltfFromFileSystem loadGltfFromFileSystem, IWebRequestSystem webRequestSystem)
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

        public AssetThumbnail GetThumbnailById(Guid id)
        {
            // check if id is a builder asset else return null
            if (!_loaderState.Data.TryGetValue(id, out var data))
            {
                return null;
            }

            // get thumbnail hash from asset id
            var hash = data.ThumbnailHash;

            // check if hash is loaded
            if (data.ThumbnailCacheState == BuilderAssetLoaderState.DataStorage.CacheState.Loaded)
            {
                // if hash is loaded, return the Thumbnail
                var thumbnail = _loaderState.LoadedThumbnails[hash];
                return new AssetThumbnail {Id = id, State = AssetData.State.IsAvailable, Texture = thumbnail};
            }

            // check if thumbnail is already loading
            if (data.ThumbnailCacheState == BuilderAssetLoaderState.DataStorage.CacheState.Loading)
            {
                return new AssetThumbnail {Id = id, State = AssetData.State.IsLoading, Texture = null};
            }

            // check if thumbnail is downloaded
            if (IsFileDownloaded(hash))
            {
                // if hash is downloaded, load model and return isLoading
                data.ThumbnailCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loading;

                var bytes = File.ReadAllBytes(MakeDownloadPath(hash));

                var thumbnail = LoadBytesAsImage(bytes);

                _loaderState.LoadedThumbnails.Add(hash, thumbnail);
                data.ThumbnailCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loaded;

                return new AssetThumbnail {Id = id, State = AssetData.State.IsAvailable, Texture = thumbnail};
            }

            // Download Thumbnail
            data.ThumbnailCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loading;

            DownloadFile(hash, bytes =>
            {
                var thumbnail = LoadBytesAsImage(bytes);

                _loaderState.LoadedThumbnails.Add(hash, thumbnail);
                data.ThumbnailCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loaded;

                var ids = new List<Guid> {id};
                _editorEvents.InvokeThumbnailDataUpdatedEvent(ids);
            });

            return new AssetThumbnail {Id = id, State = AssetData.State.IsLoading, Texture = null};
        }

        private Texture2D LoadBytesAsImage(byte[] bytes)
        {
            var texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);

            return texture;
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
                var copy = Object.Instantiate(_loaderState.LoadedModels[hash]);
                copy.SetActive(true);
                copy.transform.SetParent(null);
                return new ModelAssetData(id, copy);
            }

            // check if data is already loading
            if (data.DataCacheState == BuilderAssetLoaderState.DataStorage.CacheState.Loading)
            {
                return new AssetData(id, AssetData.State.IsLoading);
            }

            // check if hash is downloaded
            if (IsFileDownloaded(hash))
            {
                // if hash is downloaded, load model and return isLoading
                data.DataCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loading;

                _loadGltfFromFileSystem.LoadGltfFromPath(MakeDownloadPath(hash), go =>
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

            DownloadFile(hash, _ =>
            {
                _loadGltfFromFileSystem.LoadGltfFromPath(MakeDownloadPath(hash), go =>
                {
                    _loaderState.LoadedModels.Add(hash, go);

                    data.DataCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loaded;

                    var updatedIds = new List<Guid> {id};
                    _editorEvents.InvokeAssetDataUpdatedEvent(updatedIds);
                });
            });

            return new AssetData(id, AssetData.State.IsLoading);
        }

        private string MakeDownloadPath(string hash)
        {
            return $"{_modelCachePath}/{hash}";
        }

        private bool IsFileDownloaded(string hash)
        {
            return File.Exists(MakeDownloadPath(hash));
        }

        private void SaveBytes(string hash, byte[] bytes)
        {
            Directory.CreateDirectory(_modelCachePath);
            File.WriteAllBytes(MakeDownloadPath(hash), bytes);
        }

        private void DownloadFile(string hash, Action<byte[]> then)
        {
            _webRequestSystem.Get($"https://builder-api.decentraland.org/v1/storage/contents/{hash}", request =>
            {
                var bytes = request.webRequest.downloadHandler.data;
                SaveBytes(hash, bytes);

                then(bytes);
            });
        }
    }
}