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

    // When holding a gizmo this struct contains information about current gizmo operation.
    public GizmoData? CurrentGizmoData;
    public struct GizmoData
    {
        // The direction in which the gizmo is dragged. In world space.
        public Vector3 dragAxis;
        // The plane on which the mouse position is determined via raycasts.
        public Plane plane;
        // The initial mouse offset to the selected entity at the start of the gizmo drag.
        public Vector3 initialMouseOffset;
        // The axis around which to rotate. Only relevant when using a rotation gizmo. In local space.
        public Vector3 rotationAxis;

        public GizmoData(Vector3 dragAxis, Plane mouseCollisionPlane, Vector3 initialMouseOffset, Vector3 rotationAxis)
        {
            this.dragAxis = dragAxis;
            this.plane = mouseCollisionPlane;
            this.initialMouseOffset = initialMouseOffset;
            this.rotationAxis = rotationAxis;
        }
    }
}
