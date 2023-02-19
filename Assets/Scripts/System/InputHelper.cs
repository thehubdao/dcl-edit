using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Cursor = UnityEngine.Cursor;

namespace Assets.Scripts.System
{
    public class InputHelper
    {
        private Vector2 _mousePositionWhenHiding;

        private bool _wasLeftPressed;
        private bool _wasMiddlePressed;
        private bool _wasRightPressed;

        private bool _isLeftDown;
        private bool _isMiddleDown;
        private bool _isRightDown;

        private int _lastMouseDownCalculated = -1;

        private float mousesensitivity;

        // Dependencies
        private InputSystemAsset _inputSystemAsset;
        private UnityState _unityState;
        private SettingsSystem _settingsSystem;
        private EditorEvents _editorEvents;

        [Inject]
        private void Construct(
            UnityState unityState,
            SettingsSystem settingsSystem,
            EditorEvents editorEvents)
        {
            _inputSystemAsset = new InputSystemAsset();
            _inputSystemAsset.Modifier.Enable();

            _unityState = unityState;
            _settingsSystem = settingsSystem;
            _editorEvents = editorEvents;


            // mouse sensitivity
            SetMouseSensitivity();
            _editorEvents.onSettingsChangedEvent += SetMouseSensitivity;
        }

        private void SetMouseSensitivity()
        {
            mousesensitivity = _settingsSystem.mouseSensitivity.Get();
        }

        private void RequireMouseDowns()
        {
            if (_lastMouseDownCalculated != Time.frameCount)
            {
                UpdateMouseDowns();
                _lastMouseDownCalculated = Time.frameCount;
            }
        }

        private void UpdateMouseDowns()
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

        public void HideMouse()
        {
            _mousePositionWhenHiding = Mouse.current.position.ReadValue();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void ShowMouse()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            // set the mouse to its old position
            Mouse.current.WarpCursorPosition(_mousePositionWhenHiding);
        }

        public Vector2 GetMouseMovement()
        {
            return (Mouse.current.delta.ReadValue() / 20) * mousesensitivity; // divide by 20 to get a similar value to the GetAxis of the old input system
        }

        public Vector2 GetMousePosition()
        {
            return Mouse.current.position.ReadValue();
        }

        public bool GetIsControlPressed()
        {
            return _inputSystemAsset.Modifier.Ctrl.IsPressed();
        }

        public bool GetIsAltPressed()
        {
            return _inputSystemAsset.Modifier.Alt.IsPressed();
        }

        public bool GetIsShiftPressed()
        {
            return _inputSystemAsset.Modifier.Shift.IsPressed();
        }

        /// <summary>
        /// Returns the mouse position in viewport space of the camera in the scene panel.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMousePositionInScenePanel()
        {
            Vector3[] fourCorners = new Vector3[4];
            _unityState.SceneImage.rectTransform.GetWorldCorners(fourCorners);

            Vector2 mousePosInPanel = GetMousePosition() - new Vector2(fourCorners[0].x, fourCorners[0].y);
            Vector3 mousePosViewport = _unityState.MainCamera.ScreenToViewportPoint(mousePosInPanel);

            return mousePosViewport;
        }

        public bool IsMouseOverScenePanel()
        {
            var mousePosViewport = GetMousePositionInScenePanel();
            // Figure out, if the mouse is over the Game window
            return mousePosViewport.x >= 0 &&
                    mousePosViewport.x < 1 &&
                    mousePosViewport.y >= 0 &&
                    mousePosViewport.y < 1;
        }

        public Vector3 GetMousePositionInScene()
        {
            var mousePosition = GetMousePositionInScenePanel();
            var mouseRay = _unityState.MainCamera.ViewportPointToRay(mousePosition);
            var farClipPlane = _unityState.MainCamera.farClipPlane;
            if (Physics.Raycast(mouseRay, out RaycastHit hit, farClipPlane))
            {
                return hit.point;
            }
            else
            {
                return mouseRay.GetPoint(farClipPlane);
            }
        }

        // check for mouse buttons
        public bool IsLeftMouseButtonPressed()
        {
            return Mouse.current.leftButton.isPressed;
        }

        public bool IsRightMouseButtonPressed()
        {
            return Mouse.current.rightButton.isPressed;
        }

        public bool IsMiddleMouseButtonPressed()
        {
            return Mouse.current.middleButton.isPressed;
        }

        public bool IsLeftMouseButtonDown()
        {
            RequireMouseDowns();
            return _isLeftDown;
        }

        public bool IsMiddleMouseButtonDown()
        {
            RequireMouseDowns();
            return _isMiddleDown;
        }

        public bool IsRightMouseButtonDown()
        {
            RequireMouseDowns();
            return _isRightDown;
        }
    }

    public static class InputHelper2
    {
        public static bool IsPressed(this InputAction action)
        {
            return action.ReadValue<float>() > 0;
        }
    }
}