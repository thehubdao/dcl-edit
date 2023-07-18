using System;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class ThumbnailFormat : AssetFormat, ISpriteConvertible
{
    public Texture2D thumbnail;

    public ThumbnailFormat(Guid id, Texture2D thumbnail) : base(id)
    {
        this.thumbnail = thumbnail;
    }

    public Task<SpriteFormat> ConvertToSpriteFormat() => Task.FromResult(new SpriteFormat(id, thumbnail));
}

public interface IThumbnailConvertible
{
    Task<ThumbnailFormat> ConvertToThumbnailFormat();
}
