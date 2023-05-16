using Assets.Scripts.System;
using UnityEngine;
using Visuals.UiHandler;
using Zenject;

public class DropAssetOnSceneView : MonoBehaviour
{
    [SerializeField]
    private DropHandler dropHandler;


    // Dependencies
    private AssetBrowserSystem assetBrowserSystem;
    private AssetManagerSystem assetManagerSystem;

    [Inject]
    private void Construct(AssetBrowserSystem assetBrowserSystem, AssetManagerSystem assetManagerSystem)
    {
        this.assetBrowserSystem = assetBrowserSystem;
        this.assetManagerSystem = assetManagerSystem;
    }


    void Awake()
    {
        dropHandler.dropStrategy = new DropStrategy
        {
            dropModelAssetStrategy =
                new DropModelAssetStrategy(assetId =>
                {
                    Debug.Log("Dropped Model on scene"); // Not executing

                    var assetMetadata = assetManagerSystem.GetMetadataById(assetId);
                    assetBrowserSystem.AddAssetToSceneAtMousePositionInViewport(assetMetadata);
                })
        };
    }
}
