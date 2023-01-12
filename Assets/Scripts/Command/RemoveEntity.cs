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
        private readonly List<List<DclEntity>> sortedEntities = new List<List<DclEntity>>();
        private readonly List<List<DclEntity>> sortedParents = new List<List<DclEntity>>();
        private Guid primarySelectedEntityId;
        private List<Guid> secondarySelectedEntityIds;

        public RemoveEntity(DclEntity entity)
        {
            this.entity = entity;
            GetChildrenInOrder(entity);
        }

        public override string Name => "Remove Entity";

        public override string Description =>
            $"Removing Entity \"{entity.ShownName}\" with id \"{entity.Id.Shortened()}\"" +
            (entity.Parent != null ? $" from Parent {entity.Parent.CustomName}" : "");

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            primarySelectedEntityId = sceneState.SelectionState.PrimarySelectedEntity.Id;
            secondarySelectedEntityIds = sceneState.SelectionState.SecondarySelectedEntities.Select(entity => entity.Id).ToList();
            
            EntityUtility.DeleteEntity(sceneState, entity.Id);
            editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            ReAddEntityAndChildren(sceneState);
            
            sceneState.SelectionState.PrimarySelectedEntity = sceneState.GetEntityById(primarySelectedEntityId);
            sceneState.SelectionState.SecondarySelectedEntities =
                secondarySelectedEntityIds.Select(sceneState.GetEntityById).ToList(); 
            
            editorEvents.InvokeSelectionChangedEvent();
        }

        /// <summary>
        /// Goes through entity and parent hierarchies layer by layer, re-adds each entity and connects each to its previous parent.
        /// </summary>
        /// <param name="sceneState">Current state of the scene</param>
        private void ReAddEntityAndChildren(DclScene sceneState)
        {
            for (var i = sortedEntities.Count - 1; i >= 0; i--)
            {
                var layer = sortedEntities[i];
                var parentLayer = sortedParents[i];

                for (var index = 0; index < layer.Count; index++)
                {
                    var entity = layer[index];
                    var parent = parentLayer[index];
                    
                    EntityUtility.ReAddEntity(sceneState, entity, parent);
                }
            }
        }

        /// <summary>
        /// Goes through the entity and it's children in breadth-first manner and saves the entity structure and the 'parent of each entity' structure inside of properties.
        /// </summary>
        /// <param name="entity">The root of the entity tree</param>
        private void GetChildrenInOrder(DclEntity entity)
        {
            Queue<DclEntity> queue = new Queue<DclEntity>();
            queue.Enqueue(entity);

            while (queue.Count > 0)
            {
                var elementsInLevel = queue.Count;
                
                List<DclEntity> layer = new List<DclEntity>();
                List<DclEntity> parentLayer = new List<DclEntity>();
                    
                while (elementsInLevel > 0)
                {
                    elementsInLevel--;
                    
                    var node = queue.Dequeue();
                    
                    layer.Add(node);
                    parentLayer.Add(node.Parent);
                    
                    foreach (var childNode in node.Children)
                    {
                        queue.Enqueue(childNode);
                    }
                }

                sortedEntities.Add(layer);
                sortedParents.Add(parentLayer);
            }
        }
    }
}