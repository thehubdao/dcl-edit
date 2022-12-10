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

        public override bool Update(Atom.Data newData, int newPosition)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var posHeightHasChanged = false;
            var newSpacerData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
                posHeightHasChanged = true;
            }

            // Stage 2: Check for updated data and update, if data was changed
            //if (!newSpacerData.Equals(data))
            //{
            //    // No data to update
            //}

            // Stage 3: Check for changes in Position and Height and update, if it has changed
            if (newPosition != gameObject.position || !newSpacerData.Equals(data))
            {
                data = newSpacerData;
                UpdatePositionAndSize(newPosition, newSpacerData.height);
                posHeightHasChanged = true;
            }

            return posHeightHasChanged;
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.Spacer);
            atomObject.height = 50;
            atomObject.position = -1;
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