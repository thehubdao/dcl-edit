using Assets.Scripts.Assets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TransformerThumbnailCacheToThumbnail : AssetFormatTransformer.IAssetTransformation
{
    public Type fromType => typeof(AssetFormatThumbnailCached);
    public Type toType => typeof(AssetFormatThumbnail);

    public List<(CommonAssetTypes.AssetInfo, Type)> AdditionalRequirements(CommonAssetTypes.AssetInfo asset)
    {
        return new List<(CommonAssetTypes.AssetInfo, Type)>();
    }

    public CommonAssetTypes.AssetFormat Transform(CommonAssetTypes.AssetFormat fromFormat, CommonAssetTypes.AssetInfo asset)
    {
        var fromThumbnailCachedFormat = (AssetFormatThumbnailCached) fromFormat;

        var path = fromThumbnailCachedFormat.imagePath;
        var data = File.ReadAllBytes(path);

        var texture = new Texture2D(1, 1);
        texture.LoadImage(data);

        var sprite = Sprite.Create(
            texture,
            Rect.MinMaxRect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f));

        var toFormat = new AssetFormatThumbnail(fromThumbnailCachedFormat.hash);
        toFormat.SetThumbnail(sprite);
        return toFormat;
    }
}
