using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Assets;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

public class TransformerBuilderCloudToThumbnailCached : AssetFormatTransformer.IAssetTransformation
{
    // Dependencies
    private WebRequestSystem webRequestSystem;

    [Inject]
    private void Construct(WebRequestSystem webRequestSystem)
    {
        this.webRequestSystem = webRequestSystem;
    }


    public Type fromType => typeof(AssetFormatBuilderCloud);
    public Type toType => typeof(AssetFormatThumbnailCached);

    public List<(CommonAssetTypes.AssetInfo, Type)> AdditionalRequirements(CommonAssetTypes.AssetInfo asset)
    {
        return new List<(CommonAssetTypes.AssetInfo, Type)>();
    }

    public CommonAssetTypes.AssetFormat Transform(CommonAssetTypes.AssetFormat fromFormat, CommonAssetTypes.AssetInfo asset)
    {
        var fromBuilderCloudFormat = (AssetFormatBuilderCloud) fromFormat;
        var toFormat = new AssetFormatThumbnailCached(fromBuilderCloudFormat.hash);

        // make uri out of thumbnail cache
        var uri = $"https://builder-api.decentraland.org/v1/storage/contents/{fromBuilderCloudFormat.thumbnailCloudHash}";

        webRequestSystem.Get(uri, operation =>
        {
            if (operation.webRequest.result != UnityWebRequest.Result.Success)
            {
                toFormat.SetError();
                Debug.Log("Error while downloading Thumbnail: " + operation.webRequest.error);
                asset.InvokeAssetFormatChanged();
                return;
            }

            operation.webRequest.GetResponseHeader("");

            var data = operation.webRequest.downloadHandler.data;
            var filePath = Path.Combine(Application.persistentDataPath, "thumbnail-cache", asset.assetId + ".any");
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            File.WriteAllBytes(filePath, data);

            toFormat.SetImage(filePath);
            asset.InvokeAssetFormatChanged();
        });

        return toFormat;
    }
}
