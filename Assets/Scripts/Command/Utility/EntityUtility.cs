using Assets.Scripts.SceneState;
using System;
using System.Linq;

namespace Assets.Scripts.Command.Utility
{
    public static class EntityUtility
    {
        /// <summary>
        /// Add a new entity to the scene
        /// </summary>
        /// <param name="scene">The scene to add the entity to</param>
        /// <param name="id">The guid of the entity</param>
        /// <param name="name">The name of the entity</param>
        /// <param name="hierarchyOrder">The hierarchy order of the entity</param>
        /// <param name="parent">The parent of the entity</param>
        /// <returns>Reference to new entity</returns>
        public static DclEntity AddEntity(DclScene scene, Guid id, string name, float hierarchyOrder,
            Guid parent = default)
        {
            DclEntity entity = new DclEntity(id, name, parent, default, hierarchyOrder);
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
                //This is the case when primaryselectedentity == null and no secondaryselectedentities exist
                if (selectedEntity == null)
                {
                    return;
                }
                
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
            if (scene.SelectionState.PrimarySelectedEntity.Value == entity)
            {
                scene.SelectionState.PrimarySelectedEntity.Value = null;
            }
            
            scene.SelectionState.AllSelectedEntities.ToList().Remove(entity);
            scene.SelectionState.SecondarySelectedEntities.ToList().Remove(entity);
        }

        public static void AddDefaultTransformComponent(DclEntity entity)
        {
            var transformComponent = new DclTransformComponent();
            entity.AddComponent(transformComponent);
        }

        public static void AddComponent(DclEntity dclEntity, DclComponent.ComponentDefinition component)
        {
            dclEntity.AddComponent(new DclComponent(component));
        }
    }
}
