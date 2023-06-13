using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Zenject;

namespace Assets.Scripts.System
{
    public class SnackbarSystem
    {
        // Dependencies
        EditorEvents editorEvents;
        SnackbarState snackbarState;

        [Inject]
        public void Construct(EditorEvents editorEvents, SnackbarState snackbarState)
        {
            this.editorEvents = editorEvents;
            this.snackbarState = snackbarState;
        }

        public void AddMessage(string message, SnackbarState.MessageType type)
        {
            snackbarState.queuedMessages.Enqueue(new SnackbarState.Data { message = message, type = type });
            editorEvents.InvokeSnackbarChangedEvent();
        }
    }
}