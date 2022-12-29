using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Interaction
{
    public class GizmoModeInteraction : MonoBehaviour
    {
        private GizmoState _gizmoState;

        public Button _translateButton;
        public Button _rotateButton;
        public Button _scaleButton;

        [Inject]
        private void Construct(GizmoState gizmoState)
        {
            _gizmoState = gizmoState;
        }

        public void Start()
        {
            GizmoState.onUpdate.AddListener(UpdateVisuals);
            UpdateVisuals();
        }

        void UpdateVisuals()
        {
            _translateButton.interactable = _gizmoState.CurrentMode != GizmoState.Mode.Translate;
            _rotateButton.interactable = _gizmoState.CurrentMode != GizmoState.Mode.Rotate;
            _scaleButton.interactable = _gizmoState.CurrentMode != GizmoState.Mode.Scale;
        }

        public void SetManipulatorTranslate()
        {
            _gizmoState.CurrentMode = GizmoState.Mode.Translate;
        }
        public void SetManipulatorRotate()
        {
            _gizmoState.CurrentMode = GizmoState.Mode.Rotate;
        }
        public void SetManipulatorScale()
        {
            _gizmoState.CurrentMode = GizmoState.Mode.Scale;
        }
    }
}
