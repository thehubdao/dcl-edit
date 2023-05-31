using Assets.Scripts.EditorState;
using Assets.Scripts.Events;
using System.Collections.Generic;
using UnityEngine.Events;
using Zenject;

namespace Assets.Scripts.System
{
    public interface IMenuBarSystem
    {
        void AddMenuItem(string path, UnityAction onClick);
    }

    public class MenuBarSystem : IMenuBarSystem
    {
        // Dependencies
        private EditorEvents _editorEvents;
        private MenuBarState _state;

        [Inject]
        public void Construct(
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

        /// <summary>
        /// Add an Entry to the Menu Bar.
        /// It can be added to the context menu of the menu bar, but not directly to the bar itself.
        /// </summary>
        /// <param name="path">The path to the Menu/Submenu of the entry (e.g. "File/Open/Project")</param>
        /// <param name="onClick">The Action called when the Entry (e.g. "Project") is clicked.</param>
        /// <param name="position">The Position to add in the lowest level. -1 to append as last.</param>
        public void AddMenuItem(string path, UnityAction onClick)
        {
            _state.AddMenuItem(path, onClick);
            _editorEvents.InvokeUpdateMenuBarEvent();
        }
    }
}
