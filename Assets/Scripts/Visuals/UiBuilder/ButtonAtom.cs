using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class ButtonAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public string text;
            public UnityAction<GameObject> onClick;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is ButtonAtom.Data otherBtn))
                {
                    return false;
                }

                if (onClick != otherBtn.onClick)
                {
                    return false;
                }

                return text == otherBtn.text;
            }
        }

        protected Data data;

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newBtnData = (Data)newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newBtnData.Equals(data))
            {
                // Update data
                var btnHandler = gameObject.gameObject.GetComponent<ButtonHandler>();
                btnHandler.text.text = newBtnData.text;
                btnHandler.button.onClick.RemoveAllListeners();
                btnHandler.button.onClick.AddListener(() => newBtnData.onClick(btnHandler.button.gameObject));
                data = newBtnData;
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.Button);
            return atomObject;
        }


        public ButtonAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class ButtonPanelHelper
    {
        // The GameObject parameter of the onClick action gives access to the button UI game object.
        public static ButtonAtom.Data AddButton(this PanelAtom.Data panelAtomData, string text, UnityAction<GameObject> onClick)
        {
            var data = new ButtonAtom.Data
            {
                text = text,
                onClick = onClick
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}