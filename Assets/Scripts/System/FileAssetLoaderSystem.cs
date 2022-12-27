using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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

        private string relativePathInProject = "/assets"; // TODO: Change this, this is just for testing

        public Dictionary<Guid, AssetMetadataFile> assetMetadataCache => loaderState.assetMetadataCache;
        public Dictionary<Guid, AssetData> assetDataCache => loaderState.assetDataCache;

        [Inject]
        private void Construct(
            FileAssetLoaderState loaderState,
            PathState pathState,
            EditorEvents editorEvents,
            LoadGltfFromFileSystem loadGltfFromFileSystem,
            AssetThumbnailGeneratorSystem assetThumbnailGeneratorSystem)
        {
            this.loaderState = loaderState;
            this.pathState = pathState;
            this.editorEvents = editorEvents;
            this.loadGltfFromFileSystem = loadGltfFromFileSystem;
            this.assetThumbnailGeneratorSystem = assetThumbnailGeneratorSystem;
        }


        #region Public methods
        public void CacheAllAssetMetadata()
        {
            try
            {
                string directoryPath = pathState.ProjectPath + relativePathInProject;

                loaderState.assetHierarchy = ScanDirectory(directoryPath);
                /*
                string[] allFiles = Directory.GetFiles(directoryPath, "*.*", SearchOption.AllDirectories);  // Also search all child directories

                // Populate caches. Assets and their corresponding metadata files get added using their Guid as key.
                foreach (var assetFile in allFiles)
                {
                    if (IsMetadataFile(assetFile)) { continue; }

                    AssetMetadataFile metadataFile = ReadExistingMetadataFile(assetFile);

                    if (metadataFile == null)
                    {
                        metadataFile = GenerateMetadataFromAsset(assetFile);
                        WriteMetadataToFile(metadataFile);
                    }

                    assetMetadataCache[metadataFile.assetMetadata.assetId] = metadataFile;
                }
                */

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
            if (assetMetadataCache.TryGetValue(id, out var metadata))
            {
                if (metadata.thumbnail == null)
                {
                    assetThumbnailGeneratorSystem.Generate(id, thumbnail =>
                    {
                        metadata.thumbnail = thumbnail;
                        WriteMetadataToFile(metadata);

                        editorEvents.InvokeThumbnailDataUpdatedEvent(new List<Guid> { id });
                    });

                    return new AssetThumbnail(id, AssetData.State.IsLoading, null); // Thumbnail needs to be generated
                }

                return new AssetThumbnail(id, AssetData.State.IsAvailable, metadata.thumbnail); // Thumbnail is available
            }

            return null; // sorry but this id is in another loader system. Mamma Mia!
        }

        public AssetData GetDataById(Guid id)
        {
            if (!assetMetadataCache.ContainsKey(id))
            {
                return null;
            }

            return LoadAssetData(id);
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
        private AssetHierarchyItem ScanDirectory(string fileSystemPath, string pathInHierarchy = "")
        {
            string dirname = Path.GetFileName(fileSystemPath);
            string[] files = Directory.GetFiles(fileSystemPath, "*.*");
            string[] subdirs = Directory.GetDirectories(fileSystemPath);

            pathInHierarchy += "/" + dirname;

            List<AssetMetadata> assets = new List<AssetMetadata>();
            List<AssetHierarchyItem> childDirectories = new List<AssetHierarchyItem>();

            foreach (string assetFile in files)
            {
                // Populate caches. Assets and their corresponding metadata files get added using their Guid as key.
                if (IsMetadataFile(assetFile)) { continue; }
                AssetMetadataFile metadataFile = ReadExistingMetadataFile(assetFile);

                if (metadataFile == null)
                {
                    metadataFile = GenerateMetadataFromAsset(assetFile);
                    WriteMetadataToFile(metadataFile);
                }

                assetMetadataCache[metadataFile.assetMetadata.assetId] = metadataFile;
                assets.Add(metadataFile.assetMetadata);
            }

            foreach (string subdir in subdirs)
            {
                childDirectories.Add(ScanDirectory(subdir, pathInHierarchy));
            }

            return new AssetHierarchyItem(dirname, pathInHierarchy, childDirectories, assets);
        }

        /// <summary>
        /// Generates a new metadata object (with a new Guid) for the given asset.
        /// </summary>
        /// <param name="assetFilePath"></param>
        /// <returns></returns>
        private AssetMetadataFile GenerateMetadataFromAsset(string assetFilePath)
        {
            try
            {
                var assetFilename = Path.GetFileName(assetFilePath);
                var fileExtension = Path.GetExtension(assetFilePath);
                Guid assetId = Guid.NewGuid();

                AssetMetadata.AssetType assetType;
                switch (fileExtension)
                {
                    case ".glb":
                        assetType = AssetMetadata.AssetType.Model;
                        break;
                    case ".gltf":
                        assetType = AssetMetadata.AssetType.Model;
                        break;
                    case ".png":
                        assetType = AssetMetadata.AssetType.Image;
                        break;
                    default:
                        assetType = AssetMetadata.AssetType.Unknown;
                        break;
                }

                return new AssetMetadataFile(
                    Path.ChangeExtension(assetFilePath, ".dclasset"),
                    assetFilename,
                    new AssetMetadata(
                        Path.GetFileNameWithoutExtension(assetFilename),
                        assetId,
                        assetType)
                // Thumbnail will be added later by the thumbnail generator
                );
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while generating metadata: {e}");
            }
            return null;
        }

        /// <summary>
        /// Writes the given metadata to a .dclasset file.
        /// </summary>
        /// <param name="metadata"></param>
        private void WriteMetadataToFile(AssetMetadataFile metadata)
        {
            try
            {
                var contents = new AssetMetadataFile.Contents(
                    new AssetMetadataFile.MetaContents(
                        metadata.assetFilename,
                        metadata.assetMetadata.assetId,
                        metadata.assetMetadata.assetType,
                        metadata.assetMetadata.assetDisplayName
                    ),
                    metadata.thumbnail);

                string json = JsonConvert.SerializeObject(contents);

                File.WriteAllText(metadata.metadataFilePath, json);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while writing metadata to file: {e}");
            }
        }

        /// <summary>
        /// Tries to find and read the given metadata file. It's also possible to specify the path to an asset file as
        /// the corresponding metadata file path will be automatically determined.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private AssetMetadataFile ReadExistingMetadataFile(string filePath)
        {
            try
            {
                // Make sure the path leads to a metadata file
                var metadataFilePath = Path.ChangeExtension(filePath, ".dclasset");

                if (!File.Exists(metadataFilePath))
                {
                    return null;
                }

                var json = File.ReadAllText(metadataFilePath);
                var contents = JsonConvert.DeserializeObject<AssetMetadataFile.Contents>(json);
                return new AssetMetadataFile(contents, metadataFilePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while reading existing metadata file: {e}");
            }
            return null;
        }

        private bool IsMetadataFile(string pathToFile) => Path.GetExtension(pathToFile) == ".dclasset";
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
        #endregion
    }
}