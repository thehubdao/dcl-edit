using Assets.Scripts.Command.Utility;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using UnityEngine;

namespace Assets.Scripts.Command
{
    public class AddSceneAssetToScene : AddAssetToScene
    {
        public override string Name => "Add scene asset to scene";
        public override string Description => $"Drag and drop a scene asset from the asset browser into the scene: entity name: \"{entityCustomName}\", id: \"{entityId}\".";

        public AddSceneAssetToScene(Guid entityId, string entityCustomName, Guid assetId, Vector3 positionInScene, float hierarchyOrder) : base(entityId, entityCustomName, assetId, positionInScene, hierarchyOrder) { }

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            var newEntity = EntityUtility.AddEntity(sceneState, entityId, entityCustomName, hierarchyOrder);
            AddTransformComponent(newEntity);
            AddDclSceneComponent(newEntity);

            editorEvents.InvokeHierarchyChangedEvent();

            SelectEntity(sceneState, editorEvents, newEntity);
        }

        private void AddDclSceneComponent(DclEntity entity)
        {
            DclSceneComponent sceneComponent = new DclSceneComponent(assetId);
            entity.AddComponent(sceneComponent);
        }
    }
}