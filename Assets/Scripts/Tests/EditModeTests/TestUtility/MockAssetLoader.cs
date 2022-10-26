using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using System;
using System.Collections.Generic;
using UnityEngine;

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
                        metadata = new AssetMetadata
                        {
                            assetFilename = t.filename,
                            assetId = t.id,
                            assetType = t.type
                        }
                    },
                    $"/{t.filename}"
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

    public void CacheAllAssetMetadata()
    {
        allAssetsCached = true;
    }

    public IEnumerable<Guid> GetAllAssetIds() => loaderState.assetMetadataCache.Keys;

    public AssetData GetDataById(Guid id)
    {
        if (loaderState.assetDataCache.TryGetValue(id, out AssetData data))
        {
            return data;
        }
        return null;
    }

    public AssetMetadata GetMetadataById(Guid id)
    {
        if (loaderState.assetMetadataCache.TryGetValue(id, out AssetMetadataFile file))
        {
            return file.contents.metadata;
        }
        return null;
    }

    public Texture2D GetThumbnailById(Guid id)
    {
        throw new NotImplementedException();
    }
}
