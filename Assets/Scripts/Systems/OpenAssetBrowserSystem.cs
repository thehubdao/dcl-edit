using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenAssetBrowserSystem : MonoBehaviour,ISerializedFieldToStatic
{
    [SerializeField]
    private GameObject _assetBrowserWindow;
    private static GameObject _assetBrowserWindowInstance;


    public void SetupStatics()
    {
        _assetBrowserWindowInstance = _assetBrowserWindow;
        AssetBrowserManager.OnOpenAssetBrowser.AddListener(OpenAssetBrowser);
        AssetBrowserManager.OnCloseAssetBrowser.AddListener(CloseAssetBrowser);
    }

    public void Start()
    {
    }

    public static void OpenAssetBrowser()
    {
        _assetBrowserWindowInstance.SetActive(true);
    }

    public static void CloseAssetBrowser()
    {
        _assetBrowserWindowInstance.SetActive(false);
    }
}
