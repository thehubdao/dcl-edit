using System;
using JetBrains.Annotations;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class AssetButtonAtom : Atom
    {
        public new class Data : Atom.Data
        {
            public enum TypeIndicator
            {
                None,
                Model,
                Image,
                Scene
            }

            [NotNull]
            public SetValueStrategy<Guid> setValueStrategy;

            [NotNull]
            public ClickStrategy clickStrategy;

            [CanBeNull]
            public DragStrategy dragStrategy = null;

            [CanBeNull]
            public DropStrategy dropStrategy = null;

            public override bool Equals(Atom.Data other)
            {
                if (!(other is Data otherBtn))
                {
                    return false;
                }

                if (dragStrategy != otherBtn.dragStrategy) return false;
                if (clickStrategy != otherBtn.clickStrategy) return false;
                if (dragStrategy != otherBtn.dragStrategy) return false;
                if (dropStrategy != otherBtn.dropStrategy) return false;
                return true;
            }
        }

        protected Data data;

        public override void Update(Atom.Data newData)
        {
            UiBuilder.Stats.atomsUpdatedCount++;

            var newBtnData = (Data)newData;

            // Stage 1: Check for a GameObject and make one, if it doesn't exist
            if (gameObject == null)
            {
                // Make new game object
                gameObject = MakeNewGameObject();
            }

            // Stage 2: Check for updated data and update, if data was changed
            if (!newBtnData.Equals(data))
            {
                // Update data
                var btnHandler = gameObject.gameObject.GetComponent<AssetButtonHandler>();
                btnHandler.Setup(newBtnData.setValueStrategy, newBtnData.dragStrategy, newBtnData.dropStrategy, newBtnData.clickStrategy);

                data = newBtnData;
            }
        }

        protected virtual AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.AssetBrowserButton);
            return atomObject;
        }


        public AssetButtonAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class AssetButtonPanelHelper
    {
        public static AssetButtonAtom.Data AddAssetBrowserButton(
            this PanelAtom.Data panelAtomData,
            // Placeholder Thumbnail Handler
            [NotNull] SetValueStrategy<Guid> setValueStrategy,
            [CanBeNull] ClickStrategy clickStrategy = null,
            [CanBeNull] DragStrategy dragStrategy = null,
            [CanBeNull] DropStrategy dropStrategy = null)
        {
            clickStrategy ??= new ClickStrategy();

            var data = new AssetButtonAtom.Data
            {
                setValueStrategy = setValueStrategy,
                clickStrategy = clickStrategy,
                dragStrategy = dragStrategy,
                dropStrategy = dropStrategy
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}