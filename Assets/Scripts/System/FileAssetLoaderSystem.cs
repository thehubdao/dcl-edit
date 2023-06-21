using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Assets.Scripts.System
{
    public class FileAssetLoaderSystem : IAssetLoaderSystem
    {
        // Dependencies
        private FileAssetLoaderState loaderState;
        private PathState pathState; // TODO: Change this
        private EditorEvents editorEvents;
        private LoadGltfFromFileSystem loadGltfFromFileSystem;
        private AssetThumbnailGeneratorSystem assetThumbnailGeneratorSystem;
        private FileUpgraderSystem fileUpgraderSystem;
        private SceneManagerSystem sceneManagerSystem;
        private string relativePathInProject = "assets";
        private bool readOnly = false;

        public Dictionary<Guid, AssetMetadataFile> assetMetadataCache => loaderState.assetMetadataCache;
        public Dictionary<Guid, AssetData> assetDataCache => loaderState.assetDataCache;

        [Inject]
        private void Construct(
            FileAssetLoaderState loaderState,
            PathState pathState,
            EditorEvents editorEvents,
            LoadGltfFromFileSystem loadGltfFromFileSystem,
            AssetThumbnailGeneratorSystem assetThumbnailGeneratorSystem,
            FileUpgraderSystem fileUpgraderSystem,
            SceneManagerSystem sceneManagerSystem)
        {
            this.loaderState = loaderState;
            this.pathState = pathState;
            this.editorEvents = editorEvents;
            this.loadGltfFromFileSystem = loadGltfFromFileSystem;
            this.assetThumbnailGeneratorSystem = assetThumbnailGeneratorSystem;
            this.fileUpgraderSystem = fileUpgraderSystem;
            this.sceneManagerSystem = sceneManagerSystem;
        }

        /// <summary>
        /// Note: The relative path should never start with a slash (/). Otherwise combining paths with Path.Combine()
        /// won't work since it's parameter "path2" shouldn't be absolute.
        /// </summary>
        /// <param name="relativePathInProject"></param>
        /// <param name="readOnly"></param>
        public FileAssetLoaderSystem(string relativePathInProject, bool readOnly)
        {
            this.relativePathInProject = relativePathInProject;
            this.readOnly = readOnly;
        }

        //TODO Change when asset loading is changed
        private void CheckAssetDirectoryExists()
        {
            string directoryPath = Path.Combine(pathState.ProjectPath, relativePathInProject);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }
        }


        #region Public methods
        public void ClearAllData()
        {
            loaderState.assetDataCache.Clear();
            loaderState.assetMetadataCache.Clear();
            loaderState.assetHierarchy = new AssetHierarchyItem();
        }

        public void CacheAllAssetMetadata()
        {
            CheckAssetDirectoryExists();

            ClearAllData();
            try
            {
                string directoryPath = Path.Combine(pathState.ProjectPath, relativePathInProject);

                loaderState.assetHierarchy = ScanDirectory(directoryPath, overrideDirname: StringToTitleCase(relativePathInProject));

                editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
            }
            catch (Exception e)
            {
                Debug.Log($"Error while caching assets: {e}");
            }
        }

        public IEnumerable<Guid> GetAllAssetIds() => assetMetadataCache.Keys;

        public AssetHierarchyItem GetHierarchy()
        {
            if (loaderState.assetHierarchy == null)
            {
                CacheAllAssetMetadata();
            }
            return loaderState.assetHierarchy;
        }

        public AssetMetadata GetMetadataById(Guid id)
        {
            if (assetMetadataCache.TryGetValue(id, out var file))
            {
                return file.assetMetadata;
            }
            return null;
        }

        public AssetThumbnail GetThumbnailById(Guid id)
        {
            if (!assetMetadataCache.TryGetValue(id, out var metadata))
            {
                return null; // sorry but this id is in another loader system. Mamma Mia Pizzeria!
            }

            if (metadata.thumbnail != null)
            {
                return new AssetThumbnail(id, AssetData.State.IsAvailable, metadata.thumbnail); // Thumbnail is available
            }

            if (readOnly) return new AssetThumbnail(id, AssetData.State.IsError, null);

            assetThumbnailGeneratorSystem.Generate(id, thumbnail =>
            {
                if (thumbnail != null)
                {
                    metadata.thumbnail = thumbnail;
                    WriteMetadataToFile(metadata);
                }

                editorEvents.InvokeThumbnailDataUpdatedEvent(new List<Guid> { id });
            });

            return new AssetThumbnail(id, AssetData.State.IsLoading, null); // Thumbnail needs to be generated
        }

        public AssetData GetDataById(Guid id)
        {
            if (!assetMetadataCache.ContainsKey(id))
            {
                return null;
            }

            return LoadAssetData(id);
        }

        public Task<string> CopyAssetTo(Guid id)
        {
            if (loaderState.assetMetadataCache.TryGetValue(id, out var metadataFile))
            {
                return Task.FromResult(StaticUtilities.MakeRelativePath(pathState.ProjectPath, metadataFile.assetFilePath));
            }

            return Task.FromResult<string>(null);
        }

        public bool SetThumbnailById(Guid id, Texture2D newThumbnail)
        {
            if (newThumbnail == null) return false;

            if (assetMetadataCache.TryGetValue(id, out AssetMetadataFile metaFile))
            {
                metaFile.thumbnail = newThumbnail;
                WriteMetadataToFile(metaFile);
                editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
                return true;
            }
            return false;
        }
        #endregion

        #region Metadata related methods
        private AssetHierarchyItem ScanDirectory(string fileSystemPath, string pathInHierarchy = "", string overrideDirname = null)
        {
            CheckUpgrades(Directory.GetFiles(fileSystemPath, "*.dclasset"));
            
            string dirname = Path.GetFileName(fileSystemPath);
            string[] files = Directory.GetFiles(fileSystemPath, "*.*");
            string[] subdirs = Directory.GetDirectories(fileSystemPath);

            pathInHierarchy += "/" + dirname;

            List<AssetMetadata> assets = new List<AssetMetadata>();
            List<AssetHierarchyItem> childDirectories = new List<AssetHierarchyItem>();


            foreach (string assetFile in files)
            {
                AssetMetadata result = ScanFile(assetFile);
                if (result != null) assets.Add(result);
            }

            foreach (string subdir in subdirs)
            {
                // Treat scenes as assets instead of directories
                if (Path.GetExtension(subdir) == ".dclscene")
                {
                    var sceneMetadataFile = ReadSceneDirectory(subdir);
                    if (sceneMetadataFile != null)
                    {
                        assetMetadataCache[sceneMetadataFile.assetMetadata.assetId] = sceneMetadataFile;
                        assets.Add(sceneMetadataFile.assetMetadata);
                    }
                    continue;
                }

                childDirectories.Add(ScanDirectory(subdir, pathInHierarchy));
            }

            return new AssetHierarchyItem(overrideDirname ?? dirname, pathInHierarchy, childDirectories, assets);
        }

        private string StringToTitleCase(string dirname)
        {
            return new CultureInfo("en-US", false).TextInfo.ToTitleCase(dirname);
        }

        private void CheckUpgrades(string[] files)
        {
            foreach (var assetFile in files)
            {
                fileUpgraderSystem.CheckUpgrades(assetFile);
            }
        }

        private AssetMetadataFile ReadSceneDirectory(string pathToScene)
        {
            try
            {
                string sceneName = Path.GetFileNameWithoutExtension(pathToScene);
                string pathToSceneFile = Path.Combine(pathToScene, "scene.json");
                string rawContents = File.ReadAllText(pathToSceneFile);
                SceneManagerSystem.SceneFileContents sceneFileContents = JsonConvert.DeserializeObject<SceneManagerSystem.SceneFileContents>(rawContents);
                var contents = new AssetMetadataFile.Contents(
                    new AssetMetadataFile.MetaContents(sceneName, sceneFileContents.id, AssetMetadata.AssetType.Scene, sceneName),
                    null
                );
                var metadataFile = new AssetMetadataFile(contents, pathToSceneFile);
                return metadataFile;
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return null;
            }
        }

        protected AssetMetadata ScanFile(string pathToFile)
        {
            string fileName = Path.GetFileName(pathToFile);
            string displayName = Path.GetFileNameWithoutExtension(pathToFile);
            string fileNameExtension = Path.GetExtension(pathToFile);
            AssetMetadata result = null;
            try
            {
                switch (fileNameExtension)
                {
                    case ".dclasset":
                        string json = File.ReadAllText(pathToFile);
                        AssetMetadataFile.Contents contents = JsonConvert.DeserializeObject<AssetMetadataFile.Contents>(json);
                        assetMetadataCache[contents.metadata.assetId] = new AssetMetadataFile(contents, pathToFile);
                        result = new AssetMetadata(contents.metadata.assetDisplayName, contents.metadata.assetId, contents.metadata.assetType);
                        break;
                    case ".glb":
                    case ".gltf":
                        if (readOnly) break;
                        if (MetadataFileExists(pathToFile)) break;
                        AssetMetadataFile modelMetadataFile = new AssetMetadataFile(pathToFile + ".dclasset", fileName, new AssetMetadata(displayName, Guid.NewGuid(), AssetMetadata.AssetType.Model));
                        WriteMetadataToFile(modelMetadataFile);
                        assetMetadataCache[modelMetadataFile.assetMetadata.assetId] = modelMetadataFile;
                        result = new AssetMetadata(displayName, modelMetadataFile.assetMetadata.assetId, AssetMetadata.AssetType.Model);
                        break;
                    case ".png":
                        if (readOnly) break;
                        if (MetadataFileExists(pathToFile)) break;
                        AssetMetadataFile imageMetadataFile = new AssetMetadataFile(pathToFile + ".dclasset", fileName, new AssetMetadata(displayName, Guid.NewGuid(), AssetMetadata.AssetType.Image));
                        WriteMetadataToFile(imageMetadataFile);
                        assetMetadataCache[imageMetadataFile.assetMetadata.assetId] = imageMetadataFile;
                        result = new AssetMetadata(displayName, imageMetadataFile.assetMetadata.assetId, AssetMetadata.AssetType.Image);
                        break;
                }

                return result;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
            return null;
        }

        /// <summary>
        /// Checks if there is a metadata file (.dclasset) that belongs to the given asset file.
        /// </summary>
        /// <param name="pathToAssetFile"></param>
        /// <returns></returns>
        private bool MetadataFileExists(string pathToAssetFile) => File.Exists(pathToAssetFile + ".dclasset");

        /// <summary>
        /// Writes the given metadata to a .dclasset file.
        /// </summary>
        /// <param name="metadata"></param>
        private void WriteMetadataToFile(AssetMetadataFile metadata)
        {
            try
            {
                switch (metadata.assetMetadata.assetType)
                {
                    case AssetMetadata.AssetType.Scene:
                        // TODO write scene metadata to file
                        break;
                    default:
                        var contents = new AssetMetadataFile.Contents(
                        new AssetMetadataFile.MetaContents(
                            metadata.assetFilename,
                            metadata.assetMetadata.assetId,
                            metadata.assetMetadata.assetType,
                            metadata.assetMetadata.assetDisplayName
                        ),
                        metadata.thumbnail);
                        string json = JsonConvert.SerializeObject(contents, Formatting.Indented);
                        File.WriteAllText(metadata.metadataFilePath, json);
                        break;
                }

            }
            catch (Exception e)
            {
                Debug.LogError($"Error while writing metadata to file: {e}");
            }
        }
        #endregion

        #region Asset Data related methods

        /// <summary>
        /// Provides the data of the asset with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="onLoadingComplete"></param>
        private AssetData LoadAssetData(Guid id)
        {
            try
            {
                if (assetMetadataCache.TryGetValue(id, out AssetMetadataFile file))
                {
                    switch (file.assetMetadata.assetType)
                    {
                        case AssetMetadata.AssetType.Image:
                            return LoadAndCacheImage(id);
                        case AssetMetadata.AssetType.Model:
                            return LoadAndCacheModel(id);
                        case AssetMetadata.AssetType.Scene:
                            return LoadAndCacheScene(id);
                        default:
                            break;
                    }
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while loading asset data: {e}");
            }
            return null;
        }

        /// <summary>
        /// Loads cached image data. If the cache doesn't contain the data yet, it gets cached.
        /// </summary>
        /// <param name="id"></param>
        private AssetData LoadAndCacheImage(Guid id)
        {
            try
            {
                if (assetDataCache.TryGetValue(id, out AssetData cachedAssetData))
                {
                    return cachedAssetData;
                }

                if (assetMetadataCache.TryGetValue(id, out AssetMetadataFile value))
                {
                    var imageBytes = File.ReadAllBytes(value.assetFilePath); // TODO make loading async
                    Texture2D image = new Texture2D(2, 2); // Texture gets resized when loading the image
                    ImageConversion.LoadImage(image, imageBytes);
                    var assetData = new ImageAssetData(id, image);

                    // Add to cache
                    assetDataCache[id] = assetData;

                    return assetData;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while loading image: {e}");
            }
            return null;
        }

        /// <summary>
        /// Creates a copy of a cached model. If the cache doesn't contain the model yet, it gets cached.
        /// </summary>
        /// <param name="id"></param>
        private AssetData LoadAndCacheModel(Guid id)
        {
            ModelAssetData CreateCopyOfCachedModel(ModelAssetData data)
            {
                GameObject copy = Object.Instantiate(data.data);
                copy.SetActive(true);
                copy.transform.SetParent(null);
                return new ModelAssetData(id, copy);
            }

            try
            {
                // Model already cached
                if (assetDataCache.TryGetValue(id, out AssetData cachedAssetData))
                {
                    if (cachedAssetData.state != AssetData.State.IsAvailable)
                    {
                        return cachedAssetData;
                    }

                    if (cachedAssetData is ModelAssetData modelData)
                    {
                        return CreateCopyOfCachedModel(modelData);
                    }
                }

                // Model not yet cached
                if (assetMetadataCache.TryGetValue(id, out AssetMetadataFile metadata))
                {
                    assetDataCache[id] = new AssetData(id, AssetData.State.IsLoading);

                    loadGltfFromFileSystem.LoadGltfFromPath(metadata.assetFilePath, go =>
                    {
                        var assetData = new ModelAssetData(id, go);
                        assetDataCache[id] = assetData;

                        var updatedIds = new List<Guid> { id };
                        editorEvents.InvokeAssetDataUpdatedEvent(updatedIds);
                    });

                    return new AssetData(id, AssetData.State.IsLoading);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while loading model: {e}");
            }
            return null;
        }
        
        /// <summary>
        /// Creates a copy of a cached scene. If the cache doesn't contain the scene yet, it gets cached.
        /// </summary>
        /// <param name="id"></param>
        private AssetData LoadAndCacheScene(Guid id)
        {
            if (assetDataCache.TryGetValue(id, out AssetData cachedAssetData))
            {
                return cachedAssetData;
            }
            
            var assetsOnScene = GetGltfAssetsRecursive(id);
            var assetData = new SceneAssetData(id, false, assetsOnScene);
            
            // Add to cache
            assetDataCache[id] = assetData;
            
            return assetData;
        }

        private Dictionary<Guid, bool> GetGltfAssetsRecursive(Guid sceneId)
        {
            var assetsOnScene = new Dictionary<Guid, bool>();
            var scene = sceneManagerSystem.GetScene(sceneId);
            
            foreach (var entity in scene.AllEntities.Concat(scene.AllFloatingEntities).Select(e => e.Value))
            {
                var isScene = entity
                                        .GetFirstComponentByName("Scene", "Scene")?
                                        .GetPropertyByName("scene")?
                                        .GetConcrete<Guid>().Value;

                if (isScene.HasValue)
                {
                    var assetsOnSubScene = GetGltfAssetsRecursive(isScene.Value);
                    assetsOnScene.AddRange(assetsOnSubScene);
                    continue;
                }
                
                var assetGuid = entity
                                        .GetComponentByName("GLTFShape")?
                                        .GetPropertyByName("asset")?
                                        .GetConcrete<Guid>().Value;
                
                if (assetGuid is null) continue;
                
                assetsOnScene[assetGuid.Value] = false;
            }
            
            return assetsOnScene;
        }
        
        #endregion
    }
}
