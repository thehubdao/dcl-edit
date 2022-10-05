using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Zenject;

namespace Assets.Scripts.System
{
    public class UpdatePropertiesFromUiSystem
    {
        // dependencies
        private ICommandSystem _commandSystem;
        private ExposeEntitySystem _exposeEntitySystem;
        private EditorState.SceneState _sceneState;
        private EditorEvents _editorEvents;

        [Inject]
        public void Construct(ICommandSystem commandSystem, ExposeEntitySystem exposeEntitySystem, EditorState.SceneState sceneState, EditorEvents editorEvents)
        {
            _commandSystem = commandSystem;
            _exposeEntitySystem = exposeEntitySystem;
            _sceneState = sceneState;
            _editorEvents = editorEvents;
        }

        public void SetNewName(DclEntity entity, string newName)
        {
            _commandSystem.ExecuteCommand(_commandSystem.CommandFactory.CreateChangeEntityName(entity.Id, newName, entity.CustomName));
        }

        public void SetIsExposed(DclEntity entity, bool isExposed)
        {
            if (isExposed == entity.IsExposed)
                return;

            if (isExposed)
            {
                if (_exposeEntitySystem.IsEntityExposable(entity))
                {
                    _commandSystem.ExecuteCommand(_commandSystem.CommandFactory.CreateChangeIsExposed(entity.Id, true, entity.IsExposed));
                }
                else
                {
                    // TODO: show expose failed message
                    _editorEvents.InvokeSelectionChangedEvent();
                }
            }
            else
            {
                _commandSystem.ExecuteCommand(_commandSystem.CommandFactory.CreateChangeIsExposed(entity.Id, false, entity.IsExposed));
            }
        }

        public void UpdateFloatingProperty<T>(DclPropertyIdentifier property, T value)
        {
            var scene = _sceneState.CurrentScene;

            if (scene == null)
            {
                return;
            }

            scene.GetPropertyFromIdentifier(property).GetConcrete<T>().SetFloatingValue(value);

            _editorEvents.InvokeSelectionChangedEvent();
        }

        public void RevertFloatingProperty(DclPropertyIdentifier property)
        {
            var scene = _sceneState.CurrentScene;

            if (scene == null)
            {
                return;
            }

            scene.GetPropertyFromIdentifier(property).ResetFloating();
        }

        public void UpdateFixedProperty<T>(DclPropertyIdentifier property, T value)
        {
            var scene = _sceneState.CurrentScene;

            if (scene == null)
            {
                return;
            }

            var dclProperty = scene.GetPropertyFromIdentifier(property);
            var oldValue = dclProperty.GetConcrete<T>().FixedValue;

            if (oldValue.Equals(value))
            {
                dclProperty.ResetFloating();
                //Debug.Log("Value hasn't changed");
                return;
            }

            _commandSystem.ExecuteCommand(_commandSystem.CommandFactory.CreateChangePropertyCommand(property, oldValue, value));


            // TODO remove comments before merging
            //_commandSystem.ExecuteCommand(_commandFactory.MakeChangeProperty<T>(property, oldValue, value));
            //_commandSystem.ExecuteCommand(_commandSystem.Factory.CreateChangeProperty<T>(property, oldValue, value));
            //_commandSystem.ExecuteChangePropertyCommand<T>(property, oldValue, value);
        }
    }
}
