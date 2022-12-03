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

        private Data data;

        public override bool Update(Atom.Data newData, int newPosition)
        {
            NewUiBuilder.Stats.atomsUpdatedCount++;

            var hasChanged = false;
            var newTextData = (Data) newData;

            if (gameObject == null)
            {
                // Make new game object
                gameObject = new AtomGameObject
                {
                    gameObject = uiBuilder.GetAtomObjectFromPool(NewUiBuilder.AtomType.Text),
                    height = 50,
                    position = -1
                };

                hasChanged = true;
            }

            if (!newTextData.Equals(data))
            {
                // Update data
                var textHandler = gameObject.gameObject.gameObject.GetComponent<TextHandler>();
                textHandler.text = newTextData.text;
                data = newTextData;
            }

            if (newPosition != gameObject.position /* or height has changed */)
            {
                // Update position and size
                var tf = gameObject.gameObject.gameObject.GetComponent<RectTransform>();

                tf.offsetMin = Vector2.zero;
                tf.offsetMax = Vector2.zero;

                tf.anchoredPosition = new Vector3(0, -newPosition, 0);
                tf.sizeDelta = new Vector2(tf.sizeDelta.x, gameObject.height);

                hasChanged = true;
            }

            return hasChanged;
        }


        public TextAtom(NewUiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }
}