using System;
using Assets.Scripts.Assets;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.System;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;
using CommonAssetTypes = Assets.Scripts.Assets.CommonAssetTypes;
using System.Linq;

public class TransformerOnDiscToLoadedModel : AssetFormatTransformer.IAssetTransformation
{
    // Dependencies
    private LoadGltfFromFileSystem loadGltfFromFileSystem;

    [Inject]
    private void Construct(LoadGltfFromFileSystem loadGltfFromFileSystem)
    {
        this.loadGltfFromFileSystem = loadGltfFromFileSystem;
    }


    public Type fromType => typeof(AssetFormatOnDisc);
    public Type toType => typeof(AssetFormatLoadedModel);

    public List<(CommonAssetTypes.AssetInfo, Type)> AdditionalRequirements(CommonAssetTypes.AssetInfo asset)
    {
        return asset.dependencies.Select(dependency => (dependency, typeof(AssetFormatOnDisc))).ToList();
    }

    public CommonAssetTypes.AssetFormat Transform(CommonAssetTypes.AssetFormat fromFormat, CommonAssetTypes.AssetInfo asset)
    {
        Assert.IsTrue(asset.assetType == CommonAssetTypes.AssetType.Model3D);
        Assert.IsTrue(fromFormat is AssetFormatOnDisc);

        var fromOnDiscFormat = (AssetFormatOnDisc) fromFormat;
        var newLoadedModelFormat = new AssetFormatLoadedModel(fromOnDiscFormat.hash);

        loadGltfFromFileSystem.LoadGltfFromPath(fromOnDiscFormat.filePath, go =>
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
