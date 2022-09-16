using System;
using Assets.Scripts.Command;
using Assets.Scripts.EditorState;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using ICSharpCode.NRefactory.Visitors;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using Cursor = UnityEngine.Cursor;

namespace Assets.Scripts.Interaction
{
    public class GeneralInputInteraction : MonoBehaviour
    {
        private InputSystemAsset _inputSystemAsset;




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
            InputHelper.UpdateMouseDowns();

            ProcessHotKeys();

            switch (EditorStates.CurrentInputState.InState)
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
                case InputState.InStateType.HoldingGizmoTool:
                    UpdateHoldingGizmoTool();
                    break;
                default:
                    EditorStates.CurrentInputState.InState = InputState.InStateType.NoInput;
                    break;
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
            {
                var mousePosViewport = InputHelper.GetMousePositionInScenePanel();
                // Get the ray from the Camera, that corresponds to the mouse position in the panel
                mouseRay = EditorStates.CurrentUnityState.MainCamera.ViewportPointToRay(mousePosViewport);
                // Figure out, if the mouse is over the Game window
                isMouseOverGameWindow = mousePosViewport.x >= 0 &&
                                        mousePosViewport.x < 1 &&
                                        mousePosViewport.y >= 0 &&
                                        mousePosViewport.y < 1;
            }




            // Figure out, if the mouse is over the 3D viewport (not hovering over any UI)
            var isMouseIn3DView = /*!EventSystem.current.IsPointerOverGameObject() &&*/ isMouseOverGameWindow;

            // The position the mouse currently points to in the 3D viewport
            Vector3? mousePositionIn3DView = null;

            // Do the following block only, if the mouse is over the 3D view
            if (isMouseIn3DView)
            {
                // First check, if mouse is hovering over a Gizmo
                if (Physics.Raycast(mouseRay, out RaycastHit hitInfoGizmos, 10000, LayerMask.GetMask("Gizmos")))
                {
                    mousePositionIn3DView = hitInfoGizmos.point;
                    EditorStates.CurrentInterface3DState.CurrentlyHoveredObject = hitInfoGizmos.transform.gameObject;
                    EditorStates.CurrentInterface3DState.CurrentlyHoveredObjectType =
                        Interface3DState.HoveredObjectType.Gizmo;
                }
                // Secondly check, if mouse is hovering over a Entity
                else if (Physics.Raycast(mouseRay, out RaycastHit hitInfoEntity, 10000, LayerMask.GetMask("Entity Click"))) // mouse click layer
                {
                    mousePositionIn3DView = hitInfoEntity.point;
                    EditorStates.CurrentInterface3DState.CurrentlyHoveredObject = hitInfoEntity.transform.gameObject;
                    EditorStates.CurrentInterface3DState.CurrentlyHoveredObjectType =
                        Interface3DState.HoveredObjectType.Entity;
                }
                else
                {
                    EditorStates.CurrentInterface3DState.CurrentlyHoveredObject = null;
                    EditorStates.CurrentInterface3DState.CurrentlyHoveredObjectType =
                        Interface3DState.HoveredObjectType.None;
                }
            }
            else
            {
                EditorStates.CurrentInterface3DState.CurrentlyHoveredObject = null;
                EditorStates.CurrentInterface3DState.CurrentlyHoveredObjectType =
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
            if (InputHelper.IsLeftMouseButtonDown() && !pressingAlt && isMouseIn3DView)
            {
                switch (EditorStates.CurrentInterface3DState.CurrentlyHoveredObjectType)
                {
                    case Interface3DState.HoveredObjectType.Gizmo:
                        {
                            EditorStates.CurrentInputState.InState = InputState.InStateType.HoldingGizmoTool;

                            if (mousePositionIn3DView != null) {
                                GameObject gizmo = EditorStates.CurrentInterface3DState.CurrentlyHoveredObject;
                                if (gizmo.TryGetComponent(out GizmoDirection gizmoDir))
                                {
                                    SelectionState.GizmoMode currentMode = EditorStates.CurrentSceneState.CurrentScene.SelectionState.CurrentGizmoMode;
                                    var entity = EditorStates.CurrentSceneState.CurrentScene?.SelectionState.PrimarySelectedEntity.GetTransformComponent();
                                    Vector3 localGizmoDir = gizmoDir.GetVector();

                                    // Dragging the center block of the translation gizmo
                                    if(localGizmoDir == Vector3.one && currentMode == SelectionState.GizmoMode.Translate)
                                    {
                                        Vector3 planeNormal = EditorStates.CurrentCameraState.Position - entity.Position.Value;
                                        Plane p = new Plane(planeNormal, entity.Position.Value);

                                        // Calculate initial mouse offset to selected object
                                        Ray r = EditorStates.CurrentUnityState.MainCamera.ViewportPointToRay(InputHelper.GetMousePositionInScenePanel());
                                        if (p.Raycast(r, out float enterDistance))
                                        {
                                            Vector3 hitPoint = r.GetPoint(enterDistance);
                                            Vector3 dirToHitPoint = hitPoint - entity.GlobalPosition;
                                            EditorStates.CurrentInputState.CurrentGizmoData = new InputState.GizmoData(p, dirToHitPoint);
                                        }
                                    }
                                    // Check if dragging on a plane (XY plane, XZ plane or YZ plane)
                                    else if ((gizmoDir.x && gizmoDir.y || gizmoDir.x && gizmoDir.z || gizmoDir.y && gizmoDir.z) && currentMode == SelectionState.GizmoMode.Translate)
                                    {
                                        // Get the normal by inverting the direction vector. That way the one direction that is 0 becomes 1 which is the normal direction.
                                        Vector3 planeNormal = Vector3.one - localGizmoDir;

                                        // Transform normal into world space
                                        planeNormal = entity.TransformPoint(planeNormal) - entity.GlobalPosition;

                                        Plane p = new Plane(planeNormal, entity.GlobalPosition);

                                        // Calculate initial mouse offset to selected object
                                        Ray r = EditorStates.CurrentUnityState.MainCamera.ViewportPointToRay(InputHelper.GetMousePositionInScenePanel());
                                        if (p.Raycast(r, out float enterDistance))
                                        {
                                            Vector3 hitPoint = r.GetPoint(enterDistance);
                                            Vector3 dirToHitPoint = hitPoint - entity.GlobalPosition;
                                            EditorStates.CurrentInputState.CurrentGizmoData = new InputState.GizmoData(p, dirToHitPoint);
                                        }
                                    }
                                    else
                                    {
                                        // Along this axis the displacement of the mouse position is measured. It is also used to create a plane which handles
                                        // the collision detection for the mouse position in 3d space while using gizmos.
                                        Vector3 gizmoAxis;

                                        if (currentMode == SelectionState.GizmoMode.Rotate)
                                        {
                                            Vector3 localMousePos = entity.InverseTransformPoint((Vector3)mousePositionIn3DView);

                                            // Clean up the local space mouse position
                                            // Example: if we rotate the object around the x-axis we want a local space mouse position where
                                            // the x-value is 0. That is because the gizmos have colliders so the 3d mouse position will
                                            // probably be a bit offset from the actual gizmo position.
                                            Vector3 invertedGizmoDir = Vector3.one - localGizmoDir;        // Remove the rotation axis by setting it to 0
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
                                        Vector3 dirToCamera = EditorStates.CurrentCameraState.Position - entity.GlobalPosition;
                                        Vector3.OrthoNormalize(ref gizmoAxis, ref dirToCamera);
                                        Vector3 normal = dirToCamera;
                                        
                                        // Create a plane on which the mouse position is determined via raycasts. Lies along gizmo axis.
                                        Plane plane;
                                        if (currentMode == SelectionState.GizmoMode.Rotate)
                                        {
                                            plane = new Plane(normal, (Vector3)mousePositionIn3DView);
                                        }
                                        else
                                        {
                                            plane = new Plane(normal, entity.GlobalPosition);
                                        }

                                        // Calculate initial mouse offset to selected object
                                        Ray ray = EditorStates.CurrentUnityState.MainCamera.ViewportPointToRay(InputHelper.GetMousePositionInScenePanel());

                                        if (plane.Raycast(ray, out float enter))
                                        {
                                            Vector3 hitPoint = ray.GetPoint(enter);
                                            Vector3 dirToHitPoint = hitPoint - entity.GlobalPosition;

                                            // Pass data about the current gizmo operation to the update method.
                                            EditorStates.CurrentInputState.CurrentGizmoData = new InputState.GizmoData(plane, dirToHitPoint, false, gizmoAxis.normalized, localGizmoDir);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                    case Interface3DState.HoveredObjectType.Entity:
                        {
                            // select entity
                            var selectInteraction = EditorStates.CurrentInterface3DState.CurrentlyHoveredObject?
                                .GetComponentInParent<EntitySelectInteraction>();

                            if (selectInteraction != null)
                            {
                                if (pressingControl)
                                {
                                    selectInteraction.SelectAdditional(); // When pressing control, add entity to selection
                                }
                                else
                                {
                                    selectInteraction.SelectSingle(); // When not pressing control, set entity as single selection
                                }
                            }

                            break;
                        }
                    case Interface3DState.HoveredObjectType.None:
                        {
                            var scene = EditorStates.CurrentSceneState.CurrentScene;
                            var selectionCommand = new ChangeSelection(
                                ChangeSelection.GetPrimarySelectionFromScene(scene),
                                ChangeSelection.GetSecondarySelectionFromScene(scene),
                                Guid.Empty,
                                Array.Empty<Guid>());

                            CommandSystem.ExecuteCommand(selectionCommand);
                            break;
                        }
                    default: // ignore
                        break;
                }
            }

            // When pressing Right mouse button (without alt), switch to Camera WASD moving state
            if (InputHelper.IsRightMouseButtonPressed() && !pressingAlt && isMouseIn3DView)
            {
                EditorStates.CurrentInputState.InState = InputState.InStateType.WasdMovement;
                InputHelper.HideMouse();
            }

            // When pressing alt and Right mouse button, switch to Mouse Zooming state
            if (InputHelper.IsRightMouseButtonPressed() && pressingAlt && isMouseIn3DView)
            {
                EditorStates.CurrentInputState.InState = InputState.InStateType.MouseZoom;
                InputHelper.HideMouse();
            }

            // When pressing ald and Left mouse button, switch to Rotate around point state
            if (InputHelper.IsLeftMouseButtonPressed() && pressingAlt && isMouseIn3DView)
            {
                EditorStates.CurrentInputState.RotateCameraAroundPoint = mousePositionIn3DView;
                EditorStates.CurrentInputState.InState = InputState.InStateType.RotateAroundPoint;
                InputHelper.HideMouse();
            }

            // When pressing Middle mouse button, switch to "Camera slide moving state"
            if (InputHelper.IsMiddleMouseButtonPressed() && isMouseIn3DView)
            {
                EditorStates.CurrentInputState.InState = InputState.InStateType.SlideSideways;
                InputHelper.HideMouse();
            }

            // When scrolling Mouse wheel, zoom in or out
            if (isMouseIn3DView)
            {
                var mouseScroll = Mouse.current.scroll.ReadValue().y / 1000;

                if (Mathf.Abs(mouseScroll) > 0)
                {
                    EditorStates.CurrentCameraState.MoveStep(new Vector3(0, 0, mouseScroll), isSprinting);
                }
            }

            // When pressing the focus hotkey and having a selected primary entity, switch to Focus Transition state
            if(_inputSystemAsset.CameraMovement.Focus.triggered && EditorStates.CurrentSceneState.CurrentScene?.SelectionState.PrimarySelectedEntity != null)
            {
                // Fetch position of selected object
                var selectedEntity = EditorStates.CurrentSceneState.CurrentScene?.SelectionState.PrimarySelectedEntity;
                var entityPos = selectedEntity.GetTransformComponent().GlobalPosition;

                // Calculate an offset position so that the camera keeps its rotation and looks at the selected entity
                Vector3 cameraForward = EditorStates.CurrentCameraState.Forward;
                Vector3 destination = entityPos - cameraForward * 10;
                EditorStates.CurrentInputState.FocusTransitionDestination = destination;

                EditorStates.CurrentInputState.InState = InputState.InStateType.FocusTransition;
            }

            // When pressing the save hotkey, save the scene and workspace layout
            if (_inputSystemAsset.Hotkeys.Save.triggered)
            {
                SceneSaveSystem.Save(EditorStates.CurrentSceneState.CurrentScene);
                WorkspaceSaveSystem.Save(EditorStates.CurrentUnityState.dynamicPanelsCanvas);
            }

            // When pressing Translate hotkey, enter translation gizmo mode
            if (_inputSystemAsset.Hotkeys.Translate.triggered)
            {
                EditorStates.CurrentSceneState.CurrentScene.SelectionState.CurrentGizmoMode = SelectionState.GizmoMode.Translate;
            }

            // When pressing Rotate hotkey, enter rotation gizmo mode
            if (_inputSystemAsset.Hotkeys.Rotate.triggered)
            {
                EditorStates.CurrentSceneState.CurrentScene.SelectionState.CurrentGizmoMode = SelectionState.GizmoMode.Rotate;
            }

            // When pressing Scale hotkey, enter rotation gizmo mode
            if (_inputSystemAsset.Hotkeys.Scale.triggered)
            {
                EditorStates.CurrentSceneState.CurrentScene.SelectionState.CurrentGizmoMode = SelectionState.GizmoMode.Scale;
            }
        }

        private void UpdateWasdMovement()
        {
            var isSprinting = _inputSystemAsset.CameraMovement.Fast.IsPressed();

            // Rotate camera with mouse movement
            EditorStates.CurrentCameraState.RotateStep(InputHelper.GetMouseMovement());

            // Move camera with WASD keys. To go faster, hold shift
            EditorStates.CurrentCameraState.MoveContinuously(
                new Vector3(
                    _inputSystemAsset.CameraMovement.LeftRight.ReadValue<float>(),
                    _inputSystemAsset.CameraMovement.UpDown.ReadValue<float>(),
                    _inputSystemAsset.CameraMovement.ForwardBackward.ReadValue<float>()),
                    isSprinting);

            // When releasing Right mouse button, switch to "No input state"
            if (!InputHelper.IsRightMouseButtonPressed())
            {
                EditorStates.CurrentInputState.InState = InputState.InStateType.NoInput;
                InputHelper.ShowMouse();
            }
        }

        private void UpdateMouseZoom()
        {
            var isSprinting = _inputSystemAsset.CameraMovement.Fast.IsPressed();
            var mouseMovement = InputHelper.GetMouseMovement();
            var targetMovement = mouseMovement.x + mouseMovement.y;

            EditorStates.CurrentCameraState.MoveStep(Vector3.forward * targetMovement * 0.03f, isSprinting);


            // When releasing Right mouse button, switch to "No input state"
            if (!InputHelper.IsRightMouseButtonPressed())
            {
                EditorStates.CurrentInputState.InState = InputState.InStateType.NoInput;
                InputHelper.ShowMouse();
            }
        }

        private void UpdateRotateAroundPoint()
        {
            // Rotate camera around point with mouse movement
            if (EditorStates.CurrentInputState.RotateCameraAroundPoint != null)
                EditorStates.CurrentCameraState.RotateAroundPointStep(
                    EditorStates.CurrentInputState.RotateCameraAroundPoint.Value,
                    InputHelper.GetMouseMovement());

            // When releasing Left mouse button, switch to "No input state"
            if (!InputHelper.IsLeftMouseButtonPressed())
            {
                EditorStates.CurrentInputState.InState = InputState.InStateType.NoInput;
                InputHelper.ShowMouse();
            }
        }

        private void UpdateSlideSideways()
        {
            var isSprinting = _inputSystemAsset.CameraMovement.Fast.IsPressed();
            // Move camera sideways with mouse movement
            var mouseMovement = InputHelper.GetMouseMovement();
            var targetMovement = new Vector3(mouseMovement.x, mouseMovement.y, 0);
            EditorStates.CurrentCameraState.MoveStep(targetMovement * -0.03f, isSprinting);

            // When releasing Middle mouse button, switch to "No input state"
            if (!InputHelper.IsMiddleMouseButtonPressed())
            {
                EditorStates.CurrentInputState.InState = InputState.InStateType.NoInput;
                InputHelper.ShowMouse();
            }
        }

        private void UpdateFocusTransition()
        {
            // Control inputs are prioritized over the focus transition.
            UpdateNoInput();

            // Check if the state changed because of a user interaction.
            if(EditorStates.CurrentInputState.InState != InputState.InStateType.FocusTransition) { return; }

            
            if (EditorStates.CurrentCameraState.MoveTowards((Vector3)EditorStates.CurrentInputState.FocusTransitionDestination, true))
            {
                EditorStates.CurrentInputState.InState = InputState.InStateType.NoInput;
            }
        }

        private void UpdateHoldingGizmoTool()
        {
            SelectionState.GizmoMode mode = EditorStates.CurrentSceneState.CurrentScene.SelectionState.CurrentGizmoMode;
            DclEntity selectedEntity = EditorStates.CurrentSceneState.CurrentScene?.SelectionState.PrimarySelectedEntity;
            DclTransformComponent trans = selectedEntity.GetTransformComponent();


            // When releasing LMB, stop holding gizmo
            if (!InputHelper.IsLeftMouseButtonPressed())
            {
                EditorStates.CurrentInputState.InState = InputState.InStateType.NoInput;
                EditorStates.CurrentInputState.CurrentGizmoData = null;

                // TODO: Set final position via command
                switch (mode)
                {
                    case SelectionState.GizmoMode.Translate:
                        CommandSystem.ExecuteCommand(
                            new MoveTransform(selectedEntity.Id, trans.Position.FixedValue, trans.Position.Value)
                        );
                        break;
                    case SelectionState.GizmoMode.Rotate:
                        CommandSystem.ExecuteCommand(
                            new RotateTransform(selectedEntity.Id, trans.Rotation.FixedValue, trans.Rotation.Value)
                        );
                        break;
                    case SelectionState.GizmoMode.Scale:
                        CommandSystem.ExecuteCommand(
                            new ScaleTransform(selectedEntity.Id, trans.Scale.FixedValue, trans.Scale.Value)
                        );
                        break;
                }
                return;
            }                       

            if (EditorStates.CurrentInputState.CurrentGizmoData != null)
            {
                InputState.GizmoData gizmoData = (InputState.GizmoData)EditorStates.CurrentInputState.CurrentGizmoData;
                
                // Find mouse position in world on previously calculated plane
                Ray ray = EditorStates.CurrentUnityState.MainCamera.ViewportPointToRay(InputHelper.GetMousePositionInScenePanel());
                if (gizmoData.plane.Raycast(ray, out float enter))
                {
                    // Ignore mouse positions that are too far away
                    if(enter >= EditorStates.CurrentUnityState.MainCamera.farClipPlane) return;

                    Vector3 hitPoint = ray.GetPoint(enter);
                    Vector3 dirToHitPoint = hitPoint - trans.GlobalPosition;

                    // Moving on a plane?
                    if (gizmoData.movingOnPlane && EditorStates.CurrentSceneState.CurrentScene.SelectionState.CurrentGizmoMode == SelectionState.GizmoMode.Translate)
                    {
                        Vector3 globalPosition = gizmoData.plane.ClosestPointOnPlane(hitPoint - gizmoData.initialMouseOffset);
                        Vector3? localPosition = selectedEntity.Parent?.GetTransformComponent().InverseTransformPoint(globalPosition);
                        trans.Position.SetFloatingValue(localPosition ?? globalPosition);
                        EditorStates.CurrentSceneState.CurrentScene.SelectionState.SelectionChangedEvent.Invoke();
                        Debug.DrawLine(Vector3.zero, localPosition ?? globalPosition);
                        return;
                    }

                    // Project the dirToHitPoint onto gizmoAxis. This results in a "shadow" of the dirToHitPoint which lies
                    // on the gizmoAxis. Also factor in the mouse offset from the start of the drag to keep the object at the
                    // same position relative to the mouse cursor. This point is relative to the selected object.
                    Vector3 hitPointOnAxis = Vector3.Project(dirToHitPoint - gizmoData.initialMouseOffset, (Vector3)gizmoData.dragAxis);

                    switch (EditorStates.CurrentSceneState.CurrentScene.SelectionState.CurrentGizmoMode)
                    {
                        case SelectionState.GizmoMode.Translate:
                            Vector3 globalPosition = trans.GlobalPosition + hitPointOnAxis;
                            Vector3? localPosition = selectedEntity.Parent?.GetTransformComponent().InverseTransformPoint(globalPosition);
                            trans.Position.SetFloatingValue(localPosition ?? globalPosition);
                            break;
                        case SelectionState.GizmoMode.Rotate:
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
                        case SelectionState.GizmoMode.Scale:
                            Vector3 currentScale = trans.Scale.FixedValue;
                            
                            // The point on the gizmo axis that is closest to the current mouse position. Transformed into local space.
                            Vector3 localHitPointOnAxis = Quaternion.Inverse(trans.Rotation.FixedValue) * hitPointOnAxis;
                            Vector3 newScale = currentScale + Vector3.Scale(currentScale,localHitPointOnAxis);
                            trans.Scale.SetFloatingValue(newScale);
                            break;
                    }
                    EditorStates.CurrentSceneState.CurrentScene.SelectionState.SelectionChangedEvent.Invoke();
                }
            }
        }

        private void ProcessHotKeys()
        {
            if (_inputSystemAsset.Hotkeys.Undo.triggered)
            {
                CommandSystem.UndoCommand();
            }

            if (_inputSystemAsset.Hotkeys.Redo.triggered)
            {
                CommandSystem.RedoCommand();
            }
        }




        private static class InputHelper
        {
            private static Vector2 _mousePositionWhenHiding;

            private static bool _wasLeftPressed;
            private static bool _wasMiddlePressed;
            private static bool _wasRightPressed;

            private static bool _isLeftDown;
            private static bool _isMiddleDown;
            private static bool _isRightDown;

            public static void UpdateMouseDowns()
            {
                var leftPressed = IsLeftMouseButtonPressed();
                _isLeftDown = leftPressed && !_wasLeftPressed; // left down when left is pressed but was not pressed in the last frame
                _wasLeftPressed = leftPressed;

                var middlePressed = IsMiddleMouseButtonPressed();
                _isMiddleDown = middlePressed && !_wasMiddlePressed;
                _wasMiddlePressed = middlePressed;

                var rightPressed = IsRightMouseButtonPressed();
                _isRightDown = rightPressed && !_wasRightPressed;
                _wasRightPressed = rightPressed;
            }

            public static void HideMouse()
            {
                _mousePositionWhenHiding = Mouse.current.position.ReadValue();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

            public static void ShowMouse()
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;

                // set the mouse to its old position
                Mouse.current.WarpCursorPosition(_mousePositionWhenHiding);
            }

            public static Vector2 GetMouseMovement()
            {
                return Mouse.current.delta.ReadValue() / 20; // divide by 20 to get a similar value to the GetAxis of the old input system
            }

            public static Vector2 GetMousePosition()
            {
                return Mouse.current.position.ReadValue();
            }

            /// <summary>
            /// Returns the mouse position in viewport space of the camera in the scene panel.
            /// </summary>
            /// <returns></returns>
            public static Vector2 GetMousePositionInScenePanel()
            {
                // get the mouse position
                var mousePosition = GetMousePosition();
                // get the rectTransform from the Panel, the scene is currently visible in
                var sceneImageRectTransform = EditorStates.CurrentUnityState.SceneImage.rectTransform;
                // Get the position of the panel. This will give us the center of the panel, because the anchor is in the Center
                var panelPosCenter = new Vector2(sceneImageRectTransform.position.x, sceneImageRectTransform.position.y);
                // Calculate the bottom left corner position
                var panelPos = panelPosCenter - (sceneImageRectTransform.rect.size / 2);
                // Calculate the mouse position inside the panel
                var mousePosInPanel = mousePosition - panelPos;
                // convert the mouse position in panel into Viewport space
                var mousePosViewport = EditorStates.CurrentUnityState.MainCamera.ScreenToViewportPoint(mousePosInPanel);
                return mousePosViewport;
            }

            // check for mouse buttons
            public static bool IsLeftMouseButtonPressed()
            {
                return Mouse.current.leftButton.isPressed;
            }

            public static bool IsRightMouseButtonPressed()
            {
                return Mouse.current.rightButton.isPressed;
            }

            public static bool IsMiddleMouseButtonPressed()
            {
                return Mouse.current.middleButton.isPressed;
            }

            public static bool IsLeftMouseButtonDown()
            {
                return _isLeftDown;
            }

            public static bool IsMiddleMouseButtonDown()
            {
                return _isMiddleDown;
            }

            public static bool IsRightMouseButtonDown()
            {
                return _isRightDown;
            }
        }
    }

    internal static class InputHelper2
    {
        public static bool IsPressed(this InputAction action)
        {
            return action.ReadValue<float>() > 0;
        }
    }
}


