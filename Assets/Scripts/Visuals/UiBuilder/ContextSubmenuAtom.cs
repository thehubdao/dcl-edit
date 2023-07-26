using Assets.Scripts.EditorState;
using Assets.Scripts.System;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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
            public bool isDisabled { get; set; }

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
                    isDisabled.Equals(otherContextMenuText.isDisabled);
            }
        }

        protected Data data;

        // Dependencies
        ContextMenuSystem contextMenuSystem;

        [Inject]
        public void Construct(ContextMenuSystem contextMenuSystem)
        {
            this.contextMenuSystem = contextMenuSystem;
        }

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newContextMenuTextData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newContextMenuTextData.Equals(data))
            {
                // Update data
                var rect = gameObject.gameObject.GetComponent<RectTransform>();

                var text = gameObject.gameObject.GetComponentInChildren<TextMeshProUGUI>();
                text.text = newContextMenuTextData.title;
                
                if (newContextMenuTextData.isDisabled)
                {
                    var button = gameObject.gameObject.GetComponent<Button>();
                    button.interactable = false;
                }

                var hoverHandler = gameObject.gameObject.GetComponent<ContextMenuHoverHandler>();
                hoverHandler.OnHoverAction = () =>
                {
                    contextMenuSystem.CloseMenusUntil(newContextMenuTextData.menuId);

                    Vector3 rightExpandPosition = new Vector3(rect.position.x + newContextMenuTextData.menuWidth, rect.position.y, rect.position.z);
                    Vector3 leftExpandPosition = new Vector3(rect.position.x, rect.position.y, rect.position.z);
                    contextMenuSystem.OpenSubmenu(
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
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.ContextSubmenuItem);
            return atomObject;
        }


        public ContextSubmenuAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }

        public class Factory : PlaceholderFactory<UiBuilder, ContextSubmenuAtom> { }
    }

    public static class ContextSubmenuPanelHelper
    {
        public static ContextSubmenuAtom.Data AddContextSubmenu(
            this PanelAtom.Data panelAtomData,
            Guid menuId,
            Guid submenuId,
            string title,
            List<ContextMenuItem> submenuItems,
            float menuWidth,
            bool isDisabled)
        {
            var data = new ContextSubmenuAtom.Data
            {
                menuId = menuId,
                submenuId = submenuId,
                title = title,
                submenuItems = submenuItems,
                menuWidth = menuWidth,
                isDisabled = isDisabled
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}
