using Assets.Scripts.Command.Utility;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Command
{
    public abstract class AddAssetToScene : SceneState.Command
    {
        protected Guid entityId;
        protected string entityCustomName;
        protected Guid assetId;
        protected Vector3 positionInScene;
        protected SelectionUtility.SelectionWrapper oldSelection;

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
            RestoreOldSelection(sceneState, editorEvents);
        }

        protected void AddTransformComponent(DclEntity entity)
        {
            var transformComponent = new DclTransformComponent(positionInScene);
            entity.AddComponent(transformComponent);
        }

        protected void SelectEntity(DclScene sceneState, EditorEvents editorEvents, DclEntity entity)
        {
            StoreOldSelection(sceneState);
            sceneState.SelectionState.SecondarySelectedEntities.Clear();
            sceneState.SelectionState.PrimarySelectedEntity = entity;
            editorEvents.InvokeSelectionChangedEvent();
        }

        private void StoreOldSelection(DclScene sceneState)
        {
            Guid primaryId = sceneState.SelectionState.PrimarySelectedEntity?.Id ?? Guid.Empty;
            List<Guid> secondaryIds = new List<Guid>();
            foreach (DclEntity entity in sceneState.SelectionState.SecondarySelectedEntities)
            {
                secondaryIds.Add(entity.Id);
            }
            oldSelection = new SelectionUtility.SelectionWrapper(primaryId, secondaryIds);
        }

        private void RestoreOldSelection(DclScene sceneState, EditorEvents editorEvents)
        {
            sceneState.SelectionState.PrimarySelectedEntity = sceneState.GetEntityById(oldSelection.Primary);
            foreach (Guid id in oldSelection.Secondary)
            {
                sceneState.SelectionState.SecondarySelectedEntities.Add(sceneState.GetEntityById(id));
            }
            editorEvents.InvokeSelectionChangedEvent();
        }
    }
}