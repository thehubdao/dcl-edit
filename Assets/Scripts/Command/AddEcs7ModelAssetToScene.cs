using Assets.Scripts.Command.Utility;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using UnityEngine;

namespace Assets.Scripts.Command
{
    public class AddEcs7ModelAssetToScene : AddAssetToScene
    {
        public override string Name => "Add model asset to scene";
        public override string Description => $"Drag and drop a model asset from the asset browser into the scene: entity name: \"{entityCustomName}\", id: \"{entityId}\".";

        public AddEcs7ModelAssetToScene(Guid entityId, string entityCustomName, Guid assetId, Vector3 positionInScene, float hierarchyOrder) : base(entityId, entityCustomName, assetId, positionInScene, hierarchyOrder)
        {
        }

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            var newEntity = EntityUtility.AddEntity(sceneState, entityId, entityCustomName, hierarchyOrder, default);
            AddTransformComponent(newEntity);
            AddGltfContainerComponent(newEntity);

            editorEvents.InvokeHierarchyChangedEvent();

            SelectEntity(sceneState, editorEvents, newEntity);
        }

        private void AddGltfContainerComponent(DclEntity entity)
        {
            var gltfShape = new DclGltfContainerComponent(assetId);
            entity.AddComponent(gltfShape);
        }
    }
}
