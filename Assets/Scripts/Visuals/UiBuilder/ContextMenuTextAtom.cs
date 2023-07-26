using Assets.Scripts.System;
using System;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;
using Zenject;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class ContextMenuTextAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public Guid menuId;
            public string title;
            public UnityAction onClick;
            public bool isDisabled;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is ContextMenuTextAtom.Data otherContextMenuText))
                {
                    return false;
                }

                return
                    menuId.Equals(otherContextMenuText.menuId) &&
                    title.Equals(otherContextMenuText.title) &&
                    onClick.Equals(otherContextMenuText.onClick) &&
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

                var text = gameObject.gameObject.GetComponentInChildren<TextMeshProUGUI>();
                text.text = newContextMenuTextData.title;

                var button = gameObject.gameObject.GetComponent<Button>();
                button.onClick.AddListener(newContextMenuTextData.onClick);
                button.onClick.AddListener(contextMenuSystem.CloseMenu);
                if (newContextMenuTextData.isDisabled) button.interactable = false;

                var hoverHandler = gameObject.gameObject.GetComponent<ContextMenuHoverHandler>();
                hoverHandler.OnHoverAction = () => contextMenuSystem.CloseMenusUntil(newContextMenuTextData.menuId);

                data = newContextMenuTextData;
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.ContextMenuItem);
            return atomObject;
        }


        public ContextMenuTextAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }

        public class Factory : PlaceholderFactory<UiBuilder, ContextMenuTextAtom> { }
    }

    public static class ContextMenuTextPanelHelper
    {
        public static ContextMenuTextAtom.Data AddContextMenuText(this PanelAtom.Data panelAtomData, Guid menuId, string title, UnityAction onClick, bool isDisabled)
        {
            var data = new ContextMenuTextAtom.Data
            {
                menuId = menuId,
                title = title,
                onClick = onClick,
                isDisabled = isDisabled
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}