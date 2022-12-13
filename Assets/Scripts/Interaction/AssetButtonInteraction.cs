using System;
using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;

public class AssetButtonInteraction : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public AssetMetadata assetMetadata;
    private DclEntity newEntity;
    private bool entityInScene;
    private Vector3 mousePositionInScene;

    // Dependencies
    private CommandSystem commandSystem;
    private EditorEvents editorEvents;
    private InputHelper inputHelperSystem;
    private SceneDirectoryState sceneDirectoryState;
    private CameraState cameraState;

    [Inject]
    private void Construct(
        CommandSystem commandSystem,
        EditorEvents editorEvents,
        InputHelper inputHelperSystem,
        SceneDirectoryState sceneDirectoryState,
        CameraState cameraState)
    {
        this.commandSystem = commandSystem;
        this.inputHelperSystem = inputHelperSystem;
        this.sceneDirectoryState = sceneDirectoryState;
        this.editorEvents = editorEvents;
        this.cameraState = cameraState;
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

    public void OnBeginDrag(PointerEventData eventData) => newEntity = SetupEntity();

    public void OnDrag(PointerEventData eventData)
    {
        if (!inputHelperSystem.IsMouseOverScenePanel() && entityInScene)
        {
            sceneDirectoryState.CurrentScene.RemoveFloatingEntity(newEntity.Id);
            editorEvents.InvokeHierarchyChangedEvent();
            entityInScene = false;
            return;
        }

        if (inputHelperSystem.IsMouseOverScenePanel() && !entityInScene)
        {
            sceneDirectoryState.CurrentScene.AddFloatingEntity(newEntity);
            entityInScene = true;
        }

        mousePositionInScene = inputHelperSystem.GetMousePositionInScene();
        newEntity.GetTransformComponent().Position.SetFloatingValue(mousePositionInScene);
        editorEvents.InvokeHierarchyChangedEvent();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
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
        sceneDirectoryState.CurrentScene.RemoveFloatingEntity(newEntity.Id);
        switch (assetMetadata.assetType)
        {
            case AssetMetadata.AssetType.Model:
                commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateAddModelAssetToScene(newEntity.Id,
                    newEntity.CustomName, assetMetadata.assetId, position));
                break;
            case AssetMetadata.AssetType.Image:
                break;
        }
    } 
}