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
        public List<MenuBarItem> menuItems = new List<MenuBarItem>();

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

        public void AddMenuItem(string path, UnityAction onClick)
        {
            // Check for null action.
            if (onClick == null)
            {
                throw new System.ArgumentException($"Null not a valid onClick argument.");
            }

            Path pathParsed = new Path(path);

            // Check for root directory.
            if (pathParsed.Depth <= 1)
            {
                throw new System.ArgumentException($"Cannot add actions to the root of the menu bar. Create at least one directory. \"{path}\".");
            }

            MenuBarItem itemToAddTo = GetOrCreateMenuBarItem(pathParsed.FirstTitle);
            AddToExistingContextMenuItems(itemToAddTo.subItems, pathParsed.FirstElementRemoved(), onClick);
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


        private void AddToExistingContextMenuItems(List<ContextMenuItem> items, Path path, UnityAction onClick)
        {
            if (path.Depth == 1)
            {
                AddToNewContextMenuItems(items, path, onClick);
            }
            else
            {
                if (items == null)
                {
                    Debug.Log(path);
                }

                ContextSubmenuItem foundItem = (ContextSubmenuItem)items.SingleOrDefault(item => (item as ContextSubmenuItem)?.title == path.FirstTitle);

                if (foundItem != null)
                {
                    // recursive
                    AddToExistingContextMenuItems(foundItem.items, path.FirstElementRemoved(), onClick);
                }
                else
                {
                    // switch to adding items
                    AddToNewContextMenuItems(items, path, onClick);
                }
            }
        }


        private void AddToNewContextMenuItems(List<ContextMenuItem> items, Path path, UnityAction onClick)
        {
            while (path.Depth > 1)
            {
                ContextSubmenuItem newSubmenuItem = new ContextSubmenuItem(path.FirstTitle);
                items.Add(newSubmenuItem);

                items = newSubmenuItem.items;
                path.RemoveFirstElement();
            }

            ContextMenuTextItem newMenuTextItem = new ContextMenuTextItem(path.FirstTitle, onClick);

            int position = path.FirstPositionIndex;
            if (position < 0 || position > items.Count)
            {
                position = items.Count;
            }
            items.Insert(position, newMenuTextItem);
        }

        /// <summary>
        /// Used to extract atomic informations from paths for th emenu bar.
        /// </summary>
        private class Path
        {
            private int firstElement; //used to skip elements, by just incrementing. Starting with 0.
            private string[] titles;
            private int[] positionIndices;

            /// <summary>
            /// The title of the first element of the path without its position index.
            /// </summary>
            public string FirstTitle { get => titles[firstElement]; }

            /// <summary>
            /// The position index of the first element of the path.
            /// </summary>
            public int FirstPositionIndex { get => positionIndices[firstElement]; } //the position index of the first element of the path

            /// <summary>
            /// The number of elements in th remaining path.
            /// </summary>
            public int Depth { get => titles.Length - firstElement; }


            private Path(Path path)
            {
                firstElement = path.firstElement;
                titles = path.titles;
                positionIndices = path.positionIndices;
            }

            /// <summary>
            /// Contert a string path to at Path object.
            /// </summary>
            /// <param name="path"></param>
            public Path(string path)
            {
                firstElement = 0;
                titles = path.Split('/');

                // Check for empty titles.
                foreach (string title in titles)
                {
                    if (title == "")
                    {
                        throw new System.ArgumentException($"Elements are not allowed to have no title: \"{path}\".");
                    }
                }

                positionIndices = new int[titles.Length];

                for (int i = 0; i < titles.Length; i++)
                {
                    int splitterIndex = titles[i].IndexOf('#');
                    if (splitterIndex > 0)
                    {
                        try
                        {
                            positionIndices[i] = int.Parse(titles[i].Substring(splitterIndex + 1));
                        }
                        catch
                        {
                            throw new System.ArgumentException($"The position index \"{titles[i].Substring(splitterIndex)}\" is not valid. From the Path: \"{path}\".");
                        }
                        titles[i] = titles[i].Substring(0, splitterIndex);
                    }
                    else
                    {
                        positionIndices[i] = -1;
                    }
                }
            }

            /// <summary>
            /// Remove the first element from the Path to use the rest of the path.
            /// </summary>
            /// <returns>This with all other elements.</returns>
            /// <exception cref="System.Exception"></exception>
            public Path RemoveFirstElement()
            {
                if (Depth <= 1)
                {
                    throw new System.Exception($"Cannot Remove any more elements the Path \"{ToString()}\".");
                }
                firstElement ++;

                return this;
            }

            /// <summary>
            /// Remove the first element from the Path to use the rest of the path.
            /// </summary>
            /// <returns>New Path object with all other elements.</returns>
            /// <exception cref="System.Exception"></exception>
            public Path FirstElementRemoved()
            {
                return new Path(this).RemoveFirstElement();
            }

            /// <summary>
            /// Convert back to a string Path. Used for logging.
            /// </summary>
            /// <returns>The path as string.</returns>
            public override string ToString()
            {
                string str = "";

                for (int i = firstElement; i < titles.Length - 1; i++)
                {
                    str += titles[i];
                    if (positionIndices[i] >= 0)
                    {
                        str += "#" + positionIndices[i];
                    }
                    str += "/";
                }

                return str;
            }
        }
    }
}
