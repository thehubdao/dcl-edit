using UnityEngine;
using Zenject;

namespace Assets.Scripts.EditorState
{
    public class GizmoState
    {
        // Dependencies
        private SceneState _sceneState;

        [Inject]
        private void Construct(SceneState sceneState)
        {
            _sceneState = sceneState;
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
                _sceneState.CurrentScene?.SelectionState.SelectionChangedEvent.Invoke();
            }
        }
    }
}