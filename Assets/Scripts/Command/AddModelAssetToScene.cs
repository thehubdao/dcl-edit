using Assets.Scripts.Command.Utility;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using UnityEngine;

namespace Assets.Scripts.Command
{
    public class AddModelAssetToScene : SceneState.Command
    {
        public override string Name => "Add image asset to scene";
        public override string Description => $"Drag and drop a model asset from the asset manager into the scene: entity name: \"{_entityCustomName}\", id: \"{_entityId}\".";

        Guid _entityId;
        string _entityCustomName;
        Guid _assetId;
        Vector3 _positionInScene;

        public AddModelAssetToScene(Guid entityId, string entityCustomName, Guid assetId, Vector3 positionInScene)
        {
            _entityId = entityId;
            _entityCustomName = entityCustomName;
            _assetId = assetId;
            _positionInScene = positionInScene;
        }

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            var newEntity = EntityUtility.AddEntity(sceneState, _entityId, _entityCustomName);

            var transformComponent = new DclTransformComponent(_positionInScene);
            newEntity.AddComponent(transformComponent);

            // TODO the same commands are used in LoadSaveSystem. Maybe write a utility class for that?
            /*
            var newGltfShapeComponent = new DclComponent("GLTFShape", "Shape");
            newEntity.AddComponent(newGltfShapeComponent);
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<Guid>("asset", _assetId));
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<bool>("visible", true));
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<bool>("withCollisions", true));
            newGltfShapeComponent.Properties.Add(new DclComponent.DclComponentProperty<bool>("isPointerBlocker", true));
            */
            var gltfShape = new DclGltfShapeComponent(_assetId);
            newEntity.AddComponent(gltfShape);

            editorEvents.InvokeHierarchyChangedEvent();
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            EntityUtility.DeleteEntity(sceneState, _entityId);
            editorEvents.InvokeHierarchyChangedEvent();
        }
    }
}
