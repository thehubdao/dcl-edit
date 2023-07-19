using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityGLTF.Loader;
using Zenject;

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
        private BuilderAssetLoaderState loaderState;
        private EditorEvents editorEvents;
        private IWebRequestSystem webRequestSystem;
        private IPathState pathState;
        private AssetCacheSystem assetCacheSystem;
        private BuilderAssetFormat.Factory builderAssetFormatFactory;
        private BuilderAssetDownLoader assetDownloader;

        [Inject]
        public void Construct(
            BuilderAssetLoaderState loaderState,
            EditorEvents editorEvents,
            LoadGltfFromFileSystem loadGltfFromFileSystem,
            IWebRequestSystem webRequestSystem,
            IPathState pathState,
            AssetCacheSystem assetCacheSystem,
            BuilderAssetFormat.Factory builderAssetFormatFactory,
            BuilderAssetDownLoader assetDownloader)
        {
            this.loaderState = loaderState;
            this.editorEvents = editorEvents;
            this.webRequestSystem = webRequestSystem;
            this.pathState = pathState;
            this.assetCacheSystem = assetCacheSystem;
            this.builderAssetFormatFactory = builderAssetFormatFactory;
            this.assetDownloader = assetDownloader;
        }

        public void ClearAllData()
        {
            loaderState.Data.Clear();
            loaderState.assetHierarchy.assets = new List<AssetMetadata>();
            loaderState.assetHierarchy.childDirectories = new List<AssetHierarchyItem>();
        }

        public void CacheAllAssetMetadata()
        {
            ClearAllData();

            // load all asset metadata from the official builder
            webRequestSystem.Get("https://builder-api.decentraland.org/v1/assetPacks?owner=default", request =>
            {
                var assetData = JsonConvert.DeserializeObject<AssetPacks>(request.webRequest.downloadHandler.text);

                foreach (AssetPacksData assetPack in assetData.data)
                {
                    string assetPackPath = loaderState.assetHierarchy.path + "/" + assetPack.title;
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


                        loaderState.Data.Add(id, new BuilderAssetLoaderState.DataStorage
                        {
                            Id = id,
                            Name = asset.name,
                            //modelHash = asset.contents[asset.Model]?.Value<string>(),
                            modelPath = asset.model,
                            contentsPathToHash = asset.contents.ToObject<Dictionary<string, string>>(),
                            ThumbnailHash = asset.thumbnail
                        });
                        assetCacheSystem.Add(
                            id,
                            builderAssetFormatFactory.Create(
                                id,
                                asset.name,
                                asset.model,
                                asset.contents.ToObject<Dictionary<string, string>>(),
                                asset.thumbnail
                        ));
                    }

                    foreach (var category in categories)
                    {
                        assetPackHierarchyItem.childDirectories.Add(category.Value);
                    }

                    loaderState.assetHierarchy.childDirectories.Add(assetPackHierarchyItem);
                }

                editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
            });
        }

        public AssetHierarchyItem GetHierarchy() => loaderState.assetHierarchy;


        public IEnumerable<Guid> GetAllAssetIds()
        {
            return loaderState.Data.Keys;
        }

        public string modelBuildPath => Path.Combine(pathState.ProjectPath, "dcl-edit/build/builder_assets/");

        public async Task<string> CopyAssetTo(Guid id)
        {
            // check if id is a builder asset else return null
            if (!loaderState.Data.TryGetValue(id, out var data))
            {
                return null;
            }

            // Copy all files into the build path
            foreach (var (path, hash) in data.contentsPathToHash)
            {
                try
                {
                    var filePath = await assetDownloader.GetFileFromHash(hash);

                    var destFileName = Path.Combine(modelBuildPath, path);
                    Directory.CreateDirectory(Path.GetDirectoryName(destFileName) ?? throw new InvalidOperationException());
                    File.Copy(filePath, destFileName, true);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error while copying file {path} with hash {hash} to build path {modelBuildPath}.");
                    Debug.LogException(e);
                }
            }

            var modelFilePathTask = assetDownloader.GetFileFromHash(data.contentsPathToHash[data.modelPath]);
            if (!modelFilePathTask.IsCompleted)
            {
                modelFilePathTask.Wait();
            }

            return StaticUtilities.MakeRelativePath(pathState.ProjectPath, Path.Combine(modelBuildPath, data.modelPath));
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
                throw new ArgumentNullException(nameof(relativeFilePath));
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

    public class BuilderAssetDownLoader
    {
        // Dependencies
        private IWebRequestSystem webRequestSystem;

        private readonly string cachePath;

        [Inject]
        public void Construct(IWebRequestSystem webRequestSystem)
        {
            this.webRequestSystem = webRequestSystem;
        }

        public BuilderAssetDownLoader(string cachePath)
        {
            this.cachePath = cachePath;
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