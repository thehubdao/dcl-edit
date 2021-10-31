using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class Interface3DManager : MonoBehaviour
{
    public Camera gizmoCamera;
    public RightClickCameraController cameraController;

    //private Material _lastHovered = null;
    private Interface3DHover _lastHoveredVisualIndicator = null;

    private EntityManipulator _activeManipulator;
    private Vector3? _lastMousePosition = null;
    private Plane _activeManipulatorPlane;


    private StateMachine _interfaceStateMachine;
    private StateMachine.State _freeMouseState;
    private StateMachine.State _cameraMovingState;
    private StateMachine.State _holdingManipulatorState;

    void Start()
    {
        // This State is normally active, when the user just hovers the mouse over the 3D view
        _freeMouseState = new StateMachine.State("Free mouse state");
        _freeMouseState.OnStateUpdate = state =>
        {
            // Get the ray from the camera, where the mouse currently is
            var mouseRay = gizmoCamera.ViewportPointToRay(gizmoCamera.ScreenToViewportPoint(Input.mousePosition));

            // Figuring out, what the mouse currently hovers
            Interface3DHover hoveredVisualIndicator = null;
            EntityManipulator hoveredManipulator = null;
            Entity hoveredEntity = null;

            // First check, if mouse is hovering over Gizmo
            if (!Input.GetMouseButton((int)MouseButton.RightMouse) && Physics.Raycast(mouseRay, out RaycastHit hitInfoGizmos, 10000, LayerMask.GetMask("Gizmos")))
            {
                hoveredManipulator = hitInfoGizmos.transform.GetComponent<EntityManipulator>();
                if (hitInfoGizmos.transform.gameObject.TryGetComponent(out Interface3DHover hoverIndicator))
                {
                    hoveredVisualIndicator = hoverIndicator; // set the hover indicator if one is present
                }
            }
            // Secondly check, if mouse is hovering over Entity
            else if (!Input.GetMouseButton((int)MouseButton.RightMouse) && Physics.Raycast(mouseRay, out RaycastHit hitInfoEntity, 10000, LayerMask.GetMask("Entity")))
            {
                hoveredEntity = hitInfoEntity.transform.GetComponentInParent<Entity>();
                if (hoveredEntity.gameObject.TryGetComponent(out Interface3DHover hoverIndicator))
                {
                    hoveredVisualIndicator = hoverIndicator; // set the hover indicator if one is present
                }
            }

            // Update hover indicator when necessary
            if (hoveredVisualIndicator != _lastHoveredVisualIndicator)
            {
                hoveredVisualIndicator?.StartHover();
                _lastHoveredVisualIndicator?.EndHover();
                _lastHoveredVisualIndicator = hoveredVisualIndicator;
            }

            // When Left mouse button is clicked, do necessary actions
            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse))
            {
                if (hoveredManipulator != null)
                {
                    _activeManipulator = hoveredManipulator;
                    _activeManipulatorPlane = _activeManipulator.GetPlane(gizmoCamera);
                    _interfaceStateMachine.ActiveState = _holdingManipulatorState; // Switching state to "holding manipulator state"
                }
                else //if (hoveredEntity != null)
                {
                    SceneManager.SelectedEntity = hoveredEntity;
                }
            }

            // When pressing Right mouse button, switch to "Camera moving state"
            if (Input.GetMouseButton((int) MouseButton.RightMouse))
            {
                _interfaceStateMachine.ActiveState = _cameraMovingState;
            }
        };

        // This state is active, when the user is holding a Manipulator
        _holdingManipulatorState = new StateMachine.State("Holding manipulator state");
        _holdingManipulatorState.OnStateEnter = state =>
        {
            // set null, to indicate that this state was just entered
            _lastMousePosition = null;
        };
        _holdingManipulatorState.OnStateUpdate = state =>
        {
            // Get the ray from the camera, where the mouse currently is
            var mouseRay = gizmoCamera.ViewportPointToRay(gizmoCamera.ScreenToViewportPoint(Input.mousePosition));
            
            // Get the 3D position on the Manipulator plane
            _activeManipulatorPlane.Raycast(mouseRay, out var distanceOnPlane);
            var mousePositionOnPlane = mouseRay.GetPoint(distanceOnPlane);
            
            //Debug.DrawLine(mousePositionOnPlane, _lastMousePosition.Value, Color.red, 10);
            _activeManipulatorPlane.DrawGizmo(_activeManipulator.GetOneRay());

            // if this is the first update, where the manipulator is hold, there is no movement to apply
            if (_lastMousePosition != null)
            {
                var globalMouseChange = mousePositionOnPlane - _lastMousePosition.Value;
                var localMouseChange = _activeManipulator.transform.InverseTransformDirection(globalMouseChange);
                var cameraSpaceMouseChange = gizmoCamera.transform.InverseTransformDirection(globalMouseChange);
                _activeManipulator.Change(globalMouseChange, localMouseChange, cameraSpaceMouseChange, gizmoCamera);
                //Debug.Log("Mouse change: "+globalMouseChange/Time.deltaTime);
            }
            
            // save the mouse position in a field. This is used to calculate the mouse movement in the next update
            _lastMousePosition = mousePositionOnPlane;

            // When the left mouse button is released, return to "Free mouse state"
            if (!Input.GetMouseButton((int) MouseButton.LeftMouse)) 
            {
                _interfaceStateMachine.ActiveState = _freeMouseState;
            }
        };

        // This state is active, when the user moves around
        _cameraMovingState = new StateMachine.State("Camera moving state");
        _cameraMovingState.OnStateEnter = _ => cameraController.StartMovement();
        _cameraMovingState.OnStateExit = _ => cameraController.EndMovement();
        _cameraMovingState.OnStateUpdate = _ =>
        {
            cameraController.UpdateMovement();

            // When releasing Right mouse button, switch to "Free mouse state"
            if (!Input.GetMouseButton((int) MouseButton.RightMouse))
            {
                _interfaceStateMachine.ActiveState = _freeMouseState;
            }
        };


        _interfaceStateMachine = new StateMachine(_freeMouseState);
    }

    void Update()
    {
        _interfaceStateMachine.Update();
    }
}

static class Util
{
    public static Vector3 randomVector3()
    {

        return new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
    }

    public static void DrawGizmo(this Plane p, Ray? startRay = null, int lines = 21, float spacing = 1)
    {
        startRay ??= new Ray(Vector3.zero, Vector3.up);

        var startPoint = startRay.Value.origin;

        // Put startpoint on plane
        var startPointOnPlane = p.ClosestPointOnPlane(startPoint);

        // Move startpoint to left border
        var horizontalRay = new Ray(startPointOnPlane, Vector3.Cross(p.normal, startRay.Value.direction));
        startPointOnPlane = horizontalRay.GetPoint(-(lines - 1) * spacing / 2);

        // Move startpoint to left bottom corner
        var verticalRay = new Ray(startPointOnPlane, Vector3.Cross(p.normal, horizontalRay.direction));

        // Draw Horizontal lines
        for (int i = 0; i < lines; i++)
        {
            var rayStartPoint = verticalRay.GetPoint((-(lines - 1) * spacing / 2) + i * spacing);
            Debug.DrawRay(rayStartPoint, horizontalRay.direction.normalized * (lines - 1) * spacing);
        }

        horizontalRay.origin = verticalRay.GetPoint(-(lines - 1) * spacing / 2);

        // Draw Vertical lines
        for (int i = 0; i < lines; i++)
        {
            var rayStartPoint = horizontalRay.GetPoint(/*(-(lines-1) * (-spacing/4))+*/i * spacing);
            Debug.DrawRay(rayStartPoint, verticalRay.direction.normalized * (lines - 1) * spacing);
        }

        //Debug.DrawRay(p.normal*-p.distance,Vector3.Cross(p.normal,Util.randomVector3()),Color.green,10);
    }
}