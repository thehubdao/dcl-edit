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
            transform.rotation = _cameraState.Rotation;

            if (_cameraState.HasMovementDestination)
            {
                float time = 0.5f;
                float distance = (_cameraState.MovementDestination - transform.position).magnitude;
                
                LeanTween
                    .move(gameObject, _cameraState.MovementDestination, time)
                    .setEaseInCubic()
                    .setOnComplete(() => _cameraState.Position = transform.position);
            }
            else
            {
                if (LeanTween.isTweening(gameObject))
                {
                    _cameraState.Position = transform.position;
                    LeanTween.cancel(gameObject);
                }
                else
                {
                    transform.position = _cameraState.Position;
                }
            }
        }
    }
}