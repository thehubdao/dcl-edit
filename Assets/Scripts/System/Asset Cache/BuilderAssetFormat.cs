using Assets.Scripts.System;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

/// <summary>
/// This format represents a Builder asset that has not yet been downloaded.
/// </summary>
public class BuilderAssetFormat : AssetFormat, ILoadedModelConvertible, IThumbnailConvertible, ISpriteConvertible
{
    // Dependencies
    LoadGltfFromFileSystem gltfLoader;
    BuilderAssetDownLoader downloader;

    public string Name;
    public string modelPath;
    public Dictionary<string, string> contentsPathToHash;
    public string ThumbnailHash;


    [Inject]
    public void Construct(LoadGltfFromFileSystem gltfLoader, BuilderAssetDownLoader downloader)
    {
        this.gltfLoader = gltfLoader;
        this.downloader = downloader;
    }

    public BuilderAssetFormat(Guid id, string name, string modelPath, Dictionary<string, string> contentsPathToHash, string thumbnailHash) : base(id)
    {
        this.Name = name;
        this.modelPath = modelPath;
        this.contentsPathToHash = contentsPathToHash;
        this.ThumbnailHash = thumbnailHash;
    }

    public async Task<LoadedModelFormat> ConvertToLoadedModelFormat(LoadedModelFormat.Factory loadedModelFactory)
    {
        var tcs = new TaskCompletionSource<LoadedModelFormat>();
        gltfLoader.LoadGltfFromPath(Path.GetFileName(modelPath), go =>
        {
            var loadedModelFormat = loadedModelFactory.Create(id, go);
            tcs.TrySetResult(loadedModelFormat);
        }, new BuilderAssetGltfDataLoader(Path.GetDirectoryName(modelPath!), contentsPathToHash, downloader));
        return await tcs.Task;
    }

    public async Task<ThumbnailFormat> ConvertToThumbnailFormat()
    {
        var path = await downloader.GetFileFromHash(ThumbnailHash);

        var preCreatedTexture = new Texture2D(2, 2); // pre create Texture, because Textures might not be creatable in non Main-Threads

        var stream = File.OpenRead(path);

        var bytes = new byte[stream.Length];

        var readByteCount = await stream.ReadAsync(bytes, 0, (int)stream.Length);
        Debug.Assert(stream.Length == readByteCount);

        var thumbnail = LoadBytesAsImage(bytes, preCreatedTexture);

        return new ThumbnailFormat(id, thumbnail);
    }

    private Texture2D LoadBytesAsImage(byte[] bytes, Texture2D inTexture = null)
    {
        inTexture ??= new Texture2D(2, 2);
        inTexture.LoadImage(bytes);

        return inTexture;
    }

    public async Task<SpriteFormat> ConvertToSpriteFormat()
    {
        ThumbnailFormat tf = await ConvertToThumbnailFormat();
        SpriteFormat sf = await tf.ConvertToSpriteFormat();
        return sf;
    }

    public class Factory : PlaceholderFactory<Guid, string, string, Dictionary<string, string>, string, BuilderAssetFormat>
    {
    }
}