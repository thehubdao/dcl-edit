using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class ContextMenuVisuals : MonoBehaviour
    {
        [SerializeField] float width = 200;

        // Dependencies
        ContextMenuState state;
        EditorEvents editorEvents;
        ContextMenuSystem contextMenuSystem;
        UiBuilder.UiBuilder.Factory uiBuilderFactory;
        UnityState unityState;
        float CanvasScale => GetComponentInParent<CanvasScaler>().scaleFactor;

        [Inject]
        void Construct(
            ContextMenuState contextMenuState,
            EditorEvents editorEvents,
            ContextMenuSystem contextMenuSystem,
            UiBuilder.UiBuilder.Factory uiBuilderFactory,
            UnityState unityState)
        {
            this.state = contextMenuState;
            this.editorEvents = editorEvents;
            this.contextMenuSystem = contextMenuSystem;
            this.uiBuilderFactory = uiBuilderFactory;
            this.unityState = unityState;

            SetupEventListeners();
        }

        private void SetupEventListeners()
        {
            editorEvents.onUpdateContextMenuEvent += () =>
            {
                transform.SetAsLastSibling(); // Draw context menu on top of UI
                RemoveObsoleteMenuVisuals();
                CreateMissingMenuVisuals();
            };
        }

        void RemoveObsoleteMenuVisuals()
        {
            var objectsToDelete = state.menuGameObjects.Keys.ToList();
            foreach (var data in state.menuData)
            {
                if (objectsToDelete.Contains(data.menuId))
                {
                    objectsToDelete.Remove(data.menuId);
                }
            }

            foreach (var delObj in objectsToDelete)
            {
                GameObject go = state.menuGameObjects[delObj];
                Destroy(go);
                state.menuGameObjects.Remove(delObj);
            }
        }

        void CreateMissingMenuVisuals()
        {
            foreach (var menuData in state.menuData)
            {
                if (!state.menuGameObjects.ContainsKey(menuData.menuId))
                {
                    if (menuData.items.Count == 0)
                    {
                        break;
                    }

                    CreateSingleMenu(menuData);
                }
            }
        }

        void CreateSingleMenu(ContextMenuState.Data menuData)
        {
            var menuRect = CreateMenuParent();
            state.menuGameObjects.Add(menuData.menuId, menuRect.gameObject);
            menuRect.SetAsLastSibling();

            var mainContent = menuRect.GetComponent<PanelHandler>().content;
            var itemsBuilder = uiBuilderFactory.Create(mainContent);

            var menuPanel = new PanelAtom.Data();

            foreach (var item in menuData.items)
            {
                switch (item)
                {
                    case ContextMenuTextItem tItem:
                        menuPanel.AddContextMenuText(menuData.menuId, tItem.title, tItem.onClick, tItem.isDisabled);
                        break;
                    case ContextSubmenuItem subItem:
                        var isDisabled = CheckAllSubItemsDisabledRecursive(subItem.items);
                        menuPanel.AddContextSubmenu(menuData.menuId, subItem.submenuId, subItem.title, subItem.items,
                            width * CanvasScale, isDisabled);
                        break;
                    case ContextMenuSpacerItem spItem:
                        menuPanel.AddContextMenuSpacer(menuData.menuId);
                        break;
                }
            }

            itemsBuilder.Update(menuPanel);

            LayoutRebuilder.ForceRebuildLayoutImmediate(itemsBuilder.parentObject.GetComponent<RectTransform>());
            menuRect.sizeDelta = new Vector2(width, Mathf.Min(Screen.height, itemsBuilder.height));

            // Find a placement for the new menu where it is fully visible on the screen
            // Info: 0,0 is bottom left of screen
            foreach (var placement in menuData.possiblePlacements)
            {
                Vector3 pos = placement.position;

                // 1. position on y axis
                var contentRect = mainContent.GetComponent<RectTransform>();
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

                        pos = new Vector3(pos.x - width * CanvasScale, pos.y, pos.z);
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

        private static bool CheckAllSubItemsDisabledRecursive(List<ContextMenuItem> contextMenuItems)
        {
            foreach (var item in contextMenuItems)
            {
                switch (item)
                {
                    case ContextMenuTextItem { isDisabled: false }:
                        return false;
                    case ContextSubmenuItem contextSubmenuItem:
                        if (!CheckAllSubItemsDisabledRecursive(contextSubmenuItem.items))
                        {
                            return false;
                        }

                        break;
                }
            }

            return true;
        }

        RectTransform CreateMenuParent()
        {
            var menu = Instantiate(unityState.ContextMenuAtom).GetComponent<RectTransform>();
            menu.SetParent(transform, false);
            menu.pivot = new Vector2(0, 1);
            menu.sizeDelta = new Vector2(width, 0);
            return menu;
        }
    }
}