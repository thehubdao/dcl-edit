using System;
using System.Collections.Generic;

namespace Assets.Scripts.Visuals.NewUiBuilder
{
    public class PanelWithBorderAtom : PanelAtom
    {
        public new class Data : PanelAtom.Data
        {
        }

        protected override AtomGameObject MakeNewAtomGameObject()
        {
            var atomObject = uiBuilder.GetAtomObjectFromPool(NewUiBuilder.AtomType.PanelWithBorder);
            atomObject.height = 40;
            atomObject.position = -1;
            return atomObject;
        }

        protected override int totalBoarderHeight { get; set; } = 40;

        public PanelWithBorderAtom(NewUiBuilder uiBuilder) : base(uiBuilder)
        {
        }
    }
}