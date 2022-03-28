using System;
using Assets.Scripts.Command.Utility;
using Assets.Scripts.State;
using JetBrains.Annotations;

namespace Assets.Scripts.Command
{
    public class AddEntity : Command
    {
        private string _name;
        private DclScene _scene;
        [CanBeNull] private DclEntity _parent;
        private Guid _id;

        public AddEntity(DclScene scene, string name = "", DclEntity parent = null)
        {
            _scene = scene;
            _name = name;
            _parent = parent;
            _id = Guid.NewGuid();
        }

        internal override void Do()
        {
            EntityUtility.AddEntity(_scene, _id, _name, _parent);
        }

        internal override void Undo()
        {
            EntityUtility.DeleteEntity(_scene, _id);
        }
    }
}