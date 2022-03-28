using System;
using Assets.Scripts.State;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Assets.Scripts.Command.Utility
{
    public class EntityUtility
    {
        public static void AddEntity(DclScene scene, Guid id, string name, DclEntity parent)
        {
            // Boilerplate code to create a new entity
            var entityObject = new GameObject(name == "" ? "Entity" : name);
            var entity = entityObject.AddComponent<DclEntity>();

            var entityChildrenObject = new GameObject("Children");
            var entityComponentsObject = new GameObject("Components");

            entityChildrenObject.transform.parent = entityObject.transform;
            entityComponentsObject.transform.parent = entityObject.transform;

            entity.ChildrenGameObject = entityChildrenObject;
            entity.ComponentsGameObject = entityComponentsObject;
            
            // Add the entity to the scene
            entity.transform.parent =
                parent == null ?
                    scene.EntityRootObject.transform :
                    parent.ChildrenGameObject.transform;

            // Setting up the entity
            entity.Id = id;
            entity.CustomName = name;
            entity.Parent = parent;
            entity.Scene = scene;
        }

        public static void DeleteEntity(DclScene scene, Guid id)
        {
            var entity = scene.GetEntityFormId(id);
            
            Object.Destroy(entity.gameObject);
        }
    }
}
