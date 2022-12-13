using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Zenject;

namespace Assets.Scripts.System
{
    public class MenuBarSystem
    {
        // Dependencies
        private EditorEvents _editorEvents;
        private MenuBarState _state;

        [Inject]
        private void Construct(
            EditorEvents editorEvents,
            MenuBarState menuBarState
            )
        {
            _editorEvents = editorEvents;
            _state = menuBarState;
        }

        public class MenuBarItem
        {
            public string title;
            public List<ContextMenuItem> contextMenuItems;

            public MenuBarItem(string title, List<ContextMenuItem> contextMenuItems)
            {
                this.title = title;
                this.contextMenuItems = contextMenuItems;
            }
        }

        public void AddMenuItem(string path, UnityAction onClick, int position = -1)
        {
            _state.AddMenuItem(path, onClick, position);
            _editorEvents.InvokeUpdateMenuBarEvent();
        }

        public void DebugCreateExampleMenu()
        {
            Debug.Log("DebugCreateExampleMenu()");

            string[] testMenues = {
                "A1/A2/A3",
                "A1/A2/A4",
                "A1/A2/A5",
                "A1/A6",
                "B1/B2",
                "B1/B3/B4",
                "B1/B3/B5",
                "B1/B6",
                "C1/C2/C3/C4/C5/C6/C7/C8/C9"
            };

            foreach (string testMenue in testMenues)
            {
                AddMenuItem(testMenue, () => TestOnClick(testMenue));
            }
        }

        private void TestOnClick(string log)
        {
            Debug.Log(log);
        }
    }
}
