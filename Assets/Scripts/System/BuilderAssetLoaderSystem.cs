using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
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

        private bool thumbnailRequestCoroutineRunning = false;

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

        public void ClearAllData()
        {
            _loaderState.Data.Clear();
            _loaderState.loadedModels.Clear();
            _loaderState.LoadedThumbnails.Clear();
            _loaderState.thumbnailRequestQueue.Clear();
            _loaderState.assetHierarchy.assets = new List<AssetMetadata>();
            _loaderState.assetHierarchy.childDirectories = new List<AssetHierarchyItem>();
        }

        public void CacheAllAssetMetadata()
        {
            ClearAllData();

            // load all asset metadata from the official builder
            _webRequestSystem.Get("https://builder-api.decentraland.org/v1/assetPacks?owner=default", request =>
            {
                var assetData = JsonConvert.DeserializeObject<AssetPacks>(request.webRequest.downloadHandler.text);

                foreach (AssetPacksData assetPack in assetData.data)
                {
                    string assetPackPath = _loaderState.assetHierarchy.path + "/" + assetPack.title;
                    AssetHierarchyItem assetPackHierarchyItem = new AssetHierarchyItem(assetPack.title, assetPackPath);

                    // Builder assets are in categories which are identified by a string name. E.g. "category":"decorations"
                    Dictionary<String, AssetHierarchyItem> categories = new Dictionary<string, AssetHierarchyItem>();

                    foreach (AssetPacksAsset asset in assetPack.assets)
                    {
                        Guid id = Guid.Parse(asset.id);

                        if (!categories.ContainsKey(asset.category))
                        {
                            // Capital first letter
                            string name = char.ToUpper(asset.category[0]) + asset.category.Substring(1);
                            string path = assetPackPath + "/" + name;
                            AssetHierarchyItem categoryHierarchyItem = new AssetHierarchyItem(name, path);
                            categories.Add(asset.category, categoryHierarchyItem);
                        }
                        categories[asset.category].assets.Add(new AssetMetadata(asset.name, id, AssetMetadata.AssetType.Model));


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

                    foreach (var category in categories)
                    {
                        assetPackHierarchyItem.childDirectories.Add(category.Value);
                    }

                    _loaderState.assetHierarchy.childDirectories.Add(assetPackHierarchyItem);
                }

                _editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
            });
        }

        public AssetHierarchyItem GetHierarchy() => _loaderState.assetHierarchy;


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
                if (!_loaderState.thumbnailRequestQueue.Contains(id))
                {
                    _loaderState.thumbnailRequestQueue.Enqueue(id);
                    if (!thumbnailRequestCoroutineRunning)
                    {
                        var unityState = GameObject.Find("UnityState").GetComponent<UnityState>();  // This is currently only used to have a GameObject on which a coroutine can be started
                        unityState.StartCoroutine(ThumbnailRequestCoroutine());
                    }
                }

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

            var readByteCount = await stream.ReadAsync(bytes, 0, (int)stream.Length);
            Debug.Assert(stream.Length == readByteCount);

            var thumbnail = LoadBytesAsImage(bytes, preCreatedTexture);


            // get thumbnail hash from asset id
            var hash = data.ThumbnailHash;

            _loaderState.LoadedThumbnails.Add(hash, thumbnail);
            data.ThumbnailCacheState = BuilderAssetLoaderState.DataStorage.CacheState.Loaded;

        }

        private IEnumerator ThumbnailRequestCoroutine()
        {
            thumbnailRequestCoroutineRunning = true;
            while (_loaderState.thumbnailRequestQueue.Count > 0)
            {
                var id = _loaderState.thumbnailRequestQueue.Dequeue();
                if (_loaderState.Data.TryGetValue(id, out var data))
                {
                    var loadingTask = LoadThumbnailAsync(data);
                    yield return new WaitUntil(() => loadingTask.IsCompleted);

                    _editorEvents.InvokeThumbnailDataUpdatedEvent(new List<Guid> { data.Id });

                    yield return null;
                }
            }
            thumbnailRequestCoroutineRunning = false;
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

                var updatedIds = new List<Guid> { id };
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