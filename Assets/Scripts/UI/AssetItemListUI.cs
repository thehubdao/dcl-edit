using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetItemListUI : MonoBehaviour
{
    void Start()
    {
        AssetManager.OnAssetChange.AddListener(UpdateVisuals);
        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var asset in AssetManager.allAssets)
        {
            var newAssetItemObject = Instantiate(AssetAssetsList.AssetUiTemplate, transform);
            var newAssetItemUI = newAssetItemObject.GetComponent<AssetItemUI>();
            newAssetItemUI.asset = asset;
            newAssetItemUI.UpdateVisuals();
        }
    }
}
