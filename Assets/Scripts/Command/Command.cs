using System.Collections.Generic;
using Assets.Scripts.EditorState;

namespace Assets.Scripts.Command
{
    public abstract class Command
    {
        internal abstract void Do(SceneState sceneState);
        internal abstract void Undo(SceneState sceneState);
        
        private static List<Command> _commandHistory = new List<Command>();
        private static int _currentCommandIndex = -1;

        public static void ExecuteCommand<T>(T command) where T : Command
        {
            _commandHistory.Add(command);
            _currentCommandIndex = _commandHistory.Count - 1;
            
            command.Do(EditorStates.CurrentSceneState);
        }

        public static void UndoCommand()
        {
            if (_currentCommandIndex >= 0)
            {
                _commandHistory[_currentCommandIndex].Undo(EditorStates.CurrentSceneState);
                _currentCommandIndex--;
            }
        }
        
        public static void RedoCommand()
        {
            if (_currentCommandIndex < _commandHistory.Count - 1)
            {
                _currentCommandIndex++;
                _commandHistory[_currentCommandIndex].Do(EditorStates.CurrentSceneState);
            }
        }
    }
}