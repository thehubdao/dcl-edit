using System;
using Assets.Scripts.Command.Utility;
using Assets.Scripts.EditorState;
using Assets.Scripts.State;
using JetBrains.Annotations;

namespace Assets.Scripts.Command
{
    public class AddEntity : Command
    {
        private string _name;
        [CanBeNull] private DclEntity _parent;
        private Guid _id;

        public AddEntity(string name = "", DclEntity parent = null)
        {
            _name = name;
            _parent = parent;
            _id = Guid.NewGuid();
        }

        internal override void Do(SceneState sceneState)
        {
            EntityUtility.AddEntity(sceneState.CurrentScene, _id, _name, _parent);
        }

        internal override void Undo(SceneState sceneState)
        {
            EntityUtility.DeleteEntity(sceneState.CurrentScene, _id);
        }
    }
}