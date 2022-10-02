using System;
using Assets.Scripts.SceneState;

namespace Assets.Scripts.Command.Utility
{
    public class EntityUtility
    {
        public static void AddEntity(DclScene scene, Guid id, string name, DclEntity parent)
        {
            var entity = new DclEntity(id, name, parent.Id);
            scene.AddEntity(entity);
        }

        public static void DeleteEntity(DclScene scene, Guid id)
        {
            scene.RemoveEntity(id);
        }
    }
}
