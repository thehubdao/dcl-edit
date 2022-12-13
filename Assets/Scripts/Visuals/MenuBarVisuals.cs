using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using Assets.Scripts.System;
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
        UiBuilder.Factory _uiBuilderFactory;

        [Inject]
        void Construct(MenuBarState menuBarState, EditorEvents editorEvents, ContextMenuSystem contextMenuSystem, UiBuilder.Factory uiBuilderFactory)
        {
            _state = menuBarState;
            _editorEvents = editorEvents;
            _contextMenuSystem = contextMenuSystem;
            _uiBuilderFactory = uiBuilderFactory;
        }

        public void SetupSceneEventListeners()
        {
            _editorEvents.onUpdateMenuBarEvent += UpdateMenuBar;
        }

        private void UpdateMenuBar()
        {
            Debug.Log("ToDo: Clear and fill the menu bar is not optimal.");
            ClearMenuBar();
            FillMenuBar();
        }

        private void ClearMenuBar()
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
        }

        private void FillMenuBar()
        {
            var itemsBuilder = _uiBuilderFactory.Create();

            foreach (MenuBarItem item in _state.menuItems)
            {
                itemsBuilder.MenuBarButon(item.title, new UnityAction<GameObject>((button) => OpenMenuBarContextMenu(button.GetComponent<RectTransform>(), item.subItems)));
                //itemsBuilder.MenuBarButon(item.title, OpenMenuBarContextMenu);
            }

            itemsBuilder.ClearAndMake(gameObject);
        }

        private void OpenMenuBarContextMenu(RectTransform transform, List<ContextMenuItem> menuItems)
        {
            Vector3[] fourCorners = new Vector3[4];
            transform.GetWorldCorners(fourCorners);
            Debug.Log(fourCorners[0]);
            _contextMenuSystem.OpenMenu(fourCorners[0], menuItems);
        }
    }
}
