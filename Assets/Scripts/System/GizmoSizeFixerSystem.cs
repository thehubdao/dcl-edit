using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class GizmoSizeFixerSystem : MonoBehaviour
    {
        // Dependencies
        private CameraState _cameraState;

        [Inject]
        private void Construct(CameraState cameraState)
        {
            _cameraState = cameraState;
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
            var dist = proj.magnitude;

            // some artificial multiplier to give the gizmo a nice size
            var multiplier = 0.2f; // TODO: gizmo size settings

            dist *= multiplier;

            // set the scale of the gizmo
            transform.localScale = new Vector3(dist, dist, dist);
        }
    }
}
