using Assets.Scripts.SceneState;
using System;
using System.Linq;

namespace Assets.Scripts.Command.Utility
{
    public class EntityUtility
    {
        /// <summary>
        /// Add a new entity to the scene
        /// </summary>
        /// <param name="scene">The scene to add the entity to</param>
        /// <param name="id">The guid of the entity</param>
        /// <param name="name">The name of the entity</param>
        /// <param name="parent">The parent of the entity</param>
        /// <returns>Reference to new entity</returns>
        public static DclEntity AddEntity(DclScene scene, Guid id, string name, DclEntity parent = null)
        {
            DclEntity entity = new DclEntity(id, name, parent?.Id ?? Guid.Empty);

            scene.AddEntity(entity);

            return entity;
        }

        /// <summary>
        /// Re-adds existing entities into the scene
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="entity"></param>
        /// <param name="parent"></param>
        public static void ReAddEntity(DclScene scene, DclEntity entity, DclEntity parent)
        {
            entity.Parent = parent;
            scene.AddEntity(entity);
            //scene.SelectionState.PrimarySelectedEntity = entity;
        }

        /// <summary>
        /// Removes an Entity from the Scene
        /// </summary>
        /// <param name="scene">The dcl scene from which to remove the entity</param>
        /// <param name="entityId">The id of the entity to be removed</param>
        public static void DeleteEntity(DclScene scene, Guid entityId)
        {
            RemoveSelectionOfEntityAndChildren(scene, entityId);
            scene.RemoveEntity(entityId);
        }

        /// <summary>
        /// Goes through the entity and its children and removes every existing selection
        /// </summary>
        /// <param name="scene">The dcl scene the entity is from</param>
        /// <param name="entityId">The id of the entity whose selection should be removed</param>
        private static void RemoveSelectionOfEntityAndChildren(DclScene scene, Guid entityId)
        {
            foreach (var selectedEntity in scene.SelectionState.AllSelectedEntities)
            {
                var entity = scene.GetEntityById(entityId);
                
                if (selectedEntity.Id == entityId)
                {
                    RemoveEntityFromSelectionLists(scene, entity);
                }
                
                foreach (var child in entity.Children)
                {
                    if (child == selectedEntity)
                    {
                        RemoveEntityFromSelectionLists(scene, child);
                    }
                }
            }
        }

        /// <summary>
        /// Removes an entity from all selection state lists
        /// </summary>
        /// <param name="scene"></param>
        /// <param name="entity"></param>
        private static void RemoveEntityFromSelectionLists(DclScene scene, DclEntity entity)
        {
            if (scene.SelectionState.PrimarySelectedEntity == entity)
            {
                scene.SelectionState.PrimarySelectedEntity = null;
            }
            
            scene.SelectionState.AllSelectedEntities.ToList().Remove(entity);
            scene.SelectionState.SecondarySelectedEntities.ToList().Remove(entity);
        }
    }
}