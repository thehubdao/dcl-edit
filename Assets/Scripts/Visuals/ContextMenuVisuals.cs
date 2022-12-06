using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using System.Linq;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class ContextMenuVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        [SerializeField]
        float width = 200;

        // Dependencies
        ContextMenuState _state;
        EditorEvents _editorEvents;
        ContextMenuSystem _contextMenuSystem;
        UiBuilder.Factory _uiBuilderFactory;

        [Inject]
        void Construct(ContextMenuState contextMenuState, EditorEvents editorEvents, ContextMenuSystem contextMenuSystem, UiBuilder.Factory uiBuilderFactory)
        {
            _state = contextMenuState;
            _editorEvents = editorEvents;
            _contextMenuSystem = contextMenuSystem;
            _uiBuilderFactory = uiBuilderFactory;
        }

        public void SetupSceneEventListeners()
        {
            _editorEvents.onUpdateContextMenuEvent += () =>
            {
                transform.SetAsLastSibling();               // Draw context menu on top of UI
                RemoveObsoleteMenuVisuals();
                CreateMissingMenuVisuals();
            };
        }

        void RemoveObsoleteMenuVisuals()
        {
            var objectsToDelete = _state.menuGameObjects.Keys.ToList();
            foreach (var data in _state.menuData)
            {
                if (objectsToDelete.Contains(data.menuId))
                {
                    objectsToDelete.Remove(data.menuId);
                }
            }
            foreach (var delObj in objectsToDelete)
            {
                GameObject go = _state.menuGameObjects[delObj];
                Destroy(go);
                _state.menuGameObjects.Remove(delObj);
            }
        }

        void CreateMissingMenuVisuals()
        {
            foreach (var menuData in _state.menuData)
            {
                if (!_state.menuGameObjects.ContainsKey(menuData.menuId))
                {
                    if (menuData.items.Count == 0) { break; }
                    CreateSingleMenu(menuData);
                }
            }
        }

        void CreateSingleMenu(ContextMenuState.Data menuData)
        {
            var menuRect = CreateMenuParent(menuData.menuId.ToString());
            _state.menuGameObjects.Add(menuData.menuId, menuRect.gameObject);
            menuRect.SetAsLastSibling();

            var itemsBuilder = _uiBuilderFactory.Create();
            for (int i = 0; i < menuData.items.Count; i++)
            {
                var item = menuData.items[i];
                if (item is ContextMenuTextItem tItem)
                {
                    itemsBuilder.ContextMenuTextItem(menuData.menuId, tItem.title, tItem.onClick, tItem.isDisabled, _contextMenuSystem);
                }
                else if (item is ContextSubmenuItem subItem)
                {
                    itemsBuilder.ContextSubmenuItem(menuData.menuId, subItem.submenuId, subItem.title, subItem.items, width, _contextMenuSystem);
                }
                else if (item is ContextMenuSpacerItem spItem)
                {
                    itemsBuilder.ContextMenuSpacerItem(menuData.menuId, _contextMenuSystem);
                }
            }

            var mainContextMenuBuilder = _uiBuilderFactory.Create();
            mainContextMenuBuilder.ContextMenu(itemsBuilder);
            mainContextMenuBuilder.ClearAndMake(menuRect.gameObject);

            // Find a placement for the new menu where it is fully visible on the screen
            // Info: 0,0 is bottom left of screen
            foreach (var placement in menuData.possiblePlacements)
            {
                Vector3 pos = placement.position;

                // 1. position on y axis
                var contentRect = menuRect.GetComponentInChildren<PanelHandler>().Content.GetComponent<RectTransform>();
                Vector2 bottomRightCorner = new Vector2(pos.x + menuRect.sizeDelta.x, pos.y - contentRect.sizeDelta.y);
                if (menuRect.sizeDelta.y >= Screen.height)
                {
                    pos = new Vector3(pos.x, Screen.height, pos.z);
                }
                else if (bottomRightCorner.y < 0)
                {
                    pos = new Vector3(pos.x, contentRect.sizeDelta.y, pos.z);
                }
                // 2. position on x axis
                switch (placement.expandDirection)
                {
                    case ContextMenuState.Placement.Direction.Any:
                        if (bottomRightCorner.x > Screen.width)
                        {
                            var overshoot = bottomRightCorner.x - Screen.width;
                            pos = new Vector3(pos.x - overshoot, pos.y, pos.z);
                        }
                        break;
                    case ContextMenuState.Placement.Direction.Left:
                        if (pos.x - width < 0)
                        {
                            continue;
                        }
                        pos = new Vector3(pos.x - width, pos.y, pos.z);
                        break;
                    case ContextMenuState.Placement.Direction.Right:
                        if (bottomRightCorner.x > Screen.width)
                        {
                            continue;
                        }
                        break;
                    default:
                        break;
                }
                menuRect.position = pos;
                break;
            }
        }

        RectTransform CreateMenuParent(string name = "ContextMenu")
        {
            var menu = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            menu.SetParent(transform);
            menu.pivot = new Vector2(0, 1);
            menu.sizeDelta = new Vector2(width, 0);
            return menu;
        }
    }
}