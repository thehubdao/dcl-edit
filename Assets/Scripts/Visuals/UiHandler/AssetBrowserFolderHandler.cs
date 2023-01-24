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
    AssetBrowserState assetBrowserState;


    [Inject]
    void Construct(UiBuilder.Factory uiBuilderFactory, UnityState unityState, AssetBrowserState assetBrowserState)
    {
        uiBuilder = uiBuilderFactory.Create(gameObject);
        this.unityState = unityState;
        this.assetBrowserState = assetBrowserState;
    }


    public void Initialize(AssetHierarchyItem hierarchyItem, ScrollRect scrollViewRect)
    {
        this.hierarchyItem = hierarchyItem;
        headerText.text = hierarchyItem.name;
        this.scrollViewRect = scrollViewRect;

        // Check if folder was expanded before the UI rebuild
        if (assetBrowserState.expandedFoldersPaths.Contains(hierarchyItem.path))
        {
            SetExpanded(true);
        }
        else
        {
            SetExpanded(false);
        }
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
            assetBrowserState.expandedFoldersPaths.Add(hierarchyItem.path);
            BuildFolderHierarchy();
        }
        else
        {
            // Mark all folders as closed that are at the current position or further down in the asset hierarchy
            assetBrowserState.expandedFoldersPaths.RemoveAll((path) => path.Contains(hierarchyItem.path));
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
                grid.AddAssetBrowserButton(asset, scrollViewRect);
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
