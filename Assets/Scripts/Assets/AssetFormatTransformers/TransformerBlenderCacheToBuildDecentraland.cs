using System;
using Assets.Scripts.Assets;
using Assets.Scripts.EditorState;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

public class TransformerBlenderCacheToBuildDecentraland : AssetFormatTransformer.IAssetTransformation
{
    public Type fromType => typeof(AssetFormatBlendCached);
    public Type toType => typeof(AssetFormatBuildForDecentraland);

// Dependencies
    private GlbInterpreterSystem glbInterpreterSystem;
    private PathState pathsState;

    [Inject]
    private void Construct(
        GlbInterpreterSystem glbInterpreterSystem,
        PathState pathsState)
    {
        this.glbInterpreterSystem = glbInterpreterSystem;
        this.pathsState = pathsState;
    }


    public List<(CommonAssetTypes.AssetInfo, Type)> AdditionalRequirements(CommonAssetTypes.AssetInfo asset)
    {
        return new List<(CommonAssetTypes.AssetInfo, Type)>();
    }

    public CommonAssetTypes.AssetFormat Transform(CommonAssetTypes.AssetFormat fromFormat, CommonAssetTypes.AssetInfo asset)
    {
        var fromBlendCachedFormat = (AssetFormatBlendCached) fromFormat;

        var nextPath = AssetFormatBuildForDecentraland.GetNextName(".glb");

        File.Copy(fromBlendCachedFormat.glbPath, Path.Combine(pathsState.BuildPath, nextPath), true);

        var assetInBuildFormat = new AssetFormatBuildForDecentraland(fromBlendCachedFormat.hash, nextPath);
        return assetInBuildFormat;
    }
}
