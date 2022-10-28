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
        private PathState _pathState;                           // TODO: Change this
        private EditorEvents _editorEvents;
        private UnityState _unityState;

        private string relativePathInProject = "/assets";       // TODO: Change this, this is just for testing

        public Dictionary<Guid, AssetMetadataFile> AssetMetadataCache => _loaderState.assetMetadataCache;
        public Dictionary<Guid, AssetData> AssetDataCache => _loaderState.assetDataCache;

        [Inject]
        private void Construct(FileAssetLoaderState loaderState, PathState pathState, EditorEvents editorEvents, UnityState unityState)
        {
            _loaderState = loaderState;
            _pathState = pathState;
            _editorEvents = editorEvents;
            _unityState = unityState;
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
                return file.contents.metadata;
            }
            return null;
        }

        public Texture2D GetThumbnailById(Guid id)
        {
            throw new NotImplementedException();
        }

        public AssetData GetDataById(Guid id)
        {
            if (!AssetMetadataCache.ContainsKey(id)) { return null; }
            return LoadAssetData(id);
        }
        #endregion


        #region Metadata related methods
        /// <summary>
        /// Generates a new metadata object (with a new Guid) for the given asset.
        /// </summary>
        /// <param name="assetFilePath"></param>
        /// <returns></returns>
        private AssetMetadata GenerateMetadataFromAsset(string assetFilePath)
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

                return new AssetMetadata
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
        private AssetMetadataFile WriteMetadataToFile(AssetMetadata metadata, string targetDirectoryPath)
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
        private AssetData LoadAssetData(Guid id)
        {
            try
            {
                if (AssetMetadataCache.TryGetValue(id, out AssetMetadataFile file))
                {
                    switch (file.contents.metadata.assetType)
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
        /// <param name="onLoadingComplete"></param>
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
                    var imageBytes = File.ReadAllBytes(value.AssetFilePath);        // TODO make loading async
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
        /// <param name="onLoadingComplete"></param>
        private AssetData LoadAndCacheModel(Guid id)
        {
            ModelAssetData CreateCopyOfCachedModel(ModelAssetData data)
            {
                GameObject copy = GameObject.Instantiate(data.data);
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

                    var options = new ImportOptions()
                    {
                        DataLoader = new FileLoader(URIHelper.GetDirectoryName(metadata.AssetFilePath)),
                        AsyncCoroutineHelper = _unityState.AsyncCoroutineHelper
                    };

                    var importer = new GLTFSceneImporter(metadata.AssetFilePath, options);
                    importer.CustomShaderName = "Shader Graphs/GLTFShader";

                    _unityState.AsyncCoroutineHelper.StartCoroutine(importer.LoadScene(
                        onLoadComplete: (o, info) =>
                        {
                            if (o == null)
                            {
                                Debug.LogError(info.SourceException.Message + "\n" + info.SourceException.StackTrace);
                                return;
                            }

                            // Prepare loaded model
                            // reset transform
                            o.transform.localPosition = Vector3.zero;
                            o.transform.localScale = Vector3.one;
                            o.transform.localRotation = Quaternion.identity;

                            var allTransforms = new List<Transform>(); // All loaded transforms including all children of any level

                            // Fill the allTransforms List
                            var stack = new Stack<Transform>();
                            stack.Push(o.transform);
                            while (stack.Any())
                            {
                                var next = stack.Pop();
                                allTransforms.Add(next);

                                foreach (var child in next.GetChildren())
                                    stack.Push(child);
                            }

                            // Make all decentraland collider invisible
                            allTransforms
                                .Where(t => t.name.EndsWith("_collider"))
                                .Where(t => t.TryGetComponent<MeshFilter>(out _))
                                .ForAll(t => t.gameObject.SetActive(false));

                            // Find all transforms of visible GameObjects
                            var visibleChildren = allTransforms
                                .Where(t => !t.name.EndsWith("_collider"))
                                .Where(t => t.TryGetComponent<MeshFilter>(out _) || t.TryGetComponent<SkinnedMeshRenderer>(out _));


                            // Add click collider to all visible GameObjects
                            foreach (var child in visibleChildren)
                            {
                                var colliderGameObject = new GameObject("Collider");
                                colliderGameObject.transform.parent = o.transform;
                                colliderGameObject.transform.position = child.position;
                                colliderGameObject.transform.rotation = child.rotation;
                                colliderGameObject.transform.localScale = child.localScale;

                                colliderGameObject.layer = 10; // Entity Click Layer
                                var newCollider = colliderGameObject.AddComponent<MeshCollider>();

                                if (child.TryGetComponent<MeshFilter>(out var meshFilter))
                                    newCollider.sharedMesh = meshFilter.sharedMesh;

                                if (child.TryGetComponent<SkinnedMeshRenderer>(out var skinnedMeshRenderer))
                                    newCollider.sharedMesh = skinnedMeshRenderer.sharedMesh;
                            }

                            // Add the loaded model to cache. Copies of it will be created when the cache is used.
                            GameObject parent = GameObject.Find("ModelCache") ?? new GameObject("ModelCache");
                            parent.transform.position = new Vector3(0, -5000, 0);           // Place out of sight to avoid the user seeing objects get instantiated
                            o.transform.SetParent(parent.transform);
                            o.SetActive(false);
                            var assetData = new ModelAssetData(id, o);
                            AssetDataCache[id] = assetData;


                            var updatedIds = new List<Guid>();
                            updatedIds.Add(id);
                            _editorEvents.InvokeAssetDataUpdatedEvent(updatedIds);
                        }));
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