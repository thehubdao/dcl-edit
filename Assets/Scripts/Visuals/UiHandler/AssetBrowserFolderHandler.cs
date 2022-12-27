using Assets.Scripts.EditorState;
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
    UnityState unityState;


    [Inject]
    void Construct(UiBuilder.Factory uiBuilderFactory, UnityState unityState)
    {
        uiBuilder = uiBuilderFactory.Create(gameObject);
        this.unityState = unityState;
    }


    public void Initialize(AssetHierarchyItem hierarchyItem, ScrollRect scrollViewRect)
    {
        this.hierarchyItem = hierarchyItem;
        headerText.text = hierarchyItem.name;
        this.scrollViewRect = scrollViewRect;

        SetExpanded(false);
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
        PanelAtom.Data panel = new PanelAtom.Data();

        foreach (AssetHierarchyItem subfolder in hierarchyItem.childDirectories)
        {
            if (!subfolder.IsEmpty())
            {
                panel.AddAssetBrowserFolder(subfolder, scrollViewRect);
            }
        }

        if (hierarchyItem.assets.Count > 0)
        {
            GridAtom.Data grid = panel.AddGrid();
            foreach (AssetMetadata asset in hierarchyItem.assets)
            {
                Texture2D typeIndicator = null;
                switch (asset.assetType)
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

                grid.AddAssetBrowserButton(asset, typeIndicator, scrollViewRect);
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
                handler.RemoveSingleFolder();
            }
        }

        uiBuilder.Update(new PanelAtom.Data());
    }


    public void RemoveSingleFolder()
    {
        uiBuilder.Update(new PanelAtom.Data());
    }


    public class Factory : PlaceholderFactory<AssetBrowserFolderHandler>
    {
    }
}
