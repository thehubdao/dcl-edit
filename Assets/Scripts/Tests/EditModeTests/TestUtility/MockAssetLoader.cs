using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Tests.EditModeTests.TestUtility
{
    public class MockAssetLoader : IAssetLoaderSystem
    {
        private FileAssetLoaderState loaderState = new FileAssetLoaderState();
        public bool allAssetsCached = false;

        public struct TestData
        {
            public Guid id;
            public string filename;
            public AssetMetadata.AssetType type;
        }

        public MockAssetLoader(params TestData[] testData)
        {
            foreach (var t in testData)
            {
                loaderState.assetMetadataCache.Add(
                    t.id,
                    new AssetMetadataFile(
                        new AssetMetadataFile.Contents
                        {
                            metadata = new AssetMetadataFile.MetaContents()
                            {
                                assetFilename = t.filename,
                                assetId = t.id,
                                assetType = t.type
                            }
                        },
                        $"/{t.filename}.dclasset"
                    )
                );

                switch (t.type)
                {
                    case AssetMetadata.AssetType.Unknown:
                        break;
                    case AssetMetadata.AssetType.Model:
                        loaderState.assetDataCache.Add(t.id, new ModelAssetData(t.id, new GameObject(t.filename)));
                        break;
                    case AssetMetadata.AssetType.Image:
                        loaderState.assetDataCache.Add(t.id, new ImageAssetData(t.id, new Texture2D(2, 2)));
                        break;
                    default:
                        break;
                }
            }
        }

        public void ClearAllData() => allAssetsCached = false;

        public void CacheAllAssetMetadata()
        {
            allAssetsCached = true;
        }

        public AssetHierarchyItem GetHierarchy()
        {
            return new AssetHierarchyItem();
        }

        public IEnumerable<Guid> GetAllAssetIds() => loaderState.assetMetadataCache.Keys;

        public AssetData GetDataById(Guid id)
        {
            if (loaderState.assetDataCache.TryGetValue(id, out AssetData fileData))
            {
                AssetData data = fileData switch
                {
                    ImageAssetData imageFileAssetData => new ImageAssetData(imageFileAssetData.id, imageFileAssetData.data),
                    ModelAssetData modelFileAssetData => new ModelAssetData(modelFileAssetData.id, modelFileAssetData.data),
                    _ => throw new ArgumentOutOfRangeException(nameof(fileData))
                };

                return data;
            }

            return null;
        }

        public Task<string> CopyAssetTo(Guid id)
        {
            return null;
        }

        public AssetData GetOnlyAssetDataById(Guid id)
        {
            if (loaderState.assetDataCache.TryGetValue(id, out var assetData))
                return assetData;

            return GetDataById(id);
        }

        public AssetMetadata GetMetadataById(Guid id)
        {
            if (loaderState.assetMetadataCache.TryGetValue(id, out AssetMetadataFile file))
            {
                return file.assetMetadata;
            }

            return null;
        }

        public AssetThumbnail GetThumbnailById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
