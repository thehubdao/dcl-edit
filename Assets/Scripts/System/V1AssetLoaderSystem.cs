using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Assets.Scripts.ClassModels;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System.Utility;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

namespace Assets.Scripts.System
{
    public class V1AssetLoaderSystem : IAssetLoaderSystem
    {
        // Dependencies
        private LoadGltfFromFileSystem loadGltfFromFileSystem;
        private PathState pathState;
        private FileIOUtility fileUtility;
        private FileAssetLoaderState loaderState;
        private AssetThumbnailGeneratorSystem assetThumbnailGeneratorSystem;
        private EditorEvents editorEvents;

        private const string sectionName = "V1Assets";
        
        private Dictionary<Guid, AssetMetadataFile> assetMetadataCache => loaderState.assetMetadataCache;
        private Dictionary<Guid, AssetData> assetDataCache => loaderState.assetDataCache;

        [Inject]
        private void Construct(
            LoadGltfFromFileSystem loadGltfFromFileSystem,
            PathState pathState,
            FileIOUtility fileUtility,
            FileAssetLoaderState loaderState,
            AssetThumbnailGeneratorSystem assetThumbnailGeneratorSystem,
            EditorEvents editorEvents)
        {
            this.pathState = pathState;
            this.loadGltfFromFileSystem = loadGltfFromFileSystem;
            this.fileUtility = fileUtility;
            this.loaderState = loaderState;
            this.assetThumbnailGeneratorSystem = assetThumbnailGeneratorSystem;
            this.editorEvents = editorEvents;
        }

        public void ClearAllData()
        {
            loaderState.assetDataCache.Clear();
            loaderState.assetMetadataCache.Clear();
            loaderState.assetHierarchy = new AssetHierarchyItem();
        }

        public void CacheAllAssetMetadata()
        {
            ClearAllData();

            // If doesnt exist just return nulls or nothing
            // Not V1 version
            if (!fileUtility.CheckFileExists(pathState.ProjectPath + GlobalConstants.V1Paths.save)) return;

            var assetSaveFilePath = pathState.ProjectPath + GlobalConstants.V1Paths.assets;

            // Read asset.json file from V1
            var (assetsDidParse, assetsJson) = fileUtility.ReadFileToJson<AssetsJsonWrapper>(assetSaveFilePath);
            if (!assetsDidParse) return;

            var assets = new List<AssetMetadata>();

            foreach (var asset in assetsJson.gltfAssets)
            {
                AssetMetadata result;
                var assetPath = $"{pathState.ProjectPath}/{asset.gltfPath}";

                if (MetadataFileExists(assetPath))
                {
                    var (contentsDidParse, contents) =
                        fileUtility.ReadFileToJson<AssetMetadataFile.Contents>(assetPath,
                            GlobalConstants.Extensions.dclAsset);
                    if (!contentsDidParse) continue;

                    var (didChangeExt, newAssetPath) =
                        fileUtility.ChangeExtension(assetPath, GlobalConstants.Extensions.dclAsset);
                    if (!didChangeExt) continue;
                    
                    assetMetadataCache[contents.metadata.assetId] = new AssetMetadataFile(contents, newAssetPath);

                    result = new AssetMetadata(contents.metadata.assetDisplayName,
                        contents.metadata.assetId,
                        contents.metadata.assetType);
                }
                else
                {
                    var assetId = Guid.TryParse(asset.id, out var parsedId) ? parsedId : Guid.NewGuid();
                    var (didChangeExt, newAssetPath) =
                        fileUtility.ChangeExtension(assetPath, GlobalConstants.Extensions.dclAsset);
                    if (!didChangeExt) continue;
                    
                    var (didGetName, assetFileName) = fileUtility.GetFileName(asset.gltfPath);
                    if (!didGetName) continue;
                    
                    var modelMetadataFile = new AssetMetadataFile(
                        newAssetPath,
                        assetFileName,
                        new AssetMetadata(asset.name, assetId, AssetMetadata.AssetType.Model));
                    WriteMetadataToFile(modelMetadataFile);
                    assetMetadataCache[assetId] = modelMetadataFile;
                    result = new AssetMetadata(asset.name, assetId, AssetMetadata.AssetType.Model);
                }

                assets.Add(result);
            }
            
            loaderState.assetHierarchy =
                new AssetHierarchyItem(sectionName, $"/{sectionName}", new List<AssetHierarchyItem>(), assets);
            editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
        }

        private bool MetadataFileExists(string pathToAssetFile) =>
            fileUtility.CheckFileExists(pathToAssetFile, GlobalConstants.Extensions.dclAsset);

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
            return assetMetadataCache.TryGetValue(id, out var file) ? file.assetMetadata : null;
        }

        public AssetThumbnail GetThumbnailById(Guid id)
        {
            if (!assetMetadataCache.TryGetValue(id, out var metadata)) return null;

            if (metadata.thumbnail != null)
            {
                return new AssetThumbnail(id, AssetData.State.IsAvailable,
                    metadata.thumbnail); // Thumbnail is available
            }

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

        private void WriteMetadataToFile(AssetMetadataFile metadata)
        {
            switch (metadata.assetMetadata.assetType)
            {
                case AssetMetadata.AssetType.Scene:
                    // TODO: maybe remove switch
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
                    var json = JsonConvert.SerializeObject(contents, Formatting.Indented);
                    fileUtility.WriteToFile(metadata.metadataFilePath, json);
                    break;
            }
        }

        public AssetData GetDataById(Guid id)
        {
            return !assetMetadataCache.ContainsKey(id) ? null : LoadAssetData(id);
        }

        private AssetData LoadAssetData(Guid id)
        {
            if (!assetMetadataCache.TryGetValue(id, out AssetMetadataFile file)) return null;

            switch (file.assetMetadata.assetType)
            {
                case AssetMetadata.AssetType.Image:
                    return LoadAndCacheImage(id);
                case AssetMetadata.AssetType.Model:
                    return LoadAndCacheModel(id);
            }

            return null;
        }

        private AssetData LoadAndCacheImage(Guid id)
        {
            if (assetDataCache.TryGetValue(id, out var cachedAssetData))
                return cachedAssetData;

            if (!assetMetadataCache.TryGetValue(id, out var value)) return null;
            
            var (didRead, imageBytes) = fileUtility.ReadAllBytes(value.assetFilePath);
            if (!didRead) return null;
            
            var image = new Texture2D(2, 2); // Texture gets resized when loading the image
            image.LoadImage(imageBytes);

            var assetData = new ImageAssetData(id, image);

            // Add to cache
            assetDataCache[id] = assetData;

            return assetData;
        }

        private AssetData LoadAndCacheModel(Guid id)
        {
            // Model already cached
            if (assetDataCache.TryGetValue(id, out var cachedAssetData))
            {
                if (cachedAssetData.state != AssetData.State.IsAvailable)
                {
                    return cachedAssetData;
                }

                if (cachedAssetData is ModelAssetData modelData)
                {
                    return CreateCopyOfCachedModel(modelData, id);
                }
            }

            // Model not yet cached
            if (assetMetadataCache.TryGetValue(id, out var metadata))
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

            return null;
        }
        
        private ModelAssetData CreateCopyOfCachedModel(ModelAssetData data, Guid id)
        {
            var copy = Object.Instantiate(data.data, null, true);
            copy.SetActive(true);
            return new ModelAssetData(id, copy);
        }

        public Task<string> CopyAssetTo(Guid id)
        {
            if (loaderState.assetMetadataCache.TryGetValue(id, out var metadataFile))
            {
                return Task.FromResult(StaticUtilities.MakeRelativePath(pathState.ProjectPath,
                    metadataFile.assetFilePath));
            }

            return Task.FromResult<string>(null);
        }
    }
}