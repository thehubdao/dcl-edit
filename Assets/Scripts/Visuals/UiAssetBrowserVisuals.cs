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
        private GameObject _scrollViewContent;
        [SerializeField]
        private GameObject _headerContent;
        [SerializeField]
        private GameObject _footerContent;

        // Dependencies
        private UiBuilder.Factory _uiBuilderFactory;
        private EditorEvents _editorEvents;
        private AssetBrowserSystem _assetBrowserSystem;
        private AssetManagerSystem _assetManagerSystem;
        private UnityState _unityState;
        private ContextMenuSystem _contextMenuSystem;

        [Inject]
        private void Construct(UiBuilder.Factory uiBuilderFactory, EditorEvents editorEvents, AssetBrowserSystem assetBrowserSystem, AssetManagerSystem assetManagerSystem, UnityState unityState, ContextMenuSystem contextMenuSystem)
        {
            _uiBuilderFactory = uiBuilderFactory;
            _editorEvents = editorEvents;
            _assetBrowserSystem = assetBrowserSystem;
            _assetManagerSystem = assetManagerSystem;
            _unityState = unityState;
            _contextMenuSystem = contextMenuSystem;
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
            _assetBrowserSystem.ChangeSorting(new AssetNameSorting(true));
            UpdateVisuals();
        }


        public void SetupSceneEventListeners()
        {
            _editorEvents.onAssetMetadataCacheUpdatedEvent += UpdateVisuals;
            _editorEvents.onUiChangedEvent += UpdateVisuals;
        }

        private void UpdateVisuals()
        {
            var assets = _assetBrowserSystem.GetFilteredMetadata();
            var rectTransform = _scrollViewContent.GetComponent<RectTransform>();

            // Build header
            var headerBuilder = _uiBuilderFactory.Create();
            var rowBuilder = _uiBuilderFactory.Create();
            rowBuilder.Text("Filter:");
            var headerButtonStyle = new TextHandler.TextStyle
            {
                color = TextHandler.TextColor.Normal,
                horizontalAlignment = TMPro.HorizontalAlignmentOptions.Center,
                verticalAlignment = TMPro.VerticalAlignmentOptions.Middle,
            };
            foreach (var f in _assetBrowserSystem.Filters)
            {
                switch (f)
                {
                    case AssetTypeFilter typeFilter:
                        rowBuilder.Button($"{typeFilter.GetName()} x", headerButtonStyle, () =>
                        {
                            _assetBrowserSystem.RemoveFilter(typeFilter);
                            _editorEvents.InvokeAssetMetadataCacheUpdatedEvent();
                        });
                        break;
                    default:
                        break;
                }
            }
            rowBuilder.Button("+", headerButtonStyle, (GameObject atom) =>
            {
                var rect = atom.GetComponent<RectTransform>();
                _contextMenuSystem.OpenMenu(new List<ContextMenuState.Placement>
                {
                    new ContextMenuState.Placement
                    {
                        position = rect.position + new Vector3(-rect.sizeDelta.x/2,-rect.sizeDelta.y,0),
                        expandDirection = ContextMenuState.Placement.Direction.Right,
                    },
                    new ContextMenuState.Placement{
                        position = rect.position + new Vector3(rect.sizeDelta.x/2,-rect.sizeDelta.y,0),
                        expandDirection = ContextMenuState.Placement.Direction.Left,
                    }
                }, new List<ContextMenuItem>
                {
                    new ContextMenuTextItem("Add model filter",() => _assetBrowserSystem.AddFilter(new AssetTypeFilter(AssetMetadata.AssetType.Model))),
                    new ContextMenuTextItem("Add image filter",() => _assetBrowserSystem.AddFilter(new AssetTypeFilter(AssetMetadata.AssetType.Image))),
                    new ContextMenuTextItem("Sort by name (A-Z)",() => _assetBrowserSystem.ChangeSorting(new AssetNameSorting(true))),
                    new ContextMenuTextItem("Sort by name (Z-A)",() => _assetBrowserSystem.ChangeSorting(new AssetNameSorting(false))),
                });
            });

            headerBuilder.Row(rowBuilder);
            headerBuilder.ClearAndMake(_headerContent);



            // Build asset library grid
            var scrollViewBuilder = _uiBuilderFactory.Create();
            var gridBuilder = _uiBuilderFactory.Create();
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
                            typeIndicator = _unityState.AssetTypeModelIcon;
                            break;
                        case AssetMetadata.AssetType.Image:
                            typeIndicator = _unityState.AssetTypeImageIcon;
                            break;
                        default:
                            break;
                    }

                    // Request thumbnail from Thumbnail Manager system
                    gridBuilder.AssetButton(
                        text: a.assetDisplayName,
                        textStyle: buttonTextStyle,
                        assetTypeIndicator: typeIndicator,
                        //thumbnail: thumbnail,
                        assetMetadata: a
                    );
                }
            }
            scrollViewBuilder.Grid(gridBuilder);
            scrollViewBuilder.ClearAndMake(_scrollViewContent);


            // Build footer
            var footerBuilder = _uiBuilderFactory.Create();
            var footerRowBuilder = _uiBuilderFactory.Create();
            footerRowBuilder.Button("Refresh", headerButtonStyle, () => _assetManagerSystem.CacheAllAssetMetadata());
            footerBuilder.Row(footerRowBuilder);
            footerBuilder.ClearAndMake(_footerContent);
        }
    }
}