using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;
using Cursor = UnityEngine.Cursor;

public class View3DInputSystem : MonoBehaviour
{
    //public Camera gizmoCamera;
    //public CameraController cameraController;

    //[SerializeField]
    //private float _cameraNormalFlySpeed;
    //
    //[SerializeField]
    //private float _cameraFastFlySpeed;
    //
    //[SerializeField]
    //private float _mouseSensitivity;



    private StateMachine _interfaceStateMachine;
    private StateMachine.State _freeMouseState;
    private StateMachine.State _cameraWasdMovingState;
    private StateMachine.State _cameraMouseZoomingState;
    private StateMachine.State _cameraRotateAroundState;
    private StateMachine.State _cameraSlideMovingState;
    //private StateMachine.State _holdingManipulatorState;


    private Vector3? _mouseInWorldPoint;

    void Start()
    {
        // This State is normally active, when the user just hovers the mouse over the 3D view
        _freeMouseState = SetupFreeMouseState();

        // This state is active, when the user is holding a Manipulator
        //_holdingManipulatorState = SetupHoldingManipulatorState();

        // This state is active, when the user moves around using the WASD controlls
        _cameraWasdMovingState = SetupCameraWasdMovingState();

        // This state is active, when the user zooms by holding alt + right mouse
        _cameraMouseZoomingState = SetupCameraMouseZoomingState();

        // This state is active, when the user rotates the camera around a point in the scene
        _cameraRotateAroundState = SetupCameraRotateAroundState();

        // This state is active, when the user slides the camera around (Middle mouse button)
        _cameraSlideMovingState = SetupCameraSlideMovingState();

        // Create the state machine starting with the free mouse state
        _interfaceStateMachine = new StateMachine(_freeMouseState);
    }


    private Interface3DHover _lastHoveredVisualIndicator = null;
    private StateMachine.State SetupFreeMouseState()
    {
        var freeMouseState = new StateMachine.State("Free mouse state");
        freeMouseState.OnStateExit = _ =>
        {
            if (_lastHoveredVisualIndicator != null)
                _lastHoveredVisualIndicator.EndHover();
            _lastHoveredVisualIndicator = null;
        };
        freeMouseState.OnStateUpdate = state =>
        {
            // Get the ray from the camera, where the mouse currently is
            var mouseRay = CameraManager.MainCamera.ViewportPointToRay(CameraManager.MainCamera.ScreenToViewportPoint(Input.mousePosition));

            // Figure out, if the mouse is over the Game window
            var isMouseOverGameWindow =
                !(0 > Input.mousePosition.x ||
                  0 > Input.mousePosition.y ||
                  Screen.width < Input.mousePosition.x ||
                  Screen.height < Input.mousePosition.y);

            // Figure out, if the mouse is over the 3D viewport (not hovering over any UI)
            var isMouseIn3DView = !EventSystem.current.IsPointerOverGameObject() && isMouseOverGameWindow;

            // Figuring out, what the mouse currently hovers
            Interface3DHover hoveredVisualIndicator = null;
            EntityManipulator hoveredManipulator = null;
            Entity hoveredEntity = null;
            _mouseInWorldPoint = null;

            // Do the following block only, if the mouse is over the 3D view
            if (isMouseIn3DView)
            {
                // First check, if mouse is hovering over Gizmo
                if (Physics.Raycast(mouseRay, out RaycastHit hitInfoGizmos, 10000, LayerMask.GetMask("Gizmos")))
                {
                    hoveredManipulator = hitInfoGizmos.transform.GetComponent<EntityManipulator>();
                    if (hitInfoGizmos.transform.gameObject.TryGetComponent(out Interface3DHover hoverIndicator))
                    {
                        hoveredVisualIndicator = hoverIndicator; // set the hover indicator if one is present
                    }

                    _mouseInWorldPoint = hitInfoGizmos.point;

                }
                // Secondly check, if mouse is hovering over Entity
                else if (Physics.Raycast(mouseRay, out RaycastHit hitInfoEntity, 10000, LayerMask.GetMask("Entity")))
                {
                    hoveredEntity = hitInfoEntity.transform.GetComponentInParent<Entity>();
                    if (hoveredEntity.gameObject.TryGetComponent(out Interface3DHover hoverIndicator))
                    {
                        hoveredVisualIndicator = hoverIndicator; // set the hover indicator if one is present
                    }

                    _mouseInWorldPoint = hitInfoEntity.point;
                }

            }

            if (_mouseInWorldPoint == null)
            {
                var groundPlane = new Plane(Vector3.up, Vector3.zero);
                groundPlane.Raycast(mouseRay, out float enter);
                if (enter > 2000 || enter < 0)
                {
                    _mouseInWorldPoint = mouseRay.GetPoint(10);
                }
                else
                {
                    _mouseInWorldPoint = mouseRay.GetPoint(enter);
                }
            }

            // Update hover indicator when necessary
            if (hoveredVisualIndicator != _lastHoveredVisualIndicator)
            {
                if (hoveredVisualIndicator != null)
                    hoveredVisualIndicator.StartHover();
                if (_lastHoveredVisualIndicator != null)
                    _lastHoveredVisualIndicator.EndHover();
                _lastHoveredVisualIndicator = hoveredVisualIndicator;
            }

            var pressingAlt = Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
            var pressingControl = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
            var pressingShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

            // When Left mouse button is clicked, select or deselect Entity
            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse) && !pressingAlt && isMouseIn3DView)
            {
                if (hoveredManipulator != null)
                {
                    // generate new holding manipulator state for the clicked Tool manipulator
                    _interfaceStateMachine.ActiveState = new HoldingManipulatorState(hoveredManipulator, CameraManager.MainCamera,
                        () => _interfaceStateMachine.ActiveState = _freeMouseState);
                }
                else// if (hoveredEntity != null)
                {
                    if (!pressingControl)
                    {
                        SceneManager.SetSelection(hoveredEntity);
                    }
                    else
                    {
                        SceneManager.AddSelection(hoveredEntity);
                    }
                }
            }



            // When pressing Right mouse button (without alt), switch to "Camera WASD moving state"
            if (Input.GetMouseButtonDown((int)MouseButton.RightMouse) && !pressingAlt && isMouseIn3DView)
            {
                _interfaceStateMachine.ActiveState = _cameraWasdMovingState;
            }

            // When pressing ald and Right mouse button, switch to "Camera WASD moving state"
            if (Input.GetMouseButtonDown((int)MouseButton.RightMouse) && pressingAlt && isMouseIn3DView)
            {
                _interfaceStateMachine.ActiveState = _cameraMouseZoomingState;
            }

            // When pressing ald and Right mouse button, switch to "Camera rotate around state"
            if (Input.GetMouseButtonDown((int)MouseButton.LeftMouse) && pressingAlt && isMouseIn3DView)
            {
                _interfaceStateMachine.ActiveState = _cameraRotateAroundState;
            }

            // When pressing Middle mouse button, switch to "Camera slide moving state"
            if (Input.GetMouseButtonDown((int)MouseButton.MiddleMouse) && isMouseIn3DView)
            {
                _interfaceStateMachine.ActiveState = _cameraSlideMovingState;
            }

            // When scrolling Mouse wheel, zoom in or out
            if (isMouseIn3DView)
            {
                var isSprinting = Input.GetButton("Sprint");
                //var distanceToMoveForward = Input.GetAxis("Mouse ScrollWheel") * (isSprinting ? _cameraFastFlySpeed : _cameraNormalFlySpeed);
                //
                //if (Mathf.Abs(distanceToMoveForward) > 0)
                //{
                //    CameraManager.Position += CameraManager.Forward * distanceToMoveForward;
                //}

                if (Mathf.Abs(Input.GetAxis("Mouse ScrollWheel")) > 0)
                {
                    CameraManager.MoveStep(new Vector3(0, 0, Input.GetAxis("Mouse ScrollWheel")), isSprinting);
                }

                //cameraController.ApplyZoom();
            }


            // Switch Manipulator by shortcut
            // E -> Translate
            if (Input.GetKeyDown(KeyCode.E) && isMouseOverGameWindow && !CanvasManager.IsAnyInputFieldFocused)
            {
                GizmoToolManager.CurrentTool = GizmoToolManager.Tool.Translate;
            }

            // R -> Rotate
            if (Input.GetKeyDown(KeyCode.R) && isMouseOverGameWindow && !CanvasManager.IsAnyInputFieldFocused)
            {
                GizmoToolManager.CurrentTool = GizmoToolManager.Tool.Rotate;
            }

            // T -> Scale
            if (Input.GetKeyDown(KeyCode.T) && isMouseOverGameWindow && !CanvasManager.IsAnyInputFieldFocused)
            {
                GizmoToolManager.CurrentTool = GizmoToolManager.Tool.Scale;
            }

            var ctrlPlusS = pressingControl && Input.GetKeyDown(KeyCode.S);
            if (ctrlPlusS && isMouseOverGameWindow)
            {
                SaveBackupSystem.BackupSave();
                ScriptGenerator.MakeScript();
                SceneSaveSystem.Save();
                AssetSaverSystem.Save();
            }

            if (pressingControl && Input.GetKeyDown(KeyCode.Z) && isMouseOverGameWindow &&
                !CanvasManager.IsAnyInputFieldFocused
#if UNITY_EDITOR
                && pressingShift
#endif
                )
            {
                UndoManager.Undo();
            }

            if (pressingControl && Input.GetKeyDown(KeyCode.Y) && isMouseOverGameWindow &&
                !CanvasManager.IsAnyInputFieldFocused
#if UNITY_EDITOR
                && pressingShift
#endif
                )
            {
                UndoManager.Redo();
            }

            // Duplicate Selected Entities
            if (pressingControl && Input.GetKeyDown(KeyCode.D) && isMouseOverGameWindow && !CanvasManager.IsAnyInputFieldFocused)
            {
                var newEntities = new List<Entity>();

                foreach (var selectedEntity in SceneManager.AllSelectedEntities)
                {
                    var newEntityObject = Instantiate(SceneManager.EntityTemplate, selectedEntity.transform.position, selectedEntity.transform.rotation, selectedEntity.transform.parent);
                    foreach (var component in selectedEntity.Components)
                    {
                        var newComponent = newEntityObject.AddComponent(component.GetType()) as EntityComponent;
                        newComponent.ApplySpecificJson(component.SpecificJson);
                    }


                    var newEntity = newEntityObject.GetComponent<Entity>();
                    newEntity.CustomName = selectedEntity.CustomName;
                    newEntity.WantsToBeExposed = false; // always sets exposed status to false to prevent duplicate names
                    newEntities.Add(newEntity);
                }

                var previouslySelected = SceneManager.AllSelectedEntities.ToList();

                SceneManager.SetSelectionRaw(null);
                foreach (var entity in newEntities)
                {
                    SceneManager.AddSelectedRaw(entity);
                }

                SceneManager.OnUpdateSelection.Invoke();
                SceneManager.OnUpdateHierarchy.Invoke();

                UndoManager.RecordUndoItem(
                    "Duplicate " + (previouslySelected.Count > 1 ? "Entities" : previouslySelected.First().ShownName),
                    () =>
                    {
                        foreach (var entity in newEntities)
                        {
                            TrashBinManager.DeleteEntity(entity);
                        }

                        SceneManager.SetSelectionRaw(null);
                        foreach (var entity in previouslySelected)
                        {
                            SceneManager.AddSelectedRaw(entity);
                        }

                        SceneManager.OnUpdateSelection.Invoke();
                        SceneManager.OnUpdateHierarchy.Invoke();

                    },
                    () =>
                    {
                        SceneManager.SetSelectionRaw(null);
                        foreach (var entity in newEntities)
                        {
                            TrashBinManager.RestoreEntity(entity);
                            SceneManager.AddSelectedRaw(entity);
                        }

                        SceneManager.OnUpdateSelection.Invoke();
                        SceneManager.OnUpdateHierarchy.Invoke();

                    });

            }


            // Delete the Selected Entity
            if (Input.GetKeyDown(KeyCode.Delete) && isMouseOverGameWindow && !CanvasManager.IsAnyInputFieldFocused)
            {

                var entities = SceneManager.AllSelectedEntities.ToList();

                foreach (var entity in entities)
                {
                    TrashBinManager.DeleteEntity(entity);
                }

                UndoManager.RecordUndoItem("Delete " + (entities.Count > 1 ? "Entities" : entities.First().ShownName),
                    () =>
                    {
                        SceneManager.SetSelectionRaw(null);
                        foreach (var entity in entities)
                        {
                            TrashBinManager.RestoreEntity(entity);
                            SceneManager.AddSelectedRaw(entity);
                        }

                    },
                    () =>
                    {
                        foreach (var entity in entities)
                        {
                            TrashBinManager.DeleteEntity(entity);
                        }

                        SceneManager.SetSelectionRaw(null);
                    });

                SceneManager.SetSelectionRaw(null);
            }


        };

        return freeMouseState;
    }

    private class HoldingManipulatorState : StateMachine.State
    {
        private Plane _activeManipulatorPlane;
        private Vector3? _lastMousePosition = null;

        private TransformUndo transformUndo;

        public HoldingManipulatorState(EntityManipulator activeManipulator, Camera gizmoCamera, Action returnToFreeMouseState) : base("Holding manipulator state")
        {
            OnStateEnter = state =>
            {
                transformUndo = new TransformUndo(SceneManager.AllSelectedEntities);

                transformUndo.SaveBeginningState();

                _activeManipulatorPlane = activeManipulator.GetPlane(gizmoCamera);

                // set null, to indicate that this state was just entered
                _lastMousePosition = null;
            };
            OnStateExit = state =>
            {
                transformUndo.SaveEndingState();

                transformUndo.AddUndoItem();

                GizmoRelationManager.onUpdate.Invoke();
            };
            OnStateUpdate = state =>
            {
                // Get the ray from the camera, where the mouse currently is
                var mouseRay = gizmoCamera.ViewportPointToRay(gizmoCamera.ScreenToViewportPoint(Input.mousePosition));

                // Get the 3D position on the Manipulator plane
                _activeManipulatorPlane.Raycast(mouseRay, out var distanceOnPlane);
                var mousePositionOnPlane = mouseRay.GetPoint(distanceOnPlane);

                //Debug.DrawLine(mousePositionOnPlane, _lastMousePosition.Value, Color.red, 10);
                _activeManipulatorPlane.DrawGizmo(activeManipulator.GetOneRay());

                // if this is the first update, where the manipulator is hold, there is no movement to apply
                if (_lastMousePosition != null)
                {
                    var globalMouseChange = mousePositionOnPlane - _lastMousePosition.Value;
                    var localMouseChange = activeManipulator.transform.InverseTransformDirection(globalMouseChange);
                    var cameraSpaceMouseChange = gizmoCamera.transform.InverseTransformDirection(globalMouseChange);
                    activeManipulator.Change(globalMouseChange, localMouseChange, cameraSpaceMouseChange, gizmoCamera);
                    //Debug.Log("Mouse change: "+globalMouseChange/Time.deltaTime);
                }

                // save the mouse position in a field. This is used to calculate the mouse movement in the next update
                _lastMousePosition = mousePositionOnPlane;

                // When the left mouse button is released, return to "Free mouse state"
                if (!Input.GetMouseButton((int)MouseButton.LeftMouse))
                {
                    returnToFreeMouseState.Invoke();
                    //_interfaceStateMachine.ActiveState = _freeMouseState;
                }

                SceneManager.OnSelectedEntityTransformChange.Invoke();
            };
        }
    }



    private StateMachine.State SetupHoldingManipulatorState()
    {
        var holdingManipulatorState = new StateMachine.State("Holding manipulator state");

        return holdingManipulatorState;
    }

    private StateMachine.State SetupCameraWasdMovingState()
    {
        var cameraWasdMovingState = new StateMachine.State("Camera WASD moving state");
        cameraWasdMovingState.OnStateEnter = _ => { Cursor.lockState = CursorLockMode.Locked; };
        cameraWasdMovingState.OnStateExit = _ => { Cursor.lockState = CursorLockMode.None; };
        cameraWasdMovingState.OnStateUpdate = _ =>
        {
            // Rotate camera with mouse movement
            CameraManager.RotateStep(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

            CameraManager.MoveContinuously(
                new Vector3(
                        Input.GetAxis("Horizontal"),
                        Input.GetAxis("UpDown"),
                        Input.GetAxis("Vertical")),
                Input.GetButton("Sprint"));

            // When releasing Right mouse button, switch to "Free mouse state"
            if (!Input.GetMouseButton((int)MouseButton.RightMouse))
            {
                _interfaceStateMachine.ActiveState = _freeMouseState;
            }
        };

        return cameraWasdMovingState;
    }

    private StateMachine.State SetupCameraMouseZoomingState()
    {
        var cameraMouseZoomingState = new StateMachine.State("Camera mouse zooming state");
        cameraMouseZoomingState.OnStateEnter = _ => { Cursor.lockState = CursorLockMode.Locked; };
        cameraMouseZoomingState.OnStateExit = _ => { Cursor.lockState = CursorLockMode.None; };
        cameraMouseZoomingState.OnStateUpdate = _ =>
        {
            //cameraController.UpdateZoomMovement();
            var isSprinting = Input.GetButton("Sprint");
            var targetMovement = Input.GetAxis("Mouse X") + Input.GetAxis("Mouse Y");
            //transform.Translate(0,0,(isSprinting ? sprintSpeed : speed) * targetMovement*0.03f);

            CameraManager.MoveStep(Vector3.forward * targetMovement * 0.03f, isSprinting);

            // When releasing Right mouse button, switch to "Free mouse state"
            if (!Input.GetMouseButton((int)MouseButton.RightMouse))
            {
                _interfaceStateMachine.ActiveState = _freeMouseState;
            }
        };

        return cameraMouseZoomingState;
    }

    private StateMachine.State SetupCameraRotateAroundState()
    {
        var cameraRotateAroundState = new StateMachine.State("Camera rotate around state");
        cameraRotateAroundState.OnStateEnter = _ => { Cursor.lockState = CursorLockMode.Locked; };
        cameraRotateAroundState.OnStateExit = _ => { Cursor.lockState = CursorLockMode.None; };
        cameraRotateAroundState.OnStateUpdate = _ =>
        {
            //if (_mouseInWorldPoint != null) cameraController.UpdateRotateAroundMovement(_mouseInWorldPoint.Value);

            if (_mouseInWorldPoint != null)
            {
                //transform.RotateAround(_mouseInWorldPoint.Value,Vector3.up, Input.GetAxis("Mouse X") * sensitivity);
                //transform.RotateAround(_mouseInWorldPoint.Value,transform.right, Input.GetAxis("Mouse Y") * -sensitivity);

                CameraManager.RotateAroundPointStep(_mouseInWorldPoint.Value, new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
            }


            // When releasing Left mouse button, switch to "Free mouse state"
            if (!Input.GetMouseButton((int)MouseButton.LeftMouse))
            {
                _interfaceStateMachine.ActiveState = _freeMouseState;
            }
        };

        return cameraRotateAroundState;
    }

    private StateMachine.State SetupCameraSlideMovingState()
    {
        var cameraSlideMovingState = new StateMachine.State("Camera slide moving state");
        cameraSlideMovingState.OnStateEnter = _ => { Cursor.lockState = CursorLockMode.Locked; };
        cameraSlideMovingState.OnStateExit = _ => { Cursor.lockState = CursorLockMode.None; };
        cameraSlideMovingState.OnStateUpdate = _ =>
        {
            //cameraController.UpdateSlideMovement();

            var isSprinting = Input.GetButton("Sprint");
            var targetMovement = new Vector3(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"), 0);
            //transform.Translate((isSprinting ? sprintSpeed : speed) * targetMovement*-0.03f);

            CameraManager.MoveStep(targetMovement * -0.03f, isSprinting);

            // When releasing Middle mouse button, switch to "Free mouse state"
            if (!Input.GetMouseButton((int)MouseButton.MiddleMouse))
            {
                _interfaceStateMachine.ActiveState = _freeMouseState;
            }
        };

        return cameraSlideMovingState;
    }


    void Update()
    {
        _interfaceStateMachine.Update();
    }
}

static class Util
{
    //public static Vector3 randomVector3()
    //{
    //
    //    return new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f));
    //}

    public static void DrawGizmo(this Plane p, Ray? startRay = null, int lines = 21, float spacing = 1)
    {
        startRay ??= new Ray(Vector3.zero, Vector3.up);

        var startPoint = startRay.Value.origin;

        // Put start point on plane
        var startPointOnPlane = p.ClosestPointOnPlane(startPoint);

        // Move start point to left border
        var horizontalRay = new Ray(startPointOnPlane, Vector3.Cross(p.normal, startRay.Value.direction));
        startPointOnPlane = horizontalRay.GetPoint(-(lines - 1) * spacing / 2);

        // Move start point to left bottom corner
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