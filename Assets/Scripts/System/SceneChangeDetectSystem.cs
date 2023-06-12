using Assets.Scripts.Command;
using Assets.Scripts.SceneState;
using Zenject;

namespace Assets.Scripts.System
{
    /// <summary>
    /// Responsible for detecting if the current state the scene matches the state when the scene was last saved.
    /// </summary>
    public class SceneChangeDetectSystem : ISceneChangeDetectSystem
    {
        // Dependencies
        private ISceneViewSystem sceneViewSystem;

        // Commands are referenced in the command history by an index. To remember which command was the last one when the scene
        // is saved, the offset to the index of that command is remembered.
        int savedCommandIndexOffset = 0;

        public enum CommandEvent
        {
            ExecuteNew,
            OverwriteHistory,
            Undo,
            Redo
        }

        [Inject]
        public void Construct(ISceneViewSystem sceneViewSystem)
        {
            this.sceneViewSystem = sceneViewSystem;
        }

        public void RememberCurrentState() => savedCommandIndexOffset = 0;

        public bool HasSceneChanged() => savedCommandIndexOffset != 0;

        /// <summary>
        /// Checks if the given command manipulated the scene. Commands like "ChangeSelection" do not do that.
        /// </summary>
        /// <param name="historyState"></param>
        /// <param name="commandIndex"></param>
        /// <returns></returns>
        private bool IsManipulatingCommand(CommandHistoryState historyState, int commandIndex)
        {
            if (commandIndex == -1) return false;
            return historyState.CommandHistory[commandIndex] is not ChangeSelection;
        }

        /// <summary>
        /// Reevaluate if the scene was changed when the last command was executed.
        /// </summary>
        public void Reevaluate(CommandHistoryState historyState, CommandEvent commandEvent)
        {
            // The saved command was lost because it was overwritten by executing another command.
            // Until the scene is saved again, it will remain at this state (int.MinValue).
            if (savedCommandIndexOffset == int.MinValue)
            {
                sceneViewSystem.UpdateSceneTabTitle();
                return;
            }

            // Command didn't change the scene, keep last changed state.
            switch (commandEvent)
            {
                case CommandEvent.ExecuteNew:
                    if (IsManipulatingCommand(historyState, historyState.CurrentCommandIndex))
                    {
                        savedCommandIndexOffset++;
                    }
                    break;
                case CommandEvent.OverwriteHistory:
                    // The saved command is somewhere in the future commands which are now lost.
                    if (savedCommandIndexOffset < 0)
                    {
                        savedCommandIndexOffset = int.MinValue;     // int.MinValue means that no command is saved
                    }
                    else
                    {
                        savedCommandIndexOffset++;
                    }
                    break;
                case CommandEvent.Undo:
                    if (IsManipulatingCommand(historyState, historyState.CurrentCommandIndex + 1)) savedCommandIndexOffset--;
                    break;
                case CommandEvent.Redo:
                    if (IsManipulatingCommand(historyState, historyState.CurrentCommandIndex)) savedCommandIndexOffset++;
                    break;
                default:
                    break;
            }

            sceneViewSystem.UpdateSceneTabTitle();
        }
    }
}