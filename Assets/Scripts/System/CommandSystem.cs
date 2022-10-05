using Assets.Scripts.Events;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class CommandSystem : ICommandSystem
    {
        // Dependencies
        public CommandFactorySystem CommandFactory { get; private set; }
        private EditorState.SceneFile _sceneFile;
        private EditorEvents _editorEvents;

        [Inject]
        public void Construct(CommandFactorySystem commandFactory, EditorState.SceneFile sceneFile, EditorEvents editorEvents)
        {
            CommandFactory = commandFactory;
            _sceneFile = sceneFile;
            _editorEvents = editorEvents;
        }

        public void ExecuteCommand<T>(T command) where T : SceneState.Command
        {
            if (command == null)
                return;

            var commandState = _sceneFile.CurrentScene?.CommandHistoryState;

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

            command.Do(_sceneFile.CurrentScene, _editorEvents);
        }

        public void UndoCommand()
        {
            var commandState = _sceneFile.CurrentScene?.CommandHistoryState;

            if (commandState == null)
            {
                Debug.LogError("No Scene State found");
                return;
            }

            if (commandState.CurrentCommandIndex >= 0)
            {
                commandState.CommandHistory[commandState.CurrentCommandIndex].Undo(_sceneFile.CurrentScene, _editorEvents);
                commandState.CurrentCommandIndex--;
            }
        }

        public void RedoCommand()
        {
            var commandState = _sceneFile.CurrentScene?.CommandHistoryState;

            if (commandState == null)
            {
                Debug.LogError("No Scene State found");
                return;
            }

            if (commandState.CurrentCommandIndex < commandState.CommandHistory.Count - 1)
            {
                commandState.CurrentCommandIndex++;
                commandState.CommandHistory[commandState.CurrentCommandIndex].Do(_sceneFile.CurrentScene, _editorEvents);
            }
        }
    }
}