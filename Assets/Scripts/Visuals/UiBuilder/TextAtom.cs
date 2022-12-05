using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Visuals.NewUiBuilder;
using Assets.Scripts.Visuals.UiHandler;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Scripts.Visuals.NewUiBuilder
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
            NewUiBuilder.Stats.atomsUpdatedCount++;

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
            var atomObject = uiBuilder.GetAtomObjectFromPool(NewUiBuilder.AtomType.Text);
            atomObject.height = 50;
            atomObject.position = -1;
            return atomObject;
        }


        public TextAtom(NewUiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }
}