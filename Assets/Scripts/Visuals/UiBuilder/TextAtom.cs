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

            public bool Equals(Data other)
            {
                if (other == null)
                {
                    return false;
                }

                return text == other.text;
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
                var textHandler = gameObject.gameObject.gameObject.GetComponent<TextHandler>();
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
            return new AtomGameObject
            {
                gameObject = uiBuilder.GetAtomObjectFromPool(NewUiBuilder.AtomType.Text),
                height = 50,
                position = -1
            };
        }


        public TextAtom(NewUiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }
}