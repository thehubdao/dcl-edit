using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.EditorState
{
    public class ContextMenuState
    {
        public Stack<Data> menuData = new Stack<Data>();
        public Dictionary<Guid, GameObject> menuGameObjects = new Dictionary<Guid, GameObject>();

        public struct Data
        {
            public Guid menuId;
            public List<ContextMenuItem> items;
            public List<Placement> possiblePlacements;

            public Data(Guid menuId, List<ContextMenuItem> items, List<Placement> possiblePlacements)
            {
                this.menuId = menuId;
                this.items = items;
                this.possiblePlacements = possiblePlacements;
            }
        }
        public struct Placement
        {
            public enum Direction
            {
                Any,
                Left,
                Right
            }
            public Direction expandDirection; // Which direction should the menu expand to when placed at this position.
            public Vector3 position;
        }

        /// <summary>
        /// Sort all the items in the context menu and all sub menues by their sortingPriotiry.
        /// The Sort is stable, so elements with the same sorting priority stay in order.
        /// </summary>
        /// <param name="items">The context menu to sort.</param>
        public static void SortItems(List<ContextMenuItem> items)
        {
            // use System.Linq.OrderBy sorting because it is stable unlike the System.Collections.Generic.Sort.
            List<ContextMenuItem>  itemsSorted = items.OrderBy(item => item.sortingPriotiry).ToList();
            // inset sorted items into the original list, so the reference to the list does not change.
            items.Clear();
            items.AddRange(itemsSorted);

            // sort sub menu recursively
            foreach (var item in items)
            {
                if (item is ContextSubmenuItem)
                {
                    SortItems(((ContextSubmenuItem)item).items);
                }
            }
        }
    }

    public abstract class ContextMenuItem
    {
        public int sortingPriotiry = 0;

        public ContextMenuItem(int sortingPriority = 0)
        {
            this.sortingPriotiry = sortingPriority;
        }
    }

    public class ContextSubmenuItem : ContextMenuItem
    {
        public Guid submenuId;
        public string title;
        public List<ContextMenuItem> items;

        public ContextSubmenuItem(string title, List<ContextMenuItem> subItems = null, int sortingPriority = 0) : base(sortingPriority)
        {
            this.submenuId = Guid.NewGuid();
            this.title = title;
            if (subItems != null)
            {
                this.items = subItems;
            }
            else
            {
                items = new List<ContextMenuItem>();
            }
        }
    }
    public class ContextMenuTextItem : ContextMenuItem
    {
        public string title;
        public UnityAction onClick;
        public bool isDisabled;

        public ContextMenuTextItem(string title, UnityAction onClick, bool isDisabled = false, int sortingPriority = 0) : base(sortingPriority)
        {
            this.title = title;
            this.onClick = onClick;
            this.isDisabled = isDisabled;
        }
    }
    public class ContextMenuSpacerItem : ContextMenuItem
    {
        public ContextMenuSpacerItem(int sortingPriority = 0) : base(sortingPriority)
        { }
    }
}