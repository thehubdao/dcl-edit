using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Command;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using UnityEngine;

public class CommandSystem : MonoBehaviour
{
    public static void ExecuteCommand<T>(T command) where T : Command
    {
        var commandState = EditorStates.CurrentSceneState.CurrentScene?.CommandHistoryState;

        if (commandState == null)
        {
            Debug.LogError("No Scene State found");
            return;
        }

        commandState.CommandHistory.Add(command);
        commandState.CurrentCommandIndex = commandState.CommandHistory.Count - 1;

        command.Do(EditorStates.CurrentSceneState.CurrentScene);
    }

    public static void UndoCommand()
    {
        var commandState = EditorStates.CurrentSceneState.CurrentScene?.CommandHistoryState;

        if (commandState == null)
        {
            Debug.LogError("No Scene State found");
            return;
        }

        if (commandState.CurrentCommandIndex >= 0)
        {
            commandState.CommandHistory[commandState.CurrentCommandIndex].Undo(EditorStates.CurrentSceneState.CurrentScene);
            commandState.CurrentCommandIndex--;
        }
    }

    public static void RedoCommand()
    {
        var commandState = EditorStates.CurrentSceneState.CurrentScene?.CommandHistoryState;

        if (commandState == null)
        {
            Debug.LogError("No Scene State found");
            return;
        }

        if (commandState.CurrentCommandIndex < commandState.CommandHistory.Count - 1)
        {
            commandState.CurrentCommandIndex++;
            commandState.CommandHistory[commandState.CurrentCommandIndex].Do(EditorStates.CurrentSceneState.CurrentScene);
        }
    }
}
