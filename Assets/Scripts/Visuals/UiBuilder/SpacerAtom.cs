using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using Visuals.UiHandler;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class SpacerAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public int height;

            [CanBeNull]
            public RightClickStrategy rightClickStrategy;

            [CanBeNull]
            public DropStrategy dropStrategy;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is SpacerAtom.Data otherSpacer))
                {
                    return false;
                }

                return
                    height.Equals(otherSpacer.height) &&
                    rightClickStrategy == otherSpacer.rightClickStrategy &&
                    dropStrategy == otherSpacer.dropStrategy;
            }
        }

        protected Data data;

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newSpacerData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newSpacerData.Equals(data))
            {
                // Update data
                var spacerHandler = gameObject.gameObject.GetComponent<SpacerHandler>();
                var le = gameObject.gameObject.GetComponent<LayoutElement>();
                
                le.minHeight = newSpacerData.height;
                spacerHandler.clickHandler.rightClickStrategy = newSpacerData.rightClickStrategy;
                spacerHandler.dropHandler.dropStrategy = newSpacerData.dropStrategy;

                data = newSpacerData;
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.Spacer);
            return atomObject;
        }


        public SpacerAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class SpacerPanelHelper
    {
        public static SpacerAtom.Data AddSpacer(this PanelAtom.Data panelAtomData, int height, RightClickStrategy rightClickStrategy = null, DropStrategy dropStrategy = null)
        {
            var data = new SpacerAtom.Data
            {
                height = height,
                rightClickStrategy = rightClickStrategy,
                dropStrategy = dropStrategy
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}
