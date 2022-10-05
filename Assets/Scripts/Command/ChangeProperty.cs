using Assets.Scripts.Events;
using Assets.Scripts.SceneState;

namespace Assets.Scripts.Command
{
    public class ChangeProperty<T> : SceneState.Command
    {
        public override string Name => $"Changing property value {_identifier.Component} -> {_identifier.Property}";

        public override string Description =>
            $"For the Entity {_identifier.Entity.ToString()}\n" +
            $"in the Component {_identifier.Component}\n" +
            $"the property {_identifier.Property}\n" +
            $"was changed from {_oldValue.ToString()} to {_newValue.ToString()}";

        private readonly DclPropertyIdentifier _identifier;
        private readonly T _oldValue, _newValue;

        public ChangeProperty(DclPropertyIdentifier identifier, T oldValue, T newValue)
        {
            _identifier = identifier;
            _oldValue = oldValue;
            _newValue = newValue;
        }

        public override void Do(DclScene sceneState, EditorEvents editorEvents)
        {
            sceneState.GetPropertyFromIdentifier(_identifier).GetConcrete<T>().SetFixedValue(_newValue);
            editorEvents.InvokeSelectionChangedEvent();
        }

        public override void Undo(DclScene sceneState, EditorEvents editorEvents)
        {
            sceneState.GetPropertyFromIdentifier(_identifier).GetConcrete<T>().SetFixedValue(_oldValue);
            editorEvents.InvokeSelectionChangedEvent();
        }
    }
}
