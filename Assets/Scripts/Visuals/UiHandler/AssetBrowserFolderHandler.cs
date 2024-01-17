using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using Zenject;
using static AssetBrowserSystem;
using Debug = UnityEngine.Debug;

public class AssetBrowserFolderHandler : MonoBehaviour, IOnReturnToPool
{
    public Button header;
    public TextMeshProUGUI headerText;

    public RectTransform expandIcon;

    public RectTransform subFolderContainer;

    public RectTransform assetButtonContainer;
    //private AssetHierarchyItem hierarchyItem;

    public AssetBrowserFolderFillerHandler folderFiller;

    private AssetBrowserSystem.AbStructFolder currentFolder;

    // Dependencies
    AssetBrowserState assetBrowserState;
    EditorEvents editorEvents;

    [Inject]
    void Construct(AssetBrowserState assetBrowserState, EditorEvents editorEvents)
    {
        this.assetBrowserState = assetBrowserState;
        this.editorEvents = editorEvents;
    }


    //private List<string> logs = new();

    public void Init(AssetBrowserSystem.AbStructFolder abStructFolder)
    {
        //logs.Add($"Init: {abStructFolder?.name ?? "null"}, Was before: {currentFolder?.name ?? "null"} (should be null) \n StackTrace: {Environment.StackTrace}");

        Assert.IsNull(currentFolder, "Initialized multiple times without returning to pool");

        currentFolder = abStructFolder;

        currentFolder.Change += UpdateContent;

        UpdateContent();
    }

    public void OnReturnToPool()
    {
        //logs.Add($"Return to Pool: {currentFolder?.name ?? "null"} (should not be null) \n StackTrace: {Environment.StackTrace}");

        if (currentFolder == null)
        {
            //Debug.Log(logs.Aggregate((current, next) => current + "\n" + next));
            //Assert.IsTrue(false);
        }
        else
        {
            //Assert.IsNotNull(currentFolder);

            currentFolder.Change -= UpdateContent;

            currentFolder = null;
        }
    }

    public void UpdateContent()
    {
        headerText.text = currentFolder.name;

        var scale = expandIcon.localScale;
        scale.y = currentFolder.isExpanded ? -1 : 1; // Flip icon vertically if expanded
        expandIcon.localScale = scale;

        if (currentFolder.isExpanded)
        {
            folderFiller.UpdateFolderContent(currentFolder);
        }
        else
        {
            folderFiller.ClearFolderContent();
        }
    }

    public void ToggleExpanded()
    {
        currentFolder.isExpanded = !currentFolder.isExpanded;
    }

    public class Factory : PlaceholderFactory<AssetBrowserFolderHandler>
    {
    }
}
