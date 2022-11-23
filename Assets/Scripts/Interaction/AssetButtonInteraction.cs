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
    private DclEntity _newEntity;
    private Vector3 mousePositionInScene;

    // Dependencies
    CommandSystem _commandSystem;
    InputHelper _inputHelperSystem;
    SceneDirectoryState _sceneDirectoryState;
    EditorEvents _editorEvents;

    [Inject]
    private void Construct(CommandSystem commandSystem, InputHelper inputHelperSystem, SceneDirectoryState sceneDirectoryState, EditorEvents editorEvents)
    {
        _commandSystem = commandSystem;
        _inputHelperSystem = inputHelperSystem;
        _sceneDirectoryState = sceneDirectoryState;
        _editorEvents = editorEvents;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _newEntity = new DclEntity(Guid.NewGuid(), assetMetadata.assetDisplayName);
        _newEntity.AddComponent(new DclTransformComponent());

        switch (assetMetadata.assetType)
        {
            case AssetMetadata.AssetType.Model:
                var gltfShape = new DclGltfShapeComponent(assetMetadata.assetId);
                _newEntity.AddComponent(gltfShape);
                _sceneDirectoryState.CurrentScene.AddFloatingEntity(_newEntity);
                break;
            case AssetMetadata.AssetType.Image:
                break;
            default:
                break;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_inputHelperSystem.IsMouseOverScenePanel())
        {
            mousePositionInScene = _inputHelperSystem.GetMousePositionInScene();
            _newEntity.GetTransformComponent().Position.SetFloatingValue(mousePositionInScene);
            _editorEvents.InvokeHierarchyChangedEvent();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        _sceneDirectoryState.CurrentScene.RemoveFloatingEntity(_newEntity.Id);
        switch (assetMetadata.assetType)
        {
            case AssetMetadata.AssetType.Model:
                _commandSystem.ExecuteCommand(_commandSystem.CommandFactory.CreateAddModelAssetToScene(_newEntity.Id, _newEntity.CustomName, assetMetadata.assetId, mousePositionInScene));
                break;
            case AssetMetadata.AssetType.Image:
                break;
            default:
                break;
        }
    }
}
