using System;
using Assets.Scripts.Assets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class TransformerModelLoadedToThumbnail : AssetFormatTransformer.IAssetTransformation
{
    // Dependencies
    private AssetThumbnailGeneratorSystem assetThumbnailGeneratorSystem;

    [Inject]
    private void Construct(AssetThumbnailGeneratorSystem assetThumbnailGeneratorSystem)
    {
        this.assetThumbnailGeneratorSystem = assetThumbnailGeneratorSystem;
    }


    public Type fromType => typeof(AssetFormatLoadedModel);
    public Type toType => typeof(AssetFormatThumbnail);

    public List<(CommonAssetTypes.AssetInfo, Type)> AdditionalRequirements(CommonAssetTypes.AssetInfo asset)
    {
        return new List<(CommonAssetTypes.AssetInfo, Type)>();
    }

    public CommonAssetTypes.AssetFormat Transform(CommonAssetTypes.AssetFormat fromFormat, CommonAssetTypes.AssetInfo asset)
    {
        var fromLoadedModelFormat = (AssetFormatLoadedModel) fromFormat;
        var toFormat = new AssetFormatThumbnail(fromLoadedModelFormat.hash);

        assetThumbnailGeneratorSystem.GenerateFromModel(fromLoadedModelFormat, sprite =>
        {
            if (sprite == null)
            {
                toFormat.SetError();
                asset.InvokeAssetFormatChanged();
            }
            else
            {
                toFormat.SetThumbnail(sprite);
                asset.InvokeAssetFormatChanged();
            }
        });

        return toFormat;
    }
}
