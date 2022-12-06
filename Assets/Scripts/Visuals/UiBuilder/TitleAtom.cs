namespace Assets.Scripts.Visuals.NewUiBuilder
{
    public class TitleAtom : TextAtom
    {
        public new class Data : TextAtom.Data
        {
        }

        protected override AtomGameObject MakeNewGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(NewUiBuilder.AtomType.Title);
            atomObject.height = 130;
            atomObject.position = -1;
            return atomObject;
        }

        public TitleAtom(NewUiBuilder uiBuilder) : base(uiBuilder)
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