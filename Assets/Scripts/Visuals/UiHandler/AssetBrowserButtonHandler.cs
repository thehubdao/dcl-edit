using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
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
    public GameObject loadingSymbol;

    private ScrollRect scrollViewRect;

    [Header("Asset Type Indicator Textures")]
    public Sprite modelTypeIndicator;
    public Sprite imageTypeIndicator;
    public Sprite sceneTypeIndicator;

    [Header("Thumbnail Sprites")]
    public Sprite errorAssetThumbnail;

    // Dependencies
    EditorEvents editorEvents;
    SceneManagerSystem sceneManagerSystem;
    PromptSystem promptSystem;
    AssetManagerSystem assetManagerSystem;

    [Inject]
    void Construct(EditorEvents editorEvents, SceneManagerSystem sceneManagerSystem, AssetManagerSystem assetManagerSystem, PromptSystem promptSystem)
    {
        this.editorEvents = editorEvents;
        this.sceneManagerSystem = sceneManagerSystem;
        this.assetManagerSystem = assetManagerSystem;
        this.promptSystem = promptSystem;
    }

    public void Init(AssetMetadata metadata, bool enableDragAndDrop, Action<Guid> onClick, ScrollRect scrollViewRect = null)
    {
        this.metadata = metadata;
        assetButtonInteraction.assetMetadata = metadata;
        maskedImage.sprite = null;          // Clear thumbnail. There might be one still set because the prefab gets reused from the pool

        SetText(metadata);
        SetTypeIndicator(metadata);
        if (IsCyclicScene())
        {
            button.enabled = false;
            assetButtonInteraction.enableDragAndDrop = false;
            maskedImage.color = Color.red;
        }
        else
        {
            button.enabled = true;
            assetButtonInteraction.enableDragAndDrop = enableDragAndDrop;
            maskedImage.color = Color.white;
            SetOnClickAction(metadata, onClick);
        }

        editorEvents.onAssetThumbnailUpdatedEvent += OnAssetThumbnailUpdatedCallback;

        if (scrollViewRect != null)
        {
            this.scrollViewRect = scrollViewRect;
            scrollViewRect.onValueChanged.AddListener(ShowThumbnailWhenVisible);
        }

        // TODO: unsubscribe from assetthumbnailupdated and scrollviewupdated on destroy

        ShowThumbnailWhenVisible(Vector2.zero);
    }

    private void OnDestroy()
    {
        editorEvents.onAssetThumbnailUpdatedEvent -= OnAssetThumbnailUpdatedCallback;
    }

    #region Initialization
    private bool IsCyclicScene()
    {
        if (metadata == null) return false;
        if (metadata.assetType != AssetMetadata.AssetType.Scene) return false;

        SceneDirectoryState currentDirState = sceneManagerSystem.GetCurrentDirectoryState();
        if (currentDirState.id == metadata.assetId) return true;

        // Check all entities in the scene asset if they contain the current scene
        bool CheckForCyclicScenesRecursive(DclScene scene)
        {
            if (scene == null) return false;

            foreach (var childEntity in scene.AllEntities)
            {
                var sceneComponent = childEntity.Value.GetComponentByName("Scene");
                if (sceneComponent == null) continue;
                Guid? childSceneId = sceneComponent.GetPropertyByName("scene")?.GetConcrete<Guid>().FixedValue;
                if (childSceneId == null) continue;
                if (childSceneId == currentDirState.id) return true;

                DclScene childScene = sceneManagerSystem.GetScene(childSceneId.Value);
                if (CheckForCyclicScenesRecursive(childScene) == true) return true;
            }

            return false;
        }

        DclScene sceneFromAsset = sceneManagerSystem.GetScene(metadata.assetId);
        return CheckForCyclicScenesRecursive(sceneFromAsset);
    }
    private void SetTypeIndicator(AssetMetadata metadata)
    {
        if (metadata == null)
        {
            assetTypeIndicatorImage.sprite = null;
            assetTypeIndicatorImage.enabled = false;
            return;
        }

        assetTypeIndicatorImage.enabled = true;
        switch (this.metadata.assetType)
        {
            case AssetMetadata.AssetType.Unknown:
                break;
            case AssetMetadata.AssetType.Model:
                assetTypeIndicatorImage.sprite = modelTypeIndicator;
                break;
            case AssetMetadata.AssetType.Image:
                assetTypeIndicatorImage.sprite = imageTypeIndicator;
                break;
            case AssetMetadata.AssetType.Scene:
                assetTypeIndicatorImage.sprite = sceneTypeIndicator;
                break;
            default:
                break;
        }
    }
    private void SetText(AssetMetadata metadata)
    {
        if (metadata == null)
        {
            text.text = "None";
            return;
        }

        text.text = this.metadata.assetDisplayName;
    }
    private void SetOnClickAction(AssetMetadata metadata, Action<Guid> onClick)
    {
        button.onClick.RemoveAllListeners();

        if (onClick == null)
        {
            button.onClick.AddListener(assetButtonInteraction.OnClick);
            return;
        }

        if (metadata == null)
        {
            button.onClick.AddListener(() => onClick(Guid.Empty));
            return;
        }

        button.onClick.AddListener(() => onClick(metadata.assetId));
    }
    #endregion

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


    private async void ShowThumbnailWhenVisible(Vector2 _)
    {
        if (metadata == null) return;

        loadingSymbol.SetActive(false);

        if (!IsVisibleInScrollView())
        {
            maskedImage.enabled = false;
            return;
        }

        loadingSymbol.SetActive(true);

        var thumbnail = await assetManagerSystem.GetThumbnailSpriteById(metadata.assetId);

        loadingSymbol.SetActive(false);

        if (thumbnail != null)
        {
            SetImage(thumbnail);
        }
        else
        {
            SetImage(errorAssetThumbnail);
        }
    }

    public async void OnAssetThumbnailUpdatedCallback(List<Guid> ids)
    {
        if (metadata == null) return;

        if (ids.Contains(metadata.assetId))
        {
            if (loadingSymbol != null) loadingSymbol.SetActive(false);

            var thumbnail = await assetManagerSystem.GetThumbnailById(metadata.assetId);
            if (thumbnail != null) SetImage(thumbnail);
            else SetImage(errorAssetThumbnail);
        }
    }

    public void SetImage(Texture2D tex)
    {
        if (tex == null) return;
        SetImage(Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100));
    }

    public void SetImage(Sprite sprite)
    {
        if (maskedImage == null) return;

        if (sprite == null)
        {
            maskedImage.enabled = false;
            return;
        }

        maskedImage.sprite = sprite;
        maskedImage.enabled = true;
    }

    public class Factory : PlaceholderFactory<AssetBrowserButtonHandler>
    {
    }
}