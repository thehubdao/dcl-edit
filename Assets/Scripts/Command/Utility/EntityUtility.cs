using System;
using Assets.Scripts.SceneState;

namespace Assets.Scripts.Command.Utility
{
    public class EntityUtility
    {
        public static void AddEntity(DclScene scene, Guid id, string name, DclEntity parent)
        {
            var entity = new DclEntity(scene, id, name, parent);
            scene.AllEntities.Add(id, entity);
        }

        public static void DeleteEntity(DclScene scene, Guid id)
        {
            scene.AllEntities.Remove(id);
        }
    }
}
