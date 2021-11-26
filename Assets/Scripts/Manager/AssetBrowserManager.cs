using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AssetBrowserManager : Manager
{
    public static UnityEvent OnOpenAssetBrowser = new UnityEvent();

    public static Action<AssetManager.Asset> OnSelected { get; private set; }
    public static Action<AssetManager.Asset> OnSelectionFailed { get; private set; }

    public static void OpenAssetBrowser(Action<AssetManager.Asset> onSelected = null,Action<AssetManager.Asset> onSelectionFailed = null)
    {
        OnSelected = onSelected;
        OnSelectionFailed = onSelectionFailed;

        OnOpenAssetBrowser.Invoke();
    }
}
