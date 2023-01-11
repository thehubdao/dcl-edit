using UnityEngine.UI;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class SpacerAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public int height;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is SpacerAtom.Data otherSpacer))
                {
                    return false;
                }

                return height == otherSpacer.height;
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
                var le = gameObject.gameObject.GetComponent<LayoutElement>();
                le.minHeight = newSpacerData.height;

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
        public static SpacerAtom.Data AddSpacer(this PanelAtom.Data panelAtomData, int height)
        {
            var data = new SpacerAtom.Data
            {
                height = height
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}