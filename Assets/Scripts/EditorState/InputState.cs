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
        FocusTransition,
        UiInput
    }

    public InStateType InState = InStateType.NoInput;

    // When Rotating camera around point, this variable keeps track of the point, that is being rotated around
    public Vector3? RotateCameraAroundPoint = null;

    // When the focus button was pressed, this variable keeps track of the position the camera transitions to
    public Vector3? FocusTransitionDestination = null;
}
