using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityGLTF.Loader;
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

        // Disabling some Compiler and Resharper/Rider warnings because the following block represents the data from an web resource
        // Therefore we can't choose names, that follow our naming convention

#pragma warning disable CS0649 // "Field is never assigned" The field will be assigned by `JsonConvert.DeserializeObject`

        // ReSharper disable CollectionNeverUpdated.Local InconsistentNaming
        private struct AssetPacks
        {
            public bool ok;
            public List<AssetPacksData> data;
        }

        private struct AssetPacksData
        {
            public string id;
            public string title;
            public string thumbnail;
            public string created_at;
            public string updated_at;
            public string eth_address;
            public List<AssetPacksAsset> assets;
        }

        private struct AssetPacksAsset
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

        private struct AssetPacksMetrics
        {
            public int triangles;
            public int materials;
            public int textures;
            public int meshes;
            public int bodies;
            public int entities;
        }

        // ReSharper restore CollectionNeverUpdated.Local InconsistentNaming

#pragma warning restore CS0649

        #endregion

        // Dependencies
        private BuilderAssetLoaderState _loaderState;
        private EditorEvents _editorEvents;
        private LoadGltfFromFileSystem _loadGltfFromFileSystem;
        private IWebRequestSystem _webRequestSystem;

        private BuilderAssetDownLoader assetDownLoader;

        [Inject]
        public void Construct(BuilderAssetLoaderState loaderState, EditorEvents editorEvents, LoadGltfFromFileSystem loadGltfFromFileSystem, IWebRequestSystem webRequestSystem)
        {
            _loaderState = loaderState;
            _editorEvents = editorEvents;
            _loadGltfFromFileSystem = loadGltfFromFileSystem;
            _webRequestSystem = webRequestSystem;

            this.assetDownLoader = new BuilderAssetDownLoader(modelCachePath, webRequestSystem);
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
                            //modelHash = asset.contents[asset.Model]?.Value<string>(),
                            modelPath = asset.model,
                            contentsPathToHash = asset.contents.ToObject<Dictionary<string, string>>(),
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
                return new AssetThumbnail(id, AssetData.State.IsAvailable, thumbnail);
            }

            // check if thumbnail is already loading
            if (data.ThumbnailCacheState == BuilderAssetLoaderState.DataStorage.CacheState.Loading)
            {
                return new AssetThumbnail(id, AssetData.State.IsLoading, null);
            }

            // Download and load thumbnail
            {
                _ = LoadThumbnailAsync(data);

                return new AssetThumbnail(id, AssetData.State.IsLoading, null);
            }
        }

        private async Task LoadThumbnailAsync(BuilderAssetLoaderState.DataStorage data)
        {
            // if hash is not loaded, load thumbnail and return isLoading
            data.ThumbnailCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loading;

            var path = await assetDownLoader.GetFileFromHash(data.ThumbnailHash);

            var preCreatedTexture = new Texture2D(2, 2); // pre create Texture, because Textures might not be creatable in non Main-Threads

            var stream = File.OpenRead(path);

            var bytes = new byte[stream.Length];

            var readByteCount = await stream.ReadAsync(bytes, 0, (int) stream.Length);
            Debug.Assert(stream.Length == readByteCount);

            var thumbnail = LoadBytesAsImage(bytes, preCreatedTexture);


            // get thumbnail hash from asset id
            var hash = data.ThumbnailHash;

            _loaderState.LoadedThumbnails.Add(hash, thumbnail);
            data.ThumbnailCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loaded;

            _editorEvents.InvokeThumbnailDataUpdatedEvent(new List<Guid> {data.Id});
        }

        private Texture2D LoadBytesAsImage(byte[] bytes, Texture2D inTexture = null)
        {
            inTexture ??= new Texture2D(2, 2);
            inTexture.LoadImage(bytes);

            return inTexture;
        }

        private readonly string modelCachePath = Application.persistentDataPath + "/cache/builder";

        public AssetData GetDataById(Guid id)
        {
            // check if id is a builder asset else return null
            if (!_loaderState.Data.TryGetValue(id, out var data))
            {
                return null;
            }

            // check if hash is loaded
            if (data.DataCacheState == BuilderAssetLoaderState.DataStorage.CacheState.Loaded)
            {
                // if hash is loaded, return instance of loaded model
                var copy = Object.Instantiate(_loaderState.loadedModels[data.Id]);
                copy.SetActive(true);
                copy.transform.SetParent(null);
                return new ModelAssetData(id, copy);
            }

            // check if data is already loading
            if (data.DataCacheState == BuilderAssetLoaderState.DataStorage.CacheState.Loading)
            {
                return new AssetData(id, AssetData.State.IsLoading);
            }

            // if model isn't loaded, load model and return isLoading
            data.DataCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loading;

            _loadGltfFromFileSystem.LoadGltfFromPath(Path.GetFileName(data.modelPath), go =>
            {
                _loaderState.loadedModels.Add(data.Id, go);

                data.DataCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loaded;

                var updatedIds = new List<Guid> {id};
                _editorEvents.InvokeAssetDataUpdatedEvent(updatedIds);
            }, new BuilderAssetGltfDataLoader(Path.GetDirectoryName(data.modelPath!), data.contentsPathToHash, assetDownLoader));

            return new AssetData(id, AssetData.State.IsLoading);
        }
    }

    class BuilderAssetGltfDataLoader : IDataLoader
    {
        private readonly string directory;
        private readonly Dictionary<string, string> contentsPathToHash;
        private readonly BuilderAssetDownLoader assetDownLoader;

        public BuilderAssetGltfDataLoader(string directory, Dictionary<string, string> contentsPathToHash, BuilderAssetDownLoader builderAssetDownLoader)
        {
            this.directory = directory;
            this.contentsPathToHash = contentsPathToHash;
            this.assetDownLoader = builderAssetDownLoader;
        }

        public Task<Stream> LoadStreamAsync(string relativeFilePath)
        {
            return LoadStream(relativeFilePath);
        }

        public async Task<Stream> LoadStream(string relativeFilePath)
        {
            if (relativeFilePath == null)
            {
                throw new ArgumentNullException("relativeFilePath");
            }

            var fullPath = Path.Combine(directory, relativeFilePath);

            var hash = contentsPathToHash.First(pair => pair.Key.PathEqual(fullPath)).Value;

            //var hash = contentsPathToHash[fullPath];

            var pathToLoad = await assetDownLoader.GetFileFromHash(hash);

            if (!File.Exists(pathToLoad))
            {
                throw new FileNotFoundException("Buffer file not found", relativeFilePath);
            }

            return File.OpenRead(pathToLoad);
        }
    }

    class BuilderAssetDownLoader
    {
        private readonly string cachePath;
        private readonly IWebRequestSystem webRequestSystem;

        public BuilderAssetDownLoader(string cachePath, IWebRequestSystem webRequestSystem)
        {
            this.cachePath = cachePath;
            this.webRequestSystem = webRequestSystem;
        }

        public async Task<string> GetFileFromHash(string hash)
        {
            if (IsFileDownloaded(hash))
            {
                return MakeDownloadPath(hash);
            }

            var promise = new TaskCompletionSource<string>();

            DownloadFile(hash, _ => promise.TrySetResult(MakeDownloadPath(hash)));

            return await promise.Task;
        }

        private void DownloadFile(string hash, Action<byte[]> then)
        {
            webRequestSystem.Get($"https://builder-api.decentraland.org/v1/storage/contents/{hash}", request =>
            {
                var bytes = request.webRequest.downloadHandler.data;
                SaveBytes(hash, bytes);

                then(bytes);
            });
        }

        private void SaveBytes(string hash, byte[] bytes)
        {
            Directory.CreateDirectory(cachePath);
            File.WriteAllBytes(MakeDownloadPath(hash), bytes);
        }

        private string MakeDownloadPath(string hash)
        {
            return Path.Combine(cachePath, hash);
        }

        private bool IsFileDownloaded(string hash)
        {
            return File.Exists(MakeDownloadPath(hash));
        }
    }
}