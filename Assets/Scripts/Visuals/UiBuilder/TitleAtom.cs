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
            atomObject.height = 130;
            atomObject.position = -1;
            return atomObject;
        }

        public TitleAtom(UiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }

    public static class TitlePanelHelper
    {
        public static TitleAtom.Data AddTitle(this PanelAtom.Data panelAtomData, string title)
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