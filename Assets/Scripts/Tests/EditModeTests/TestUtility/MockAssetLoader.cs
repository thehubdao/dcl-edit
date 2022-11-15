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
        public FileAssetMetadata.AssetType type;
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
                        metadata = new FileAssetMetadata
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
                case FileAssetMetadata.AssetType.Unknown:
                    break;
                case FileAssetMetadata.AssetType.Model:
                    loaderState.assetDataCache.Add(t.id, new ModelFileAssetData(t.id, new GameObject(t.filename)));
                    break;
                case FileAssetMetadata.AssetType.Image:
                    loaderState.assetDataCache.Add(t.id, new ImageFileAssetData(t.id, new Texture2D(2, 2)));
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
        if (loaderState.assetDataCache.TryGetValue(id, out FileAssetData fileData))
        {
            AssetData data = fileData switch
            {
                ImageFileAssetData imageFileAssetData => new ImageAssetData(imageFileAssetData.id, imageFileAssetData.data),
                ModelFileAssetData modelFileAssetData => new ModelAssetData(modelFileAssetData.id, modelFileAssetData.data),
                _ => throw new ArgumentOutOfRangeException(nameof(fileData))
            };

            return data;
        }
        return null;
    }

    public AssetMetadata GetMetadataById(Guid id)
    {
        if (loaderState.assetMetadataCache.TryGetValue(id, out AssetMetadataFile file))
        {
            var fileAssetMetadata = file.contents.metadata;

            var type = fileAssetMetadata.assetType switch
            {
                FileAssetMetadata.AssetType.Unknown => AssetMetadata.AssetType.Unknown,
                FileAssetMetadata.AssetType.Model => AssetMetadata.AssetType.Model,
                FileAssetMetadata.AssetType.Image => AssetMetadata.AssetType.Image,
                _ => throw new ArgumentOutOfRangeException()
            };

            return new AssetMetadata(fileAssetMetadata.assetDisplayName, fileAssetMetadata.assetId, type);
        }
        return null;
    }

    public AssetThumbnail GetThumbnailById(Guid id)
    {
        throw new NotImplementedException();
    }
}
