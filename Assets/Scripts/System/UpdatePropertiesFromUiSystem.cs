using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Zenject;

namespace Assets.Scripts.System
{
    public class UpdatePropertiesFromUiSystem
    {
        // dependencies
        private ICommandSystem commandSystem;
        private ExposeEntitySystem exposeEntitySystem;
        private EditorEvents editorEvents;
        private SceneManagerSystem sceneManagerSystem;

        [Inject]
        public void Construct(ICommandSystem commandSystem,
            ExposeEntitySystem exposeEntitySystem,
            EditorEvents editorEvents,
            SceneManagerSystem sceneManagerSystem)
        {
            this.commandSystem = commandSystem;
            this.exposeEntitySystem = exposeEntitySystem;
            this.editorEvents = editorEvents;
            this.sceneManagerSystem = sceneManagerSystem;
        }

        public void SetNewName(DclEntity entity, string newName)
        {
            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangeEntityName(entity.Id, newName, entity.CustomName));
        }

        public void SetIsExposed(DclEntity entity, bool isExposed)
        {
            if (isExposed == entity.IsExposed)
                return;

            if (isExposed)
            {
                if (exposeEntitySystem.IsEntityExposable(entity))
                {
                    commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangeIsExposed(entity.Id, true, entity.IsExposed));
                }
                else
                {
                    // TODO: show expose failed message
                    editorEvents.InvokeSelectionChangedEvent();
                }
            }
            else
            {
                commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangeIsExposed(entity.Id, false, entity.IsExposed));
            }
        }


        public void UpdateFixedProperty<T>(DclPropertyIdentifier property, T value)
        {
            var scene = sceneManagerSystem.GetCurrentSceneOrNull();

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

            commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateChangePropertyCommand(property, oldValue, value));
        }
    }
}
