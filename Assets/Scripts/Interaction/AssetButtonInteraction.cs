using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class AssetButtonInteraction : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public AssetMetadata assetMetadata;
    public bool enableDragAndDrop;
    private DclEntity newEntity;
    private bool entityInScene;
    private Vector3 mousePositionInScene;

    // Dependencies
    private EditorEvents editorEvents;
    private InputHelper inputHelperSystem;
    private SceneManagerSystem sceneManagerSystem;
    private CameraState cameraState;
    private AddEntitySystem addEntitySystem;

    [Inject]
    private void Construct(
        EditorEvents editorEvents,
        InputHelper inputHelperSystem,
        SceneManagerSystem sceneManagerSystem,
        CameraState cameraState,
        AddEntitySystem addEntitySystem)
    {
        this.inputHelperSystem = inputHelperSystem;
        this.sceneManagerSystem = sceneManagerSystem;
        this.editorEvents = editorEvents;
        this.cameraState = cameraState;
        this.addEntitySystem = addEntitySystem;
    }

    public void OnClick()
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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!enableDragAndDrop) return;
        newEntity = SetupEntity();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!enableDragAndDrop) return;

        var currentScene = sceneManagerSystem.GetCurrentScene();

        if (currentScene == null)
        {
            return;
        }

        if (!inputHelperSystem.IsMouseOverScenePanel() && entityInScene)
        {
            currentScene.RemoveFloatingEntity(newEntity.Id);
            editorEvents.InvokeHierarchyChangedEvent();
            entityInScene = false;
            return;
        }

        if (inputHelperSystem.IsMouseOverScenePanel() && !entityInScene)
        {
            currentScene.AddFloatingEntity(newEntity);
            entityInScene = true;
        }

        mousePositionInScene = inputHelperSystem.GetMousePositionInScene();
        newEntity.GetTransformComponent().Position.SetFloatingValue(mousePositionInScene);
        editorEvents.InvokeHierarchyChangedEvent();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!enableDragAndDrop) return;
        if (!inputHelperSystem.IsMouseOverScenePanel()) return;

        AddEntityToScene(mousePositionInScene);
    }

    private DclEntity SetupEntity()
    {
        var e = new DclEntity(Guid.NewGuid(), assetMetadata.assetDisplayName);
        e.AddComponent(new DclTransformComponent());

        switch (assetMetadata.assetType)
        {
            case AssetMetadata.AssetType.Model:
                var gltfShape = new DclGltfShapeComponent(assetMetadata.assetId);
                e.AddComponent(gltfShape);
                break;
            case AssetMetadata.AssetType.Image:
                break;
        }

        return e;
    }

    private void AddEntityToScene(Vector3 position)
    {
        sceneManagerSystem.GetCurrentScene()?.RemoveFloatingEntity(newEntity.Id);
        switch (assetMetadata.assetType)
        {
            case AssetMetadata.AssetType.Model:
                addEntitySystem.AddModelAssetEntityAsCommand(newEntity, assetMetadata, position);

                break;
            case AssetMetadata.AssetType.Image:
                break;
        }
    }
}