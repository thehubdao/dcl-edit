using Assets.Scripts.EditorState;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Cursor = UnityEngine.Cursor;

namespace Assets.Scripts.Interaction
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

        // Dependencies
        UnityState _unityState;

        [Inject]
        private void Construct(UnityState unityState)
        {
            _unityState = unityState;
        }        

        public void UpdateMouseDowns()
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
            return Mouse.current.delta.ReadValue() / 20; // divide by 20 to get a similar value to the GetAxis of the old input system
        }

        public Vector2 GetMousePosition()
        {
            return Mouse.current.position.ReadValue();
        }

        /// <summary>
        /// Returns the mouse position in viewport space of the camera in the scene panel.
        /// </summary>
        /// <returns></returns>
        public Vector2 GetMousePositionInScenePanel()
        {
            // get the mouse position
            var mousePosition = GetMousePosition();
            // get the rectTransform from the Panel, the scene is currently visible in
            var sceneImageRectTransform = _unityState.SceneImage.rectTransform;
            // Get the position of the panel. This will give us the center of the panel, because the anchor is in the Center
            var panelPosCenter = new Vector2(sceneImageRectTransform.position.x, sceneImageRectTransform.position.y);
            // Calculate the bottom left corner position
            var panelPos = panelPosCenter - (sceneImageRectTransform.rect.size / 2);
            // Calculate the mouse position inside the panel
            var mousePosInPanel = mousePosition - panelPos;
            // convert the mouse position in panel into Viewport space
            var mousePosViewport = _unityState.MainCamera.ScreenToViewportPoint(mousePosInPanel);
            return mousePosViewport;
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
            return _isLeftDown;
        }

        public bool IsMiddleMouseButtonDown()
        {
            return _isMiddleDown;
        }

        public bool IsRightMouseButtonDown()
        {
            return _isRightDown;
        }
    }
}