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
                float time;
                float distance = (_cameraState.MovementDestination - transform.position).magnitude;
                if (_cameraState.IsMovingFast)
                {
                    time = distance / _cameraState.CameraFastFlySpeed;
                }
                else
                {
                    time = distance / _cameraState.CameraNormalFlySpeed;
                }
                LeanTween
                    .move(gameObject, _cameraState.MovementDestination, time)
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