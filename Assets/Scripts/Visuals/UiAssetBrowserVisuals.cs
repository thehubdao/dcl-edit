using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class UiAssetBrowserVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
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
        private GameObject assetButtonPrefab;
        [SerializeField]
        private GameObject assetBrowserFolderPrefab;
        [SerializeField]
        private GameObject assetGridPrefab;


        // Dependencies
        private EditorEvents editorEvents;
        private AssetBrowserSystem assetBrowserSystem;
        private AssetManagerSystem assetManagerSystem;
        private ContextMenuSystem contextMenuSystem;


        [Inject]
        private void Construct(
            UiBuilder.UiBuilder.Factory uiBuilderFactory,
            EditorEvents editorEvents,
            AssetBrowserSystem assetBrowserSystem,
            AssetManagerSystem assetManagerSystem,
            ContextMenuSystem contextMenuSystem)
        {
            headerUiBuilder = uiBuilderFactory.Create(headerContent);
            contentUiBuilder = uiBuilderFactory.Create(scrollViewContent);
            footerUiBuilder = uiBuilderFactory.Create(footerContent);
            this.editorEvents = editorEvents;
            this.assetBrowserSystem = assetBrowserSystem;
            this.assetManagerSystem = assetManagerSystem;
            this.contextMenuSystem = contextMenuSystem;
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
            assetBrowserSystem.ChangeSorting(AssetBrowserState.Sorting.NameAscending);
            UpdateVisuals();
        }


        public void SetupSceneEventListeners()
        {
            editorEvents.onAssetMetadataCacheUpdatedEvent += UpdateVisuals;
            editorEvents.onUiChangedEvent += UpdateVisuals;
        }


        private void UpdateVisuals()
        {
            UpdateHeader();
            UpdateContent();
            UpdateFooter();
        }


        private void UpdateHeader()
        {
            var headerData = new PanelAtom.Data
            {
                layoutDirection = PanelHandler.LayoutDirection.Horizontal,
                useFullWidth = false
            };

            headerData.AddText("Filter:");
            foreach (AssetMetadata.AssetType type in assetBrowserSystem.filters)
            {
                headerData.AddButton(type.ToString() + " x", _ => assetBrowserSystem.RemoveFilter(type));
            }

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
                    new ContextMenuTextItem("Add model filter", () => assetBrowserSystem.AddFilter(AssetMetadata.AssetType.Model)),
                    new ContextMenuTextItem("Add image filter", () => assetBrowserSystem.AddFilter(AssetMetadata.AssetType.Image)),
                    new ContextMenuTextItem("Sort by name (A-Z)", () => assetBrowserSystem.ChangeSorting(AssetBrowserState.Sorting.NameAscending)),
                    new ContextMenuTextItem("Sort by name (Z-A)", () => assetBrowserSystem.ChangeSorting(AssetBrowserState.Sorting.NameDescending)),
                });
            });

            headerUiBuilder.Update(headerData);
        }


        private void UpdateContent()
        {
            var panel = new PanelAtom.Data();

            var assetHierarchy = assetBrowserSystem.GetFilteredAssetHierarchy();

            foreach (var item in assetHierarchy)
            {
                panel.AddAssetBrowserFolder(item, scrollViewRect);
            }

            contentUiBuilder.Update(panel);
        }


        private void UpdateFooter()
        {
            var footerData = new PanelAtom.Data
            {
                layoutDirection = PanelHandler.LayoutDirection.Horizontal,
                useFullWidth = false
            };

            footerData.AddButton("Refresh", _ => assetManagerSystem.CacheAllAssetMetadata());
            footerUiBuilder.Update(footerData);
        }
    }
}