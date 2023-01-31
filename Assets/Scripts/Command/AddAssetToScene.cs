using Assets.Scripts.Command.Utility;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using UnityEngine;

namespace Assets.Scripts.Command
{
    public abstract class AddAssetToScene : SceneState.Command
    {
        protected Guid entityId;
        protected string entityCustomName;
        protected Guid assetId;
        protected Vector3 positionInScene;

        public AddAssetToScene(Guid entityId, string entityCustomName, Guid assetId, Vector3 positionInScene)
        {
            this.entityId = entityId;
            this.entityCustomName = entityCustomName;
            this.assetId = assetId;
            this.positionInScene = positionInScene;
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            EntityUtility.DeleteEntity(sceneState, entityId);
            editorEvents.InvokeHierarchyChangedEvent();
        }

        protected void AddTransformComponent(DclEntity entity)
        {
            var transformComponent = new DclTransformComponent(positionInScene);
            entity.AddComponent(transformComponent);
        }

        protected void SelectEntity(DclScene sceneState, EditorEvents editorEvents, DclEntity entity)
        {
            sceneState.SelectionState.SecondarySelectedEntities.Clear();
            sceneState.SelectionState.PrimarySelectedEntity = entity;
            editorEvents.InvokeSelectionChangedEvent();
        }
    }
}