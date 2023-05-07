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
        private SceneChangeDetectSystem sceneChangeDetectSystem;

        [Inject]
        public void Construct(
            CommandFactorySystem commandFactory,
            EditorEvents editorEvents,
            SceneManagerSystem sceneManagerSystem,
            MenuBarSystem menuBarSystem,
            SceneChangeDetectSystem sceneChangeDetectSystem)
        {
            CommandFactory = commandFactory;
            this.editorEvents = editorEvents;
            this.sceneManagerSystem = sceneManagerSystem;
            this.menuBarSystem = menuBarSystem;
            this.sceneChangeDetectSystem = sceneChangeDetectSystem;

            CreateMenuBarItems();
        }

        public void ExecuteCommand<T>(T command) where T : SceneState.Command
        {
            if (command == null)
                return;

            var commandState = sceneManagerSystem.GetCurrentSceneOrNull()?.CommandHistoryState;

            if (commandState == null)
            {
                Debug.LogError("No Scene State found");
                return;
            }

            bool historyOverwritten = false;
            if (commandState.CommandHistory.Count > commandState.CurrentCommandIndex + 1)
            {
                var firstDeprecatedCommandIndex = commandState.CurrentCommandIndex + 1;
                commandState.CommandHistory.RemoveRange(firstDeprecatedCommandIndex, commandState.CommandHistory.Count - firstDeprecatedCommandIndex);
                historyOverwritten = true;
            }

            commandState.CommandHistory.Add(command);
            commandState.CurrentCommandIndex = commandState.CommandHistory.Count - 1;

            command.Do(sceneManagerSystem.GetCurrentSceneOrNull(), editorEvents);

            if (historyOverwritten) sceneChangeDetectSystem.Reevaluate(commandState, SceneChangeDetectSystem.CommandEvent.OverwriteHistory);
            else sceneChangeDetectSystem.Reevaluate(commandState, SceneChangeDetectSystem.CommandEvent.ExecuteNew);
        }

        public void UndoCommand()
        {
            var commandState = sceneManagerSystem.GetCurrentSceneOrNull()?.CommandHistoryState;

            if (commandState == null)
            {
                Debug.LogError("No Scene State found");
                return;
            }

            if (commandState.CurrentCommandIndex >= 0)
            {
                commandState.CommandHistory[commandState.CurrentCommandIndex].Undo(sceneManagerSystem.GetCurrentSceneOrNull(), editorEvents);
                commandState.CurrentCommandIndex--;
                sceneChangeDetectSystem.Reevaluate(commandState, SceneChangeDetectSystem.CommandEvent.Undo);
            }
        }

        public void RedoCommand()
        {
            var commandState = sceneManagerSystem.GetCurrentSceneOrNull()?.CommandHistoryState;

            if (commandState == null)
            {
                Debug.LogError("No Scene State found");
                return;
            }

            if (commandState.CurrentCommandIndex < commandState.CommandHistory.Count - 1)
            {
                commandState.CurrentCommandIndex++;
                commandState.CommandHistory[commandState.CurrentCommandIndex].Do(sceneManagerSystem.GetCurrentSceneOrNull(), editorEvents);
                sceneChangeDetectSystem.Reevaluate(commandState, SceneChangeDetectSystem.CommandEvent.Redo);
            }
        }

        private void CreateMenuBarItems()
        {
            menuBarSystem.AddMenuItem("Edit#2/Undo#1", UndoCommand);
            menuBarSystem.AddMenuItem("Edit#2/Redo#2", RedoCommand);
        }
    }
}