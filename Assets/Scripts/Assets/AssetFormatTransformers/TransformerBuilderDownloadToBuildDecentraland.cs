using System;
using Assets.Scripts.Assets;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using Assets.Scripts.EditorState;
using UnityEngine.Assertions;
using Zenject;

public class TransformerBuilderDownloadToBuildDecentraland : AssetFormatTransformer.IAssetTransformation
{
    public Type fromType => typeof(AssetFormatBuilderDownload);
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
        return asset.dependencies.Select(dependency => (dependency, typeof(AssetFormatBuildForDecentraland))).ToList();
    }

    public CommonAssetTypes.AssetFormat Transform(CommonAssetTypes.AssetFormat fromFormat, CommonAssetTypes.AssetInfo asset)
    {
        var assetInBuilderDownloadFormat = (AssetFormatBuilderDownload) fromFormat;

        var nextPath = AssetFormatBuildForDecentraland.GetNextName(Path.GetExtension(assetInBuilderDownloadFormat.basePath));

        if (asset.assetType == CommonAssetTypes.AssetType.Model3D)
        {
            // change infile dependencies
            // buildup translation table
            var translationTable = new Dictionary<string, string>();
            foreach (var assetDependency in asset.dependencies)
            {
                // both formats should exist for all dependencies
                var builderDownloadFormat = assetDependency.availableFormats.First(f => f is AssetFormatBuilderDownload) as AssetFormatBuilderDownload;
                var buildFormat = assetDependency.availableFormats.First(f => f is AssetFormatBuildForDecentraland) as AssetFormatBuildForDecentraland;

                Assert.IsNotNull(builderDownloadFormat);
                Assert.IsNotNull(buildFormat);

                translationTable.Add(Path.GetFileName(builderDownloadFormat!.basePath!), Path.GetFileName(buildFormat!.buildPath));
            }

            // open up the glb
            var glb = glbInterpreterSystem.ReadGlb(assetInBuilderDownloadFormat.basePath);
            var json = glb.GetGltfJson();
            var uris = json.FindUriValues();

            // translate dependencies
            foreach (var uri in uris)
            {
                uri.Replace(translationTable[uri.ToString()]);
            }

            // pack gbl
            glb.SetGltfJson(json);

            File.WriteAllBytes(Path.Combine(pathsState.buildPath, nextPath), glb.GetAllBytes());
        }
        else
        {
            File.Copy(assetInBuilderDownloadFormat.basePath!, Path.Combine(pathsState.buildPath, nextPath), true);
        }

        var assetInBuildFormat = new AssetFormatBuildForDecentraland(assetInBuilderDownloadFormat.hash, nextPath);
        return assetInBuildFormat;
    }
}
