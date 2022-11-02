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
        private FileAssetLoaderState _loaderState;
        private PathState _pathState; // TODO: Change this
        private EditorEvents _editorEvents;
        private LoadGltfFromFileSystem _loadGltfFromFileSystem;

        private string relativePathInProject = "/assets"; // TODO: Change this, this is just for testing

        public Dictionary<Guid, AssetMetadataFile> AssetMetadataCache => _loaderState.assetMetadataCache;
        public Dictionary<Guid, AssetData> AssetDataCache => _loaderState.assetDataCache;

        [Inject]
        private void Construct(FileAssetLoaderState loaderState, PathState pathState, EditorEvents editorEvents, LoadGltfFromFileSystem loadGltfFromFileSystem)
        {
            _loaderState = loaderState;
            _pathState = pathState;
            _editorEvents = editorEvents;
            _loadGltfFromFileSystem = loadGltfFromFileSystem;
        }


        #region Public methods
        public void CacheAllAssetMetadata()
        {
            try
            {
                string directoryPath = _pathState.ProjectPath + relativePathInProject;
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

                    AssetMetadataCache[metadataFile.assetMetadata.assetId] = metadataFile;
                }

                _editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
            }
            catch (Exception e)
            {
                Debug.Log($"Error while caching assets: {e}");
            }
        }

        public IEnumerable<Guid> GetAllAssetIds() => AssetMetadataCache.Keys;

        public AssetMetadata GetMetadataById(Guid id)
        {
            if (AssetMetadataCache.TryGetValue(id, out AssetMetadataFile file))
            {
                return file.assetMetadata;
            }
            return null;
        }

        public Texture2D GetThumbnailById(Guid id) => AssetMetadataCache[id]?.contents.Thumbnail;

        public AssetData GetDataById(Guid id)
        {
            if (!AssetMetadataCache.ContainsKey(id))
            {
                return null;
            }

            return LoadAssetData(id);
        }

        public bool SetThumbnailById(Guid id, Texture2D newThumbnail)
        {
            if (newThumbnail == null) return false;

            if (AssetMetadataCache.TryGetValue(id, out AssetMetadataFile metaFile))
            {
                metaFile.contents.Thumbnail = newThumbnail;
                WriteMetadataToFile(Path.GetDirectoryName(metaFile.metadataFilePath), metaFile.contents.Metadata, newThumbnail);
                _editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
                return true;
            }
            return false;
        }
        #endregion

        #region Metadata related methods

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
                    null);

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
                if (AssetMetadataCache.TryGetValue(id, out AssetMetadataFile file))
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
                if (AssetDataCache.TryGetValue(id, out AssetData cachedAssetData))
                {
                    return cachedAssetData;
                }

                if (AssetMetadataCache.TryGetValue(id, out AssetMetadataFile value))
                {
                    var imageBytes = File.ReadAllBytes(value.assetFilePath); // TODO make loading async
                    Texture2D image = new Texture2D(2, 2);        // Texture gets resized when loading the image
                    ImageConversion.LoadImage(image, imageBytes);
                    var assetData = new ImageAssetData(id, image);

                    // Add to cache
                    AssetDataCache[id] = assetData;

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
                if (AssetDataCache.TryGetValue(id, out AssetData cachedAssetData))
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
                if (AssetMetadataCache.TryGetValue(id, out AssetMetadataFile metadata))
                {
                    AssetDataCache[id] = new AssetData(id, AssetData.State.IsLoading);

                    _loadGltfFromFileSystem.LoadGltfFromPath(metadata.assetFilePath, go =>
                    {
                        var assetData = new ModelAssetData(id, go);
                        AssetDataCache[id] = assetData;

                        var updatedIds = new List<Guid> {id};
                        _editorEvents.InvokeAssetDataUpdatedEvent(updatedIds);
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