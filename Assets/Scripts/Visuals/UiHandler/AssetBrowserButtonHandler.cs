using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AssetBrowserButtonHandler : ButtonHandler
{
    public AssetMetadata metadata;
    public Image maskedImage;       // Uses a child object with an image component. This allows setting an image that is influenced by the buttons mask.
    public Image assetTypeIndicatorImage;
    public AssetButtonInteraction assetButtonInteraction;
    private ScrollRect scrollViewRect;

    [Header("Asset Type Indicator Textures")]
    public Sprite modelTypeIndicator;
    public Sprite imageTypeIndicator;

    // Dependencies
    EditorEvents editorEvents;
    AssetThumbnailManagerSystem assetThumbnailManagerSystem;

    [Inject]
    void Construct(EditorEvents editorEvents, AssetThumbnailManagerSystem assetThumbnailManagerSystem)
    {
        this.editorEvents = editorEvents;
        this.assetThumbnailManagerSystem = assetThumbnailManagerSystem;
    }

    public void Init(AssetMetadata metadata, ScrollRect scrollViewRect = null)
    {
        this.metadata = metadata;

        assetButtonInteraction.assetMetadata = metadata;

        text.text = metadata.assetDisplayName;

        maskedImage.sprite = null;          // Clear thumbnail. There might be one still set because the prefab gets reused from the pool

        switch (metadata.assetType)
        {
            case AssetMetadata.AssetType.Unknown:
                break;
            case AssetMetadata.AssetType.Model:
                assetTypeIndicatorImage.sprite = modelTypeIndicator;
                break;
            case AssetMetadata.AssetType.Image:
                assetTypeIndicatorImage.sprite = imageTypeIndicator;
                break;
            default:
                break;
        }

        editorEvents.onAssetThumbnailUpdatedEvent += OnAssetThumbnailUpdatedCallback;

        if(scrollViewRect != null)
        {
            this.scrollViewRect = scrollViewRect;
            scrollViewRect.onValueChanged.AddListener(ShowThumbnailWhenVisible);
        }

        // TODO: unsubscribe from assetthumbnailupdated and scrollviewupdated on destroy

        ShowThumbnailWhenVisible(Vector2.zero);
    }



    private bool IsVisibleInScrollView()
    {
        // If not placed inside a scroll view, the content is always displayed.
        if (scrollViewRect == null)
        {
            return true;
        }

        var myRect = GetComponent<RectTransform>();

        var viewportTop = scrollViewRect.viewport.position.y;
        var viewportBottom = viewportTop - scrollViewRect.viewport.rect.height;

        return myRect.position.y <= viewportTop + 100 && myRect.position.y >= viewportBottom - 100;
    }


    private void ShowThumbnailWhenVisible(Vector2 _)
    {
        if (IsVisibleInScrollView())
        {
            if (maskedImage.sprite == null)
            {
                var result = assetThumbnailManagerSystem.GetThumbnailById(metadata.assetId);

                switch (result.state)
                {
                    case AssetData.State.IsAvailable:
                        SetImage(result.texture);
                        break;
                    case AssetData.State.IsLoading:
                        break;
                    case AssetData.State.IsError:
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (!maskedImage.enabled)
                {
                    maskedImage.enabled = true;
                }
            }
        }
        else
        {
            if (maskedImage.enabled)
            {
                maskedImage.enabled = false;
            }
        }
    }

    public void OnAssetThumbnailUpdatedCallback(List<Guid> ids)
    {
        if (ids.Contains(metadata.assetId))
        {
            var thumbnail = assetThumbnailManagerSystem.GetThumbnailById(metadata.assetId);
            if (thumbnail.texture != null)
            {
                SetImage(thumbnail.texture);
            }
        }
    }

    public void SetImage(Texture2D tex)
    {
        if (tex != null)
        {
            maskedImage.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
        }

        if (!maskedImage.enabled)
        {
            maskedImage.enabled = true;
        }
    }

    public class Factory : PlaceholderFactory<AssetBrowserButtonHandler>
    {
    }
}