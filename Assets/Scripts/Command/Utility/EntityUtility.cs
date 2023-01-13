using Assets.Scripts.SceneState;
using System;

namespace Assets.Scripts.Command.Utility
{
    public class EntityUtility
    {
        public static DclEntity AddEntity(DclScene scene, Guid id, string name, Guid parent = default)
        {
            var entity = new DclEntity(id, name, parent);

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
