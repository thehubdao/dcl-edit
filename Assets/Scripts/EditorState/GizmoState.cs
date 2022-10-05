using Assets.Scripts.Events;
using Zenject;

namespace Assets.Scripts.EditorState
{
    public class GizmoState
    {
        // Dependencies
        private SceneState _sceneState;
        private EditorEvents _editorEvents;

        [Inject]
        private void Construct(SceneState sceneState, EditorEvents editorEvents)
        {
            _sceneState = sceneState;
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