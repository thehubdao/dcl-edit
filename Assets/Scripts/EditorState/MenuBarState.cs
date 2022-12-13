using Assets.Scripts.EditorState;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEditor;

namespace Assets.Scripts.EditorState
{
    public class MenuBarState
    {
        public List<MenuBarItem> menuItems = new List<MenuBarItem>(); //wish to be Lis<MenuBarItem>

        public class MenuBarItem : ContextMenuItem
        {
            public string title;
            public List<ContextMenuItem> subItems;

            public MenuBarItem(string title)
            {
                this.title = title;
                this.subItems = new List<ContextMenuItem>();
            }

            public MenuBarItem(string title, List<ContextMenuItem> subItems)
            {
                this.title = title;
                this.subItems = subItems;
            }
        }

        public void AddMenuItem(string path, UnityAction onClick, int position = -1)
        {
            // Check for root directory.
            if (path.IndexOf('/') < 0)
            {
                Debug.LogError($"Cannot add actions to the root of the menu bar. Create at least one directory like \"File/My Action\".");
            }

            // Check for nameless directories.
            foreach (string pathPart in path.Split('/'))
            {
                if (pathPart == "")
                {
                    Debug.LogError($"Unvalid Path for adding to menu bar: \"{path}\".");
                    return;
                }
            }

            // Check for null action.
            if (onClick == null)
            {
                Debug.LogError("Null cannot be added to the menu bar items.");
                return;
            }

            (string pathPartFirst, string pathPartSub) = SplitPath(path);

            MenuBarItem itemToAddTo = GetOrCreateMenuBarItem(pathPartFirst);
            AddToExistingContextMenuItems(itemToAddTo.subItems, pathPartSub, onClick, position);
        }


        private MenuBarItem GetOrCreateMenuBarItem(string title)
        {
            MenuBarItem item = menuItems.SingleOrDefault(item => item.title == title);

            // Create new menu bar item if it does not exist yet.
            if (item == null)
            {
                item = new MenuBarItem(title);
                menuItems.Add(item);
            }

            return item;
        }


        private void AddToExistingContextMenuItems(List<ContextMenuItem> items, string path, UnityAction onClick, int position)
        {
            if (path.IndexOf('/') < 0)
            {
                AddToNewContextMenuItems(items, path, onClick, position);
            }
            else
            {
                (string pathPartFirst, string pathPartSub) = SplitPath(path);

                if (items == null)
                {
                    Debug.Log(path);
                }

                ContextSubmenuItem foundItem = (ContextSubmenuItem)items.SingleOrDefault(item => (item as ContextSubmenuItem)?.title == pathPartFirst);

                if (foundItem != null)
                {
                    //recursive
                    AddToExistingContextMenuItems(foundItem.items, pathPartSub, onClick, position);
                }
                else
                {
                    //switch to adding items
                    AddToNewContextMenuItems(items, path, onClick, position);
                }
            }
        }


        private void AddToNewContextMenuItems(List<ContextMenuItem> items, string path, UnityAction onClick, int position)
        {
            while (path.IndexOf('/') > 0)
            {
                (string pathPartFirst, string pathPartSub) = SplitPath(path);

                ContextSubmenuItem newSubmenuItem = new ContextSubmenuItem(pathPartFirst);
                items.Add(newSubmenuItem);

                items = newSubmenuItem.items;
                path = pathPartSub;
            }

            ContextMenuTextItem newMenuTextItem = new ContextMenuTextItem(path, onClick);
            items.Add(newMenuTextItem);
        }

        private (string, string) SplitPath(string path)
        {
            string pathPartFirst = path.Substring(0, path.IndexOf('/')); //the first part of the path
            string pathPartSub = path.Substring(path.IndexOf('/') + 1); //the path except the first part

            return (pathPartFirst, pathPartSub);
        }
    }
}
