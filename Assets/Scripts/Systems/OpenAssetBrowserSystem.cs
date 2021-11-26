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
    }

    public void Start()
    {
        AssetBrowserManager.OnOpenAssetBrowser.AddListener(OpenAssetBrowser);
    }

    public static void OpenAssetBrowser()
    {
        _assetBrowserWindowInstance.SetActive(true);
    }
}
