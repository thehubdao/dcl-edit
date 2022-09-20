using UnityEngine;

namespace Assets.Scripts.EditorState
{
    public class GizmoState
    {
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
                EditorStates.CurrentSceneState.CurrentScene.SelectionState.SelectionChangedEvent.Invoke();
            }
        }
    }
}
