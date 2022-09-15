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

    // When dragging a translation-gizmo, this variable keeps track of the direction of the dragged gizmo.
    public Vector3? GizmoDragAxis = null;
    // When dragging a translation-gizmo, this variable stores the plane on which the mouse position is determined via raycasts.
    public Plane? GizmoDragMouseCollisionPlane = null;
    // When dragging a translation-gizmo, this variable keeps track of the initial mouse offset to the selected entity.
    public Vector3 GizmoDragMouseOffset = Vector3.zero;
    public Vector3 LocalGizmoAxis;
    // Mouse position at the start of the drag
    public Vector3 GizmoDragStartPos = Vector3.zero;
}
