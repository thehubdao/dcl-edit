using Assets.Scripts.System;
using System;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

[System.Serializable]
public class LoadedModelFormat : AssetFormat, IThumbnailConvertible
{
    /// <summary>
    /// Reference to a model GameObject in the scene.
    /// </summary>
    public GameObject model;

    // Dependencies
    AssetThumbnailGeneratorSystem thumbnailGeneratorSystem;

    [Inject]
    public void Construct(AssetThumbnailGeneratorSystem thumbnailGeneratorSystem)
    {
        this.thumbnailGeneratorSystem = thumbnailGeneratorSystem;
    }


    public LoadedModelFormat(Guid id, GameObject model) : base(id)
    {
        this.model = model;
    }

    public async Task<ThumbnailFormat> ConvertToThumbnailFormat()
    {
        Texture2D thumbnail = await thumbnailGeneratorSystem.Generate(model);
        return new ThumbnailFormat(id, thumbnail);
    }

    public class Factory : PlaceholderFactory<Guid, GameObject, LoadedModelFormat>
    {
    }
}

public interface ILoadedModelConvertible
{
    Task<LoadedModelFormat> ConvertToLoadedModelFormat(LoadGltfFromFileSystem gltfLoader, BuilderAssetDownLoader downloader, LoadedModelFormat.Factory loadedModelFactory);
}
