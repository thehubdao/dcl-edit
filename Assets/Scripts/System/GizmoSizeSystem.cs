using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;
using static Assets.Scripts.Utility.StaticUtilities;

namespace Assets.Scripts.System
{
    public class GizmoSizeSystem
    {
        // Dependencies
        private CameraState _cameraState;
        private SettingsSystem _settingsSystem;

        // this is used in addition to the gizmo scale setting, so a gizmo scale of 1.0 is reasonable.
        private const float scaleFactor = 0.1f;

        [Inject]
        private void Construct(CameraState cameraState, SettingsSystem settingsSystem)
        {
            _cameraState = cameraState;
            _settingsSystem = settingsSystem;
        }

        public float GetGizmoSize(Vector3 gizmoPosition)
        {
            // vector from camera to gizmo
            var diffVector = VectorFromTo(_cameraState.Position.Value, gizmoPosition);

            // camera forward vector
            var camForward = _cameraState.Forward;

            // project diffVector onto forward vector
            var proj = Vector3.Project(diffVector, camForward);

            // the length of the projected vector is the perceived distance to the gizmo
            var size = proj.magnitude;

            // apply gizmo scale setting
            size *= scaleFactor * _settingsSystem.gizmoSize.Get();

            return size;
        }
    }
}