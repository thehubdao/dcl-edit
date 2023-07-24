using System;
using System.Threading.Tasks;
using UnityEngine;

public class SpriteFormat : AssetFormat
{
    public Sprite sprite;

    public SpriteFormat(Guid id, Texture2D texture) : base(id)
    {
        sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f), 100);
    }

    public SpriteFormat(Guid id, Sprite sprite) : base(id)
    {
        this.sprite = sprite;
    }
}

public interface ISpriteConvertible
{
    Task<SpriteFormat> ConvertToSpriteFormat();
}
