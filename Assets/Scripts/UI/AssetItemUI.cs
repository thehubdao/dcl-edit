using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
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

    [SerializeField]
    private Button _button;

    [SerializeField]
    public bool isInInspector = false;

    


    [NonSerialized]
    public AssetManager.Asset asset;

    void Start()
    {
        AssetManager.OnAssetChange.AddListener(UpdateVisuals);
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (!gameObject.activeSelf)
            return;

        if (asset == null)
        {
            _nameText.text = "No Asset";
            _typeText.text = "";
        }
        else
        {
            _nameText.text = asset.name;
            _typeText.text = asset.TypeName;
        }


        _button.onClick = new Button.ButtonClickedEvent();
        _button.interactable = true;
        if (isInInspector)
        {
            _button.onClick.AddListener(() =>
            {
                
                AssetBrowserManager.OpenAssetBrowser((asset) =>
                {
                    Debug.Log("New asset selected: "+asset);
                    
                    var previousAsset = SceneManager.PrimarySelectedEntity.GetComponent<GLTFShapeComponent>().asset;
                    var nextAsset = (AssetManager.GLTFAsset)asset;

                    SceneManager.PrimarySelectedEntity.GetComponent<GLTFShapeComponent>().asset =
                        nextAsset;
                    SceneManager.OnUpdateSelection.Invoke();

                    UndoManager.RecordUndoItem($"Select asset: {asset.name}",
                        () =>
                        {
                            SceneManager.PrimarySelectedEntity.GetComponent<GLTFShapeComponent>().asset =
                                previousAsset;
                            SceneManager.OnUpdateSelection.Invoke();
                        },
                        () =>
                        {
                            SceneManager.PrimarySelectedEntity.GetComponent<GLTFShapeComponent>().asset =
                                nextAsset;
                            SceneManager.OnUpdateSelection.Invoke();
                        });

                });
            });
        }
        else
        {
            if (AssetBrowserManager.OnSelected != null)
            {
                _button.onClick.AddListener(() =>
                    {
                        AssetBrowserManager.OnSelected.Invoke(asset);
                        AssetBrowserManager.CloseAssetBrowser();
                    });
            }
            else
            {
                _button.interactable = false;
            }
        }

        _thumbnail.texture = AssetAssetsList.DefaultThumbnail;

        if (asset != null && asset.GetType() == typeof(AssetManager.GLTFAsset))
        {
            var gltfAsset = (AssetManager.GLTFAsset)asset;
            var pathParts = gltfAsset.gltfPath.Split('/');

            var gltfFolderPath = SceneManager.DclProjectPath + "/" + Path.GetDirectoryName(gltfAsset.gltfPath);
            var thumbnailPath = gltfFolderPath + "/thumbnail.png";

            //var thumbnailPath = string.Concat(
            //    pathParts
            //        .Take(pathParts.Length - 1)
            //        .SelectMany(s => s)
            //) + "/thumbnail.png";

            //Debug.Log(thumbnailPath);

            ThumbnailManager.GetThumbnail(thumbnailPath,thumbnail =>
            {
                if(_thumbnail == null)
                    return;
                _thumbnail.texture = thumbnail;
            });

            //if (thumbnailPath != "" && File.Exists(thumbnailPath))
            //{
            //    StartCoroutine(GetGltfThumbnail(thumbnailPath));
            //}
        }
    }

    //private IEnumerator GetGltfThumbnail(string thumbnailPath)
    //{
    //    var request = UnityWebRequestTexture.GetTexture("file://" + thumbnailPath);
    //    yield return request.SendWebRequest();
    //
    //    if (request.result != UnityWebRequest.Result.Success)
    //    {
    //        Debug.Log(request.error);
    //    }
    //    else
    //    {
    //        _thumbnail.texture = DownloadHandlerTexture.GetContent(request);
    //    }
    //}

}
