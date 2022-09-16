using Assets.Scripts.EditorState;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class GizmoSizeFixerSystem : MonoBehaviour
    {
        void LateUpdate()
        {
            // vector from camera to gizmo
        var diffVector = transform.position - EditorStates.CurrentCameraState.Position;

        // camera forward vector
        var camForward = EditorStates.CurrentCameraState.Forward;

        // project diffVector onto forward vector
        var proj = Vector3.Project(diffVector, camForward);

        // the length of the projected vector is the perceived distance to the gizmo
        var dist = proj.magnitude;

        // some artificial multiplier to give the gizmo a nice size
        var multiplier = 0.2f; // TODO: gizmo size settings

        dist *= multiplier;

        // set the scale of the gizmo
        transform.localScale = new Vector3(dist,dist,dist);
    }
}
