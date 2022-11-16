using System;
using System.Collections.Generic;
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
            public Direction expandDirection;               // Which direction should the menu expand to when placed at this position.
            public Vector3 position;
        }
    }

    public abstract class ContextMenuItem { }
    public class ContextSubmenuItem : ContextMenuItem
    {
        public Guid submenuId;
        public string title;
        public List<ContextMenuItem> items;

        public ContextSubmenuItem(string title, List<ContextMenuItem> subItems)
        {
            this.submenuId = Guid.NewGuid();
            this.title = title;
            this.items = subItems;
        }
    }
    public class ContextMenuTextItem : ContextMenuItem
    {
        public string title;
        public UnityAction onClick;
        public bool isDisabled;

        public ContextMenuTextItem(string title, UnityAction onClick, bool isDisabled = false)
        {
            this.title = title;
            this.onClick = onClick;
            this.isDisabled = isDisabled;
        }
    }
    public class ContextMenuSpacerItem : ContextMenuItem { }
}