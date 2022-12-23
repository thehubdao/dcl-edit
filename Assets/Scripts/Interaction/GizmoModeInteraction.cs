using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Interaction
{
    public class GizmoModeInteraction : MonoBehaviour
    {
        private GizmoState _gizmoState;
        
        [Inject]
        private void Construct(GizmoState gizmoState)
        {
            _gizmoState = gizmoState;
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
