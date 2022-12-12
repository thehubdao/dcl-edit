using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.Interaction
{
    public class GeneralInputInteraction : MonoBehaviour
    {
        private InputSystemAsset _inputSystemAsset;

        // Dependencies
        private ISceneSaveSystem _sceneSaveSystem;
        private ICommandSystem _commandSystem;
        private InputState _inputState;
        private Interface3DState _interface3DState;
        private WorkspaceSaveSystem _workspaceSaveSystem;
        private GizmoState _gizmoState;
        private UnityState _unityState;
        private InputHelper _inputHelper;
        private EditorState.SceneDirectoryState _sceneDirectoryState;
        private CameraState _cameraState;
        private EditorEvents _editorEvents;
        private TypeScriptGenerationSystem _typeScriptGenerationSystem;
        private EntitySelectSystem _entitySelectSystem;
        private ContextMenuSystem _contextMenuSystem;

        [Inject]
        private void Construct(
            ISceneSaveSystem sceneSaveSystem,
            ICommandSystem commandSystem,
            InputState inputState,
            Interface3DState interface3DState,
            WorkspaceSaveSystem workspaceSaveSystem,
            EditorState.SceneDirectoryState sceneDirectoryState,
            CameraState cameraState,
            GizmoState gizmoState,
            UnityState unityState,
            InputHelper inputHelper,
            EditorEvents editorEvents,
            TypeScriptGenerationSystem typeScriptGenerationSystem,
            EntitySelectSystem entitySelectSystem,
            ContextMenuSystem contextMenuSystem)
        {
            _sceneSaveSystem = sceneSaveSystem;
            _commandSystem = commandSystem;
            _inputState = inputState;
            _interface3DState = interface3DState;
            _workspaceSaveSystem = workspaceSaveSystem;
            _gizmoState = gizmoState;
            _unityState = unityState;
            _inputHelper = inputHelper;
            _sceneDirectoryState = sceneDirectoryState;
            _cameraState = cameraState;
            _editorEvents = editorEvents;
            _typeScriptGenerationSystem = typeScriptGenerationSystem;
            _entitySelectSystem = entitySelectSystem;
            _contextMenuSystem = contextMenuSystem;
        }


        void Awake()
        {
            _inputSystemAsset = new InputSystemAsset();
            _inputSystemAsset.CameraMovement.Enable();
            _inputSystemAsset.Modifier.Enable();
            _inputSystemAsset.Hotkeys.Enable();
        }

        // Update is called once per frame
        void Update()
        {
            ProcessHotKeys();

            InvokeMouseButtonDownEvent();

            switch (_inputState.InState)
            {
                case InputState.InStateType.NoInput:
                    UpdateNoInput();
                    break;
                case InputState.InStateType.WasdMovement:
                    UpdateWasdMovement();
                    break;
                case InputState.InStateType.MouseZoom:
                    UpdateMouseZoom();
                    break;
                case InputState.InStateType.RotateAroundPoint:
                    UpdateRotateAroundPoint();
                    break;
                case InputState.InStateType.SlideSideways:
                    UpdateSlideSideways();
                    break;
                case InputState.InStateType.FocusTransition:
                    UpdateFocusTransition();
                    break;
                case InputState.InStateType.UiInput:
                    UpdateUiInput();
                    break;

                case InputState.InStateType.HoldingGizmoTool:
                    UpdateHoldingGizmoTool();
                    break;
                default:
                    _inputState.InState = InputState.InStateType.NoInput;
                    break;
            }
        }

        private void InvokeMouseButtonDownEvent()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _editorEvents.InvokeOnMouseButtonDownEvent(0);
            }
            if (Input.GetMouseButtonDown(1))
            {
                _editorEvents.InvokeOnMouseButtonDownEvent(1);
            }
        }

        private void UpdateNoInput()
        {
            var pressingAlt = _inputSystemAsset.Modifier.Alt.IsPressed();
            var pressingControl = _inputSystemAsset.Modifier.Ctrl.IsPressed();
            var pressingShift = _inputSystemAsset.Modifier.Shift.IsPressed();

            var isSprinting = _inputSystemAsset.CameraMovement.Fast.IsPressed();

            // Figure out where the mouse points at
            Ray mouseRay;
            bool isMouseOverGameWindow;
            bool isMouseOverContextMenu;
            {
                var mousePosViewport = _inputHelper.GetMousePositionInScenePanel();
                // Get the ray from the Camera, that corresponds to the mouse position in the panel
                mouseRay = _unityState.MainCamera.ViewportPointToRay(mousePosViewport);
                // Figure out, if the mouse is over the Game window
                isMouseOverGameWindow = mousePosViewport.x >= 0 &&
                                        mousePosViewport.x < 1 &&
                                        mousePosViewport.y >= 0 &&
                                        mousePosViewport.y < 1;
                isMouseOverContextMenu = _contextMenuSystem.IsMouseOverMenu();
            }


            


            // Figure out, if the mouse is over the 3D viewport (not hovering over any UI)
            var isMouseIn3DView = /*!EventSystem.current.IsPointerOverGameObject() &&*/ isMouseOverGameWindow && !isMouseOverContextMenu;

            // The position the mouse currently points to in the 3D viewport
            Vector3? mousePositionIn3DView = null;

            // Do the following block only, if the mouse is over the 3D view
            if (isMouseIn3DView)
            {
                // First check, if mouse is hovering over a Gizmo
                if (Physics.Raycast(mouseRay, out RaycastHit hitInfoGizmos, 10000, LayerMask.GetMask("Gizmos")))
                {
                    mousePositionIn3DView = hitInfoGizmos.point;
                    _interface3DState.CurrentlyHoveredObject = hitInfoGizmos.transform.gameObject;
                    _interface3DState.CurrentlyHoveredObjectType =
                        Interface3DState.HoveredObjectType.Gizmo;
                }
                // Secondly check, if mouse is hovering over a Entity
                else if (Physics.Raycast(mouseRay, out RaycastHit hitInfoEntity, 10000, LayerMask.GetMask("Entity Click"))) // mouse click layer
                {
                    mousePositionIn3DView = hitInfoEntity.point;
                    _interface3DState.CurrentlyHoveredObject = hitInfoEntity.transform.gameObject;
                    _interface3DState.CurrentlyHoveredObjectType =
                        Interface3DState.HoveredObjectType.Entity;
                }
                else
                {
                    _interface3DState.CurrentlyHoveredObject = null;
                    _interface3DState.CurrentlyHoveredObjectType =
                        Interface3DState.HoveredObjectType.None;
                }
            }
            else
            {
                _interface3DState.CurrentlyHoveredObject = null;
                _interface3DState.CurrentlyHoveredObjectType =
                    Interface3DState.HoveredObjectType.None;
            }


            // If the mouse is not over any Gizmo or Entity, then get the mouse position on the Ground plane
            if (mousePositionIn3DView == null)
            {
                var groundPlane = new Plane(Vector3.up, Vector3.zero);
                groundPlane.Raycast(mouseRay, out float enter);
                if (enter > 2000 || enter < 0)
                {
                    mousePositionIn3DView = mouseRay.GetPoint(10);
                }
                else
                {
                    mousePositionIn3DView = mouseRay.GetPoint(enter);
                }
            }

            // When pressing(down) Left mouse button, select the hovered entity
            if (_inputHelper.IsLeftMouseButtonDown() && !pressingAlt && isMouseIn3DView)
            {
                switch (_interface3DState.CurrentlyHoveredObjectType)
                {
                    case Interface3DState.HoveredObjectType.Gizmo:
                    {
                        _inputState.InState = InputState.InStateType.HoldingGizmoTool;

                        if (mousePositionIn3DView != null)
                        {
                            GizmoDirection gizmoDir = _interface3DState.CurrentlyHoveredObject.GetComponent<GizmoDirection>();
                            if (gizmoDir == null) break;
                            Vector3 localGizmoDir = gizmoDir.GetVector();

                            var entity = _sceneDirectoryState.CurrentScene?.SelectionState.PrimarySelectedEntity.GetTransformComponent();

                            GizmoState.Mode gizmoMode = _gizmoState.CurrentMode;

                            // Two special cases when in translation mode:
                            // Dragging on planes is handled differently from the rest of the gizmo operations.
                            if (gizmoMode == GizmoState.Mode.Translate)
                            {
                                // Dragging the center block of the translation gizmo (all three axis active in GizmoDirection component)
                                if (localGizmoDir == Vector3.one)
                                {
                                    Vector3 planeNormal = _cameraState.Position - entity.GlobalPosition;
                                    Plane p = new Plane(planeNormal, entity.GlobalPosition);

                                    // Calculate initial mouse offset to selected object
                                    Ray r = _unityState.MainCamera.ViewportPointToRay(_inputHelper.GetMousePositionInScenePanel());
                                    if (p.Raycast(r, out float enterDistance))
                                    {
                                        Vector3 hitPoint = r.GetPoint(enterDistance);
                                        Vector3 dirToHitPoint = hitPoint - entity.GlobalPosition;
                                        _inputState.CurrentGizmoData = new InputState.GizmoData(p, dirToHitPoint);
                                    }

                                    break;
                                }

                                // Dragging on a plane (XY plane, XZ plane or YZ plane)
                                if ((gizmoDir.x && gizmoDir.y || gizmoDir.x && gizmoDir.z || gizmoDir.y && gizmoDir.z))
                                {
                                    // Get the normal by inverting the direction vector. That way the one direction that is 0 becomes 1 which is the normal direction.
                                    Vector3 planeNormal = Vector3.one - localGizmoDir;

                                    // Transform normal into world space
                                    planeNormal = entity.TransformPoint(planeNormal) - entity.GlobalPosition;

                                    Plane p = new Plane(planeNormal, entity.GlobalPosition);

                                    // Calculate initial mouse offset to selected object
                                    Ray r = _unityState.MainCamera.ViewportPointToRay(_inputHelper.GetMousePositionInScenePanel());
                                    if (p.Raycast(r, out float enterDistance))
                                    {
                                        Vector3 hitPoint = r.GetPoint(enterDistance);
                                        Vector3 dirToHitPoint = hitPoint - entity.GlobalPosition;
                                        _inputState.CurrentGizmoData = new InputState.GizmoData(p, dirToHitPoint);
                                    }

                                    break;
                                }
                            }


                            // Along this axis the displacement of the mouse position is measured. It is also used to create a plane which handles
                            // the collision detection for the mouse position in 3d space while using gizmos.
                            Vector3 gizmoAxis;

                            if (gizmoMode == GizmoState.Mode.Rotate)
                            {
                                Vector3 localMousePos = entity.InverseTransformPoint((Vector3) mousePositionIn3DView);

                                // Clean up the local space mouse position
                                // Example: if we rotate the object around the x-axis we want a local space mouse position where
                                // the x-value is 0. That is because the gizmos have colliders so the 3d mouse position will
                                // probably be a bit offset from the actual gizmo position.
                                Vector3 invertedGizmoDir = Vector3.one - localGizmoDir; // Remove the rotation axis by setting it to 0
                                localMousePos = Vector3.Scale(localMousePos, invertedGizmoDir);

                                // Calculate the gizmo axis
                                // Gizmo axis must be perpendicular to both the direction to local mouse position and the gizmo direction.
                                // That way the gizmo axis matches the slope of the rotation gizmo at the clicked position. 
                                Vector3 localGizmoAxis = Vector3.Cross(localMousePos, localGizmoDir);

                                // Transform gizmo axis to world space
                                gizmoAxis = entity.TransformPoint(localMousePos + localGizmoAxis) - entity.TransformPoint(localMousePos);
                            }
                            else
                            {
                                gizmoAxis = (entity.TransformPoint(localGizmoDir) - entity.GlobalPosition).normalized;
                            }

                            // Then make sure that dirToCamera is orthogonal to gizmoAxis. This orthogonal dirToCamera vector
                            // now becomes the normal of the plane which does the mouse collision.
                            Vector3 dirToCamera = _cameraState.Position - entity.GlobalPosition;
                            Vector3.OrthoNormalize(ref gizmoAxis, ref dirToCamera);
                            Vector3 normal = dirToCamera;

                            // Create a plane on which the mouse position is determined via raycasts. Lies along gizmo axis.
                            Plane plane;
                            if (gizmoMode == GizmoState.Mode.Rotate)
                            {
                                plane = new Plane(normal, (Vector3) mousePositionIn3DView);
                            }
                            else
                            {
                                plane = new Plane(normal, entity.GlobalPosition);
                            }

                            // Calculate initial mouse offset to selected object
                            Ray ray = _unityState.MainCamera.ViewportPointToRay(_inputHelper.GetMousePositionInScenePanel());

                            if (plane.Raycast(ray, out float enter))
                            {
                                Vector3 hitPoint = ray.GetPoint(enter);
                                Vector3 dirToHitPoint = hitPoint - entity.GlobalPosition;

                                // Pass data about the current gizmo operation to the update method.
                                _inputState.CurrentGizmoData = new InputState.GizmoData(plane, dirToHitPoint, false, gizmoAxis.normalized, localGizmoDir);
                            }
                        }

                        break;
                    }
                    case Interface3DState.HoveredObjectType.Entity:
                    {
                        // select entity
                        var selectInteraction = _interface3DState.CurrentlyHoveredObject?
                            .GetComponentInParent<EntitySelectInteraction>();

                        if (selectInteraction != null)
                        {
                            selectInteraction.Select();
                        }

                        break;
                    }
                    case Interface3DState.HoveredObjectType.None:
                    {
                        _entitySelectSystem.DeselectAll();
                        break;
                    }
                    default: // ignore
                        break;
                }
            }

            // When pressing Right mouse button (without alt), switch to Camera WASD moving state
            if (_inputHelper.IsRightMouseButtonPressed() && !pressingAlt && isMouseIn3DView)
            {
                _inputState.InState = InputState.InStateType.WasdMovement;
                _inputHelper.HideMouse();
            }

            // When pressing alt and Right mouse button, switch to Mouse Zooming state
            if (_inputHelper.IsRightMouseButtonPressed() && pressingAlt && isMouseIn3DView)
            {
                _inputState.InState = InputState.InStateType.MouseZoom;
                _inputHelper.HideMouse();
            }

            // When pressing ald and Left mouse button, switch to Rotate around point state
            if (_inputHelper.IsLeftMouseButtonPressed() && pressingAlt && isMouseIn3DView)
            {
                _inputState.RotateCameraAroundPoint = mousePositionIn3DView;
                _inputState.InState = InputState.InStateType.RotateAroundPoint;
                _inputHelper.HideMouse();
            }

            // When pressing Middle mouse button, switch to "Camera slide moving state"
            if (_inputHelper.IsMiddleMouseButtonPressed() && isMouseIn3DView)
            {
                _inputState.InState = InputState.InStateType.SlideSideways;
                _inputHelper.HideMouse();
            }

            // When scrolling Mouse wheel, zoom in or out
            if (isMouseIn3DView)
            {
                var mouseScroll = Mouse.current.scroll.ReadValue().y / 1000;

                if (Mathf.Abs(mouseScroll) > 0)
                {
                    _cameraState.MoveStep(new Vector3(0, 0, mouseScroll), isSprinting);
                }
            }

            // When pressing the focus hotkey and having a selected primary entity, switch to Focus Transition state
            if (_inputSystemAsset.CameraMovement.Focus.triggered && _sceneDirectoryState.CurrentScene?.SelectionState.PrimarySelectedEntity != null)
            {
                // Fetch position of selected object
                var selectedEntity = _sceneDirectoryState.CurrentScene?.SelectionState.PrimarySelectedEntity;
                var entityPos = selectedEntity.GetTransformComponent().GlobalPosition;

                // Calculate an offset position so that the camera keeps its rotation and looks at the selected entity
                Vector3 cameraForward = _cameraState.Forward;
                Vector3 destination = entityPos - cameraForward * 10;
                _inputState.FocusTransitionDestination = destination;

                _inputState.InState = InputState.InStateType.FocusTransition;
            }

            // When pressing the save hotkey, save the scene and workspace layout
            if (_inputSystemAsset.Hotkeys.Save.triggered)
            {
                _sceneSaveSystem.Save(_sceneDirectoryState);
                _workspaceSaveSystem.Save(_unityState.dynamicPanelsCanvas);
                _typeScriptGenerationSystem.GenerateTypeScript();
            }

            // When pressing Translate hotkey, enter translation gizmo mode
            if (_inputSystemAsset.Hotkeys.Translate.triggered)
            {
                _gizmoState.CurrentMode = GizmoState.Mode.Translate;
            }

            // When pressing Rotate hotkey, enter rotation gizmo mode
            if (_inputSystemAsset.Hotkeys.Rotate.triggered)
            {
                _gizmoState.CurrentMode = GizmoState.Mode.Rotate;
            }

            // When pressing Scale hotkey, enter rotation gizmo mode
            if (_inputSystemAsset.Hotkeys.Scale.triggered)
            {
                _gizmoState.CurrentMode = GizmoState.Mode.Scale;
            }
        }

        private void UpdateWasdMovement()
        {
            var isSprinting = _inputSystemAsset.CameraMovement.Fast.IsPressed();

            // Rotate camera with mouse movement
            _cameraState.RotateStep(_inputHelper.GetMouseMovement());

            // Move camera with WASD keys. To go faster, hold shift
            _cameraState.MoveContinuously(
                new Vector3(
                    _inputSystemAsset.CameraMovement.LeftRight.ReadValue<float>(),
                    _inputSystemAsset.CameraMovement.UpDown.ReadValue<float>(),
                    _inputSystemAsset.CameraMovement.ForwardBackward.ReadValue<float>()),
                isSprinting);

            // When releasing Right mouse button, switch to "No input state"
            if (!_inputHelper.IsRightMouseButtonPressed())
            {
                _inputState.InState = InputState.InStateType.NoInput;
                _inputHelper.ShowMouse();
            }
        }

        private void UpdateMouseZoom()
        {
            var isSprinting = _inputSystemAsset.CameraMovement.Fast.IsPressed();
            var mouseMovement = _inputHelper.GetMouseMovement();
            var targetMovement = mouseMovement.x + mouseMovement.y;

            _cameraState.MoveStep(Vector3.forward * targetMovement * 0.03f, isSprinting);


            // When releasing Right mouse button, switch to "No input state"
            if (!_inputHelper.IsRightMouseButtonPressed())
            {
                _inputState.InState = InputState.InStateType.NoInput;
                _inputHelper.ShowMouse();
            }
        }

        private void UpdateRotateAroundPoint()
        {
            // Rotate camera around point with mouse movement
            if (_inputState.RotateCameraAroundPoint != null)
                _cameraState.RotateAroundPointStep(
                    _inputState.RotateCameraAroundPoint.Value,
                    _inputHelper.GetMouseMovement());

            // When releasing Left mouse button, switch to "No input state"
            if (!_inputHelper.IsLeftMouseButtonPressed())
            {
                _inputState.InState = InputState.InStateType.NoInput;
                _inputHelper.ShowMouse();
            }
        }

        private void UpdateSlideSideways()
        {
            var isSprinting = _inputSystemAsset.CameraMovement.Fast.IsPressed();
            // Move camera sideways with mouse movement
            var mouseMovement = _inputHelper.GetMouseMovement();
            var targetMovement = new Vector3(mouseMovement.x, mouseMovement.y, 0);
            _cameraState.MoveStep(targetMovement * -0.03f, isSprinting);

            // When releasing Middle mouse button, switch to "No input state"
            if (!_inputHelper.IsMiddleMouseButtonPressed())
            {
                _inputState.InState = InputState.InStateType.NoInput;
                _inputHelper.ShowMouse();
            }
        }

        private void UpdateFocusTransition()
        {
            // Control inputs are prioritized over the focus transition.
            UpdateNoInput();

            // Check if the state changed because of a user interaction.
            if (_inputState.InState != InputState.InStateType.FocusTransition)
            {
                return;
            }


            if (_cameraState.MoveTowards((Vector3)_inputState.FocusTransitionDestination, true))
            {
                _inputState.InState = InputState.InStateType.NoInput;
            }
        }

        private void UpdateUiInput()
        {
            // Prevent Inputs if currently typing in a text field
        }

        private void UpdateHoldingGizmoTool()
        {
            GizmoState.Mode mode = _gizmoState.CurrentMode;
            DclEntity selectedEntity = _sceneDirectoryState.CurrentScene?.SelectionState.PrimarySelectedEntity;
            DclTransformComponent trans = selectedEntity.GetTransformComponent();

            // When releasing LMB, stop holding gizmo
            if (!_inputHelper.IsLeftMouseButtonPressed())
            {
                _inputState.InState = InputState.InStateType.NoInput;
                _inputState.CurrentGizmoData = null;

                switch (mode)
                {
                    case GizmoState.Mode.Translate:
                        _commandSystem.ExecuteCommand(
                            _commandSystem.CommandFactory.CreateTranslateTransform(selectedEntity.Id, trans.Position.FixedValue, trans.Position.Value)
                        );
                        break;
                    case GizmoState.Mode.Rotate:
                        _commandSystem.ExecuteCommand(
                            _commandSystem.CommandFactory.CreateRotateTransform(selectedEntity.Id, trans.Rotation.FixedValue, trans.Rotation.Value)
                        );
                        break;
                    case GizmoState.Mode.Scale:
                        _commandSystem.ExecuteCommand(
                            _commandSystem.CommandFactory.CreateScaleTransform(selectedEntity.Id, trans.Scale.FixedValue, trans.Scale.Value)
                        );
                        break;
                }

                return;
            }

            if (_inputState.CurrentGizmoData != null)
            {
                InputState.GizmoData gizmoData = (InputState.GizmoData)_inputState.CurrentGizmoData;

                // Find mouse position in world on previously calculated plane
                Ray ray = _unityState.MainCamera.ViewportPointToRay(_inputHelper.GetMousePositionInScenePanel());
                if (gizmoData.plane.Raycast(ray, out float enter))
                {
                    // Ignore mouse positions that are too far away
                    if (enter >= _unityState.MainCamera.farClipPlane) return;

                    Vector3 hitPoint = ray.GetPoint(enter);
                    Vector3 dirToHitPoint = hitPoint - trans.GlobalPosition;

                    // Handle movement on planes separately:
                    if (gizmoData.movingOnPlane && _gizmoState.CurrentMode == GizmoState.Mode.Translate)
                    {
                        Vector3 globalPosition = gizmoData.plane.ClosestPointOnPlane(hitPoint - gizmoData.initialMouseOffset);
                        Vector3? localPosition = selectedEntity.Parent?.GetTransformComponent().InverseTransformPoint(globalPosition);
                        trans.Position.SetFloatingValue(localPosition ?? globalPosition);
                        _editorEvents.InvokeSelectionChangedEvent();
                        return;
                    }

                    // Project the dirToHitPoint onto gizmoAxis. This results in a "shadow" of the dirToHitPoint which lies
                    // on the gizmoAxis. Also factor in the mouse offset from the start of the drag to keep the object at the
                    // same position relative to the mouse cursor. This point is relative to the selected object.
                    Vector3 hitPointOnAxis = Vector3.Project(dirToHitPoint - gizmoData.initialMouseOffset, (Vector3)gizmoData.dragAxis);


                    switch (_gizmoState.CurrentMode)
                    {
                        case GizmoState.Mode.Translate:
                            Vector3 globalPosition = trans.GlobalPosition + hitPointOnAxis;
                            Vector3? localPosition = selectedEntity.Parent?.GetTransformComponent().InverseTransformPoint(globalPosition);
                            trans.Position.SetFloatingValue(localPosition ?? globalPosition);
                            break;
                        case GizmoState.Mode.Rotate:
                            // The distance along the gizmo axis at which the hit point lies.
                            // If the hit point on axis lies in the positive direction, the dot product returns 1. If it lies
                            // in the negative direction, the dot product returns -1. Therefore we can determine how far we pointed
                            // along the gizmo axis and in which direction.
                            float signedHitDistance = Vector3.Dot(hitPointOnAxis.normalized, (Vector3)gizmoData.dragAxis) * hitPointOnAxis.magnitude;

                            // Measure the radius of the rotation gizmo circle. As the initial mouse position is pretty close
                            // to the circle we can take that as a radius.
                            float radius = gizmoData.initialMouseOffset.magnitude;

                            // The distance moved by the mouse is the length of the arc. 
                            float arcLength = signedHitDistance;

                            // Calculate the angle of the arc. This is the amount that the object will be rotated by.
                            float angle = (arcLength * 360) / (2 * Mathf.PI * radius);

                            // Invert to rotate in the correct direction
                            angle *= -1;

                            Quaternion newRotation = trans.Rotation.FixedValue * Quaternion.Euler((Vector3)gizmoData.rotationAxis * angle);

                            trans.Rotation.SetFloatingValue(newRotation);
                            break;
                        case GizmoState.Mode.Scale:
                            Vector3 currentScale = trans.Scale.FixedValue;

                            // The point on the gizmo axis that is closest to the current mouse position. Transformed into local space.
                            Vector3 localHitPointOnAxis = Quaternion.Inverse(trans.GlobalRotation) * hitPointOnAxis;
                            Vector3 newScale = currentScale + Vector3.Scale(currentScale, localHitPointOnAxis);
                            trans.Scale.SetFloatingValue(newScale);
                            break;
                    }

                    _editorEvents.InvokeSelectionChangedEvent();
                }
            }
        }

        private void ProcessHotKeys()
        {
            if (_inputSystemAsset.Hotkeys.Undo.triggered)
            {
                _commandSystem.UndoCommand();
            }

            if (_inputSystemAsset.Hotkeys.Redo.triggered)
            {
                _commandSystem.RedoCommand();
            }
        }
    }
}