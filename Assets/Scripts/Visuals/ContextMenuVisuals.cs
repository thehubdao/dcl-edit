using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using System.Linq;
using Assets.Scripts.Visuals.NewUiBuilder;
using UnityEngine;
using Zenject;

namespace Assets.Scripts.Visuals
{
    public class ContextMenuVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        [SerializeField]
        float width = 200;

        // Dependencies
        ContextMenuState state;
        EditorEvents editorEvents;
        ContextMenuSystem contextMenuSystem;
        NewUiBuilder.NewUiBuilder.Factory newUiBuilderFactory;
        UnityState unityState;

        [Inject]
        void Construct(
            ContextMenuState contextMenuState,
            EditorEvents editorEvents,
            ContextMenuSystem contextMenuSystem,
            NewUiBuilder.NewUiBuilder.Factory newUiBuilderFactory,
            UnityState unityState)
        {
            this.state = contextMenuState;
            this.editorEvents = editorEvents;
            this.contextMenuSystem = contextMenuSystem;
            this.newUiBuilderFactory = newUiBuilderFactory;
            this.unityState = unityState;
        }

        public void SetupSceneEventListeners()
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
            var menuRect = CreateMenuParent(menuData.menuId.ToString());
            state.menuGameObjects.Add(menuData.menuId, menuRect.gameObject);
            menuRect.SetAsLastSibling();

            var mainContent = menuRect.GetComponent<PanelHandler>().Content;
            var itemsBuilder = newUiBuilderFactory.Create(mainContent);

            var menuPanel = new PanelAtom.Data();

            foreach (var item in menuData.items)
            {
                switch (item)
                {
                    case ContextMenuTextItem tItem:
                        menuPanel.AddContextMenuText(menuData.menuId, tItem.title, tItem.onClick, tItem.isDisabled, contextMenuSystem);
                        break;
                    case ContextSubmenuItem subItem:
                        menuPanel.AddContextSubmenu(menuData.menuId, subItem.submenuId, subItem.title, subItem.items, width, contextMenuSystem);
                        break;
                    case ContextMenuSpacerItem spItem:
                        menuPanel.AddContextMenuSpacer(menuData.menuId, contextMenuSystem);
                        break;
                }
            }

            itemsBuilder.Update(menuPanel);

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
            //var menu = new GameObject(name, typeof(RectTransform)).GetComponent<RectTransform>();
            var menu = Instantiate(unityState.ContextMenuAtom).GetComponent<RectTransform>();
            menu.SetParent(transform);
            menu.pivot = new Vector2(0, 1);
            menu.sizeDelta = new Vector2(width, 0);
            return menu;
        }
    }
}