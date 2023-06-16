using Assets.Scripts.Events;
using System;
using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class AssetButtonHandler : MonoBehaviour, IUpdateValue
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
    AssetManagerSystem assetManagerSystem;

    [Inject]
    void Construct(EditorEvents editorEvents, AssetThumbnailManagerSystem assetThumbnailManagerSystem, SceneManagerSystem sceneManagerSystem, AssetManagerSystem assetManagerSystem)
    {
        this.editorEvents = editorEvents;
        this.assetThumbnailManagerSystem = assetThumbnailManagerSystem;
        this.sceneManagerSystem = sceneManagerSystem;
        this.assetManagerSystem = assetManagerSystem;
    }

    #region Initialization

    private SetValueStrategy<Guid> valueBindStrategy;

    public void UpdateValue()
    {
        var assetId = valueBindStrategy.value();
        var assetData = assetManagerSystem.GetMetadataById(assetId);

        SetText(assetData.assetDisplayName);

        var typeIndicator = assetData.assetType switch
        {
            AssetMetadata.AssetType.Unknown => AssetButtonAtom.Data.TypeIndicator.None,
            AssetMetadata.AssetType.Model => AssetButtonAtom.Data.TypeIndicator.Model,
            AssetMetadata.AssetType.Image => AssetButtonAtom.Data.TypeIndicator.Image,
            AssetMetadata.AssetType.Scene => AssetButtonAtom.Data.TypeIndicator.Scene,
            _ => throw new Exception($"Unknown asset type: {assetData.assetType}")
        };

        SetTypeIndicator(typeIndicator);

        SetThumbnail(assetThumbnailManagerSystem.GetThumbnailById(assetId));
    }


    public void Setup(
        SetValueStrategy<Guid> valueBindStrategy,
        [CanBeNull] DragStrategy dragStrategy,
        [NotNull] ClickStrategy clickStrategy)
    {
        this.valueBindStrategy = valueBindStrategy;

        UpdateValue();

        dragHandler.dragStrategy = dragStrategy;
        clickHandler.clickStrategy = clickStrategy;
    }


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

    private void SetText(string assetName)
    {
        textHandler.SetTextValueStrategy(assetName);
    }

    private void SetThumbnail(AssetThumbnail thumbnail)
    {
        SetImage(thumbnail.texture);
    }

    #endregion


    public void SetImage(Texture2D tex)
    {
        if (tex == null)
        {
            tex = new Texture2D(2, 2);
        }

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

    public class Factory : PlaceholderFactory<AssetButtonHandler>
    {
    }
}