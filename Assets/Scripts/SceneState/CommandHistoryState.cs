using System.Collections.Generic;

namespace Assets.Scripts.SceneState
{
    public class CommandHistoryState
    {
        public List<Command> CommandHistory = new List<Command>();
        public int CurrentCommandIndex = -1;
    }
}
