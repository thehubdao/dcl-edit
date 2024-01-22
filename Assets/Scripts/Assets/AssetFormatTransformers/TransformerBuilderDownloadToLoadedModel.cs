using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Assets;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

public class TransformerBuilderDownloadToLoadedModel : AssetFormatTransformer.IAssetTransformation
{
    // Dependencies
    private LoadGltfFromFileSystem loadGltfFromFileSystem;

    [Inject]
    private void Construct(LoadGltfFromFileSystem loadGltfFromFileSystem)
    {
        this.loadGltfFromFileSystem = loadGltfFromFileSystem;
    }


    public Type fromType => typeof(AssetFormatBuilderDownload);
    public Type toType => typeof(AssetFormatLoadedModel);

    public List<(CommonAssetTypes.AssetInfo, Type)> AdditionalRequirements(CommonAssetTypes.AssetInfo asset)
    {
        return asset.dependencies.Select(dependency => (dependency, typeof(AssetFormatBuilderDownload))).ToList();
    }

    public CommonAssetTypes.AssetFormat Transform(CommonAssetTypes.AssetFormat fromFormat, CommonAssetTypes.AssetInfo asset)
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
