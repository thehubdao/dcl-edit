using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using UnityEngine;

public class GizmoSizeFixerSystem : MonoBehaviour
{
    
    // Update is called once per frame
    void Update()
    {
        var dist = Vector3.Distance(transform.position, EditorStates.CurrentCameraState.Position);

        var multiplier = 0.2f; // TODO: gizmo size settings

        dist *= multiplier;

        transform.localScale = new Vector3(dist,dist,dist);
    }
}
