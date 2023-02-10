using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using UnityEngine;
using Zenject;

public class GizmoDebugTool : MonoBehaviour
{
    // Dependencies
    private GizmoState gizmoState;

    [Inject]
    private void Construct(GizmoState gizmoState)
    {
        this.gizmoState = gizmoState;
    }


    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(gizmoState.mouseContextCenter, gizmoState.mouseContextXVector);

        Gizmos.color = Color.green;
        Gizmos.DrawRay(gizmoState.mouseContextCenter, gizmoState.mouseContextYVector);
    }
}
