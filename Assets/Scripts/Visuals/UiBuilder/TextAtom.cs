using Assets.Scripts.Visuals.UiHandler;
using TMPro;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class TextAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public string text;
            public TextAlignmentOptions textAlignment = TextAlignmentOptions.Center;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is TextAtom.Data otherText))
                {
                    return false;
                }

                return text == otherText.text;
            }
        }

        protected Data data;
        
        private void UpdateLayout(Data newData)
        {
            gameObject.gameObject.GetComponent<TextMeshProUGUI>().alignment = newData.textAlignment;
        }

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newTextData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newTextData.Equals(data))
            {
                // Update data
                UpdateLayout(newTextData);
                var textHandler = gameObject.gameObject.GetComponent<TextHandler>();
                textHandler.text = newTextData.text;
                data = newTextData;
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.Text);
            return atomObject;
        }


        public TextAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class TextPanelHelper
    {
        public static TextAtom.Data AddText(this PanelAtom.Data panelAtomData, string text, TextAlignmentOptions textAlignment = TextAlignmentOptions.Center)
        {
            var data = new TextAtom.Data
            {
                text = text,
                textAlignment = textAlignment
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}