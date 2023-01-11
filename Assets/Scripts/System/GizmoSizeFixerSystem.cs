using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class GizmoSizeFixerSystem : MonoBehaviour
    {
        // Dependencies
        private CameraState _cameraState;
        private SettingsSystem _settingsSystem;

        // this is used in addition to the gizmo scale setting, so a gizmo scale of 1.0 is reasonable.
        private const float scaleFactor = 0.2f;

        [Inject]
        private void Construct(CameraState cameraState, SettingsSystem settingsSystem)
        {
            _cameraState = cameraState;
            _settingsSystem = settingsSystem;
        }

        void LateUpdate()
        {
            // vector from camera to gizmo
            var diffVector = transform.position - _cameraState.Position;

            // camera forward vector
            var camForward = _cameraState.Forward;

            // project diffVector onto forward vector
            var proj = Vector3.Project(diffVector, camForward);

            // the length of the projected vector is the perceived distance to the gizmo
            var size = proj.magnitude;

            // apply gizmo scale setting
            size *= scaleFactor * _settingsSystem.gizmoSize.Get();

            // set the scale of the gizmo
            transform.localScale = new Vector3(size, size, size);
        }
    }
}
