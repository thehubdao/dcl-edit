using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Assets;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

public class TransformerBuilderDownloadToLoadedModel : AssetFormatTransformer.AssetTransformation
{
    // Dependencies
    private LoadGltfFromFileSystem loadGltfFromFileSystem;

    [Inject]
    private void Construct(LoadGltfFromFileSystem loadGltfFromFileSystem)
    {
        this.loadGltfFromFileSystem = loadGltfFromFileSystem;
    }


    public override CommonAssetTypes.AssetFormat Transform(CommonAssetTypes.AssetFormat fromFormat, CommonAssetTypes.AssetInfo asset)
    {
        var builderDownload = (AssetFormatBuilderDownload)fromFormat;

        var newLoadedModelFormat = new AssetFormatLoadedModel("");

        loadGltfFromFileSystem.LoadGltfFromPath(builderDownload.basePath, go =>
        {
            if (go == null)
            {
                newLoadedModelFormat.SetError();
                asset.InvokeAssetFormatChanged();
            }
            else
            {
                newLoadedModelFormat.SetModel(go);
                asset.InvokeAssetFormatChanged();
            }

        });

        return newLoadedModelFormat;
    }
}
