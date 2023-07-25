using System;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using JetBrains.Annotations;
using UnityEngine;
using Visuals.UiHandler;
using Zenject;

public class DropAssetOnSceneView : MonoBehaviour
{
    [SerializeField]
    private DropHandler dropHandler;


    // Dependencies
    private AssetBrowserSystem assetBrowserSystem;
    private AssetManagerSystem assetManagerSystem;
    private SceneManagerSystem sceneManagerSystem;
    private InputHelper inputHelper;
    private EditorEvents editorEvents;

    [Inject]
    private void Construct(
        AssetBrowserSystem assetBrowserSystem,
        AssetManagerSystem assetManagerSystem,
        SceneManagerSystem sceneManagerSystem,
        InputHelper inputHelper,
        EditorEvents editorEvents)
    {
        this.assetBrowserSystem = assetBrowserSystem;
        this.assetManagerSystem = assetManagerSystem;
        this.sceneManagerSystem = sceneManagerSystem;
        this.inputHelper = inputHelper;
        this.editorEvents = editorEvents;
    }


    // Cache
    [CanBeNull]
    private DclEntity tmpEntity = null;

    private DclEntity GetTemporaryEntityFromAsset(AssetMetadata asset)
    {
        if (tmpEntity != null) return tmpEntity;


        tmpEntity = new DclEntity(Guid.NewGuid());
        tmpEntity.AddComponent(new DclTransformComponent());

        switch (asset.assetType)
        {
            case AssetMetadata.AssetType.Unknown:
                throw new ArgumentOutOfRangeException();

            case AssetMetadata.AssetType.Model:
                tmpEntity.AddComponent(new DclGltfShapeComponent(asset.assetId));
                break;

            case AssetMetadata.AssetType.Scene:
                tmpEntity.AddComponent(new DclSceneComponent(asset.assetId));
                break;

            case AssetMetadata.AssetType.Image:
                throw new NotImplementedException();

            default:
                throw new ArgumentOutOfRangeException();
        }

        sceneManagerSystem.GetCurrentScene().AddFloatingEntity(tmpEntity);

        return tmpEntity;
    }

    private void RemoveTemporaryEntity()
    {
        sceneManagerSystem.GetCurrentScene().ClearFloatingEntities();

        tmpEntity = null;
    }


    void Awake()
    {
        void AddAssetToScene(Guid assetId)
        {
            RemoveTemporaryEntity();

            var assetMetadata = assetManagerSystem.GetMetadataById(assetId);
            assetBrowserSystem.AddAssetToSceneAtMousePositionInViewport(assetMetadata);
        }

        void ShowAssetInScene(Guid assetId)
        {
            var assetMetadata = assetManagerSystem.GetMetadataById(assetId);
            var entity = GetTemporaryEntityFromAsset(assetMetadata);

            var position = inputHelper.GetMousePositionInScene();

            entity.GetTransformComponent().position.SetFloatingValue(position);

            editorEvents.InvokeHierarchyChangedEvent();
        }

        dropHandler.dropStrategy = new DropStrategy
        {
            dropModelAssetStrategy =
                new DropModelAssetStrategy(AddAssetToScene, ShowAssetInScene),
            dropSceneAssetStrategy =
                new DropSceneAssetStrategy(AddAssetToScene, ShowAssetInScene)
        };
    }
}
