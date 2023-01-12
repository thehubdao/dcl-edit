using Assets.Scripts.SceneState;
using System;
using System.Linq;

namespace Assets.Scripts.Command.Utility
{
    public class EntityUtility
    {
        public static DclEntity AddEntity(DclScene scene, Guid id, string name, DclEntity parent = null)
        {
            DclEntity entity = new DclEntity(id, name, parent?.Id ?? Guid.Empty);

            scene.AddEntity(entity);

            return entity;
        }

        public static void ReAddEntity(DclScene scene, DclEntity entity, DclEntity parent)
        {
            entity.Parent = parent;
            scene.AddEntity(entity);
        }

        public static void DeleteEntity(DclScene scene, Guid id)
        {
            scene.RemoveEntity(id);
        }
    }
}