using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityGLTF;
using UnityGLTF.Loader;
using Zenject;

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
        public Dictionary<Guid, FileAssetData> AssetDataCache => _loaderState.assetDataCache;

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
                        var metadata = GenerateMetadataFromAsset(assetFile);
                        metadataFile = WriteMetadataToFile(metadata, Path.GetDirectoryName(assetFile));
                    }

                    AssetMetadataCache[metadataFile.contents.metadata.assetId] = metadataFile;
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
                var metadata = file.contents.metadata;
                var assetType = metadata.assetType switch
                {
                    FileAssetMetadata.AssetType.Unknown => AssetMetadata.AssetType.Unknown,
                    FileAssetMetadata.AssetType.Model => AssetMetadata.AssetType.Model,
                    FileAssetMetadata.AssetType.Image => AssetMetadata.AssetType.Image,
                    _ => throw new ArgumentOutOfRangeException()
                };
                return new AssetMetadata(metadata.assetDisplayName, metadata.assetId, assetType);
            }
            return null;
        }

        public AssetThumbnail GetThumbnailById(Guid id)
        {
            throw new NotImplementedException();
        }

        public AssetData GetDataById(Guid id)
        {
            if (!AssetMetadataCache.ContainsKey(id))
            {
                return null;
            }

            var fileAssetData = LoadAssetData(id);
            AssetData assetData = fileAssetData switch
            {
                ImageFileAssetData imageFileAssetData => new ImageAssetData(imageFileAssetData.id, imageFileAssetData.data),
                ModelFileAssetData modelFileAssetData => new ModelAssetData(modelFileAssetData.id, modelFileAssetData.data),
                _ => throw new ArgumentOutOfRangeException(nameof(fileAssetData))
            };

            return assetData;
        }

        #endregion


        #region Metadata related methods
        /// <summary>
        /// Generates a new metadata object (with a new Guid) for the given asset.
        /// </summary>
        /// <param name="assetFilePath"></param>
        /// <returns></returns>
        private FileAssetMetadata GenerateMetadataFromAsset(string assetFilePath)
        {
            try
            {
                var assetFilename = Path.GetFileName(assetFilePath);
                var fileExtension = Path.GetExtension(assetFilePath);
                Guid assetId = Guid.NewGuid();

                FileAssetMetadata.AssetType assetType;
                switch (fileExtension)
                {
                    case ".glb":
                        assetType = FileAssetMetadata.AssetType.Model;
                        break;
                    case ".gltf":
                        assetType = FileAssetMetadata.AssetType.Model;
                        break;
                    case ".png":
                        assetType = FileAssetMetadata.AssetType.Image;
                        break;
                    default:
                        assetType = FileAssetMetadata.AssetType.Unknown;
                        break;
                }

                return new FileAssetMetadata
                {
                    assetFilename = assetFilename,
                    assetId = assetId,
                    assetType = assetType
                    // Thumbnail will be added later by the thumbnail generator
                };
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while generating metadata: {e}");
            }
            return null;
        }

        /// <summary>
        /// Writes the given metadata to a .dclasset file in the given directory.
        /// </summary>
        /// <param name="metadata"></param>
        /// <returns></returns>
        private AssetMetadataFile WriteMetadataToFile(FileAssetMetadata metadata, string targetDirectoryPath)
        {
            try
            {
                // Make sure this path points to a directory and not a file
                if (!Directory.Exists(targetDirectoryPath))
                {
                    targetDirectoryPath = Path.GetDirectoryName(targetDirectoryPath);
                }

                var filename = Path.ChangeExtension(metadata.assetFilename, ".dclasset");
                var fullFilePath = Path.Combine(targetDirectoryPath, filename);
                var contents = new AssetMetadataFile.Contents { metadata = metadata };
                string json = JsonConvert.SerializeObject(contents);
                File.WriteAllText(fullFilePath, json);
                return new AssetMetadataFile(contents, fullFilePath);
            }
            catch (Exception e)
            {
                Debug.LogError($"Error while writing metadata to file: {e}");
            }
            return null;
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
        private FileAssetData LoadAssetData(Guid id)
        {
            try
            {
                if (AssetMetadataCache.TryGetValue(id, out AssetMetadataFile file))
                {
                    switch (file.contents.metadata.assetType)
                    {
                        case FileAssetMetadata.AssetType.Image:
                            return LoadAndCacheImage(id);
                        case FileAssetMetadata.AssetType.Model:
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
        /// <param name="onLoadingComplete"></param>
        private FileAssetData LoadAndCacheImage(Guid id)
        {
            try
            {
                if (AssetDataCache.TryGetValue(id, out FileAssetData cachedAssetData))
                {
                    return cachedAssetData;
                }

                if (AssetMetadataCache.TryGetValue(id, out AssetMetadataFile value))
                {
                    var imageBytes = File.ReadAllBytes(value.AssetFilePath);        // TODO make loading async
                    Texture2D image = new Texture2D(2, 2);        // Texture gets resized when loading the image
                    ImageConversion.LoadImage(image, imageBytes);
                    var assetData = new ImageFileAssetData(id, image);

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
        /// <param name="onLoadingComplete"></param>
        private FileAssetData LoadAndCacheModel(Guid id)
        {
            ModelFileAssetData CreateCopyOfCachedModel(ModelFileAssetData data)
            {
                GameObject copy = GameObject.Instantiate(data.data);
                copy.SetActive(true);
                copy.transform.SetParent(null);
                return new ModelFileAssetData(id, copy);
            }

            try
            {
                // Model already cached
                if (AssetDataCache.TryGetValue(id, out FileAssetData cachedAssetData))
                {
                    if (cachedAssetData.state != AssetData.State.IsAvailable)
                    {
                        return cachedAssetData;
                    }
                    if (cachedAssetData is ModelFileAssetData modelData)
                    {
                        return CreateCopyOfCachedModel(modelData);
                    }
                }

                // Model not yet cached
                if (AssetMetadataCache.TryGetValue(id, out AssetMetadataFile metadata))
                {
                    AssetDataCache[id] = new AssetData(id, AssetData.State.IsLoading);
                    
                    _loadGltfFromFileSystem.LoadGltfFromPath(metadata.AssetFilePath, go =>
                    {
                        var assetData = new ModelFileAssetData(id, go);
                        AssetDataCache[id] = assetData;

                        var updatedIds = new List<Guid> {id};
                        _editorEvents.InvokeAssetDataUpdatedEvent(updatedIds);
                    });

                    return new FileAssetData(id, FileAssetData.State.IsLoading);
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