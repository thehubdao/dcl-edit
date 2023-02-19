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

            [CanBeNull]
            public UnityAction onClose = null;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is PanelHeaderAtom.Data otherPanelHeader))
                {
                    return false;
                }

                return
                    title.Equals(otherPanelHeader.title) &&
                    onClose == otherPanelHeader.onClose;
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

                if (newHeaderData.onClose != null)
                {
                    headerHandler.CloseButtonContainer.SetActive(true);
                    headerHandler.CloseButton.onClick.RemoveAllListeners();
                    headerHandler.CloseButton.onClick.AddListener(newHeaderData.onClose);
                }
                else
                {
                    headerHandler.CloseButtonContainer.SetActive(false);
                    headerHandler.CloseButton.onClick.RemoveAllListeners();
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
        public static PanelHeaderAtom.Data AddPanelHeader(this PanelAtom.Data panelAtomData, string title, [CanBeNull] UnityAction onClose = null)
        {
            var data = new PanelHeaderAtom.Data
            {
                title = title,
                onClose = onClose
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}