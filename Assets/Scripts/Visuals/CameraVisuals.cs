using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{

    public class CameraVisuals : MonoBehaviour
    {
        // Dependencies
        private CameraState _cameraState;

        [Inject]
        private void Construct(CameraState cameraState)
        {
            _cameraState = cameraState;
        }

        void Start()
        {
            _cameraState.OnCameraStateChanged.AddListener(SetDirty);

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