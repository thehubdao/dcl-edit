using System.Collections.Generic;

namespace Assets.Scripts.EditorState
{
    public class SnackbarState
    {
        public enum MessageType
        {
            Debug,
            Info,
            Success,
            Warning,
            Error
        }

        public class Data
        {
            public string message;
            public MessageType type;
        }

        // These messages get picked up by the visuals and are removed from the queue when they are displayed
        public SubscribableQueue<Data> queuedMessages = new();
    }
}