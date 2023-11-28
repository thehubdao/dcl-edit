using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Assets;
using JetBrains.Annotations;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

public class TransformerBuilderCloudToBuilderDownload : AssetFormatTransformer.IAssetTransformation
{
    // https://builder-api.decentraland.org/v1/storage/contents/<hash>

    // Dependencies
    private WebRequestSystem webRequestSystem;
    private GlbInterpreterSystem glbInterpreterSystem;
    private DiscoveredAssets discoveredAssets;

    [Inject]
    private void Construct(
        WebRequestSystem webRequestSystem,
        GlbInterpreterSystem glbInterpreterSystem,
        DiscoveredAssets discoveredAssets)
    {
        this.webRequestSystem = webRequestSystem;
        this.glbInterpreterSystem = glbInterpreterSystem;
        this.discoveredAssets = discoveredAssets;
    }

    public Type fromType => typeof(AssetFormatBuilderCloud);
    public Type toType => typeof(AssetFormatBuilderDownload);

    public List<(CommonAssetTypes.AssetInfo, Type)> AdditionalRequirements(CommonAssetTypes.AssetInfo asset)
    {
        return new List<(CommonAssetTypes.AssetInfo, Type)>();
    }

    public CommonAssetTypes.AssetFormat Transform(CommonAssetTypes.AssetFormat fromFormat, CommonAssetTypes.AssetInfo asset)
    {
        var assetFormatBuilderCloud = fromFormat as AssetFormatBuilderCloud ?? throw new ArgumentException("TransformerBuilderCloudToBuilderDownload needs an AssetFormatBuilderCloud as argument");

        // find the correct hash
        var uri = $"https://builder-api.decentraland.org/v1/storage/contents/{assetFormatBuilderCloud.cloudHash}";

        // make new Format
        var newFormat = new AssetFormatBuilderDownload();

        // download model
        webRequestSystem.Get(uri, requestOperation =>
        {
            var data = requestOperation.webRequest.downloadHandler.data;

            // check if its glb
            if (asset.assetType == CommonAssetTypes.AssetType.Model3D)
            {
                // detect dependencies
                var glb = glbInterpreterSystem.ReadGlb(data);

                var gltf = glb.GetGltfJson();
                var uris = gltf.FindUriValues();
                foreach (var dependentUriToken in uris)
                {
                    // get string
                    var dependentPath = (Path.GetDirectoryName(assetFormatBuilderCloud.cloudPath) ?? "") + "/" + dependentUriToken.ToString();

                    // find asset hash
                    var hash = assetFormatBuilderCloud.contents[dependentPath];

                    // generate id
                    var id = StaticUtils.GuidFromString(hash);

                    // check if id is there
                    if (!discoveredAssets.discoveredAssets.TryGetValue(id, out var dependentAsset))
                    {
                        // if not, create the asset
                        var baseFormat = new AssetFormatBuilderCloud(hash, dependentPath);
                        dependentAsset = new CommonAssetTypes.AssetInfo
                        {
                            assetId = id,
                            assetName = asset.assetName + " Dependency",
                            visible = false,
                            assetSource = CommonAssetTypes.AssetSource.DecentralandBuilder,
                            assetType = CommonAssetTypes.AssetType.Unknown,
                            availableFormats = new List<CommonAssetTypes.AssetFormat> {baseFormat},
                            baseFormat = baseFormat,
                            dependencies = new List<CommonAssetTypes.AssetInfo>(),
                            displayPath = ""
                        };
                        discoveredAssets.discoveredAssets.Add(id, dependentAsset);
                    }

                    // set the new name for dependencies in gltf
                    var dependentExtension = Path.GetExtension(dependentPath);
                    var dependentRelativeSavePath = id + dependentExtension;

                    dependentUriToken.Replace(dependentRelativeSavePath);

                    // add dependency
                    asset.dependencies.Add(dependentAsset);
                }


                // write changed gltf back
                glb.SetGltfJson(gltf);

                data = glb.GetAllBytes();
            }

            // create save path
            var extension = Path.GetExtension(assetFormatBuilderCloud.cloudPath);
            var savePath = Path.Combine(Application.persistentDataPath, "BuilderAssetCache", asset.assetId.ToString() + extension);

            // create directory
            var directory = Path.GetDirectoryName(savePath);
            Assert.IsNotNull(directory);
            Directory.CreateDirectory(directory!);

            // save file
            File.WriteAllBytes(savePath, data);

            newFormat.SetBasePath(savePath);

            asset.InvokeAssetFormatChanged();
        });

        return newFormat;
    }
}
