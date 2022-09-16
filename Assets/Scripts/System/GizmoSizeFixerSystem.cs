using Assets.Scripts.EditorState;
using UnityEngine;

namespace Assets.Scripts.System
{
    public class GizmoSizeFixerSystem : MonoBehaviour
    {
        void LateUpdate()
        {
            var dist = Vector3.Distance(transform.position, EditorStates.CurrentCameraState.Position);

            var multiplier = 0.2f; // TODO: gizmo size settings

            dist *= multiplier;

            transform.localScale = new Vector3(dist, dist, dist);
        }
    }
}
