using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using System;
using System.Collections.Generic;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AssetButtonHandler : MonoBehaviour
{
    public Image maskedImage; // Uses a child object with an image component. This allows setting an image that is influenced by the buttons mask.
    public Image assetTypeIndicatorImage;

    //public AssetButtonInteraction assetButtonInteraction;
    public GameObject loadingSymbol;
    public ClickHandler clickHandler;
    public TextHandler textHandler;
    public DragHandler dragHandler;

    private ScrollRect scrollViewRect;

    [Header("Asset Type Indicator Textures")]
    public Sprite modelTypeIndicator;

    public Sprite imageTypeIndicator;
    public Sprite sceneTypeIndicator;

    [Header("Thumbnail Sprites")]
    public Sprite errorAssetThumbnail;

    // Dependencies
    EditorEvents editorEvents;
    AssetThumbnailManagerSystem assetThumbnailManagerSystem;
    SceneManagerSystem sceneManagerSystem;

    [Inject]
    void Construct(EditorEvents editorEvents, AssetThumbnailManagerSystem assetThumbnailManagerSystem, SceneManagerSystem sceneManagerSystem)
    {
        this.editorEvents = editorEvents;
        this.assetThumbnailManagerSystem = assetThumbnailManagerSystem;
        this.sceneManagerSystem = sceneManagerSystem;
    }

    #region Initialization

    public void Init(string assetName,
        AssetButtonAtom.Data.TypeIndicator typeIndicator,
        // Placeholder Thumbnail Handler
        LeftClickStrategy leftClick,
        RightClickStrategy rightClick,
        DragStrategy dragStrategy)
    {
        SetText(assetName);

        SetTypeIndicator(typeIndicator);

        SetOnClick(leftClick, rightClick);

        SetDrag(dragStrategy);
    }

    private void SetDrag(DragStrategy dragStrategy)
    {
        dragHandler.dragStrategy = dragStrategy;
    }

    private void SetOnClick(LeftClickStrategy leftClick, RightClickStrategy rightClick)
    {
        clickHandler.leftClickStrategy = leftClick;
        clickHandler.rightClickStrategy = rightClick;
    }

    /*private void OnDestroy()
    {
        editorEvents.onAssetThumbnailUpdatedEvent -= OnAssetThumbnailUpdatedCallback;
    }*/

    /*private bool IsCyclicScene()
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
    }*/

    private void SetTypeIndicator(AssetButtonAtom.Data.TypeIndicator typeIndicator)
    {
        switch (typeIndicator)
        {
            case AssetButtonAtom.Data.TypeIndicator.None:
                assetTypeIndicatorImage.enabled = false;
                break;
            case AssetButtonAtom.Data.TypeIndicator.Model:
                assetTypeIndicatorImage.enabled = true;
                assetTypeIndicatorImage.sprite = modelTypeIndicator;
                break;
            case AssetButtonAtom.Data.TypeIndicator.Image:
                assetTypeIndicatorImage.enabled = true;
                assetTypeIndicatorImage.sprite = imageTypeIndicator;
                break;
            case AssetButtonAtom.Data.TypeIndicator.Scene:
                assetTypeIndicatorImage.enabled = true;
                assetTypeIndicatorImage.sprite = sceneTypeIndicator;
                break;
            default:
                break;
        }
    }

    private void SetText(ValueStrategy<string> assetName)
    {
        textHandler.SetTextValueStrategy(assetName);
    }

    #endregion


    /*
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
        if (metadata == null) return;

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
        }
    }

    public void OnAssetThumbnailUpdatedCallback(List<Guid> ids)
    {
        if (metadata == null) return;

        if (ids.Contains(metadata.assetId))
        {
            if (loadingSymbol != null) loadingSymbol.SetActive(false);

            var thumbnail = assetThumbnailManagerSystem.GetThumbnailById(metadata.assetId);
            if (thumbnail.texture != null) SetImage(thumbnail.texture);
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
    }*/

    public class Factory : PlaceholderFactory<AssetButtonHandler>
    {
    }
}