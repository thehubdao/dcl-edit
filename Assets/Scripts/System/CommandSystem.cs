using Assets.Scripts.Events;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class CommandSystem : ICommandSystem
    {
        // Dependencies
        public CommandFactorySystem CommandFactory { get; private set; }
        private EditorEvents editorEvents;
        private SceneManagerSystem sceneManagerSystem;
        private MenuBarSystem menuBarSystem;

        [Inject]
        public void Construct(
            CommandFactorySystem commandFactory,
            EditorEvents editorEvents,
            SceneManagerSystem sceneManagerSystem,
            MenuBarSystem menuBarSystem)
        {
            CommandFactory = commandFactory;
            this.editorEvents = editorEvents;
            this.sceneManagerSystem = sceneManagerSystem;
            this.menuBarSystem = menuBarSystem;

            CreateMenuBarItems();
        }

        public void ExecuteCommand<T>(T command) where T : SceneState.Command
        {
            if (command == null)
                return;

            var commandState = sceneManagerSystem.GetCurrentScene()?.CommandHistoryState;

            if (commandState == null)
            {
                Debug.LogError("No Scene State found");
                return;
            }

            if (commandState.CommandHistory.Count > commandState.CurrentCommandIndex + 1)
            {
                var firstDeprecatedCommandIndex = commandState.CurrentCommandIndex + 1;
                commandState.CommandHistory.RemoveRange(firstDeprecatedCommandIndex, commandState.CommandHistory.Count - firstDeprecatedCommandIndex);
            }

            commandState.CommandHistory.Add(command);
            commandState.CurrentCommandIndex = commandState.CommandHistory.Count - 1;

            command.Do(sceneManagerSystem.GetCurrentScene(), editorEvents);
        }

        public void UndoCommand()
        {
            var commandState = sceneManagerSystem.GetCurrentScene()?.CommandHistoryState;

            if (commandState == null)
            {
                Debug.LogError("No Scene State found");
                return;
            }

            if (commandState.CurrentCommandIndex >= 0)
            {
                commandState.CommandHistory[commandState.CurrentCommandIndex].Undo(sceneManagerSystem.GetCurrentScene(), editorEvents);
                commandState.CurrentCommandIndex--;
            }
        }

        public void RedoCommand()
        {
            var commandState = sceneManagerSystem.GetCurrentScene()?.CommandHistoryState;

            if (commandState == null)
            {
                Debug.LogError("No Scene State found");
                return;
            }

            if (commandState.CurrentCommandIndex < commandState.CommandHistory.Count - 1)
            {
                commandState.CurrentCommandIndex++;
                commandState.CommandHistory[commandState.CurrentCommandIndex].Do(sceneManagerSystem.GetCurrentScene(), editorEvents);
            }
        }

        private void CreateMenuBarItems()
        {
            menuBarSystem.AddMenuItem("Edit#2/Undo#1", UndoCommand);
            menuBarSystem.AddMenuItem("Edit#2/Redo#2", RedoCommand);
        }
    }
}