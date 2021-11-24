using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class AssetItemUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _nameText;

    [SerializeField]
    private TextMeshProUGUI _typeText;

    [SerializeField]
    private RawImage _thumbnail;


    [NonSerialized]
    public AssetManager.Asset asset;

    void Start()
    {
        AssetManager.OnAssetChange.AddListener(UpdateVisuals);
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (asset == null)
            return;

        _nameText.text = asset.name;
        _typeText.text = asset.TypeName;

        _thumbnail.texture = AssetAssetsList.DefaultThumbnail;

        if (asset.GetType() == typeof(AssetManager.GLTFAsset))
        {
            var gltfAsset = asset as AssetManager.GLTFAsset;
            var pathParts = gltfAsset.gltfPath.Split('/');

            var gltfFolderPath = SceneManager.DclProjectPath + "/" + Path.GetDirectoryName(gltfAsset.gltfPath);
            var thumbnailPath = gltfFolderPath + "/thumbnail.png";

            //var thumbnailPath = string.Concat(
            //    pathParts
            //        .Take(pathParts.Length - 1)
            //        .SelectMany(s => s)
            //) + "/thumbnail.png";

            Debug.Log(thumbnailPath);


            if (thumbnailPath != "" && File.Exists(thumbnailPath))
            {
                StartCoroutine(GetGltfThumbnail(thumbnailPath));
            }
        }
    }

    private IEnumerator GetGltfThumbnail(string thumbnailPath)
    {
        var request = UnityWebRequestTexture.GetTexture("file://" + thumbnailPath);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            _thumbnail.texture = DownloadHandlerTexture.GetContent(request);
        }
    }

}
