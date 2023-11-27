using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Assets.Scripts.Assets;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using Zenject;

public class TransformerBuilderCloudToBuilderDownload : AssetFormatTransformer.IAssetTransformation
{
    // https://builder-api.decentraland.org/v1/storage/contents/<hash>

    // Dependencies
    private WebRequestSystem webRequestSystem;

    [Inject]
    private void Construct(WebRequestSystem webRequestSystem)
    {
        this.webRequestSystem = webRequestSystem;
    }

    public Type fromType => typeof(AssetFormatBuilderCloud);
    public Type toType => typeof(AssetFormatBuilderDownload);

    public CommonAssetTypes.AssetFormat Transform(CommonAssetTypes.AssetFormat fromFormat, CommonAssetTypes.AssetInfo asset)
    {
        var assetFormatBuilderCloud = fromFormat as AssetFormatBuilderCloud ?? throw new ArgumentException("TransformerBuilderCloudToBuilderDownload needs an AssetFormatBuilderCloud as argument");

        // find the correct hash
        var modelHash = assetFormatBuilderCloud.contents[assetFormatBuilderCloud.modelCloudPath];
        var uri = $"https://builder-api.decentraland.org/v1/storage/contents/{modelHash}";

        // make new Format
        var newFormat = new AssetFormatBuilderDownload();

        // download model
        webRequestSystem.Get(uri, requestOperation =>
        {
            var data = requestOperation.webRequest.downloadHandler.data;

            // create save path
            var savePath = Path.Combine(Application.persistentDataPath, "BuilderAssetCache", asset.assetId.ToString(), assetFormatBuilderCloud.modelCloudPath);

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
