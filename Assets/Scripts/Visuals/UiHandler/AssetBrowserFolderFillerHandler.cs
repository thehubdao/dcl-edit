using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Assets;
using UnityEngine;
using Zenject;

public class AssetBrowserFolderFillerHandler : MonoBehaviour
{
    private List<CommonAssetTypes.GameObjectInstance> childGameObjectInstances = new();

    // Dependencies
    private SpecialAssets specialAssets;

    [Inject]
    private void Construct(SpecialAssets specialAssets)
    {
        this.specialAssets = specialAssets;
    }


    private void UpdateFolderContent(GameObject parentObject, AssetBrowserSystem.AbStructFolder parentAbStructFolder)
    {
        Debug.Log("Update Folder Content");


        foreach (var abStructItem in parentAbStructFolder.GetItems())
        {
            switch (abStructItem)
            {
                case AssetBrowserSystem.AbStructAsset abStructAsset:
                    AddAssetContent(parentObject, abStructAsset);
                    break;
                case AssetBrowserSystem.AbStructFolder abStructFolder:
                    AddFolder(parentObject, abStructFolder);
                    break;
            }
        }
    }

    public void ClearFolderContent()
    {
        foreach (var gameObjectInstance in childGameObjectInstances)
        {
            gameObjectInstance.ReturnToPool();
        }
    }

    private void AddFolder(GameObject parentObject, AssetBrowserSystem.AbStructFolder abStructFolder)
    {
        Debug.Log("Add folder");
        var folderUiElement = specialAssets.assetFolderUiElement.CreateInstance();
        folderUiElement.gameObject.transform.SetParent(parentObject.GetComponent<AssetBrowserFolderHandler>()?.subFolderContainer ?? parentObject.transform, false);
        folderUiElement.gameObject.GetComponent<AssetBrowserFolderHandler>().Init(abStructFolder);
        childGameObjectInstances.Add(folderUiElement);
    }

    private void AddAssetContent(GameObject parentObject, AssetBrowserSystem.AbStructAsset abStructAsset)
    {
        Debug.Log("Add content");
        var buttonUiElement = specialAssets.assetButtonUiElement.CreateInstance();
        buttonUiElement.gameObject.transform.SetParent(parentObject.GetComponent<AssetBrowserFolderHandler>().assetButtonContainer, false);
        buttonUiElement.gameObject.GetComponent<AssetBrowserButtonHandler>().InitUsageInUiAssetBrowser(abStructAsset);
        childGameObjectInstances.Add(buttonUiElement);
    }
}
