using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetAssetsList : MonoBehaviour, ISerializedFieldToStatic
{
    [SerializeField]
    private Texture2D _defaultThumbnail;
    public static Texture2D DefaultThumbnail { get; private set; }

    [SerializeField]
    private GameObject _errorModel;
    public static GameObject ErrorModel { get; private set; }

    [SerializeField]
    private GameObject _assetUiTemplate;
    public static GameObject AssetUiTemplate { get; private set; }


    public void SetupStatics()
    {
        DefaultThumbnail = _defaultThumbnail;
        ErrorModel = _errorModel;
        AssetUiTemplate = _assetUiTemplate;
    }
}
