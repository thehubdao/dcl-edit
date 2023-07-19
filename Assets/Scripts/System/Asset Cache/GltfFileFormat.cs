using Assets.Scripts.System;
using System;
using System.Threading.Tasks;
using Zenject;

[System.Serializable]
public class GltfFileFormat : AssetFormat, ILoadedModelConvertible, IThumbnailConvertible
{
    public string pathToFile;

    // Dependencies
    LoadGltfFromFileSystem gltfLoader;
    LoadedModelFormat.Factory loadedModelFormatFactory;
    AssetThumbnailGeneratorSystem assetThumbnailGeneratorSystem;

    [Inject]
    public void Construct(LoadGltfFromFileSystem gltfLoader, LoadedModelFormat.Factory loadedModelFormatFactory, AssetThumbnailGeneratorSystem assetThumbnailGeneratorSystem)
    {
        this.gltfLoader = gltfLoader;
        this.loadedModelFormatFactory = loadedModelFormatFactory;
        this.assetThumbnailGeneratorSystem = assetThumbnailGeneratorSystem;
    }

    public GltfFileFormat(Guid id, string pathToFile) : base(id)
    {
        this.pathToFile = pathToFile;
    }

    public async Task<LoadedModelFormat> ConvertToLoadedModelFormat(LoadedModelFormat.Factory loadedModelFactory)
    {
        var tcs = new TaskCompletionSource<LoadedModelFormat>();

        gltfLoader.LoadGltfFromPath(pathToFile, go =>
        {
            var loadedModelFormat = loadedModelFactory.Create(id, go);
            tcs.TrySetResult(loadedModelFormat);
        });

        return await tcs.Task;
    }

    public async Task<ThumbnailFormat> ConvertToThumbnailFormat()
    {
        var loadedFormat = await ConvertToLoadedModelFormat(loadedModelFormatFactory);
        var model = loadedFormat.model;
        var thumbnail = await assetThumbnailGeneratorSystem.Generate(model);
        return new ThumbnailFormat(id, thumbnail);
    }

    public class Factory : PlaceholderFactory<Guid, string, GltfFileFormat>
    {
    }
}
