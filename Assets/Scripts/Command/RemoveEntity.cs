using System.Collections.Generic;
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
            EntityUtility.DeleteEntity(sceneState, entity.Id);
            editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            ReAddEntityAndChildren(sceneState);
            editorEvents.InvokeSelectionChangedEvent();
        }

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

        private void GetChildrenInOrder(DclEntity parentEntity)
        {
            Queue<DclEntity> queue = new Queue<DclEntity>();
            queue.Enqueue(parentEntity);

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