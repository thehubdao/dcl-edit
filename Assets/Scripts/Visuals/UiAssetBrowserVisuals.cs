using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class UiAssetBrowserVisuals : MonoBehaviour
    {
        public Action<Guid> assetButtonOnClickOverride = null;

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

        // Dependencies
        private EditorEvents editorEvents;

        //private AssetBrowserSystem assetBrowserSystem;
        private AssetBrowserState assetBrowserState;
        private AssetManagerSystem assetManagerSystem;
        private ContextMenuSystem contextMenuSystem;

        [Inject]
        private void Construct(
            UiBuilder.UiBuilder.Factory uiBuilderFactory,
            EditorEvents editorEvents,
            //AssetBrowserSystem assetBrowserSystem,
            AssetBrowserState assetBrowserState,
            AssetManagerSystem assetManagerSystem,
            ContextMenuSystem contextMenuSystem)
        {
            headerUiBuilder = uiBuilderFactory.Create(headerContent);
            contentUiBuilder = uiBuilderFactory.Create(scrollViewContent);
            footerUiBuilder = uiBuilderFactory.Create(footerContent);
            this.editorEvents = editorEvents;
            //this.assetBrowserSystem = assetBrowserSystem;
            this.assetBrowserState = assetBrowserState;
            this.assetManagerSystem = assetManagerSystem;
            this.contextMenuSystem = contextMenuSystem;

            SetupSceneEventListeners();
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
        }

        public void SetupSceneEventListeners()
        {
            editorEvents.onAssetMetadataCacheUpdatedEvent += UpdateVisuals;
            editorEvents.onUiChangedEvent += UpdateVisuals;
            editorEvents.OnCurrentSceneChangedEvent += UpdateContent;
        }

        private void OnDestroy()
        {
            editorEvents.onAssetMetadataCacheUpdatedEvent -= UpdateVisuals;
            editorEvents.onUiChangedEvent -= UpdateVisuals;
            editorEvents.OnCurrentSceneChangedEvent -= UpdateContent;
        }

        private void UpdateVisuals()
        {
            UpdateHeader();
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
            var panel = new PanelAtom.Data();
            //var assetHierarchy = assetBrowserSystem.GetFilteredAssetHierarchy();
            //
            //foreach (AssetHierarchyItem hierarchyItem in assetHierarchy)
            //{
            //    BuildHierarchy(hierarchyItem, panel);
            //}

            contentUiBuilder.Update(panel);
        }

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