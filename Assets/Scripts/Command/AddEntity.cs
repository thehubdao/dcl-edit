using Assets.Scripts.Command.Utility;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;
using JetBrains.Annotations;
using System;

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

        public override string Name => "Add Entity";
        public override string Description => $"Adding Entity \"{_name}\" with id \"{_id.Shortened()}\"" + (_parent != null ? $" as Child to {_parent.CustomName}" : "");

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            EntityUtility.AddEntity(sceneState, _id, _name, _parent);
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            EntityUtility.DeleteEntity(sceneState, _id);
        }
    }
}