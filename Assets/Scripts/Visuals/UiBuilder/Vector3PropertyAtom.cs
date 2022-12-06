using System;
using System.Collections.Generic;
using Assets.Scripts.Visuals.PropertyHandler;
using UnityEngine;

namespace Assets.Scripts.Visuals.NewUiBuilder
{
    public class Vector3PropertyAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public string name;
            public List<string> placeholders;
            public Vector3 currentContents;
            public StringPropertyAtom.UiPropertyActions<Vector3> actions;


            public override bool Equals(Atom.Data other)
            {
                if (!(other is Vector3PropertyAtom.Data otherVector3Property))
                {
                    return false;
                }

                return
                    name.Equals(otherVector3Property.name) &&
                    placeholders.Equals(otherVector3Property.placeholders) &&
                    currentContents.Equals(otherVector3Property.currentContents) &&
                    actions.Equals(otherVector3Property.actions);
            }
        }

        protected Data data;

        public override bool Update(Atom.Data newData, int newPosition)
        {
            NewUiBuilder.Stats.atomsUpdatedCount++;

            var posHeightHasChanged = false;
            var newVector3PropertyData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
                posHeightHasChanged = true;
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newVector3PropertyData.Equals(data))
            {
                // Update data
                var vector3PropertyHandler = gameObject.gameObject.GetComponent<Vector3PropertyHandler>();

                vector3PropertyHandler.propertyNameText.text = newVector3PropertyData.name;

                vector3PropertyHandler.numberInputX.SetCurrentNumber(newVector3PropertyData.currentContents.x);
                vector3PropertyHandler.numberInputX.TextInputHandler.SetPlaceHolder(newVector3PropertyData.placeholders[0]);

                vector3PropertyHandler.numberInputY.SetCurrentNumber(newVector3PropertyData.currentContents.y);
                vector3PropertyHandler.numberInputY.TextInputHandler.SetPlaceHolder(newVector3PropertyData.placeholders[1]);

                vector3PropertyHandler.numberInputZ.SetCurrentNumber(newVector3PropertyData.currentContents.z);
                vector3PropertyHandler.numberInputZ.TextInputHandler.SetPlaceHolder(newVector3PropertyData.placeholders[2]);

                // setup actions
                vector3PropertyHandler.SetActions(newVector3PropertyData.actions);
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
            var atomObject = uiBuilder.GetAtomObjectFromPool(NewUiBuilder.AtomType.Vector3PropertyInput);
            atomObject.height = 50;
            atomObject.position = -1;
            return atomObject;
        }


        public Vector3PropertyAtom(NewUiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class Vector3PropertyPanelHelper
    {
        public static Vector3PropertyAtom.Data AddVector3Property(this PanelAtom.Data panelAtomData, string name, List<string> placeholders, Vector3 currentContents, StringPropertyAtom.UiPropertyActions<Vector3> actions)
        {
            var data = new Vector3PropertyAtom.Data
            {
                name = name,
                placeholders = placeholders,
                currentContents = currentContents,
                actions = actions
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}