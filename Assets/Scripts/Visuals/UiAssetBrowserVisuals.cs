using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Assets.Scripts.Assets;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static AssetBrowserSystem;

namespace Assets.Scripts.Visuals
{
    public class UiAssetBrowserVisuals : MonoBehaviour
    {
        //private struct GameObjectInstanceTree
        //{
        //    public struct GameObjectInstanceTreeNode
        //    {
        //        private CommonAssetTypes.GameObjectInstance gameObjectInstance;
        //
        //        private List<GameObjectInstanceTreeNode> children;
        //
        //        public void ReturnToPool()
        //        {
        //            ReturnChildrenToPool();
        //
        //            gameObjectInstance.ReturnToPool();
        //        }
        //
        //        public void ReturnChildrenToPool()
        //        {
        //            foreach (var child in children)
        //            {
        //                child.ReturnToPool();
        //            }
        //
        //            children.Clear();
        //        }
        //
        //        public void AddChild(GameObjectInstanceTreeNode newChild)
        //        {
        //            children.Add(newChild);
        //        }
        //    }
        //
        //    public List<GameObjectInstanceTreeNode> rootNodes;
        //
        //    public void ReturnAllToPool()
        //    {
        //        foreach (var node in rootNodes)
        //        {
        //            node.ReturnToPool();
        //        }
        //    }
        //}

        public Action<Guid> assetButtonOnClickOverride = null;

        public List<CommonAssetTypes.GameObjectInstance> allGameObjectInstances = new();

        [SerializeField]
        private ScrollRect scrollViewRect;

        [SerializeField]
        private GameObject headerContent;
        private UiBuilder.UiBuilder headerUiBuilder;

        [SerializeField]
        private GameObject scrollViewContent;
        private UiBuilder.UiBuilder contentUiBuilder;

        [SerializeField]
        private GameObject footerContent;
        private UiBuilder.UiBuilder footerUiBuilder;

        [SerializeField]
        private AssetBrowserFolderFillerHandler assetBrowserFolderFillerHandler;

        // Dependencies
        private EditorEvents editorEvents;

        private AssetBrowserSystem assetBrowserSystem;
        private AssetBrowserState assetBrowserState;
        private AssetManagerSystem assetManagerSystem;
        private ContextMenuSystem contextMenuSystem;
        private DiscoveredAssets discoveredAssets;
        private SpecialAssets specialAssets;

        [Inject]
        private void Construct(
            UiBuilder.UiBuilder.Factory uiBuilderFactory,
            EditorEvents editorEvents,
            AssetBrowserSystem assetBrowserSystem,
            AssetBrowserState assetBrowserState,
            AssetManagerSystem assetManagerSystem,
            ContextMenuSystem contextMenuSystem,
            DiscoveredAssets discoveredAssets,
            SpecialAssets specialAssets)
        {
            headerUiBuilder = uiBuilderFactory.Create(headerContent);
            contentUiBuilder = uiBuilderFactory.Create(scrollViewContent);
            footerUiBuilder = uiBuilderFactory.Create(footerContent);
            this.editorEvents = editorEvents;
            this.assetBrowserSystem = assetBrowserSystem;
            this.assetBrowserState = assetBrowserState;
            this.assetManagerSystem = assetManagerSystem;
            this.contextMenuSystem = contextMenuSystem;
            this.discoveredAssets = discoveredAssets;
            this.specialAssets = specialAssets;
        }

        void Start()
        {
            // Workaround: Ui container of asset browser is not initialized in first frame and has 0 width. Therefore
            // wait one frame before initial UpdateVisuals().
            StartCoroutine(LateStart());
        }
        IEnumerator LateStart()
        {
            yield return null;
            //assetBrowserSystem.ChangeSorting(AssetBrowserState.Sorting.NameAscending);
            UpdateVisuals();
            SetupSceneEventListeners();
        }

        public void SetupSceneEventListeners()
        {
            //editorEvents.onAssetMetadataCacheUpdatedEvent += UpdateVisuals;
            //editorEvents.onUiChangedEvent += UpdateVisuals;
            //editorEvents.OnCurrentSceneChangedEvent += UpdateContent;
            assetBrowserSystem.rootItem.Change += UpdateContent;
        }

        private void OnDestroy()
        {
            //editorEvents.onAssetMetadataCacheUpdatedEvent -= UpdateVisuals;
            //editorEvents.onUiChangedEvent -= UpdateVisuals;
            //editorEvents.OnCurrentSceneChangedEvent -= UpdateContent;
        }

        private void UpdateVisuals()
        {
            //var sb = new StringBuilder();
            //
            //foreach (var asset in discoveredAssets.discoveredAssets)
            //{
            //    sb.AppendLine($"{asset.displayPath}/{asset.assetName}");
            //}
            //
            //Debug.Log(sb);

            //UpdateHeader();
            UpdateContent();
            //UpdateFooter();
        }

        private void UpdateHeader()
        {
            var headerData = new PanelAtom.Data
            {
                layoutDirection = PanelHandler.LayoutDirection.Horizontal,
                useFullWidth = false
            };

            headerData.AddText("Filter:");

            //foreach (AssetMetadata.AssetType type in assetBrowserSystem.filters)
            //{
            //    headerData.AddButton(type.ToString() + " x", _ => assetBrowserSystem.RemoveFilter(type));
            //}

            headerData.AddButton("+", btn =>
            {
                var rect = btn.GetComponent<RectTransform>();
                contextMenuSystem.OpenMenu(new List<ContextMenuState.Placement>
                {
                    new ContextMenuState.Placement
                    {
                        position = rect.position + new Vector3(0, -rect.sizeDelta.y, 0),
                        expandDirection = ContextMenuState.Placement.Direction.Right,
                    },
                    new ContextMenuState.Placement
                    {
                        position = rect.position + new Vector3(rect.sizeDelta.x, -rect.sizeDelta.y, 0),
                        expandDirection = ContextMenuState.Placement.Direction.Left,
                    }
                }, new List<ContextMenuItem>
                {
                    //new ContextMenuTextItem("Add model filter", () => assetBrowserSystem.AddFilter(AssetMetadata.AssetType.Model)),
                    ////new ContextMenuTextItem("Add image filter", () => assetBrowserSystem.AddFilter(AssetMetadata.AssetType.Image)),
                    //new ContextMenuTextItem("Add scene filter", () => assetBrowserSystem.AddFilter(AssetMetadata.AssetType.Scene)),
                    //new ContextMenuTextItem("Sort by name (A-Z)", () => assetBrowserSystem.ChangeSorting(AssetBrowserState.Sorting.NameAscending)),
                    //new ContextMenuTextItem("Sort by name (Z-A)", () => assetBrowserSystem.ChangeSorting(AssetBrowserState.Sorting.NameDescending)),
                });
            });

            headerUiBuilder.Update(headerData);
        }

        private void UpdateContent()
        {
            Debug.Log("Update Content");
            // clear folder Content
            //ClearFolderContent();

            UpdateFolderContent(assetBrowserSystem.rootItem);
        }

        private void UpdateFolderContent(AssetBrowserSystem.AbStructFolder parentAbStructFolder)
        {
            Debug.Log("Update Folder Content");

            assetBrowserFolderFillerHandler.UpdateFolderContent(parentAbStructFolder);

            //foreach (var abStructItem in parentAbStructFolder.GetItems())
            //{
            //    switch (abStructItem)
            //    {
            //        case AssetBrowserSystem.AbStructAsset abStructAsset:
            //            AddAssetContent(parentObject, abStructAsset);
            //            break;
            //        case AssetBrowserSystem.AbStructFolder abStructFolder:
            //            AddFolder(parentObject, abStructFolder);
            //            break;
            //    }
            //}
        }

        //private void ClearFolderContent()
        //{
        //    foreach (var gameObjectInstance in allGameObjectInstances)
        //    {
        //        gameObjectInstance.ReturnToPool();
        //    }
        //}

        //private void AddFolder(GameObject parentObject, AssetBrowserSystem.AbStructFolder abStructFolder)
        //{
        //    Debug.Log("Add folder");
        //    var folderUiElement = specialAssets.assetFolderUiElement.CreateInstance();
        //    folderUiElement.gameObject.transform.SetParent(parentObject.GetComponent<AssetBrowserFolderHandler>()?.subFolderContainer ?? parentObject.transform, false);
        //    folderUiElement.gameObject.GetComponent<AssetBrowserFolderHandler>().Init(abStructFolder);
        //    allGameObjectInstances.Add(folderUiElement);
        //    UpdateFolderContent(folderUiElement.gameObject, abStructFolder);
        //}
        //
        //private void AddAssetContent(GameObject parentObject, AssetBrowserSystem.AbStructAsset abStructAsset)
        //{
        //    Debug.Log("Add content");
        //    var buttonUiElement = specialAssets.assetButtonUiElement.CreateInstance();
        //    buttonUiElement.gameObject.transform.SetParent(parentObject.GetComponent<AssetBrowserFolderHandler>().assetButtonContainer, false);
        //    buttonUiElement.gameObject.GetComponent<AssetBrowserButtonHandler>().InitUsageInUiAssetBrowser(abStructAsset);
        //    allGameObjectInstances.Add(buttonUiElement);
        //}

        /*private void UpdateFooter()
        {
            var footerData = new PanelAtom.Data
            {
                layoutDirection = PanelHandler.LayoutDirection.Horizontal,
                useFullWidth = false
            };

            footerData.AddButton("Refresh", _ => assetManagerSystem.CacheAllAssetMetadata());
            footerUiBuilder.Update(footerData);
        }

        private void BuildHierarchy(AssetHierarchyItem hierarchyItem, PanelAtom.Data panel)
        {
            if (hierarchyItem.IsEmpty()) return;

            int indentationLevel = GetIndentationLevel(hierarchyItem);
            panel.AddAssetBrowserFolder(hierarchyItem, indentationLevel);

            if (!assetBrowserState.expandedFoldersPaths.Contains(hierarchyItem.path)) return;

            foreach (AssetHierarchyItem dir in hierarchyItem.childDirectories)
            {
                if (dir.IsEmpty()) continue;
                BuildHierarchy(dir, panel);
            }

            if (hierarchyItem.assets.Count == 0) return;

            var grid = panel.AddGrid(indentationLevel);
            bool enableDragAndDrop = assetButtonOnClickOverride == null;
            foreach (AssetMetadata asset in hierarchyItem.assets)
            {
                grid.AddAssetBrowserButton(asset, enableDragAndDrop, assetButtonOnClickOverride, scrollViewRect);
            }
        }

        int GetIndentationLevel(AssetHierarchyItem hierarchyItem)
        {
            string[] directories = hierarchyItem.path.Split(Path.AltDirectorySeparatorChar);
            return directories.Length - 1;
        }*/

        public class Factory : PlaceholderFactory<UiAssetBrowserVisuals>
        {

        }
    }
}