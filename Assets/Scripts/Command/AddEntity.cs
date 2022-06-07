using System;
using Assets.Scripts.Command.Utility;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using JetBrains.Annotations;

namespace Assets.Scripts.Command
{
    public class AddEntity : SceneState.Command
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
        
        public override void Do(DclScene sceneState)
        {
            EntityUtility.AddEntity(sceneState, _id, _name, _parent);
        }

        public override void Undo(DclScene sceneState)
        {
            EntityUtility.DeleteEntity(sceneState, _id);
        }
    }
}