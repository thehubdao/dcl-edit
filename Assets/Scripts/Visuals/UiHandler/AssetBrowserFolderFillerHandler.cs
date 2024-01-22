using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Assets;
using UnityEngine;
using Zenject;

public class AssetBrowserFolderFillerHandler : MonoBehaviour, IOnReturnToPool
{
    private List<CommonAssetTypes.GameObjectInstance> childGameObjectInstances = new();

    // Dependencies
    private SpecialAssets specialAssets;

    [Inject]
    private void Construct(SpecialAssets specialAssets)
    {
        this.specialAssets = specialAssets;
    }

    [SerializeField]
    private GameObject subFolderParent;

    [SerializeField]
    private GameObject subAssetParent;


    public void UpdateFolderContent(AssetBrowserSystem.AbStructFolder parentAbStructFolder)
    {
        ClearFolderContent();

        foreach (var abStructItem in parentAbStructFolder.GetItems())
        {
            switch (abStructItem)
            {
                case AssetBrowserSystem.AbStructAsset abStructAsset:
                    AddAssetContent(subAssetParent, abStructAsset);
                    break;
                case AssetBrowserSystem.AbStructFolder abStructFolder:
                    AddFolder(subFolderParent, abStructFolder);
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

        childGameObjectInstances.Clear();
    }

    private void AddFolder(GameObject parentObject, AssetBrowserSystem.AbStructFolder abStructFolder)
    {
        var folderUiElement = specialAssets.assetFolderUiElement.CreateInstance();
        folderUiElement.gameObject.transform.SetParent(parentObject.transform, false);
        folderUiElement.gameObject.GetComponent<AssetBrowserFolderHandler>().Init(abStructFolder);
        childGameObjectInstances.Add(folderUiElement);
    }

    private void AddAssetContent(GameObject parentObject, AssetBrowserSystem.AbStructAsset abStructAsset)
    {
        var buttonUiElement = specialAssets.assetButtonUiElement.CreateInstance();
        buttonUiElement.gameObject.transform.SetParent(parentObject.transform, false);
        buttonUiElement.gameObject.GetComponent<AssetBrowserButtonHandler>().InitUsageInUiAssetBrowser(abStructAsset);
        childGameObjectInstances.Add(buttonUiElement);
    }

    public void OnReturnToPool()
    {
        ClearFolderContent();
    }
}
