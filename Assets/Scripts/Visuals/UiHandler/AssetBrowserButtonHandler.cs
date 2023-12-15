using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using System;
using System.Collections.Generic;
using Assets.Scripts.Assets;
using UnityEngine;
using UnityEngine.Profiling.Memory.Experimental;
using UnityEngine.UI;
using Zenject;

public class AssetBrowserButtonHandler : ButtonHandler
{
    //public AssetMetadata metadata;
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

    //AssetThumbnailManagerSystem assetThumbnailManagerSystem;
    SceneManagerSystem sceneManagerSystem;
    PromptSystem promptSystem;

    [Inject]
    void Construct(EditorEvents editorEvents, /*AssetThumbnailManagerSystem assetThumbnailManagerSystem,*/ SceneManagerSystem sceneManagerSystem, PromptSystem promptSystem)
    {
        this.editorEvents = editorEvents;
        /*this.assetThumbnailManagerSystem = assetThumbnailManagerSystem;*/
        this.sceneManagerSystem = sceneManagerSystem;
        this.promptSystem = promptSystem;
    }

    public void InitUsageInUiAssetBrowser(AssetBrowserSystem.AbStructAsset abStructAsset)
    {
        //assetButtonInteraction.assetMetadata = metadata;
        maskedImage.sprite = null; // Clear thumbnail. There might be one still set because the prefab gets reused from the pool

        SetText(abStructAsset);
        SetTypeIndicator(abStructAsset);
        SetEnabled(true);
    }

    private void SetTypeIndicator(AssetBrowserSystem.AbStructAsset abStructAsset)
    {
        if (abStructAsset == null)
        {
            assetTypeIndicatorImage.sprite = null;
            assetTypeIndicatorImage.enabled = false;
            return;
        }

        assetTypeIndicatorImage.enabled = true;
        switch (abStructAsset.assetInfo.assetType)
        {
            case CommonAssetTypes.AssetType.Unknown:
                assetTypeIndicatorImage.sprite = null;
                assetTypeIndicatorImage.enabled = false;
                break;
            case CommonAssetTypes.AssetType.Model3D:
                assetTypeIndicatorImage.sprite = modelTypeIndicator;
                break;
            case CommonAssetTypes.AssetType.Image:
                assetTypeIndicatorImage.sprite = imageTypeIndicator;
                break;
            case CommonAssetTypes.AssetType.Scene:
                assetTypeIndicatorImage.sprite = sceneTypeIndicator;
                break;
            case CommonAssetTypes.AssetType.Entity:
                assetTypeIndicatorImage.sprite = null;
                assetTypeIndicatorImage.enabled = false;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void SetText(AssetBrowserSystem.AbStructAsset abStructAsset)
    {
        if (abStructAsset == null)
        {
            text.text = "None";
            return;
        }

        text.text = abStructAsset.name;
    }

    private void SetEnabled(bool value)
    {
        button.enabled = value;
        assetButtonInteraction.enableDragAndDrop = value && IsDragAndDropEnabled();
        maskedImage.color = value ? Color.white : Color.red;
    }

    private bool IsDragAndDropEnabled()
    {
        return false;
    }

    private void OnDestroy()
    {
        //editorEvents.onAssetThumbnailUpdatedEvent -= OnAssetThumbnailUpdatedCallback;
    }

    //#region Initialization
    private bool IsCyclicScene()
    {
        /*
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
        */

        return false;
    } /*
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
    #endregion*/

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
        /*if (metadata == null) return;

        loadingSymbol.SetActive(false);

        if (!IsVisibleInScrollView())
        {
            maskedImage.enabled = false;
            return;
        }

        var result = assetThumbnailManagerSystem.GetThumbnailById(metadata.assetId);
        switch (result.state)
        {
            case AssetData.State.IsAvailable:
                SetImage(result.texture);
                break;
            case AssetData.State.IsLoading:
                loadingSymbol.SetActive(true);
                break;
            case AssetData.State.IsError:
                SetImage(errorAssetThumbnail);
                break;
            default:
                break;
        }*/
    }

    public void OnAssetThumbnailUpdatedCallback(List<Guid> ids)
    {
        /*if (metadata == null) return;

        if (ids.Contains(metadata.assetId))
        {
            if (loadingSymbol != null) loadingSymbol.SetActive(false);

            var thumbnail = assetThumbnailManagerSystem.GetThumbnailById(metadata.assetId);
            if (thumbnail.texture != null) SetImage(thumbnail.texture);
            else SetImage(errorAssetThumbnail);
        }*/
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