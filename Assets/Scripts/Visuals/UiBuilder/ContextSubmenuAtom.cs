using System;
using System.Collections.Generic;
using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class ContextSubmenuAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public Guid menuId;
            public Guid submenuId;
            public string title;
            public List<ContextMenuItem> submenuItems;
            public float menuWidth;
            public ContextMenuSystem contextMenuSystem;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is ContextSubmenuAtom.Data otherContextMenuText))
                {
                    return false;
                }

                return
                    menuId.Equals(otherContextMenuText.menuId) &&
                    submenuId.Equals(otherContextMenuText.submenuId) &&
                    title.Equals(otherContextMenuText.title) &&
                    submenuItems.Equals(otherContextMenuText.submenuItems) &&
                    menuWidth.Equals(otherContextMenuText.menuWidth) &&
                    contextMenuSystem.Equals(otherContextMenuText.contextMenuSystem);
            }
        }

        protected Data data;

        public override bool Update(Atom.Data newData, int newPosition)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var posHeightHasChanged = false;
            var newContextMenuTextData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
                posHeightHasChanged = true;
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newContextMenuTextData.Equals(data))
            {
                // Update data
                var rect = gameObject.gameObject.GetComponent<RectTransform>();

                var text = gameObject.gameObject.GetComponentInChildren<TextMeshProUGUI>();
                text.text = newContextMenuTextData.title;

                var hoverHandler = gameObject.gameObject.GetComponent<ContextMenuHoverHandler>();
                hoverHandler.OnHoverAction = () =>
                {
                    newContextMenuTextData.contextMenuSystem.CloseMenusUntil(newContextMenuTextData.menuId);

                    Vector3 rightExpandPosition = new Vector3(rect.position.x + newContextMenuTextData.menuWidth, rect.position.y, rect.position.z);
                    Vector3 leftExpandPosition = new Vector3(rect.position.x, rect.position.y, rect.position.z);
                    newContextMenuTextData.contextMenuSystem.OpenSubmenu(
                        newContextMenuTextData.submenuId,
                        new List<ContextMenuState.Placement>
                        {
                            new ContextMenuState.Placement {position = rightExpandPosition, expandDirection = ContextMenuState.Placement.Direction.Right},
                            new ContextMenuState.Placement {position = leftExpandPosition, expandDirection = ContextMenuState.Placement.Direction.Left}
                        },
                        newContextMenuTextData.submenuItems
                    );
                };
                data = newContextMenuTextData;
            }

            // Stage 3: Check for changes in Position and Height and update, if it has changed
            if (newPosition != gameObject.position)
            {
                UpdatePositionAndSize(newPosition, gameObject.height);
                posHeightHasChanged = true;
            }

            return posHeightHasChanged;
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.ContextSubmenuItem);
            atomObject.height = 30;
            atomObject.position = -1;
            return atomObject;
        }


        public ContextSubmenuAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class ContextSubmenuPanelHelper
    {
        public static ContextSubmenuAtom.Data AddContextSubmenu(this PanelAtom.Data panelAtomData, Guid menuId, Guid submenuId, string title, List<ContextMenuItem> submenuItems, float menuWidth, ContextMenuSystem contextMenuSystem)
        {
            var data = new ContextSubmenuAtom.Data
            {
                menuId = menuId,
                submenuId = submenuId,
                title = title,
                submenuItems = submenuItems,
                menuWidth = menuWidth,
                contextMenuSystem = contextMenuSystem
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}