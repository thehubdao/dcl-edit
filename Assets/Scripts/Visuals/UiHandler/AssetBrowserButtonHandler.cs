using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using System;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Security.Cryptography;
using Assets.Scripts.Assets;
using UnityEngine;
using UnityEngine.Profiling.Memory.Experimental;
using UnityEngine.UI;
using Zenject;
using Assets.Scripts.Visuals.UiHandler;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;
using static Assets.Scripts.Assets.CommonAssetTypes;
using static AssetBrowserSystem;

public class AssetBrowserButtonHandler : ButtonHandler, IPointerClickHandler, IDragHandler, IOnReturnToPool
{
    //public AssetMetadata metadata;
    public Image maskedImage; // Uses a child object with an image component. This allows setting an image that is influenced by the buttons mask.
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

    private AssetBrowserSystem.AbStructAsset currentAsset;

    private enum InteractionStrategy
    {
        UiAssetBrowser
    }

    private InteractionStrategy interactionStrategy;

    // Dependencies
    EditorEvents editorEvents;

    //AssetThumbnailManagerSystem assetThumbnailManagerSystem;
    SceneManagerSystem sceneManagerSystem;
    DiscoveredAssets discoveredAssets;
    PromptSystem promptSystem;
    AddEntitySystem addEntitySystem;
    CameraState cameraState;
    UnityState unityState;

    [Inject]
    void Construct(EditorEvents editorEvents,
        /*AssetThumbnailManagerSystem assetThumbnailManagerSystem,*/
        SceneManagerSystem sceneManagerSystem,
        DiscoveredAssets discoveredAssets,
        PromptSystem promptSystem,
        AddEntitySystem addEntitySystem,
        CameraState cameraState,
        UnityState unityState)
    {
        this.editorEvents = editorEvents;
        /*this.assetThumbnailManagerSystem = assetThumbnailManagerSystem;*/
        this.sceneManagerSystem = sceneManagerSystem;
        this.discoveredAssets = discoveredAssets;
        this.promptSystem = promptSystem;
        this.addEntitySystem = addEntitySystem;
        this.cameraState = cameraState;
        this.unityState = unityState;
    }

    public void InitUsageInUiAssetBrowser(AssetBrowserSystem.AbStructAsset abStructAsset)
    {
        Assert.IsNull(currentAsset, "Double initialization");

        currentAsset = abStructAsset;
        //assetButtonInteraction.assetMetadata = metadata;
        maskedImage.sprite = null; // Clear thumbnail. There might be one still set because the prefab gets reused from the pool

        UpdateAll();
        currentAsset.assetInfo.assetFormatChanged += UpdateAll;
    }

    public void UpdateAll()
    {
        SetText(currentAsset);
        SetTypeIndicator(currentAsset);
        SetThumbnail(currentAsset);
        SetEnabled(true);
    }

    public void OnReturnToPool()
    {
        if (currentAsset == null) return;

        currentAsset.assetInfo.assetFormatChanged -= UpdateAll;
        currentAsset = null;
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

    private void SetThumbnail(AbStructAsset abStructAsset)
    {
        var (availability, thumbnail) = discoveredAssets.GetAssetFormat<AssetFormatThumbnail>(abStructAsset.assetInfo.assetId);

        switch (availability)
        {
            case DiscoveredAssets.AssetFormatAvailability.Available:
                loadingSymbol.SetActive(false);
                maskedImage.sprite = thumbnail.thumbnail;
                maskedImage.enabled = true;
                break;
            case DiscoveredAssets.AssetFormatAvailability.Loading:
                loadingSymbol.SetActive(true);
                maskedImage.sprite = null;
                maskedImage.enabled = false;
                break;
            case DiscoveredAssets.AssetFormatAvailability.FormatError:
            case DiscoveredAssets.AssetFormatAvailability.FormatNotAvailable:
            case DiscoveredAssets.AssetFormatAvailability.DoesNotExist:
            default:
                Debug.Log(availability);
                loadingSymbol.SetActive(false);
                maskedImage.sprite = unityState.DefaultAssetThumbnail;
                maskedImage.enabled = true;
                break;
        }
    }

    private void SetEnabled(bool value)
    {
        button.enabled = value;
        //assetButtonInteraction.enableDragAndDrop = value && IsDragAndDropEnabled();
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
    }

    private void SetOnClickActionInUiAssetBrowser()
    {
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

    private DclEntity newEntity = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        switch (interactionStrategy)
        {
            case InteractionStrategy.UiAssetBrowser:
                OnPointerClickUiAssetBrowser(eventData);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnPointerClickUiAssetBrowser(PointerEventData eventData)
    {
        newEntity = SetupEntity();

        Ray ray = new Ray(cameraState.Position, cameraState.Forward);
        if (Physics.Raycast(ray, out RaycastHit hit, 50))
        {
            AddEntityToScene(hit.point);
        }
        else
        {
            AddEntityToScene(ray.GetPoint(10));
        }
    }

    private DclEntity SetupEntity()
    {
        var e = new DclEntity(Guid.NewGuid(), currentAsset.name);
        e.AddComponent(new DclTransformComponent());

        switch (currentAsset.assetInfo.assetType)
        {
            case AssetType.Model3D:
                var gltfShape = new DclGltfContainerComponent(currentAsset.assetInfo.assetId);
                e.AddComponent(gltfShape);
                break;
            case AssetType.Image:
                break;
            case AssetType.Scene:
                DclSceneComponent sceneComponent = new DclSceneComponent(currentAsset.assetInfo.assetId);
                e.AddComponent(sceneComponent);

                // Make all entities in the child scene to floating
                DclScene scene = sceneManagerSystem.GetScene(currentAsset.assetInfo.assetId);
                foreach (var entity in scene.AllEntities)
                {
                    scene.AddFloatingEntity(entity.Value);
                }

                scene.ClearAllEntities();
                break;
        }

        return e;
    }

    private void AddEntityToScene(Vector3 position)
    {
        sceneManagerSystem.GetCurrentSceneOrNull()?.RemoveFloatingEntity(newEntity.Id);
        switch (currentAsset.assetInfo.assetType)
        {
            case AssetType.Model3D:
                addEntitySystem.AddModelAssetEntityAsCommand(newEntity, currentAsset.assetInfo.assetId, position);
                break;
            case AssetType.Image:
                break;
            case AssetType.Scene:
                // Make all floating entities in child scene normal again
                DclScene scene = sceneManagerSystem.GetScene(currentAsset.assetInfo.assetId);
                foreach (var entity in scene.AllFloatingEntities)
                {
                    scene.AddEntity(entity.Value);
                }

                scene.ClearFloatingEntities();

                addEntitySystem.AddSceneAssetEntityAsCommand(newEntity, currentAsset.assetInfo.assetId, position);
                break;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        switch (interactionStrategy)
        {
            case InteractionStrategy.UiAssetBrowser:
                OnDragUiAssetBrowser(eventData);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void OnDragUiAssetBrowser(PointerEventData eventData)
    {
    }
}