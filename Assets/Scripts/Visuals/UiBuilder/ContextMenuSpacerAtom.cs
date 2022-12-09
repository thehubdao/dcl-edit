using System;
using Assets.Scripts.System;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class ContextMenuSpacerAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public Guid menuId;
            public ContextMenuSystem contextMenuSystem;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is ContextMenuSpacerAtom.Data otherContextMenuText))
                {
                    return false;
                }

                return
                    menuId.Equals(otherContextMenuText.menuId) &&
                    contextMenuSystem.Equals(otherContextMenuText.contextMenuSystem);
            }
        }

        protected Data data;

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newContextMenuSpacerData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newContextMenuSpacerData.Equals(data))
            {
                // Update data
                var hoverHandler = gameObject.gameObject.GetComponent<ContextMenuHoverHandler>();
                hoverHandler.OnHoverAction = () => newContextMenuSpacerData.contextMenuSystem.CloseMenusUntil(newContextMenuSpacerData.menuId);

                data = newContextMenuSpacerData;
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.ContextMenuSpacerItem);
            return atomObject;
        }


        public ContextMenuSpacerAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class ContextMenuSpacerPanelHelper
    {
        public static ContextMenuSpacerAtom.Data AddContextMenuSpacer(this PanelAtom.Data panelAtomData, Guid menuId, ContextMenuSystem contextMenuSystem)
        {
            var data = new ContextMenuSpacerAtom.Data
            {
                menuId = menuId,
                contextMenuSystem = contextMenuSystem
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}