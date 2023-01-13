using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.SceneState;
using Assets.Scripts.System;
using ModestTree;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Assets.Scripts.Interaction
{
    public class GeneralInputInteraction : MonoBehaviour
    {
        private InputSystemAsset inputSystemAsset;

        // Dependencies
        private ISceneSaveSystem sceneSaveSystem;
        private ICommandSystem commandSystem;
        private InputState inputState;
        private Interface3DState interface3DState;
        private GizmoState gizmoState;
        private UnityState unityState;
        private InputHelper inputHelper;
        private CameraState cameraState;
        private EditorEvents editorEvents;
        private EntitySelectSystem entitySelectSystem;
        private ContextMenuSystem contextMenuSystem;
        private SceneManagerSystem sceneManagerSystem;
        private GizmoToolInteraction gizmoToolInteraction;
        private SettingsSystem settingsSystem;

        [Inject]
        private void Construct(
            ISceneSaveSystem sceneSaveSystem,
            ICommandSystem commandSystem,
            InputState inputState,
            Interface3DState interface3DState,
            CameraState cameraState,
            GizmoState gizmoState,
            UnityState unityState,
            InputHelper inputHelper,
            EditorEvents editorEvents,
            EntitySelectSystem entitySelectSystem,
            ContextMenuSystem contextMenuSystem,
            SceneManagerSystem sceneManagerSystem,
            GizmoToolInteraction gizmoToolInteraction,
            SettingsSystem settingsSystem)
        {
            this.sceneSaveSystem = sceneSaveSystem;
            this.commandSystem = commandSystem;
            this.inputState = inputState;
            this.interface3DState = interface3DState;
            this.gizmoState = gizmoState;
            this.unityState = unityState;
            this.inputHelper = inputHelper;
            this.cameraState = cameraState;
            this.editorEvents = editorEvents;
            this.entitySelectSystem = entitySelectSystem;
            this.contextMenuSystem = contextMenuSystem;
            this.sceneManagerSystem = sceneManagerSystem;
            this.gizmoToolInteraction = gizmoToolInteraction;
            this.settingsSystem = settingsSystem;
        }


        void Awake()
        {
            inputSystemAsset = new InputSystemAsset();
            inputSystemAsset.CameraMovement.Enable();
            inputSystemAsset.Modifier.Enable();
            inputSystemAsset.Hotkeys.Enable();
        }

        // Update is called once per frame
        void Update()
        {
            ProcessHotKeys();

            InvokeMouseButtonDownEvent();

            switch (inputState.InState)
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
                    inputState.InState = InputState.InStateType.NoInput;
                    break;
            }
        }

        private void InvokeMouseButtonDownEvent()
        {
            if (Input.GetMouseButtonDown(0))
            {
                editorEvents.InvokeOnMouseButtonDownEvent(0);
            }
            if (Input.GetMouseButtonDown(1))
            {
                editorEvents.InvokeOnMouseButtonDownEvent(1);
            }
        }

        private void UpdateNoInput()
        {
            var pressingAlt = inputSystemAsset.Modifier.Alt.IsPressed();
            var pressingControl = inputSystemAsset.Modifier.Ctrl.IsPressed();
            var pressingShift = inputSystemAsset.Modifier.Shift.IsPressed();

            var isSprinting = inputSystemAsset.CameraMovement.Fast.IsPressed();

            // Figure out where the mouse points at
            Ray mouseRay;
            bool isMouseOverGameWindow;
            bool isMouseOverContextMenu;
            {
                var mousePosViewport = inputHelper.GetMousePositionInScenePanel();
                // Get the ray from the Camera, that corresponds to the mouse position in the panel
                mouseRay = unityState.MainCamera.ViewportPointToRay(mousePosViewport);
                // Figure out, if the mouse is over the Game window
                isMouseOverGameWindow = mousePosViewport.x >= 0 &&
                                        mousePosViewport.x < 1 &&
                                        mousePosViewport.y >= 0 &&
                                        mousePosViewport.y < 1;
                isMouseOverContextMenu = contextMenuSystem.IsMouseOverMenu();
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
                    interface3DState.CurrentlyHoveredObject = hitInfoGizmos.transform.gameObject;
                    interface3DState.CurrentlyHoveredObjectType =
                        Interface3DState.HoveredObjectType.Gizmo;
                }
                // Secondly check, if mouse is hovering over a Entity
                else if (Physics.Raycast(mouseRay, out RaycastHit hitInfoEntity, 10000, LayerMask.GetMask("Entity Click"))) // mouse click layer
                {
                    mousePositionIn3DView = hitInfoEntity.point;
                    interface3DState.CurrentlyHoveredObject = hitInfoEntity.transform.gameObject;
                    interface3DState.CurrentlyHoveredObjectType =
                        Interface3DState.HoveredObjectType.Entity;
                }
                else
                {
                    interface3DState.CurrentlyHoveredObject = null;
                    interface3DState.CurrentlyHoveredObjectType =
                        Interface3DState.HoveredObjectType.None;
                }
            }
            else
            {
                interface3DState.CurrentlyHoveredObject = null;
                interface3DState.CurrentlyHoveredObjectType =
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

            // When pressing Duplicates selected entity
            if (inputSystemAsset.Hotkeys.Duplicate.triggered)
            {
                var currentScene = sceneManagerSystem.GetCurrentScene();
                if (currentScene != null)
                {
                    var selectedEntity = currentScene.SelectionState.PrimarySelectedEntity;
                    commandSystem.ExecuteCommand(commandSystem.CommandFactory.CreateDuplicateEntity(selectedEntity.Id));
                }
            }
            // When pressing(down) Left mouse button, select the hovered entity
            if (inputHelper.IsLeftMouseButtonDown() && !pressingAlt && isMouseIn3DView)
            {
                switch (interface3DState.CurrentlyHoveredObjectType)
                {
                    case Interface3DState.HoveredObjectType.Gizmo:
                    {
                        gizmoToolInteraction.ClickOnGizmoTool(mousePositionIn3DView.Value);

                        break;
                    }
                    case Interface3DState.HoveredObjectType.Entity:
                    {
                        // select entity
                        var selectInteraction = interface3DState.CurrentlyHoveredObject?
                            .GetComponentInParent<EntitySelectInteraction>();

                        if (selectInteraction != null)
                        {
                            selectInteraction.Select();
                        }

                        break;
                    }
                    case Interface3DState.HoveredObjectType.None:
                    {
                        entitySelectSystem.DeselectAll();
                        break;
                    }
                    default: // ignore
                        break;
                }
            }

            // When pressing Right mouse button (without alt), switch to Camera WASD moving state
            if (inputHelper.IsRightMouseButtonPressed() && !pressingAlt && isMouseIn3DView)
            {
                inputState.InState = InputState.InStateType.WasdMovement;
                inputHelper.HideMouse();
            }

            // When pressing alt and Right mouse button, switch to Mouse Zooming state
            if (inputHelper.IsRightMouseButtonPressed() && pressingAlt && isMouseIn3DView)
            {
                inputState.InState = InputState.InStateType.MouseZoom;
                inputHelper.HideMouse();
            }

            // When pressing ald and Left mouse button, switch to Rotate around point state
            if (inputHelper.IsLeftMouseButtonPressed() && pressingAlt && isMouseIn3DView)
            {
                inputState.RotateCameraAroundPoint = mousePositionIn3DView;
                inputState.InState = InputState.InStateType.RotateAroundPoint;
                inputHelper.HideMouse();
            }

            // When pressing Middle mouse button, switch to "Camera slide moving state"
            if (inputHelper.IsMiddleMouseButtonPressed() && isMouseIn3DView)
            {
                inputState.InState = InputState.InStateType.SlideSideways;
                inputHelper.HideMouse();
            }

            // When scrolling Mouse wheel, zoom in or out
            if (isMouseIn3DView)
            {
                var mouseScroll = Mouse.current.scroll.ReadValue().y / 1000;

                if (Mathf.Abs(mouseScroll) > 0)
                {
                    cameraState.MoveStep(new Vector3(0, 0, mouseScroll), isSprinting);
                }
            }

            // When pressing the focus hotkey and having a selected primary entity, switch to Focus Transition state
            if (inputSystemAsset.CameraMovement.Focus.triggered && sceneManagerSystem.GetCurrentScene()?.SelectionState.PrimarySelectedEntity != null)
            {
                // Fetch position of selected object
                var selectedEntity = sceneManagerSystem.GetCurrentScene()?.SelectionState.PrimarySelectedEntity;
                var entityPos = selectedEntity.GetTransformComponent().GlobalPosition;

                // Calculate an offset position so that the camera keeps its rotation and looks at the selected entity
                Vector3 cameraForward = cameraState.Forward;
                Vector3 destination = entityPos - cameraForward * 10;
                inputState.FocusTransitionDestination = destination;

                inputState.InState = InputState.InStateType.FocusTransition;
            }

            // When pressing the save hotkey, save the scene
            if (inputSystemAsset.Hotkeys.Save.triggered)
            {
                sceneManagerSystem.SaveCurrentScene();
            }

            // When pressing Translate hotkey, enter translation gizmo mode
            if (inputSystemAsset.Hotkeys.Translate.triggered)
            {
                gizmoState.CurrentMode = GizmoState.Mode.Translate;
            }

            // When pressing Rotate hotkey, enter rotation gizmo mode
            if (inputSystemAsset.Hotkeys.Rotate.triggered)
            {
                gizmoState.CurrentMode = GizmoState.Mode.Rotate;
            }

            // When pressing Scale hotkey, enter rotation gizmo mode
            if (inputSystemAsset.Hotkeys.Scale.triggered)
            {
                gizmoState.CurrentMode = GizmoState.Mode.Scale;
            }

            // When pressing ToggleSnapping hot key, toggle snapping setting
            if (inputSystemAsset.Hotkeys.ToggleSnapping.triggered)
            {
                settingsSystem.gizmoToolSnapping.Set(!settingsSystem.gizmoToolSnapping.Get());
            }
        }

        private void UpdateWasdMovement()
        {
            var isSprinting = inputSystemAsset.CameraMovement.Fast.IsPressed();

            // Rotate camera with mouse movement
            cameraState.RotateStep(inputHelper.GetMouseMovement());

            // Move camera with WASD keys. To go faster, hold shift
            cameraState.MoveContinuously(
                new Vector3(
                    inputSystemAsset.CameraMovement.LeftRight.ReadValue<float>(),
                    inputSystemAsset.CameraMovement.UpDown.ReadValue<float>(),
                    inputSystemAsset.CameraMovement.ForwardBackward.ReadValue<float>()),
                isSprinting);

            // When releasing Right mouse button, switch to "No input state"
            if (!inputHelper.IsRightMouseButtonPressed())
            {
                inputState.InState = InputState.InStateType.NoInput;
                inputHelper.ShowMouse();
            }
        }

        private void UpdateMouseZoom()
        {
            var isSprinting = inputSystemAsset.CameraMovement.Fast.IsPressed();
            var mouseMovement = inputHelper.GetMouseMovement();
            var targetMovement = mouseMovement.x + mouseMovement.y;

            cameraState.MoveStep(Vector3.forward * targetMovement * 0.03f, isSprinting);


            // When releasing Right mouse button, switch to "No input state"
            if (!inputHelper.IsRightMouseButtonPressed())
            {
                inputState.InState = InputState.InStateType.NoInput;
                inputHelper.ShowMouse();
            }
        }

        private void UpdateRotateAroundPoint()
        {
            // Rotate camera around point with mouse movement
            if (inputState.RotateCameraAroundPoint != null)
                cameraState.RotateAroundPointStep(
                    inputState.RotateCameraAroundPoint.Value,
                    inputHelper.GetMouseMovement());

            // When releasing Left mouse button, switch to "No input state"
            if (!inputHelper.IsLeftMouseButtonPressed())
            {
                inputState.InState = InputState.InStateType.NoInput;
                inputHelper.ShowMouse();
            }
        }

        private void UpdateSlideSideways()
        {
            var isSprinting = inputSystemAsset.CameraMovement.Fast.IsPressed();
            // Move camera sideways with mouse movement
            var mouseMovement = inputHelper.GetMouseMovement();
            var targetMovement = new Vector3(mouseMovement.x, mouseMovement.y, 0);
            cameraState.MoveStep(targetMovement * -0.03f, isSprinting);

            // When releasing Middle mouse button, switch to "No input state"
            if (!inputHelper.IsMiddleMouseButtonPressed())
            {
                inputState.InState = InputState.InStateType.NoInput;
                inputHelper.ShowMouse();
            }
        }

        private void UpdateFocusTransition()
        {
            // Control inputs are prioritized over the focus transition.
            UpdateNoInput();

            // Check if the state changed because of a user interaction.
            if (inputState.InState != InputState.InStateType.FocusTransition)
            {
                return;
            }


            if (cameraState.MoveTowards((Vector3)inputState.FocusTransitionDestination, true))
            {
                inputState.InState = InputState.InStateType.NoInput;
            }
        }

        private void UpdateUiInput()
        {
            // Prevent Inputs if currently typing in a text field
        }

        private void UpdateHoldingGizmoTool()
        {
            gizmoToolInteraction.UpdateHoldingGizmoTool();
        }

        private void ProcessHotKeys()
        {
            if (inputSystemAsset.Hotkeys.Undo.triggered)
            {
                commandSystem.UndoCommand();
            }

            if (inputSystemAsset.Hotkeys.Redo.triggered)
            {
                commandSystem.RedoCommand();
            }
        }
    }
}