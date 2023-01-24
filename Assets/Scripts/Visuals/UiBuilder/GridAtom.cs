using JetBrains.Annotations;

namespace Assets.Scripts.Visuals.UiBuilder
{
    public class GridAtom : PanelAtom
    {
        public new class Data : PanelAtom.Data { }

        protected override AtomGameObject MakeNewAtomGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.Grid);
            return atomObject;
        }

        protected override void MakeLayoutGroup(PanelAtom.Data newPanelData) { }

        public GridAtom(UiBuilder uiBuilder) : base(uiBuilder) { }
    }

    public static class GridPanelHelper
    {
        public static GridAtom.Data AddGrid(
            this PanelAtom.Data panelAtomData,
            int indentationLevel = 0,
            [CanBeNull] AtomDataList childDates = null)
        {
            var data = new GridAtom.Data
            {
                indentationLevel = indentationLevel,
                childDates = childDates ?? new AtomDataList()
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}