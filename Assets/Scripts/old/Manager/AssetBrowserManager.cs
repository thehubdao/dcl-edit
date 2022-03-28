using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AssetBrowserManager : Manager
{
    public static UnityEvent OnOpenAssetBrowser = new UnityEvent();
    public static UnityEvent OnCloseAssetBrowser = new UnityEvent();

    public static Action<AssetManager.Asset> OnSelected { get; private set; } = default;
    public static Action OnSelectionFailed { get; private set; } = default;

    public static void OpenAssetBrowser(Action<AssetManager.Asset> onSelected = null,Action onSelectionFailed = null)
    {
        OnSelected = onSelected;
        OnSelectionFailed = onSelectionFailed;

        OnOpenAssetBrowser.Invoke();
        AssetManager.OnAssetChange.Invoke();
    }

    public static void CloseAssetBrowser()
    {
        OnCloseAssetBrowser.Invoke();
    }
}
