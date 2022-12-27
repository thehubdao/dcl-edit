using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AssetBrowserFolderHandler : MonoBehaviour
{
    public Button header;
    public TextMeshProUGUI headerText;
    public RectTransform expandIcon;
    private AssetHierarchyItem hierarchyItem;
    private ScrollRect scrollViewRect;


    private bool expanded = false;


    // Dependencies
    private UiBuilder uiBuilder;
    AssetManagerSystem assetManagerSystem;
    UnityState unityState;


    [Inject]
    void Construct(UiBuilder.Factory uiBuilderFactory, AssetManagerSystem assetManagerSystem, UnityState unityState)
    {
        uiBuilder = uiBuilderFactory.Create(gameObject);
        this.assetManagerSystem = assetManagerSystem;
        this.unityState = unityState;
    }


    public void Init(AssetHierarchyItem hierarchyItem, ScrollRect scrollViewRect)
    {
        this.hierarchyItem = hierarchyItem;
        headerText.text = hierarchyItem.name;
        this.scrollViewRect = scrollViewRect;
    }


    public void SetExpanded(bool expanded)
    {
        this.expanded = expanded;

        // Flip icon vertically
        var scale = expandIcon.localScale;
        scale.y = expanded ? -1 : 1;
        expandIcon.localScale = scale;

        if (expanded)
        {
            BuildFolderHierarchy();
        }
        else
        {
            ClearFolderHierarchy();
        }
    }


    public void ToggleExpanded()
    {
        expanded = !expanded;
        SetExpanded(expanded);
    }


    private void BuildFolderHierarchy()
    {
        var panel = new PanelAtom.Data();

        foreach (var subfolder in hierarchyItem.childDirectories)
        {
            panel.AddAssetBrowserFolder(subfolder, scrollViewRect);
        }

        if (hierarchyItem.assetIds.Count > 0)
        {
            var grid = panel.AddGrid();
            foreach (var childItem in hierarchyItem.assetIds)
            {
                var assetMetadata = assetManagerSystem.GetMetadataById(childItem);

                Texture2D typeIndicator = null;
                switch (assetMetadata.assetType)
                {
                    case AssetMetadata.AssetType.Unknown:
                        break;
                    case AssetMetadata.AssetType.Model:
                        typeIndicator = unityState.AssetTypeModelIcon;
                        break;
                    case AssetMetadata.AssetType.Image:
                        typeIndicator = unityState.AssetTypeImageIcon;
                        break;
                    default:
                        break;
                }

                grid.AddAssetBrowserButton(assetMetadata, typeIndicator, scrollViewRect);
            }
        }

        uiBuilder.Update(panel);
    }


    private void ClearFolderHierarchy()
    {
        foreach (var handler in GetComponentsInChildren<AssetBrowserFolderHandler>())
        {
            if (handler != this)
            {
                handler.Remove();
            }
        }

        uiBuilder.Update(new PanelAtom.Data());
    }

    public void Remove()
    {
        /*
        var panel = transform.Find("Panel(Clone)");
        if (panel)
        {
            Destroy(panel.gameObject);
        }*/
        uiBuilder.Update(new PanelAtom.Data());

    }


    public class Factory : PlaceholderFactory<AssetBrowserFolderHandler>
    {
    }
}
