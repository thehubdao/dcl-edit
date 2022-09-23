using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.System
{
    public interface ICommandSystem
    {
        void ExecuteCommand<T>(T command) where T : SceneState.Command;
        void UndoCommand();
        void RedoCommand();

        CommandFactorySystem CommandFactory { get; }
    }
}
