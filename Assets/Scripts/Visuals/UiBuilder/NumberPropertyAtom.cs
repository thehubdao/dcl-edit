using Assets.Scripts.Visuals.PropertyHandler;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class NumberPropertyAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public string name;
            public string placeholder;
            public ValueBindStrategy<float> valueBindStrategy;


            public override bool Equals(Atom.Data other)
            {
                if (!(other is NumberPropertyAtom.Data otherNumberProperty))
                {
                    return false;
                }


                return
                    name.Equals(otherNumberProperty.name) &&
                    placeholder.Equals(otherNumberProperty.placeholder) &&
                    valueBindStrategy.Equals(otherNumberProperty.valueBindStrategy);
            }
        }

        protected Data data;

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newNumberPropertyData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newNumberPropertyData.Equals(data))
            {
                // Update data
                var numberPropertyHandler = gameObject.gameObject.GetComponent<NumberPropertyHandler>();

                numberPropertyHandler.ResetActions();

                numberPropertyHandler.propertyNameText.text = newNumberPropertyData.name;
                numberPropertyHandler.numberInput.TextInputHandler.SetPlaceHolder(newNumberPropertyData.placeholder);

                // setup actions
                numberPropertyHandler.Setup(newNumberPropertyData.valueBindStrategy);
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.NumberPropertyInput);
            return atomObject;
        }


        public NumberPropertyAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class NumberPropertyPanelHelper
    {
        public static NumberPropertyAtom.Data AddNumberProperty(this PanelAtom.Data panelAtomData, string name, string placeholder, ValueBindStrategy<float> valueBindStrategy)
        {
            var data = new NumberPropertyAtom.Data
            {
                name = name,
                placeholder = placeholder,
                valueBindStrategy = valueBindStrategy
            };

            panelAtomData.childDates.Add(data);
            return data;
        }

        public static NumberPropertyAtom.Data AddNumberProperty(this PanelAtom.Data panelAtomData, string name, string placeholder, ValueBindStrategy<int> valueBindStrategy)
        {
            var data = new NumberPropertyAtom.Data
            {
                name = name,
                placeholder = placeholder,
                valueBindStrategy = new ValueBindStrategy<float>(
                    () => valueBindStrategy.value(),
                    onValueSubmitted: value => valueBindStrategy.onValueSubmitted?.Invoke((int) value),
                    onValueChanged: value => valueBindStrategy.onValueChanged?.Invoke((int) value),
                    onErrorSubmitted: valueBindStrategy.onErrorSubmitted,
                    onErrorChanged: valueBindStrategy.onErrorChanged)
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}