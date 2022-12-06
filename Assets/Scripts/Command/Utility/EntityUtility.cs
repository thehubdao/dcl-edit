using Assets.Scripts.SceneState;
using System;

namespace Assets.Scripts.Command.Utility
{
    public class EntityUtility
    {
        public static DclEntity AddEntity(DclScene scene, Guid id, string name, DclEntity parent = null)
        {
            var entity = new DclEntity(id, name, parent?.Id ?? Guid.Empty);

            scene.AddEntity(entity);

            return entity;
        }

        public static void DeleteEntity(DclScene scene, Guid id)
        {
            scene.RemoveEntity(id);
        }

        public static void AddDefaultTransformComponent(DclEntity entity)
        {
            var transformComponent = new DclTransformComponent();
            entity.AddComponent(transformComponent);
        }
    }
}
