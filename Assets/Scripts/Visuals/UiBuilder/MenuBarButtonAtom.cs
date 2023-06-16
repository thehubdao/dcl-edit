using Assets.Scripts.Visuals.PropertyHandler;
using Assets.Scripts.Visuals.UiBuilder;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class MenuBarButtonAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public string title;
            public UnityAction<GameObject> onClick;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is MenuBarButtonAtom.Data otherMenuBarButton))
                {
                    return false;
                }

                return title.Equals(otherMenuBarButton.title);
            }
        }

        protected Data data;

        public MenuBarButtonAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newMenuBarButtonData = (Data)newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newMenuBarButtonData.Equals(data))
            {
                // Update data
                MenuBarButtonHandler menuBarButton = gameObject.gameObject.GetComponent<MenuBarButtonHandler>();
                menuBarButton.Initialize(newMenuBarButtonData.title, () => newMenuBarButtonData.onClick(gameObject.gameObject));
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.MenuBarButton);
            return atomObject;
        }
    }

    public static class MenuBarButtonPanelHelper
    {
        public static MenuBarButtonAtom.Data AddMenuBarButton(
            this PanelAtom.Data panelAtomData,
            string title,
            UnityAction<GameObject> onClick)
        {
            var data = new MenuBarButtonAtom.Data
            {
                title = title,
                onClick = onClick
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}
