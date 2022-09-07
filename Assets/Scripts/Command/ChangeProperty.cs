using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.SceneState;
using Assets.Scripts.Utility;

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

        public override void Do(DclScene sceneState)
        {
            sceneState.GetPropertyFromIdentifier(_identifier).GetConcrete<T>().SetFixedValue(_newValue);
            sceneState.SelectionState.SelectionChangedEvent.Invoke();
        }

        public override void Undo(DclScene sceneState)
        {
            sceneState.GetPropertyFromIdentifier(_identifier).GetConcrete<T>().SetFixedValue(_oldValue);
            sceneState.SelectionState.SelectionChangedEvent.Invoke();
        }
    }
}
