using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputState : MonoBehaviour
{
    public enum InStateType
    {
        NoInput,
        WasdMovement,
        MouseZoom,
        RotateAroundPoint,
        SlideSideways,
        HoldingGizmoTool,
        FocusTransition
    }

    public InStateType InState = InStateType.NoInput;

    // When Rotating camera around point, this variable keeps track of the point, that is being rotated around
    public Vector3? RotateCameraAroundPoint = null;

    // When the focus button was pressed, this variable keeps track of the position the camera transitions to
    public Vector3? FocusTransitionDestination = null;

    // When dragging a move-gizmo, these variables keep track of the direction of the dragged gizmo
    // and the mouse position at the start of the drag.
    public Vector3? GizmoDragAxis = null;
    public Vector2? GizmoDragStartMousePos = null;
}
