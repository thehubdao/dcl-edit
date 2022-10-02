using UnityEngine;
using Zenject;

namespace Assets.Scripts.EditorState
{
    public class GizmoState
    {
        // Dependencies
        private SceneFile _sceneFile;

        [Inject]
        private void Construct(SceneFile sceneFile)
        {
            _sceneFile = sceneFile;
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
                _sceneFile.CurrentScene?.SelectionState.SelectionChangedEvent.Invoke();
            }
        }
    }
}