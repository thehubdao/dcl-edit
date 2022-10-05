using Assets.Scripts.Events;
using Zenject;

namespace Assets.Scripts.EditorState
{
    public class GizmoState
    {
        // Dependencies
        private SceneDirectoryState _sceneDirectoryState;
        private EditorEvents _editorEvents;

        [Inject]
        private void Construct(SceneDirectoryState sceneDirectoryState, EditorEvents editorEvents)
        {
            _sceneDirectoryState = sceneDirectoryState;
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
                _editorEvents.InvokeSelectionChangedEvent();
            }
        }
    }
}