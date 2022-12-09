using Assets.Scripts.Visuals.UiHandler;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class TextAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public string text;

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

        public override bool Update(Atom.Data newData, int newPosition)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var posHeightHasChanged = false;
            var newTextData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
                posHeightHasChanged = true;
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newTextData.Equals(data))
            {
                // Update data
                var textHandler = gameObject.gameObject.GetComponent<TextHandler>();
                textHandler.text = newTextData.text;
                data = newTextData;
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
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.Text);
            atomObject.height = 50;
            atomObject.position = -1;
            return atomObject;
        }


        public TextAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class TextPanelHelper
    {
        public static TextAtom.Data AddText(this PanelAtom.Data panelAtomData, string text)
        {
            var data = new TextAtom.Data
            {
                text = text
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}