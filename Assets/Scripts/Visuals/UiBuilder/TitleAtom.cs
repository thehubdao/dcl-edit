namespace Assets.Scripts.Visuals.UiBuilder
{
    public class TitleAtom : TextAtom
    {
        public new class Data : TextAtom.Data
        {
        }

        protected override AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(UiBuilder.AtomType.Title);
            return atomObject;
        }

        public TitleAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class TitlePanelHelper
    {
        public static TitleAtom.Data AddTitle(
            this PanelAtom.Data panelAtomData,
            SetValueStrategy<string> title)
        {
            var data = new TitleAtom.Data
            {
                text = title
            };

            panelAtomData.childDates.Add(data);
            return data;
        }
    }
}