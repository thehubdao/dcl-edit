using JetBrains.Annotations;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class PanelWithBorderAtom : PanelAtom
    {
        public new class Data : PanelAtom.Data
        {
        }

        protected override AtomGameObject MakeNewAtomGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.PanelWithBorder);
            return atomObject;
        }

        protected override int totalBorderHeight { get; set; } = 40;

        public PanelWithBorderAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class PanelWithBorderPanelHelper
    {
        public static PanelWithBorderAtom.Data AddPanelWithBorder(this PanelAtom.Data panelAtomData, [CanBeNull] AtomDataList childDates = null)
        {
            var data = new PanelWithBorderAtom.Data
            {
                childDates = childDates ?? new AtomDataList()
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}