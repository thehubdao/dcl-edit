using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{

    public class CameraVisuals : MonoBehaviour
    {
        // Dependencies
        private CameraState _cameraState;
        private EditorEvents _editorEvents;

        [Inject]
        private void Construct(CameraState cameraState, EditorEvents editorEvents)
        {
            _cameraState = cameraState;
            _editorEvents = editorEvents;
        }

        void Start()
        {
            _editorEvents.onCameraStateChangedEvent += SetDirty;

            SetDirty();
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
            transform.position = _cameraState.Position;
            transform.rotation = _cameraState.Rotation;
        }
    }
}