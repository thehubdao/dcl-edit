using Assets.Scripts.Events;
using UnityEngine.Events;
using Zenject;

namespace Assets.Scripts.EditorState
{
    public class GizmoState
    {
        // Dependencies
        private EditorEvents _editorEvents;

        public static UnityEvent onUpdate = new UnityEvent();

        [Inject]
        private void Construct(EditorEvents editorEvents)
        {
            _editorEvents = editorEvents;
        }

        public enum Mode
        {
            Translate,
            Rotate,
            Scale
        }

        private Mode _currentMode;

        public Mode CurrentMode
        {
            get => _currentMode;
            set
            {
                _currentMode = value;
                onUpdate.Invoke();
                _editorEvents.InvokeSelectionChangedEvent();
            }
        }
    }
}