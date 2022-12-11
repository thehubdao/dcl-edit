using System;
using Assets.Scripts.Visuals.PropertyHandler;
using JetBrains.Annotations;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class StringPropertyAtom : Atom
    {
        public struct UiPropertyActions<T>
        {
            [CanBeNull]
            public Action<T> OnChange;

            [CanBeNull]
            public Action OnInvalid;

            [CanBeNull]
            public Action<T> OnSubmit;

            [CanBeNull]
            public Action<T> OnAbort;

            public bool Equals(UiPropertyActions<T> other)
            {
                return
                    OnChange == other.OnChange &&
                    OnInvalid == other.OnInvalid &&
                    OnSubmit == other.OnSubmit &&
                    OnAbort == other.OnAbort;
            }
        }

        public new class Data : Atom.Data
        {
            public string name;
            public string placeholder;
            public string currentContents;
            public UiPropertyActions<string> actions;


            public override bool Equals(Atom.Data other)
            {
                if (!(other is StringPropertyAtom.Data otherStringProperty))
                {
                    return false;
                }

                return
                    name.Equals(otherStringProperty.name) &&
                    placeholder.Equals(otherStringProperty.placeholder) &&
                    currentContents.Equals(otherStringProperty.currentContents) &&
                    actions.Equals(otherStringProperty.actions);
            }
        }

        protected Data data;

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newStringPropertyData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newStringPropertyData.Equals(data))
            {
                // Update data
                var stringPropertyHandler = gameObject.gameObject.GetComponent<StringPropertyHandler>();

                stringPropertyHandler.ResetActions();

                stringPropertyHandler.propertyNameText.text = newStringPropertyData.name;
                stringPropertyHandler.stringInput.SetCurrentText(newStringPropertyData.currentContents);
                stringPropertyHandler.stringInput.SetPlaceHolder(newStringPropertyData.placeholder);

                // setup actions
                stringPropertyHandler.SetActions(newStringPropertyData.actions);
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.StringPropertyInput);
            return atomObject;
        }


        public StringPropertyAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class StringPropertyPanelHelper
    {
        public static StringPropertyAtom.Data AddStringProperty(this PanelAtom.Data panelAtomData, string name, string placeholder, string currentContents, StringPropertyAtom.UiPropertyActions<string> actions)
        {
            var data = new StringPropertyAtom.Data
            {
                name = name,
                placeholder = placeholder,
                currentContents = currentContents,
                actions = actions
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}