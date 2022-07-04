using Assets.Scripts.EditorState;
using UnityEngine;

namespace Assets.Scripts.Visuals
{

    public class CameraVisuals : MonoBehaviour
    {
        void Start()
        {
            EditorStates.CurrentCameraState.OnCameraStateChanged.AddListener(SetDirty);

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
            transform.position = EditorStates.CurrentCameraState.Position;
            transform.rotation = EditorStates.CurrentCameraState.Rotation;
        }
    }
}