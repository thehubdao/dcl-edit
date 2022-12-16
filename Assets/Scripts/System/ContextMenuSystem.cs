using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.System
{
    public class ContextMenuSystem
    {
        // Dependencies
        EditorEvents _editorEvents;
        ContextMenuState _state;

        [Inject]
        private void Construct(EditorEvents editorEvents, ContextMenuState contextMenuState)
        {
            _editorEvents = editorEvents;
            _state = contextMenuState;

            _editorEvents.onMouseButtonDownEvent += OnMouseButtonDownCallback;
        }

        private void OnMouseButtonDownCallback(int button)
        {
            if (button == 0)
            {
                CloseIfMouseNotOverMenu();
            }
        }

        public void OpenMenu(Vector3 position, List<ContextMenuItem> items)
        {
            var placement = new ContextMenuState.Placement { expandDirection = ContextMenuState.Placement.Direction.Any, position = position };
            OpenMenu(new List<ContextMenuState.Placement> { placement }, items);
        }
        public void OpenMenu(List<ContextMenuState.Placement> possiblePlacements, List<ContextMenuItem> items)
        {
            var data = new ContextMenuState.Data(Guid.NewGuid(), items, possiblePlacements);
            _state.menuData.Clear();
            _state.menuData.Push(data);
            _editorEvents.InvokeUpdateContextMenuEvent();
        }

        public void OpenSubmenu(Guid menuId, List<ContextMenuState.Placement> possiblePlacements, List<ContextMenuItem> items)
        {
            // Check if menu is already open
            foreach (var menu in _state.menuData)
            {
                if (menu.menuId == menuId) { return; }
            }
            var data = new ContextMenuState.Data(menuId, items, possiblePlacements);
            _state.menuData.Push(data);
            _editorEvents.InvokeUpdateContextMenuEvent();
        }

        public void CloseMenu()
        {
            _state.menuData.Clear();
            _editorEvents.InvokeUpdateContextMenuEvent();
        }

        /// <summary>
        /// Closes all submenus below the submenu with the given id. The given submenu will stay open.
        /// </summary>
        /// <param name="id"></param>
        public void CloseMenusUntil(Guid id)
        {
            if (_state.menuData.Peek().menuId == id) { return; }               // Menu is already at the top
            while (_state.menuData.Count > 0)
            {
                if (_state.menuData.Peek().menuId == id) { break; }
                _state.menuData.Pop();
            }
            _editorEvents.InvokeUpdateContextMenuEvent();
        }

        private void CloseIfMouseNotOverMenu()
        {
            bool mouseOverMenu = false;
            foreach (var item in _state.menuGameObjects)
            {
                var mousePos = Input.mousePosition;
                var rect = item.Value.GetComponent<RectTransform>();
                var pos = rect.position;
                var size = rect.sizeDelta;
                var bottomLeftCorner = new Vector2(pos.x, pos.y - size.y);
                var topRightCorner = new Vector2(pos.x + size.x, pos.y);
                if (mousePos.x >= bottomLeftCorner.x && mousePos.y >= bottomLeftCorner.y && mousePos.x <= topRightCorner.x && mousePos.y <= topRightCorner.y)
                {
                    mouseOverMenu = true;
                    break;
                }
            }

            if (!mouseOverMenu)
            {
                CloseMenu();
            }
        }
    }
}