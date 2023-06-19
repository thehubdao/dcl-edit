using Assets.Scripts.Visuals.UiHandler;
using JetBrains.Annotations;
using UnityEngine.Events;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class PanelHeaderAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public string title;

            public bool showCloseButton = false;

            [NotNull]
            public ClickStrategy clickCloseStrategy = null;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is PanelHeaderAtom.Data otherPanelHeader))
                {
                    return false;
                }

                return
                    title.Equals(otherPanelHeader.title) &&
                    clickCloseStrategy == otherPanelHeader.clickCloseStrategy;
            }
        }

        protected Data data;

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newHeaderData = (Data) newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newHeaderData.Equals(data))
            {
                // Update data
                var headerHandler = gameObject.gameObject.GetComponent<PanelHeaderHandler>();
                headerHandler.Title.text = newHeaderData.title;

                if (newHeaderData.showCloseButton)
                {
                    headerHandler.CloseButtonContainer.SetActive(true);
                    headerHandler.CloseButton.clickStrategy = newHeaderData.clickCloseStrategy;
                }
                else
                {
                    headerHandler.CloseButtonContainer.SetActive(false);
                    headerHandler.CloseButton.clickStrategy = new ClickStrategy();
                }
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.PanelHeader);
            return atomObject;
        }


        public PanelHeaderAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class PanelHeaderPanelHelper
    {
        public static PanelHeaderAtom.Data AddPanelHeader(
            this PanelAtom.Data panelAtomData,
            string title,
            [CanBeNull] ClickStrategy clickCloseStrategy = null)
        {
            var data = new PanelHeaderAtom.Data
            {
                title = title,
                showCloseButton = clickCloseStrategy != null,
                clickCloseStrategy = clickCloseStrategy ?? new ClickStrategy()
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}