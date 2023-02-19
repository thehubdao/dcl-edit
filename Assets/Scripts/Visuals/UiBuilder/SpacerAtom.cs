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
            [CanBeNull] public Action<Vector3> rightClickAction;
            [CanBeNull] public Action<GameObject> dropAction;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is SpacerAtom.Data otherSpacer))
                {
                    return false;
                }

                return
                    height.Equals(otherSpacer.height) &&
                    rightClickAction == otherSpacer.rightClickAction &&
                    dropAction == otherSpacer.dropAction;
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
                spacerHandler.rightClickHandler.onRightClick = newSpacerData.rightClickAction;
                spacerHandler.dropHandler.onDrop = newSpacerData.dropAction;
                spacerHandler.dropHandler.ResetHandler();
                
                if (newSpacerData.dropAction != null)
                {
                    spacerHandler.dropHandler.SetEnabled(true);
                }

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
        public static SpacerAtom.Data AddSpacer(this PanelAtom.Data panelAtomData, int height, Action<Vector3> rightClickAction = null, Action<GameObject> dropAction = null)
        {
            var data = new SpacerAtom.Data
            {
                height = height,
                rightClickAction = rightClickAction,
                dropAction = dropAction
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}
