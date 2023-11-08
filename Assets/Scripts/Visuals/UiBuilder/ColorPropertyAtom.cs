using System;
using Assets.Scripts.Visuals.PropertyHandler;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class ColorPropertyAtom : Atom
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
            public Color currentContents;
            public UiPropertyActions<Color> actions;
            public UnityAction<GameObject> onClick;


            public override bool Equals(Atom.Data other)
            {
                if (!(other is ColorPropertyAtom.Data otherColorProperty))
                {
                    return false;
                }

                if (onClick != otherColorProperty.onClick)
                {
                    return false;
                }

                return
                    name.Equals(otherColorProperty.name) &&
                    placeholder.Equals(otherColorProperty.placeholder) &&
                    currentContents.Equals(otherColorProperty.currentContents) &&
                    actions.Equals(otherColorProperty.actions);
            }
        }

        protected Data data;

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newColorPropertyData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newColorPropertyData.Equals(data))
            {
                // Update data
                var colorPropertyHandler = gameObject.gameObject.GetComponent<ColorPropertyHandler>();

                colorPropertyHandler.ResetActions();

                colorPropertyHandler.propertyNameText.text = newColorPropertyData.name;
                colorPropertyHandler.stringInput.SetCurrentText(ColorUtility.ToHtmlStringRGBA(newColorPropertyData.currentContents));
                colorPropertyHandler.stringInput.SetPlaceHolder(newColorPropertyData.placeholder);
                colorPropertyHandler.colorPicker.button.onClick.RemoveAllListeners();
                colorPropertyHandler.colorPicker.button.onClick.AddListener(() => newColorPropertyData.onClick(colorPropertyHandler.colorPicker.button.gameObject));

                // setup actions
                colorPropertyHandler.colorImage.color = newColorPropertyData.currentContents;
                colorPropertyHandler.SetActions(newColorPropertyData.actions);
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.ColorPropertyInput);
            return atomObject;
        }


        public ColorPropertyAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class ColorPropertyPanelHelper
    {
        public static ColorPropertyAtom.Data AddColorProperty(this PanelAtom.Data panelAtomData, string name, string placeholder, Color currentContents, ColorPropertyAtom.UiPropertyActions<Color> actions, UnityAction<GameObject> onClick)
        {
            var data = new ColorPropertyAtom.Data
            {
                name = name,
                placeholder = placeholder,
                currentContents = currentContents,
                actions = actions,
                onClick = onClick
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}