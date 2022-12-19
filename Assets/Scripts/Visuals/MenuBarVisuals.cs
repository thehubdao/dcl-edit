using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
using Assets.Scripts.Visuals.UiBuilder;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;
using static Assets.Scripts.EditorState.MenuBarState;

namespace Assets.Scripts.Visuals
{
    public class MenuBarVisuals : MonoBehaviour, ISetupSceneEventListeners
    {
        // Dependencies
        MenuBarState _state;
        EditorEvents _editorEvents;
        ContextMenuSystem _contextMenuSystem;
        UiBuilder.UiBuilder _uiBuilder;

        PanelAtom.Data panel;

        [Inject]
        void Construct(MenuBarState menuBarState, EditorEvents editorEvents, ContextMenuSystem contextMenuSystem, UiBuilder.UiBuilder.Factory uiBuilder)
        {
            _state = menuBarState;
            _editorEvents = editorEvents;
            _contextMenuSystem = contextMenuSystem;
            _uiBuilder = uiBuilder.Create(gameObject);

            panel = new PanelAtom.Data();
            panel.layoutDirection = UiHandler.PanelHandler.LayoutDirection.Horizontal;
        }

        public void SetupSceneEventListeners()
        {
            _editorEvents.onUpdateMenuBarEvent += UpdateMenuBar;
        }

        private void UpdateMenuBar()
        {
            FillMenuBar();
        }

        private void FillMenuBar()
        {
            panel.childDates.Clear();

            foreach (MenuBarItem item in _state.menuItems)
            {
                panel.AddMenuBarButton(item.title, new UnityAction<GameObject>((button) => OpenMenuBarContextMenu(button.GetComponent<RectTransform>(), item.subItems)));
            }

            _uiBuilder.Update(panel);
        }

        private void OpenMenuBarContextMenu(RectTransform transform, List<ContextMenuItem> menuItems)
        {
            Vector3[] fourCorners = new Vector3[4];
            transform.GetWorldCorners(fourCorners);
            _contextMenuSystem.OpenMenu(fourCorners[0], menuItems);
        }
    }
}
