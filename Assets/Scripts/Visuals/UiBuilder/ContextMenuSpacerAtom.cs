using Assets.Scripts.System;
using System;
using Zenject;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class ContextMenuSpacerAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public Guid menuId;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is ContextMenuSpacerAtom.Data otherContextMenuText))
                {
                    return false;
                }

                return
                    menuId.Equals(otherContextMenuText.menuId);
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
                hoverHandler.OnHoverAction = () => contextMenuSystem.CloseMenusUntil(newContextMenuSpacerData.menuId);

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

        public class Factory : PlaceholderFactory<UiBuilder, ContextMenuSpacerAtom> { }
    }

    public static class ContextMenuSpacerPanelHelper
    {
        public static ContextMenuSpacerAtom.Data AddContextMenuSpacer(this PanelAtom.Data panelAtomData, Guid menuId)
        {
            var data = new ContextMenuSpacerAtom.Data
            {
                menuId = menuId
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}