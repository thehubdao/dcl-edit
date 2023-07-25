using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{

    public class CameraVisuals : MonoBehaviour
    {
        // Dependencies
        private CameraState _cameraState;
        
        [Inject]
        private void Construct(CameraState cameraState, EditorEvents editorEvents)
        {
            _cameraState = cameraState;
        }

        private void OnEnable()
        {
            _cameraState.Position.OnValueChanged += SetDirty;
            _cameraState.AddOnPitchChanged(SetDirty);
            _cameraState.Yaw.OnValueChanged += SetDirty;
            SetDirty();
        }

        private void OnDisable()
        {
            _cameraState.Position.OnValueChanged -= SetDirty;
            _cameraState.RemoveOnPitchChanged(SetDirty);
            _cameraState.Yaw.OnValueChanged -= SetDirty;
        }
        private bool _isDirty = false;

        public void SetDirty()
        {
            _isDirty = true;
        }

        void LateUpdate()
        {
            if (_isDirty)
            {
                _isDirty = false;
                UpdateCameraTransform();
            }
        }

        private void UpdateCameraTransform()
        {
            transform.position = _cameraState.Position.Value;
            transform.rotation = _cameraState.Rotation;
        }
    }
}