using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiHandler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class UiAssetBrowserVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        [SerializeField]
        private GameObject scrollViewContent;

        [SerializeField]
        private GameObject headerContent;

        [SerializeField]
        private GameObject footerContent;

        // Dependencies
        private UiBuilder.Factory uiBuilderFactory;
        private EditorEvents editorEvents;
        private AssetBrowserSystem assetBrowserSystem;
        private AssetManagerSystem assetManagerSystem;
        private UnityState unityState;
        private ContextMenuSystem contextMenuSystem;
        private AssetThumbnailManagerSystem assetThumbnailManagerSystem;

        [Inject]
        private void Construct(
            UiBuilder.Factory uiBuilderFactory,
            EditorEvents editorEvents,
            AssetBrowserSystem assetBrowserSystem,
            AssetThumbnailManagerSystem assetThumbnailManagerSystem,
            AssetManagerSystem assetManagerSystem,
            UnityState unityState,
            ContextMenuSystem contextMenuSystem)
        {
            this.uiBuilderFactory = uiBuilderFactory;
            this.editorEvents = editorEvents;
            this.assetBrowserSystem = assetBrowserSystem;
            this.assetManagerSystem = assetManagerSystem;
            this.unityState = unityState;
            this.contextMenuSystem = contextMenuSystem;
            this.assetThumbnailManagerSystem = assetThumbnailManagerSystem;
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
            assetBrowserSystem.ChangeSorting(new AssetNameSorting(true));
            UpdateVisuals();
        }


        public void SetupSceneEventListeners()
        {
            editorEvents.onAssetMetadataCacheUpdatedEvent += UpdateVisuals;
            editorEvents.onUiChangedEvent += UpdateVisuals;
            editorEvents.onAssetThumbnailUpdatedEvent += _ => UpdateVisuals();
        }

        private void UpdateVisuals()
        {
            Debug.Log("Update Asset Browser Visuals");
            var assets = assetBrowserSystem.GetFilteredMetadata();
            //var rectTransform = scrollViewContent.GetComponent<RectTransform>();

            // Build header
            var headerBuilder = uiBuilderFactory.Create();
            var rowBuilder = uiBuilderFactory.Create();
            rowBuilder.Text("Filter:");
            var headerButtonStyle = new TextHandler.TextStyle
            {
                color = TextHandler.TextColor.Normal,
                horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center,
                verticalAlignment = TMPro.VerticalAlignmentOptions.Middle,
            };
            foreach (var f in assetBrowserSystem.Filters)
            {
                switch (f)
                {
                    case AssetTypeFilter typeFilter:
                        rowBuilder.Button($"{typeFilter.GetName()} x", headerButtonStyle, () =>
                        {
                            assetBrowserSystem.RemoveFilter(typeFilter);
                            editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
                        });
                        break;
                    default:
                        break;
                }
            }
            rowBuilder.Button("+", headerButtonStyle, (GameObject atom) =>
            {
                var rect = atom.GetComponent<RectTransform>();
                contextMenuSystem.OpenMenu(new List<ContextMenuState.Placement>
                {
                    new ContextMenuState.Placement
                    {
                        position = rect.position + new Vector3(-rect.sizeDelta.x / 2, -rect.sizeDelta.y, 0),
                        expandDirection = ContextMenuState.Placement.Direction.Right,
                    },
                    new ContextMenuState.Placement
                    {
                        position = rect.position + new Vector3(rect.sizeDelta.x / 2, -rect.sizeDelta.y, 0),
                        expandDirection = ContextMenuState.Placement.Direction.Left,
                    }
                }, new List<ContextMenuItem>
                {
                    new ContextMenuTextItem("Add model filter", () => assetBrowserSystem.AddFilter(new AssetTypeFilter(AssetMetadata.AssetType.Model))),
                    new ContextMenuTextItem("Add image filter", () => assetBrowserSystem.AddFilter(new AssetTypeFilter(AssetMetadata.AssetType.Image))),
                    new ContextMenuTextItem("Sort by name (A-Z)", () => assetBrowserSystem.ChangeSorting(new AssetNameSorting(true))),
                    new ContextMenuTextItem("Sort by name (Z-A)", () => assetBrowserSystem.ChangeSorting(new AssetNameSorting(false))),
                });
            });

            headerBuilder.Row(rowBuilder);
            headerBuilder.ClearAndMake(headerContent);


            // Build asset library grid
            var scrollViewBuilder = uiBuilderFactory.Create();
            var gridBuilder = uiBuilderFactory.Create();
            var buttonTextStyle = new TextHandler.TextStyle
            {
                horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center,
                verticalAlignment = TMPro.VerticalAlignmentOptions.Middle,
                color = TextHandler.TextColor.Normal
            };

            if (assets != null)
            {
                var i = 0;
                foreach (var a in assets)
                {
                    if (i++ > 100)
                    {
                        break;
                    }

                    Texture2D typeIndicator = null;
                    switch (a.assetType)
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

                    var thumbnail = assetThumbnailManagerSystem.GetThumbnailById(a.assetId);
                    gridBuilder.AssetButton(
                        text: a.assetDisplayName,
                        textStyle: buttonTextStyle,
                        assetTypeIndicator: typeIndicator,
                        thumbnail: thumbnail.texture,
                        assetMetadata: a
                    );
                }
            }
            scrollViewBuilder.Grid(gridBuilder);
            scrollViewBuilder.ClearAndMake(scrollViewContent);


            // Build footer
            var footerBuilder = uiBuilderFactory.Create();
            var footerRowBuilder = uiBuilderFactory.Create();
            footerRowBuilder.Button("Refresh", headerButtonStyle, () => assetManagerSystem.CacheAllAssetMetadata());
            footerBuilder.Row(footerRowBuilder);
            footerBuilder.ClearAndMake(footerContent);
        }
    }
}