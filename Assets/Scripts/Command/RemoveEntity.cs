using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Command.Utility;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Command
{
    public class RemoveEntity : SceneState.Command
    {
        private readonly DclEntity entity;
        private readonly List<DclEntity> secondarySelectedEntity;
        private readonly List<DclEntity> sortedEntities = new List<DclEntity>();
        private readonly List<DclEntity> sortedParents = new List<DclEntity>();
        private Guid primarySelectedEntityId;

        public RemoveEntity(DclEntity entity)
        {
            this.entity = entity;

            GetChildrenInOrder(entity);
            
            secondarySelectedEntity = entity.Scene.SelectionState.SecondarySelectedEntities;
            foreach (var secondaryEntity in secondarySelectedEntity)
            {
                GetChildrenInOrder(secondaryEntity);
            }
        }

        public override string Name => "Remove Entity";

        public override string Description =>
            $"Removing Entity \"{entity.ShownName}\" with id \"{entity.Id.Shortened()}\"" +
            (entity.Parent != null ? $" from Parent {entity.Parent.CustomName}" : "");

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            if (sceneState.SelectionState.PrimarySelectedEntity != null)
            {
                primarySelectedEntityId = sceneState.SelectionState.PrimarySelectedEntity.Id;
            }
            
            EntityUtility.DeleteEntity(sceneState, primarySelectedEntityId);
            foreach (var secondaryEntityToDelete in secondarySelectedEntity)
            {
                if (sceneState.GetEntityById(secondaryEntityToDelete.Id) != null)
                {
                    EntityUtility.DeleteEntity(sceneState, secondaryEntityToDelete.Id);
                }
            }

            editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            ReAddEntityAndChildren(sceneState);

            sceneState.SelectionState.PrimarySelectedEntity = sceneState.GetEntityById(primarySelectedEntityId);
            sceneState.SelectionState.SecondarySelectedEntities = secondarySelectedEntity;

            editorEvents.InvokeSelectionChangedEvent();
        }

        /// <summary>
        /// Re-adds all removed entities and attaches them to their parents.
        /// </summary>
        /// <param name="sceneState">Current state of the scene</param>
        private void ReAddEntityAndChildren(DclScene sceneState)
        {
            for (var i = sortedEntities.Count - 1; i >= 0; i--)
            {
                var entity = sortedEntities[i];
                var parent = sortedParents[i];
                EntityUtility.ReAddEntity(sceneState, entity, parent);
            }
        }

        /// <summary>
        /// Goes through the entity and it's children and saves the entity and parents inside of properties.
        /// </summary>
        /// <param name="entity">The root of the entity tree</param>
        private void GetChildrenInOrder(DclEntity entity)
        {
            if (sortedEntities.Contains(entity)) return; 
            
            Queue<DclEntity> queue = new Queue<DclEntity>();
            queue.Enqueue(entity);

            while (queue.Count > 0)
            {
                var node = queue.Dequeue();

                sortedEntities.Add(node);
                sortedParents.Add(node.Parent);
                
                foreach (var childNode in node.Children)
                {
                    if (!sortedEntities.Contains(childNode))
                    {
                        queue.Enqueue(childNode);
                    }
                }
            }
        }
    }
}
