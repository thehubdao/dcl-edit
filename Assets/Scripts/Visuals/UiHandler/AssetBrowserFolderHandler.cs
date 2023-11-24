using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AssetBrowserFolderHandler : MonoBehaviour
{
    public Button header;
    public TextMeshProUGUI headerText;

    public RectTransform expandIcon;
    //private AssetHierarchyItem hierarchyItem;

    // Dependencies
    AssetBrowserState assetBrowserState;
    EditorEvents editorEvents;

    [Inject]
    void Construct(AssetBrowserState assetBrowserState, EditorEvents editorEvents)
    {
        this.assetBrowserState = assetBrowserState;
        this.editorEvents = editorEvents;
    }

    public void Initialize( /*AssetHierarchyItem hierarchyItem*/)
    {
        /*this.hierarchyItem = hierarchyItem;
        headerText.text = hierarchyItem.name;*/

        var scale = expandIcon.localScale;
        scale.y = IsExpanded() ? -1 : 1; // Flip icon vertically if expanded
        expandIcon.localScale = scale;
    }

    private void SetExpanded(bool expanded)
    {
        /*if (expanded)
        {
            assetBrowserState.expandedFoldersPaths.Add(hierarchyItem.path);
        }
        else
        {
            // Mark all folders as closed that are at the current position or further down in the asset hierarchy
            assetBrowserState.expandedFoldersPaths.RemoveAll((path) => path.Contains(hierarchyItem.path));
        }
        editorEvents.InvokeUiChangedEvent();*/
    }

    public void ToggleExpanded() => SetExpanded(!IsExpanded());

    private bool IsExpanded() => false; //assetBrowserState.expandedFoldersPaths.Contains(hierarchyItem.path);

    public class Factory : PlaceholderFactory<AssetBrowserFolderHandler>
    {
    }
}
